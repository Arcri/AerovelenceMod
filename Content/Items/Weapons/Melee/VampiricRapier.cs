using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class VampiricRapier : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vampiric Rapier");
			Tooltip.SetDefault("`Must've been a previous traveller's`");
		}
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 45;
            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTime = 11;
            item.useAnimation = 11;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
			item.value = Item.sellPrice(0, 10, 50, 0);
			item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
        }


		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
			if (target.type != NPCID.TargetDummy)
			{
				Player p = Main.player[Main.myPlayer];
				int healingAmount = damage / 15;
				p.statLife += healingAmount;
				p.HealEffect(healingAmount, true);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwooooooah");
			tooltips.Add(line);

			line = new TooltipLine(mod, "VampiricRapier", "Artifact")
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