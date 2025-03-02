using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class PhanticOre : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PhanticOreTile>());
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarities.MidPHM;
            Item.autoReuse = true;
        }
    }

    public class PhanticOreTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(203, 032, 087));
            DustType = 59;
            HitSound = SoundID.Tink;
        }
    }
}