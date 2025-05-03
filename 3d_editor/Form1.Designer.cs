namespace _3d_editor
{
    partial class CFE
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CFE));
            OpenGL_Window = new OpenGL_Window(components);
            timer = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            ToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            cameraToolStripMenuItem = new ToolStripMenuItem();
            resetCameraToolStripMenuItem = new ToolStripMenuItem();
            changeControlToolStripMenuItem = new ToolStripMenuItem();
            infoToolStripMenuItem = new ToolStripMenuItem();
            controlToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            panel2 = new Panel();
            textBox1 = new TextBox();
            numericUpDown1 = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // OpenGL_Window
            // 
            OpenGL_Window.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            OpenGL_Window.APIVersion = new Version(3, 3, 0, 0);
            OpenGL_Window.Dock = DockStyle.Fill;
            OpenGL_Window.Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible;
            OpenGL_Window.IsEventDriven = true;
            OpenGL_Window.Location = new Point(0, 28);
            OpenGL_Window.Name = "OpenGL_Window";
            OpenGL_Window.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            OpenGL_Window.SharedContext = null;
            OpenGL_Window.Size = new Size(1017, 612);
            OpenGL_Window.TabIndex = 1;
            OpenGL_Window.Load += OpenGL_Window_Load;
            OpenGL_Window.Click += OpenGL_Window_Click;
            OpenGL_Window.Paint += OpenGL_Window_Paint;
            OpenGL_Window.KeyDown += OpenGL_Window_KeyDown;
            OpenGL_Window.KeyUp += OpenGL_Window_KeyUp;
            OpenGL_Window.MouseDown += OpenGL_Window_MouseDown;
            OpenGL_Window.MouseMove += OpenGL_Window_MouseMove;
            OpenGL_Window.MouseUp += OpenGL_Window_MouseUp;
            OpenGL_Window.PreviewKeyDown += OpenGL_Window_PreviewKeyDown;
            OpenGL_Window.Resize += OpenGL_Window_Resize;
            // 
            // timer
            // 
            timer.Tick += Timer_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, cameraToolStripMenuItem, infoToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1017, 28);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItem, saveToolStripMenuItem, exitToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(59, 24);
            toolStripMenuItem1.Text = "Файл";
            // 
            // ToolStripMenuItem
            // 
            ToolStripMenuItem.Name = "ToolStripMenuItem";
            ToolStripMenuItem.Size = new Size(150, 26);
            ToolStripMenuItem.Text = "Открыть";
            ToolStripMenuItem.Click += ToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(150, 26);
            saveToolStripMenuItem.Text = "Save";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(150, 26);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // cameraToolStripMenuItem
            // 
            cameraToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { resetCameraToolStripMenuItem, changeControlToolStripMenuItem });
            cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            cameraToolStripMenuItem.Size = new Size(74, 24);
            cameraToolStripMenuItem.Text = "Camera";
            // 
            // resetCameraToolStripMenuItem
            // 
            resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
            resetCameraToolStripMenuItem.Size = new Size(191, 26);
            resetCameraToolStripMenuItem.Text = "ResetCamera";
            // 
            // changeControlToolStripMenuItem
            // 
            changeControlToolStripMenuItem.Name = "changeControlToolStripMenuItem";
            changeControlToolStripMenuItem.Size = new Size(191, 26);
            changeControlToolStripMenuItem.Text = "ChangeControl";
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { controlToolStripMenuItem, aboutToolStripMenuItem });
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new Size(49, 24);
            infoToolStripMenuItem.Text = "Info";
            // 
            // controlToolStripMenuItem
            // 
            controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            controlToolStripMenuItem.Size = new Size(141, 26);
            controlToolStripMenuItem.Text = "Control";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(141, 26);
            aboutToolStripMenuItem.Text = "About";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.AutoSize = true;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(flowLayoutPanel1);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(flowLayoutPanel2);
            panel1.Location = new Point(773, 28);
            panel1.Name = "panel1";
            panel1.Size = new Size(244, 164);
            panel1.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(0, 160);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(242, 0);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(numericUpDown1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 48);
            panel2.Name = "panel2";
            panel2.Size = new Size(242, 112);
            panel2.TabIndex = 3;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(121, 61);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(93, 27);
            textBox1.TabIndex = 3;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Location = new Point(121, 24);
            numericUpDown1.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(93, 27);
            numericUpDown1.TabIndex = 2;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(34, 64);
            label2.Name = "label2";
            label2.Size = new Size(81, 20);
            label2.TabIndex = 1;
            label2.Text = "Шаг сетки:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(2, 26);
            label1.Name = "label1";
            label1.Size = new Size(113, 20);
            label1.TabIndex = 0;
            label1.Text = "Уровень сетки:";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel2.Controls.Add(button1);
            flowLayoutPanel2.Controls.Add(button2);
            flowLayoutPanel2.Controls.Add(button3);
            flowLayoutPanel2.Controls.Add(button4);
            flowLayoutPanel2.Controls.Add(button5);
            flowLayoutPanel2.Dock = DockStyle.Top;
            flowLayoutPanel2.Location = new Point(0, 0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(242, 48);
            flowLayoutPanel2.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackgroundImage = Properties.Resources.icons8_курсор_50;
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(40, 40);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackgroundImage = Properties.Resources.icons8_перемещение_100;
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.Location = new Point(49, 3);
            button2.Name = "button2";
            button2.Size = new Size(40, 40);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackgroundImage = Properties.Resources.free_icon_circle_14448782;
            button3.BackgroundImageLayout = ImageLayout.Zoom;
            button3.Location = new Point(95, 3);
            button3.Name = "button3";
            button3.Size = new Size(40, 40);
            button3.TabIndex = 2;
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.BackgroundImage = Properties.Resources.free_icon_cylinder_8726474;
            button4.BackgroundImageLayout = ImageLayout.Zoom;
            button4.Location = new Point(141, 3);
            button4.Name = "button4";
            button4.Size = new Size(40, 40);
            button4.TabIndex = 3;
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.BackgroundImage = Properties.Resources.free_icon_cross_7699001;
            button5.BackgroundImageLayout = ImageLayout.Zoom;
            button5.Location = new Point(187, 3);
            button5.Name = "button5";
            button5.Size = new Size(40, 40);
            button5.TabIndex = 4;
            button5.UseVisualStyleBackColor = true;
            // 
            // CFE
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 640);
            Controls.Add(panel1);
            Controls.Add(OpenGL_Window);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "CFE";
            Text = "CFE";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenGL_Window OpenGL_Window;
        private System.Windows.Forms.Timer timer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem ToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem cameraToolStripMenuItem;
        private ToolStripMenuItem resetCameraToolStripMenuItem;
        private ToolStripMenuItem changeControlToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStripMenuItem controlToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button button1;
        private Button button2;
        private Panel panel2;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private NumericUpDown numericUpDown1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button button3;
        private Button button4;
        private Button button5;
    }
}
