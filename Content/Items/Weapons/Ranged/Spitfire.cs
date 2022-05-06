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
			Item.UseSound = SoundID.Item12;
			Item.crit = 8;
            Item.damage = 50;
            Item.reuseDelay = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 5;
			Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.ScutlixLaserFriendly;
            Item.shootSpeed = 16f;
        }
    }
}