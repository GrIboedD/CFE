namespace _3d_editor
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenGL_Window_Load(object sender, EventArgs e)
        {
            OpenGL_Window.DoLoad();
        }

        private void OpenGL_Window_Paint(object sender, PaintEventArgs e)
        {
            OpenGL_Window.UpdateFrame(OpenGL_Window.ClientSize.Width, OpenGL_Window.ClientSize.Height);
            OpenGL_Window.RenderFrame();
        }
       
        private void OpenGL_Window_Resize(object sender, EventArgs e)
        {
            OpenGL_Window.DoResize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OpenGL_Window.UpdateFrame(OpenGL_Window.ClientSize.Width, OpenGL_Window.ClientSize.Height);
            OpenGL_Window.RenderFrame();
        }
    }
}
