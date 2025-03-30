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

        private void OpenGL_Window_Enter(object sender, EventArgs e)
        {
            Console.WriteLine("Focus in");
        }

        private void OpenGL_Window_Leave(object sender, EventArgs e)
        {
            Console.WriteLine("Focus out");
        }

        private void OpenGL_Window_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Focus();
        }

        private void OpenGL_Window_KeyDown(object sender, KeyEventArgs e)
        {
            OpenGL_Window.KeyDownProcessing(e);
        }

        private void OpenGL_Window_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void OpenGL_Window_KeyUp(object sender, KeyEventArgs e)
        {
            OpenGL_Window.KeyUpProcessing(e);
        }
    }
}
