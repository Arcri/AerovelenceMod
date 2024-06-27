/*
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using static System.Formats.Asn1.AsnWriter;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    
	public class CyverWhip : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.damage = 57;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;

			Item.shoot = ModContent.ProjectileType<CyverWhipProj>();
			Item.shootSpeed = 2; //2

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 59; 
			Item.useAnimation = 59; 
			Item.UseSound = SoundID.Item152;
			Item.noMelee = true;
			Item.noUseGraphic = true;

			Item.rare = ItemRarityID.LightRed;
		}

		public override void AddRecipes()
		{
			//CreateRecipe()
				//.AddIngredient<Items.Others.Crafting.BurnshockBar>(10)
				//.AddTile(TileID.WorkBenches)
				//.Register();
		}
        public override bool MeleePrefix()
		{
			return true;
		}
	}

    public class CyverWhipProj : ModProjectile
    {
        Vector2 WhipEndPos;
        bool shouldDust = false;
        int DustCounter = 50;

        float justHitPower = 0f;
        float starPower = 1f;

        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; // This prevents the projectile from hitting through solid tiles.
            Projectile.extraUpdates = 2; //1
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.WhipSettings.Segments = 7;
            Projectile.WhipSettings.RangeMultiplier = 2f;

        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Timer > 10)
            {

                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                if (Main.player[Projectile.owner].Distance(points[points.Count - 1]) > 50)
                {

                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");


                    int d = GlowDustHelper.DrawGlowDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, ModContent.DustType<GlowCircleQuadStar>(),
                        Color.DeepPink, 0.65f, 0.4f, 0, dustShader);
                    Main.dust[d].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 2;
                    Main.dust[d].noGravity = true;


                    //int dust = Dust.NewDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, DustID.Firework_Blue);
                    //Main.dust[dust].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 2;
                    //Main.dust[dust].noGravity = true;
                }

            }

            // VANILLA DEFAULT BEHAVIOR
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            Timer++;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2)
            {
                // Plays a whipcrack sound at the tip of the whip.
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            Projectile.damage = (int)(damageDone * 0.85f); // Multihit penalty. Decrease the damage the more enemies the whip hits.

            Player owner = Main.player[Projectile.owner];
            owner.AddBuff(ModContent.BuffType<CyverBotBuff>(), 240);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int j = 0; j < 3; j++)
            {
                Dust d = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                    Color.DeepPink, 0.5f, 0.4f, 0f,
                    dustShader);
                d.velocity *= 0.5f;
            }
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 2; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Pink); //Color.White
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, Color.HotPink, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

            Color c1 = Color.HotPink;
            Color c2 = Color.HotPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);
            #endregion

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();
            DrawLine(list);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
            float t = Timer / timeToFlyOut;
            float tipScale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 10, 26);
                Vector2 origin = new Vector2(5, 8);
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (i == list.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    scale = tipScale;
                }
                else if (i > 10)
                {
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                if (i == list.Count - 2 && Timer > 10)
                {
                    Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");
                    Main.EntitySpriteDraw(Star, pos - Main.screenPosition, Star.Frame(1, 1, 0, 0), Color.DeepPink with { A = 0 } * 1.75f, rotation, Star.Size() / 2, scale * 1f, SpriteEffects.None, 0f);
                    Main.EntitySpriteDraw(Star, pos - Main.screenPosition, Star.Frame(1, 1, 0, 0), Color.HotPink with { A = 0 } * 1.75f, rotation, Star.Size() / 2, scale * 0.75f, SpriteEffects.None, 0f);
                }

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
    public class TrojanForceBot : ModProjectile
    {
        // a whole lot of variables
        Vector2 movePos;
        Vector2 targetPos;

        NPC closestNPC;
        Player p;

        int triangleLaserTimer;
        int laserSpamTimer1;
        int laserSpamTimer2;
        int orbitTimer;
        int triLaserOrbit;

        float eyeStarRot = 0f;
        float justShotPower = 0f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }
        public ref float AI_State => ref Projectile.ai[0];
        public ref float AI_Timer => ref Projectile.ai[1];
        private enum Action
        {
            TriangleLaser,
            LaserSpam,
            PlayerOrbit,
            Glitching
        }
        public override void AI()
        {
            switch (AI_State)
            {
                case (float)Action.TriangleLaser:
                    TriangleLaser();
                    break;
                case (float)Action.LaserSpam:
                    LaserSpam();
                    break;
                case (float)Action.PlayerOrbit:
                    PlayerOrbit();
                    break;
                case (float)Action.Glitching:
                    Glitching();
                    break;
            }

            float maxDetectRadius = 1000f;

            p = Main.player[Projectile.owner];
            closestNPC = FindClosestNPC(maxDetectRadius);

            CheckActive(p);

            if (closestNPC == null)
            {
                targetPos = p.Center;

                AI_State = (float)Action.PlayerOrbit;
                AI_Timer = 0;
            }

            if (closestNPC != null)
            {
                targetPos = closestNPC.Center;

                AI_Timer++;

                if (AI_Timer <= 360)
                {
                    AI_State = (float)Action.TriangleLaser;
                }
                else if (AI_Timer >= 360 & AI_Timer <= 660)
                {
                    AI_State = (float)Action.LaserSpam;
                }
                else if (AI_Timer > 660)
                {
                    AI_Timer = 0;
                }
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.25f, 0.05f), 0f, 2f);
        }

        private void TriangleLaser()
        {
            triangleLaserTimer++;
            if (triangleLaserTimer % 60 == 0)
            {
                SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .75f, Volume = 0.075f, MaxInstances = -1 }; //1f
                SoundEngine.PlaySound(stylec, Projectile.Center);

                Vector2 offset = (Projectile.rotation + MathHelper.Pi).ToRotationVector2();
                Vector2 speed = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 7f;
                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset * 20, speed, ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
                Main.projectile[a].scale = 0.8f;
                Main.projectile[a].timeLeft = 300;
                Main.projectile[a].hostile = false;
                Main.projectile[a].friendly = true;
                Main.projectile[a].tileCollide = false;

                justShotPower = 1f;
                eyeStarRot = Main.rand.NextFloat(6.28f);

                orbitTimer += 120;
            }

            movePos = targetPos + new Vector2(200, 0).RotatedBy(MathHelper.ToRadians(orbitTimer));
            Projectile.rotation = (Projectile.Center - targetPos).ToRotation();

            Projectile.velocity = (10 * Projectile.velocity + movePos - Projectile.Center) / 20f;

            laserSpamTimer1 = 0; // just in case it's not zero
        }

        private void LaserSpam()
        {
            laserSpamTimer1++;
            Projectile.rotation = (Projectile.Center - targetPos).ToRotation();

            if (laserSpamTimer1 <= 120)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                Projectile.velocity = (10 * Projectile.velocity + (p.Center + new Vector2(0, -150)) - Projectile.Center) / 20f;
                Vector2 dustPos = Projectile.Center + Main.rand.NextVector2CircularEdge(100, 100);

                float rotation = (float)Math.Atan2(dustPos.Y - Projectile.Center.Y, dustPos.X - Projectile.Center.X);
                float speedX = (float)((Math.Cos(rotation) * 7f) * -1);
                float speedY = (float)((Math.Sin(rotation) * 7f) * -1);

                for (int i = 0; i < 4; i++)
                {
                    GlowDustHelper.DrawGlowDustPerfect(dustPos, ModContent.DustType<GlowCircleQuadStar>(), new Vector2(speedX, speedY),
                    Color.DeepPink, 0.5f, 0.4f, 0f,
                    dustShader);
                }
            }
            else if (laserSpamTimer1 >= 120 & laserSpamTimer1 <= 300)
            {
                if (laserSpamTimer1 % 5 == 0)
                {
                    SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .75f, Volume = 0.075f, MaxInstances = -1 }; //1f
                    SoundEngine.PlaySound(stylec, Projectile.Center);

                    Vector2 offset = (Projectile.rotation + MathHelper.Pi).ToRotationVector2();
                    Vector2 speed = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 7f;
                    int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + offset * 20, speed, ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
                    Main.projectile[a].scale = 0.8f;
                    Main.projectile[a].timeLeft = 300;
                    Main.projectile[a].hostile = false;
                    Main.projectile[a].friendly = true;
                    Main.projectile[a].tileCollide = false;

                    justShotPower = 1f;
                    eyeStarRot = Main.rand.NextFloat(6.28f);
                }
            }
            else if (laserSpamTimer1 >= 300)
            {
                laserSpamTimer1 = 0;
            }
            //Main.NewText("Laser spam");
        }

        private void PlayerOrbit()
        {
            orbitTimer++;

            movePos = targetPos + new Vector2(300, 0).RotatedBy(MathHelper.ToRadians(orbitTimer));
            Projectile.rotation = (Projectile.Center - targetPos).ToRotation();

            Projectile.velocity = (10 * Projectile.velocity + movePos - Projectile.Center) / 20f;

            //Main.NewText("Player orbit");

        }

        private void Glitching()
        {
            //wip
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<CyverBotBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<CyverBotBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            if (owner.ownedProjectileCounts[ModContent.ProjectileType<TrojanForceBot>()] > 1)
            {
                Projectile.Kill();
            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            //SpawnGores
            int g1 = Gore.NewGore(null, Projectile.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore1>(), Scale: 1f);
            int g2 = Gore.NewGore(null, Projectile.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore2>(), Scale: 1f);
            int g3 = Gore.NewGore(null, Projectile.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore3>(), Scale: 1f);


            for (double m = 0; m < 6.28; m += 1)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f);
                dust.color = Color.DeepPink;
                dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                dust.scale = 0.4f;


                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.05f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 1f, fadePower: 0.87f, shouldFadeColor: false);

            }

            SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_44") with { Pitch = 0f, PitchVariance = 0, MaxInstances = -1, Volume = 0.1f };
            SoundEngine.PlaySound(style, Projectile.Center);

        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {

                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);


                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }

                }

            }
            return closestNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY) + new Vector2(0, 4);
                Color newDrawCol = Color.DeepPink * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, newDrawCol with { A = 0 } * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }

            Vector2 backGlowPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(0, 4);
            for (int a = 0; a < 4; a++)
            {
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, backGlowPos + Main.rand.NextVector2Circular(2.25f, 2.25f), sourceRectangle, Color.DeepPink with { A = 0 } * 1.5f, Projectile.rotation, drawOrigin, Projectile.scale * 1.1f, effects, 0f);
            }
            return true;

        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Texture2D eyeStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, 4), sourceRectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);


            Main.EntitySpriteDraw(eyeStar, Projectile.Center - Main.screenPosition + (Projectile.rotation.ToRotationVector2() * 22f * Projectile.direction) + new Vector2(0, 4), null, Color.DeepPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.85f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(eyeStar, Projectile.Center - Main.screenPosition + (Projectile.rotation.ToRotationVector2() * 22f * Projectile.direction) + new Vector2(0, 4), null, Color.HotPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.6f, SpriteEffects.None, 0f);

        }
    }

    #region NPCbot
    /*
    public class TrojanForceBot : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 99999;
            NPC.damage = 10;
            NPC.defense = 999;
            NPC.width = 66;
            NPC.height = 40;
            NPC.boss = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.alpha = 0;
            NPC.scale = 1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Texture2D eyeStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!shouldHide)
                Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0, 4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);


            Main.EntitySpriteDraw(eyeStar, NPC.Center - Main.screenPosition + (NPC.rotation.ToRotationVector2() * 22f * NPC.direction) + new Vector2(0, 4), null, Color.DeepPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.85f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(eyeStar, NPC.Center - Main.screenPosition + (NPC.rotation.ToRotationVector2() * 22f * NPC.direction) + new Vector2(0, 4), null, Color.HotPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.6f, SpriteEffects.None, 0f);

        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
            }
        }

        int timer = 0;
        int advancer = 0;

        bool shouldHide = false;
        public bool bugged = false;

        float justShotPower = 0f;
        float eyeStarRot = 0f;

        public float rotIntensity = 0.02f;
        public float angleToCover = 120f;


        //(New Point; Shoot)x4 -> Charge -> Spam -> Bug -? Repeat but on player
        public override void AI()
        {
            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.25f, 0.05f), 0f, 2f);

            timer++;
            NPC.damage = 0;
        }

        public override bool CheckActive() { return false;  }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            if (shouldHide)
            {
                return false;
            }
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin + new Vector2(0f, NPC.gfxOffY) + new Vector2(0, 4);
                //drawPos = drawPos + (drawSpikeFrame ? new Vector2(-6, -18) : new Vector2(0, 10));
                Color newDrawCol = Color.DeepPink * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);

                if (advancer >= 2)
                    spriteBatch.Draw(texture, drawPos, NPC.frame, newDrawCol with { A = 0 } * 0.5f, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                else
                    spriteBatch.Draw(texture, drawPos, NPC.frame, newDrawCol * 0.5f, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }

            Vector2 backGlowPos = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY) + new Vector2(0, 4);

            for (int a = 0; a < 4; a++)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, backGlowPos + Main.rand.NextVector2Circular(2.25f, 2.25f), NPC.frame, Color.DeepPink with { A = 0 } * 1.5f, NPC.rotation, drawOrigin, NPC.scale * 1.1f, effects, 0f);
            }
            return true;

        }

        public void KillFX()
        {
            //SpawnGores
            int g1 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore1>(), Scale: 1f);
            int g2 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore2>(), Scale: 1f);
            int g3 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore3>(), Scale: 1f);


            for (double m = 0; m < 6.28; m += 1)
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowPixelCross>(), new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f);
                dust.color = Color.DeepPink;
                dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                dust.scale = 0.4f;


                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.05f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 1f, fadePower: 0.87f, shouldFadeColor: false);

            }

            SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_44") with { Pitch = 0f, PitchVariance = 0, MaxInstances = -1, Volume = 0.1f };
            SoundEngine.PlaySound(style, NPC.Center);

        }
    }
    
    #endregion
}
*/