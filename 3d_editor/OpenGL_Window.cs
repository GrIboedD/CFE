using _3d_editor.Geometric_figures;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.ComponentModel;
using System.Data;


namespace _3d_editor
{
    [ToolboxItem(true)]
    public partial class OpenGL_Window: GLControl
    {   

        private bool RayCastingObjectEnable = true;

        private bool CoordGridEnable = true;

        private bool LeftMouseMoveEnable = false;

        private bool RayCastingGridEnable = false;

        const float keySpeed = 0.005f;
        const float mouseRotateSensitivity = 0.007f;
        const float mouseMoveSensitivity = 0.007f;
        const float zoomFactorSensitivity = 0.5f;

        private const string vertexPathSphere = "Shaders/sphere.vert";
        private const string fragmentPathSphere = "Shaders/sphere.frag";

        private const string vertexPathCoordinateGrid = "Shaders/coordinateGrid.vert";
        private const string fragmentPathCoordinateGrid = "Shaders/coordinateGrid.frag";

        private const string vertexPathCylinders = "Shaders/cylinder.vert";
        private const string fragmentPathCylinders = "Shaders/cylinder.frag";

        private readonly Camera Camera = new();

        private Matrix4 projectionMatrix;

        private Spheres Spheres;

        private Cylinders Cylinders;

        private CoordinateGrid CoordinateGrid;

        private DateTime lastCallTime = DateTime.Now;

        private FlowLayoutPanel flowPanel;

        private readonly Color4 backgroundColor = new(42, 59, 61, 255);

        private int pickedIndex = -1;

        private bool isSpherePicked = true;



        private readonly Dictionary<string, bool> keyStates = new()
        {
            {"up", false },
            {"down", false },
            {"left", false },
            {"right", false },
            {"rightMouse", false },
            {"leftMouse", false },
            {"middleMouse", false },
        };

        private static class SpherePattern
        {
            static public float radius = 0.5f;
            static public Color color = Color.Red;
            static public string text = "";
        }

        private enum RayCastingObjectMod
        {
            change,
            delete,
            connect,
        }

        private RayCastingObjectMod currentRayCastingObjectMod = RayCastingObjectMod.change;

        private int lastMouseX = 0;
        private int lastMouseY = 0;
        private int currentMouseX = 0;
        private int currentMouseY = 0;

        private static readonly GLControlSettings setting = new()
        {
            NumberOfSamples = 4,
        };

        public OpenGL_Window() : base(setting) => InitializeComponent();

        public OpenGL_Window(IContainer container) : base(setting)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void UpdateProjectionMatrix()
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(float.Pi / 4, width / (float)height, 0.1f, 100);
        }

        public void DoLoad(FlowLayoutPanel flowPanel)
        {
            GL.ClearColor(backgroundColor);
            
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Multisample);

            UpdateProjectionMatrix();

            this.Spheres = new Spheres(vertexPathSphere, fragmentPathSphere);

            this.Cylinders = new(vertexPathCylinders, fragmentPathCylinders);

            CoordinateGrid = new(vertexPathCoordinateGrid, fragmentPathCoordinateGrid);

