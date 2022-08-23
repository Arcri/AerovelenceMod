using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class CyverCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyver-Cannon");
            Tooltip.SetDefault("Fires lasers that split off into homing bolts");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.shootSpeed = 20f;
            Item.knockBack = 2f;
            Item.width = 70;
            Item.height = 38;
            Item.damage = 70;
            Item.shoot = Mod.Find<ModProjectile>("CyverCannonProj").Type;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5, 20, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
        }
    }

    public class CyverCannonProj : ModProjectile
    {
        public int Timer = 0;
        public float shootSpeed = 0.5f;

        bool upOrDown = false;

        float dustCycle = 0;

        Vector2 storedPlayerCenter = Vector2.Zero;
        Vector2 storedCenter = Vector2.Zero;
        Vector2 storedMouse = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 70;
            Projectile.aiStyle = 75;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Player player = Main.player[Projectile.owner];

            if (Timer == 0)
            {
                int dustInt = 0;

                switch (dustCycle)
                {
                    case 0:
                        dustInt = ModContent.DustType<GlowCircleSoft>();
                        break;
                    case 1:
                        dustInt = ModContent.DustType<GlowCircleRise>();
                        break;
                    case 2:
                        dustInt = ModContent.DustType<GlowCircleSpinner>();
                        break;
                    case 3:
                        dustInt = ModContent.DustType<GlowCircleRiseFlare>();
                        break;
                    case 4:
                        dustInt = ModContent.DustType<GlowCircleQuadStar>();
                        dustCycle = -1;
                        break;
                }
                dustCycle++;

                for (int j = 0; j < 20; j++)
                {
                    GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, dustInt, Vector2.One.RotatedByRandom(6) * 3, 
                        Color.ForestGreen * 1.5f, 1f, 0.8f, 0f,
                        dustShader);

                }
            }

            //Fires at 23, kills at 47
            var entitySource = Projectile.GetSource_FromAI();
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            {
                Projectile.ai[0] += 1f;
                int num2 = 0;
                //if (Projectile.ai[0] >= 40f)
                //{
                  //  num2++;
                //}
                //if (Projectile.ai[0] % 2 == 0f)
                //{
                    //num2++;
                //}
                //if (Projectile.ai[0] >= 120f)
                //{
                    //num2++;
                //}
                int num3 = 24;
                int num4 = 6;
                Projectile.ai[1] += 1f;
                bool flag = false;
                if (Projectile.ai[1] >= (float)(num3 - num4 * num2))
                {
                    Projectile.ai[1] = 0f;
                    flag = true;
                }
                Projectile.frameCounter += 1 + num2;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }
                if (Projectile.soundDelay <= 0)
                {
                    Projectile.soundDelay = num3 - num4 * num2;
                    if (Projectile.ai[0] != 1f)
                    {
                        //SoundStyle style = new SoundStyle("Terraria/Sounds/Research_2") with { Volume = .4f, Pitch = -.48f, PitchVariance = .34f, MaxInstances = 0, };
                        //SoundEngine.PlaySound(style);
                        //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1};
                        //SoundEngine.PlaySound(style);
                    }
                }
                if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
                {
                    Vector2 spinningpoint = Vector2.UnitX * 24f;
                    spinningpoint = spinningpoint.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    Vector2 value = base.Projectile.Center + spinningpoint;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 135, base.Projectile.velocity.X / 2f, base.Projectile.velocity.Y / 2f, 100);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == Projectile.owner)
                {
                    if (player.channel && player.CheckMana(player.inventory[player.selectedItem], -1, pay: true) && !player.noItems && !player.CCed)
                    {
                        float num6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                        Vector2 value2 = vector;
                        Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - value2;
                        if (player.gravDir == -1f)
                        {
                            value3.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
                        }
                        Vector2 velocity = Vector2.Normalize(value3);
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                        {
                            velocity = -Vector2.UnitY;
                        }
                        velocity *= num6;
                        if (velocity.X != base.Projectile.velocity.X || velocity.Y != base.Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity = velocity;
                        float scaleFactor = 14f;
                        int num8 = 7;
                        for (int j = 0; j < 1; j++)
                        {
                            //value2 = Projectile.Center + new Vector2(Main.rand.Next(-num8, num8 + 1), Main.rand.Next(-num8, num8 + 1));
                            //Vector2 spinningpoint2 = Vector2.Normalize(base.Projectile.velocity) * scaleFactor;
                            //spinningpoint2 = spinningpoint2.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914);
                            //if (float.IsNaN(spinningpoint2.X) || float.IsNaN(spinningpoint2.Y))
                            //{
                            //  spinningpoint2 = -Vector2.UnitY;
                            //}
                            if (Timer != 23)
                            {
                                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .8f, MaxInstances = 1 };
                                SoundEngine.PlaySound(style);
                                Vector2 vecToMouse = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                                Vector2 location = Projectile.Center + vecToMouse.SafeNormalize(Vector2.UnitX) * 18;
                                int a = Projectile.NewProjectile(entitySource, location.X, location.Y, vecToMouse.X, vecToMouse.Y, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                Main.projectile[a].scale = 1.85f;
                                Main.projectile[a].CritChance = 100;
                            }

                        }
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }

                if (Timer == 27 - 5 || Timer == 31 - 5 || Timer == 35 - 5)
                {
                    if (Timer == 27 - 5)
                    {
                        storedCenter = Projectile.Center;
                        storedMouse = Main.MouseWorld;
                        storedPlayerCenter = player.Center;
                    }
                    Vector2 vecToMouse = (storedMouse - storedPlayerCenter).SafeNormalize(Vector2.UnitX);
                    Vector2 adjustedVelocity = vecToMouse.RotatedBy(MathHelper.ToRadians(4));
                    Vector2 adjustedVelocity2 = vecToMouse.RotatedBy(MathHelper.ToRadians(-4));

                    //ugly, but only runs 3 times total and is clear
                    if (upOrDown)
                    {
                        if (Timer == 27 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, adjustedVelocity, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Timer == 35 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, adjustedVelocity2, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Timer == 31 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, vecToMouse, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    } else
                    {
                        if (Timer == 35 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, adjustedVelocity, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Timer == 27 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, adjustedVelocity2, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Timer == 31 - 5) Projectile.NewProjectile(entitySource, storedCenter + vecToMouse.SafeNormalize(Vector2.UnitX) * 18, vecToMouse, ModContent.ProjectileType<DarkLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    

                    //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                    //SoundEngine.PlaySound(style);
                    SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Pitch = 1.5f, PitchVariance = .47f, MaxInstances = 0, Volume = 0.3f };
                    SoundEngine.PlaySound(style);

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                    SoundEngine.PlaySound(style2);
                }
                if (Timer == 47)
                {
                    upOrDown = !upOrDown;
                    Timer = -1;
                }
                Timer++;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/CyverCannonProj_Glow");
            Main.EntitySpriteDraw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
    }

    public class DarkLaser : ModProjectile
    {
        public Vector2 endPoint;
        public float LaserRotation = (float)Math.PI / 2f;
        int timer = 0;
        int secondTimer = 0;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 50;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (timer == 0)
                LaserRotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Zero;

            additional = Math.Clamp(MathHelper.Lerp(additional, 120 * Projectile.scale, 0.04f), 0, 50 * Projectile.scale);

            timer++;
        }

        float additional = 0f;
        public override bool PreDraw(ref Color lightColor)
         {
            //int sin = (int)(Math.Sin(secondTimer * 0.05) * 40f);
            //var color = new Color(255, 160 + sin, 40 + sin / 2);

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * 2);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.8f);
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                endPoint = LaserRotation.ToRotationVector2() * 2000f;
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/GlowTrailMoreRes").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = (50f * Projectile.scale) - additional; //15
                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(LaserRotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1.2f));

                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);
                Main.spriteBatch.Draw(texBeam, target, null, Color.DeepPink, LaserRotation, origin2, 0, 0);


                for (int i = 0; i < width; i += 6)
                    Lighting.AddLight(pos + Vector2.UnitX.RotatedBy(LaserRotation) * i + Main.screenPosition, Color.DeepPink.ToVector3() * height * 0.020f); //0.030

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                float progress = additional / (50f * Projectile.scale);
                var spotTex = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;
                Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f - progress, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(spotTex, pos, spotTex.Frame(1, 1, 0, 0), Color.DeepPink, Projectile.rotation, spotTex.Size() / 2, 1f - progress, SpriteEffects.None, 0);


                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            }
            secondTimer++;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer > 1) return false;

            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * 1000, 22, ref point);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.NewText(Projectile.scale);
            target.immune[Projectile.owner] = 2; //Collision only lasts for 1 frame so it doesn't matter
            if (crit && Projectile.scale > 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.player[Projectile.owner].Center, 
                        new Vector2(6, 0).RotatedByRandom(6), ModContent.ProjectileType<CannonSplit>(), (int)(Projectile.damage * 0.5), 0, Main.myPlayer);
                }
            }
        }
    }

    public class DarkLaserOld : ModProjectile
    {
        public int i;
        public int counter = 0;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            var entitySource = Projectile.GetSource_FromAI();
            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(base.Projectile.Center - Projectile.velocity / 2f, 0, 0, 242, 0f, 0f, 100, default(Color), 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (Projectile.ai[1] != 1f)
            {
                Projectile.ai[1] = 1f;
                base.Projectile.position += base.Projectile.velocity;
                Projectile.velocity = Projectile.velocity;
            }
            i++;
            if (i % Main.rand.Next(1, 50) == 0)
            {
                Projectile.NewProjectile(entitySource, Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X - 2f, Projectile.velocity.Y - 2, ModContent.ProjectileType<CannonSplit>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
            }
        }
    }

    public class CannonSplit : ModProjectile
    {
        public int i;
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            int count = 0;

            foreach (Projectile proj in Main.projectile.Where(x => x.active && x.whoAmI != Projectile.whoAmI && x.type == Projectile.type))
            {
                count++;

                if (count >= 5)
                    proj.Kill();
            }
            if (Projectile.alpha > 30)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 30)
                {
                    Projectile.alpha = 30;
                }
            }

            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 4000f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (target && timer >= 20)
            {
                AdjustMagnitude(ref move);
                Projectile.velocity = (5 * Projectile.velocity + move) / 6f;
                AdjustMagnitude(ref Projectile.velocity);
            }
            if (Projectile.alpha <= 30 && timer % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 242);
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].noGravity = true;
            }
            timer++;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 3f)
            {
                vector *= 3f / magnitude;
            }
        }
    }
}