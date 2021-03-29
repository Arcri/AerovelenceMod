using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Spitfire : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spitfire");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item12;
			item.crit = 8;
            item.damage = 50;
            item.reuseDelay = 10;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 5;
			item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = ProjectileID.ScutlixLaserFriendly;
            item.shootSpeed = 16f;
        }
    }
}