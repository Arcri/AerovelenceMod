using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Modules;
using ReLogic.Utilities;

namespace AerovelenceMod.Common.Systems.Generation.GenUtils
{
    public static class AeroActions
    {
        public class SwapSolidTileInclusive : GenAction
        {
            private ushort _type;

            public SwapSolidTileInclusive(ushort type)
            {
                _type = type;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = _tiles[x, y];
                if (WorldGen.SolidOrSlopedTile(tile))
                {
                    tile.ResetToType(_type);
                    return UnitApply(origin, x, y, args);
                }

                return Fail();
            }
        }

        public class PlaceTail : GenAction
        {
            private readonly ushort _type;
            private readonly int _width;
            private readonly Vector2D _offset;
            private readonly int _widthVariance;
            private readonly int _xOffsetVariance;
            private readonly int _yOffsetVariance;

            public PlaceTail(ushort type, int width, Vector2D offset, int widthVariance = 0, int xOffsetVariance = 0, int yOffsetVariance = 0)
            {
                _type = type;
                _width = width;
                _offset = offset;
                _widthVariance = widthVariance;
                _xOffsetVariance = xOffsetVariance;
                _yOffsetVariance = yOffsetVariance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = _tiles[x, y];

                WorldUtils.Gen(new Point(x, y), new Shapes.Tail(
                    _width + WorldGen.genRand.Next(-_widthVariance, _widthVariance + 1),
                    new Vector2D(
                        _offset.X + WorldGen.genRand.Next(-_xOffsetVariance, _xOffsetVariance + 1),
                        _offset.Y + WorldGen.genRand.Next(-_yOffsetVariance, _yOffsetVariance + 1)
                    )
                ), new Actions.SetTileKeepWall(_type));

                return UnitApply(origin, x, y, args);
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

        public class SolidBelow : GenAction
        {
            private int _distance;

            public SolidBelow(int distance)
            {
                _distance = distance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (WorldUtils.Find(new Point(x, y), Searches.Chain(new Searches.Down(1), new Conditions.IsSolid().AreaAnd(1, _distance)), out Point _))
                    return UnitApply(origin, x, y, args);
                return Fail();
            }
        }

        public class NotSolidAbove : GenAction
        {
            private int _distance;

            public NotSolidAbove(int distance)
            {
                _distance = distance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (!WorldUtils.Find(new Point(x, y - _distance), Searches.Chain(new Searches.Up(1), new Conditions.IsSolid().AreaOr(1, _distance)), out Point _))
                {
                    return UnitApply(origin, x, y, args);
                }
                return Fail();
            }
        }
    }
}
