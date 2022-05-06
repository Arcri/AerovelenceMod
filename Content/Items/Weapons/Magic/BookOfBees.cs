using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
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
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.width = 36;
            Item.height = 48;
            Item.useTime = Item.useAnimation = 30; 
            Item.UseSound = SoundID.Item42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HiveProjectile>();
            Item.shootSpeed = 12f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "Why do I exist, dawg");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Book of Bees", "Artifact weapon")
            {
                overrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }
    }
}