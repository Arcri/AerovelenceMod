using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class MandiBlaster : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mandi-Blaster");
			Tooltip.SetDefault("'My name is Jim, But people call me Jim!'");
		}
        public override void SetDefaults()
        {
			Item.UseSound = SoundID.Item41;
			Item.crit = 6;
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 22; 
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = AmmoID.Bullet;
			Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 24f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}