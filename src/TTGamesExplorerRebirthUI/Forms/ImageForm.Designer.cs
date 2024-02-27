namespace TTGamesExplorerRebirthUI.Forms
{
    partial class ImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageForm));
            splitContainer1 = new SplitContainer();
            darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            pictureBox1 = new PictureBox();
            darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            splitContainer2 = new SplitContainer();
            darkSectionPanel4 = new DarkUI.Controls.DarkSectionPanel();
            darkListView2 = new DarkUI.Controls.DarkListView();
            darkSectionPanel3 = new DarkUI.Controls.DarkSectionPanel();
            darkListView1 = new DarkUI.Controls.DarkListView();
            darkButton3 = new DarkUI.Controls.DarkButton();
            darkButton2 = new DarkUI.Controls.DarkButton();
            darkButton1 = new DarkUI.Controls.DarkButton();
            darkLabel1 = new DarkUI.Controls.DarkLabel();
            darkLabel3 = new DarkUI.Controls.DarkLabel();
            trackBar1 = new TrackBar();
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog2 = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            darkSectionPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            darkSectionPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            darkSectionPanel4.SuspendLayout();
            darkSectionPanel3.SuspendLayout();
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
            splitContainer1.Size = new Size(842, 533);
            splitContainer1.SplitterDistance = 324;
            splitContainer1.TabIndex = 0;
            // 
            // darkSectionPanel1
            // 
            darkSectionPanel1.Controls.Add(flowLayoutPanel1);
            darkSectionPanel1.Dock = DockStyle.Fill;
            darkSectionPanel1.Location = new Point(0, 0);
            darkSectionPanel1.Name = "darkSectionPanel1";
            darkSectionPanel1.SectionHeader = "Preview:";
            darkSectionPanel1.Size = new Size(842, 324);
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
            flowLayoutPanel1.Size = new Size(840, 298);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(840, 298);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // darkSectionPanel2
            // 
            darkSectionPanel2.Controls.Add(splitContainer2);
            darkSectionPanel2.Dock = DockStyle.Fill;
            darkSectionPanel2.Location = new Point(0, 0);
            darkSectionPanel2.Name = "darkSectionPanel2";
            darkSectionPanel2.SectionHeader = "Options:";
            darkSectionPanel2.Size = new Size(842, 205);
            darkSectionPanel2.TabIndex = 7;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(1, 25);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(darkSectionPanel4);
            splitContainer2.Panel1.Controls.Add(darkSectionPanel3);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(darkButton3);
            splitContainer2.Panel2.Controls.Add(darkButton2);
            splitContainer2.Panel2.Controls.Add(darkButton1);
            splitContainer2.Panel2.Controls.Add(darkLabel1);
            splitContainer2.Panel2.Controls.Add(darkLabel3);
            splitContainer2.Panel2.Controls.Add(trackBar1);
            splitContainer2.Size = new Size(840, 179);
            splitContainer2.SplitterDistance = 457;
            splitContainer2.TabIndex = 7;
            // 
            // darkSectionPanel4
            // 
            darkSectionPanel4.Controls.Add(darkListView2);
            darkSectionPanel4.Dock = DockStyle.Left;
            darkSectionPanel4.Location = new Point(292, 0);
            darkSectionPanel4.Name = "darkSectionPanel4";
            darkSectionPanel4.SectionHeader = "MipMap(s)";
            darkSectionPanel4.Size = new Size(162, 179);
            darkSectionPanel4.TabIndex = 1;
            // 
            // darkListView2
            // 
            darkListView2.Dock = DockStyle.Fill;
            darkListView2.Location = new Point(1, 25);
            darkListView2.Name = "darkListView2";
            darkListView2.ShowIcons = true;
            darkListView2.Size = new Size(160, 153);
            darkListView2.TabIndex = 0;
            darkListView2.Text = "darkListView2";
            darkListView2.SelectedIndicesChanged += DarkListView2_SelectedIndicesChanged;
            // 
            // darkSectionPanel3
            // 
            darkSectionPanel3.Controls.Add(darkListView1);
            darkSectionPanel3.Dock = DockStyle.Left;
            darkSectionPanel3.Location = new Point(0, 0);
            darkSectionPanel3.Name = "darkSectionPanel3";
            darkSectionPanel3.SectionHeader = "Image(s)";
            darkSectionPanel3.Size = new Size(292, 179);
            darkSectionPanel3.TabIndex = 0;
            // 
            // darkListView1
            // 
            darkListView1.Dock = DockStyle.Fill;
            darkListView1.Location = new Point(1, 25);
            darkListView1.Name = "darkListView1";
            darkListView1.ShowIcons = true;
            darkListView1.Size = new Size(290, 153);
            darkListView1.TabIndex = 0;
            darkListView1.Text = "darkListView1";
            darkListView1.SelectedIndicesChanged += DarkListView1_SelectedIndicesChanged;
            // 
            // darkButton3
            // 
            darkButton3.Anchor = AnchorStyles.None;
            darkButton3.Location = new Point(3, 68);
            darkButton3.Name = "darkButton3";
            darkButton3.Padding = new Padding(5);
            darkButton3.Size = new Size(174, 32);
            darkButton3.TabIndex = 10;
            darkButton3.Text = "Toggle Background";
            darkButton3.Click += DarkButton3_Click;
            // 
            // darkButton2
            // 
            darkButton2.Image = Properties.Resources.disk;
            darkButton2.Location = new Point(3, 106);
            darkButton2.Name = "darkButton2";
            darkButton2.Padding = new Padding(5);
            darkButton2.Size = new Size(174, 32);
            darkButton2.TabIndex = 9;
            darkButton2.Text = "Extract to DDS...";
            darkButton2.TextImageRelation = TextImageRelation.ImageBeforeText;
            darkButton2.Visible = false;
            darkButton2.Click += DarkButton2_Click;
            // 
            // darkButton1
            // 
            darkButton1.Image = Properties.Resources.disk;
            darkButton1.Location = new Point(3, 144);
            darkButton1.Name = "darkButton1";
            darkButton1.Padding = new Padding(5);
            darkButton1.Size = new Size(174, 32);
            darkButton1.TabIndex = 8;
            darkButton1.Text = "Save as PNG...";
            darkButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            darkButton1.Click += DarkButton1_Click;
            // 
            // darkLabel1
            // 
            darkLabel1.AutoSize = true;
            darkLabel1.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel1.Location = new Point(44, 7);
            darkLabel1.Name = "darkLabel1";
            darkLabel1.Size = new Size(35, 15);
            darkLabel1.TabIndex = 7;
            darkLabel1.Text = "100%";
            // 
            // darkLabel3
            // 
            darkLabel3.AutoSize = true;
            darkLabel3.ForeColor = Color.FromArgb(220, 220, 220);
            darkLabel3.Location = new Point(3, 7);
            darkLabel3.Name = "darkLabel3";
            darkLabel3.Size = new Size(42, 15);
            darkLabel3.TabIndex = 6;
            darkLabel3.Text = "Zoom:";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(3, 25);
            trackBar1.Maximum = 200;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(174, 45);
            trackBar1.TabIndex = 1;
            trackBar1.TickFrequency = 10;
            trackBar1.TickStyle = TickStyle.Both;
            trackBar1.Value = 100;
            trackBar1.Scroll += TrackBar1_Scroll;
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 533);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(842, 24);
            darkStatusStrip1.SizingGrip = false;
            darkStatusStrip1.TabIndex = 1;
            darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Margin = new Padding(0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(10, 16);
            toolStripStatusLabel1.Text = " ";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.RestoreDirectory = true;
            // 
            // saveFileDialog2
            // 
            saveFileDialog2.DefaultExt = "png";
            saveFileDialog2.Filter = "Portable Network Graphic files (*.png)|*.png";
            saveFileDialog2.RestoreDirectory = true;
            saveFileDialog2.Title = "Save as PNG";
            // 
            // ImageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(842, 557);
            Controls.Add(splitContainer1);
            Controls.Add(darkStatusStrip1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ImageForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Image / Texture Viewer";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            darkSectionPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            darkSectionPanel2.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            darkSectionPanel4.ResumeLayout(false);
            darkSectionPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private PictureBox pictureBox1;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TrackBar trackBar1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private SplitContainer splitContainer2;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel4;
        private DarkUI.Controls.DarkListView darkListView2;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel3;
        private DarkUI.Controls.DarkListView darkListView1;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton darkButton1;
        private SaveFileDialog saveFileDialog1;
        private DarkUI.Controls.DarkButton darkButton2;
        private SaveFileDialog saveFileDialog2;
        private FlowLayoutPanel flowLayoutPanel1;
        private DarkUI.Controls.DarkButton darkButton3;
    }
}