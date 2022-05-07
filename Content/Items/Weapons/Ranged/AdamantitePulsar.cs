using AerovelenceMod.Content.NPCs.Bosses.LightningMoth;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AdamantitePulsar : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Adamantite Pulsar");

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, 0f);
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item60;
            Item.crit = 8;
            Item.damage = 50;
            Item.reuseDelay = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AdamantitePulsarProj>();
            Item.shootSpeed = 16f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.AdamantiteBar, 15)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 1 + Main.rand.Next(1);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 2f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3));
                if (i == 1)
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, 2f, player.whoAmI);
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, 2f, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, type, damage, 2f, player.whoAmI);
                }
            }
            return true;
        }
    }
}