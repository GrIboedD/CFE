﻿using _3d_editor.Geometric_figures;
using _3d_editor.View;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;

namespace _3d_editor
{
    [ToolboxItem(true)]
    public partial class OpenGL_Window: GLControl
    {
        const float keySpeed = 0.001f;

        private const string vertexPathSphere = "../../../Shaders/sphere.vert";
        private const string fragmentPathSphere = "../../../Shaders/sphere.frag";

        private readonly Camera Camera = new();

        Sphere sphere;

        private DateTime lastCallTime = DateTime.Now;

        private bool isKeyUp = false;
        private bool isKeyDown = false;
        private bool isKeyLeft = false;
        private bool isKeyRight = false;


        public OpenGL_Window() => InitializeComponent();

        public OpenGL_Window(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void DoLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            this.sphere = new Sphere(vertexPathSphere, fragmentPathSphere, Camera);
        }

        public void DoResize()
        {
            this.MakeCurrent();

            if (this.ClientRectangle.Height == 0)
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, 1);

            GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }



        public void UpdateFrame(int width, int height)
        {
            float deltaTime = (float)(DateTime.Now - lastCallTime).TotalMilliseconds;
            lastCallTime = DateTime.Now;
            this.sphere.Update(width, height, deltaTime);
            MoveCamera(deltaTime);
        }

        public void RenderFrame()
        {
            this.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.sphere.Draw();
            this.SwapBuffers();
        }

        public void KeyDownProcessing(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    isKeyDown = false;
                    isKeyUp = true;
                    break;
                case Keys.Down:
                    isKeyUp = false;
                    isKeyDown = true;
                    break;
                case Keys.Left:
                    isKeyRight = false;
                    isKeyLeft = true;
                    break;
                case Keys.Right:
                    isKeyLeft = false;
                    isKeyRight = true;
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
                    isKeyUp = false;
                    break;
                case Keys.Down:
                    isKeyDown = false;
                    break;
                case Keys.Left:
                    isKeyLeft = false;
                    break;
                case Keys.Right:
                    isKeyRight = false;
                    break;
                default:
                    break;
            }
        }

        private void MoveCamera(float deltaTime)
        {
            if (isKeyUp) Camera.MoveCameraUpDown(-keySpeed * deltaTime);
            if (isKeyDown) Camera.MoveCameraUpDown(keySpeed * deltaTime);
            if (isKeyRight) Camera.MoveCameraLeftRight(-keySpeed * deltaTime);
            if (isKeyLeft) Camera.MoveCameraLeftRight(keySpeed * deltaTime);
        }

    }
}
