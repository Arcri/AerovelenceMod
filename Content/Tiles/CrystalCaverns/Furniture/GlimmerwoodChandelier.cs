using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    public class GlimmerwoodChandelier : ModTile
    {
        private static Asset<Texture2D> _flameTexture;

        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupChandelier(
                this,
                new Color(123, 123, 123),
                ModContent.ItemType<GlimmerwoodChandelierItem>(),
                1f, 0.75f, 1f,
                "AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodChandelier_Flame", 
                ref _flameTexture
            );
        }
    }
}