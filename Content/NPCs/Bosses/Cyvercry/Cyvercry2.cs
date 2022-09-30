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
        }
        ArmorShaderData dustShader = null; 

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
                    Vector2 from = NPC.Center;

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

            /*
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                    Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw(CyverTexture, drawPos, NPC.frame, color * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            */
            return false;
        }

        int whatAttack = 0;
        int timer = 0;

        public override void AI()
        {

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player myPlayer = Main.player[NPC.target];

            switch (whatAttack)
            {
                case 0:
                    IdleLaser(myPlayer);
                    break;
                case 1:
                    IdleDash(myPlayer);
                    break;
            }


            if (timer == 100)
            {
                SkyManager.Instance.Activate("AerovelenceMod:Cyvercry2");
            }

            NPC.velocity = (myPlayer.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;

            thrusterValue = Math.Clamp(MathHelper.Lerp(thrusterValue, 3, 0.06f), 0, 2);
            pinkGlowMaskTimer++;
            timer++;

        }

        //Laser Attacks
        public void IdleLaser(Player myPlayer)
        {

        }


        //Dash Attacks
        public void IdleDash(Player myPlayer)
        {

        }

        //Special Attacks

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
        }
        bool hasDoneDrop = false;

    }
}
