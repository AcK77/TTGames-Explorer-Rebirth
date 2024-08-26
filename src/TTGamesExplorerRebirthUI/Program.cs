using TTGamesExplorerRebirthUI.Forms;

namespace TTGamesExplorerRebirthUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            if (args.Length != 0 && (File.Exists(args[0]) || Directory.Exists(args[0])))
            {
                Application.Run(new MainForm(args[0]));
                return;
            }

            Application.Run(new MainForm());
        }
    }
}