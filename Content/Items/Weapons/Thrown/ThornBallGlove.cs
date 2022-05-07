using AerovelenceMod.Content.Projectiles.Weapons.Throwing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class ThornBallGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Glove");
            Tooltip.SetDefault("Throws a spiky ball that bounces around");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 57;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 22;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PlanteraSeed>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           if(player.statLife <= player.statLifeMax2 && type == ModContent.ProjectileType<PlanteraSeed>())
           {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<PlanteraSeed>(), 51, 13, player.whoAmI);

           }
           if (player.statLife <= player.statLifeMax2 / 2 && type == ModContent.ProjectileType<PlanteraSeed>())
           {
                type = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<PlanteraGreenSeed>(), 43, 13, player.whoAmI);

           }
           if (player.statLife <= player.statLifeMax2 / 3)
           {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ThornBall>(), 60, 13, player.whoAmI);

           }

            return false;
        }
    }
}