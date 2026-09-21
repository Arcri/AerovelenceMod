using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;
using Terraria.Graphics;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class TelegraphLineCyver : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public Vector2 endPoint;
        public float Rotation = 0;

        public bool sweepTell = false;
        public bool sweepDir = false;

        float rotOffset = 0f;

        public bool custom = false;
        public int timeToLast = 0;

        int timer = 0;

        public NPC NPCTetheredTo = null;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 110 + 5;
            Projectile.hide = true;
        }
        public override bool? CanDamage() {  return false; }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Rotation = Projectile.velocity.ToRotation();
                rotOffset = Rotation;
            }


            if (NPCTetheredTo != null)
            {
                if (NPCTetheredTo.active == false)
                    Projectile.active = false;
                if (sweepTell)
                {
                    uColorIntensity = 0.9f;
                    Projectile.Center = NPCTetheredTo.Center + NPCTetheredTo.rotation.ToRotationVector2() * -30;
                    Rotation += 0.06f * (sweepDir ? 1 : -1);// + (timer * 0.002f);
                } 
                else
                {
                    Projectile.Center = NPCTetheredTo.Center;
                    Rotation = NPCTetheredTo.rotation + MathHelper.Pi + rotOffset;
                }


            }
            endPoint = Projectile.Center + Rotation.ToRotationVector2() * 2500f;
            Projectile.velocity = Vector2.Zero;

            Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(0f, 1f, Easings.easeInQuad(timer / 15f)), 0f, 1f);
            timer++;
        }

        public float uColorIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(255, 25, 155).ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.3f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = 30f * Projectile.scale * (sweepTell ? 0.5f : 1); //15
                float height2 = 15f * Projectile.scale * (sweepTell ? 0.5f : 1); //15

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Rotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1f));
                var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height2 * 1f));



                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink * Projectile.ai[0], Rotation, origin2, 0, 0);

                if (!sweepTell)
                {
                    Main.spriteBatch.Draw(texBeam, target2, null, Color.HotPink * Projectile.ai[0], Rotation, origin2, 0, 0);

                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}