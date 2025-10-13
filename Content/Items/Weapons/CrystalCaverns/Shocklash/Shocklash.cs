using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.Shocklash
{
	public class Shocklash : ModItem
	{
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/Shocklash/Shocklash";

        public override void SetDefaults()
		{
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.damage = 20;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Green;

            Item.shoot = ModContent.ProjectileType<ShocklashProjectile>();
            Item.shootSpeed = 4;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 13; 
            Item.useAnimation = 26; //26
            Item.reuseDelay = 30;

            Item.UseSound = SoundID.Item152;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

		public override void AddRecipes()
		{
			//CreateRecipe()
				//.AddIngredient<Items.Others.Crafting.BurnshockBar>(10)
				//.AddTile(TileID.WorkBenches)
				//.Register();
		}

        public override bool MeleePrefix() => true;
		
	}

    public class ShocklashProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        int whipSegments = 14;
        public Vector2 TipPosition => Projectile.WhipPointsForCollision[whipSegments - 1] + new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;

            Projectile.WhipSettings.Segments = whipSegments; //10
            Projectile.WhipSettings.RangeMultiplier = 1.45f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private LightningUtils.LightningData lightningData;
        public override void AI()
        {
            if (justTipperedTimer > 0)
            {
                justTipperedTimer--;
                return;
            }

            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            Timer++;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation * 4 <= 0)
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

            float swingProgress = Timer / swingTime;
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f)
            {

                //ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.98f, FadeVelPower: 0.92f, Pixelize: true, XScale: 1f, YScale: 0.75f); //0.91
                //dust.customData = esb;
            }

            if (Timer > 10)
            {

                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                if (Main.player[Projectile.owner].Distance(points[points.Count - 1]) > 50)
                {
                    Vector2 vel = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 3;

                    Dust dust = Dust.NewDustPerfect(points[points.Count - 1], ModContent.DustType<GlowPixelCross>(), newColor: Color.DeepSkyBlue, Scale: 0.15f);
                    dust.position -= vel * 8;

                    dust.velocity = vel;


                    //int dust = Dust.NewDust(points[points.Count - 1], Projectile.width / 2, Projectile.height / 2, ModContent.DustType<GlowPixelAlts>(), newColor: Color.DeepSkyBlue);
                    //Main.dust[dust].velocity = (points[points.Count - 1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 2;
                    //Main.dust[dust].noGravity = true;
                    //Main.dust[dust].alpha = 2;
                }

            }

            #region store TipPositions
            if (swingProgress >= 0.4f && swingProgress <= 0.85f) // 0.7f | 0.3
            {
                int trailCount = 6; //12
                List<Vector2> w_points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, w_points);

                Vector2 tip = w_points[w_points.Count - 2];
                Vector2 diff = w_points[w_points.Count - 1] - tip;
                float rotation = diff.ToRotation() - MathHelper.PiOver2;

                previousTipPostions.Add(tip);
                previousTipRotations.Add(rotation);

                if (previousTipPostions.Count > trailCount)
                    previousTipPostions.RemoveAt(0);

                if (previousTipRotations.Count > trailCount)
                    previousTipRotations.RemoveAt(0);
            }
            else
            {
                previousTipPostions.Clear();
                previousTipRotations.Clear();
            }
            #endregion

            if (swingProgress > 0.45f && swingProgress <= 0.85f)
            {
                if (lightningData == null || !lightningData.Initialized)
                {
                    lightningData = new LightningUtils.LightningData(Projectile, LightningUtils.LightningStyle.Smooth);
                    lightningData.NoiseFrequency = 1f;

                    lightningData.CoreColorOverride = Color.White;
                    lightningData.MidColorOverride = Color.SkyBlue;
                    lightningData.OuterColorOverride = Color.DeepSkyBlue;
                    lightningData.FlashColorOverride = Color.Black;
                    lightningData.DistColorOverride = Color.White;

                    lightningData.GlowIntensity = 0.5f;
                    lightningData.GlowScale = 0.05f;
                }

                LightningUtils.InitializeBetweenPoints(lightningData, TipPosition, owner.Center);
                LightningUtils.UpdateSegments(lightningData);
                LightningUtils.UpdateBranches(lightningData);
                LightningUtils.SpawnDust(lightningData);
            }

        }

        int justTipperedTimer = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            List<Vector2> points = Projectile.WhipPointsForCollision;
            Projectile.FillWhipControlPoints(Projectile, points);
            Vector2 tip = points[points.Count - 2];

            if (target.Center.Distance(tip) < 55)
            {
                for (int i = 0; i < 22 + Main.rand.Next(0, 2); i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(10f, 10f) * 1.5f;

                    Color col = Color.DeepSkyBlue;

                    Dust pa = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelatedLineSpark>(), vel,
                        newColor: col, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 0.5f);

                    pa.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.83f, preShrinkPower: 0.99f, postShrinkPower: 0.82f, timeToStartShrink: 8 + Main.rand.Next(-5, 5), killEarlyTime: 40,
                        1f, 0.5f, shouldFadeColor: false);
                }

                float rot = (target.Center - Main.player[Projectile.owner].Center).ToRotation();

                Dust star1 = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: 1.5f);
                Dust star2 = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: Color.White, Scale: 0.6f);
                star1.rotation = star2.rotation = rot;

                justTipperedTimer = 10;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.25f, Pitch = 0.45f, PitchVariance = 0.1f, MaxInstances = -1, };
                SoundEngine.PlaySound(style, tip);

                CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.55f, true, 2, 0.8f, 0.8f);

                Dust d1 = Dust.NewDustPerfect(target.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.DeepSkyBlue * 0.25f);
                d1.scale = 0.25f;
                d1.customData = cpb2;
                d1.velocity = new Vector2(-0.01f, 0f).RotatedBy(rot);

                Dust d2 = Dust.NewDustPerfect(target.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.SkyBlue * 0.25f);
                d2.customData = cpb2;
                d2.velocity = new Vector2(0.01f, 0f).RotatedBy(rot);
            }

            //target.AddBuff(ModContent.BuffType<Electrified>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(hit.Damage * 0.8f); // Multihit penalty. 
        }

        public List<float> previousTipRotations = new List<float>();
        public List<Vector2> previousTipPostions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            float swingTime = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float swingProgress = Timer / swingTime;

            if (lightningData != null && lightningData.Initialized && swingProgress > 0.45f && swingProgress <= 0.85f)
            {
                LightningUtils.DrawLightning(lightningData, Main.spriteBatch);
            }

            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            //DrawLine(list);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                DrawPixelatedStarAfterImage(false);
            });

            //DrawTipperAfterImage();

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 22, 32); //BasePart Dimensions
                Vector2 origin = new Vector2(11, 10);
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (i == list.Count - 2)
                {
                    frame.Y = 80; //Starting y of whip tip
                    frame.Height = 14; //Height of whip tip
                    origin.Y = 7;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    frame.Y = 64; //Height of third whip chain
                    frame.Height = 14;
                    origin.Y = 7;
                }
                else if (i > 5)
                {
                    frame.Y = 48; //Height of second whip chain
                    frame.Height = 14;
                    origin.Y = 7;
                }
                else if (i > 0)
                {
                    frame.Y = 32; //Height of first whip chain
                    frame.Height = 14;
                    origin.Y = 7;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                if (i == list.Count - 2)
                {
                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), frame, Color.SkyBlue with { A = 0 } * 1f, rotation, origin, scale * 1.1f, flip, 0);

                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, Color.SkyBlue with { A = 0 } * 1f, rotation, origin, scale * 1.05f, flip, 0);

                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                }
                else
                {

                    //Main.EntitySpriteDraw(texture, pos - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), frame, Color.SkyBlue with { A = 0 } * 0.5f, rotation, origin, scale, flip, 0);
                }




                pos += diff;
            }
            return false;
        }

        public void DrawPixelatedStarAfterImage(bool giveUp = false)
        {
            if (giveUp)
                return;

            Texture2D Core = CommonTextures.CrispStarPMA.Value;


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0, 0, 10, 26);
            Vector2 origin = Core.Size() / 2f;// new Vector2(5, 8);
            frame.Y = 74;
            frame.Height = 18;
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
            float t = Timer / timeToFlyOut;
            float scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true)) * 0.75f;

            for (int i = 0; i < previousTipPostions.Count; i++)
            {
                float progress = (float)i / previousTipPostions.Count;

                Vector2 tipPos = previousTipPostions[i];
                float tipRot = previousTipRotations[i] + ((float)Main.timeForVisualEffects * 0.06f);

                float colOpacity = Easings.easeInCirc(progress) * 0.5f;

                Color col = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, progress) with { A = 0 } * Easings.easeInCirc(progress) * 0.5f;

                Main.EntitySpriteDraw(Core, tipPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col, tipRot, origin, scale, flip);
                Main.EntitySpriteDraw(Core, tipPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White with { A = 0 } * colOpacity, tipRot, origin, scale * 0.5f, flip);

                //Draw two after images in between each tip
                if (i != 0)
                {
                    Vector2 previousTipPos = previousTipPostions[i - 1];
                    float previousTipRot = previousTipRotations[i - 1];

                    Vector2 inBetweenPos = Vector2.Lerp(tipPos, previousTipPos, 0.33f);
                    float inBetweenRot = MathHelper.Lerp(tipRot, previousTipRot, 0.33f);

                    Main.EntitySpriteDraw(Core, inBetweenPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col, inBetweenRot, origin, scale, flip);
                    Main.EntitySpriteDraw(Core, inBetweenPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White with { A = 0 } * colOpacity, tipRot, origin, scale * 0.5f, flip);

                    inBetweenPos = Vector2.Lerp(tipPos, previousTipPos, 0.66f);
                    inBetweenRot = MathHelper.Lerp(tipRot, previousTipRot, 0.66f);

                    Main.EntitySpriteDraw(Core, inBetweenPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col, inBetweenRot, origin, scale, flip);
                    Main.EntitySpriteDraw(Core, inBetweenPos - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, Color.White with { A = 0 } * colOpacity, tipRot, origin, scale * 0.5f, flip);

                }
            }
            if (giveUp)
                return;
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
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1f * 0.5f, (diff.Length() + 2) / frame.Height);
                Vector2 scale2 = new Vector2(1f, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color * 0.75f, rotation, origin, scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), frame, Color.DeepSkyBlue with { A = 0 }, rotation, origin, scale2, SpriteEffects.None, 0);


                pos += diff;
            }
        }
    }
}