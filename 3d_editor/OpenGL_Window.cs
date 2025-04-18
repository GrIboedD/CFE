using _3d_editor.Geometric_figures;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.ComponentModel;


namespace _3d_editor
{
    [ToolboxItem(true)]
    public partial class OpenGL_Window: GLControl
    {

        const float keySpeed = 0.005f;
        const float mouseSensitivity = 0.007f;
        const float zoomFactorSensitivity = 0.05f;

        private const string vertexPathSphere = "../../../Shaders/sphere.vert";
        private const string fragmentPathSphere = "../../../Shaders/sphere.frag";

        private const string vertexPathCoordinateGrid = "../../../Shaders/coordinateGrid.vert";
        private const string fragmentPathCoordinateGrid = "../../../Shaders/coordinateGrid.frag";

        private const string vertexPathCylinders = "../../../Shaders/cylinder.vert";
        private const string fragmentPathCylinders = "../../../Shaders/cylinder.frag";

        private readonly Camera Camera = new();

        private Matrix4 projectionMatrix;

        private Spheres Spheres;

        private Cylinders Cylinders;

        private CoordinateGrid CoordinateGrid;

        private DateTime lastCallTime = DateTime.Now;

        private readonly Color4 backgroundColor = new(42, 59, 61, 255);

        private readonly Dictionary<string, bool> keyStates = new()
        {
            {"up", false },
            {"down", false },
            {"left", false },
            {"right", false },
            {"rightMouse", false },
        };


        private int lastMouseX = 0;
        private int lastMouseY = 0;
        private int currentMouseX = 0;
        private int currentMouseY = 0;

        private float zoomFactor = 1;

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

        public void DoLoad()
        {
            GL.ClearColor(backgroundColor);
            
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.GetInteger(GetPName.Samples, out int samples);
            Console.WriteLine(samples);


            GL.Enable(EnableCap.Multisample);

            UpdateProjectionMatrix();

            this.Spheres = new Spheres(vertexPathSphere, fragmentPathSphere);
            this.Spheres.CreateNewSphere(new Vector3(1, 1, 0), 0.5f, Color.Red, "O");
            this.Spheres.CreateNewSphere(new Vector3(-1, 0, 0), 0.5f, Color.Blue, "H");
            this.Spheres.CreateNewSphere(new Vector3(1, -1, 0), 0.5f, Color.Blue, "H");
            this.Spheres.ConnectSpheres(0, 1);
            this.Spheres.ConnectSpheres(0, 2);


            this.Cylinders = new(vertexPathCylinders, fragmentPathCylinders, Spheres);

            CoordinateGrid = new(vertexPathCoordinateGrid, fragmentPathCoordinateGrid);
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
            catch (OpenTK.Windowing.GraphicsLibraryFramework.GLFWException ex)
            {
                Console.WriteLine($"GLWindow MakeCurrent error: {ex}");
            }
        }



        public void UpdateFrame()
        {
            float deltaTime = (float)(DateTime.Now - lastCallTime).TotalMilliseconds;
            lastCallTime = DateTime.Now;

            MoveCamera(deltaTime);
            RotateCamera();
            SetCameraZoom();

            Spheres.Update(projectionMatrix, Camera.GetViewMatrix(), Camera.GetCameraPositionVector());
            Cylinders.Update(projectionMatrix, Camera.GetViewMatrix(), Camera.GetCameraPositionVector());
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
                CoordinateGrid.Draw();
                this.SwapBuffers();
            }
            catch(OpenTK.Windowing.GraphicsLibraryFramework.GLFWException ex)
            {
                Console.WriteLine($"GLWindow MakeCurrent error: {ex}");
            }

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

        public void MouseDownProcessing(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) keyStates["rightMouse"] = true;
            lastMouseX = e.X;
            lastMouseY = e.Y;
            currentMouseX = e.X;
            currentMouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
               int index = Spheres.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));
               if (index <= -1) Console.WriteLine("Miss");
               else Console.WriteLine($"Sphere {index} is picked");

                CoordinateGrid.RayCasting(Camera.GetCameraPositionVector(), GetRayDirection(e.X, e.Y));
            }
               
        }

        public void MouseUpProcessing(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) keyStates["rightMouse"] = false;
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
            Camera.RotateCamera(pitch: -deltaX * mouseSensitivity, yaw: -deltaY * mouseSensitivity);
            lastMouseX = currentMouseX;
            lastMouseY = currentMouseY;
        }

        public void MouseWheelProcessing(MouseEventArgs e)
        {
            if (e.Delta > 0) zoomFactor += zoomFactorSensitivity;
            else zoomFactor -= zoomFactorSensitivity;
        }

        private void SetCameraZoom()
        {
            Camera.SetZoom(zoomFactor);
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

    }
}

