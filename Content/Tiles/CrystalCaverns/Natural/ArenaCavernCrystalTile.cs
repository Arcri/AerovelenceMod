using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("ArenaCavernCrystal")]
    public class ArenaCavernCrystalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystalGrassTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernCrystalTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernStoneTile>()] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(102, 108, 117));
            DustType = 116;
            HitSound = SoundID.Tink;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float lightFactor = MathHelper.Lerp(0.3f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.02f, j * 0.02f), Main.GlobalTimeWrappedHourly * 0.2f)), 2)));
            r = 0.0f * lightFactor;
            g = 0.6f * lightFactor;
            b = 0.9f * lightFactor;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }

    public class ArenaCavernCrystalItem : ModItem
    {
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
            Item.createTile = ModContent.TileType<ArenaCavernCrystalTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}
