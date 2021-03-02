using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Crystallizer : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystallizer");
			Tooltip.SetDefault("Rains down on enemies when near an enemy");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 2;
            item.damage = 22;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 22;
            item.useAnimation = 22;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 80, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("CrystallizerProj");
            item.shootSpeed = 2f;
        }
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 6);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class CrystallizerProj : ModProjectile
    {
		private int shootTimer;
		public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 6;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 216f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 13f;
        }
        public override void AI()
        {
			
			float distance = 192f;
			bool npcNearby = false;
			for (int k = 0; k < 200; k++)
			{
				if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
				{
					Vector2 newMove = Main.npc[k].Center - projectile.Center;
					float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
					if (distanceTo < distance)
					{
						distance = distanceTo;
						npcNearby = true;
					}

				}

			}
			shootTimer++;
			if (shootTimer >= Main.rand.Next(20, 30))
				if (npcNearby)
				{

					{
						int type = mod.ProjectileType("CrystallizerProjectile");
						Vector2 offset = projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
						Projectile.NewProjectileDirect(offset, new Vector2(Main.rand.NextFloat(-1f, 1f), -5f + Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<CrystallizerProjectile>(), 5, 0.5f, Main.myPlayer);
						shootTimer = 0;
					}
				}

		}
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{
	partial class CrystallizerProjectile : ModProjectile
	{
		private void ApplyTrailFx()
		{
			Projectile proj = projectile;
			for (int dusts = 0; dusts < 1; dusts++)
			{
				int castAheadDist = 6;
				var pos = new Vector2(
					proj.position.X + castAheadDist,
					proj.position.Y + castAheadDist
                );

				for (int subDusts = 0; subDusts < 3; subDusts++)
				{
					float dustCastAheadX = proj.velocity.X / 3f * subDusts;
					float dustCastAheadY = proj.velocity.Y / 3f * subDusts;

					int dustIdx = Dust.NewDust(
						Position: pos,
						Width: proj.width - castAheadDist * 2,
						Height: proj.height - castAheadDist * 2,
						Type: 59,
						SpeedX: 0f,
						SpeedY: 0f,
						Alpha: 100,
						newColor: default,
						Scale: 1.2f
					);

					Main.dust[dustIdx].noGravity = true;
					Main.dust[dustIdx].velocity *= 0.3f;
					Main.dust[dustIdx].velocity += proj.velocity * 0.5f;

					Dust dust = Main.dust[dustIdx];
					dust.position.X -= dustCastAheadX;
					dust.position.Y -= dustCastAheadY;
				}

				if (Main.rand.Next(8) == 0)
				{
					int dustIdx = Dust.NewDust(
						Position: pos,
						Width: proj.width - castAheadDist * 2,
						Height: proj.height - castAheadDist * 2,
						Type: 60,
						SpeedX: 0f,
						SpeedY: 0f,
						Alpha: 100,
						newColor: default,
						Scale: 0.75f
					);
					Main.dust[dustIdx].velocity *= 0.5f;
					Main.dust[dustIdx].velocity += proj.velocity * 0.5f;
				}
			}
		}
	}
}
namespace AerovelenceMod.Items.Weapons.Melee
{
	partial class CrystallizerProjectile : ModProjectile
	{
		private static int AquaSceptreAiStyle;
		public override string Texture => "Terraria/Projectile_" + ProjectileID.WaterStream;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Spray");

			var aquaSceptreProj = new Projectile();
			aquaSceptreProj.SetDefaults(ProjectileID.WaterStream);

            AquaSceptreAiStyle = aquaSceptreProj.aiStyle;
		}
		public override void SetDefaults()
		{
			projectile.width = 12;
			projectile.damage = 10;
			projectile.height = 12;
			projectile.tileCollide = true;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.melee = true;
			projectile.aiStyle = 2;
			projectile.alpha = 255;
			projectile.timeLeft = 600;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.extraUpdates = 1;
		}
		public override bool PreAI()
		{
			ApplyTrailFx();
			projectile.velocity.Y += 0.2f;
			return false;
		}
    }
}