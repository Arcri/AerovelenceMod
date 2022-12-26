using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.Rimegeist
{
    public class BigSoul : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rimegeist");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0f;
        }
        public Player target = Main.player[Main.myPlayer];
        public bool leftOrRight = false;
        public override void AI()
        {
            target = Main.player[Main.myPlayer];

            if (timer % 2 == 0 && timer > 20)
            {
                //ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                //int dust = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleSoft>(), 
                //Color.DarkGray * 0.5f, 3f, 1, 0, dustShader);
                //Main.dust[dust].velocity *= 5;
                int dust1 = Dust.NewDust(Projectile.position,  Projectile.width, Projectile.height, ModContent.DustType<VoidDust>(), Scale: 1f );
                Main.dust[dust1].velocity *= 4f;




            }

            if (timer < 60)
            {
                Projectile.localAI[0] = (float)timer / 700;
                //move to player left then disapate 
                float scalespeed = MathHelper.Clamp((timer * 1.4f) / 150f, 0f, 1f) * 7f;
                Vector2 move = target.Center - Projectile.Center;

                Projectile.velocity.X = (Projectile.velocity.X + move.X + (leftOrRight ? 500 : -500)) / 20f * scalespeed;
                Projectile.velocity.Y = (Projectile.velocity.Y + move.Y) / 20f * scalespeed;

                Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 3.5f, Projectile.localAI[0]), 0, 3);


            }
            if (timer >= 60)
            {
                if (timer == 60)
                {

                    for (int i = 0; i < 40; i++)
                    {
                        int dust1 = Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<VoidDust>(), Scale: 2f);
                        Main.dust[dust1].velocity *= 4f;
                    }

                    for (int i = -3; i < 4; i++)
                    {
                        if (i != 10)
                        {
                            Vector2 vel = new Vector2(7 * (leftOrRight ? -1 : 1), 0).RotatedBy(MathHelper.ToRadians(i * 20));
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<WispSouls>(), 10, 0);
                        }

                    }

                    SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .32f, PitchVariance = .26f, Volume = 0.7f };
                    SoundEngine.PlaySound(stylecs, Projectile.Center);

                    SoundStyle stylege = new SoundStyle("Terraria/Sounds/Item_103") with { Volume = .8f, Pitch = .82f, MaxInstances = 1, PitchVariance = 0.3f };
                    SoundEngine.PlaySound(stylege, Projectile.Center);

                    SoundStyle styleas = new SoundStyle("Terraria/Sounds/Item_131") with { Pitch = .16f, PitchVariance = .33f, Volume = 0.95f };
                    SoundEngine.PlaySound(styleas, Projectile.Center);
                }
                float velMultiplier = MathHelper.Clamp(MathHelper.Lerp((timer - 40), 50, 0.02f), 0, 30);
                Projectile.velocity = new Vector2((leftOrRight ? -1 : 1), 0) * velMultiplier;
            }
            //Projectile.width = (int)(50 * Projectile.scale);
            //Projectile.height = (int)(50 * Projectile.scale);
            timer++;
        }

        string AssetDirectory = "AerovelenceMod/Content/NPCs/Bosses/Rimegeist/";

        public override bool PreDraw(ref Color lightColor)
         {
            Texture2D solidColor = (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "BigSoulDark");
            Texture2D Tex = (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "BigSoul");

            Vector2 previousCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            Vector2 dir = previousCenter - Projectile.Center;

            Main.EntitySpriteDraw(solidColor, Projectile.Center - Main.screenPosition, solidColor.Frame(1,1,0,0), Color.Black, Projectile.rotation, solidColor.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            float i = 1;
            for (float f = 0; f < Projectile.velocity.Length(); f += 0.10f)
            {
                i *= 0.994f;
                float scale = 10f;
                Main.EntitySpriteDraw(solidColor, Projectile.Center + (Vector2.Normalize(Projectile.velocity) * -f * scale) - Main.screenPosition, solidColor.Frame(1, 1, 0, 0), Color.Black * (1f - (f / Projectile.velocity.Length())), Projectile.rotation, solidColor.Size() / 2f, Math.Clamp(Projectile.scale * i, 0, 10), SpriteEffects.None, 0);
            }

            //THis shit cool
            /*
            for (float f = 0; f < Projectile.velocity.Length(); f += 0.10f)
            {
                float scale = 3f;
                Main.EntitySpriteDraw(solidColor, Projectile.Center + (Vector2.Normalize(Projectile.velocity) * -f * scale) - Main.screenPosition, solidColor.Frame(1, 1, 0, 0), Color.Black * (1f - (f / Projectile.velocity.Length())), Projectile.rotation, solidColor.Size() / 2f, Projectile.scale - f, SpriteEffects.None, 0);
            }
            */

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1,1,0,0), Color.White, Projectile.rotation, Tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
} 