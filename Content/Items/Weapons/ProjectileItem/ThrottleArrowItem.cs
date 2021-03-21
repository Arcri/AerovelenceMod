using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.ProjectileItem
{
    public class ThrottleArrowItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Throttle Arrow");
		}
		public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 18;
            item.height = 38;
            item.knockBack = 4;
            item.rare = ItemRarityID.Pink;
			item.maxStack = 999;
			item.consumable = true;
            item.shoot = mod.ProjectileType("ThrottleArrow");
			item.shootSpeed = 10f;
            item.value = Item.sellPrice(0, 0, 1, 10);
            item.ammo = AmmoID.Arrow;
        }
    }
}