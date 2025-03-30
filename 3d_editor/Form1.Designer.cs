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
            SuspendLayout();
            // 
            // OpenGL_Window
            // 
            OpenGL_Window.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            OpenGL_Window.APIVersion = new Version(3, 3, 0, 0);
            OpenGL_Window.Dock = DockStyle.Fill;
            OpenGL_Window.Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible;
            OpenGL_Window.IsEventDriven = true;
            OpenGL_Window.Location = new Point(0, 0);
            OpenGL_Window.Name = "OpenGL_Window";
            OpenGL_Window.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            OpenGL_Window.SharedContext = null;
            OpenGL_Window.Size = new Size(1017, 640);
            OpenGL_Window.TabIndex = 1;
            OpenGL_Window.Load += OpenGL_Window_Load;
            OpenGL_Window.Click += OpenGL_Window_Click;
            OpenGL_Window.Paint += OpenGL_Window_Paint;
            OpenGL_Window.Enter += OpenGL_Window_Enter;
            OpenGL_Window.KeyDown += OpenGL_Window_KeyDown;
            OpenGL_Window.KeyUp += OpenGL_Window_KeyUp;
            OpenGL_Window.Leave += OpenGL_Window_Leave;
            OpenGL_Window.PreviewKeyDown += OpenGL_Window_PreviewKeyDown;
            OpenGL_Window.Resize += OpenGL_Window_Resize;
            // 
            // timer
            // 
            timer.Tick += Timer_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 640);
            Controls.Add(OpenGL_Window);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private OpenGL_Window OpenGL_Window;
        private System.Windows.Forms.Timer timer;
    }
}
