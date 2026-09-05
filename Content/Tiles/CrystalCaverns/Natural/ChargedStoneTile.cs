using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("ChargedStone")]
    public class ChargedStoneTile : ModTile
    {
        
        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CrystalDirt").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
            //Main.tileMerge[Type][Mod.Find<ModTile>("ChargedStone").Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(80, 110, 170));
			DustType = 59;
            CommonTileHelper.SetTileProtection(this);
            TileID.Sets.GeneralPlacementTiles[Type] = false;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float lightFactor = MathHelper.Lerp(0.3f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.02f, j * 0.02f), Main.GlobalTimeWrappedHourly * 0.2f)), 2)));
            r = 0.0f * lightFactor;
            g = 0.6f * lightFactor;
            b = 0.9f * lightFactor;
        }
    }

    public class ChargedStoneItem : ModItem
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
            Item.createTile = ModContent.TileType<ChargedStoneTile>();
            Item.rare = ItemRarityID.White;
            Item.value = 5;
        }
    }
}
