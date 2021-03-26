using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Core.Prim;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class MothLeg : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth Leg");
            Tooltip.SetDefault("Hitting an enemy can chain electricity to hit multiple enemies");
        }
        public override void SetDefaults()
        {	
            item.crit = 8;
            item.damage = 20;
            item.melee = true;
            item.width = 20;
            item.height = 30;
            item.useTime = 24;
            item.useAnimation = 24;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("MothLegProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 20f;
        }


        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class MothLegProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth Leg");
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 82;
            projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.magic = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            int num622 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X, projectile.position.Y - projectile.velocity.Y), projectile.width, projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            projectile.rotation += 0.05f;
        }
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            if (projectile.tileCollide)
            {
                int proj = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<MothLegProj2>(), projectile.damage, projectile.knockBack, projectile.owner, target.whoAmI);
                if (Main.netMode != NetmodeID.Server)
                {
                    AerovelenceMod.primitives.CreateTrail(new LegPrimTrail(Main.projectile[proj]));
                }
                target.immune[projectile.owner] = 3;
            }
        }
    }
    public class MothLegProj2 : ModProjectile
    {
        NPC[] hit = new NPC[8];
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electricity");
		}

        private NPC Target {
			get => Main.npc[(int)projectile.ai[1]];
			set { projectile.ai[1] = value.whoAmI; }
		}

		private Vector2 Origin {
			get => new Vector2(projectile.localAI[0], projectile.localAI[1]);
			set {
				projectile.localAI[0] = value.X;
				projectile.localAI[1] = value.Y;
			}
		}

		public override void SetDefaults()
		{
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ranged = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 300;
			projectile.aiStyle = -1;
			projectile.height = 7;
			projectile.width = 7;
            projectile.tileCollide = false;
			projectile.alpha = 255;
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.velocity = Vector2.Zero;
			hit[projectile.penetrate - 1] = target;
			target = TargetNext(target);
			if (target != null)
            {
                projectile.Center = target.Center;
            }
			else
				projectile.Kill();

			projectile.netUpdate = true;
		}
        private NPC TargetNext(NPC current)
		{
			float range = 25 * 14;
			range *= range;
			NPC target = null;
			var center = projectile.Center;
			for (int i = 0; i < 200; ++i) {
				NPC npc = Main.npc[i];
				//if npc is a valid target (active, not friendly, and not a critter)
				if (npc != current && npc.active && npc.CanBeChasedBy(null) && CanTarget(npc)) {
					float dist = Vector2.DistanceSquared(center, npc.Center);
					if (dist < range) {
						range = dist;
						target = npc;
					}
				}
			}
			return target;
		}
        private bool CanTarget(NPC target)
		{
			foreach (var npc in hit)
				if (target == npc)
					return false;
			return true;
		}
    }
}