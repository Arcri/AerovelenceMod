using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Humanizer;
using Terraria.ID;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;

namespace AerovelenceMod.Content.Tiles.Relics
{
    public class BaseBossRelic : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1;

        public Asset<Texture2D> RelicTexture;
        public virtual string RelicTextureName => "AerovelenceMod/Content/Tiles/Relics/CyvercryBossRelic";
        public override string Texture => "AerovelenceMod/Content/Tiles/Relics/RelicPedestal";

        public override void Load() => RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);

        public override void SetStaticDefaults()
        {
            CommonTileHelper.SetupBossRelic(this, new Color(123, 123, 123), DustID.CosmicEmber, ModContent.ItemType<CyvercryBossRelicItem>(), true, true, true);
        }
        public override bool CreateDust(int i, int j, ref int type) { return false; }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CommonTileHelper.drawRelics(this, RelicTexture.Value, FrameWidth, FrameHeight, HorizontalFrames, VerticalFrames, i, j, spriteBatch);
        }
    }

    public class CyvercryBossRelic : BaseBossRelic
    {
        public override string RelicTextureName => "AerovelenceMod/Content/Tiles/Relics/CyvercryBossRelic";

        public override void SetStaticDefaults() => base.SetStaticDefaults();
    }

    public class CyvercryBossRelicItem : ModItem
    {

        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<CyvercryBossRelic>(), 0);

    }
}