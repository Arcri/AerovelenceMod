using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class GlassPulseBow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glass Pulse Bow");
            Tooltip.SetDefault("Fires a bolt of energy");
        }

        public override void SetDefaults()
        {
            Item.crit = 8;
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 88;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item5;
            Item.shootSpeed = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 35, 20);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            SoundEngine.PlaySound(SoundID.Item72);
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f));
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<PrismaticBolt>(), damage, knockBack, Main.myPlayer);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}