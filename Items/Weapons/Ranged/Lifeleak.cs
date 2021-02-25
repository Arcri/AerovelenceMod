using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Lifeleak : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lifeleak");
			Tooltip.SetDefault("'You feel numb holding it'\nUses your life as ammo");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item41;
			item.crit = 3;
            item.damage = 15;
            item.ranged = true;
            item.width = 72;
            item.height = 28; 
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.75f;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {

            return new Vector2(-8, 0);
        }
        public override bool UseItem(Player player)
        {
            Main.NewText("HHHH");
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " didn't follow the instructions"), 5, 0, false);
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.IronBar, 15);
            modRecipe.AddIngredient(ItemID.IllegalGunParts, 1);
            modRecipe.AddIngredient(ItemID.LifeCrystal, 5);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}