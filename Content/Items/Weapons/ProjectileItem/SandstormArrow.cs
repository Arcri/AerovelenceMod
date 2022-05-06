using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.ProjectileItem
{
    public class SandstormArrow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandstorm Arrow");
		}
		public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 34;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Pink;
			Item.maxStack = 999;
			Item.consumable = true;
            Item.shoot = Mod.Find<ModProjectile>("ShockingArrow").Type;
			Item.shootSpeed = 10f;
            Item.value = Item.sellPrice(0, 0, 1, 10);
            Item.ammo = AmmoID.Arrow;
        }
    }
}