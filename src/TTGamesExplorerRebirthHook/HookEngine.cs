using TTGamesExplorerRebirthHook.Games;

namespace TTGamesExplorerRebirthHook
{
#pragma warning disable
    public class HookEngine
    {
        static int Initialize(string arg)
        {
            new TTGames();

            return 0;
        }
    }
#pragma warning restore
}