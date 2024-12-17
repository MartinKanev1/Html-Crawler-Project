namespace Html_Crawler_Final_version
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
            panel1 = new Panel();
            lblStatus = new Label();
            btn_clear = new Button();
            btn_execute = new Button();
            btn_vizualize = new Button();
            btn_save = new Button();
            btn_load = new Button();
            txtHtmlCode = new TextBox();
            txtCommand = new TextBox();
            PictureBox1 = new PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(lblStatus);
            panel1.Controls.Add(btn_clear);
            panel1.Controls.Add(btn_execute);
            panel1.Controls.Add(btn_vizualize);
            panel1.Controls.Add(btn_save);
            panel1.Controls.Add(btn_load);
            panel1.Controls.Add(txtHtmlCode);
            panel1.Controls.Add(txtCommand);
            panel1.Controls.Add(PictureBox1);
            panel1.Location = new Point(-2, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(1063, 688);
            panel1.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(48, 139);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(38, 15);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "status";
            // 
            // btn_clear
            // 
            btn_clear.Location = new Point(386, 105);
            btn_clear.Name = "btn_clear";
            btn_clear.Size = new Size(104, 23);
            btn_clear.TabIndex = 7;
            btn_clear.Text = "Clear";
            btn_clear.UseVisualStyleBackColor = true;
            btn_clear.Click += btn_clear_Click;
            // 
            // btn_execute
            // 
            btn_execute.Location = new Point(254, 105);
            btn_execute.Name = "btn_execute";
            btn_execute.Size = new Size(104, 23);
            btn_execute.TabIndex = 6;
            btn_execute.Text = "Execute command";
            btn_execute.UseVisualStyleBackColor = true;
            btn_execute.Click += btn_execute_Click;
            // 
            // btn_vizualize
            // 
            btn_vizualize.Location = new Point(766, 135);
            btn_vizualize.Name = "btn_vizualize";
            btn_vizualize.Size = new Size(104, 23);
            btn_vizualize.TabIndex = 5;
            btn_vizualize.Text = "Vizualize";
            btn_vizualize.UseVisualStyleBackColor = true;
            btn_vizualize.Click += btn_vizualize_Click;
            // 
            // btn_save
            // 
            btn_save.Location = new Point(193, 32);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(104, 23);
            btn_save.TabIndex = 4;
            btn_save.Text = "Save";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // btn_load
            // 
            btn_load.Location = new Point(48, 32);
            btn_load.Name = "btn_load";
            btn_load.Size = new Size(104, 23);
            btn_load.TabIndex = 3;
            btn_load.Text = "Load";
            btn_load.UseVisualStyleBackColor = true;
            btn_load.Click += btn_load_Click;
            // 
            // txtHtmlCode
            // 
            txtHtmlCode.Location = new Point(48, 173);
            txtHtmlCode.Multiline = true;
            txtHtmlCode.Name = "txtHtmlCode";
            txtHtmlCode.ScrollBars = ScrollBars.Vertical;
            txtHtmlCode.Size = new Size(353, 429);
            txtHtmlCode.TabIndex = 2;
            // 
            // txtCommand
            // 
            txtCommand.Location = new Point(48, 106);
            txtCommand.Name = "txtCommand";
            txtCommand.Size = new Size(176, 23);
            txtCommand.TabIndex = 1;
            // 
            // PictureBox1
            // 
            PictureBox1.Location = new Point(640, 173);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(356, 429);
            PictureBox1.TabIndex = 0;
            PictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 688);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btn_clear;
        private Button btn_execute;
        private Button btn_vizualize;
        private Button btn_save;
        private Button btn_load;
        public TextBox txtHtmlCode;
        private TextBox txtCommand;
        private PictureBox PictureBox1;
        private Label lblStatus;
    }
}
