using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class CrystalTree : ModTree
	{
		private Mod mod => ModLoader.GetMod("AerovelenceMod");

		public override int CreateDust()
		{
			return DustType<Sparkle>();
		}

		public override int GrowthFXGore()
		{
			return mod.Find<ModGore>("Gores/ExampleTreeFX").Type;
		}

		public override int DropWood()
		{
			return ItemType <Glimmerwood>();
		}

		public override Texture2D GetTexture()
		{
			return mod.Assets.Request<Texture2D>("Content/Tiles/CrystalCaverns/Tiles/CrystalTree").Value;
		}

		public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
		{
			return mod.Assets.Request<Texture2D>("Content/Tiles/CrystalCaverns/Tiles/CrystalTree_Tops").Value;
		}

		public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
		{
			return mod.Assets.Request<Texture2D>("Content/Tiles/CrystalCaverns/Tiles/CrystalTree_Branches").Value;
		}
	}
}