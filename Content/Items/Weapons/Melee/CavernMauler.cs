using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class CavernMauler : ModItem
	{
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Cavern Mauler");
		}
        public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.sellPrice(silver: 5);
			Item.rare = ItemRarityID.Green;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 40;
			Item.useTime = 40;
			Item.knockBack = 4f;
			Item.damage = 40;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<CavernMaulerProjectile>();
			Item.shootSpeed = 15.1f;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Melee;
			Item.crit = 9;
			Item.channel = true;
		}
	}
}