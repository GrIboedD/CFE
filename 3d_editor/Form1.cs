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
            OpenGL_Window.MouseWheel += OpenGL_Window_MouseWheel;
        }

        private void OpenGL_Window_Paint(object sender, PaintEventArgs e)
        {
            OpenGL_Window.UpdateFrame();
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
            OpenGL_Window.UpdateFrame();
            OpenGL_Window.RenderFrame();
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

        private void OpenGL_Window_MouseDown(object sender, MouseEventArgs e)
        {
            OpenGL_Window.MouseDownProcessing(e);
        }

        private void OpenGL_Window_MouseUp(object sender, MouseEventArgs e)
        {
            OpenGL_Window.MouseUpProcessing(e);
        }

        private void OpenGL_Window_MouseMove(object sender, MouseEventArgs e)
        {
            OpenGL_Window.MouseMoveProcessing(e);
        }

        private void OpenGL_Window_MouseWheel(object sender, MouseEventArgs e)
        {
            OpenGL_Window.MouseWheelProcessing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Выбор модели";
            openFileDialog.Filter = "3d модель|*.flyp";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                OpenGL_Window.LoadFromFlypFile(selectedFilePath);
            }

        }
    }
}
