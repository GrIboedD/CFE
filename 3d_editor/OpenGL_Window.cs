using _3d_editor.Geometric_figures;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;


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
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                Spheres.Draw();
                Cylinders.Draw();

                if (CoordGridEnable)
                    CoordinateGrid.Draw();

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

        public void MouseDownProcessing(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                keyStates["middleMouse"] = true;

            if (e.Button == MouseButtons.Right)
                keyStates["rightMouse"] = true;

            lastMouseX = e.X;   
            lastMouseY = e.Y;
            currentMouseX = e.X;
            currentMouseY = e.Y;    

            if (e.Button == MouseButtons.Left)
            {
                keyStates["leftMouse"] = true;

                if (RayCastingObjectEnable)
                {

                    flowPanel.Controls.Clear();

                    int sphereIndex;
                    float sphereDist;

                    int cylinderIndex;
                    float cylinderDist;

                    (sphereIndex, sphereDist) = Spheres.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));

                    (cylinderIndex, cylinderDist) = Cylinders.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));
                    


                    if (sphereIndex == -1 && cylinderIndex == -1)
                    {
                        flowPanel.Controls.Clear();
                        flowPanel.Height = 0;
                    }
                    else if (sphereIndex == -1)
                    {
                        Console.WriteLine($"Cylinder {cylinderIndex} is picked");
                    }
                    else if (cylinderIndex == -1)
                    {
                        fillFlowPanelBySphere(sphereIndex);
                    }
                    else
                    {
                        if (cylinderDist < sphereDist)
                        {
                            Console.WriteLine($"Cylinder {cylinderIndex} is picked");
                        }
                        else
                        {
                            fillFlowPanelBySphere(sphereIndex);
                        }
                    }
                }

                if (CoordGridEnable && RayCastingGridEnable)
                    CoordinateGrid.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));
            }
               
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
            Spheres.DelAllSpheres();
            Cylinders.DelAllCylinders();
            var flypLoader = new FlypLoader(this);
            flypLoader.OpenFlypFile(path);
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

        public void EnableClickMode()
        {
            LeftMouseMoveEnable = true;
            RayCastingObjectEnable = true;
            RayCastingGridEnable = false;
        }

        private void fillFlowPanelBySphere(int index)
        {
            flowPanel.Controls.Clear();
            Vector3 position = Spheres.GetSpheresCenterCord(index);
            float radius = Spheres.GetSpheresRadius(index);
            Vector4 color = Spheres.GetSpheresColor(index);
            string text = Spheres.GetSpheresText(index);

            AddLabelAndTextBox("Position X:", position.X.ToString("F2"), index, 0);
            AddLabelAndTextBox("Position Y:", position.Y.ToString("F2"), index, 1);
            AddLabelAndTextBox("Position Z:", position.Z.ToString("F2"), index, 2);

            AddLabelAndTextBox("Radius:", radius.ToString("F2"), index, 3);

            AddColorPicker("Color:", color, index, 0);

            AddLabelAndTextBox("Text:", text, index, 4);

            flowPanel.Height = 245;
        }

        private void AddColorPicker(string labelText, Vector4 color, int index, int mod)
        {
            var label = new Label();
            label.Text = labelText;
            label.Width = 100;
            label.Margin = new Padding(3, 10, 3, 3);

            var colorPanel = new Panel();
            colorPanel.Width = 100;
            colorPanel.Height = 20;
            colorPanel.BackColor = ConvertVector4ToColor(color);
            colorPanel.BorderStyle = BorderStyle.FixedSingle;
            colorPanel.Cursor = Cursors.Hand;
            colorPanel.Margin = new Padding(3, 10, 3, 3);

            colorPanel.Click += (sender, e) =>
            {
                using (var colorDialog = new ColorDialog())
                {
                    colorDialog.Color = colorPanel.BackColor;
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        colorPanel.BackColor = colorDialog.Color;

                        Vector4 newColor = ConvertColorToVector4(colorDialog.Color);
                        if (mod == 0)
                        {
                            Spheres.SetSphereColor(index, newColor);
                        }
                    }
                }
            };

            flowPanel.Controls.Add(label);
            flowPanel.Controls.Add(colorPanel);
            flowPanel.SetFlowBreak(colorPanel, true);
        }

        private Color ConvertVector4ToColor(Vector4 vec)
        {
            int r = (int)(vec.X * 255);
            int g = (int)(vec.Y * 255);
            int b = (int)(vec.Z * 255);
            int a = (int)(vec.W * 255);
            return Color.FromArgb(a, r, g, b);
        }

        private Vector4 ConvertColorToVector4(Color color)
        {
            return new Vector4(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f
            );
        }

        private void AddLabelAndTextBox(string labelText, string textBoxText, int index, int mod)
        {
            var label = new Label();
            label.Text = labelText;
            label.Width = 100;
            label.Margin = new Padding(3, 10, 3, 3);

            var textBox = new TextBox();
            textBox.Text = textBoxText;
            textBox.Tag = textBoxText;
            textBox.Width = 100;
            textBox.Margin = new Padding(3, 10, 3, 3);
            textBox.TextChanged += (s, e) =>
            {
                string currentText = textBox.Text;
                if (mod is 0 or 1 or 2 or 3)
                {
                    try
                    {
                        float value = float.Parse(currentText);
                        textBox.Tag = currentText;
                        if (mod is 3)
                        {
                            Spheres.SetSpheresRadius(index, value);
                        }
                        else if (mod is 0 or 1 or 2)
                        {
                            Vector3 newPos = Spheres.GetSpheresCenterCord(index);
                            switch (mod)
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
                            Spheres.SetSpheresCenterCord(index, newPos);
                        }
                    }
                    catch
                    {
                        textBox.Text = (string)textBox.Tag;
                    }
                }
                if (mod is 4)
                {
                    Spheres.SetSpheresText(index, currentText);
                }
            };

            flowPanel.Controls.Add(label);
            flowPanel.Controls.Add(textBox);

            flowPanel.SetFlowBreak(textBox, true);

        }
    }
}

