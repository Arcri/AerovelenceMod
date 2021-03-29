using System.Collections.Generic;
using AerovelenceMod.Common.Globals.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class PrismaticSoul : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Soul");
            Tooltip.SetDefault("Damage and defense are increased while in the Crystal Caverns" +
                               "\nIncreases movement speed slightly" +
                               "\nYou are now lighter, making you able to jump higher" +
                               "\nExpert");
            
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.accessory = true;

            item.rare = ItemRarityID.Expert;

            item.value = Item.sellPrice(0, 2, 50);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
            tooltips.Add(line);

            foreach (TooltipLine line2 in tooltips)
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.5f;
            player.jumpSpeedBoost += 1.5f;

            if (player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns)
            {
                player.statDefense += 5;
                player.allDamage += 0.3f;
            }
        }
    }
}