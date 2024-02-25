using DarkUI.Forms;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class AboutForm : DarkForm
    {
        public AboutForm()
        {
            InitializeComponent();

            darkLabel6.Text = "Luigi Auriemma, Trevor Natiuk, Jay Franco, Connor Harrison,\ndniel888, and more...";
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void DarkButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}