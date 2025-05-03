namespace _3d_editor
{
    partial class Form1
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
            OpenGL_Window = new OpenGL_Window(components);
            timer = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            ToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            menuStrip1.SuspendLayout();
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
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1017, 28);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItem });
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
            // panel1
            // 
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 28);
            panel1.Name = "panel1";
            panel1.Size = new Size(285, 612);
            panel1.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 640);
            Controls.Add(panel1);
            Controls.Add(OpenGL_Window);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenGL_Window OpenGL_Window;
        private System.Windows.Forms.Timer timer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem ToolStripMenuItem;
        private Panel panel1;
    }
}
