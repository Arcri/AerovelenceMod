using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Tiles.IceExpansion
{
	public class ValleyTree : ModTree
	{
		// This is a blind copy-paste from Vanilla's PurityPalmTree settings.
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
		{
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		public override void SetStaticDefaults()
		{
			// Makes Example Tree grow on ExampleBlock
			GrowsOnTileId = new int[1] { ModContent.TileType<ValleyGrass>() };
		}

		public override void SetTreeFoliageSettings(Tile tile, int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
		{
			// Copied from ExampleMod on 5/26/2022 at 11:36PM EST
			// This function is for advanced stuff
		}

		public override int CreateDust()
		{
			return DustType<Sparkle>();
		}

		public override int GrowthFXGore()
		{
			return ModContent.Find<ModGore>("Gores/ExampleTreeFX").Type;
		}

		public override int DropWood()
		{
			return ItemType<Glimmerwood>();
		}

		public override Asset<Texture2D> GetTexture()
		{
			return ModContent.Request<Texture2D>("Blocks/CrystalCaverns/Tiles/ValleyTree");
		}

		public override Asset<Texture2D> GetTopTextures()
		{
			return ModContent.Request<Texture2D>("Blocks/CrystalCaverns/Tiles/ValleyTree_Tops");
		}

		public override Asset<Texture2D> GetBranchTextures()
		{
			return ModContent.Request<Texture2D>("Blocks/CrystalCaverns/Tiles/ValleyTree_Branches");
		}
	}
}
