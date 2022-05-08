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
            Item.crit = 8;
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("MothLegProjectile").Type;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 20f;
        }


        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
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
            Projectile.width = 90;
            Projectile.height = 82;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            // projectile.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            int num622 = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y), Projectile.width, Projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            Projectile.rotation += 0.05f;
        }
        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            if (Projectile.tileCollide)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<MothLegProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
                if (Main.netMode != NetmodeID.Server)
                {
                    AerovelenceMod.primitives.CreateTrail(new LegPrimTrail(Main.projectile[proj]));
                }
                target.immune[Projectile.owner] = 3;
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
			get => Main.npc[(int)Projectile.ai[1]];
			set { Projectile.ai[1] = value.whoAmI; }
		}

		private Vector2 Origin {
			get => new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
			set {
				Projectile.localAI[0] = value.X;
				Projectile.localAI[1] = value.Y;
			}
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 300;
			Projectile.aiStyle = -1;
			Projectile.height = 7;
			Projectile.width = 7;
            Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.velocity = Vector2.Zero;
			hit[Projectile.penetrate - 1] = target;
			target = TargetNext(target);
			if (target != null)
            {
                Projectile.Center = target.Center;
            }
			else
				Projectile.Kill();

			Projectile.netUpdate = true;
		}
        private NPC TargetNext(NPC current)
		{
			float range = 25 * 14;
			range *= range;
			NPC target = null;
			var center = Projectile.Center;
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