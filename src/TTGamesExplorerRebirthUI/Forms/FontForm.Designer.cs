namespace TTGamesExplorerRebirthUI.Forms
{
    partial class FontForm
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
            splitContainer1 = new SplitContainer();
            darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            pictureBox1 = new PictureBox();
            darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            darkButton2 = new DarkUI.Controls.DarkButton();
            darkLabel2 = new DarkUI.Controls.DarkLabel();
            darkLabel1 = new DarkUI.Controls.DarkLabel();
            darkLabel3 = new DarkUI.Controls.DarkLabel();
            trackBar1 = new TrackBar();
            darkComboBox1 = new DarkUI.Controls.DarkComboBox();
            darkCheckBox1 = new DarkUI.Controls.DarkCheckBox();
            saveFileDialog1 = new SaveFileDialog();
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            darkSectionPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            darkSectionPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            darkStatusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(darkSectionPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(darkSectionPanel2);
            splitContainer1.Size = new Size(769, 587);
            splitContainer1.SplitterDistance = 467;
            splitContainer1.TabIndex = 1;
            // 
            // darkSectionPanel1
            // 
            darkSectionPanel1.Controls.Add(flowLayoutPanel1);
            darkSectionPanel1.Dock = DockStyle.Fill;
            darkSectionPanel1.Location = new Point(0, 0);
            darkSectionPanel1.Name = "darkSectionPanel1";
            darkSectionPanel1.SectionHeader = "Preview:";
            darkSectionPanel1.Size = new Size(769, 467);
            darkSectionPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackgroundImage = Properties.Resources.transparent_background;
            flowLayoutPanel1.Controls.Add(pictureBox1);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(1, 25);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(767, 441);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(767, 485);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // darkSectionPanel2
            // 
            darkSectionPanel2.Controls.Add(darkButton2);
            darkSectionPanel2.Controls.Add(darkLabel2);
            darkSectionPanel2.Controls.Add(darkLabel1);
            darkSectionPanel2.Controls.Add(darkLabel3);
            darkSectionPanel2.Controls.Add(trackBar1);
            darkSectionPanel2.Controls.Add(darkComboBox1);
            darkSectionPanel2.Controls.Add(darkCheckBox1);
            darkSectionPanel2.Dock = DockStyle.Fill;
            darkSectionPanel2.Location = new Point(0, 0);
            darkSectionPanel2.Name = "darkSectionPanel2";
            darkSectionPanel2.SectionHeader = "Options:";
            darkSectionPanel2.Size = new Size(769, 116);
            darkSectionPanel2.TabIndex = 7;
            // 
            // darkButton2
            // 
            darkButton2.Location = new Point(333, 34);
            darkButton2.Name = "darkButton2";
            darkButton2.Padding = new Padding(5);
            darkButton2.Size = new Size(151, 23);
            darkButton2.TabIndex = 19;
            darkButton2.Text = "Extract to DDS...";
            darkButton2.Click += DarkButton2_Click;
            // 
            // darkLabel2
            // 
            darkLabel2.AutoSize = true;
            darkLabel2.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel2.Location = new Point(12, 34);
            darkLabel2.Name = "darkLabel2";
            darkLabel2.Size = new Size(48, 15);
            darkLabel2.TabIndex = 18;
            darkLabel2.Text = "Char(s):";
            // 
            // darkLabel1
            // 
            darkLabel1.AutoSize = true;
            darkLabel1.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel1.Location = new Point(230, 34);
            darkLabel1.Name = "darkLabel1";
            darkLabel1.Size = new Size(10, 15);
            darkLabel1.TabIndex = 17;
            darkLabel1.Text = " ";
            // 
            // darkLabel3
            // 
            darkLabel3.AutoSize = true;
            darkLabel3.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel3.Location = new Point(182, 34);
            darkLabel3.Name = "darkLabel3";
            darkLabel3.Size = new Size(42, 15);
            darkLabel3.TabIndex = 16;
            darkLabel3.Text = "Zoom:";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(176, 52);
            trackBar1.Maximum = 200;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(151, 45);
            trackBar1.TabIndex = 15;
            trackBar1.TickFrequency = 10;
            trackBar1.TickStyle = TickStyle.Both;
            trackBar1.Value = 100;
            trackBar1.Scroll += TrackBar1_Scroll;
            // 
            // darkComboBox1
            // 
            darkComboBox1.DrawMode = DrawMode.OwnerDrawVariable;
            darkComboBox1.FormattingEnabled = true;
            darkComboBox1.Location = new Point(14, 52);
            darkComboBox1.MaxDropDownItems = 10;
            darkComboBox1.Name = "darkComboBox1";
            darkComboBox1.Size = new Size(149, 24);
            darkComboBox1.TabIndex = 14;
            // 
            // darkCheckBox1
            // 
            darkCheckBox1.AutoSize = true;
            darkCheckBox1.Location = new Point(14, 84);
            darkCheckBox1.Name = "darkCheckBox1";
            darkCheckBox1.Size = new Size(101, 19);
            darkCheckBox1.TabIndex = 13;
            darkCheckBox1.Text = "Show all chars";
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 587);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(769, 24);
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
            // FontForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(769, 611);
            Controls.Add(splitContainer1);
            Controls.Add(darkStatusStrip1);
            DoubleBuffered = true;
            Name = "FontForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Font File Viewer";
            Load += FontForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            darkSectionPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            darkSectionPanel2.ResumeLayout(false);
            darkSectionPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private PictureBox pictureBox1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private FlowLayoutPanel flowLayoutPanel1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private TrackBar trackBar1;
        private DarkUI.Controls.DarkComboBox darkComboBox1;
        private DarkUI.Controls.DarkCheckBox darkCheckBox1;
        private SaveFileDialog saveFileDialog1;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}