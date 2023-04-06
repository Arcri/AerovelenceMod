using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class OzoneShredderHeldProj : ModProjectile
    {
        public override bool ShouldUpdatePosition() => true;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ozone Shredder");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; 
        }

        int maxTime = 0;
        public override void SetDefaults()
        {
            maxTime = 100;
            Projectile.timeLeft = maxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float endingAng = 0;
        float currentAng = 0;

        float Angle = 0;

        bool playedSound = false;
        bool hasDashed = false;

        float storedDirection = 1;


        Projectile distortProj;
        public override void AI()
        {
            //Matenince 
            Player player = Main.player[Projectile.owner];
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Projectile.Center - (player.Center)).ToRotation();
            }

            if (firstFrame)
                storedDirection = player.direction;

            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = Angle + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            if (easingProgress > 0.5)
                player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {
                maxTime = 0;
                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                
                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);
                
                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the slate set. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    endingAng = startingAng + MathHelper.ToRadians(-160);
                    startingAng = startingAng - MathHelper.ToRadians(-160);
                }
                else
                {
                    endingAng = startingAng - MathHelper.ToRadians(-160);
                    startingAng = startingAng + MathHelper.ToRadians(-160);
                }

                currentAng = startingAng;
                firstFrame = false;
            }
            

            if (maxTime >= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((320 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((320 * getProgress(easingProgress)));

                easingProgress = Math.Clamp(easingProgress + 0.014f, 0.05f, 0.95f);
            }

            if (easingProgress >= 0.35 && !hasDashed)
            {
                float dashAngle = startingAng + MathHelper.ToRadians(160);

                //Vector2 mousePos = Main.MouseWorld;
                //player.velocity = (player.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX) * -12;
                hasDashed = true;
            }

            if (easingProgress >= 0.85f)
            {
                Projectile.active = false;
                //if (distortProj != null)
                    //distortProj.active = false;
            }


            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * 65) + player.RotatedRelativePoint(player.MountedCenter);
            player.itemTime = 10;
            player.itemAnimation = 10;

            maxTime++;

            if (getProgress(easingProgress) >= 0.35f && getProgress(easingProgress) <= 0.78f)
            {
                Projectile.ai[1] = 1;

                //if (getProgress(easingProgress) >= 0.45f && getProgress(easingProgress) <= 0.55f)
                    //Projectile.ai[1] = 0;
            }
            else
                Projectile.ai[1] = 0;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                //int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OzoneShredderDistort>(), 0, 0, Main.myPlayer);
                //distortProj = Main.projectile[proj];

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.4f, PitchVariance = 0.15f, Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = 0.4f }, Projectile.Center);

                //String extra = Main.rand.NextBool() ? "M_a" : "L_a";
                String soundLocation = Main.rand.NextBool() ? "AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_L_a" : "AerovelenceMod/Sounds/Effects/GGS/Sword_Sharp_L_b"; 
                SoundStyle slash = new SoundStyle(soundLocation) with { Pitch = 0, PitchVariance = .3f, Volume = 0.43f };
                SoundEngine.PlaySound(slash, Projectile.Center);
                playedSound = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            //return false;

            SpriteEffects spriteEffects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            Texture2D glowTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/OzoneShredderHeldProjGlow");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.SkyBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(glowTex, drawPos + new Vector2(10,8), null, color * (0.5f -(k * 0.05f)), Projectile.oldRot[k] + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), spriteEffects, 0);
            }


            //Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/twirl_02");
            //float extraRotation = Projectile.ai[0] != 1 ? MathHelper.PiOver4 + 0.3f : MathHelper.PiOver2 - 1f;
            //Main.spriteBatch.Draw(a, Main.player[Projectile.owner].Center - Main.screenPosition, null, Color.White, Projectile.rotation + extraRotation, a.Size() / 2, new Vector2(0.5f, 0.5f) * Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            
            #region old version need to try something with a new tex
            /*
            SpriteEffects spriteEffects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.5f, Projectile.oldRot[k], origin, Projectile.scale, spriteEffects, 0);
            }
            */
            #endregion

            return false;
        }

        bool hasBonked = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Player player = Main.player[Projectile.owner];
            if (!hasBonked && player.timeSinceLastDashStarted < 30)
            {
                //if the player is using SoC dash (which they very likely are at this stage) we don't want to bonk an ememy
                //and then bonk with the sword because we'll end up going forward again
                if (player.dashType != 2 && player.immuneNoBlink)
                {
                    return;
                }

                player.velocity.X = -player.velocity.X / 2;
                player.velocity.Y = -player.velocity.X / 2;
                hasBonked = true;
                player.GiveImmuneTimeForCollisionAttack(60);

                SoundStyle slash = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Pitch = 0.8f, PitchVariance = .4f, Volume = 0.5f };
                SoundEngine.PlaySound(slash, Projectile.Center);

                //Impact Dust out
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                for (int i = 0; i < 11; i++)
                {
                    Vector2 vel = (target.Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowSpark>(), vel.SafeNormalize(Vector2.UnitX) * (14f + Main.rand.NextFloat(-2f, 2f)),
                        Color.LightBlue, Main.rand.NextFloat(0.1f, 0.3f), 0.9f, 0f, dustShader);
                    p.fadeIn = 50 + Main.rand.NextFloat(-10, 15);
                    p.velocity *= 0.7f;
                }

            }

            //Need to move position by player zoom cause it will be wrong if not (still breaks while zooming during its existence but w/e
            Vector2 awayFromNPC = (target.Center - player.Center);// * -2f * (1 - Main.GameZoomTarget);
            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<OzoneShredderImpact>(), 0, 0);
            Main.projectile[a].rotation = awayFromNPC.ToRotation() - MathHelper.PiOver2;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] == 1;
        }

        float easingProgress = 0;
        //https://easings.net/#easeInOutExpo easing function used below
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            #region easeExpo
            
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) -10)) / 2;
            } 
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;
            
            
            #endregion;

            #region easeCircle
            
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            } 
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            
            #endregion
        }

    }

    public class OzoneShredderImpact : ModProjectile
    {
        public Texture2D tex;
        int timer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("OzoneDistort");
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;

        }
        int projectileAttachedTo = 0;
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0, 0.2f);
            timer++;

            if (Projectile.scale < 0.05f)
                Projectile.active = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
