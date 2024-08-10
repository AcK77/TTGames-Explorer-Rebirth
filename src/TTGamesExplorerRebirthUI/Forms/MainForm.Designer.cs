namespace TTGamesExplorerRebirthUI.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadGameFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            darkTreeView1 = new DarkUI.Controls.DarkTreeView();
            pictureBox1 = new PictureBox();
            darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            darkListView1 = new DarkUI.Controls.DarkListView();
            darkContextMenu1 = new DarkUI.Controls.DarkContextMenu();
            extractToolStripMenuItem = new ToolStripMenuItem();
            folderBrowserDialog1 = new FolderBrowserDialog();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            openFileDialog1 = new OpenFileDialog();
            darkStatusStrip1.SuspendLayout();
            darkMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            darkSectionPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            darkSectionPanel2.SuspendLayout();
            darkContextMenu1.SuspendLayout();
            SuspendLayout();
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 739);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(1127, 24);
            darkStatusStrip1.SizingGrip = false;
            darkStatusStrip1.TabIndex = 0;
            darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Margin = new Padding(0, -1, 0, 2);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(39, 15);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // darkMenuStrip1
            // 
            darkMenuStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkMenuStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkMenuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem });
            darkMenuStrip1.Location = new Point(0, 0);
            darkMenuStrip1.Name = "darkMenuStrip1";
            darkMenuStrip1.Padding = new Padding(3, 2, 0, 2);
            darkMenuStrip1.Size = new Size(1127, 24);
            darkMenuStrip1.TabIndex = 1;
            darkMenuStrip1.Text = "darkMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, loadGameFolderToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            fileToolStripMenuItem.Image = Properties.Resources.page;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(53, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadGameFolderToolStripMenuItem
            // 
            loadGameFolderToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            loadGameFolderToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            loadGameFolderToolStripMenuItem.Image = Properties.Resources.folder_brick;
            loadGameFolderToolStripMenuItem.Name = "loadGameFolderToolStripMenuItem";
            loadGameFolderToolStripMenuItem.Size = new Size(180, 22);
            loadGameFolderToolStripMenuItem.Text = "Load game folder...";
            loadGameFolderToolStripMenuItem.Click += LoadGameFolderToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.BackColor = Color.FromArgb(60, 63, 65);
            toolStripSeparator1.ForeColor = Color.FromArgb(220, 220, 220);
            toolStripSeparator1.Margin = new Padding(0, 0, 0, 1);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            exitToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            exitToolStripMenuItem.Image = Properties.Resources.cancel;
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            aboutToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            aboutToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            aboutToolStripMenuItem.Image = Properties.Resources.brick_icon;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(68, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.TextImageRelation = TextImageRelation.TextBeforeImage;
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(darkSectionPanel2);
            splitContainer1.Size = new Size(1127, 715);
            splitContainer1.SplitterDistance = 240;
            splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(darkSectionPanel1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(pictureBox1);
            splitContainer2.Panel2Collapsed = true;
            splitContainer2.Size = new Size(240, 715);
            splitContainer2.SplitterDistance = 401;
            splitContainer2.TabIndex = 1;
            // 
            // darkSectionPanel1
            // 
            darkSectionPanel1.Controls.Add(darkTreeView1);
            darkSectionPanel1.Dock = DockStyle.Fill;
            darkSectionPanel1.Location = new Point(0, 0);
            darkSectionPanel1.Name = "darkSectionPanel1";
            darkSectionPanel1.SectionHeader = "Folder(s)";
            darkSectionPanel1.Size = new Size(240, 715);
            darkSectionPanel1.TabIndex = 0;
            // 
            // darkTreeView1
            // 
            darkTreeView1.Dock = DockStyle.Fill;
            darkTreeView1.Location = new Point(1, 25);
            darkTreeView1.MaxDragChange = 20;
            darkTreeView1.Name = "darkTreeView1";
            darkTreeView1.ShowIcons = true;
            darkTreeView1.Size = new Size(238, 689);
            darkTreeView1.TabIndex = 0;
            darkTreeView1.Text = "darkTreeView1";
            darkTreeView1.SelectedNodesChanged += DarkTreeView1_SelectedNodesChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(150, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // darkSectionPanel2
            // 
            darkSectionPanel2.Controls.Add(darkListView1);
            darkSectionPanel2.Dock = DockStyle.Fill;
            darkSectionPanel2.Location = new Point(0, 0);
            darkSectionPanel2.Name = "darkSectionPanel2";
            darkSectionPanel2.SectionHeader = "File(s)";
            darkSectionPanel2.Size = new Size(883, 715);
            darkSectionPanel2.TabIndex = 1;
            // 
            // darkListView1
            // 
            darkListView1.ContextMenuStrip = darkContextMenu1;
            darkListView1.Dock = DockStyle.Fill;
            darkListView1.Location = new Point(1, 25);
            darkListView1.Name = "darkListView1";
            darkListView1.ShowIcons = true;
            darkListView1.Size = new Size(881, 689);
            darkListView1.TabIndex = 0;
            darkListView1.Text = "darkListView1";
            darkListView1.DoubleClick += DarkListView1_DoubleClick;
            // 
            // darkContextMenu1
            // 
            darkContextMenu1.BackColor = Color.FromArgb(60, 63, 65);
            darkContextMenu1.ForeColor = Color.FromArgb(220, 220, 220);
            darkContextMenu1.Items.AddRange(new ToolStripItem[] { extractToolStripMenuItem });
            darkContextMenu1.Name = "darkContextMenu1";
            darkContextMenu1.Size = new Size(110, 26);
            darkContextMenu1.Opening += DarkContextMenu1_Opening;
            // 
            // extractToolStripMenuItem
            // 
            extractToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            extractToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            extractToolStripMenuItem.Size = new Size(109, 22);
            extractToolStripMenuItem.Text = "Extract";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            openFileToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            openFileToolStripMenuItem.Image = Properties.Resources.page_go;
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(180, 22);
            openFileToolStripMenuItem.Text = "Open file...";
            openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "All files|*.*";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1127, 763);
            Controls.Add(splitContainer1);
            Controls.Add(darkStatusStrip1);
            Controls.Add(darkMenuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = darkMenuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TTGames Explorer Rebirth v0.1";
            Load += MainForm_Load;
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            darkMenuStrip1.ResumeLayout(false);
            darkMenuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            darkSectionPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            darkSectionPanel2.ResumeLayout(false);
            darkContextMenu1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadGameFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private SplitContainer splitContainer1;
        private DarkUI.Controls.DarkListView darkListView1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DarkUI.Controls.DarkTreeView darkTreeView1;
        private FolderBrowserDialog folderBrowserDialog1;
        private SplitContainer splitContainer2;
        private PictureBox pictureBox1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkContextMenu darkContextMenu1;
        private ToolStripMenuItem extractToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
    }
}