            this.flowPanel = flowPanel;
            this.flowPanel.AutoSize = true;
        }

        public void DoResize()
        {
            try
            {
                this.MakeCurrent();

                if (this.ClientRectangle.Height == 0)
                    this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, 1);

                GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                UpdateProjectionMatrix();
            }
            catch (OpenTK.Windowing.GraphicsLibraryFramework.GLFWException) { }
        }



        public void UpdateFrame()
        {
            float deltaTime = (float)(DateTime.Now - lastCallTime).TotalMilliseconds;
            lastCallTime = DateTime.Now;

            MoveCamera(deltaTime);

            if (LeftMouseMoveEnable)
                MoveCamera();

            RotateCamera();
            RollCamera();

            Light.SetLightDirection(Camera.GetCameraUpDirection());
            
            Spheres.Update(projectionMatrix, Camera.GetViewMatrix(), Camera.GetCameraPositionVector());
            Cylinders.Update(projectionMatrix, Camera.GetViewMatrix(), Camera.GetCameraPositionVector());

            if (CoordGridEnable)
                CoordinateGrid.Update(projectionMatrix, Camera.GetViewMatrix(), Camera.GetCameraPositionVector());
        }

        public void RenderFrame()
        {
            try
            {
                this.MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                if (isSpherePicked)
                {
                    Spheres.Draw(pickedIndex);
                    Cylinders.Draw();
                }
                else
                {
                    Spheres.Draw();
                    Cylinders.Draw(pickedIndex);
                }

                if (CoordGridEnable)
                    CoordinateGrid.Draw();

                if (pickedIndex >= 0)
                {
                    if (isSpherePicked)
                    {
                        Spheres.DrawPickedSphere(pickedIndex);
                        Spheres.DrawPickedSphereOutlining(pickedIndex, RayCastingGridEnable);
                    }
                    else
                    {
                        Cylinders.DrawPickedCylinder(pickedIndex);
                        Cylinders.DrawPickedCylinderOutlining(pickedIndex);
                    }
                }

                this.SwapBuffers();
            }
            catch(OpenTK.Windowing.GraphicsLibraryFramework.GLFWException) { }
        }

        public void KeyDownProcessing(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    keyStates["down"] = false;
                    keyStates["up"] = true;
                    break;
                case Keys.Down:
                    keyStates["up"] = false;
                    keyStates["down"] = true;
                    break;
                case Keys.Left:
                    keyStates["right"] = false;
                    keyStates["left"] = true;
                    break;
                case Keys.Right:
                    keyStates["left"] = false;
                    keyStates["right"] = true;
                    break;
                default:
                    break;

            }
        }

        public void KeyUpProcessing(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    keyStates["up"] = false;
                    break;
                case Keys.Down:
                    keyStates["down"] = false;
                    break;
                case Keys.Left:
                    keyStates["left"] = false;
                    break;
                case Keys.Right:
                    keyStates["right"] = false;
                    break;
                default:
                    break;
            }
        }

        private void MoveCamera(float deltaTime)
        {
            if (keyStates["up"]) Camera.MoveCamera(upDown: -keySpeed * deltaTime);
            if (keyStates["down"]) Camera.MoveCamera(upDown: keySpeed * deltaTime);
            if (keyStates["right"]) Camera.MoveCamera(leftRight: -keySpeed * deltaTime);
            if (keyStates["left"]) Camera.MoveCamera(leftRight: keySpeed * deltaTime);

        }

        private void MoveCamera()
        {
            if (!keyStates["leftMouse"]) return;

            float deltaY = currentMouseX - lastMouseX;
            float deltaX = currentMouseY - lastMouseY;
            Camera.MoveCamera(x: -deltaY * mouseMoveSensitivity, y: deltaX * mouseMoveSensitivity);

            lastMouseX = currentMouseX;
            lastMouseY = currentMouseY;
        }


        private void RayCastingGridMouseMoveProceed(MouseEventArgs e)
        {
            if (!CoordGridEnable || !RayCastingGridEnable)
            {
                return;
            }

            Vector3? nullablePoint = CoordinateGrid.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));
            if (nullablePoint is null)
            {
                return;
            }

            isSpherePicked = true;

            Vector3 point = nullablePoint.Value;
            
            if (pickedIndex < 0)
            {
                int oldIndex = Spheres.GetLastSphereIndex();
                Spheres.CreateNewSphere(point, SpherePattern.radius, SpherePattern.color, SpherePattern.text);
                int index = Spheres.GetLastSphereIndex();

                if (oldIndex == index)
                {
                    return;
                }

                pickedIndex = index;
                fillFlowPanelBySphere(true, true);
                return;
            }

            SpherePattern.radius = Spheres.GetSpheresRadius(pickedIndex);
            SpherePattern.color = ConvertVector4ToColor(Spheres.GetSpheresColor(pickedIndex));
            SpherePattern.text = Spheres.GetSpheresText(pickedIndex);

            changeSphereCordsInFlowPanel(point);

            Spheres.SetSpheresCenterCord(pickedIndex, point, true);

        }

        private void RayCastigGridMouseDownProceed()
        {
            if (!CoordGridEnable || !RayCastingGridEnable)
            {
                return;
            }
            if (pickedIndex < 0 || !isSpherePicked)
            {
                return;
            }

            if (Spheres.isTheSphereOverlappingAnother(pickedIndex))
            {
                MessageBox.Show("Сфера пересекается с другими сферами!", "Внимане", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            resetPickedObject();
        }

        private void RayCastingObgProceed(MouseEventArgs e)
        {
            if (!RayCastingObjectEnable)
            {
                return;
            }

            int sphereIndex;
            float sphereDist;

            int cylinderIndex;
            float cylinderDist;

            (sphereIndex, sphereDist) = Spheres.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));

            (cylinderIndex, cylinderDist) = Cylinders.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));

            List<(int, float, bool)> candidates = [(sphereIndex, sphereDist, true), (cylinderIndex, cylinderDist, false)];
            var validCandidates = candidates.Where(x => x.Item1 >= 0).ToList();

            void changeAction()
            {
                if (validCandidates.Count == 0)
                {
                    resetPickedObject();
                }
                else
                {
                    var closetCandidate = validCandidates.MinBy(x => x.Item2);
                    isSpherePicked = closetCandidate.Item3;
                    pickedIndex = closetCandidate.Item1;
                    if (isSpherePicked)
                    {
                        fillFlowPanelBySphere();
                    }
                    else
                    {
                        fillFlowPanelByCylinder();
                    }
                }
            }

            void deleteAction()
            {
                if (validCandidates.Count == 0)
                {
                    return;
                }

                var closetCandidate = validCandidates.MinBy(x => x.Item2);
                bool isSpherePicked = closetCandidate.Item3;
                int pickedIndex = closetCandidate.Item1;

                if (pickedIndex == this.pickedIndex && isSpherePicked == this.isSpherePicked)
                {
                    resetPickedObject();
                }
                else if (pickedIndex < this.pickedIndex && isSpherePicked == this.isSpherePicked)
                {
                    this.pickedIndex--;
                }    


                if (isSpherePicked)
                {
                    Vector3 spherePos = Spheres.GetSpheresCenterCord(pickedIndex);
                    float radius = Spheres.GetSpheresRadius(pickedIndex);
                    List<int> cylinderIndicesInSphere = Cylinders.GetCylindersIndeciesInSphere(spherePos, radius);
                    cylinderIndicesInSphere = [.. cylinderIndicesInSphere.OrderDescending()];

                    if (!this.isSpherePicked && cylinderIndicesInSphere.Contains(this.pickedIndex))
                    {
                        resetPickedObject();
                    }

                    foreach (int index in cylinderIndicesInSphere)
                    {
                        if (index < this.pickedIndex && !this.isSpherePicked)
                        {
                            this.pickedIndex--;
                        }
                        Cylinders.DelCylinderByIndex(index);
                    }
                    Spheres.DelSphereByIndex(pickedIndex);
                }
                else
                {
                    Cylinders.DelCylinderByIndex(pickedIndex);
                }



            }

            void connectAction()
            {
                if (validCandidates.Count == 0)
                {
                    return;
                }

                var closetCandidate = validCandidates.MinBy(x => x.Item2);
                bool isSpherePicked = closetCandidate.Item3;
                int pickedIndex = closetCandidate.Item1;

                if (!isSpherePicked)
                {
                    return;
                }


                if (!this.isSpherePicked || this.pickedIndex < 0)
                {
                    this.isSpherePicked = true;
                    this.pickedIndex = pickedIndex;
                    fillFlowPanelBySphere();
                    return;
                }

                if (this.pickedIndex == pickedIndex)
                {
                    resetPickedObject();
                    return;
                }

                Vector3 spherePos1 = Spheres.GetSpheresCenterCord(this.pickedIndex);
                float radius1 = Spheres.GetSpheresRadius(this.pickedIndex);
                Vector3 spherePos2 = Spheres.GetSpheresCenterCord(pickedIndex);
                float radius2 = Spheres.GetSpheresRadius(pickedIndex);

                int numberOfCylindersInConnection = Cylinders.GetCylindersCountBetweenTwoSpheres(spherePos1, radius1, spherePos2, radius2);
                
                if (numberOfCylindersInConnection > 0)
                {
                    MessageBox.Show("Соедние между сферами уже существует!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    resetPickedObject();
                }
                else
                {
                    Cylinders.CreateNewCylinder(spherePos1, spherePos2, 0.06f, Color.Black);
                    this.isSpherePicked = false;
                    this.pickedIndex = Cylinders.GetLastCylinderIndex();
                    fillFlowPanelByCylinder();
                }
            }

            Dictionary<RayCastingObjectMod, Action> actions = new()
                    {
                        {RayCastingObjectMod.change,  changeAction},
                        {RayCastingObjectMod.delete, deleteAction },
                        {RayCastingObjectMod.connect, connectAction }
                    };

            actions[currentRayCastingObjectMod]();




        }

        public void MouseDownProcessing(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                keyStates["middleMouse"] = true;

            if (e.Button == MouseButtons.Right)
                keyStates["rightMouse"] = true;

            if (e.Button == MouseButtons.Left)
            {
                keyStates["leftMouse"] = true;
                RayCastingObgProceed(e);
                RayCastigGridMouseDownProceed();
            }

            lastMouseX = e.X;   
            lastMouseY = e.Y;
            currentMouseX = e.X;
            currentMouseY = e.Y;
        }

        public void MouseUpProcessing(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                keyStates["rightMouse"] = false;

            if (e.Button == MouseButtons.Left)
                keyStates["leftMouse"] = false;

            if (e.Button == MouseButtons.Middle)
                keyStates["middleMouse"] = false;
        }

        public void MouseMoveProcessing(MouseEventArgs e)
        {
            currentMouseX = e.X;
            currentMouseY = e.Y;
            RayCastingGridMouseMoveProceed(e);
        }
        private void RotateCamera()
        {
            if (!keyStates["rightMouse"]) return;

            float deltaY = currentMouseX - lastMouseX;
            float deltaX = currentMouseY - lastMouseY;
            Camera.RotateCamera(pitch: -deltaX * mouseRotateSensitivity, yaw: -deltaY * mouseRotateSensitivity);
            lastMouseX = currentMouseX;
            lastMouseY = currentMouseY;
        }

        private void RollCamera()
        {
            if (!keyStates["middleMouse"])
                return;

            float deltaX = currentMouseX - lastMouseX;
            Camera.RotateCamera(roll: -deltaX * mouseMoveSensitivity);
            lastMouseX = currentMouseX;
            lastMouseY = currentMouseY;
        }

        public void MouseWheelProcessing(MouseEventArgs e)
        {
            float value;
            if (e.Delta > 0) value = -zoomFactorSensitivity;
            else value = zoomFactorSensitivity;

            Camera.MoveCamera(backForward: value);
        }

        private Vector3 GetRayDirection(int mouseX, int mouseY)
        {
            float x = (2 * mouseX) / (float)ClientSize.Width - 1;
            float y = 1 - (2 * mouseY) / (float)ClientSize.Height;
            
            Vector4 rayClip = (x, y, -1, 1);
            Vector4 rayEye = rayClip * Matrix4.Invert(projectionMatrix);
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1, 0);
            Vector3 rayWor = (rayEye * Matrix4.Invert(Camera.GetViewMatrix())).Xyz;
            return Vector3.Normalize(rayWor);
        }

        public void LoadFromFlypFile(string path)
        {
            resetPickedObject();
            Spheres.DelAllSpheres();
            Cylinders.DelAllCylinders();
            var flypLoader = new FlypLoader(this);
            flypLoader.OpenFlypFile(path);
        }

        public void LoadFromJson(string path)
        {
            resetPickedObject();
            Spheres.DelAllSpheres();
            Cylinders.DelAllCylinders();
            Json_OpenGL_Window.ReadJson(path, Spheres, Cylinders);
        }

        public void AddSphere(Vector3 position, float radius, Color color, string text)
        {
            Spheres.CreateNewSphere(position, radius, color, text);
        }

        public void AddCylinder(Vector3 pos1, Vector3 pos2, float radius, Color color)
        {
            Cylinders.CreateNewCylinder(pos1, pos2, radius, color);
        }
        

        public void SetGridStep(float step)
        {
            CoordinateGrid.step = step;
        }

        public void SetGridLevel(float level)
        {
            CoordinateGrid.yCord = level;
        }

        public float getGridStep()
        {
            return CoordinateGrid.step;
        }


        private void SetPickObjSettings()
        {
            LeftMouseMoveEnable = false;
            RayCastingObjectEnable = true;
        }

        private void DisableAddMode()
        {
            if (pickedIndex >= 0 && isSpherePicked && RayCastingGridEnable)
            {
                Spheres.DelSphereByIndex(pickedIndex);
                resetPickedObject();
            }
            RayCastingGridEnable = false;
        }

        public void EnableClickMode()
        {
            SetPickObjSettings();
            DisableAddMode();
            currentRayCastingObjectMod = RayCastingObjectMod.change;
        }

        public void EnableMoveMode()
        {
            DisableAddMode();
            LeftMouseMoveEnable = true;
            RayCastingObjectEnable = false;
            RayCastingGridEnable = false;
        }

        public void EnableDeleteMode()
        {
            DisableAddMode();
            SetPickObjSettings();
            currentRayCastingObjectMod = RayCastingObjectMod.delete;
        }

        public void EnableAddMode()
        {
            LeftMouseMoveEnable = false;
            RayCastingObjectEnable = false;
            RayCastingGridEnable = true;
            resetPickedObject();
        }

        public void EnableConnectionMode()
        {
            DisableAddMode();
            SetPickObjSettings();
            currentRayCastingObjectMod = RayCastingObjectMod.connect;
        }


        private void fillFlowPanelBySphere(bool isCordReadOnly = false, bool OverlapsEnable = false)
        {
            flowPanel.Controls.Clear();
            Vector3 position = Spheres.GetSpheresCenterCord(pickedIndex);
            float radius = Spheres.GetSpheresRadius(pickedIndex);
            Vector4 color = Spheres.GetSpheresColor(pickedIndex);
            string text = Spheres.GetSpheresText(pickedIndex);

            AddLabelAndTextBox("Position X:", position.X.ToString("F2"), 0, isCordReadOnly, OverlapsEnable);
            AddLabelAndTextBox("Position Y:", position.Y.ToString("F2"), 1, isCordReadOnly, OverlapsEnable);
            AddLabelAndTextBox("Position Z:", position.Z.ToString("F2"), 2, isCordReadOnly, OverlapsEnable);

            AddLabelAndTextBox("Radius:", radius.ToString("F2"), 3, OverlapsEnable: true);

            AddColorPicker("Color:", color, 0);

            AddLabelAndTextBox("Text:", text, 5);
        }

        private void fillFlowPanelByCylinder()
        {
            flowPanel.Controls.Clear();

            float radius = Cylinders.GetCylinderRadius(pickedIndex);
            Vector4 color = Cylinders.GetCylinderColor(pickedIndex);

            AddLabelAndTextBox("Radius:", radius.ToString("F2"), 4);

            AddColorPicker("Color:", color, 1);
        }



        private void AddColorPicker(string labelText, Vector4 color, int mod)
        {
            var label = new Label
            {
                Text = labelText,
                Width = 100,
                Margin = new Padding(3, 10, 3, 3)
            };

            var colorPanel = new Panel
            {
                Width = 100,
                Height = 20,
                BackColor = ConvertVector4ToColor(color),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Margin = new Padding(3, 10, 3, 3)
            };

            colorPanel.Click += (sender, e) =>
            {
                using var colorDialog = new ColorDialog();
                colorDialog.Color = colorPanel.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorPanel.BackColor = colorDialog.Color;

                    Vector4 newColor = ConvertColorToVector4(colorDialog.Color);
                    switch (mod)
                    {
                        case 0:
                            Spheres.SetSphereColor(pickedIndex, newColor);
                            break;
                        case 1:
                            Cylinders.SetCylinderColor(pickedIndex, newColor);
                            break;
                    }
                }
            };

            flowPanel.Controls.Add(label);
            flowPanel.Controls.Add(colorPanel);
            flowPanel.SetFlowBreak(colorPanel, true);
        }

        private static Color ConvertVector4ToColor(Vector4 vec)
        {
            int r = (int)(vec.X * 255);
            int g = (int)(vec.Y * 255);
            int b = (int)(vec.Z * 255);
            int a = (int)(vec.W * 255);
            return Color.FromArgb(a, r, g, b);
        }

        private static Vector4 ConvertColorToVector4(Color color)
        {
            return new Vector4(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f
            );
        }

        private void UpdateSphereCoordinate(int coordIndex, float value, bool OverlapsEnable = false)
        {
            Vector3 newPos = Spheres.GetSpheresCenterCord(pickedIndex);
            switch (coordIndex)
            {
                case 0:
                    newPos.X = value;
                    break;
                case 1:
                    newPos.Y = value;
                    break;
                case 2:
                    newPos.Z = value;
                    break;
            }
            Vector3 oldPos = Spheres.GetSpheresCenterCord(pickedIndex);
            float radius = Spheres.GetSpheresRadius(pickedIndex);
            Vector3 moveVector = newPos - oldPos;
            Spheres.SetSpheresCenterCord(pickedIndex, newPos, OverlapsEnable);
            Cylinders.MoveCylindersWithSphere(oldPos, radius, moveVector);
        }

        private void UpdateObjectRadius(int mod, float radius, bool OverlapsEnable = false)
        {
            if (radius <= 0)
            {
               throw new ArgumentException("Радиус должен быть больше 0!");
            }

            switch (mod)
            {
                case 3:
                    Spheres.SetSpheresRadius(pickedIndex, radius, OverlapsEnable);
                    break;
                case 4:
                    Cylinders.SetCylinderRadius(pickedIndex, radius);
                    break;

            }
        }

        private void resetPickedObject()
        {
            clearFlowPanel();
            pickedIndex = -1;
        }

        private void clearFlowPanel()
        {
            flowPanel.Controls.Clear();
        }

        private void TextBoxProceedValue(TextBox textBox, int mod, bool OverlapsEnable = false)
        {
            string currentText = textBox.Text;
            try
            {
                switch (mod)
                {
                    case 0:
                    case 1:
                    case 2:
                        float cord = float.Parse(currentText.Replace('.', ','));
                        UpdateSphereCoordinate(mod, cord, OverlapsEnable);
                        break;
                    case 3:
                    case 4:
                        float radius = float.Parse(currentText.Replace('.', ','));
                        UpdateObjectRadius(mod, radius, OverlapsEnable);
                        break;
                    case 5:
                        string invalidChars = "<>:\"/\\|?*";
                        if (currentText.IndexOfAny(invalidChars.ToCharArray()) != -1)
                            throw new ArgumentException("Текст содержит недопустимые символы!");
                        Spheres.SetSpheresText(pickedIndex, currentText);
                        break;
                }
                textBox.Tag = currentText;
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат числа!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Изменение параметров ведет к пересечению сфер!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                textBox.Text = (string)textBox.Tag;
            }
        }
        private void AddLabelAndTextBox(string labelText, string textBoxText, int mod, bool isTextBoxReadOnly = false, bool OverlapsEnable = false)
        {
            var label = new Label
            {
                Text = labelText,   
                Width = 100,
                Margin = new Padding(3, 10, 3, 3)
            };

            var textBox = new TextBox
            {
                Text = textBoxText,
                Tag = textBoxText,
                Width = 100,
                Margin = new Padding(3, 10, 3, 3),
                ReadOnly = isTextBoxReadOnly
            };

            textBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }
                TextBoxProceedValue(textBox, mod, OverlapsEnable);
                e.SuppressKeyPress = true;
            };

            textBox.Leave += (s, e) =>
            {
                TextBoxProceedValue(textBox, mod, OverlapsEnable);
            };

            flowPanel.Controls.Add(label);
            flowPanel.Controls.Add(textBox);

            flowPanel.SetFlowBreak(textBox, true);

        }

        private void changeSphereCordsInFlowPanel(Vector3 cords)
        {
            List<Control> controlList = [.. flowPanel.Controls.Cast<Control>()];

            Control xTextBox = controlList[1];
            Control yTextBox = controlList[3];
            Control zTextBox = controlList[5];
            xTextBox.Text = cords.X.ToString("F2");
            yTextBox.Text = cords.Y.ToString("F2");
            zTextBox.Text = cords.Z.ToString("F2");
        }

        public string? GetJsonStringWithData()
        {
            return Json_OpenGL_Window.GetJson(Spheres, Cylinders);
        }

        public void ResetCamera()
        {
            Camera.ResetCamera();
        }
    }
}

