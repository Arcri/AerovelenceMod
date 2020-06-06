using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class BookOfBees : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Book of Bees");
			Tooltip.SetDefault("Wait, those arn't bees...\nRelic");
		}
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 28;
            item.magic = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 10;
            item.useAnimation = 10;
			item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.Wasp;
            item.shootSpeed = 12f;
        }
    }
}