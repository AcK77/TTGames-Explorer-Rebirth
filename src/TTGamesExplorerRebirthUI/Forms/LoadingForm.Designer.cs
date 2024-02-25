namespace TTGamesExplorerRebirthUI.Forms
{
    partial class LoadingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            progressBar1 = new ProgressBar();
            darkLabel1 = new DarkUI.Controls.DarkLabel();
            darkLabel2 = new DarkUI.Controls.DarkLabel();
            darkLabel3 = new DarkUI.Controls.DarkLabel();
            darkButton1 = new DarkUI.Controls.DarkButton();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 45);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(497, 14);
            progressBar1.TabIndex = 1;
            // 
            // darkLabel1
            // 
            darkLabel1.AutoSize = true;
            darkLabel1.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel1.Location = new Point(9, 18);
            darkLabel1.Name = "darkLabel1";
            darkLabel1.Size = new Size(64, 15);
            darkLabel1.TabIndex = 5;
            darkLabel1.Text = "darkLabel1";
            // 
            // darkLabel2
            // 
            darkLabel2.AutoSize = true;
            darkLabel2.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel2.Location = new Point(9, 71);
            darkLabel2.Name = "darkLabel2";
            darkLabel2.Size = new Size(64, 15);
            darkLabel2.TabIndex = 6;
            darkLabel2.Text = "darkLabel2";
            // 
            // darkLabel3
            // 
            darkLabel3.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel3.Location = new Point(448, 71);
            darkLabel3.Name = "darkLabel3";
            darkLabel3.RightToLeft = RightToLeft.No;
            darkLabel3.Size = new Size(64, 15);
            darkLabel3.TabIndex = 7;
            darkLabel3.Text = "darkLabel3";
            darkLabel3.TextAlign = ContentAlignment.TopRight;
            // 
            // darkButton1
            // 
            darkButton1.Location = new Point(405, 98);
            darkButton1.Name = "darkButton1";
            darkButton1.Padding = new Padding(5);
            darkButton1.Size = new Size(104, 23);
            darkButton1.TabIndex = 8;
            darkButton1.Text = "darkButton1";
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(521, 138);
            Controls.Add(darkButton1);
            Controls.Add(darkLabel3);
            Controls.Add(darkLabel2);
            Controls.Add(darkLabel1);
            Controls.Add(progressBar1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoadingForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "LoadingForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public ProgressBar progressBar1;
        public DarkUI.Controls.DarkLabel darkLabel1;
        public DarkUI.Controls.DarkLabel darkLabel2;
        public DarkUI.Controls.DarkLabel darkLabel3;
        public DarkUI.Controls.DarkButton darkButton1;
    }
}