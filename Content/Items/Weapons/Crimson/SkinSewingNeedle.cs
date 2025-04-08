
using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Projectiles.Other;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Crimson
{
    public class SkinSewingNeedle : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.damage = 8;
            Item.shoot = ModContent.ProjectileType<SkinSewingNeedleProj>();
            Item.shootSpeed = 32;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.reuseDelay = 8;
            Item.crit = 25;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Long-ranged attacks skill strike [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);

        }

    }

    

    public class SkinSewingNeedleProj : TrailProjBase 
    {
        static Asset<Texture2D> glow;
        static Asset<Texture2D> piercingStrike;
        static Asset<Texture2D> sparkAtTheTip;
        static Asset<Texture2D> sparkAtTheTip2;
        public override void Load()
        {
            glow = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Crimson/NeedleGlow");
            piercingStrike = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/AdamantitePulseShot");
            sparkAtTheTip = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_2");
            sparkAtTheTip2 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_3");
        }
        private NeedleState state 
        {
        
            set {
                currentState = value;
                Timer = 0;
                Projectile.extraUpdates = 0;
            }
            get => currentState;

        }
        private NeedleState currentState = NeedleState.JustFired;

        private SpriteEffects spriteEffects = SpriteEffects.None;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Progress => ref Projectile.ai[1];

        private Vector2 hitNpcCenterOffset;
        private Vector2 startingVel;
        private NPC hitNpc;

        
        private enum NeedleState : byte
        {
            JustFired,
            Latched,
            Returning,
            Swinging
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            
        }
        public override void SetDefaults()
        {
            state = NeedleState.JustFired;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
            Projectile.ArmorPenetration = 15;
        }

        public override float WidthFunction(float progress)
        {
            return 7f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(5f));
            startingVel = Projectile.velocity;
            state = NeedleState.JustFired;
            Projectile.rotation = Projectile.velocity.ToRotation();
            SoundStyle swif = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = 1f, Volume = 0.27f };
            SoundEngine.PlaySound(swif, Projectile.Center);
            
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            switch (state) 
            {
            
                case NeedleState.JustFired:
                    state = NeedleState.Returning;

                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = oldVelocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));

                        Dust p = Dust.NewDustPerfect(Projectile.Center + oldVelocity, ModContent.DustType<LineSpark>(), vel.SafeNormalize(Vector2.UnitX) * (-8f + Main.rand.NextFloat(-1f, 1f)),
                            newColor: Color.Red, Scale: 0.45f);

                        p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.92f, preShrinkPower: 0.97f, postShrinkPower: 0.85f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 20,
                            0.5f, 0.35f);
                    }

                    SoundStyle tileCollideSS = new SoundStyle("AerovelenceMod/Sounds/Effects/Metallic/joker_stab1") with { Pitch = 0f, Volume = 0.27f };
                    SoundEngine.PlaySound(tileCollideSS,Projectile.Center);

                    break;

                case NeedleState.Returning:
                case NeedleState.Swinging:
                case NeedleState.Latched:
                    break;
            
            }

            return false;
        }
        private void DrawLine(Vector2 pos, Color lightColor)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Player player = Main.player[Projectile.owner];
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);



            Main.EntitySpriteDraw(texture, Vector2.Lerp(player.Center, pos, 0.5f) - Main.screenPosition, frame, lightColor.MultiplyRGB(Color.IndianRed * 0.7f), player.Center.DirectionTo(Projectile.Center).ToRotation(), origin + new Vector2(0, 3), new Vector2(Projectile.Distance(player.Center) / 2f, 0.5f), SpriteEffects.None);

            Main.EntitySpriteDraw(texture, Vector2.Lerp(player.Center, pos, 0.5f) - Main.screenPosition, frame, lightColor.MultiplyRGB(Color.IndianRed), player.Center.DirectionTo(Projectile.Center).ToRotation(), origin, new Vector2(Projectile.Distance(player.Center) / 2f,0.25f),SpriteEffects.None);

        }

        private void DrawNeedle(Vector2 shaky, Color lightColor) 
        {

            Vector2 needleTip = Projectile.Center + new Vector2(-15, 0).RotatedBy(Projectile.rotation);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);



            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + shaky, null, Color.DarkRed, Projectile.rotation - MathHelper.PiOver2, glow.Size() / 2f - new Vector2(0, -18f * 1.25f), new Vector2(1f, 1f), spriteEffects);

            if(state == NeedleState.JustFired) 
                Main.EntitySpriteDraw(piercingStrike.Value, needleTip - Main.screenPosition, null, Color.DarkRed * MathHelper.Lerp(0.1f, 3f, Progress), Projectile.rotation + MathHelper.PiOver2, new Vector2(144 / 2f, 512 / 2f + 200), new Vector2(1f, 0.5f), SpriteEffects.None);
            else if(state == NeedleState.Latched)
                Main.EntitySpriteDraw(piercingStrike.Value, needleTip - Main.screenPosition + new Vector2(155 * InExpo(1f - Progress,8f),0).RotatedBy(Projectile.rotation), null, Color.DarkRed * MathHelper.Lerp(3f, 0, Progress), Projectile.rotation + MathHelper.PiOver2, new Vector2(144 / 2f, 512 / 2f + 200), new Vector2(0.8f, 0.5f), SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition + shaky, null, lightColor, Projectile.rotation - MathHelper.PiOver2, TextureAssets.Projectile[Type].Size() / 2f - new Vector2(0, -18f), 1f, spriteEffects);

            if(state == NeedleState.Latched) 
            {

                Main.EntitySpriteDraw(sparkAtTheTip.Value, needleTip - Main.screenPosition + shaky, null, Color.Red, Projectile.timeLeft * 0.2f, sparkAtTheTip.Size() / 2f, MathHelper.Lerp(0.0f, 0.2f, Utils.PingPongFrom01To010(OutExpo(Progress, 5f))), spriteEffects);
                Main.EntitySpriteDraw(sparkAtTheTip2.Value, needleTip - Main.screenPosition + shaky, null, Color.Red, Projectile.timeLeft * 0.2f, sparkAtTheTip.Size() / 2f, MathHelper.Lerp(0.0f, 0.5f, Utils.PingPongFrom01To010(OutExpo(Progress, 5f))), spriteEffects);


            }



        }
        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 stringPos = Projectile.Center;
            switch (state) 
            {
                case NeedleState.Returning:


                    if(hitNpc != null)
                        for (int i = 0; i < Projectile.oldPos.Length; i++)
                            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + TextureAssets.Projectile[Type].Size() / 2f - new Vector2(0,9) - Main.screenPosition, null, lightColor.MultiplyRGB(Color.Lerp(Color.Crimson,Color.White, MathHelper.Lerp(1f, 0f, i / (float)Projectile.oldPos.Length))) * MathHelper.Lerp(1f,0f,i / (float)Projectile.oldPos.Length), Projectile.rotation - MathHelper.PiOver2, TextureAssets.Projectile[Type].Size() / 2f - new Vector2(0, -18f), 1f, spriteEffects);
                    
                    DrawNeedle(Vector2.Zero, lightColor);
                    DrawLine(stringPos, lightColor);


                    break;
                case NeedleState.JustFired:
                    DrawNeedle(Vector2.Zero, lightColor);
                    DrawLine(stringPos, lightColor);

                    
                    break;
                case NeedleState.Swinging:
                    TrailDrawing();

                    DrawNeedle(Vector2.Zero, lightColor);
                    DrawLine(stringPos, lightColor);

                    break;
                case NeedleState.Latched:
                    Vector2 shaky = Main.rand.NextVector2Circular(Timer, Timer);
                    DrawNeedle(shaky, lightColor);

                    stringPos += shaky;
                    DrawLine(stringPos, lightColor);

                    break;

            }


            return false;
        }

        public override void PostDraw(Color lightColor)
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (state)
            {

                case NeedleState.JustFired:
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));

                        Dust p = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineSpark>(), vel.SafeNormalize(Vector2.UnitX) * (-8f + Main.rand.NextFloat(-1f, 1f)),
                            newColor: Color.Red, Scale: 0.45f);

                        p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.92f, preShrinkPower: 0.97f, postShrinkPower: 0.85f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 20,
                            0.5f, 0.35f);


                    }
                    var hitEffect = Projectile.NewProjectileDirect(null, Projectile.Center - Projectile.velocity, Projectile.Center.DirectionFrom(Main.player[Projectile.owner].Center) * 8f, ModContent.ProjectileType<NeedleHit>(), 0, 0, Main.myPlayer);

                    if (Progress < 0.66f) 
                    {
                        state = NeedleState.Returning;


                        break;

                    }

                    hitNpc = target;
                    state = NeedleState.Latched;
                    hitNpcCenterOffset = target.DirectionTo(Projectile.Center) * target.Distance(Projectile.Center);
                    Projectile.Center = hitNpc.Center + hitNpcCenterOffset;

                    int b = Projectile.NewProjectile(null, Projectile.Center - Projectile.velocity, Projectile.velocity.SafeNormalize(Vector2.UnitX) * -0.5f, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[b].rotation = Projectile.velocity.ToRotation();
                    if (Main.projectile[b].ModProjectile is CirclePulse pulseb)
                    {
                        pulseb.color = Color.Red;
                        pulseb.size = 0.25f;
                    }

                    SoundStyle hitsound = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_butterfly_blade") with { Pitch = 0f, Volume = 0.27f };
                    SoundEngine.PlaySound(hitsound,Projectile.Center);

                    break;


            }
        }

        //Credits to Terraria Roguelike Mod For these Lerp Progression functions
        private static float InExpo(float t, float strength) => (float)Math.Pow(2, strength * (t - 1));
        public static float OutExpo(float t, float strength) => 1 - InExpo(1 - t, strength);
        private static float InOutExpo(float t, float strength)
        {
            if (t < 0.5) return InExpo(t * 2, strength) * .5f;
            return 1 - InExpo((1 - t) * 2, strength) * .5f;
        }
        public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) * -.5f;
        public static float InSine(float t) => (float)-Math.Cos(t * MathHelper.PiOver2);
        public static float OutSine(float t) => (float)Math.Sin(t * MathHelper.PiOver2);
        public static float InBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }
        public static float OutBack(float t) => 1 - InBack(1 - t);

        public override void AI()
        {
            base.AI();

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrail").Value;
            trailColor = Color.IndianRed;
            trailPointLimit = 400;
            trailWidth = 15;
            trailMaxLength = 155;
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center + Projectile.velocity;
            TrailLogic();

            Player player = Main.player[Projectile.owner];
            player.itemAnimation = (int)MathHelper.Clamp(player.itemAnimation, 2, player.itemAnimation);
            player.itemTime = (int)MathHelper.Clamp(player.itemTime, 2, player.itemTime);

            Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();

            switch (state) 
            {
                
                case NeedleState.JustFired:
                    JustFiredAI( player); break;
                case NeedleState.Latched:
                    LatchedAI( player);
                    Projectile.velocity = Vector2.Zero;
                    break;
                case NeedleState.Swinging:
                    SwingingAI( player);
                    Projectile.velocity = Vector2.Zero;
                    break;
                case NeedleState.Returning:
                    ReturningAI( player); break;
            
            }






        }

        public void JustFiredAI( Player player) 
        {

            Timer++;
            Projectile.extraUpdates = 1;
            Progress = (Timer) / (12f);
            Projectile.velocity = Vector2.Lerp(startingVel, Vector2.Zero, InExpo(Progress,11f));
            if(Progress == 1f) 
            {
            
                state = NeedleState.Returning;
                hitNpcCenterOffset = Projectile.Center;
                
            }
        }

        public void LatchedAI( Player player) 
        {
        
            if(hitNpc == null || !hitNpc.active)
            {
                state = NeedleState.Returning;
                return;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);

            Projectile.Center = hitNpc.Center + hitNpcCenterOffset;
            Timer++;
            Progress = Timer / 30f;

            if (Timer < 30)
                return;


            Vector2 dir = hitNpc.DirectionTo(player.Center);
            var blood = Projectile.NewProjectileDirect(null, Projectile.Center + dir * 25, Vector2.Zero, ModContent.ProjectileType<NeedleBlood>(), 0, 0, Main.myPlayer);
            blood.rotation = dir.ToRotation();
            SoundStyle swif = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = 0f, Volume = 0.27f };
            SoundEngine.PlaySound(swif, Projectile.Center);
            SkillStrikeUtil.setSkillStrike(Projectile, 1.2f, impactVolume: 0.35f);
            Projectile.ResetLocalNPCHitImmunity();
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 18;
            for (int i = 0; i < 15; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, dir.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(15f,30f));
            state = NeedleState.Returning;
            
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(state == NeedleState.Returning && target == hitNpc) 
            {

                SkillStrikeUtil.setSkillStrike(Projectile, 3.5f,impactVolume: 0.35f);
                if (target.boss) 
                {
                    // deal bonus max HP damage to low max hp bosses cuz its kinda difficult to skill strike bosses like EOC with this weapon
                    int maxLifedamageScale = (int)(3000f * (Main.expertMode ? Main.masterMode ? 1.2f : 1.1f : 1f));
                    int maxLifeDamage = (int)(target.lifeMax * 0.025f);
                    modifiers.FinalDamage += Utils.GetLerpValue(maxLifeDamage,0, target.life / maxLifedamageScale,true);
                
                }

            }

            

        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public void SwingingAI( Player player) 
        {
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
            Timer++;
            Vector2 arcHeight = new Vector2(0, -555 );
            Progress = ( Timer / 32f);
            Vector2 endPoint = player.Center + arcHeight * Utils.PingPongFrom01To010(OutSine(Progress));
            Projectile.Center = Vector2.Lerp(hitNpcCenterOffset, endPoint, Progress);
            KillOnPlayerReached(player);

        }

        public void ReturningAI( Player player) 
        {
            Timer++;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);

            if (Timer < 1)
                return;

            if (hitNpc != null)
                Projectile.extraUpdates = 1;


            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
            Progress = Timer / 12f;
            Projectile.Center = Vector2.Lerp(Projectile.Center,player.Center,Progress);
            KillOnPlayerReached(player);
        }

        public void KillOnPlayerReached(Player player) 
        {

            if (Projectile.Distance(player.Center) < 4)
                Projectile.Kill();

        }
    }
    public class NeedleBlood : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        static Asset<Texture2D> Tex;
        public override void Load()
        {
            Tex = Mod.Assets.Request<Texture2D>("Assets/BloodHit");
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 26;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;


        public override void AI()
        {
            if (Projectile.frame == 0 && Projectile.timeLeft == 200)
            {
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 1)
            {
                if (Projectile.frame == 9)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {


            int frameHeight = Tex.Height() / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width(), frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            

            Main.spriteBatch.Draw(Tex.Value, Projectile.Center - Main.screenPosition, sourceRectangle, Color.Red, Projectile.rotation + MathHelper.PiOver2, origin, new Vector2(0.5f,1.5f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex.Value, Projectile.Center - Main.screenPosition, sourceRectangle, Color.DarkRed, Projectile.rotation + MathHelper.PiOver2, origin, new Vector2(0.5f, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex.Value, Projectile.Center - Main.screenPosition, sourceRectangle, Color.Crimson, Projectile.rotation + MathHelper.PiOver2, origin, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
    public class NeedleHit : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        static Asset<Texture2D> Tex;
        public override void Load()
        {
            Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Pincer");

        }
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 26;
            Projectile.timeLeft = 15;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            Projectile.velocity *= 0.9533f;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(255,0,0,0), Color.Red, Projectile.timeLeft / 15f), Projectile.rotation + MathHelper.PiOver4, Tex.Size() /2f, new Vector2(MathHelper.Lerp(0f,2f, Projectile.timeLeft / 15f)), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

    }
    
}
