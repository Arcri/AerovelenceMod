using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class CrystalGrowingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Growing Kit");
            Tooltip.SetDefault("'The instructions were missing'");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = Mod.Find<ModProjectile>("CrystalGrowingKitProj").Type;
            Item.shootSpeed = 16f;
        }
    }

    public class CrystalGrowingKitProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int spawnX = (int)(Projectile.Center.X / 64) * 64;
            int spawnY = (int)((Projectile.position.Y - Projectile.height) / 16) * 16;
            int index = Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnX, spawnY + 70, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<CrystalGrowingKitField>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
            Projectile.Kill();
            return true;
        }
        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
            }
			

        }
    }

    public class CrystalGrowingKitField : ModProjectile
    {
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 500;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0f;
            return true;
        }
        public override void AI()
        {
            int count = 0;

            foreach (Projectile proj in Main.projectile.Where(x => x.active && x.whoAmI != Projectile.whoAmI && x.type == Projectile.type))
            {
                count++;

                if (count >= 7)
                    proj.Kill();
            }
            i++;
            if (i % 2 == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 132);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
            Projectile.alpha -= 2;
            Projectile.velocity *= 0f;
        }
    }
}