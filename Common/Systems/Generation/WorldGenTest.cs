using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Terraria.Utilities;
using static Terraria.WorldBuilding.Shapes;
using ReLogic.Utilities;
using System;
using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Common.Utilities.StructureStamper;

namespace AerovelenceMod.Common.Systems.Generation
{
    public class WorldGenTest : ModSystem
    {
		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.RightAlt))
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
        }
        private void TestMethod(int x, int y)
        {
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            /* Useful methods here:
            WorldGen.TileRunner(x, y, WorldGen.genRand.Next(30, 40), WorldGen.genRand.Next(45, 50), ModContent.TileType<ChargedStone>());
            WorldUtils.Gen(new Point(x, y), new Shapes.Tail(8, new ReLogic.Utilities.Vector2D(WorldGen.genRand.Next(-25, 25), WorldGen.genRand.Next(-25, 25))), new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()));
            WorldUtils.Gen(new Point(x, y), new Shapes.Mound(20, 40), Actions.Chain(new GenAction[]
            {
                new Modifiers.Flip(false, true),
                new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()),
            }));
            */
            // Code to test placed here:
            Point origin = new Point(x, y);
            Random rand = new Random(); // Use WorldGen.genRand.Next() for actual world generation
            Point temp = WorldGen.digTunnel(origin.X, origin.Y, 3, 0, 50, 5).ToPoint();
            

            //WorldGen.digTunnel(origin.X, origin.Y, 3, 0, 50, 5);
            //WorldGen.digTunnel(origin.X, origin.Y, -3, 0, 50, 5);

            //WorldUtils.Gen(origin, new AeroShapes.LightningBoltShape(350, 50, 3, 150, 30), new Actions.SetTile(TileID.Bubble));

            /// Creates an opening similar to the surface caves
            //WorldGen.CaveOpenater(origin.X, origin.Y);

            /// Creates a medium-sized, blobby underground cave
            //WorldGen.Caverer(origin.X, origin.Y);

            /// Creates a long, winding cave, presumably used in vanilla at the end of CaveOpenater's generation 
            //WorldGen.Cavinator(origin.X, origin.Y, 50);

            /// Creates a corruption chasm, extends existing corruption caves
            //WorldGen.ChasmRunner(origin.X, origin.Y, 50, true);

            /// Creates a single, configurable tunnel in a straight line extending from the origin. The result will not be the same every time.
            //WorldGen.digTunnel(origin.X, origin.Y, 1, 1, 50, 3);
        }
    }
}