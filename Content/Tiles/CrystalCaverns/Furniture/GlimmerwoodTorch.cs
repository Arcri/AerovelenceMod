using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Localization;
using AerovelenceMod.Content.Biomes;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodTorch : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            flameTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodTorch_Flame");
            CommonTileHelper.SetupTorchTile(this, "AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodTorch_Flame", ref flameTexture, new Color(200, 200, 200), DustID.BlueCrystalShard, new int[] { TileID.Torches });
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = WorldGen.SolidTile(i, j - 1) ? 4 : 0;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CommonTileHelper.HandleTorchDraw(Main.tile[i, j], i, j, spriteBatch, flameTexture);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            CommonTileHelper.ModifyTorchLight(i, j, ref r, ref g, ref b, 0.9f, 0.9f, 0.9f);
        }

        public override float GetTorchLuck(Player player)
        {
            return CommonTileHelper.GetTorchLuck(player, ModContent.GetInstance<CrystalCavernsSurfaceBiome>(), 1f, -0.1f);
        }
    }
}