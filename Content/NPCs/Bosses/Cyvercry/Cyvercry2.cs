using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using System;
using Terraria.Graphics.Effects;
using System.Net;
using AerovelenceMod.Content.Dusts;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry //Change me
{
    [AutoloadBossHead]
    public class Cyvercry2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyvercry2"); //DONT Change me
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
        }
        ArmorShaderData dustShader = null;
        ArmorShaderData dustShader2 = null;


        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 37500;
            NPC.damage = 105;
            NPC.defense = 30;
            NPC.knockBackResist = 0f;
            NPC.width = 100;
            NPC.height = 100;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4 with { Pitch = -0.5f, PitchVariance = 0.14f};
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = Item.buyPrice(0, 22, 11, 5);   
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Cyvercry");
            }
            dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 41500;
            NPC.damage = 125;
            NPC.defense = 35;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if(NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 4 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if(NPC.frame.Y == 3 * frameHeight) //booster frame
                {

                    for (int i = 0; i < 5; i++) //4 //2,2
                    {
                        Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.Next(5, 10) * -1f;

                        Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + vel * -5, ModContent.DustType<GlowCircleQuadStar>(), vel * -1f,
                            Color.DeepSkyBlue, Main.rand.NextFloat(0.6f, 1.2f), 0.4f, 0f, dustShader);
                        p.noLight = true;
                        p.velocity += NPC.velocity * (0.4f + Main.rand.NextFloat(-0.1f, -0.2f));
                        //p.rotation = Main.rand.NextFloat(6.28f);
                    }

                    /*
                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(24, 0).RotatedBy(MathHelper.ToRadians(i));
                        circular.Y *= 0.7f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = new Vector2(12, 0).RotatedBy(NPC.rotation);


                        Dust d = GlowDustHelper.DrawGlowDustPerfect(from - new Vector2(5) + circular, ModContent.DustType<GlowCircleSpinner>(), dustVelo, new Color(67, 215, 209), 0.4f, 0.6f, 0f, dustShader);
                        d.scale = 0.4f;
                    }
                    */
                    thrusterValue = 0;
                    ThrusterRotaion = Main.rand.NextBool();
                }
            }
        }


        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;

            DownedWorld.DownedCyvercry = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
        }
        float pinkGlowMaskTimer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //PinkGlow
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 from = NPC.Center + new Vector2(-98, 0).RotatedBy(NPC.rotation);
            Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Main.EntitySpriteDraw(Ball, from - Main.screenPosition, Ball.Frame(), Color.HotPink, NPC.rotation, Ball.Frame().Size() / 2f, (ballScale / 150) * 0.08f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Ball, from - Main.screenPosition, Ball.Frame(), Color.HotPink, NPC.rotation, Ball.Frame().Size() / 2f, (ballScale / 150) * 0.13f, SpriteEffects.None, 0);


            Texture2D Bloommy = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/RegreGlowCyvercry");
            Main.EntitySpriteDraw(Bloommy, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Vector2 drawOffset = new Vector2(0, 0).RotatedBy(NPC.rotation);
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskPink");
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White * (0.5f * (float)Math.Sin(pinkGlowMaskTimer / 60f) + 0.5f), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            //Blue Glow
            Texture2D texture3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
            Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

        }

        bool ThrusterRotaion = Main.rand.NextBool();
        float thrusterValue = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Vector2 drawOriginAI = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Cyvercry").Value;
            if (NPC.velocity.Length() > 18f)
            {
                Color drawingCol = drawColor;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOriginAI + new Vector2(0f, NPC.gfxOffY);
                    drawingCol = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos + new Vector2(-40,0), NPC.frame, drawingCol * 0.5f, NPC.rotation, drawOriginAI, NPC.scale, SpriteEffects.None, 0);
                }
            }

            /*
            Texture2D CyverPink = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskPink");
            Vector2 drawOrigin2 = new Vector2(CyverPink.Width * 0.5f, NPC.height * 0.5f);
            Color color = Color.White;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin2 + new Vector2(0f, NPC.gfxOffY);
                color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                Main.EntitySpriteDraw(CyverPink, drawPos + new Vector2(-40,0), NPC.frame, color, NPC.rotation, drawOrigin2, NPC.scale, SpriteEffects.None, 0);
            }
            */

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Texture2D texture3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/CyverGlowMaskBlue");
            //Main.EntitySpriteDraw(texture3, NPC.Center - Main.screenPosition + drawOffset, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/muzzle_05").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(0, 255, 255).ToVector3() * (2 - thrusterValue));
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition + new Vector2(66,0).RotatedBy(NPC.rotation), null, Color.Black, NPC.rotation + MathHelper.PiOver2, texture2.Size() / 2, 0.3f, ThrusterRotaion ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);

            Texture2D CyverTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/CyvercryNoThruster").Value;

            Main.EntitySpriteDraw(CyverTexture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * 1f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        int whatAttack = 0;
        int timer = 0;
        int advancer = 0;
        float accelFloat = 0;

        public override void AI()
        {

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player myPlayer = Main.player[NPC.target];

            //
            //Cyver moves to point on rotating circle edge

            



            switch (whatAttack)
            {
                case 0:
                    IdleLaser(myPlayer);
                    break;
                case 1:
                    Spin(myPlayer);
                    break;
                case 2: 
                    IdleDash(myPlayer);
                    break;
            }


            if (timer == 20)
            {
                SkyManager.Instance.Activate("AerovelenceMod:Cyvercry2");
            }



            //NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4;
            //NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;

            thrusterValue = Math.Clamp(MathHelper.Lerp(thrusterValue, 3, 0.06f), 0, 2);
            pinkGlowMaskTimer++;

        }

        //Laser Attacks
        public void IdleLaser(Player myPlayer)
        {
            Vector2 goalPoint = new Vector2(550, 0).RotatedBy(MathHelper.ToRadians(advancer * 0.4f + 60));

            Vector2 move = (goalPoint + myPlayer.Center) - NPC.Center;

            float scalespeed = 0.6f * 2f;//2
            
            NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
            NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

            NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();


            if (timer % 30 == 0 && timer > 40)
            {
                SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, MaxInstances = -1 };
                SoundEngine.PlaySound(stylea, NPC.Center);
                SoundEngine.PlaySound(stylea, NPC.Center);


                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, NPC.Center);
                SoundEngine.PlaySound(styleb, NPC.Center);


                SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .38f, Volume = 0.7f }; //1f
                SoundEngine.PlaySound(stylec, NPC.Center);

                Dust a = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - new Vector2(70, 0).RotatedBy(NPC.rotation), ModContent.DustType<GlowCircleQuadStar>(),
                (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 12, Color.HotPink, 0.7f, 0.2f, 0f, dustShader2);

                /*
                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(i));
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - new Vector2(70, 0).RotatedBy(NPC.rotation), ModContent.DustType<LineGlow>(),
                        circular * 2, Color.DeepPink, Main.rand.NextFloat(0.2f, 0.2f), 0.2f, 0f, dustShader2);
                    p.fadeIn = 50;

                }
                */

                /*
                int pulseIndex = Projectile.NewProjectile(null, NPC.Center - new Vector2(80, 0).RotatedBy(NPC.rotation), (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 3, ModContent.ProjectileType<CyverLaserPulse>(), 0, 0, Main.myPlayer);
                Projectile pulse = Main.projectile[pulseIndex];
                if (pulse.ModProjectile is CyverLaserPulse pulse2)
                {
                    pulse2.ParentIndex = NPC.whoAmI;
                }
                */
                FireLaser(ModContent.ProjectileType<CyverLaser>()); //Death Laser
            }

            if (timer == 220)
            {
                advancer = 0;
                timer = -1;
                whatAttack = 2;
            }

            advancer = (timer > 140 ? ++advancer : advancer + 2);
            timer++;
        }


        //Dash Attacks

        float storedRotaion = 0;
        public void IdleDash(Player myPlayer)
        {
            if (timer < 110)
            {
                Vector2 goalPoint;
                if (timer < 70)
                    goalPoint = new Vector2(-350, 0).RotatedBy(MathHelper.ToRadians(advancer * -0.6f)); //250 || 0.4
                else
                    goalPoint = new Vector2(-500, 0).RotatedBy(MathHelper.ToRadians(advancer * -0.6f)); //250 || 0.4

                Vector2 move = (goalPoint + myPlayer.Center) - NPC.Center;

                float scalespeed = (timer < 70 ? 0.6f * 4f : 0.6f * 2f);//2

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                if (timer < 70)
                    NPC.rotation = MathHelper.ToRadians(180) + (myPlayer.Center - NPC.Center).ToRotation();
                else
                    NPC.rotation = storedRotaion;

            }
            else if (timer >= 110)
            {
                if (NPC.velocity.Length() > 20)
                    Dust.NewDust(NPC.Center, 12, 12, ModContent.DustType<DashTrailDust>(), NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 0, new Color(0, 255, 255), 1f);
                if (timer == 118) //120
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = .15f, MaxInstances = -1, };
                    SoundEngine.PlaySound(style, NPC.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = .59f, Pitch = 1f, MaxInstances = -1 }; 
                    SoundEngine.PlaySound(style2, NPC.Center);
                }
                accelFloat = MathHelper.Clamp(MathHelper.Lerp(accelFloat, 50f, 0.2f), 0, 40f);
                NPC.rotation = storedRotaion;
                NPC.velocity = storedRotaion.ToRotationVector2() * accelFloat * -1;
            }

            if (timer == 69)
            {
                NPC.velocity = Vector2.Zero;
                storedRotaion = NPC.rotation;
            }

            if (timer == 135)
            {
                advancer = 0;
                timer = -1;
                whatAttack = 1;
            }
            if (timer < 70)
                advancer++;
            timer++;
        }

        //Special Attacks

        bool spammingLaser = false;
        float ballScale = 0;
        public void Spin(Player myPlayer)
        {
            if (timer == 20)
            {
                int retIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CyverReticle>(), 0, 0, myPlayer.whoAmI);
                Projectile Reticle = Main.projectile[retIndex];
                if (Reticle.ModProjectile is CyverReticle target)
                {
                    target.ParentIndex = NPC.whoAmI;
                    //Main.instance.DrawCacheProjsBehindNPCs.Add(sawIndex);
                }

                Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);

                for (int i = 0; i < 360; i += 20)
                {
                    Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(i));
                    //circular.X *= 0.6f;
                    circular = circular.RotatedBy(NPC.rotation);
                    Vector2 dustVelo = -circular * 0.1f;

                    Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.4f, 0.6f, 0f, dustShader2);
                }

                SoundStyle style = new SoundStyle("Terraria/Sounds/Zombie_68");
                SoundEngine.PlaySound(style, NPC.Center);
            }
            if (timer <= 150)
            {
                ballScale += 1;
                if (timer >= 20)
                {
                    Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(j * 120 + timer * 4));
                        //circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.09f;
                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleQuadStar>(), Vector2.Zero, Color.DeepPink, 0.4f, 0.6f, 0f, dustShader2);
                    }
                }
                
                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4;
            }
            else
                NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 0.5f;

            NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.Pi;

            if (timer > 150)
            {
                if (timer == 151)
                {
                    ballScale = 0;
                    Vector2 from = NPC.Center + new Vector2(-102, 0).RotatedBy(NPC.rotation);

                    for (int i = 0; i < 360; i += 20)
                    {
                        Vector2 circular = new Vector2(32, 0).RotatedBy(MathHelper.ToRadians(i));
                        //circular.X *= 0.6f;
                        circular = circular.RotatedBy(NPC.rotation);
                        Vector2 dustVelo = -circular * 0.1f;

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(from + circular, ModContent.DustType<GlowCircleDust>(), Vector2.Zero, Color.DeepPink, 0.5f, 0.6f, 0f, dustShader2);
                    }
                }


                spammingLaser = true;
                if (timer % 5 == 0)
                {
                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .56f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(stylea, NPC.Center);
                    SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .33f, Pitch = .73f, PitchVariance = .27f, };
                    SoundEngine.PlaySound(styleb, NPC.Center);
                    FireLaser(ModContent.ProjectileType<CyverLaser>(), 13f, 0.7f);
                }
            }
            if (timer == 400)
            {
                ballScale = 0;
                spammingLaser = false;
                timer = -1;
                whatAttack = 0;
            }
            timer++;
        }

        public void FireLaser(int type, float speed = 6f, float recoilMult = 2f, float ai1 = 0, float ai2 = 0)
        {
            var entitySource = NPC.GetSource_FromAI();
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;
            toPlayer = toPlayer.SafeNormalize(new Vector2(1, 0));
            toPlayer *= speed;
            Vector2 from = NPC.Center - new Vector2(96, 0).RotatedBy(NPC.rotation);
            int damage = 75 / 4;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(entitySource, from, toPlayer, type, damage, 3, Main.myPlayer, ai1, ai2);
            }
            NPC.velocity -= toPlayer * recoilMult;

            if (type == ModContent.ProjectileType<CyverLaser>())
            {

                float addition = (whatAttack == 0 ? -1 : 0);
                for (int i = 0; i < 4 + (addition * 2); i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.4f;
                }
                for (int i = 0; i < 3 - addition; i++) //4 //2,2
                {

                    Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(5, 10) * -1f;

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * -50, ModContent.DustType<GlowLine1Fast>(), vel * 3f,
                        Color.HotPink, Main.rand.NextFloat(0.1f, 0.2f), 0.6f, 0f, dustShader2);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = (whatAttack == 0 ? 30 : 40) + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.2f;
                }


            }

        }

        //This is for the custom sky being reactive
        public int getAttack()
        {
            if (spammingLaser)
                return -1;
            else
                return whatAttack;
        }
    }
}
