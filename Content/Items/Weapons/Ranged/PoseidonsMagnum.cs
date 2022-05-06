using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
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
			Item.damage = 400;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 46;
			Item.height = 32;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.value = 100000;
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item41;
			Item.autoReuse = false;
			Item.shoot = Mod.Find<ModProjectile>("PoseidonStream").Type;
			Item.shootSpeed = 34f;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)

        {
			if (Main.rand.Next(100) <= 20)
            {
				for (int i = 0; i < 1; i++)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f)); 
					Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 0.4f, perturbedSpeed.Y * 0.4f, ProjectileID.Typhoon, damage, knockBack, player.whoAmI);
				}
            }
			else {
				for (int i = 0; i < 1; i++)
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f)); 
					Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, Mod.Find<ModProjectile>("PoseidonStream").Type, damage, knockBack, player.whoAmI);
				}
				
			}
			return false;
        }		

		public override void AddRecipes() 
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.VenusMagnum, 1)
				.AddIngredient(ItemID.FragmentVortex, 12)
				.AddIngredient(ItemID.Starfish, 15)
				.AddIngredient(ItemID.LunarBar, 8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}