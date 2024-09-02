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
        private Asset<Texture2D> texture;
        private Asset<Texture2D> branchesTexture;
        private Asset<Texture2D> topsTexture;

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
            GrowsOnTileId = [ModContent.TileType<CrystalGrass>()];
            texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree");
            branchesTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree_Branches");
            topsTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CrystalTree_Tops");
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {

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

        public override Asset<Texture2D> GetBranchTextures() => branchesTexture;

        // Top Textures
        public override Asset<Texture2D> GetTopTextures() => topsTexture;

        public override Asset<Texture2D> GetTexture()
        {
            return texture;
        }
    }
}