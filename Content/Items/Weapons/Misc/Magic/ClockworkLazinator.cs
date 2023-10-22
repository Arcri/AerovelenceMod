using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic
{
    public class ClockworkLazinator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 50;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Pink;

            Item.shoot = ModContent.ProjectileType<LazinatorHeldProj>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 4;
            Item.useAnimation = 20;
            Item.reuseDelay = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.mana = 20;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool CanUseItem(Player player)
        {

            if (player.altFunctionUse == 2)
            {
                int windNumber = Main.player[player.whoAmI].GetModPlayer<LazinatorPlayer>().winds;

                if (windNumber == Main.player[player.whoAmI].GetModPlayer<LazinatorPlayer>().WINDUP_MAX)
                {
                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Menu_Close") with { Pitch = -1f, MaxInstances = 0, Volume = 1f };

                    SoundEngine.PlaySound(style3, Main.player[player.whoAmI].Center);
                    SoundEngine.PlaySound(style3, Main.player[player.whoAmI].Center);

                    return false;
                }
            }


            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<LazinatorWindUp>();
            }


            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class LazinatorShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public Vector2 endPoint = new Vector2(0,0);
        public float Rotation = 0;

        int timer = 0;

        bool collided = false;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.scale = 1f;

            Projectile.timeLeft = 3100;
            Projectile.extraUpdates = 100;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            //Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Rotation.ToRotationVector2() * -6, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.HotPink, 0.4f, 0.4f, 0, dustShader);
            //d.fadeIn = 3;

            collided = true;

            for (int ia = 0; ia < 1 + (Main.rand.NextBool() ? 1 : 0); ia++)
            {
                Vector2 speed = new Vector2(5, 0).RotatedBy(Rotation);
                int a = Dust.NewDust(Projectile.position, 5, 5, ModContent.DustType<ColorSpark>(), SpeedX: speed.X, SpeedY: speed.Y, newColor: Color.HotPink, Scale: 0.3f);
                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                extraInfo.gravityIntensity = 0.1f;
                Main.dust[a].fadeIn = 0.5f;
                Main.dust[a].alpha = 53;
                Main.dust[a].customData = extraInfo;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            //Dust d = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Rotation.ToRotationVector2() * 1, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.HotPink, 0.4f, 0.4f, 0, dustShader);
            //d.fadeIn = 3;

            for (int ia = 0; ia < 2; ia++)
            {
                Vector2 speed = new Vector2(-7, 0).RotatedBy(Rotation);
                int a = Dust.NewDust(Projectile.position, 5, 5, ModContent.DustType<ColorSpark>(), SpeedX: speed.X, SpeedY: speed.Y, newColor: Color.HotPink, Scale: 0.3f);
                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                extraInfo.gravityIntensity = 0.05f;
                Main.dust[a].fadeIn = 0.3f;
                Main.dust[a].alpha = 50;
                Main.dust[a].customData = extraInfo;
            }

            collided = true;
            Projectile.velocity = Vector2.Zero;
        }

        public override bool? CanDamage()
        {
            return !collided;
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

                endPoint = Projectile.Center;
                //if (endPoint == new Vector2(0,0))
                    //endPoint = Projectile.Center;
            }

            if (collided)
            {
                Projectile.velocity = Vector2.Zero;
                if (timeAfterCollided > 300)
                    lineWidth = Math.Clamp(MathHelper.Lerp(lineWidth, -0.2f, 0.001f), 0, 1f);

                if (lineWidth <= 0.4f)
                    Projectile.active = false;
                timeAfterCollided++;
            }

            if (timer == 700)
            {
                collided = true;
            }
            timer++;
        }

        int timeAfterCollided = 0;
        float uColorIntensity = 1.2f;
        float lineWidth = 1;
        public override bool PreDraw(ref Color lightColor)
        {

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * uColorIntensity);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.7f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2f);

                float height = 0.15f * Projectile.scale * lineWidth;

                if (height == 0f)
                    Projectile.active = false;

                //int width = (int)(Projectile.Center - endPoint).Length() - 24;

                //var pos = Projectile.Center - Main.screenPosition;
                //var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));
                
                //Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, Rotation, origin2, 0, 0);
                //Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, Rotation, origin2, 0, 0);

                float distance = (Projectile.Center - endPoint).Length() / 256f;

                Vector2 v2Scale = new Vector2(distance, height);
                Main.spriteBatch.Draw(texBeam, Projectile.Center - Main.screenPosition, null, Color.DeepPink, Rotation, origin2, v2Scale, 0, 0);
                Main.spriteBatch.Draw(texBeam, Projectile.Center - Main.screenPosition, null, Color.DeepPink, Rotation, origin2, v2Scale, 0, 0);


                Texture2D circle = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;

                Vector2 offset = new Vector2(0f, (0.5f * Projectile.height) * -Main.player[Projectile.owner].direction).RotatedBy(Rotation - MathHelper.Pi) * Projectile.scale;

                Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth * Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth * Projectile.scale, SpriteEffects.None, 0);

                //Main.spriteBatch.Draw(circle, endPoint - Main.screenPosition + offset, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth * Projectile.scale, SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(circle, endPoint - Main.screenPosition + offset, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth * Projectile.scale, SpriteEffects.None, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
            // old
            /*
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.HotPink.ToVector3() * uColorIntensity);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.7f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = 25f * Projectile.scale * lineWidth;

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, Rotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, Rotation, origin2, 0, 0);

                //Texture2D circle = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
                //Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth, SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, 0f, circle.Size() / 2, 0.4f * lineWidth, SpriteEffects.None, 0);

                //Main.spriteBatch.Draw(circle, endPoint + new Vector2(-26, 0f).RotatedBy(Rotation) - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, Rotation, circle.Size() / 2, 0.4f * lineWidth, SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(circle, endPoint + new Vector2(-26, 0f).RotatedBy(Rotation) - Main.screenPosition, circle.Frame(1, 1, 0, 0), Color.HotPink, Rotation, circle.Size() / 2, 0.4f * lineWidth, SpriteEffects.None, 0);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            */
            return false;
        }
    }

    public class LazinatorHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public float OFFSET = 20; 

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;
        public bool ShouldFire = true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lazinator");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 50; //50
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        bool firstFrame = true;
        public override void AI()
        {
            if (firstFrame)
            {
                Projectile.timeLeft = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds * 20 + 50;
                firstFrame = false;
            }

            HeldProjCode(false);
            
            if (ShouldFire)
            {
                if (timer % 4 == 0 && timer > 15 && timer < 40 + (Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds * 20)) //40
                {
                    Vector2 vel = new Vector2(10, 0).RotatedBy(Angle);
                    Vector2 pos = Projectile.Center;

                    Vector2 muzzleOffset = Vector2.Normalize(vel) * 35f;
                    if (Collision.CanHit(Projectile.Center, 0, 0, Projectile.Center + muzzleOffset, 0, 0))
                    {
                        pos += muzzleOffset;
                    }
                    int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel.RotatedByRandom(0.05f), ModContent.ProjectileType<LazinatorShot>(), 10, 0, Main.player[Projectile.owner].whoAmI);

                    if (Main.projectile[a].ModProjectile is LazinatorShot shot)
                        shot.endPoint = pos;

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Research_3") with { Pitch = .65f, PitchVariance = .2f, Volume = 0.3f };
                    SoundEngine.PlaySound(style, Main.player[Projectile.owner].Center);
                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .44f, };
                    SoundEngine.PlaySound(style2, Main.player[Projectile.owner].Center);


                    //ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    //Dust d = GlowDustHelper.DrawGlowDustPerfect(pos, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.HotPink, 0.4f, 0.4f, 0, dustShader);
                    //d.fadeIn = 3;

                    glowIntensity = 1f;
                    drawXScale = 0.85f;
                }
            }

            glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.75f, 0.08f), 0, 1);

            if (Projectile.timeLeft == 2)
            {
                Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds = 0;
            }
        }

        public void HeldProjCode(bool windup)
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            if (lerpToStuff == 0.6f)
            {
                hasReachedDestination = true;
            }

            if (timer == 0)
            {
                OFFSET = 0f;
            }
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 25f, 0.07f), 0, 20);
            drawXScale = Math.Clamp(MathHelper.Lerp(drawXScale, 1.2f, 0.2f), 0, 1);

            if (Projectile.timeLeft < 10 && !windup)
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.25f), 0, 1);
            else
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.2f), 0, 1);

            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();
            
            timer++;
        }

        float drawXScale = 1f;
        float alpha = 0f;
        public float glowIntensity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinator");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinatorGlow");
            Texture2D White = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinatorWhite");

            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawScale = new Vector2(drawXScale, 1f);
            Main.spriteBatch.Draw(Weapon, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, Weapon.Size() / 2, drawScale, mySE, 0f);
            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, Weapon.Size() / 2, drawScale, mySE, 0f);
            Main.spriteBatch.Draw(White, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * alpha * glowIntensity, Projectile.rotation, White.Size() / 2, drawScale, mySE, 0f);

            return false;
        }
    }

    public class LazinatorWindUp : LazinatorHeldProj
    {
        public int windUpTimer = 0;
        public float windUpPercent = 0;
        public float windUpValue = 0;
        public bool shouldKill = false;


        public override void AI()
        {
            HeldProjCode(true);

            Projectile.timeLeft = 2;

            if (!Main.mouseRight)
            {
                shouldKill = true;
            }

            if (windUpTimer >= 20)
            {
                if (windUpTimer == 20)
                {

                    int windNumber = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds;

                    if (windNumber == Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().WINDUP_MAX)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .33f, PitchVariance = 0.2f, Volume = 0.65f };
                        SoundEngine.PlaySound(style, Projectile.Center);
                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = .7f, Pitch = .33f, }; SoundEngine.PlaySound(style2, Projectile.Center);


                        glowIntensity = 1;
                        Projectile.active = false;
                    }

                    if (Projectile.active)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_149") with { Pitch = .43f, Volume = 0.75f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        
                    }

                    //SoundStyle styl2e = new SoundStyle("Terraria/Sounds/Item_82") with { Volume = .4f, Pitch = .33f }; SoundEngine.PlaySound(styl2e, Projectile.Center);
                    //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_75") with { Volume = .7f, Pitch = .33f, };
                    //SoundEngine.PlaySound(style2, Projectile.Center);
                    //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = .19f, Pitch = .33f, }; SoundEngine.PlaySound(style2, Projectile.Center);
                }

                windUpPercent = Math.Clamp((windUpTimer - 20) * 0.3f, 0, MathHelper.TwoPi); 

                if (windUpPercent == MathHelper.TwoPi)
                {
                    int windNumber = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds;

                    Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds = Math.Clamp(windNumber + 1, 0, 4);

                    windUpTimer = -20;
                    windUpValue = 0;
                    windUpPercent = 0;

                    if (shouldKill)
                    {

                        if (windNumber == Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().WINDUP_MAX - 1)
                        {
                            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .33f, PitchVariance = 0.2f, Volume = 0.65f };
                            SoundEngine.PlaySound(style, Projectile.Center);
                            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = .7f, Pitch = .33f, }; SoundEngine.PlaySound(style2, Projectile.Center);
                        }
                        Projectile.active = false;
                    }

                    //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .33f, PitchVariance = 0.2f, Volume = 0.65f };
                    //SoundEngine.PlaySound(style, Projectile.Center);


                }
            }
            windUpValue = (float)Math.Sin(windUpPercent) * 0.4f;


            Main.player[Projectile.owner].SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() - MathHelper.PiOver2 + windUpValue);

            windUpTimer++;
        }
    }

    public class LazinatorPlayer : ModPlayer
    {
        public int winds = 0;
        public int WINDUP_MAX = 4;
    }

}
