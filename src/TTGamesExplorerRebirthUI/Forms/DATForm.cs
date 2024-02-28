using DarkUI.Controls;
using DarkUI.Forms;
using System.Diagnostics;
using TTGamesExplorerRebirthLib.Formats.DAT;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class DATForm : DarkForm
    {
        private readonly DATArchive _datArchive;
        private DATArchive _extractingTempArchive;
        private readonly List<string> _datArchiveFolders = [];

        private string _gameFolderPath;
        private readonly string _datFilePath;

        private bool _extractingTaskCanceled;
        private bool _fullExtraction;

        public DATForm(string gameFolderPath, string datFilePath)
        {
            InitializeComponent();

            _gameFolderPath = gameFolderPath;
            _datFilePath = datFilePath;

            // Show loadingForm.

            LoadingForm loadingForm = new()
            {
                Text = "Opening DAT file...",
                StartPosition = FormStartPosition.CenterParent
            };

            new Thread(() =>
            {
                Stopwatch stopwatch = new();
                System.Windows.Forms.Timer timer = new();

                timer.Start();
                stopwatch.Start();
                timer.Tick += (s, e) =>
                {
                    loadingForm?.Invoke((MethodInvoker)(() =>
                    {
                        loadingForm.darkLabel3.Text = $"{stopwatch.Elapsed:mm\\:ss}";
                        loadingForm.darkLabel3.Refresh();
                    }));
                };

                loadingForm.Height = 135;
                loadingForm.progressBar1.Maximum = 100;
                loadingForm.darkLabel1.Text = "Opening DAT file, please wait...";
                loadingForm.darkLabel2.Text = "";
                loadingForm.progressBar1.Style = ProgressBarStyle.Marquee;
                loadingForm.progressBar1.MarqueeAnimationSpeed = 30;
                loadingForm.darkButton1.Visible = false;

                loadingForm.ShowDialog();
                timer.Stop();
                stopwatch.Stop();
            }).Start();

            // Load DAT file.

            _datArchive = new(datFilePath);

            foreach (var file in _datArchive.Files)
            {
                string folderPath = file.Path.Replace(Path.GetFileName(file.Path), "");
                folderPath = folderPath.Remove(folderPath.Length - 1);

                if (!_datArchiveFolders.Contains(folderPath))
                {
                    _datArchiveFolders.Add(folderPath);
                }
            }

            _datArchiveFolders.Sort();

            toolStripStatusLabel1.Text = $"{Path.GetFileName(datFilePath)} - {_datArchive.Files.Count} file(s)";

            // Close loadingForm.

            loadingForm?.Invoke((MethodInvoker)(() =>
            {
                loadingForm?.Close();
            }));
        }

        private void LoadFolderInTreeView()
        {
            darkTreeView1.Nodes.Clear();

            DarkTreeNode rootNode = new("Root")
            {
                ExpandedIcon = new Bitmap(Properties.Resources.folder_page),
                Icon = new Bitmap(Properties.Resources.folder),
                ExpandAreaHot = true,
                IsRoot = true,
                Expanded = true,
            };

            DarkTreeNode parentNode = null;
            DarkTreeNode tempRootNode = null;
            string subPathRoot;

            foreach (string path in _datArchiveFolders)
            {
                subPathRoot = "Root\\";

                parentNode ??= rootNode;

                if (tempRootNode != null)
                {
                    parentNode = tempRootNode;
                    tempRootNode = null;
                }

                var pathSplit = path.Split("\\").Skip(1).ToArray();
                var i = 0;
                var count = pathSplit.Length;

                foreach (string subPath in pathSplit)
                {
                    if (subPath.Trim() != "")
                    {
                        subPathRoot += subPath + "\\";

                        DarkTreeNode current = parentNode.Nodes.Find(node => node.FullPath == subPathRoot.TrimEnd('\\'));
                        if (current == null)
                        {
                            DarkTreeNode node = new(subPath)
                            {
                                ExpandedIcon = new Bitmap(Properties.Resources.folder_page),
                                Icon = new Bitmap(Properties.Resources.folder),
                                Expanded = true,
                            };

                            parentNode.Nodes.Add(node);

                            parentNode = node;
                        }
                        else
                        {
                            tempRootNode = parentNode;
                            parentNode = current;
                        }
                    }

                    if (++i == count)
                    {
                        parentNode = rootNode;
                        tempRootNode = null;
                    }
                }
            }

            darkTreeView1.Nodes.Add(rootNode);
            darkTreeView1.SelectNode(rootNode);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void DATForm_Load(object sender, EventArgs e)
        {
            LoadFolderInTreeView();
        }

        private void DarkTreeView1_SelectedNodesChanged(object sender, EventArgs e)
        {
            if (darkTreeView1.SelectedNodes.Count == 0)
            {
                return;
            }

            darkTreeView1.SelectedNodes[0].Expanded = true;

            darkListView1.Items.Clear();

            if (darkTreeView1.SelectedNodes[0].FullPath != "Root")
            {
                string parent = Path.GetDirectoryName(darkTreeView1.SelectedNodes[0].FullPath);

                darkListView1.Items.Add(new DarkListItem("..")
                {
                    Icon = new Bitmap(Properties.Resources.arrow_up),
                    Tag = parent,
                });
            }

            foreach (DarkTreeNode folder in darkTreeView1.SelectedNodes[0].Nodes)
            {
                darkListView1.Items.Add(new DarkListItem(folder.Text)
                {
                    Icon = new Bitmap(Properties.Resources.folder),
                    Tag = folder.FullPath,
                });
            }

            foreach (DATFile file in _datArchive.Files.OrderBy(file => file.Path))
            {
                if (file.Path.Replace(Path.GetFileName(file.Path), "").TrimEnd('\\') == darkTreeView1.SelectedNodes[0].FullPath[4..])
                {
                    DarkListItem listItem = new(Path.GetFileName(file.Path))
                    {
                        Tag = file.Path,
                        Icon = Helper.GetIconByFileName(file.Path)
                    };

                    listItem.Text += $" ({Helper.FormatSize(file.Size)})";

                    darkListView1.Items.Add(listItem);
                }
            }
        }

        private void DarkListView1_DoubleClick(object sender, EventArgs e)
        {
            string path = (string)darkListView1.Items[darkListView1.SelectedIndices[0]].Tag;

            if (path[..4] == "Root")
            {
                DarkTreeNode parent = null;
                string oldSubPath = "";

                foreach (var subPath in path.Split("\\"))
                {
                    parent ??= darkTreeView1.Nodes[0];

                    DarkTreeNode node;
                    if (oldSubPath == "")
                    {
                        oldSubPath = "Root";
                        node = darkTreeView1.Nodes.Find(item => item.FullPath == oldSubPath + subPath);
                    }
                    else
                    {
                        oldSubPath += "\\" + subPath;
                        node = parent.Nodes.Find(item => item.FullPath == oldSubPath);
                    }

                    if (node != null)
                    {
                        parent = node;
                    }
                }

                if (parent != null)
                {
                    darkTreeView1.SelectNode(parent);
                    parent.Expanded = true;
                }
            }
            else
            {
                DATFile file = _datArchive.Files.Where(file => file.Path == path).First();

                byte[] fileBuffer = _datArchive.ExtractFile(file, true);

                Helper.OpenFileInternal(_datFilePath, path, fileBuffer, file);
            }
        }

        private void ExtractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string oldgameFolderPath = _gameFolderPath;

            if (Path.GetDirectoryName(_datFilePath) != _gameFolderPath)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    _gameFolderPath = folderBrowserDialog1.SelectedPath;
                }
            }

            _extractingTempArchive = _datArchive;
            _fullExtraction = true;

            LoadingForm loadingForm = new()
            {
                Text = $"Extracting {Path.GetFileName(_datFilePath)}..."
            };

            loadingForm.progressBar1.Maximum = _extractingTempArchive.Files.Count;
            loadingForm.Shown += LoadingForm_Shown;
            loadingForm.darkButton1.Text = "Cancel";
            loadingForm.darkButton1.Click += DarkButton1_Click;

            loadingForm.ShowDialog();

            _extractingTempArchive = null;
            _gameFolderPath = oldgameFolderPath;
        }

        private void DarkButton1_Click(object sender, EventArgs e)
        {
            _extractingTaskCanceled = true;
        }

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            Extract(sender);
        }

        private void DarkContextMenu1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (darkListView1.SelectedIndices.Count < 1)
            {
                e.Cancel = true;
            }
        }

        private void ExtractFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _extractingTempArchive = _datArchive;
                
                List<DATFile> files = [];
                foreach (int index in darkListView1.SelectedIndices)
                {
                    files.Add(_datArchive.Files.Where(file => file.Path == (string)darkListView1.Items[index].Tag).First());
                }

                _extractingTempArchive.Files = files;

                string oldgameFolderPath = _gameFolderPath;

                _gameFolderPath = folderBrowserDialog1.SelectedPath;
                _fullExtraction = false;

                LoadingForm loadingForm = new()
                {
                    Text = $"Extracting {Path.GetFileName(_datFilePath)}..."
                };

                loadingForm.progressBar1.Maximum = _extractingTempArchive.Files.Count;
                loadingForm.Shown += LoadingForm_Shown;
                loadingForm.darkButton1.Text = "Cancel";
                loadingForm.darkButton1.Click += DarkButton1_Click;

                loadingForm.ShowDialog();

                _extractingTempArchive = null;
                _gameFolderPath = oldgameFolderPath;
            }
        }

        private void Extract(object sender)
        {
            List<string> failedFiles = [];

            new Thread(() =>
            {
                LoadingForm loadingForm = (LoadingForm)sender;
                int i = 0;

                Stopwatch timer = new();
                timer.Start();

                loadingForm.progressBar1.Maximum = _extractingTempArchive.Files.Count;

                foreach (DATFile file in _extractingTempArchive.Files)
                {
                    if (_extractingTaskCanceled)
                    {
                        _extractingTaskCanceled = false;

                        break;
                    }

                    loadingForm.Invoke((MethodInvoker)(() =>
                    {
                        loadingForm.darkLabel1.Text = $"Extracting: \"{file.Path}\"";
                        loadingForm.darkLabel1.Refresh();

                        loadingForm.progressBar1.Value = i;

                        loadingForm.darkLabel2.Text = $"{i} / {_extractingTempArchive.Files.Count} files...";
                        loadingForm.darkLabel2.Refresh();

                        loadingForm.darkLabel3.Text = $"{timer.Elapsed:mm\\:ss}";
                        loadingForm.darkLabel3.Refresh();

                        byte[] fileBuffer;

                        try
                        {
                            fileBuffer = _extractingTempArchive.ExtractFile(file, true);
                        }
                        catch
                        {
                            failedFiles.Add(file.Path);

                            fileBuffer = _extractingTempArchive.ExtractFile(file);
                        }

                        string fullFilePath = Path.GetFullPath(Path.Join(_gameFolderPath, _fullExtraction ? file.Path : Path.GetFileName(file.Path)));

                        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath));

                        File.WriteAllBytes(fullFilePath, fileBuffer);

                        i++;
                    }));
                }

                timer.Stop();

                loadingForm.Invoke((MethodInvoker)(() =>
                {
                    loadingForm.Close();
                }));

                MessageBox.Show($"{i} file(s) extracted!", "Extracting file(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }).Start();

            // TODO: Do something with the failedFiles list which contains decompressions/decrypt issue.
        }
    }
}
