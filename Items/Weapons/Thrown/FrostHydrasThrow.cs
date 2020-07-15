using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class FrostHydrasThrow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Hydra's Throw");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 20;
            item.damage = 36;
            item.melee = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("FrostHydrasThrowProjectile");
            item.shootSpeed = 2f;
        }
    }
}