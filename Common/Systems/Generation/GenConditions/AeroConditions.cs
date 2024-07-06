using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.GenConditions
{
    public static class AeroConditions
    {
        public class HasShimmer : GenCondition
        {
            protected override bool CheckValidity(int x, int y)
            {
                if (GenBase._tiles[x, y].LiquidAmount > 0)
                    return GenBase._tiles[x, y].LiquidType == 3;

                return false;
            }
        }
    }
}
