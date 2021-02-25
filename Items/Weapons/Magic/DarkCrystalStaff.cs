using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using AerovelenceMod.Items.Weapons.Ranged;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DarkCrystalStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Dark Crystal Staff");
            Tooltip.SetDefault("Fires a lightning bolt");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 35;
            item.magic = true;
            item.mana = 20;
            item.width = 64;
            item.height = 64;
            item.useTime = 35;
            item.useAnimation = 35;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 1, 15, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<InvisibleProj>();
            item.shootSpeed = 11f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(1));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SkylightProjectile>(), damage * 1, knockBack, player.whoAmI);
            }
            return true;
        }
    }
}