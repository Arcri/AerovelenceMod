using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.ProjectileItem
{
    public class SandstormArrowItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandstorm Arrow");
		}
		public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 14;
            item.height = 34;
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