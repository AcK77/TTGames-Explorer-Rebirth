using DarkUI.Controls;
using DarkUI.Forms;
using TTGamesExplorerRebirthLib.Formats;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class PCShadersForm : DarkForm
    {
        private readonly PCShaders _pcShaders;

        public PCShadersForm(string filePath, byte[] fileBuffer)
        {
            InitializeComponent();

            _pcShaders = (fileBuffer != null) ? new(fileBuffer) : new(filePath);

            Text                       += $" - {Path.GetFileName(filePath)} - {_pcShaders.Shaders.Count} shader(s)";
            toolStripStatusLabel1.Text  = _pcShaders.ProjectName;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void PCShadersForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _pcShaders.Shaders.Count; i++)
            {
                darkListView1.Items.Add(new DarkListItem()
                {
                    Text = $"Shader_{i + 1}.{(_pcShaders.Shaders[i].Type == PCShadersType.DXBC ? "dxbc" : "ctab")}",
                    Tag  = i,
                    Icon = new Bitmap(Properties.Resources.page_white_music),
                });
            }
        }

        private void ExtractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < _pcShaders.Shaders.Count; i++)
                {
                    string path = Path.Join(folderBrowserDialog1.SelectedPath, $"Shader_{i + 1}.{(_pcShaders.Shaders[i].Type == PCShadersType.DXBC ? "dxbc" : "ctab")}");

                    File.WriteAllBytes(path, _pcShaders.Shaders[i].Data);
                }

                MessageBox.Show($"{_pcShaders.Shaders.Count} shader(s) extracted!", "Extracting shader(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DarkContextMenu1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (darkListView1.SelectedIndices.Count < 1)
            {
                e.Cancel = true;
            }
        }

        private void ExtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < darkListView1.SelectedIndices.Count; i++)
                {
                    PCShadersFile pcShadersFile = _pcShaders.Shaders[darkListView1.SelectedIndices[i]];

                    string path = Path.Join(folderBrowserDialog1.SelectedPath, $"Shader_{darkListView1.SelectedIndices[i] + 1}.{(pcShadersFile.Type == PCShadersType.DXBC ? "dxbc" : "ctab")}");

                    File.WriteAllBytes(path, pcShadersFile.Data);
                }

                MessageBox.Show($"{darkListView1.SelectedIndices.Count} shader(s) extracted!", "Extracting shader(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}