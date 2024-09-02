using Microsoft.Xna.Framework;
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

        public class NotTouchingAir : GenAction
        {
            private static readonly int[] DIRECTIONS = new int[16] {
            0,
            -1,
            1,
            0,
            -1,
            0,
            0,
            1,
            -1,
            -1,
            1,
            -1,
            -1,
            1,
            1,
            1
            };
            private bool _useDiagonals;

            public NotTouchingAir(bool useDiagonals = false)
            {
                _useDiagonals = useDiagonals;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                int num = (_useDiagonals ? 16 : 8);
                for (int i = 0; i < num; i += 2)
                {
                    if (!_tiles[x + DIRECTIONS[i], y + DIRECTIONS[i + 1]].HasTile)

                        return Fail();
                }
                return UnitApply(origin, x, y, args);
            }
        }
    }
}
