namespace TTGamesExplorerRebirthUI.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            darkLabel1 = new DarkUI.Controls.DarkLabel();
            darkLabel2 = new DarkUI.Controls.DarkLabel();
            darkLabel3 = new DarkUI.Controls.DarkLabel();
            darkLabel4 = new DarkUI.Controls.DarkLabel();
            darkButton1 = new DarkUI.Controls.DarkButton();
            label1 = new Label();
            darkLabel5 = new DarkUI.Controls.DarkLabel();
            pictureBox1 = new PictureBox();
            darkLabel6 = new DarkUI.Controls.DarkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // darkLabel1
            // 
            darkLabel1.AutoSize = true;
            darkLabel1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            darkLabel1.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel1.Location = new Point(105, 12);
            darkLabel1.Name = "darkLabel1";
            darkLabel1.Size = new Size(220, 32);
            darkLabel1.TabIndex = 0;
            darkLabel1.Text = "TTGames Explorer";
            // 
            // darkLabel2
            // 
            darkLabel2.AutoSize = true;
            darkLabel2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            darkLabel2.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel2.Location = new Point(253, 44);
            darkLabel2.Name = "darkLabel2";
            darkLabel2.Size = new Size(72, 25);
            darkLabel2.TabIndex = 1;
            darkLabel2.Text = "Rebirth";
            // 
            // darkLabel3
            // 
            darkLabel3.AutoSize = true;
            darkLabel3.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel3.Location = new Point(344, 29);
            darkLabel3.Name = "darkLabel3";
            darkLabel3.Size = new Size(28, 15);
            darkLabel3.TabIndex = 2;
            darkLabel3.Text = "v0.1";
            // 
            // darkLabel4
            // 
            darkLabel4.AutoSize = true;
            darkLabel4.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel4.Location = new Point(344, 44);
            darkLabel4.Name = "darkLabel4";
            darkLabel4.Size = new Size(49, 15);
            darkLabel4.TabIndex = 3;
            darkLabel4.Text = "by Ac_K";
            // 
            // darkButton1
            // 
            darkButton1.Location = new Point(284, 189);
            darkButton1.Name = "darkButton1";
            darkButton1.Padding = new Padding(5);
            darkButton1.Size = new Size(110, 23);
            darkButton1.TabIndex = 4;
            darkButton1.Text = "OK";
            darkButton1.Click += DarkButton1_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Gainsboro;
            label1.Location = new Point(331, 21);
            label1.Name = "label1";
            label1.Size = new Size(1, 45);
            label1.TabIndex = 5;
            // 
            // darkLabel5
            // 
            darkLabel5.AutoSize = true;
            darkLabel5.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            darkLabel5.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel5.Location = new Point(12, 118);
            darkLabel5.Name = "darkLabel5";
            darkLabel5.Size = new Size(64, 15);
            darkLabel5.TabIndex = 6;
            darkLabel5.Text = "Thanks to:";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.head_ico;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(87, 93);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // darkLabel6
            // 
            darkLabel6.AutoSize = true;
            darkLabel6.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel6.Location = new Point(12, 143);
            darkLabel6.Name = "darkLabel6";
            darkLabel6.Size = new Size(10, 15);
            darkLabel6.TabIndex = 8;
            darkLabel6.Text = " ";
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 225);
            Controls.Add(darkLabel6);
            Controls.Add(pictureBox1);
            Controls.Add(darkLabel5);
            Controls.Add(label1);
            Controls.Add(darkButton1);
            Controls.Add(darkLabel4);
            Controls.Add(darkLabel3);
            Controls.Add(darkLabel2);
            Controls.Add(darkLabel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About...";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkButton darkButton1;
        private Label label1;
        private DarkUI.Controls.DarkLabel darkLabel5;
        private PictureBox pictureBox1;
        private DarkUI.Controls.DarkLabel darkLabel6;
    }
}