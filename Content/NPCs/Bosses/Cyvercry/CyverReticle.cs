using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class CyverReticle : ModProjectile
    {
        float scaleVal = 0;
        public int timer = 0;
        public int timeToLast = 350;

        public int ParentIndex
        {
            get => (int)Projectile.ai[0] - 1;
            set => Projectile.ai[0] = value + 1;
        }

        float rotationValue = 0f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Target");

        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            Projectile.Center = player.Center;
            //Projectile.rotation = MathHelper.ToRadians(timer);

            NPC parentNPC = Main.npc[ParentIndex];

            if (parentNPC != null)
            {
                rotationValue = player.AngleTo(parentNPC.Center);
            }
            if (parentNPC.life <= 0)
            {
                Projectile.active = false;
            }
            
            if (timer < timeToLast)
            {
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, 1.5f, 0.03f), 0f, 1f);
            }
            else
            {
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, -0.5f, 0.03f), 0f, 1f);
            }

            boostPower = Math.Clamp(MathHelper.Lerp(scaleVal, -0.5f, 0.03f), 0f, 1f);

            timer++;

        }

        public float fadeProgress = 1f;
        public float boostPower = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[(int)Projectile.ai[0]];

            Texture2D Inner = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/NewInnerRet").Value;
            Texture2D Outer = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/NewOuterRet").Value;
            Texture2D Danger = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger5").Value;

            Main.spriteBatch.Draw(Danger, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), Danger.Frame(1, 1, 0, 0), Color.Pink with { A = 0 } * scaleVal, 0f, Danger.Size() / 2, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Danger, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), Danger.Frame(1, 1, 0, 0), Color.White * scaleVal * 0.85f, 0f, Danger.Size() / 2, 2f, SpriteEffects.None, 0f);


            float scale = 0.75f;
            float newRot = MathHelper.Lerp(0f, MathF.PI * 2f, boostPower) + rotationValue;
            float distance = MathHelper.Lerp(70f, 35f, scaleVal);

            Main.spriteBatch.Draw(Outer, Projectile.Center - Main.screenPosition + new Vector2(0, distance).RotatedBy(newRot), Outer.Frame(1, 1, 0, 0), Color.White with { A = 0 } * scaleVal, newRot + MathHelper.Pi, Outer.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Outer, Projectile.Center - Main.screenPosition + new Vector2(0,-distance).RotatedBy(newRot), Outer.Frame(1, 1, 0, 0), Color.White with { A = 0 } * scaleVal, newRot, Outer.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Inner, Projectile.Center - Main.screenPosition + new Vector2(0, distance).RotatedBy(newRot), Inner.Frame(1, 1, 0, 0), Color.HotPink * scaleVal * 0.8f, newRot + MathHelper.Pi, Inner.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Inner, Projectile.Center - Main.screenPosition + new Vector2(0, -distance).RotatedBy(newRot), Inner.Frame(1, 1, 0, 0), Color.HotPink * scaleVal * 0.8f, newRot, Inner.Size() / 2, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }

    
    public class CyverDangerSign : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Target");

        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        int timer = 0;
        float scaleVal = 0;
        public override void AI()
        {
            //Player player = Main.player[Main.myPlayer];

            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            
            //Projectile.Center = player.Center;

            if (Projectile.timeLeft > 50)
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, 1.5f, 0.05f), 0f, 1f);
            else
                scaleVal = Math.Clamp(MathHelper.Lerp(scaleVal, -0.5f, 0.1f), 0f, 1f);

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex3 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/GlowConnectorSolid").Value;
            Texture2D Tex4 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/GlowConnectorGhost").Value;

            Texture2D Danger = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger4").Value;
            Texture2D DangerGlow = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/Danger4Glow").Value;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            for (int j = 0; j < 4; j++)
            {
                for (int i = -2; i < 3; i++)
                {
                    if (i != 0)
                    {
                        Main.spriteBatch.Draw(Tex4, Projectile.Center - Main.screenPosition + new Vector2(25 * i, 0).RotatedBy(j * MathHelper.PiOver4), Tex4.Frame(1, 1, 0, 0), Color.White, j * MathHelper.PiOver4, Tex4.Size() / 2, scaleVal * 1.75f, SpriteEffects.None, 0f);
                    }
                }
            }

            Main.spriteBatch.Draw(DangerGlow, Projectile.Center - Main.screenPosition + new Vector2(0, -5), DangerGlow.Frame(1, 1, 0, 0), Color.White, 0f, DangerGlow.Size() / 2, 1.5f * scaleVal, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + new Vector2(0, 35).RotatedBy(rotationValue), Tex2.Frame(1, 1, 0, 0), Color.White * scaleVal, rotationValue + MathHelper.Pi, Tex2.Size() / 2, 2, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex2, Projectile.Center - Main.screenPosition + new Vector2(0, -35).RotatedBy(rotationValue), Tex2.Frame(1, 1, 0, 0), Color.White * scaleVal, rotationValue, Tex2.Size() / 2, 2, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int j = 0; j < 4; j++)
            {
                for (int i = -2; i < 3; i++)
                {
                    if (i != 0)
                    {
                        Main.spriteBatch.Draw(Tex3, Projectile.Center - Main.screenPosition + new Vector2(25 * i, 0).RotatedBy(j * MathHelper.PiOver4), Tex3.Frame(1, 1, 0, 0), Color.White, j * MathHelper.PiOver4, Tex3.Size() / 2, scaleVal * 1.75f, SpriteEffects.None, 0f);
                    }
                }
            }


            Main.spriteBatch.Draw(Danger, Projectile.Center - Main.screenPosition + new Vector2(0,-5), Danger.Frame(1, 1, 0, 0), Color.White, 0f, Danger.Size() / 2, 1.5f * scaleVal, SpriteEffects.None, 0f);
            



            return false;
        }

    }
    
} 