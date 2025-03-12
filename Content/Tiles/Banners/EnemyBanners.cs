using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Tiles.Banners
{
    public class EnemyBanners : ModBannerTile
    {
        public struct BannerID(int column, int row)
        {
            public int Column = column;
            public int Row = row;
        }

        public enum StyleID
        {
            //Crystal Caverns
            Charger = 0,
            CrystalSlime = 1,
            CaveSlime = 2,
            Lumurker = 3,
            Commander = 4,
            ElectricTetra = 5,
            AdultJelly = 6,
            BabyJelly = 7,
            CrystalWorm = 8,
            CrystalBat = 9,
            Sapper = 10,
            MiniTumbler = 11,
            Condurtle = 12,
            Dredger = 13,
            //Misc
            SlateRoller = 14,
            SlateDemon = 15
        }

        public static readonly Dictionary<StyleID, BannerID> BannerPositions = new()
        {
            { StyleID.Charger, new BannerID(column: 0, row: 0) },
            { StyleID.CrystalSlime, new BannerID(column: 1, row: 0) },
            { StyleID.CaveSlime, new BannerID(column: 2, row: 0) },
            { StyleID.Lumurker, new BannerID(column: 3, row: 0) },
            { StyleID.Commander, new BannerID(column: 4, row: 0) },
            { StyleID.ElectricTetra, new BannerID(column: 5, row: 0) },
            { StyleID.AdultJelly, new BannerID(column: 6, row: 0) },
            { StyleID.BabyJelly, new BannerID(column: 7, row: 0) },
            { StyleID.CrystalWorm, new BannerID(column: 8, row: 0) },
            { StyleID.CrystalBat, new BannerID(column: 9, row: 0) },
            { StyleID.Sapper, new BannerID(column: 10, row: 0) },
            { StyleID.MiniTumbler, new BannerID(column: 11, row: 0) },
            { StyleID.Condurtle, new BannerID(column: 12, row: 0) },
            { StyleID.Dredger, new BannerID(column: 13, row: 0) },
            { StyleID.SlateRoller, new BannerID(column: 0, row: 0) },
            { StyleID.SlateDemon, new BannerID(column: 0, row: 1) }
        };

        private static readonly Dictionary<StyleID, int> BannerItemTypes = [];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 14;
            TileObjectData.addTile(Type);

            BannerItemTypes.Clear();
            BannerItemTypes.Add(StyleID.Charger, ModContent.ItemType<ChargerBanner>());
            BannerItemTypes.Add(StyleID.CrystalSlime, ModContent.ItemType<CrystalSlimeBanner>());
            BannerItemTypes.Add(StyleID.CaveSlime, ModContent.ItemType<CaveSlimeBanner>());
            BannerItemTypes.Add(StyleID.Lumurker, ModContent.ItemType<LumurkerBanner>());
            BannerItemTypes.Add(StyleID.Commander, ModContent.ItemType<CommanderBanner>());
            BannerItemTypes.Add(StyleID.ElectricTetra, ModContent.ItemType<ElectricTetraBanner>());
            BannerItemTypes.Add(StyleID.AdultJelly, ModContent.ItemType<AdultJellyBanner>());
            BannerItemTypes.Add(StyleID.BabyJelly, ModContent.ItemType<BabyJellyBanner>());
            BannerItemTypes.Add(StyleID.CrystalWorm, ModContent.ItemType<CrystalWormBanner>());
            BannerItemTypes.Add(StyleID.CrystalBat, ModContent.ItemType<CrystalBatBanner>());
            BannerItemTypes.Add(StyleID.Sapper, ModContent.ItemType<SapperBanner>());
            BannerItemTypes.Add(StyleID.MiniTumbler, ModContent.ItemType<MiniTumblerBanner>());
            BannerItemTypes.Add(StyleID.Condurtle, ModContent.ItemType<CondurtleBanner>());
            BannerItemTypes.Add(StyleID.Dredger, ModContent.ItemType<DredgerBanner>());
            BannerItemTypes.Add(StyleID.SlateRoller, ModContent.ItemType<SlateRollerBanner>());
            BannerItemTypes.Add(StyleID.SlateDemon, ModContent.ItemType<SlateDemonBanner>());

            foreach (var kvp in BannerItemTypes)
                RegisterItemDrop(kvp.Value, (int)kvp.Key);
        }

        public static BannerID GetBannerPosition(StyleID style)
        {
            if (BannerPositions.TryGetValue(style, out BannerID pos))
                return pos;
            return new BannerID(0, 0);
        }
    }

    public abstract class BaseBannerItem : ModItem
    {
        private EnemyBanners.StyleID bannerStyle;

        protected BaseBannerItem(EnemyBanners.StyleID style) => bannerStyle = style;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanners>(), (int)bannerStyle);
            Item.width = 10;
            Item.height = 24;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
        }
    }

    public class ChargerBanner : BaseBannerItem { public ChargerBanner() : base(EnemyBanners.StyleID.Charger) { } }
    public class CrystalSlimeBanner : BaseBannerItem { public CrystalSlimeBanner() : base(EnemyBanners.StyleID.CrystalSlime) { } }
    public class CaveSlimeBanner : BaseBannerItem { public CaveSlimeBanner() : base(EnemyBanners.StyleID.CaveSlime) { } }
    public class LumurkerBanner : BaseBannerItem { public LumurkerBanner() : base(EnemyBanners.StyleID.Lumurker) { } }
    public class CommanderBanner : BaseBannerItem { public CommanderBanner() : base(EnemyBanners.StyleID.Commander) { } }
    public class ElectricTetraBanner : BaseBannerItem { public ElectricTetraBanner() : base(EnemyBanners.StyleID.ElectricTetra) { } }
    public class AdultJellyBanner : BaseBannerItem { public AdultJellyBanner() : base(EnemyBanners.StyleID.AdultJelly) { } }
    public class BabyJellyBanner : BaseBannerItem { public BabyJellyBanner() : base(EnemyBanners.StyleID.BabyJelly) { } }
    public class CrystalWormBanner : BaseBannerItem { public CrystalWormBanner() : base(EnemyBanners.StyleID.CrystalWorm) { } }
    public class CrystalBatBanner : BaseBannerItem { public CrystalBatBanner() : base(EnemyBanners.StyleID.CrystalBat) { } }
    public class SapperBanner : BaseBannerItem { public SapperBanner() : base(EnemyBanners.StyleID.Sapper) { } }
    public class MiniTumblerBanner : BaseBannerItem { public MiniTumblerBanner() : base(EnemyBanners.StyleID.MiniTumbler) { } }
    public class CondurtleBanner : BaseBannerItem { public CondurtleBanner() : base(EnemyBanners.StyleID.Condurtle) { } }
    public class DredgerBanner : BaseBannerItem { public DredgerBanner() : base(EnemyBanners.StyleID.Dredger) { } }
    public class SlateRollerBanner : BaseBannerItem { public SlateRollerBanner() : base(EnemyBanners.StyleID.SlateRoller) { } }
    public class SlateDemonBanner : BaseBannerItem { public SlateDemonBanner() : base(EnemyBanners.StyleID.SlateDemon) { } }
}
