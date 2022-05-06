using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Stinger : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stinger");
		}
        public override void SetDefaults()
        {
			Item.crit = 4;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 26;
            Item.useTime = 20;
            Item.useAnimation = 20;
			Item.UseSound = SoundID.Item5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
			Item.shoot = ProjectileID.StyngerShrapnel;
            Item.autoReuse = true;
            Item.shootSpeed = 7f;
        }
    }
}