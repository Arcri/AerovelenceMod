using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class BookOfBees : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Book of Bees");
            Tooltip.SetDefault("Wait, those aren't bees...");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 15;
            item.magic = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item82;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.Wasp;
            item.shootSpeed = 12f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(mod, "Verbose:RemoveMe", "Why do I exist, dawg");
            tooltips.Add(line);

            line = new TooltipLine(mod, "Book of Bees", "Legendary item")
            {
                overrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }
    }
}