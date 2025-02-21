using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("CavernCrystal")]
    public class CavernCrystalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            MinPick = 55;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            DustType = DustID.BlueFairy;
            HitSound = SoundID.Tink;

            CommonTileHelper.SetMergeGroup(this, CrystalCaverns: true);
            CommonTileHelper.SetTileProtection(this);
            AddMapEntry(new Color(115, 230, 250));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }
    }

    public class CavernCrystalItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CavernCrystalTile>());
            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(copper: 5);
        }
    }
}