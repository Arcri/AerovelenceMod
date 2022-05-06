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
			Tooltip.SetDefault("`Must've been a previous traveller's`\nHeals your life by 6.66% of the damage you deal per-hit");
		}
        public override void SetDefaults()
        {
			Item.crit = 20;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 11;
            Item.useAnimation = 11;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
			Item.value = Item.sellPrice(0, 10, 50, 0);
			Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
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
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwooooooah");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "VampiricRapier", "Artifact")
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