using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using AerovelenceMod;
using AerovelenceMod.Core.Prim;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
	public class ElectricRailgunProj1 : ModProjectile
	{
		private const float CHARGERATE = 0.01f;

		private float charge = 0;

		private bool charged = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electric Railgun");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999;
			Projectile.ignoreWater = true;
			Projectile.hide = true;
		}
        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundLoader.customSoundType, (int)Projectile.position.X, (int)Projectile.position.Y, Mod.GetSoundSlot(SoundType.Custom, "Sounds/Effects/AnnihilatorCharge"));
            }

            
            Player player = Main.player[Projectile.owner];
			player.ChangeDir(Main.MouseWorld.X > player.position.X ? 1 : -1);

			player.itemTime = 45; // Set item time to 2 frames while we are used
			player.itemAnimation = 45; // Set item animation time to 2 frames while we are used
			Projectile.position = player.Center;
			Projectile.velocity = Vector2.Zero;

			Vector2 direction = Main.MouseWorld - player.Center;
			direction.Normalize();

			player.itemRotation = direction.ToRotation();
			if (player.direction != 1)
				player.itemRotation -= 3.14f;

			if (player.channel)
			{
                if (charge < 1)
                {
                    charge += CHARGERATE;
                    Vector2 dustUnit = direction.RotatedBy(Main.rand.NextFloat(-1, 1)) * 16;
                    Vector2 dustOffset = player.Center + (direction * 70) + player.velocity;
                    Dust dust = Dust.NewDustPerfect(dustOffset + dustUnit, 226);
                    dust.velocity = Vector2.Zero - (dustUnit * 0.1f);
                    dust.noGravity = true;
                    dust.scale = (float)Math.Sqrt(charge);
                }
                else if (!charged)
                {
                    charged = true;
                }
			}
			else
			{
                //Play firing sound!
                SoundEngine.PlaySound(SoundLoader.customSoundType, (int)Projectile.position.X, (int)Projectile.position.Y, Mod.GetSoundSlot(SoundType.Custom, "Sounds/Effects/AnnihilatorShot"));
                Projectile proj = Projectile.NewProjectileDirect(player.Center + (direction * 70), direction * 15, ModContent.ProjectileType<ElectricRailgunProj2>(), (int)(Projectile.damage * MathHelper.Lerp(0.5f, 1.5f, charge)), 0, player.whoAmI, charge);
				if (Main.netMode != NetmodeID.Server)
			    {
                    ElectricRailgunPrimTrail trail = new ElectricRailgunPrimTrail(proj, charge);
                    AerovelenceMod.primitives.CreateTrail(trail);
                    var mp = proj.modProjectile as ElectricRailgunProj2;
                    mp.trail = trail;
                }
				player.GetModPlayer<AeroPlayer>().Shake += (int)(16 * charge);
				Projectile.active = false;
			}

		}
	}
	public class ElectricRailgunProj2 : ModProjectile
	{

		private List<NPC> hit = new List<NPC>();
		private float Charge => Projectile.ai[0];

        public ElectricRailgunPrimTrail trail;
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electric Railgun");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
			Projectile.ignoreWater = true;
			Projectile.hide = true;
            Projectile.extraUpdates = 10;
		}

        private float rotDifference(float angleA, float angleB) => Math.Abs(((((angleA - angleB) % 6.28f) + 9.42f) % 6.28f) - 3.14f);

        public override void AI()
        {
			var target = Main.npc.Where(n => n.active && !n.friendly && !hit.Contains(n) && !n.immortal && !n.dontTakeDamage).OrderBy(n => (Projectile.Center - n.Center).Length()).FirstOrDefault();
            if (target != default)
            {
                Vector2 direction = target.Center - Projectile.Center;
                if (direction.Length() < 400 && (hit.Count > 0 || rotDifference(Projectile.DirectionTo(target.Center).ToRotation(), Projectile.velocity.ToRotation()) < 1f))
                {
                    direction.Normalize();
                    direction *= 15;
                    Projectile.velocity = direction;
                    Projectile.timeLeft = 60;
                }
                else if (hit.Count > 0)
                    Projectile.active = false;
            }
            else if (hit.Count > 0)
                Projectile.active = false;

            trail?.AddPoint();
            if (trail != null)
                trail.TrailLength += Projectile.velocity.Length();
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (hit.Contains(target))
				return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (hit.Count < (int)(Charge * 5) + 400)
				Projectile.penetrate++;
            hit.Add(target);
        }
    }
    public class ElectricRailgunPrimTrail : PrimTrail
    {

        private float charge;

        public float TrailLength;

        private int deathCounter;
        public ElectricRailgunPrimTrail(Projectile projectile, float Charge) : base(projectile)
        {
            _projectile = projectile;
            _points.Add(_projectile.Center);
            _points.Add(_projectile.Center);
            charge = Charge;
        }
        public override void SetDefaults()
        {
            _width = 20;
            _cap = 3000;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            /*if (_noOfPoints <= 1) return; //for easier, but less customizable, drawing
            float colorSin = (float)Math.Sin(_counter / 3f);
            Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
            float widthVar = (float)Math.Sqrt(_points.Count) * _width;
            DrawBasicTrail(c1, widthVar);*/

            if (_noOfPoints <= 6) return;
            float widthVar;
            for (int i = 0; i < _points.Count; i++)
            {
                widthVar = _width;
                if (i == 0)
                {
                    Color c1 = _counter % 33 > 20 && _counter % 33 < 32 ? Color.White : Color.Lerp(Color.Cyan, Color.White, Math.Min((widthVar - _width) / 2f, 1));
                    Vector2 normalAhead = CurveNormal(_points, i + 1);
                    Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                    Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;
                    AddVertex(_points[i], c1, new Vector2(0, 0.5f));
                    AddVertex(secondUp, c1, new Vector2((float)(i + 1) / (float)_points.Count(), 0));
                    AddVertex(secondUp, c1, new Vector2((float)(i + 1) / (float)_points.Count(), 1));
                }
                else
                {
                    if (i != _points.Count - 1)
                    {
                        Color c = _counter % 33 > 20 && _counter % 33 < 32 ? Color.White : Color.Lerp(Color.Cyan, Color.White, Math.Min((widthVar - _width) / 2f, 1));
                        Vector2 normal = CurveNormal(_points, i);
                        Vector2 normalAhead = CurveNormal(_points, i + 1);
                        Vector2 firstUp = _points[i] - normal * widthVar;
                        Vector2 firstDown = _points[i] + normal * widthVar;
                        Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                        Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                        AddVertex(firstDown, c, new Vector2((float)(i / (float)_points.Count()), 1));
                        AddVertex(firstUp, c, new Vector2((float)(i / (float)_points.Count()), 0));
                        AddVertex(secondDown, c, new Vector2((float)(i + 1) / (float)_points.Count(), 1));

                        AddVertex(secondUp, c, new Vector2((float)(i + 1) / (float)_points.Count(), 0));
                        AddVertex(secondDown, c, new Vector2((float)(i + 1) / (float)_points.Count(), 1));
                        AddVertex(firstUp, c, new Vector2((float)(i / (float)_points.Count()), 0));
                    }
                    else
                    {

                    }
                }
            }
        }
        public override void SetShaders()
        {
            Effect effect = AerovelenceMod.RailgunShader;
            effect.Parameters["baseTexture"].SetValue(ModContent.GetInstance<AerovelenceMod>().Assets.Request<Texture2D>("Assets/GlowTrail").Value);
            effect.Parameters["pnoise"].SetValue(ModContent.GetInstance<AerovelenceMod>().Assets.Request<Texture2D>("Assets/noise").Value);
            effect.Parameters["vnoise"].SetValue(ModContent.GetInstance<AerovelenceMod>().Assets.Request<Texture2D>("Assets/vnoise").Value);
            effect.Parameters["wnoise"].SetValue(ModContent.GetInstance<AerovelenceMod>().Assets.Request<Texture2D>("Assets/wnoise").Value);
            effect.Parameters["repeats"].SetValue(TrailLength / 1000f);
            PrepareShader(effect, "MainPS", _counter * 0.6f);
        }
        public override void OnUpdate()
        {
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);
            }
            if ((!_projectile.active && _projectile != null) || _destroyed)
            {
                OnDestroy();
            }
            else
            {
                AddPoint();
            }
        }
        
        public void AddPoint()
        {
            _points.Add(_projectile.Center);
        }
        public override void OnDestroy()
        {
            _destroyed = true;
            if (deathCounter++ > 40)
                _width *= 0.75f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }
    }
}
