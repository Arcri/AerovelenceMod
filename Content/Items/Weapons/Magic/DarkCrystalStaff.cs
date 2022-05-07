using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DarkCrystalStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Dark Crystal Staff");
            Tooltip.SetDefault("Fires a lightning bolt");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 1, 15, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<InvisibleProj>();
            Item.shootSpeed = 12f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, velocity.Y).RotatedByRandom(MathHelper.ToRadians(1));
            speedX = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, velocity.Y, ModContent.ProjectileType<SkylightProjectile>(), damage * 1, knockBack, player.whoAmI);
            }
            return true;
        }
    }
}