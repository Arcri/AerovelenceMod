using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class ThunderLance : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thunder Lance");
			Tooltip.SetDefault("Conjours up energy from the storms to fire electricity sparks\nMore powerful during Crystal Torrents");
		}
		public override void SetDefaults()
		{
			Item.damage = 65;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.shootSpeed = 3.7f;
			Item.knockBack = 6.5f;
			Item.width = 58;
			Item.height = 58;
			Item.scale = 1f;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true; 
			Item.noUseGraphic = true;
			Item.autoReuse = true;

			Item.UseSound = SoundID.Item1;
			Item.shoot = ProjectileType<ThunderLanceProjectile>();
		}
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}
	}
}