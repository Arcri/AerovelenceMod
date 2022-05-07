using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class LazX : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laz-X");
			Tooltip.SetDefault("Forged from the finest metals");
		}
        public override void SetDefaults()
        {
			Item.crit = 11;
            Item.damage = 211;
            Item.DamageType = DamageClass.Magic;
            Item.width = 70;
            Item.height = 28; 
            Item.useTime = 70;
            Item.useAnimation = 70;
			Item.UseSound = SoundID.Item68;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 13;
			Item.value = Item.sellPrice(0, 10, 75, 0);
			Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.RubyBolt;
            Item.shootSpeed = 40f;
        }
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, 0);
		}
		
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "I have an announcement to make. Shadow the hedgehog is a bitchass motherfucker. He pissed on my fucking wife.");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Laz-X", "Legendary item")
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