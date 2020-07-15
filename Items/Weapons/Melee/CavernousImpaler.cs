using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class CavernousImpaler : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavernous Impaler");
			Tooltip.SetDefault("Fires a crystal that explodes on impact");
		}
		public override void SetDefaults()
		{
			item.damage = 40;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useAnimation = 18;
			item.useTime = 24;
			item.shootSpeed = 3.7f;
			item.knockBack = 6.5f;
			item.width = 32;
			item.height = 32;
			item.scale = 1f;
			item.rare = ItemRarityID.Green;
			item.value = Item.sellPrice(0, 1, 0, 0);
			item.melee = true;
			item.noMelee = true; 
			item.noUseGraphic = true;
			item.autoReuse = true;

			item.UseSound = SoundID.Item1;
			item.shoot = ProjectileType<CavernousImpalerProjectile>();
		}
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[item.shoot] < 1;
		}
	}
}