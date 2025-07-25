using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.WorldBuilding;
using static Terraria.Collision;

namespace AerovelenceMod.Common.Utilities.Generation
{
    public static class AeroGenUtils
    {
        #region GenActions
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

        public class PlaceBlob : GenAction
        {
            private readonly ushort _type;
            private readonly float _horizontalRadius;
            private readonly float _verticalRadius;
            private readonly float _horizontalVariance;
            private readonly float _verticalVariance;

            public PlaceBlob(ushort type, int radius)
            {
                _type = type;
                _horizontalRadius = radius;
                _verticalRadius = radius;
                _horizontalVariance = 0;
                _verticalVariance = 0;
            }

            public PlaceBlob(ushort type, int horizontalRadius, int verticalRadius)
            {
                _type = type;
                _horizontalRadius = horizontalRadius;
                _verticalRadius = verticalRadius;
                _horizontalVariance = 0;
                _verticalVariance = 0;
            }

            public PlaceBlob(ushort type, float horizontalRadius, float verticalRadius, float horizontalVariance, float verticalVariance)
            {
                _type = type;
                _horizontalRadius = horizontalRadius;
                _verticalRadius = verticalRadius;
                _horizontalVariance = horizontalVariance;
                _verticalVariance = verticalVariance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = _tiles[x, y];

                WorldUtils.Gen(new Point(x, y), new Shapes.Circle(
                        (int)Math.Round(_horizontalRadius + WorldGen.genRand.NextFloat(-_horizontalVariance, _horizontalVariance + 1)),
                        (int)Math.Round(_verticalRadius + WorldGen.genRand.NextFloat(-_verticalVariance, _verticalVariance + 1))),
                    Actions.Chain(new GenAction[]
                    {
                        new Modifiers.RadialDither(_horizontalRadius + _horizontalVariance - 2, _horizontalRadius + _horizontalVariance),
                        new SwapSolidTileInclusive(_type)
                    }));

                return UnitApply(origin, x, y, args);
            }
        }

        public class ClearBlobWall : GenAction
        {
            private readonly float _horizontalRadius;
            private readonly float _verticalRadius;
            private readonly float _horizontalVariance;
            private readonly float _verticalVariance;

            public ClearBlobWall(int radius)
            {
                _horizontalRadius = radius;
                _verticalRadius = radius;
                _horizontalVariance = 0;
                _verticalVariance = 0;
            }

            public ClearBlobWall(int horizontalRadius, int verticalRadius)
            {
                _horizontalRadius = horizontalRadius;
                _verticalRadius = verticalRadius;
                _horizontalVariance = 0;
                _verticalVariance = 0;
            }

            public ClearBlobWall(float horizontalRadius, float verticalRadius, float horizontalVariance, float verticalVariance)
            {
                _horizontalRadius = horizontalRadius;
                _verticalRadius = verticalRadius;
                _horizontalVariance = horizontalVariance;
                _verticalVariance = verticalVariance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = _tiles[x, y];

                WorldUtils.Gen(new Point(x, y), new Shapes.Circle(
                        (int)Math.Round(_horizontalRadius + WorldGen.genRand.NextFloat(-_horizontalVariance, _horizontalVariance + 1)),
                        (int)Math.Round(_verticalRadius + WorldGen.genRand.NextFloat(-_verticalVariance, _verticalVariance + 1))),
                    Actions.Chain(new GenAction[]
                    {
                        // Slightly borked but who cares lol
                        new Modifiers.RadialDither(_horizontalRadius + _horizontalVariance - 3, _horizontalRadius + _horizontalVariance),
                        new Actions.ClearWall()
                    }));

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
                int num = _useDiagonals ? 16 : 8;
                for (int i = 0; i < num; i += 2)
                {
                    if (!_tiles[x + DIRECTIONS[i], y + DIRECTIONS[i + 1]].HasTile)

                        return Fail();
                }
                return UnitApply(origin, x, y, args);
            }
        }

        public class NotTouchingTiles : GenAction
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
            private ushort[] _tileIDs;

            public NotTouchingTiles(bool useDiagonals, params ushort[] tileIDs)
            {
                _useDiagonals = useDiagonals;
                _tileIDs = tileIDs;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                int num = _useDiagonals ? 16 : 8;
                for (int i = 0; i < num; i += 2)
                {
                    for (int j = 0; j < _tileIDs.Length; j++)
                    {
                        if (_tiles[x + DIRECTIONS[i], y + DIRECTIONS[i + 1]].TileType == _tileIDs[j])
                            return Fail();
                    }
                }
                return UnitApply(origin, x, y, args);
            }
        }

        public class IsTouchingWall : GenAction
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
            private ushort[] _wallIds;

