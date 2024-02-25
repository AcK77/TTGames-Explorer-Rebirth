namespace TTGamesExplorerRebirthUI.Forms
{
    partial class DATForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DATForm));
            darkListView1 = new DarkUI.Controls.DarkListView();
            darkContextMenu1 = new DarkUI.Controls.DarkContextMenu();
            extractFilesToolStripMenuItem = new ToolStripMenuItem();
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            darkTreeView1 = new DarkUI.Controls.DarkTreeView();
            darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            extractAllToolStripMenuItem = new ToolStripMenuItem();
            folderBrowserDialog1 = new FolderBrowserDialog();
            darkContextMenu1.SuspendLayout();
            darkStatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            darkSectionPanel1.SuspendLayout();
            darkSectionPanel2.SuspendLayout();
            darkMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // darkListView1
            // 
            darkListView1.ContextMenuStrip = darkContextMenu1;
            darkListView1.Dock = DockStyle.Fill;
            darkListView1.Location = new Point(1, 25);
            darkListView1.MultiSelect = true;
            darkListView1.Name = "darkListView1";
            darkListView1.ShowIcons = true;
            darkListView1.Size = new Size(556, 418);
            darkListView1.TabIndex = 0;
            darkListView1.Text = "darkListView1";
            darkListView1.DoubleClick += DarkListView1_DoubleClick;
            // 
            // darkContextMenu1
            // 
            darkContextMenu1.BackColor = Color.FromArgb(60, 63, 65);
            darkContextMenu1.ForeColor = Color.FromArgb(220, 220, 220);
            darkContextMenu1.Items.AddRange(new ToolStripItem[] { extractFilesToolStripMenuItem });
            darkContextMenu1.Name = "darkContextMenu1";
            darkContextMenu1.Size = new Size(151, 26);
            darkContextMenu1.Opening += DarkContextMenu1_Opening;
            // 
            // extractFilesToolStripMenuItem
            // 
            extractFilesToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            extractFilesToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            extractFilesToolStripMenuItem.Name = "extractFilesToolStripMenuItem";
            extractFilesToolStripMenuItem.Size = new Size(150, 22);
            extractFilesToolStripMenuItem.Text = "Extract file(s)...";
            extractFilesToolStripMenuItem.Click += ExtractFilesToolStripMenuItem_Click;
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 468);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(806, 24);
            darkStatusStrip1.SizingGrip = false;
            darkStatusStrip1.TabIndex = 1;
            darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Margin = new Padding(0, 0, 0, 2);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(10, 14);
            toolStripStatusLabel1.Text = " ";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(darkSectionPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(darkSectionPanel2);
            splitContainer1.Size = new Size(806, 444);
            splitContainer1.SplitterDistance = 244;
            splitContainer1.TabIndex = 2;
            // 
            // darkSectionPanel1
            // 
            darkSectionPanel1.Controls.Add(darkTreeView1);
            darkSectionPanel1.Dock = DockStyle.Fill;
            darkSectionPanel1.Location = new Point(0, 0);
            darkSectionPanel1.Name = "darkSectionPanel1";
            darkSectionPanel1.SectionHeader = "Folder(s):";
            darkSectionPanel1.Size = new Size(244, 444);
            darkSectionPanel1.TabIndex = 4;
            // 
            // darkTreeView1
            // 
            darkTreeView1.Dock = DockStyle.Fill;
            darkTreeView1.Location = new Point(1, 25);
            darkTreeView1.MaxDragChange = 20;
            darkTreeView1.Name = "darkTreeView1";
            darkTreeView1.ShowIcons = true;
            darkTreeView1.Size = new Size(242, 418);
            darkTreeView1.TabIndex = 3;
            darkTreeView1.Text = "darkTreeView1";
            darkTreeView1.SelectedNodesChanged += DarkTreeView1_SelectedNodesChanged;
            // 
            // darkSectionPanel2
            // 
            darkSectionPanel2.Controls.Add(darkListView1);
            darkSectionPanel2.Dock = DockStyle.Fill;
            darkSectionPanel2.Location = new Point(0, 0);
            darkSectionPanel2.Name = "darkSectionPanel2";
            darkSectionPanel2.SectionHeader = "File(s):";
            darkSectionPanel2.Size = new Size(558, 444);
            darkSectionPanel2.TabIndex = 1;
            // 
            // darkMenuStrip1
            // 
            darkMenuStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkMenuStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkMenuStrip1.Items.AddRange(new ToolStripItem[] { extractAllToolStripMenuItem });
            darkMenuStrip1.Location = new Point(0, 0);
            darkMenuStrip1.Name = "darkMenuStrip1";
            darkMenuStrip1.Padding = new Padding(3, 2, 0, 2);
            darkMenuStrip1.Size = new Size(806, 24);
            darkMenuStrip1.TabIndex = 3;
            darkMenuStrip1.Text = "darkMenuStrip1";
            // 
            // extractAllToolStripMenuItem
            // 
            extractAllToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            extractAllToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            extractAllToolStripMenuItem.Image = Properties.Resources.disk_multiple;
            extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            extractAllToolStripMenuItem.Size = new Size(87, 20);
            extractAllToolStripMenuItem.Text = "Extract All";
            extractAllToolStripMenuItem.Click += ExtractAllToolStripMenuItem_Click;
            // 
            // DATForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(806, 492);
            Controls.Add(splitContainer1);
            Controls.Add(darkMenuStrip1);
            Controls.Add(darkStatusStrip1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = darkMenuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DATForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "DAT Archive";
            Load += DATForm_Load;
            darkContextMenu1.ResumeLayout(false);
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            darkSectionPanel1.ResumeLayout(false);
            darkSectionPanel2.ResumeLayout(false);
            darkMenuStrip1.ResumeLayout(false);
            darkMenuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DarkUI.Controls.DarkListView darkListView1;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private SplitContainer splitContainer1;
        private DarkUI.Controls.DarkTreeView darkTreeView1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkContextMenu darkContextMenu1;
        private ToolStripMenuItem extractFilesToolStripMenuItem;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}