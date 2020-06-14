using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
    public class PrismaticSoul : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prismatic Soul");
			Tooltip.SetDefault("Slightly increases all damage\nIncreases defense by 5\nIncreases movement speed slightly\nYou are now lighter, making you able to jump higher\nExpert");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override void SetDefaults()
        {
			item.accessory = true;
            item.width = 38;
            item.height = 46;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Expert;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeDamage *= 1.1f;
            player.rangedDamage *= 1.1f;
            player.magicDamage *= 1.1f;
            player.minionDamage *= 1.1f;
            player.statDefense += 5;
            player.moveSpeed += 0.5f;
            player.jumpSpeedBoost += 1.5f;
        }
    }
}