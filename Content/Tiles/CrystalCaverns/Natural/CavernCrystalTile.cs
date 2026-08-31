using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("CavernCrystal")]
    public class CavernCrystalTile : ModTile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            MinPick = 55;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystalGrassTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernCrystalTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<CavernStoneTile>()] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            DustType = DustID.BlueFairy;
            HitSound = SoundID.Tink;
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            CommonTileHelper.SetMergeGroup(this, CrystalCaverns: true);
            CommonTileHelper.SetTileProtection(this);
            AddMapEntry(new Color(115, 230, 250));
        }
        
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float lightFactor = MathHelper.Lerp(0.3f, 2f, ((float)Math.Pow(Math.Sin(NoiseHelper.GetDynamicNoise(new Vector2(i * 0.02f, j * 0.02f), Main.GlobalTimeWrappedHourly * 0.2f)), 2)));
            r = 0.0f * lightFactor;
            g = 0.6f * lightFactor;
            b = 0.9f * lightFactor;
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