using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using TTGamesExplorerRebirthHook.Games;
using TTGamesExplorerRebirthHook.Utils;

namespace TTGamesExplorerRebirthHook
{
#pragma warning disable
    public class HookEngine
    {
        static int Initialize(string arg)
        {
            FileStream stream = File.OpenRead(Process.GetCurrentProcess().MainModule.FileName);
            string sha1 = BitConverter.ToString(SHA1.Create().ComputeHash(stream)).Replace("-", "");

            if (sha1 == "2A0CEDE63A1C0FA76911F87B6909BB6DB2E3FBC4") // LEGOMARVEL.exe
            {
                Logger.Instance.Log("Loading LEGOMarvel-26-09-13-Steam-v0.4g_Hotfix");

                new LegoMSH1("26-09-13");
            }
            else if (sha1 == "8FAB01D1A141ECBAFF561E17A62AA835DE4E420E") // LEGOMARVEL.exe
            {
                Logger.Instance.Log("Loading LEGOMarvel-10-02-14-Steam-Call-In-Patch-v0.3b_Hotfix");

                new LegoMSH1("10-02-14");
            }

            return 0;
        }
    }
#pragma warning restore
}