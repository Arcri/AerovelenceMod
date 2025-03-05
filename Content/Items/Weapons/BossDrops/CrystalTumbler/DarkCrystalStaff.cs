using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;
using System.Collections.Generic;


namespace AerovelenceMod.Content.Items.Weapons.BossDrops.CrystalTumbler
{
    public class DarkCrystalStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarities.LatePHM;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LightningStrike>();
            Item.shootSpeed = 0f;
            Item.mana = 10;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            Vector2 mouseWorld = Main.MouseWorld;
            Vector2? targetTile = FindTileBelow(mouseWorld);

            if (targetTile.HasValue)
            {
                Vector2 start = new Vector2(targetTile.Value.X, targetTile.Value.Y - 500);
                Vector2 end = targetTile.Value;

                LightningManager.StrikeLightning(start, end, damage, knockBack, 200);
            }

            return false;
        }

        private Vector2? FindTileBelow(Vector2 position)
        {
            int tileX = (int)(position.X / 16f);
            int tileY = (int)(position.Y / 16f);

            for (int y = tileY; y < Main.maxTilesY; y++)
            {
                Tile tile = Framing.GetTileSafely(tileX, y);
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    return new Vector2(tileX * 16f + 8f, y * 16f + 8f);
                }
            }

            return null;
        }
    }
}