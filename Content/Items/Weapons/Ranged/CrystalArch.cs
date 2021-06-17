using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CrystalArch : ModItem
    {
        int projectileAmount = 3;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Crystal Arch");
        
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.damage = 33;

            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;

            item.width = 60;
            item.height = 32;
            
            item.useAnimation = item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
           
            item.shoot = ModContent.ProjectileType<IceArrow>();
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 11f;
        }    
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {   
            type = ModContent.ProjectileType<IceArrow>();
            
            for(int i = 0; i < projectileAmount; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.PiOver4 * 0.25f);
                Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
            }

           return false;

        }






        /*public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.FrostArrow, damage, knockBack, player.whoAmI);
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    Projectile.NewProjectile(position.X,
                        position.Y,
                        speedX + Main.rand.NextFloat(-2, 2),
                        speedY + Main.rand.NextFloat(-2, 2),
                        ProjectileID.FrostArrow,
                        damage,
                        knockBack,
                        player.whoAmI);
                }
            }
            return false;
        } */
    }
}