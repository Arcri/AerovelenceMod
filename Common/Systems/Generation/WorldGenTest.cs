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

            WorldUtils.Gen(new Point(x, y), new Shapes.Rectangle(5, 5), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()),
            }));

            //WorldUtils.Gen(new Point(x, y), new Shapes.Mound(20, 40), Actions.Chain(new GenAction[]
            //{
            //    new Modifiers.Flip(false, true),
            //    new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()),
            //}));
            //WorldUtils.Gen(new Point(x, y), new Shapes.Mound(20, 10), Actions.Chain(new GenAction[]
            //{

            //    new Actions.SetTile((ushort)ModContent.TileType<CavernStone>()),
            //}));
            //WorldUtils.Gen(new Point(x, y+2), new Shapes.Mound(18, 37), Actions.Chain(new GenAction[]
            //{
            //    new Modifiers.Flip(false, true),
            //    new Actions.PlaceWall(WallID.Stone)
            //}));
        }
    }
}