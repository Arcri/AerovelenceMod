using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.BookOfBees
{
    public class BookOfBees : ModItem
    {
        
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = 2;

            Item.DamageType = DamageClass.Magic;

            Item.rare = ItemRarityID.Yellow;
            Item.width = 28;
            Item.height = 28;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 12f;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.shoot = ModContent.ProjectileType<HiveProjectile>();
            //item.UseSound = SoundID.Item92;
            Item.noUseGraphic = false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
    }

    public class HiveProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        int timer = 0;
        public override void AI()
        {
            int trailCount = 16;
            previousRotations.Add(Projectile.rotation);
            previousPositions.Add(Projectile.Center + Projectile.velocity);
            previousVelrots.Add(Projectile.velocity.ToRotation());

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);

            if (previousVelrots.Count > trailCount)
                previousVelrots.RemoveAt(0);

            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            Projectile.velocity *= 0.99f;
            if (timer > 4)
                Projectile.velocity.Y += 0.35f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation += Projectile.velocity.Length() * 0.02f * (Projectile.velocity.X > 0 ? 1 : -1);


            int mod = Main.player[Projectile.owner].strongBees ? 20 : 28;
            if (timer % mod == 0 && timer != 0)
            {
                //int beeType = Main.player[Projectile.owner].beeType();

                Vector2 vel = Main.rand.NextVector2Circular(1f, 1f) + Projectile.velocity * 0.15f;
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel * 0.15f, ProjectileID.Wasp, (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Main.projectile[proj].penetrate = 1;

                //So apparently extra updates just dont fucking work on bees lmao
                //Main.projectile[proj].extraUpdates = 50;

                for (int i = 0; i < 4 + Main.rand.Next(0, 4); i++)
                {
                    Vector2 dustVel = vel.RotateRandom(0.3f) * Main.rand.NextFloat(2f, 5f);
                    if (i % 2 == 0)
                    {
                        Color col = Color.Lerp(Color.Orange, Color.OrangeRed, 0.35f);

                        Dust dp = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), dustVel,
                            newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.5f);

                        dp.noGravity = true;
                    }
                    else
                    {
                        Color col = Color.Black;

                        Dust dp = Dust.NewDustPerfect(Projectile.Center, DustID.Bee, dustVel,
                            newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 1.5f);

                        dp.noGravity = true;
                    }
                }

            }

            float fadeInTime = Math.Clamp((timer + 9f) / 25f, 0f, 1f);
            overallScale = Easings.easeInOutBack(fadeInTime, 0f, 1f);

            timer++;
        }

        float overallAlpha = 1f;
        float overallScale = 1f;
        public List<float> previousVelrots = new List<float>();
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D MainTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/BookOfBees/HiveProjectile").Value;
            Texture2D Orb = CommonTextures.feather_circle128PMA.Value;
            Texture2D flare = CommonTextures.Flare.Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 TexOrigin = MainTex.Size() / 2f;
            SpriteEffects SE = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                DrawTrail();
            });

            //Border
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(MainTex, drawPos + Main.rand.NextVector2Circular(2f, 2f), null,
                    Color.Gold with { A = 0 } * 0.35f, Projectile.rotation, TexOrigin, Projectile.scale * overallScale * 1.1f, SE);
            }

            //MainTex
            Main.EntitySpriteDraw(MainTex, drawPos, null, lightColor, Projectile.rotation, TexOrigin, Projectile.scale * overallScale, SE);

            return false;
        }

        public void DrawTrail()
        {
            Texture2D MainTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/BookOfBees/HiveProjectile").Value;
            Texture2D Orb = CommonTextures.feather_circle128PMA.Value;
            Texture2D flare = CommonTextures.Flare.Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 TexOrigin = MainTex.Size() / 2f;
            SpriteEffects SE = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(Orb, drawPos, null, Color.Goldenrod with { A = 0 } * 0.15f, Projectile.rotation, Orb.Size() / 2f, 0.7f * Projectile.scale * overallScale, SE);
            //After-Image
            for (int i = 0; i < previousPositions.Count; i++)
            {
                float progress = (float)i / previousRotations.Count;

                //Start End
                Color col = Color.Lerp(Color.Orange * 1f, Color.Gold, 1f - Easings.easeInOutCubic(progress));

                float size1 = Easings.easeOutSine(progress) * overallScale * Projectile.scale;

                Vector2 AfterImagePos = previousPositions[i] - Main.screenPosition;

                Main.EntitySpriteDraw(MainTex, AfterImagePos, null, col with { A = 0 } * progress * 0.15f,
                    previousRotations[i], TexOrigin, size1 * overallScale, SE);

                if (i < previousPositions.Count - 1)
                {
                    float middleProg = (float)i / previousPositions.Count;

                    float size2 = (0.5f + (0.5f * progress));
                    Vector2 vec2Scale = new Vector2(1f, 0.75f * size2) * overallScale * Projectile.scale * 1f;
                    Vector2 vec2Scale2 = new Vector2(1f, 0.4f * size2) * overallScale * Projectile.scale * 1f;

                    Color trailCol = Color.Orange * 1.1f;
                    Main.EntitySpriteDraw(flare, AfterImagePos, null, trailCol with { A = 0 } * 0.25f * middleProg,
                        previousVelrots[i], flare.Size() / 2f, vec2Scale, SpriteEffects.None);

                }
            }
        }

        public override bool PreKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);

            Vector2 pos = Projectile.Center + Projectile.oldVelocity;

            float triRand = Main.rand.NextFloat(6.28f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = new Vector2(1f, 0f).RotatedBy(triRand + ((MathHelper.TwoPi / 3f) * i));

                int beeType = Main.player[Projectile.owner].beeType();
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileID.Wasp, (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
                Main.projectile[proj].penetrate = 1;

            }

            for (int i = 0; i < 16; ++i)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);

                Dust dp = Dust.NewDustPerfect(pos, 153, vel * Main.rand.NextFloat(2f, 4.5f),
                    newColor: Color.White, Scale: Main.rand.NextFloat(0.9f, 1.3f) * 1.35f);

                dp.noGravity = true;
            }

            for (int i = 0; i < 18 + Main.rand.Next(0, 4); i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                Color col = Color.Black;

                Dust dp = Dust.NewDustPerfect(pos, DustID.Bee, vel * Main.rand.NextFloat(1.5f, 2.5f),
                    newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 1.55f);

                dp.noGravity = true;
            }

            for (int i = 0; i < 8; i++)
            {
                float prog = (float)i / 7f;

                float proggg = Main.rand.NextFloat();
                Color col = Color.Lerp(Color.Gold, Color.DarkGoldenrod * 0.8f, Easings.easeOutSine(proggg));

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MediumSmoke>(), Velocity: Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.9f, 3f) * 1f,
                    newColor: col * prog, Scale: Main.rand.NextFloat(0.9f, 1.35f));
                d.customData = new MediumSmokeBehavior(Main.rand.Next(15, 23), 0.93f, 0.01f, 0.95f); //12 28

            }

            return base.PreKill(timeLeft);
        }
    }

}