using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Items.Weapons.Ranged
{
	public class PoseidonsMagnum : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Poseidon's Magnum"); 
			Tooltip.SetDefault("Fires a stream of water that inflicts Wet and Cursed Inferno\nHas a 20% chance to fire a razorblade typhoon instead");
		}

		public override void SetDefaults() 
		{
			item.damage = 400;
			item.ranged = true;
			item.width = 46;
			item.height = 32;
			item.useTime = 23;
			item.useAnimation = 23;
			item.useStyle = 5;
			item.knockBack = 3;
			item.value = 100000;
			item.rare = 11;
			item.UseSound = SoundID.Item41;
			item.autoReuse = false;
			item.shoot = mod.ProjectileType("PoseidonStream");
			item.shootSpeed = 34f;
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)

        {
			if (Main.rand.Next(100) <= 20)
            {
				for (int i = 0; i < 1; i++)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f)); 
					Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 0.4f, perturbedSpeed.Y * 0.4f, 409, damage, knockBack, player.whoAmI);
				}
            }
			else {
				for (int i = 0; i < 1; i++)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f)); 
					Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("PoseidonStream"), damage, knockBack, player.whoAmI);
				}
				
			}
			return false;
        }		

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.VenusMagnum, 1);
			recipe.AddIngredient(ItemID.FragmentVortex, 12);
			recipe.AddIngredient(ItemID.Starfish, 15);
			recipe.AddIngredient(mod.ItemType("OceanEssense"), 7);
			recipe.AddIngredient(ItemID.LunarBar, 8);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}