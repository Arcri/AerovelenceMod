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
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Yellow;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 40;
			Item.useTime = 40;
			Item.knockBack = 4f;
			Item.damage = 120;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<EquinoxProjectile>();
			Item.shootSpeed = 15.1f;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Melee;
			Item.crit = 9;
			Item.channel = true;
		}
	}
}