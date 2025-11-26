using System.Data;
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
                Title = "Загрузить модель",
                Filter = "Все поддерживаемые файлы (*.flyp;*.json)|*.flyp;*.json|Flyp модель (*.flyp)|*.flyp|Json формат (*.json)|*.json",
                FilterIndex = 1,
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                string extension = Path.GetExtension(selectedFilePath).ToLower();
                switch (extension)
                {
                    case ".flyp":
                        OpenGL_Window.LoadFromFlypFile(selectedFilePath);
                        break;
                    case ".json":
                        OpenGL_Window.LoadFromJson(selectedFilePath);
                        break;
                }
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
                float step = float.Parse(value.Replace('.', ','));
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
                MessageBox.Show("Неверный формат числа!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ArgumentException)
            {
                textBox1.Text = (string)textBox1.Tag;
                MessageBox.Show("Шаг сетки от 0.2 до 2!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            OpenGL_Window.Cursor = GetCustomCursorFromImage("ImagesAndIcons/free-icon-cross-7699001.png");
            OpenGL_Window.EnableDeleteMode();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Cursor = GetCustomCursorFromImage("ImagesAndIcons/free-icon-cylinder-8726474.png");
            OpenGL_Window.EnableConnectionMode();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenGL_Window.Cursor = GetCustomCursorFromImage("ImagesAndIcons/free-icon-circle-14448782.png");
            OpenGL_Window.EnableAddMode();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? json = OpenGL_Window.GetJsonStringWithData();
            if (json is null)
            {
                MessageBox.Show("Файл сохранения пуст!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "JSON файл (*.json)|*.json",
                DefaultExt = "json",
                AddExtension = true,
                RestoreDirectory = true,
                Title = "Сохранить модель"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, json);
            }

        }

        private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenGL_Window.ResetCamera();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Cursor GetCustomCursorFromImage(string filePath)
        {
            Image img = Image.FromFile(filePath);
            Bitmap bmp = new Bitmap(img, new Size(32, 32));
            return new Cursor(bmp.GetHicon());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpText =
@"Программа предназначена для редактирования химических
соединений. С её помощью вы можете изменить информацию о
любом веществе из базы данных, включая его название,
молекулярную формулу и физико-химические свойства.
Также доступна функция редактирования 3D модели
молекулярной структуры";

            helpText = helpText.Replace(" ", "\u00A0");

            MessageBox.Show(
                helpText,
                "Справка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void controlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpText =
@"Для просмотра и взаимодействия с 3D-моделью
используйте следующие элементы управления:
• Зажмите левую кнопку мыши в режиме просмотра
и перемещайте курсор, чтобы передвигать модель
по плоскости;
• Для вращения модели вокруг оси удерживайте 
правую кнопку мыши и перемещайте курсор;
• Чтобы вращать модель относительно позиции 
камеры, зажмите среднюю кнопку мыши (колесо)
и перемещайте курсор.
Дополнительно перемещать модель можно с помощью
стрелок на клавиатуре (←, →, ↑, ↓).";

            helpText = helpText.Replace(" ", "\u00A0");

            MessageBox.Show(
                helpText,
                "Справка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

    }
}
