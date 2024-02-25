namespace TTGamesExplorerRebirthUI.Forms
{
    partial class PAKForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PAKForm));
            darkListView1 = new DarkUI.Controls.DarkListView();
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            extractAllToolStripMenuItem = new ToolStripMenuItem();
            folderBrowserDialog1 = new FolderBrowserDialog();
            darkContextMenu1 = new DarkUI.Controls.DarkContextMenu();
            extractFilesToolStripMenuItem = new ToolStripMenuItem();
            darkStatusStrip1.SuspendLayout();
            darkSectionPanel2.SuspendLayout();
            darkMenuStrip1.SuspendLayout();
            darkContextMenu1.SuspendLayout();
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
            darkListView1.Size = new Size(640, 284);
            darkListView1.TabIndex = 0;
            darkListView1.Text = "darkListView1";
            darkListView1.DoubleClick += DarkListView1_DoubleClick;
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 334);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(642, 24);
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
            // darkSectionPanel2
            // 
            darkSectionPanel2.Controls.Add(darkListView1);
            darkSectionPanel2.Dock = DockStyle.Fill;
            darkSectionPanel2.Location = new Point(0, 24);
            darkSectionPanel2.Name = "darkSectionPanel2";
            darkSectionPanel2.SectionHeader = "File(s):";
            darkSectionPanel2.Size = new Size(642, 310);
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
            darkMenuStrip1.Size = new Size(642, 24);
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
            // darkContextMenu1
            // 
            darkContextMenu1.BackColor = Color.FromArgb(60, 63, 65);
            darkContextMenu1.ForeColor = Color.FromArgb(220, 220, 220);
            darkContextMenu1.Items.AddRange(new ToolStripItem[] { extractFilesToolStripMenuItem });
            darkContextMenu1.Name = "darkContextMenu1";
            darkContextMenu1.Size = new Size(181, 48);
            darkContextMenu1.Opening += darkContextMenu1_Opening;
            // 
            // extractFilesToolStripMenuItem
            // 
            extractFilesToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            extractFilesToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            extractFilesToolStripMenuItem.Name = "extractFilesToolStripMenuItem";
            extractFilesToolStripMenuItem.Size = new Size(180, 22);
            extractFilesToolStripMenuItem.Text = "Extract file(s)...";
            extractFilesToolStripMenuItem.Click += extractFilesToolStripMenuItem_Click;
            // 
            // PAKForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 358);
            Controls.Add(darkSectionPanel2);
            Controls.Add(darkMenuStrip1);
            Controls.Add(darkStatusStrip1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = darkMenuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PAKForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "PAK Archive";
            Load += DATForm_Load;
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            darkSectionPanel2.ResumeLayout(false);
            darkMenuStrip1.ResumeLayout(false);
            darkMenuStrip1.PerformLayout();
            darkContextMenu1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DarkUI.Controls.DarkListView darkListView1;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private FolderBrowserDialog folderBrowserDialog1;
        private DarkUI.Controls.DarkContextMenu darkContextMenu1;
        private ToolStripMenuItem extractFilesToolStripMenuItem;
    }
}