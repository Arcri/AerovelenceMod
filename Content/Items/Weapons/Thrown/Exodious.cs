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
            Item.channel = true;		
            Item.crit = 20;
            Item.damage = 36;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("ExodiousProjectile").Type;
            Item.shootSpeed = 2f;
        }
    }

    public class ExodiousProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 440f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 24f;
        }
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

    }
}