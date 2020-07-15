using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class PlaroveneBlaster : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plarovene Blaster");
			Tooltip.SetDefault("I don't know what Plarovene means but that's what the box said");
		}
        public override void SetDefaults()
        {
			item.crit = 20;
            item.damage = 56;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item14;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.ScutlixLaser;
            item.shootSpeed = 82f;
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Plarovene Blaster", "Legendary item")
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
		
		
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 1);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = randomSpread(speedX, speedY, 5, 5);
            for (int i = 0; i < 5; ++i)
            {
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}