using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class Delvera : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Delvera");
            Tooltip.SetDefault("Uses magnetism to generate power");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 37;
            item.magic = true;
            item.mana = 10;
            item.width = 50;
            item.height = 50;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 10, 0);
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("BallOfFire");
            item.shootSpeed = 10f;
		}
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.Spark, ModContent.ProjectileType<BallOfFire>() });
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostRay>(), 1);
            modRecipe.AddIngredient(ItemID.WandofSparking, 1);
            modRecipe.AddIngredient(ItemID.Vilethorn, 1);
            modRecipe.AddIngredient(ItemID.NaturesGift, 1);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}