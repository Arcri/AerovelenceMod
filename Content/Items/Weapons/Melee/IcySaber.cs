using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class IcySaber : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Icy Saber");

        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.damage = 24;
            item.melee = true;
            item.width = 64;
            item.height = 72;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 7, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {

            float length = 350f;
            Vector2 projectilePos = target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * length;
            Vector2 projectileVelocity = Vector2.Normalize(target.Center - projectilePos) * 16;

            Projectile.NewProjectile(projectilePos, projectileVelocity, ModContent.ProjectileType<IcySaberProj>(), damage, knockBack);
            Main.PlaySound(SoundID.Item27);

            if(crit == true)
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}