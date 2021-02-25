using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Spitfire : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spitfire");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item5;
			item.crit = 9;
            item.damage = 26;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
			item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.ScutlixLaserFriendly;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 7f;
        }
    }
}