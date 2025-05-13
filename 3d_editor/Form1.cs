using System.Drawing;
using System.Globalization;

namespace _3d_editor
{

    public partial class CFE : Form
    {
        public CFE()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1;
            timer.Start();

            OpenGL_Window.SetGridLevel(0);
            OpenGL_Window.SetGridStep(0.5f);
            numericUpDown1.Value = 0;
            numericUpDown1.Increment = 0.5m;
            textBox1.Text = "0,5";
            textBox1.Tag = "0,5";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OpenGL_Window.UpdateFrame();
            OpenGL_Window.RenderFrame();
        }

        private void OpenGL_Window_Load(object sender, EventArgs e)
        {
            OpenGL_Window.DoLoad(flowLayoutPanel1);
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

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Выбор модели",
                Filter = "3d модель|*.flyp",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                OpenGL_Window.LoadFromFlypFile(selectedFilePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Cursor = Cursors.SizeAll;
            OpenGL_Window.EnableMoveMode();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Cursor = Cursors.Default;
            OpenGL_Window.EnableClickMode();

        }


        private void textBox1_ChangeValue_focus_leave(object sender, EventArgs e)
        {
            textBox1_ChangeValue();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            textBox1_ChangeValue();
        }

        private void textBox1_ChangeValue()
        {
            string value = textBox1.Text;
            try
            {
                float step = float.Parse(value.Replace('.',','));
                if (step < 0.2f || step > 2)
                {
                    throw new ArgumentException();
                }
                OpenGL_Window.SetGridStep(step);
                numericUpDown1.Increment = (decimal)step;
                textBox1.Tag = value;
            }
            catch (FormatException)
            {
                textBox1.Text = (string)textBox1.Tag;
                MessageBox.Show("Неверный формат числа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException)
            {
                textBox1.Text = (string)textBox1.Tag;
                MessageBox.Show("Шаг сетки от 0.2 до 2!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            float level = (float)numericUpDown1.Value;
            float step = (float)numericUpDown1.Increment;
            int numberOfSteps = (int)Math.Round(level / step);
            numericUpDown1.Value = (decimal)(numberOfSteps * step);
            OpenGL_Window.SetGridLevel(numberOfSteps * step);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Cursor = Cursors.Cross;
            OpenGL_Window.EnableDeleteMode();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
