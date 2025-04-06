using _3d_editor.Geometric_figures;
using _3d_editor.View;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.ComponentModel;

namespace _3d_editor
{
    [ToolboxItem(true)]
    public partial class OpenGL_Window: GLControl
    {
        const float keySpeed = 0.01f;
        const float mouseSensitivity = 0.007f;
        const float zoomFactorSensitivity = 0.05f;

        private const string vertexPathSphere = "../../../Shaders/sphere.vert";
        private const string fragmentPathSphere = "../../../Shaders/sphere.frag";

        private readonly Camera Camera = new();

        private Matrix4 projectionMatrix;

        Spheres spheres;

        private DateTime lastCallTime = DateTime.Now;


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

        public OpenGL_Window() => InitializeComponent();

        public OpenGL_Window(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void UpdateProjectionMatrix() => projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(float.Pi / 4, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100);

        public void DoLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            UpdateProjectionMatrix();

            this.spheres = new Spheres(vertexPathSphere, fragmentPathSphere);
            this.spheres.CreateNewSphere(2, 0, 0, 1f);
            this.spheres.CreateNewSphere(-2, 0, 0, 0.5f);
        }

        public void DoResize()
        {
            this.MakeCurrent();

            if (this.ClientRectangle.Height == 0)
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, 1);

            GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            UpdateProjectionMatrix();
        }



        public void UpdateFrame()
        {
            float deltaTime = (float)(DateTime.Now - lastCallTime).TotalMilliseconds;
            lastCallTime = DateTime.Now;

            this.spheres.Update(projectionMatrix, Camera.GetViewMatrix());

            MoveCamera(deltaTime);
            RotateCamera();
            SetCameraZoom();
        }

        public void RenderFrame()
        {
            this.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.spheres.Draw();
            this.SwapBuffers();
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

            if (e.Button == MouseButtons.Left) spheres.ClickOnObject(e.X, e.Y, this.ClientSize.Width, this.ClientSize.Height);
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
    }
}
