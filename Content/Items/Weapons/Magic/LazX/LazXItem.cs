using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Weapons.Magic.LazX
{
	public class LazXItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Laz-X");
			Tooltip.SetDefault("Forged from the finest metals \nLocks onto enemies near the cursor \nPrefers newer enemies");
		}

		public override void SetDefaults() {

			item.damage = 108;
			item.magic = true;
			item.width = 50;
			item.height = 50;
			item.useTime = 5;
			item.useAnimation = 5;
			item.reuseDelay = 5; 
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.channel = true;
			item.noMelee = true;
			item.knockBack = 2;
			item.value = 60000;
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<LazXHeldProj>();
			item.shootSpeed = 10f;
			item.noUseGraphic = true;

		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "Its a doggy dog world out there...");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Laz-X", "Artifact weapon")
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