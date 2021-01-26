using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DarknessDischarge : ModItem
    {
		int i;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Darkness Discharge");
		}
        public override void SetDefaults()
        {
			item.mana = 6;
			item.crit = 8;
            item.damage = 32;
            item.magic = true;
            item.width = 36;
            item.height = 38;
            item.useTime = 12;
            item.useAnimation = 12;
			item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
			item.shoot = ProjectileID.ClothiersCurse;
			item.shootSpeed = 18f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (i == 1)
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.ShadowFlameKnife, damage, knockBack, player.whoAmI);				
			}
			if (i >= 2)
			{
				i = 0;
				Projectile.NewProjectile(position.X, position.Y, speedX * 2, speedY * 2, ProjectileID.ShadowFlame, damage, knockBack, player.whoAmI);
			}
			i++;
            return true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DischargeBlade : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            projectile.light = 1f;
            projectile.timeLeft = 60;
            projectile.alpha = 100;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0f, 0.50f, 1f, 1f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 2);
        }

        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
            }
            projectile.alpha += 2;
            projectile.velocity *= 0.99f;
            if (!e)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                e = true;
            }
            projectile.rotation += rot;
            rot *= 0.99f;
        }
    }
}