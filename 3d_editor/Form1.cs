using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;   
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OpenTK.GLControl;
using System.Windows.Forms;

namespace _3d_editor
{
    public partial class Form1 : Form
    {

        GL_Window gl_window = new GL_Window();

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            gl_window.Load();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            gl_window.Resize(glControl1);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            gl_window.RenderFrame(glControl1);

        }
    }
}
