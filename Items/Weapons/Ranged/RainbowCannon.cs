using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class RainbowCannon : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rainbow Cannon");
			Tooltip.SetDefault("Shoots a rainbow that stays and fires rainbow blasts alongside it");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item31;
			item.crit = 20;
            item.damage = 72;
            item.ranged = true;
            item.width = 48;
            item.height = 32;
            item.useTime = 5;
			item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
			item.value = Item.sellPrice(0, 15, 50, 0);
			item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = ProjectileID.RainbowFront;
            item.shootSpeed = 8f;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}
    }
}