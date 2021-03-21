using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class LightOfTheAncients : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Light of the Ancients");
			Tooltip.SetDefault("I feel like I'm going to break this thing...");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item41;
			item.crit = 8;
            item.damage = 28;
            item.ranged = true;
            item.width = 46;
            item.height = 28; 
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }
}