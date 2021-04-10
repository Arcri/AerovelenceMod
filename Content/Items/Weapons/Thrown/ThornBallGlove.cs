using AerovelenceMod.Content.Projectiles.Weapons.Throwing;
using Microsoft.Xna.Framework;
using Terraria;
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
            item.crit = 11;
            item.damage = 57;
            item.ranged = true;
            item.width = 20;
            item.height = 22;
            item.useTime = 30;
            item.useAnimation = 30;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlanteraSeed>();
            item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
           if(player.statLife <= player.statLifeMax2 && type == ModContent.ProjectileType<PlanteraSeed>())
           {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PlanteraSeed>(), 51, 13, player.whoAmI);

           }
           if (player.statLife <= player.statLifeMax2 / 2 && type == ModContent.ProjectileType<PlanteraSeed>())
           {
                type = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PlanteraGreenSeed>(), 43, 13, player.whoAmI);

           }
           if (player.statLife <= player.statLifeMax2 / 3)
           {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ThornBall>(), 60, 13, player.whoAmI);

           }

            return false;
        }
    }
}