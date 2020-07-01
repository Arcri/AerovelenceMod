using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class GlassPulseBow : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glass Pulse Bow");
		}
        public override void SetDefaults()
        {
			item.shootSpeed = 24f;
			item.crit = 8;
            item.damage = 30;
            item.ranged = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
			item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 35, 20);
            item.rare = ItemRarityID.Green;
			item.shoot = ProjectileID.LunarFlare;
            item.autoReuse = true;
        }
    }
}