using AerovelenceMod.Content.Projectiles.NPCs.Bosses.Decurion;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Apocalypse : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apocalypse");
            Tooltip.SetDefault("Fires 3 unstable rockets");
        }
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 32;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.reuseDelay = 70;
            Item.knockBack = 0.2f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DecurionRocket>(); ;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Rocket;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, -2);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
            velocity.X = perturbedSpeed.X;
            velocity.Y = perturbedSpeed.Y;
            Vector2 muzzleOffset = new Vector2(-7, -8);
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}