using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Bows
{
    public class WarBowHeldProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("War Bow");
        }

        public int OFFSET = 10; //15
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        int timer = 0;

        int timeBeforeKill = 0; //once the player lets go, this will start ticking up and the proj will die after a certain time

        float percentDrawnBack = 0;

        int skillCritWindow = 0;
        public bool runOnceCharge = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }


        public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public int projToShootID = ProjectileID.WoodenArrowFriendly;

        /* TBD 
        * -1- Draw Arrow (DONE)
        * -2- Draw String with Utils.DrawLine (SCRAPPED)
        * -3- Bow Draw Back Anim (DONE)
        * -4- Power to charge ratio (DONE)
        * -5- Skill Strike FX and window 
        */
        public override void AI() 
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                //Main.NewText(Player.direction);
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

                percentDrawnBack = MathHelper.Clamp(percentDrawnBack + 0.03f, 0f, 1f);
                if (timer < 10)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                else if (timer < 20)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (percentDrawnBack >= 1f && runOnceCharge)
                {
                    runOnceCharge = false;
                    skillCritWindow = 10;
                }
            }
            else
            {
                Vector2 projPosition = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY).Floor();


                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (timeBeforeKill == 0)
                {
                    float vel = MathHelper.Clamp(20 * percentDrawnBack, 6, 20);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, direction.SafeNormalize(Vector2.UnitX) * vel, projToShootID, (Projectile.damage / 2 + (int)(Projectile.damage * percentDrawnBack)), 0, Player.whoAmI);
                    
                    if (skillCritWindow >= 0)
                    {
                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        for (int j = 0; j < 6; j++)
                        {
                            Dust dust1 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * vel, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 3),
                                Color.Orange, Main.rand.NextFloat(0.3f, 0.5f), 0.6f, 0f, dustShader2);
                            dust1.velocity *= 1f;

                        }
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(4f, 4f) * 0.75f;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + randomStart + direction.SafeNormalize(Vector2.UnitX) * vel, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);
                        }

                        proj.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                        proj.GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.glowProjCenter;
                        proj.GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.pixelProjCenter;

                        SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, };
                        SoundEngine.PlaySound(style3, Player.Center);
                    }
                    else
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_javelin_throwers_attack_1") with { Pitch = .85f, PitchVariance = 0.1f, Volume = 0.5f + (0.5f * percentDrawnBack) };
                        SoundEngine.PlaySound(style, Player.Center);
                    }

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_5") with { Pitch = -.53f, };
                    SoundEngine.PlaySound(style2, Player.Center);

                }

                if (timeBeforeKill >= 40)
                    Projectile.active = false;
                timeBeforeKill++;
            }

            //lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

            
            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            if (skillCritWindow >= 0)
            {
                //Main.NewText("starScaleMultiplier - " + starScaleMultiplier);
                starScaleMultiplier = MathHelper.Clamp(skillCritWindow, 2, 20);
                outerStarScale = 0.05f * starScaleMultiplier * 0.1f;
                innerStarScale = 0.035f * starScaleMultiplier * 0.1f;
            }

            skillCritWindow--;
            timer++;
        }

        float starScaleMultiplier = 0;
        float outerStarScale = 0;
        float innerStarScale = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
            }


            //Arrow Drawing
            Texture2D arrowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Bows/Wooden_Arrow").Value;
            Vector2 origin2 = new Vector2((float)arrowTexture.Width / 2f, (float)arrowTexture.Height / 2f);
            Vector2 position2 = (Projectile.position - (0.5f * (direction * OFFSET * -1.5f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 chargeOffset = new Vector2(-5 * percentDrawnBack, 0).RotatedBy(direction.ToRotation());

            Vector2 lineOffsetRot1 = new Vector2(0f, -15f).RotatedBy(Projectile.rotation);
            Vector2 lineOffsetRot2 = new Vector2(0f, 15f).RotatedBy(Projectile.rotation);

            Vector2 arrowButtPos = position2 + chargeOffset + new Vector2(0, -13).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);
            if (Player.channel)
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
            } else
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, Projectile.Center + lineOffsetRot2, lightColor, lightColor, 1.5f);
                //Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, Projectile.Center + lineOffsetRot1, lightColor, lightColor, 1.5f);
            }


            if (Player.channel)
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, skillCritWindow >= 0 ? Color.Gold : lightColor, direction.ToRotation() - MathHelper.PiOver2, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            Texture2D shineTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            Vector2 arrowTipPos = position2 + chargeOffset + new Vector2(0,10).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);

            if (skillCritWindow >= 0)
            {
                //outerStarScale = 0.08f;
                //innerStarScale = 0.065f;
                //float scaleMultiplier = skillCritWindow / 10;
                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(shineTexture, arrowTipPos, shineTexture.Frame(1, 1, 0, 0), Color.Gold * 0.75f, MathHelper.ToRadians(timer * 2),
                    shineTexture.Size() / 2, innerStarScale * 2f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(shineTexture, arrowTipPos, shineTexture.Frame(1, 1, 0, 0), Color.Orange * 0.75f, MathHelper.ToRadians(timer * 2) + (float)Math.PI,
                    shineTexture.Size() / 2, innerStarScale * 2f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

    }
}
