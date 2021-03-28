using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class CrystalArch : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Arch");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 33;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 12f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
        }
    }
}