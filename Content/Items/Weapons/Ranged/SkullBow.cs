using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class SkullBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Bow");
			Tooltip.SetDefault("Converts arrows into bones");
		}
		public override void SetDefaults()
		{
			Item.shootSpeed = 24f;
			Item.crit = 6;
			Item.damage = 13;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 26;
			Item.height = 42;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.UseSound = SoundID.Item5;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 3, 80, 0);
			Item.rare = ItemRarityID.Purple;
			Item.shoot = ProjectileID.Bone;
			Item.useAmmo = AmmoID.Arrow;
			Item.autoReuse = true;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = Main.rand.Next(new int[] { type, type, type, ProjectileID.Skull });
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Skull Bow", "Legendary item")
			{
				OverrideColor = new Color(255, 241, 000)
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