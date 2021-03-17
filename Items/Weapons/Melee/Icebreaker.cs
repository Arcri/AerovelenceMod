using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Icebreaker : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icebreaker");
			Tooltip.SetDefault("'A forgotten hero's sword, lost in the tundra'\nHas a chance to rain ice above an enemy when hit");
			Tooltip.SetDefault("This item is unfinished!");
		}

		public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 14;
            item.melee = true;
            item.width = 50;
            item.height = 54; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 0, 40, 20);
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
			Vector2 position = target.position - Vector2.UnitY * 60;
			Vector2 velocity = target.position * 5;
			Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
			base.OnHitNPC(player, target, damage, knockBack, crit);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Icebreaker", "Legendary item")
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
namespace AerovelenceMod.Items.Weapons.Melee
{
    public class IcebreakerIcicle : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.aiStyle = -1;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
			projectile.alpha = 255;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity.Y += 3;
            projectile.rotation += projectile.velocity.X * 0.01f;
			DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
			if (projectile.alpha > 0)
			{
				Main.PlaySound(SoundID.Item9, projectile.Center);
				projectile.alpha = 0;
				projectile.scale = 1.1f;
				float num101 = 16f;
				for (int num102 = 0; num102 < num101; num102++)
				{
					Vector2 spinningpoint5 = Vector2.UnitX * 0f;
					spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num102 * ((float)Math.PI * 2f / num101)) * new Vector2(1f, 4f);
					spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());
					int num103 = Dust.NewDust(projectile.Center, 0, 0, 180);
					Main.dust[num103].scale = 1.5f;
					Main.dust[num103].noGravity = true;
					Main.dust[num103].position = projectile.Center + spinningpoint5;
					Main.dust[num103].velocity = projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
				}
			}
		}
    }
}