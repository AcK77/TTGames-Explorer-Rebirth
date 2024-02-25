namespace TTGamesExplorerRebirthUI.Forms
{
    partial class SoundForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundForm));
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            button1 = new Button();
            button2 = new Button();
            trackBar1 = new TrackBar();
            timer1 = new System.Windows.Forms.Timer(components);
            darkButton1 = new DarkUI.Controls.DarkButton();
            saveFileDialog1 = new SaveFileDialog();
            darkStatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 88);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(397, 24);
            darkStatusStrip1.SizingGrip = false;
            darkStatusStrip1.TabIndex = 2;
            darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Margin = new Padding(0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(10, 16);
            toolStripStatusLabel1.Text = " ";
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(64, 64, 64);
            button1.FlatAppearance.BorderColor = Color.Gray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Image = Properties.Resources.stop_green;
            button1.Location = new Point(356, 12);
            button1.Name = "button1";
            button1.Size = new Size(32, 32);
            button1.TabIndex = 4;
            button1.UseVisualStyleBackColor = false;
            button1.Click += Button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(64, 64, 64);
            button2.FlatAppearance.BorderColor = Color.Gray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.White;
            button2.Image = Properties.Resources.play_green;
            button2.Location = new Point(9, 12);
            button2.Name = "button2";
            button2.Size = new Size(32, 32);
            button2.TabIndex = 5;
            button2.UseVisualStyleBackColor = false;
            button2.Click += Button2_Click;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(47, 19);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(305, 45);
            trackBar1.TabIndex = 6;
            trackBar1.TickStyle = TickStyle.None;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            // 
            // timer1
            // 
            timer1.Tick += Timer1_Tick;
            // 
            // darkButton1
            // 
            darkButton1.Enabled = false;
            darkButton1.Location = new Point(8, 57);
            darkButton1.Name = "darkButton1";
            darkButton1.Padding = new Padding(5);
            darkButton1.Size = new Size(97, 23);
            darkButton1.TabIndex = 7;
            darkButton1.Text = "Extract...";
            darkButton1.Click += darkButton1_Click;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "wav";
            saveFileDialog1.Filter = "WaveformAudio files (*.wav)|*.wav";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.Title = "Extract Audio File";
            // 
            // SoundForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(397, 112);
            Controls.Add(darkButton1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(darkStatusStrip1);
            Controls.Add(trackBar1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SoundForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Sound Player";
            FormClosing += SoundForm_FormClosing;
            Load += SoundForm_Load;
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button button1;
        private Button button2;
        private TrackBar trackBar1;
        private System.Windows.Forms.Timer timer1;
        private DarkUI.Controls.DarkButton darkButton1;
        private SaveFileDialog saveFileDialog1;
    }
}