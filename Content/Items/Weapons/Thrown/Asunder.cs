using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class Asunder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asunder");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.crit = 8;
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = Mod.Find<ModProjectile>("AsunderProjectile").Type;
            Item.shootSpeed = 12f;
        }
    }

    public class AsunderProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + ((float)Math.PI / 4);
            if (Projectile.timeLeft % 30 == 0 && Projectile.alpha != 255)
            {
                var proj = Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                Main.projectile[proj].alpha = Projectile.alpha + (255 / 3);
            }
        }
    }
}