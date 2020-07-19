using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class ElementalShift : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Shift");
			Tooltip.SetDefault("The great sword of the slimes.");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 24;
            item.melee = true;
            item.width = 54;
            item.height = 54; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 0, 40, 20);
            item.value = 10000;
            item.autoReuse = true;
            item.rare = ItemRarityID.Blue;
            item.shoot = item.shoot = mod.ProjectileType("ElementScythe");
            item.shootSpeed = 5f;
            item.autoReuse = false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float numberProjectiles = 2 + Main.rand.Next(1);
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(15));
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                if (i == 1)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ElementScythe>(), damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, knockBack, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 1, perturbedSpeed.Y * 1, type, damage, knockBack, player.whoAmI);
                }
            }
            return false;
        }
    }
}