using DarkUI.Config;
using DarkUI.Forms;
using FastColoredTextBoxNS;
using System.Text;
using System.Text.RegularExpressions;
using TTGamesExplorerRebirthLib.Formats.DAT;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class TextForm : DarkForm
    {
        private readonly TextStyle _blueStyle = new(Brushes.Blue, null, FontStyle.Regular);
        private readonly TextStyle _grayStyle = new(Brushes.Gray, null, FontStyle.Regular);
        private readonly TextStyle _greenStyle = new(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle _brownStyle = new(Brushes.LightBlue, null, FontStyle.Italic);

        public TextForm(string fileName, byte[] fileBuffer, object datFile = null)
        {
            InitializeComponent();

            fastColoredTextBox1.BackColor = Colors.DarkBackground;
            fastColoredTextBox1.SelectionColor = Colors.BlueHighlight;
            fastColoredTextBox1.Text = Encoding.ASCII.GetString(fileBuffer);

            switch (Path.GetExtension(fileName))
            {
                case ".xml":
                    fastColoredTextBox1.Language = Language.XML;
                    break;
            }

            toolStripStatusLabel1.Text = $"{fileName} ({Helper.FormatSize((ulong)fileBuffer.Length)})";

            if (datFile != null && datFile is DATFile file && file.Compression != CompressionFormat.None)
            {
                toolStripStatusLabel1.Text += $" - Compression: {file.Compression} ({Helper.FormatSize(file.CompressedSize)})";
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void FastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.SetStyle(_brownStyle, "\"\"|@\"\"|''|@\".*?\"|(?<!@)(?<range>\".*?[^\\\\]\")|'.*?[^\\\\]'");
            e.ChangedRange.SetStyle(_greenStyle, "//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(_greenStyle, "(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(_greenStyle, "(/\\*.*?\\*/)|(.*\\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            e.ChangedRange.SetStyle(_brownStyle, "\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b");
            e.ChangedRange.SetStyle(_grayStyle, "^\\s*(?<range>\\[.+?\\])\\s*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(_blueStyle, "\\b(area_start|area_end|level_start|level_end)");
            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("area_start", "area_end");
            e.ChangedRange.SetFoldingMarkers("level_start", "level_end");
            e.ChangedRange.SetFoldingMarkers("{", "}");
        }
    }
}
