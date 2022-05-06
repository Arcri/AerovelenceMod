using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class GlassDeflector : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glass Deflector");
			Tooltip.SetDefault("Has a chance to reflect a projectile\n2 shards of space crystal float around you\nExpert");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 36;
            Item.height = 44;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = -12;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statDefense -= 5;
			player.GetDamage(DamageClass.Melee) *= 1.15f;
			player.GetDamage(DamageClass.Ranged) *= 1.15f;
			player.GetDamage(DamageClass.Magic) *= 1.15f;
			player.GetDamage(DamageClass.Summon) *= 1.15f;
		}
    }
}