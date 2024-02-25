using DarkUI.Forms;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class LoadingForm : DarkForm
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }
    }
}