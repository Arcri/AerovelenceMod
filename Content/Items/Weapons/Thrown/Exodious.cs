using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class Exodious : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exodious");
        }
        public override void SetDefaults()
        {
            item.channel = true;		
            item.crit = 20;
            item.damage = 36;
            item.melee = true;
            item.width = 32;
            item.height = 32;
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
            item.shoot = mod.ProjectileType("ExodiousProjectile");
            item.shootSpeed = 2f;
        }
    }

    public class ExodiousProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 440f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 24f;
        }
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

    }
}