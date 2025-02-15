using Terraria;

internal static class StructureWorldGenHelpers
{


    private static bool IsValidLocation(int x, int y)
    {
        for (int dx = -20; dx < 20; dx++)
        {
            for (int dy = -20; dy < 20; dy++)
            {
                if (Main.tile[x + dx, y + dy].LiquidAmount > 0)
                    return false;
            }
        }
        return true;
    }
}