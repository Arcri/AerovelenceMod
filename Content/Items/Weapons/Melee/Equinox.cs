using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Equinox : ModItem
	{
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Equinox");
			Tooltip.SetDefault("During daytime, the Equinox deals more damage and lights enemies on fire\nDuring nighttime, the Equinox will explode into large blasts of moon");
		}
        public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = Item.sellPrice(gold: 5);
			item.rare = ItemRarityID.Yellow;
			item.noMelee = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 40;
			item.useTime = 40;
			item.knockBack = 4f;
			item.damage = 120;
			item.noUseGraphic = true;
			item.shoot = ModContent.ProjectileType<EquinoxProjectile>();
			item.shootSpeed = 15.1f;
			item.UseSound = SoundID.Item1;
			item.melee = true;
			item.crit = 9;
			item.channel = true;
		}
	}
}