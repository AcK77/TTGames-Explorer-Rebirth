using DarkUI.Controls;
using DarkUI.Forms;
using System.Diagnostics;
using TTGamesExplorerRebirthLib.Formats;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class PAKForm : DarkForm
    {
        private readonly PAK _pakArchive;
        private PAK          _pakArchiveExtraction;

        private readonly string _filePath;

        private bool   _extractingTaskCanceled;
        private string _extractingAllFolderPath;

        public PAKForm(string filePath, byte[] fileBuffer)
        {
            InitializeComponent();

            _filePath   = filePath;
            _pakArchive = (fileBuffer != null) ? new(fileBuffer) : new(filePath);

            toolStripStatusLabel1.Text = $"{Path.GetFileName(filePath)} - {_pakArchive.Files.Count} file(s)";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void DATForm_Load(object sender, EventArgs e)
        {
            LoadFilesInListView();
        }

        private void LoadFilesInListView()
        {
            foreach (PAKFile file in _pakArchive.Files.OrderBy(file => file.Name))
            {
                DarkListItem listItem = new($"{file.Name} ({Helper.FormatSize(file.Size)})")
                {
                    Tag  = file.Name,
                    Icon = Helper.GetIconByFileName(file.Name),
                };

                darkListView1.Items.Add(listItem);
            }
        }

        private void DarkListView1_DoubleClick(object sender, EventArgs e)
        {
            string  name = (string)darkListView1.Items[darkListView1.SelectedIndices[0]].Tag;
            PAKFile file = _pakArchive.Files.Where(file => file.Name == name).First();

            Helper.OpenFileInternal(_filePath, name, file.Data, file);
        }

        private void ExtractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _extractingAllFolderPath = folderBrowserDialog1.SelectedPath;
                _pakArchiveExtraction    = _pakArchive;

                LoadingForm loadingForm = new()
                {
                    Text = $"Extract {Path.GetFileName(_filePath)}..."
                };

                loadingForm.progressBar1.Maximum  = _pakArchiveExtraction.Files.Count;
                loadingForm.Shown                += LoadingForm_Shown;
                loadingForm.darkButton1.Text      = "Cancel";
                loadingForm.darkButton1.Click    += DarkButton1_Click;

                loadingForm.ShowDialog();
            }
        }

        private void DarkButton1_Click(object sender, EventArgs e)
        {
            _extractingTaskCanceled = true;
        }

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            List<string> files = [];

            new Thread(() =>
            {
                LoadingForm loadingForm = (LoadingForm)sender;
                Stopwatch timer = new();

                timer.Start();

                for (int i = 0; i < _pakArchiveExtraction.Files.Count; i++)
                {
                    if (_extractingTaskCanceled)
                    {
                        _extractingTaskCanceled = false;

                        break;
                    }

                    loadingForm.Invoke((MethodInvoker)(() =>
                    {
                        loadingForm.darkLabel1.Text = $"Extracting: \"{_pakArchiveExtraction.Files[i].Name}\"";
                        loadingForm.darkLabel1.Refresh();

                        loadingForm.progressBar1.Value = i;

                        loadingForm.darkLabel2.Text = $"{i} / {_pakArchiveExtraction.Files.Count} files...";
                        loadingForm.darkLabel2.Refresh();

                        loadingForm.darkLabel3.Text = $"{timer.Elapsed:mm\\:ss}";
                        loadingForm.darkLabel3.Refresh();

                        string path = Path.GetFullPath(Path.Join(_extractingAllFolderPath, _pakArchiveExtraction.Files[i].Name));

                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        File.WriteAllBytes(path, _pakArchiveExtraction.Files[i].Data);

                        i++;
                    }));
                }

                timer.Stop();

                loadingForm.Invoke((MethodInvoker)(() =>
                {
                    loadingForm.Close();
                }));

                MessageBox.Show($"{_pakArchiveExtraction.Files.Count} file(s) extracted!", "Extracting file(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }).Start();
        }

        private void ExtractFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _pakArchiveExtraction = _pakArchive;

                List<PAKFile> files = [];
                foreach (int index in darkListView1.SelectedIndices)
                {
                    files.Add(_pakArchiveExtraction.Files.Where(file => file.Name == (string)darkListView1.Items[index].Tag).First());
                }

                _pakArchiveExtraction.Files = files;
                _extractingAllFolderPath    = folderBrowserDialog1.SelectedPath;

                LoadingForm loadingForm = new()
                {
                    Text = $"Extract {Path.GetFileName(_filePath)}..."
                };

                loadingForm.progressBar1.Maximum  = _pakArchiveExtraction.Files.Count;
                loadingForm.Shown                += LoadingForm_Shown;
                loadingForm.darkButton1.Text      = "Cancel";
                loadingForm.darkButton1.Click    += DarkButton1_Click;

                loadingForm.ShowDialog();
            }
        }

        private void DarkContextMenu1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (darkListView1.SelectedIndices.Count < 1)
            {
                e.Cancel = true;
            }
        }
    }
}