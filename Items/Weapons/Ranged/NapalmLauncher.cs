using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class NapalmLauncher : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Napalm Launcher");
			Tooltip.SetDefault("The sun god's flame, encapsuled into something usable");
		}
        public override void SetDefaults()
        {
			item.crit = 11;
            item.damage = 125;
            item.ranged = true;
            item.width = 124;
            item.height = 88;
            item.useTime = 12;
            item.useAnimation = 12;
			item.UseSound = SoundID.Item11;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 11, 40, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.Flames;
            item.shootSpeed = 10f;
        }

        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return (Vector2[])posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 3, 3);
            for (int i = 0; i < 3; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
		}
			
    	public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Napalm Launcher", "Legendary item")
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

			// Here we will remove all tooltips whose title end with ':RemoveMe'
			// One like that is added at the start of this method
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
	}
}