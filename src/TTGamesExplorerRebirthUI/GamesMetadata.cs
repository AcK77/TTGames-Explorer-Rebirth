namespace TTGamesExplorerRebirthUI
{
    public class GamesMetadata
    {
        public string Exe;
        public string Hash;
        public string Name;
        public Image  Cover;
    }

    public static class GamesMetadataHelper
    {
#pragma warning disable CA2211
        public static GamesMetadata[] Items =
        [
            new GamesMetadata()
            {
                Exe = "LEGOMARVEL.exe",
                Hash = "2A0CEDE63A1C0FA76911F87B6909BB6DB2E3FBC4", // LEGOMarvel-26-09-13-Steam-v0.4g_Hotfix
                Name = "LEGO MARVEL Super Heroes",
                Cover = Properties.Resources.LEGOMarvel1
            },
            new GamesMetadata()
            {
                Exe = "LEGOMARVEL.exe",
                Hash = "8FAB01D1A141ECBAFF561E17A62AA835DE4E420E", // LEGOMarvel-10-02-14-Steam-Call-In-Patch-v0.3b_Hotfix
                Name = "LEGO MARVEL Super Heroes",
                Cover = Properties.Resources.LEGOMarvel1
            },
            new GamesMetadata()
            {
                Exe = "LEGOMARVEL2.exe",
                Hash = "0E704AEE9B2A1862B86133254B7B1E97BC66680B",
                Name = "LEGO MARVEL Super Heroes 2",
                Cover = Properties.Resources.LEGOMarvel2
            },
            new GamesMetadata()
            {
                Exe = "LEGOMARVEL2_DX11.exe",
                Hash = "0297912D972E51245CDB3280ABBF46BC5FA520A9",
                Name = "LEGO MARVEL Super Heroes 2",
                Cover = Properties.Resources.LEGOMarvel2
            },
            new GamesMetadata()
            {
                Exe = "LEGOLOTR.exe",
                Hash = "294F2BE808C3051AAD58F17EF2DE764DE76FE1E6",
                Name = "LEGO The Lord of the Rings",
                Cover = Properties.Resources.LEGOLOTR,
            },
            new GamesMetadata()
            {
                Exe = "LEGOStarWarsSaga.exe",
                Hash = "C52E7A40CBC9ADEC932A8935A5DC08C2D69BDCC7",
                Name = "LEGO Star Wars - The Complete Saga",
                Cover = Properties.Resources.LEGOStarWarsSaga,
            },
            new GamesMetadata() 
            {
                Exe = "LEGOIndy.exe",
                Hash = "4FCBF4A9AB06541E987BAB72F8CE4B64172AC983",
                Name = "LEGO Indiana Jones: The Original Adventures",
                Cover = Properties.Resources.LEGOIndy1
            },
            new GamesMetadata()
            {
                Exe = "LEGO_Worlds.exe",
                Hash = "B4BA0A05F3645EBCBF4C6C5E85A6B602323397F6",
                Name = "LEGO Worlds",
                Cover = Properties.Resources.LEGOWorlds
            },
            new GamesMetadata()
            {
                Exe = "LEGO_Worlds_DX11.exe",
                Hash = "C1085D8F8FE8F1CF2657E7B797F3CD083B44A6C7",
                Name = "LEGO Worlds",
                Cover = Properties.Resources.LEGOWorlds
            }
        ];
#pragma warning restore CA2211
    }
}