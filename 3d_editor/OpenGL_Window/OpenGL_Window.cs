using _3d_editor.Geometric_figures;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;

namespace _3d_editor
{
    [ToolboxItem(true)]
    public partial class OpenGL_Window: GLControl
    {
        private const string _vertexPath = "../../../Shaders/sphere.vert";
        private const string _fragmentPath = "../../../Shaders/sphere.frag";
        Sphere? sphere;
        public OpenGL_Window()
        {
            InitializeComponent();
        }

        public OpenGL_Window(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void DoLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            sphere = new Sphere(_vertexPath, _fragmentPath);
        }

        public void DoResize()
        {
            this.MakeCurrent();
            GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }

        public void UpdateFrame()
        {

        }

        public void RenderFrame()
        {
            this.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            sphere.Draw();
            this.SwapBuffers();
        }

        

    }
}
