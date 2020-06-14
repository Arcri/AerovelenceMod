using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MandiBlaster : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mandi-Blaster");
			Tooltip.SetDefault("My name is Jim... But people call me Jim!");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item41;
			item.crit = 6;
            item.damage = 16;
            item.ranged = true;
            item.width = 60;
            item.height = 32; 
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 25, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
			item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
        }
    }
}