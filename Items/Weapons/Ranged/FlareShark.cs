using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class FlareShark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flareshark");
            Tooltip.SetDefault("33% chance to not consume ammo\nShoots flares alongside bullets");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item41;
            item.crit = 4;
            item.damage = 11;
            item.ranged = true;
            item.width = 46;
            item.height = 28;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0;
            item.value = Item.sellPrice(0, 9, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 8f;
        }
        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= .33f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            {
                {
                    type = Main.rand.Next(new int[] { type, ProjectileID.Bullet, ProjectileID.Flare });
                    return true;
                }
            }
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.Minishark, 1);
            modRecipe.AddIngredient(ItemID.FlareGun, 1);
            modRecipe.AddIngredient(ItemID.IllegalGunParts, 1);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}