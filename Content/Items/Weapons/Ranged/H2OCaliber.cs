using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class H2OCaliber : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("H2O Caliber");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 12;
            item.ranged = true;
            item.width = 44;
            item.height = 26;
            item.useTime = 20;
            item.useAnimation = 20;
			item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 2;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
			item.shoot = ProjectileID.FlaironBubble;
            item.autoReuse = true;
            item.shootSpeed = 7f;
        }
    }
}