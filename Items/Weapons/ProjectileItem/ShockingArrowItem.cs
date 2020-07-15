using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.ProjectileItem
{
    public class ShockingArrowItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shocking Arrow");
		}
		public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 12;
            item.height = 12;
            item.knockBack = 4;
            item.rare = ItemRarityID.Pink;
			item.maxStack = 999;
			item.consumable = true;
            item.shoot = mod.ProjectileType("ShockingArrow");
			item.shootSpeed = 10f;
            item.value = Item.sellPrice(0, 0, 1, 10);
            item.ammo = AmmoID.Arrow;
        }
    }
}