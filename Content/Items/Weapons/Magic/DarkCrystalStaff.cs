using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(1));
            velocity.X = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<SkylightProjectile>(), damage * 1, 1f, player.whoAmI);
            }
            return true;
        }
    }
}