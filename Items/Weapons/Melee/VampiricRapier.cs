using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class VampiricRapier : ModItem
    {
				public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vampiric Rapier");
			Tooltip.SetDefault("Must've been a previous traveller's...");
		}
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 33;
            item.ranged = true;
            item.width = 124;
            item.height = 88;
            item.useTime = 11;
            item.useAnimation = 11;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
        }
		
		
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Player p = Main.player[Main.myPlayer];
            int healingAmount = damage / 20;
            p.statLife += healingAmount;
            p.HealEffect(healingAmount, true);
        }
				public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwooooooah");
			tooltips.Add(line);

			line = new TooltipLine(mod, "VampiricRapier", "Legendary item")
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