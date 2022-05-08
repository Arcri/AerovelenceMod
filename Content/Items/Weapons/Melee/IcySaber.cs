using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class IcySaber : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Icy Saber");

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 72;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {

            float length = 350f;
            Vector2 projectilePos = target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * length;
            Vector2 projectileVelocity = Vector2.Normalize(target.Center - projectilePos) * 16;

            Projectile.NewProjectile(target.GetSource_OnHit(target), projectilePos, projectileVelocity, ModContent.ProjectileType<IcySaberProj>(), damage, knockBack);
            SoundEngine.PlaySound(SoundID.Item27);

            if(crit == true)
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}