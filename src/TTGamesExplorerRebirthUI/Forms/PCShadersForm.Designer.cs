namespace TTGamesExplorerRebirthUI.Forms
{
    partial class PCShadersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PCShadersForm));
            darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            extractAllToolStripMenuItem = new ToolStripMenuItem();
            darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            darkListView1 = new DarkUI.Controls.DarkListView();
            darkContextMenu1 = new DarkUI.Controls.DarkContextMenu();
            extractToolStripMenuItem = new ToolStripMenuItem();
            folderBrowserDialog1 = new FolderBrowserDialog();
            darkStatusStrip1.SuspendLayout();
            darkMenuStrip1.SuspendLayout();
            darkSectionPanel1.SuspendLayout();
            darkContextMenu1.SuspendLayout();
            SuspendLayout();
            // 
            // darkStatusStrip1
            // 
            darkStatusStrip1.AutoSize = false;
            darkStatusStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkStatusStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkStatusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            darkStatusStrip1.Location = new Point(0, 342);
            darkStatusStrip1.Name = "darkStatusStrip1";
            darkStatusStrip1.Padding = new Padding(0, 5, 0, 3);
            darkStatusStrip1.Size = new Size(537, 24);
            darkStatusStrip1.SizingGrip = false;
            darkStatusStrip1.TabIndex = 0;
            darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Margin = new Padding(0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(10, 16);
            toolStripStatusLabel1.Text = " ";
            // 
            // darkMenuStrip1
            // 
            darkMenuStrip1.BackColor = Color.FromArgb(60, 63, 65);
            darkMenuStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            darkMenuStrip1.Items.AddRange(new ToolStripItem[] { extractAllToolStripMenuItem });
            darkMenuStrip1.Location = new Point(0, 0);
            darkMenuStrip1.Name = "darkMenuStrip1";
            darkMenuStrip1.Padding = new Padding(3, 2, 0, 2);
            darkMenuStrip1.Size = new Size(537, 24);
            darkMenuStrip1.TabIndex = 1;
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
            // darkSectionPanel1
            // 
            darkSectionPanel1.Controls.Add(darkListView1);
            darkSectionPanel1.Dock = DockStyle.Fill;
            darkSectionPanel1.Location = new Point(0, 24);
            darkSectionPanel1.Name = "darkSectionPanel1";
            darkSectionPanel1.SectionHeader = "Shader(s):";
            darkSectionPanel1.Size = new Size(537, 318);
            darkSectionPanel1.TabIndex = 2;
            // 
            // darkListView1
            // 
            darkListView1.ContextMenuStrip = darkContextMenu1;
            darkListView1.Dock = DockStyle.Fill;
            darkListView1.Location = new Point(1, 25);
            darkListView1.MultiSelect = true;
            darkListView1.Name = "darkListView1";
            darkListView1.Size = new Size(535, 292);
            darkListView1.TabIndex = 0;
            darkListView1.Text = "darkListView1";
            // 
            // darkContextMenu1
            // 
            darkContextMenu1.BackColor = Color.FromArgb(60, 63, 65);
            darkContextMenu1.ForeColor = Color.FromArgb(220, 220, 220);
            darkContextMenu1.Items.AddRange(new ToolStripItem[] { extractToolStripMenuItem });
            darkContextMenu1.Name = "darkContextMenu1";
            darkContextMenu1.Size = new Size(119, 26);
            darkContextMenu1.Opening += DarkContextMenu1_Opening;
            // 
            // extractToolStripMenuItem
            // 
            extractToolStripMenuItem.BackColor = Color.FromArgb(60, 63, 65);
            extractToolStripMenuItem.ForeColor = Color.FromArgb(220, 220, 220);
            extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            extractToolStripMenuItem.Size = new Size(118, 22);
            extractToolStripMenuItem.Text = "Extract...";
            extractToolStripMenuItem.Click += ExtractToolStripMenuItem_Click;
            // 
            // PCShadersForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(537, 366);
            Controls.Add(darkSectionPanel1);
            Controls.Add(darkStatusStrip1);
            Controls.Add(darkMenuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = darkMenuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PCShadersForm";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "PCShaders Viewer";
            Load += PCShadersForm_Load;
            darkStatusStrip1.ResumeLayout(false);
            darkStatusStrip1.PerformLayout();
            darkMenuStrip1.ResumeLayout(false);
            darkMenuStrip1.PerformLayout();
            darkSectionPanel1.ResumeLayout(false);
            darkContextMenu1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkListView darkListView1;
        private FolderBrowserDialog folderBrowserDialog1;
        private DarkUI.Controls.DarkContextMenu darkContextMenu1;
        private ToolStripMenuItem extractToolStripMenuItem;
    }
}