using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    public class CrystalTree : ModTree
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
			GrowsOnTileId = new int[1] { ModContent.TileType<CrystalGrass>() };
		}

        /*public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<CrystalSapling>();
		}*/

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            // Example settings, modify these to fit your tree's appearance
            xoffset = 5;
            treeFrame = 0;
            floorY = 0;
            topTextureFrameWidth = 80;
            topTextureFrameHeight = 100;

            if (tile.TileType == ModContent.TileType<CrystalGrass>())
            {
                // Custom settings for your Crystal Tree
                xoffset = 10;
                treeFrame = 1;
                floorY = 16;
                topTextureFrameWidth = 50;
                topTextureFrameHeight = 120;
            }
        }


        /*public override int CreateDust()
		{
			return;// DustType<Sparkle>();
		}*/

        //public override int GrowthFXGore()
        //{
        //	return ModContent.Find<ModGore>("Gores/ExampleTreeFX").Type;
        //}

        public override int DropWood()
		{
			return ItemType <ShotgunAxe>();
		}

		public override Asset<Texture2D> GetTexture()
		{
			return ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree");
		}

		public override Asset<Texture2D> GetTopTextures()
		{
			return ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree_Tops");
		}

		public override Asset<Texture2D> GetBranchTextures()
		{
			return ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree_Branches");
		}
	}
}

