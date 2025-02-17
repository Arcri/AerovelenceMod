using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Worlds
{
    public static class StupidGlobal
    {
        public static bool SuppressTileDrops = false;
    }

    public class NoTileDropGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (StupidGlobal.SuppressTileDrops)
            {
                noItem = true;
            }
            base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
        }
    }
}