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
            item.damage = 34;
            item.magic = true;
            item.mana = 5;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
			item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = ProjectileID.ScutlixLaserFriendly;
            item.shootSpeed = 7f;
        }
    }
}