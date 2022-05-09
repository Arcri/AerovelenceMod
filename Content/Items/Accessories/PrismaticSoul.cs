using System.Collections.Generic;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Biomes;
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
                               "\nYou are now lighter, making you able to jump higher\n+1 Summon Slot" +
                               "\nExpert");
            
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.rare = ItemRarityID.Expert;

            Item.value = Item.sellPrice(0, 2, 50);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
            tooltips.Add(line);

            foreach (TooltipLine line2 in tooltips)
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                    line2.OverrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.5f;
            player.jumpSpeedBoost += 1.5f;
            player.maxMinions += 1;

            if (player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()))
            {
                player.statDefense += 5;
                player.GetDamage(DamageClass.Generic) += 0.3f;
            }
        }
    }
}