            public IsTouchingWall(bool useDiagonals, params ushort[] wallIds)
            {
                _useDiagonals = useDiagonals;
                _wallIds = wallIds;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                int num = _useDiagonals ? 16 : 8;
                for (int i = 0; i < num; i += 2)
                {
                    Tile tile = _tiles[x + DIRECTIONS[i], y + DIRECTIONS[i + 1]];
                    if (!tile.HasTile)
                        continue;

                    for (int j = 0; j < _wallIds.Length; j++)
                    {
                        if (tile.WallType == _wallIds[j])
                            return UnitApply(origin, x, y, args);
                    }
                }

                return Fail();
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

        public class IsBelowSurface : GenAction
        {
            private int _yOffset;

            public IsBelowSurface()
            {
                _yOffset = 0;
            }

            public IsBelowSurface(int yOffset)
            {
                _yOffset = yOffset;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (y + _yOffset > Main.worldSurface)
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

        public class NotSolidBelow : GenAction
        {
            private int _distance;

            public NotSolidBelow(int distance)
            {
                _distance = distance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (!WorldUtils.Find(new Point(x, y), Searches.Chain(new Searches.Down(1), new Conditions.IsSolid().AreaOr(1, _distance)), out Point _))
                {
                    return UnitApply(origin, x, y, args);
                }
                return Fail();
            }
        }

        public class ClearWallRunner : GenAction
        {
            public ClearWallRunner() { }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                WorldGen.MudWallRunner(x, y);

                return UnitApply(origin, x, y, args);
            }
        }
        #endregion

        #region GenShapes
        public class LightningBoltShape : GenShape
        {
            private readonly int _length;
            private readonly int _maxWidth;
            private int _jaggedness; // Would be readonly but is modified to make the bolt less jagged at the end
            private readonly int _jagReduction;

            /// <summary>
            /// Creates a downward trending lightning bolt.
            /// </summary>
            /// <param name="length">The length of the bolt.</param>
            /// <param name="maxWidth">The initial width of the bolt.</param>
            /// <param name="jaggedness">The maximum offset of each row of tiles from the previous row of tiles. Values below 5 are recommended.</param>
            /// <param name="jagReduction">The y distance from the end of the bolt that the jaggedness will be reduced by 1. Values of 0 will do nothing.</param>
            public LightningBoltShape(int length, int maxWidth, int jaggedness, int jagReduction)
            {
                _length = length;
                _maxWidth = maxWidth;
                _jaggedness = jaggedness;
                _jagReduction = jagReduction;
            }
            /// <summary>
            /// Creates a downward trending lightning bolt.
            /// </summary>
            /// <param name="length">The length of the bolt.</param>
            /// <param name="maxWidth">The initial width of the bolt.</param>
            /// <param name="jaggedness">The maximum offset of each row of tiles from the previous row of tiles. Values below 5 are recommended.</param>
            public LightningBoltShape(int length, int maxWidth, int jaggedness)
            {
                _length = length;
                _maxWidth = maxWidth;
                _jaggedness = jaggedness;
                _jagReduction = 0;
            }

            public override bool Perform(Point origin, GenAction action)
            {
                // Initialize the starting position and direction
                int currentX = origin.X;
                int currentY = origin.Y;

                int trend = 0;

                for (int i = 0; i < _length; i++)
                {
                    // Make the bolt less jagged at the end
                    if (i == _length - _jagReduction && _jaggedness > 0)
                        _jaggedness -= 1;

                    // Randomly adjust the X position to create jaggedness as long as it is not too far from the origin
                    int randResult;
                    if (origin.X - currentX >= _maxWidth)
                    {
                        randResult = WorldGen.genRand.Next(1, _jaggedness + 1);
                        trend = Math.Sign(randResult);
                    }
                    else if (origin.X - currentX <= -_maxWidth)
                    {
                        randResult = WorldGen.genRand.Next(-_jaggedness, 0);
                        trend = Math.Sign(randResult);
                    }
                    else
                    {
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1);
                    }

                    if (Math.Sign(randResult) != trend && i < _length * 0.90)
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1); // Creates more bias towards the direction it is already going in above the last X rows
                    if (Math.Sign(randResult) != trend && i < _length * 0.70)
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1); // Creates more bias towards the direction it is already going in above the last X rows
                    trend = Math.Sign(randResult);

                    currentX += randResult;

                    // Create a vertical segment of the bolt
                    int width = _maxWidth - i * _maxWidth / _length; // Tapering effect

                    for (int w = -width / 2; w <= width / 2; w++)
                    {
                        UnitApply(action, origin, currentX + w, currentY);
                    }

                    // Move downward
                    currentY++;
                }

                return true;
            }
        }
        #endregion

        #region GenConditions
        public class HasShimmer : GenCondition
        {
            protected override bool CheckValidity(int x, int y)
            {
                if (_tiles[x, y].LiquidAmount > 0)
                    return _tiles[x, y].LiquidType == 3;

                return false;
            }
        }

        public class IsNotSolid : GenCondition
        {
            protected override bool CheckValidity(int x, int y)
            {

                if (!WorldGen.InWorld(x, y, 10))
                    return false;

                if (!_tiles[x, y].HasTile)
                    return true;

                return false;
            }
        }
        #endregion
    }
}
