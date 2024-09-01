using System;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;

namespace AerovelenceMod.Common.Systems.Generation.GenUtils
{
    public static class AeroShapes
    {
        public class LightningBoltShape : GenShape
        {
            private readonly int _length;
            private readonly int _maxWidth;
            private int _jaggedness; // Would be readonly but is modified to make the bolt less jagged at the end
            private readonly int _jagReduction;
            private readonly int _jagReduction2;

            /// <summary>
            /// Creates a downward trending lightning bolt.
            /// </summary>
            /// <param name="length">The length of the bolt.</param>
            /// <param name="maxWidth">The initial width of the bolt.</param>
            /// <param name="jaggedness">The maximum offset of each row of tiles from the previous row of tiles. Values below 5 are recommended.</param>
            /// <param name="jagReduction">The first y distance from the end of the bolt that the jaggedness will be reduced by 1. Values of 0 will do nothing.</param>
            /// <param name="jagReduction2">The second y distance from the end of the bolt that the jaggedness will be reduced by 1. Values of 0 will do nothing.</param>
            public LightningBoltShape(int length, int maxWidth, int jaggedness, int jagReduction, int jagReduction2)
            {
                _length = length;
                _maxWidth = maxWidth;
                _jaggedness = jaggedness;
                _jagReduction = jagReduction;
                _jagReduction2 = jagReduction2;
            }
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
                _jagReduction2 = 0;
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
                _jagReduction2 = 0;
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
                    if ((i == _length - _jagReduction || i == _length - _jagReduction2) && _jaggedness > 0) 
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
                    else {
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1);
                    }

                    if (Math.Sign(randResult) != trend && i < _length * 0.95)
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1); // Creates more bias towards the direction it is already going in above the last X rows
                    if (Math.Sign(randResult) != trend && i < _length * 0.75)
                        randResult = WorldGen.genRand.NextBool().ToDirectionInt() * WorldGen.genRand.Next(1, _jaggedness + 1); // Creates more bias towards the direction it is already going in above the last X rows
                    trend = Math.Sign(randResult);

                    currentX += randResult;
                    
                    // Create a vertical segment of the bolt
                    int width = _maxWidth - (i * _maxWidth / _length); // Tapering effect

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
    }
}




