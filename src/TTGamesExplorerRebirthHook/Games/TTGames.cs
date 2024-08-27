using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TTGamesExplorerRebirthHook.Games.LMSH1;

namespace TTGamesExplorerRebirthHook.Games
{
    public class TTGamesItem
    {
        public TTGamesVersion Version { get; set; }
        public ITTGames       TTGame  { get; set; }
    }

    public class TTGames
    {
        private readonly Dictionary<string, TTGamesItem> _games = new Dictionary<string, TTGamesItem>()
        {
            { "2A0CEDE63A1C0FA76911F87B6909BB6DB2E3FBC4", new TTGamesItem { Version = TTGamesVersion.LMSH1_Steamv04g,            TTGame = new LMSH1Hooks(TTGamesVersion.LMSH1_Steamv04g) } }, // LEGOMARVEL.exe
            { "8FAB01D1A141ECBAFF561E17A62AA835DE4E420E", new TTGamesItem { Version = TTGamesVersion.LMSH1_SteamCallInPatchv03b, TTGame = new LMSH1Hooks(TTGamesVersion.LMSH1_SteamCallInPatchv03b) } }, // LEGOMARVEL.exe
        };

        public TTGames()
        {
            FileStream stream = File.OpenRead(Process.GetCurrentProcess().MainModule.FileName);
            string     sha1   = BitConverter.ToString(SHA1.Create().ComputeHash(stream)).Replace("-", "");

            _games.Where(x => x.Key.Equals(sha1)).FirstOrDefault().Value.TTGame.Initialize();
        }
    }
}