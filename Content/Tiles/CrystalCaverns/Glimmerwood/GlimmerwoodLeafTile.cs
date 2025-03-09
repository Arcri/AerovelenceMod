using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood
{
<<<<<<<< HEAD:Content/Tiles/CrystalCaverns/Glimmerwood/GlimmerwoodLeafTile.cs
    public class GlimmerwoodLeafTile : ModTile
========
    [LegacyName("CavernBrick")]
    public class CavernBrickTile : ModTile
>>>>>>>> Arcri-Branch-NonBiomeCC:Content/Tiles/CrystalCaverns/Building/CavernBrickTile.cs
    {
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrackedCavernBrickTile>()] = true;
            AddMapEntry(new Color(061, 079, 110));
			DustType = 59;
			HitSound = SoundID.Grass;
        }
    }

    public class GlimmerwoodLeafItem : ModItem {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
<<<<<<<< HEAD:Content/Tiles/CrystalCaverns/Glimmerwood/GlimmerwoodLeafTile.cs
            Item.createTile = ModContent.TileType<GlimmerwoodLeafTile>();
========
            Item.createTile = ModContent.TileType<CavernBrickTile>();
>>>>>>>> Arcri-Branch-NonBiomeCC:Content/Tiles/CrystalCaverns/Building/CavernBrickTile.cs
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}