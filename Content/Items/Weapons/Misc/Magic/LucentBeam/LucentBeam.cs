using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using static Terraria.NPC;
using ReLogic.Content;
using Terraria.Graphics;
using UtfUnknown.Core.Models.SingleByte.Finnish;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.LucentBeam
{
    public class LucentBeam : ModItem
    {
        private int shotCounter = 0;
        
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = 2f;
            Item.crit = 4;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.Yellow;
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<LucentBeamHeldProj>();

            Item.mana = 8;

            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            //Do not let the player use item if they have less than 40 mana so it doesn't flicker weird when they are out of mana
            return player.CheckMana(player.inventory[player.selectedItem], amount: 40, pay: false);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/starUIToss") with { Volume = .1f, Pitch = .1f, PitchVariance = .12f, }; 
            SoundEngine.PlaySound(style, player.Center);

            return true;
        }

        public override bool OnPickup(Player player) //Another check for the problem mentioned above
        {
            shotCounter = 0;
            return base.OnPickup(player);
        }
    }

    public class LucentBeamHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => !fadeOut;


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //float maxPoints = Math.Clamp(arr_positions.Length, 0f, 100f);

            //Do a line collision between every 5 points
            for (int m = 6; m < (l_positions.Count * 0.95f); m += 5)
            {
                float discarda = 0f;
                bool collided = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), l_positions[m], l_positions[m - 5], 20f, ref discarda);

                //Do hit dust (doing here and not OnHit so we can have custom hit position)
                if (collided)
                {
                    for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++)
                    {
                        Color dustCol = Main.hslToRgb(Main.rand.NextFloat(0f, 1f), 1f, 0.7f, 0) * 1f;

                        Vector2 dustVel = Main.rand.NextVector2Circular(2f, 2f);

                        Dust.NewDustPerfect(l_positions[m], ModContent.DustType<GlowPixelCross>(), dustVel, newColor: dustCol, Scale: Main.rand.NextFloat(0.2f, 0.3f));
                    }

                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Volume = .05f, Pitch = 1f, PitchVariance = .33f, };
                    SoundEngine.PlaySound(style, l_positions[m]);

                    return true;
                }
            }
            return false;
        }

        bool fadeOut = false;

        //Tendril positions relative to itself
        Vector2[] arr_positions = new Vector2[TotalPoints];
        float[] arr_rotations = new float[TotalPoints];

        //Needed lists for dynamic beam length
        public List<Vector2> l_positions = new List<Vector2>();
        public List<float> l_rotations = new List<float>();

        const int TotalPoints = 150;

        public Vector2 anchor = Vector2.Zero;

        int timer = 0;
        public override void AI()
        {
            #region held proj code
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Player player = Main.player[Projectile.owner];

            Vector2 HandPos = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Projectile.Center = HandPos + new Vector2(0f, player.gfxOffY);// player.MountedCenter + Projectile.rotation.ToRotationVector2() * 5f;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            player.SetDummyItemTime(3);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero), player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero), 0.12f); //0.12f
            Projectile.rotation = Projectile.velocity.ToRotation();

            #endregion

            anchor = Projectile.Center;

            #region tendril code
            if (timer == 0)
            {
                //Create all of the points and set all the rotations to be the same
                float distBetweenEachPoint = 2.5f;
                for (int i = 0; i < TotalPoints; i++)
                {
                    arr_positions[i] = Vector2.Zero + Projectile.rotation.ToRotationVector2() * (distBetweenEachPoint * i);
                    arr_rotations[i] = Projectile.rotation;
                }
            }

            //Have all points try to rotate towards the acnhor
            for (int j = 0; j < TotalPoints; j++)
            {
                float progress = (j / (float)TotalPoints);

                //The further along the trail, the weaker the turning
                float lerpValue = MathHelper.Lerp(0.3f, 0.12f, progress); //92

                Vector2 newRotationVec = Vector2.Lerp(arr_rotations[j].ToRotationVector2(), Projectile.rotation.ToRotationVector2(), lerpValue);
                float newRotation = newRotationVec.ToRotation();

                arr_rotations[j] = newRotation;
                arr_positions[j] = Vector2.Zero + newRotation.ToRotationVector2() * (2.5f * j);
            }

            l_positions.Clear();
            l_rotations.Clear();
            for (int k = 0; k < (TotalPoints - 1) * overallAlpha; k++)
            {
                //We have to flip the first point over for some reason or else we get a weird tear.
                if (k == 0)
                    l_rotations.Add(arr_rotations[k] + MathHelper.Pi);
                else
                    l_rotations.Add((arr_positions[k - 1] - arr_positions[k]).ToRotation());
                l_positions.Add(arr_positions[k] + anchor);

                //if (k == 0)
                //    draw_rotations[k] = arr_rotations[k] + MathHelper.Pi;
                //else
                //    draw_rotations[k] = (arr_positions[k - 1] - arr_positions[k]).ToRotation();
                //draw_positions[k] = arr_positions[k] + anchor;
            }

            #endregion

            //Dust
            if (timer % 1 == 0 && Main.rand.NextBool() && timer != 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    Color rainbow = Main.hslToRgb(Main.rand.NextFloat(0f, 1f), 1f, 0.65f, 0) * 1f;

                    int index = Main.rand.Next(0, (int)(l_positions.Count * 0.75f));
                    Vector2 dustPos = l_positions[index] + Main.rand.NextVector2Circular(10f, 10f);
                    Vector2 dustVel = l_rotations[index].ToRotationVector2() * -Main.rand.NextFloat(3f, 6f);

                    Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: rainbow, Scale: Main.rand.NextFloat(0.5f, 0.85f) * 0.35f);
                    d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(velToBeginShrink: 2f, fadePower: 0.92f, shouldFadeColor: false);
                }
            }


            //Fade-in/Fade-out
            if (player.channel == false)
                fadeOut = true;

            if (!fadeOut)
                overallAlpha = Math.Clamp(MathHelper.Lerp(overallAlpha, 1.3f, 0.06f), 0f, 1f); //008 012
            else
                overallAlpha = Math.Clamp(MathHelper.Lerp(overallAlpha, -0.5f, 0.08f), 0f, 1f);

            beamWidth = overallAlpha;

            if (fadeOut && overallAlpha == 0f)
                Projectile.active = false;

            //Mana
            if (timer % 15 == 0)
            {
                bool manaResult = player.CheckMana(player.inventory[player.selectedItem], pay: true);
                if (manaResult == false)
                    fadeOut = true;
            }

            //Light along points
            for (int i = 0; i < l_positions.Count * 0.9f; i += 12)
            {
                Lighting.AddLight(l_positions[i], Main.hslToRgb((timer * 0.015f) % 1f, 1f, 0.75f, 0).ToVector3() * 0.75f);
            }

            timer++;
        }


        float beamWidth = 1f;
        float overallAlpha = 0f;
        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertexGradient", AssetRequestMode.ImmediateLoad).Value;

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
            {
                RainbowSigil();
                DrawTrail(false);
            });
            return false;

        }

        public void RainbowSigil()
        {
            Color rainbow = Main.hslToRgb((timer * 0.03f) % 1f, 1f, 0.75f, 0) * 0.75f;

            Texture2D portal = CommonTextures.RainbowRod.Value;
            Texture2D bloom = CommonTextures.feather_circle128PMA.Value;

            float sinScale = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.03f) * 0.2f;

            Vector2 v2Scale = new Vector2(1f, 0.6f) * beamWidth * sinScale * 0.4f;


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.PiOver2;

            //Start Portal
            Main.EntitySpriteDraw(portal, drawPos, null, Color.Black * 0.3f * overallAlpha, rot, portal.Size() / 2, v2Scale * 2f, SpriteEffects.None);

            Main.EntitySpriteDraw(bloom, drawPos, null, rainbow with { A = 0 } * overallAlpha * 0.4f, rot, bloom.Size() / 2, v2Scale * 1f, SpriteEffects.None);

            Main.EntitySpriteDraw(portal, drawPos + Main.rand.NextVector2Circular(2f, 2f), null, rainbow with { A = 0 } * overallAlpha, rot, portal.Size() / 2, v2Scale * 2f, SpriteEffects.None);
            Main.EntitySpriteDraw(portal, drawPos + Main.rand.NextVector2Circular(3f, 3f), null, rainbow with { A = 0 } * overallAlpha, rot, portal.Size() / 2, v2Scale * 1.75f, SpriteEffects.None);
            Main.EntitySpriteDraw(portal, drawPos + Main.rand.NextVector2Circular(5f, 5f), null, Color.White with { A = 0 } * overallAlpha, rot, portal.Size() / 2, v2Scale * 1f, SpriteEffects.None);


            return;
        }


        public void DrawTrail(bool giveUp = false)
        {
            if (giveUp)
                return;

            //Create arrays
            Vector2[] pos_arr = l_positions.ToArray();
            float[] rot_arr = l_rotations.ToArray();

            float widthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.07f) * 0.15f;

            Color StripColor(float progress) => Color.White * 1f;
            //float StripWidth(float progress) => 45f * Easings.easeInSine(1f) * widthMult;// * MathF.Sqrt(Utils.GetLerpValue(0f, 0.1f, progress, true));

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);
            ShaderParams();

            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
        public void ShaderParams()
        {
            //float fadeOut = shouldFade ? Math.Clamp(Easings.easeInQuad(true_alpha), 0.15f, 1f) : true_alpha;

            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/GlowTrailClear").Value);
            myEffect.Parameters["gradientTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/RainbowGrad1").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3());
            myEffect.Parameters["satPower"].SetValue(0.8f); //higher power -> less affected by background  |95 | 3f looks very goozma

            myEffect.Parameters["sampleTexture1"].SetValue(CommonTextures.ThinGlowLine.Value);
            myEffect.Parameters["sampleTexture2"].SetValue(CommonTextures.spark_06.Value);
            myEffect.Parameters["sampleTexture3"].SetValue(CommonTextures.Extra_196_Black.Value);
            myEffect.Parameters["sampleTexture4"].SetValue(CommonTextures.Trail5Loop.Value);


            myEffect.Parameters["grad1Speed"].SetValue(2f / 3f);
            myEffect.Parameters["grad2Speed"].SetValue(2f / 3f);
            myEffect.Parameters["grad3Speed"].SetValue(3.1f / 3f);
            myEffect.Parameters["grad4Speed"].SetValue(2.3f / 3f);

            myEffect.Parameters["tex1Mult"].SetValue(1.25f);
            myEffect.Parameters["tex2Mult"].SetValue(1.5f);
            myEffect.Parameters["tex3Mult"].SetValue(1.15f);
            myEffect.Parameters["tex4Mult"].SetValue(2.5f); //1.5
            myEffect.Parameters["totalMult"].SetValue(1f);


            //We want the number of repititions to be relative to the number of points
            float repValue = 0.05f * 25f;
            myEffect.Parameters["gradientReps"].SetValue(0.35f * repValue); //1f
            myEffect.Parameters["tex1reps"].SetValue(1f * repValue); //2.5
            myEffect.Parameters["tex2reps"].SetValue(0.3f * repValue);
            myEffect.Parameters["tex3reps"].SetValue(1f * repValue);
            myEffect.Parameters["tex4reps"].SetValue(0.25f * repValue);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.025f); //-0.015

        }

        public float StripWidth(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.45f, 1f - progress, clamped: true); //0.4
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);

            float width = MathHelper.Lerp(0f, 40f, Easings.easeInCirc(num)) * Easings.easeInSine(beamWidth); //50f
            return Math.Max(width, 2f) * (progress > 0.95f ? 0f : 1f);
        }
    }
}