using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;

namespace AerovelenceMod.Items.Weapons.Ranged
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
            item.width = 60;
            item.height = 32; 
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 24f;
        }
    }
}