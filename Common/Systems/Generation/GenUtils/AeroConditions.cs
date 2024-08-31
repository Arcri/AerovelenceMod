using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.GenUtils
{
    public static class AeroConditions
    {
        public class HasShimmer : GenCondition
        {
            protected override bool CheckValidity(int x, int y)
            {
                if (_tiles[x, y].LiquidAmount > 0)
                    return _tiles[x, y].LiquidType == 3;

                return false;
            }
        }
    }
}
