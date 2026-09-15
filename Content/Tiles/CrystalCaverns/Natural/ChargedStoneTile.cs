using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using static AerovelenceMod.Content.Projectiles.LightningUtils;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Natural
{
    [LegacyName("ChargedStone")]
    public class ChargedStoneTile : ModTile
    {
        private int lightningLifetimeTicks = 20;
        private int minLightningDistance = 3;
        private int maxLightningDistance = 5;
        private int lightningAttemptChanceDenominator = 5000;

        public override void SetStaticDefaults()
        {
			MineResist = 2.5f;
			MinPick = 59;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(80, 110, 170));
			DustType = DustID.BlueTorch;
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

        // Lightning spawning
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer || !Main.hasFocus)
            {
                return;
            }

            Tile tile = Main.tile[i, j];

            if (Main.rand.Next(lightningAttemptChanceDenominator) == 0)
            {
                Vector2 origin = new Vector2(i, j);
                
                // Find a destination tile with an x/y between min and max distance away
                float targetX = i + Main.rand.Next(minLightningDistance, maxLightningDistance + 1) * (Main.rand.Next(2) * 2f - 1f);
                float targetY = j + Main.rand.Next(minLightningDistance, maxLightningDistance + 1) * (Main.rand.Next(2) * 2f - 1f);
                Vector2 target = new Vector2(targetX, targetY);

                // Ensure tile is of same type
                if (tile.TileType == Main.tile[(int)target.X, (int)target.Y].TileType)
                {
                    LightningData newData = new LightningData(origin.ToWorldCoordinates(), target.ToWorldCoordinates(), LightningStyle.Default)
                    {
                        NoiseFrequency = 3f,

                        CoreColorOverride = Color.Gray,
                        MidColorOverride = Color.SteelBlue,
                        OuterColorOverride = Color.MidnightBlue,
                        FlashColorOverride = Color.Black,
                        DistColorOverride = Color.White,

                        GlowIntensity = 0.1f,
                        GlowScale = 0.15f,
                    };

                    TileLightningSystem.LightningLifetimes.Add(newData, lightningLifetimeTicks);
                }
            }
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
