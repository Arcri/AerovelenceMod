using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Reaper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper");
            Tooltip.SetDefault("Fires lost souls when near an enemy");
        }
        public override void SetDefaults()
        {
            Item.channel = true;		
            Item.crit = 2;
            Item.damage = 22;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("ReaperProj").Type;
            Item.shootSpeed = 2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ReaperProj : ModProjectile
    {
        private int shootTimer;
        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 6;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 216f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
        }
        public override void AI()
        {
			
            float distance = 192f;
            bool npcNearby = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
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
                        float speed = 3f;
                        int type = Mod.Find<ModProjectile>("ReaperProjectile").Type;
                        Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, type, Projectile.damage, 5f, Projectile.owner);
                        shootTimer = 0;
                    }
                }

        }
    }

    partial class ReaperProjectile : ModProjectile
    {
        private void ApplyTrailFx()
        {
            Projectile proj = this.Projectile;
            for (int dusts = 0; dusts < 1; dusts++)
            {
                int castAheadDist = 6;
                var pos = new Vector2(
                    proj.position.X + (float)castAheadDist,
                    proj.position.Y + (float)castAheadDist
                );

                for (int subDusts = 0; subDusts < 3; subDusts++)
                {
                    float dustCastAheadX = proj.velocity.X / 3f * (float)subDusts;
                    float dustCastAheadY = proj.velocity.Y / 3f * (float)subDusts;

                    int dustIdx = Dust.NewDust(
                        Position: pos,
                        Width: proj.width - castAheadDist * 2,
                        Height: proj.height - castAheadDist * 2,
                        Type: 60,
                        SpeedX: 0f,
                        SpeedY: 0f,
                        Alpha: 100,
                        newColor: default(Color),
                        Scale: 1.2f
                    );

                    Main.dust[dustIdx].noGravity = true;
                    Main.dust[dustIdx].velocity *= 0.3f;
                    Main.dust[dustIdx].velocity += proj.velocity * 0.5f;

                    Dust dust = Main.dust[dustIdx];
                    dust.position.X = dust.position.X - dustCastAheadX;
                    dust.position.Y = dust.position.Y - dustCastAheadY;
                }

                if (Main.rand.NextBool(8))
                {
                    int dustIdx = Dust.NewDust(
                        Position: pos,
                        Width: proj.width - castAheadDist * 2,
                        Height: proj.height - castAheadDist * 2,
                        Type: 60,
                        SpeedX: 0f,
                        SpeedY: 0f,
                        Alpha: 100,
                        newColor: default(Color),
                        Scale: 0.75f
                    );
                    Main.dust[dustIdx].velocity *= 0.5f;
                    Main.dust[dustIdx].velocity += proj.velocity * 0.5f;
                }
            }
        }
    }

    partial class ReaperProjectile : ModProjectile
    {
        private static int SpectreStaffAiStyle;
        private static int AquaSceptreAiStyle;



        ////////////////

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WaterStream;



        ////////////////

        public override void SetStaticDefaults()
        {
            this.DisplayName.SetDefault("Crystal Spray");

            var spectreStaffProj = new Projectile();
            spectreStaffProj.SetDefaults(ProjectileID.LostSoulFriendly);
            var aquaSceptreProj = new Projectile();
            aquaSceptreProj.SetDefaults(ProjectileID.WaterStream);

            ReaperProjectile.SpectreStaffAiStyle = spectreStaffProj.aiStyle;
            ReaperProjectile.AquaSceptreAiStyle = aquaSceptreProj.aiStyle;
        }

        public override void SetDefaults()
        {
            //this.projectile.CloneDefaults( ProjectileID.WaterStream );  // clones aqua sceptre unless aiStyle changes?

            //this.projectile.aiStyle = 0;	// does nothing
            //this.projectile.aiStyle = CrystalSprayProjectile.SpectreStaffAiStyle;	// both aiStyle and aiType needed?
            //this.aiType = ProjectileID.LostSoulFriendly;

            Projectile.width = 12;
            Projectile.damage = 10;
            Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          //Can the projectile collide with tiles?
            //this.projectile.penetrate = -1;           //How many monsters the projectile can penetrate.
            this.Projectile.extraUpdates = 1;   // 2 = aqua sceptre
        }


        ////////////////

        public override bool PreAI()
        {
            //Dust.QuickDust( this.projectile.Center, Color.Red );
            this.ApplyTrailFx();
            this.RunHomingAI();
            return false;
        }
        /*public override bool PreDrawExtras( SpriteBatch spriteBatch ) {
			var tempAI = this.projectile.ai.Clone();
			var tempLocalAI = this.projectile.localAI.Clone();

			this.projectile.type = ProjectileID.WaterStream;
			this.projectile.aiStyle = CrystalSprayProjectile.AquaSceptreAiStyle;
			//this.projectile.CloneDefaults( ProjectileID.WaterStream );

			this.projectile.ai = (float[])tempAI;
			this.projectile.localAI = (float[])tempLocalAI;

			return base.PreDrawExtras( spriteBatch );
		}

		public override void PostDraw( SpriteBatch spriteBatch, Color lightColor ) {
			var tempAI = this.projectile.ai.Clone();
			var tempLocalAI = this.projectile.localAI.Clone();

			this.projectile.type = ModContent.ProjectileType<CrystalSprayProjectile>();
			this.projectile.aiStyle = CrystalSprayProjectile.SpectreStaffAiStyle;
			//this.projectile.CloneDefaults( ModContent.ProjectileType<CrystalSprayProjectile>() );

			this.projectile.ai = (float[])tempAI;
			this.projectile.localAI = (float[])tempLocalAI;
		}*/
    }

    partial class ReaperProjectile : ModProjectile
    {
        private void RunHomingAI()
        {
            Projectile proj = this.Projectile;

            float projPosMidX = proj.position.X + (float)(proj.width / 2);
            float projPosMidY = proj.position.Y + (float)(proj.height / 2);
            float closestNpcPosX = proj.Center.X;
            float closestNpcPosY = proj.Center.Y;
            float closestNpcDistBothAxis = 400f;
            bool targetNpcFound = false;

            for (int npcWho = 0; npcWho < 200; npcWho++)
            {
                NPC npc = Main.npc[npcWho];
                if (!npc.CanBeChasedBy(proj, false))
                {
                    continue;
                }
                if (proj.Distance(npc.Center) >= closestNpcDistBothAxis)
                {
                    continue;
                }
                if (!Collision.CanHit(proj.Center, 1, 1, npc.Center, 1, 1))
                {
                    continue;
                }

                float npcPosMidX = npc.position.X + (float)(npc.width / 2);
                float npcPosMidY = npc.position.Y + (float)(npc.height / 2);

                float bothAxisDist = Math.Abs(projPosMidX - npcPosMidX) + Math.Abs(projPosMidY - npcPosMidY);
                if (bothAxisDist < closestNpcDistBothAxis)
                {
                    closestNpcDistBothAxis = bothAxisDist;
                    closestNpcPosX = npcPosMidX;
                    closestNpcPosY = npcPosMidY;
                    targetNpcFound = true;
                }
            }

            if (!targetNpcFound)
            {
                return;
            }

            Vector2 projPosMid = new Vector2(projPosMidX, projPosMidY);
            float closestNpcDistX = closestNpcPosX - projPosMid.X;
            float closestNpcDistY = closestNpcPosY - projPosMid.Y;
            float closestNpcDist = (float)Math.Sqrt((closestNpcDistX * closestNpcDistX) + (closestNpcDistY * closestNpcDistY));
            closestNpcDist = 6f / closestNpcDist;
            closestNpcDistX *= closestNpcDist;
            closestNpcDistY *= closestNpcDist;

            proj.velocity.X = ((proj.velocity.X * 20f) + closestNpcDistX) / 21f;
            proj.velocity.Y = ((proj.velocity.Y * 20f) + closestNpcDistY) / 21f;
        }
    }
}