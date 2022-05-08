using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class HeadOfTheBloodGod : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Head Of The Blood God");
			Tooltip.SetDefault("Fires a guy on a chain");
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item1;
			Item.crit = 8;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 54; 
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 0, 40, 20);
            Item.value = 10000;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = Item.shoot = Mod.Find<ModProjectile>("ElementScythe").Type;
            Item.shootSpeed = 5f;
            Item.autoReuse = false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float numberProjectiles = 2 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ElementScythe>(), damage, 1f, player.whoAmI);
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, 1f, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, 1f, player.whoAmI);
                }
            }
            return false;
        }
    }
}