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
			item.damage = 65;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 20;
			item.useTime = 20;
			item.shootSpeed = 3.7f;
			item.knockBack = 6.5f;
			item.width = 58;
			item.height = 58;
			item.scale = 1f;
			item.rare = ItemRarityID.Yellow;
			item.value = Item.sellPrice(0, 1, 0, 0);
			item.melee = true;
			item.noMelee = true; 
			item.noUseGraphic = true;
			item.autoReuse = true;

			item.UseSound = SoundID.Item1;
			item.shoot = ProjectileType<ThunderLanceProjectile>();
		}
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[item.shoot] < 1;
		}
	}
}