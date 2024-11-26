using DarkUI.Controls;
using DarkUI.Forms;
using System.Diagnostics;
using TTGamesExplorerRebirthLib.Formats;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class PkForm : DarkForm
    {
        private readonly Pk _pkArchive;
        private Pk _pkArchiveExtraction;

        private readonly string _archiveDataPath;
        private readonly string _archiveIndexPath;

        private bool   _extractingTaskCanceled;
        private string _extractingAllFolderPath;

        public PkForm(string filePath)
        {
            InitializeComponent();

            if (Path.GetExtension(filePath) == ".pkiwin")
            {
                _archiveIndexPath = filePath;
                _archiveDataPath  = Path.ChangeExtension(filePath, ".pkdwin");
            }
            else if (Path.GetExtension(filePath) == ".pkdwin")
            {
                _archiveIndexPath = Path.ChangeExtension(filePath, ".pkiwin");
                _archiveDataPath  = filePath;
            }
            else if (Path.GetExtension(filePath) == ".pkiswitch")
            {
                _archiveIndexPath = filePath;
                _archiveDataPath  = Path.ChangeExtension(filePath, ".pkdswitch");
            }
            else if (Path.GetExtension(filePath) == ".pkdswitch")
            {
                _archiveIndexPath = Path.ChangeExtension(filePath, ".pkiswitch");
                _archiveDataPath  = filePath;
            }
            else if (Path.GetExtension(filePath) == ".pkips4")
            {
                _archiveIndexPath = filePath;
                _archiveDataPath = Path.ChangeExtension(filePath, ".pkdps4");
            }
            else if (Path.GetExtension(filePath) == ".pkdps4")
            {
                _archiveIndexPath = Path.ChangeExtension(filePath, ".pkips4");
                _archiveDataPath = filePath;
            }

            _pkArchive = new Pk(_archiveIndexPath, _archiveDataPath);

            toolStripStatusLabel1.Text = $"{Path.GetFileName(_archiveIndexPath)} | {Path.GetFileName(_archiveDataPath)} - {_pkArchive.Files.Count} file(s)";
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
            foreach (PkFile file in _pkArchive.Files)
            {
                DarkListItem listItem = new($"{file.Path} ({Helper.FormatSize((ulong)file.Data.Length)})")
                {
                    Tag  = file.Path,
                    Icon = Helper.GetIconByFileName(file.Path),
                };

                darkListView1.Items.Add(listItem);
            }
        }

        private void DarkListView1_DoubleClick(object sender, EventArgs e)
        {
            if (darkListView1.Items.Count != 0)
            {
                string name = (string)darkListView1.Items[darkListView1.SelectedIndices[0]].Tag;
                PkFile file = _pkArchive.Files.Where(file => file.Path == name).First();

                Helper.OpenFileInternal(_archiveDataPath, name, file.Data, file);
            }
        }

        private void ExtractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _extractingAllFolderPath = folderBrowserDialog1.SelectedPath;
                _pkArchiveExtraction = _pkArchive;

                LoadingForm loadingForm = new()
                {
                    Text = $"Extract {Path.GetFileName(_archiveDataPath)}..."
                };

                loadingForm.progressBar1.Maximum = _pkArchiveExtraction.Files.Count;
                loadingForm.Shown += LoadingForm_Shown;
                loadingForm.darkButton1.Text = "Cancel";
                loadingForm.darkButton1.Click += DarkButton1_Click;

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

                for (int i = 0; i < _pkArchiveExtraction.Files.Count; i++)
                {
                    if (_extractingTaskCanceled)
                    {
                        _extractingTaskCanceled = false;

                        break;
                    }

                    loadingForm.Invoke((MethodInvoker)(() =>
                    {
                        loadingForm.darkLabel1.Text = $"Extracting: \"{_pkArchiveExtraction.Files[i].Path}\"";
                        loadingForm.darkLabel1.Refresh();

                        loadingForm.progressBar1.Value = i;

                        loadingForm.darkLabel2.Text = $"{i} / {_pkArchiveExtraction.Files.Count} files...";
                        loadingForm.darkLabel2.Refresh();

                        loadingForm.darkLabel3.Text = $"{timer.Elapsed:mm\\:ss}";
                        loadingForm.darkLabel3.Refresh();

                        string path = Path.GetFullPath(Path.Join(_extractingAllFolderPath, _pkArchiveExtraction.Files[i].Path));

                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        File.WriteAllBytes(path, _pkArchiveExtraction.Files[i].Data);

                        i++;
                    }));
                }

                timer.Stop();

                loadingForm.Invoke((MethodInvoker)(() =>
                {
                    loadingForm.Close();
                }));

                MessageBox.Show($"{_pkArchiveExtraction.Files.Count} file(s) extracted!", "Extracting file(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }).Start();
        }

        private void ExtractFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _pkArchiveExtraction = _pkArchive;

                List<PkFile> files = [];
                foreach (int index in darkListView1.SelectedIndices)
                {
                    files.Add(_pkArchiveExtraction.Files.Where(file => file.Path == (string)darkListView1.Items[index].Tag).First());
                }

                _pkArchiveExtraction.Files = files;
                _extractingAllFolderPath = folderBrowserDialog1.SelectedPath;

                LoadingForm loadingForm = new()
                {
                    Text = $"Extract {Path.GetFileName(_archiveDataPath)}..."
                };

                loadingForm.progressBar1.Maximum = _pkArchiveExtraction.Files.Count;
                loadingForm.Shown += LoadingForm_Shown;
                loadingForm.darkButton1.Text = "Cancel";
                loadingForm.darkButton1.Click += DarkButton1_Click;

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