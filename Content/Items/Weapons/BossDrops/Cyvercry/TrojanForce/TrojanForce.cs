using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.TrojanForce
{
	public class TrojanForce : ModItem
	{
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/TrojanForce/TrojanForce";

        public override void SetDefaults()
		{ 
            Item.DefaultToWhip(ModContent.ProjectileType<TrojanForceWhipProj>(), 65, 3f, 3.75f, 34);
        }
		public override bool MeleePrefix() => true;

        bool isBlue = false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 0, ai2: isBlue ? -1 : 1f);

            isBlue = !isBlue;

            //Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 1, ai2: isBlue ? -1 : 1f);


            return false;
        }
    }
    public class TrojanForceWhipProj : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/TrojanForce/TrojanForceWhipProj";

        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public int whipSegments => 12;
        public Vector2 TipPosition => Projectile.WhipPointsForCollision[whipSegments - 1] + new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

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
            Projectile.penetrate = -1;


            Projectile.WhipSettings.Segments = whipSegments;
            Projectile.WhipSettings.RangeMultiplier = 1.5f;

            Projectile.extraUpdates = 7;
        }

        float flyTime;
        public bool HitboxActive => Utils.GetLerpValue(0.1f, 0.7f, Projectile.ai[0] / flyTime, true) * Utils.GetLerpValue(0.9f, 0.7f, Projectile.ai[0] / flyTime, true) > 0.5f;

        bool doneEffect = false;
        bool isBlue = false;
        public override void AI()
        {
            if (Timer == 0)
            {
                isBlue = Projectile.ai[2] == 1;
            }

            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            
            if (Projectile.ai[1] == 0)
                Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            else
                Projectile.spriteDirection = Projectile.velocity.X >= 0f ? -1 : 1;

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
                List<Vector2> points2 = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points2);
                SoundEngine.PlaySound(SoundID.Item153, points2[points2.Count - 1]);
            }

            float swingProgress = Timer / swingTime;

            List<Vector2> points_ = Projectile.WhipPointsForCollision;
            points_.Clear();
            Projectile.FillWhipControlPoints(Projectile, points_);
            List<Vector2> points = points_;

            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && Main.rand.NextBool(6))
            {
                int pointIndex = Main.rand.Next(points.Count - 4, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));


                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, ModContent.DustType<GlowPixelCross>(), 0f, 0f, 0, isBlue ? Color.DeepSkyBlue : Color.DeepPink);

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.9f, shouldFadeColor: false);

                dust.scale = Main.rand.NextFloat(0.35f, 0.45f);

                dust.position = points[pointIndex];
                Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
                dust.velocity *= 0.5f;
                dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f)) * 0.1f;
            }

            if (swingProgress > 0.5f && swingProgress < 0.85f && Timer % 5 == 0)
            {
                var d = Dust.NewDustPerfect(points[points.Count - 1], ModContent.DustType<GlowStarSharp>(), Vector2.Zero, 0, isBlue ? Color.DeepSkyBlue : Color.DeepPink, 0.3f);
                d.position += Main.rand.NextVector2Circular(8, 8).RotatedBy(Projectile.rotation);
                d.velocity += new Vector2(0f, -Main.rand.Next(1, 3)).RotatedBy(Projectile.rotation).RotatedByRandom(0.5f);
            }


            if (swingProgress > 0.46f && swingProgress < 1f)
            {

                Vector2 top = points[points.Count - 1] - owner.Center;
                float dir = top.ToRotation();

                float whipSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.SummonMeleeSpeed);

                if (previousTipPostions.Count > 0)
                {
                    Vector2 o = previousTipPostions[previousTipPostions.Count - 1];
                    Vector2 nv = top;
                    for (float i = 0.1f; i <= 0.1f; i += 0.1f)
                    {
                        previousTipPostions.Add(Vector2.Lerp(o, nv, i) + dir.ToRotationVector2() * (4f * whipSpeed));
                        if (previousTipPostions.Count > 60)
                        {
                            previousTipPostions.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    previousTipPostions.Add(top);
                }
            }

            trailWidth = 1f - Utils.GetLerpValue(0.85f, 0.95f, swingProgress, true);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(hit.Damage * 0.8f); // Multihit penalty. 
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float swingTime = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
            float swingProgress = Timer / swingTime;

            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                TrailDraw(false);
            });


            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D Tex = TextureAssets.Projectile[Type].Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/TrojanForce/TrojanForceWhipProjGlow").Value; //


            Vector2 pos = list[0];

            float lastRot = 0f;
            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 38, 18);
                Vector2 origin = new Vector2(19, 5);
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (false && i == list.Count - 2)
                {
                    frame.Y = 66;
                    frame.Height = 18;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOutA, out int _, out float _);
                    float t = Timer / timeToFlyOutA;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

                    origin.Y = 9;
                }
                else if (i > 10) // All 3 whip segments are identical but im still doing this in case a resprite changes that
                {
                    frame.Y = 50;
                    frame.Height = 16;
                    origin.Y = 8;
                }
                else if (i > 5)
                {
                    frame.Y = 34;
                    frame.Height = 16;
                    origin.Y = 8;

                }
                else if (i > 0)
                {
                    frame.Y = 18;
                    frame.Height = 16;
                    origin.Y = 8;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                if (true)
                {

                    float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                    Color color = Lighting.GetColor(element.ToTileCoordinates());

                    Main.EntitySpriteDraw(Tex, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                    lastRot = rotation;
                }
                pos += diff;




            }

            //Draw the handle last so it is drawn over the chain segments
            Color handleCol = Lighting.GetColor(list[0].ToTileCoordinates());

            float myRot = (list[1] - list[0]).ToRotation() - MathHelper.PiOver2;
            Main.EntitySpriteDraw(Tex, list[0] - Main.screenPosition, new Rectangle(0, 0, 38, 18), handleCol, myRot,  new Vector2(19, 0), 1f, flip, 0);
            Main.EntitySpriteDraw(Glowmask, list[0] - Main.screenPosition, new Rectangle(0, 0, 38, 18), Color.White, myRot, new Vector2(19, 0), 1f, flip, 0);

            //Who up playing wit they orb
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
            float t2 = Timer / timeToFlyOut;
            float tipScale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t2, true) * Utils.GetLerpValue(0.9f, 0.7f, t2, true));


            Texture2D orb = CommonTextures.feather_circle128PMA.Value;
            Vector2 originPoint = list[list.Count - 1] - Main.screenPosition + (lastRot + MathHelper.PiOver2).ToRotationVector2() * 5f;

            Color[] cols = { Color.LightSkyBlue * 0.75f, Color.SkyBlue * 0.525f, Color.DeepSkyBlue * 0.375f };
            if (!isBlue)
            {
                cols[0] = Color.Pink;
                cols[1] = Color.HotPink;
                cols[2] = Color.DeepPink;
            }

            float[] scales = { 0.85f, 1.6f, 2.5f };

            float orbAlpha = 0.15f;
            Vector2 orbScale = new Vector2(0.9f, 0.9f) * 0.35f * tipScale;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

            Main.EntitySpriteDraw(orb, originPoint, null, cols[0] with { A = 0 } * orbAlpha, lastRot, orb.Size() / 2f, orbScale * scales[0], SpriteEffects.None);
            Main.EntitySpriteDraw(orb, originPoint, null, cols[1] with { A = 0 } * orbAlpha, lastRot, orb.Size() / 2f, orbScale * scales[1] * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, originPoint, null, cols[2] with { A = 0 } * orbAlpha, lastRot, orb.Size() / 2f, orbScale * scales[2] * sineScale2, SpriteEffects.None);



            //Draw the tip in the actual correct position 
            Color tipCol = Lighting.GetColor(list[list.Count - 1].ToTileCoordinates());



            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(Tex, list[list.Count - 1] - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), new Rectangle(0, 66, 38, 18), Color.White with { A = 0 } * 0.4f, lastRot, new Vector2(19, 5), tipScale, flip, 0);
            }

            Main.EntitySpriteDraw(Tex, list[list.Count - 1] - Main.screenPosition, new Rectangle(0, 66, 38, 18), Color.White, lastRot, new Vector2(19, 5), tipScale, flip, 0);

            return false;
        }

        List<float> previousTipRotations = new List<float>();
        List<Vector2> previousTipPostions = new List<Vector2>();

        List<Vector2> newPositions = new List<Vector2>();
        List<float> newRotations = new List<float>();

        float trailWidth = 1f;
        float trailAlpha = 1f;
        Effect trailEffect = null;
        public void TrailDraw(bool giveUp)
        {
            if (giveUp || previousTipPostions.Count < 3)
                return;

            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/ThinGlowLine").Value; //
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Trails/spark_07_Black").Value; //

            if (trailEffect == null)
                trailEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            newPositions.Clear();
            newRotations.Clear();
            for (int i = 1; i < previousTipPostions.Count; i++)
            {
                Vector2 startPos = previousTipPostions[i - 1] + Main.player[Projectile.owner].Center;
                Vector2 endPos = previousTipPostions[i] + Main.player[Projectile.owner].Center;

                float rot = (endPos - startPos).ToRotation();

                newPositions.Add(startPos);
                newRotations.Add(rot);
            }


            Color colA = isBlue ? Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.15f) : Color.DeepPink;
            Color colB = isBlue ? Color.SkyBlue : Color.HotPink;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = newPositions.ToArray();
            float[] rot_arr = newRotations.ToArray();


            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0f;


            Color StripColor(float progress) => Color.White * Easings.easeInQuad(progress);

            float StripWidth(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.5f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.5f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = Easings.easeOutSine(1f - LV);
                }

                return toReturn * sineWidthMult * 0.75f * 60f * trailWidth;
            }

            float StripWidth2(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.5f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.5f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = Easings.easeOutSine(1f - LV);
                }

                return toReturn * sineWidthMult * 1f * 60f * trailWidth;
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            trailEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.015f); //0.02
            trailEffect.Parameters["reps"].SetValue(0.5f * (previousTipPostions.Count() / 60f));


            //Over layer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            trailEffect.Parameters["ColorOne"].SetValue(colA.ToVector3() * 2f * trailAlpha); //2f
            trailEffect.Parameters["glowThreshold"].SetValue(0.9f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            //UnderLayer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.Parameters["ColorOne"].SetValue(colB.ToVector3() * 5f * trailAlpha);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);


            bool isBlue = true;
            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1f * 0.6f, (diff.Length() + 2) / frame.Height);
                Vector2 scale2 = new Vector2(1f, (diff.Length() + 2) / frame.Height);

                Color glowCol = isBlue ? Color.DeepSkyBlue : Color.DeepPink;
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f), frame, glowCol with { A = 0 }, rotation, origin, scale2, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color * 0.75f, rotation, origin, scale, SpriteEffects.None, 0);



                pos += diff;

                if (i % 2 == 0)
                    isBlue = !isBlue;
            }
        }
    }
}
