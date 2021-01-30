using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class FrostHydrasThrow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Hydra's Throw");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 20;
            item.damage = 36;
            item.melee = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("FrostHydrasThrowProjectile");
            item.shootSpeed = 2f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{

    public class FrostHydrasThrowProjectile : ModProjectile
    {
        public float timerFloat = 0f;
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 440f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 24f;
        }
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;
        }


        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Vector2 projPos = projectile.position;
            float speedVertical = 10f;
            float speedHorizontal = 10f;
            Vector2 countedSpeed = new Vector2(speedHorizontal, speedVertical).RotatedBy(MathHelper.ToRadians(0));
            Vector2 countedSpeed2 = new Vector2(speedHorizontal, speedVertical).RotatedBy(MathHelper.ToRadians(90));
            if (timerFloat == 1f)
            {
                Projectile.NewProjectile(projPos.X, projPos.Y, countedSpeed.X, countedSpeed.Y, mod.ProjectileType("IcyShard"), 60, 10f);
                Projectile.NewProjectile(projPos.X, projPos.Y, countedSpeed2.X, countedSpeed2.Y, mod.ProjectileType("IcyShardClone"), 60, 10f);
                timerFloat = 2f;
            }
            timer++;
            if (timer >= 30)
            {
                timerFloat += 1f;
            }

        }
    }
}