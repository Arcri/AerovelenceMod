using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using Terraria.DataStructures;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System.Linq;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class MarbleMusket : ModItem 
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.useTime = Item.useAnimation = 35;
            Item.shootSpeed = 16;
            Item.knockBack = 3;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<MarbleBullet>();

            Item.width = 58;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 55, 40);
            Item.rare = ItemRarities.EarlyPHM;

            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            SoundStyle style1 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_0") with { Pitch = .9f, PitchVariance = .25f, MaxInstances = -1, Volume = 0.35f };
            SoundEngine.PlaySound(style1, position);

            SoundStyle style2 = SoundID.Item110 with { Volume = 0.35f, PitchVariance = 0.15f, Pitch = 0.25f };
            SoundEngine.PlaySound(style2, position);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_38") with { Volume = .4f, Pitch = 1f, PitchVariance = 0.1f}; 
            SoundEngine.PlaySound(style, position);

            Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<MarbleMusketHeldProjectile>(), 0, 0, player.whoAmI);

            Dust star = Dust.NewDustPerfect(position + position.SafeNormalize(Vector2.UnitX) * 4f, ModContent.DustType<GlowPixelCross>(),
                velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(2.5f, 3.25f) * 2f, newColor: Color.Gold, Scale: 0.65f);

            star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                            rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.90f, shouldFadeColor: false);


            for (int i = 0; i < 3; i++)
            {
                Dust smoke = Dust.NewDustPerfect(position + velocity.SafeNormalize(Vector2.UnitX) * 15f, ModContent.DustType<HighResSmoke>(),
                    Main.rand.NextVector2CircularEdge(1f, 1f), newColor: Color.Gold, Scale: Main.rand.NextFloat(0.35f, 0.5f));

                smoke.velocity += velocity.SafeNormalize(Vector2.UnitX) * 0.75f;

                smoke.customData = AssignBehavior_HRSBase(5, 20, 0.9f, 0.35f, true, 1f);
            }

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MarbleBullet>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Marble, 25)
                .AddRecipeGroup("AerovelenceMod:GoldOrPlatinum", 5)
                .AddIngredient(ItemID.FlintlockPistol, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }
    public class MarbleMusketHeldProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private bool firstFrame = false;
        public float yRecoilProgress = 0;
        public bool yRecoilDone = false;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;

        private int timer = 0;

        Player owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
        

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];
            KillHeldProjIfPlayerDeadOrStunned(Projectile);

            if (owner.itemTime <= 1)
                Projectile.active = false;

            Projectile.velocity = Vector2.Zero;

            if (Projectile.owner == Main.myPlayer && timer == 0)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);


            if (timer == 2)
            {
                Offset = 5f;
            }
            if (timer > 2)
            {
                float easeProgress = MathHelper.Lerp(0f, 1f, Math.Clamp((timer - 3f) / 20f, 0f, 1f));
                Offset = MathHelper.Lerp(5f, 22f, Easings.easeOutQuart(easeProgress));
            }

            if (timer > 1)
            {
                if (yRecoilDone == false)
                    yRecoilProgress = Math.Clamp(MathHelper.Lerp(yRecoilProgress, 1f, 0.12f), 0, 0.3f);
                else
                    yRecoilProgress = Math.Clamp(MathHelper.Lerp(yRecoilProgress, -0.2f, 0.06f), 0, 0.3f);

                if (yRecoilProgress == 0.3f)
                    yRecoilDone = true;

                if (timer > 3)
                    glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.20f, 0.1f), 0f, 1f);
            }

            

            direction = Angle.ToRotationVector2().RotatedBy(yRecoilProgress * Player.direction * -1f); ;
            Projectile.Center = Player.MountedCenter + (direction * Offset);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();


            timer++;
        }

        private float Offset = 15;
        private float glowIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/MarbleMusket").Value;
            Texture2D TextureGlowLayer = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/MarbleMuskeGlowLayer").Value;

            Texture2D MuzzleFlash = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/WhitePixelMuzzleFlash").Value;
            Texture2D MuzzleFlashGlow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/WhitePixelMuzzleFlashGlow").Value;

            Player Player = Main.player[Projectile.owner];
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);

            Vector2 muzzleFlashPos = drawPos + new Vector2(24f, -1f * Player.direction).RotatedBy(Projectile.rotation);
            Vector2 muzzleFlashOrigin = new Vector2(0f, MuzzleFlash.Height / 2f);
            Main.spriteBatch.Draw(MuzzleFlashGlow, muzzleFlashPos, null, Color.White with { A = 0 } * glowIntensity * 0.5f, Projectile.rotation, muzzleFlashOrigin, Projectile.scale * glowIntensity, mySE, 0f);
            Main.spriteBatch.Draw(MuzzleFlash, muzzleFlashPos, null, Color.White * glowIntensity * 0.75f, Projectile.rotation, muzzleFlashOrigin, Projectile.scale * glowIntensity, mySE, 0f);

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation, Texture.Size() / 2, Projectile.scale, mySE, 0f);
            Main.spriteBatch.Draw(TextureGlowLayer, drawPos, null, Color.White with { A = 0 } * glowIntensity * 1f, Projectile.rotation, TextureGlowLayer.Size() / 2, Projectile.scale, mySE, 0f);

            return false;
        }
    }
    public class MarbleBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 95;
            
            Projectile.extraUpdates = 1;
        }
        
        public override void AI()
        {
            Dust star = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                Vector2.Zero, newColor: Color.Gold, Scale: 0.4f);

            Dust star2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                Vector2.Zero, newColor: Color.LightGoldenrodYellow, Scale: 0.4f);

            //Move the second dust a little forward to cover gaps
            star2.position += Projectile.velocity * 0.5f;

            star.rotation = Main.rand.NextFloat(6.28f);
            star2.rotation = Main.rand.NextFloat(6.28f);
            
            star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.89f, shouldFadeColor: true);
            star2.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.89f, shouldFadeColor: true);

            Projectile.velocity.Y += 0.075f; //0.1f

            Projectile.rotation = Projectile.velocity.ToRotation();

            starRot += Projectile.velocity.X > 0 ? 0.1f : -0.1f;
            starPower = Math.Clamp(MathHelper.Lerp(starPower, -0.1f, 0.05f), 0f, 0.75f);

            Lighting.AddLight(Projectile.Center, Color.DarkGoldenrod.ToVector3() * 0.65f);
        }

        bool hasHitNPCThisFrame = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player p = Main.player[Projectile.owner];
            hasHitNPCThisFrame = true;

            // 3 Hits in a row, spawn the star and do highest pitch sound
            if (p.GetModPlayer<MarbleMusketPlayer>().consecutiveHits == 2)
            {
                //Spawn star
                float rotAmount = Main.rand.NextBool() ? MathHelper.ToRadians(132f) : MathHelper.ToRadians(-130f);
                Vector2 outVec = (target.Center - p.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(132f)) * 7.75f;
                Vector2 outVec2 = (target.Center - p.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(-128f)) * 8f;


                int marstar = Projectile.NewProjectile(Projectile.GetSource_FromAI(), p.Center, outVec, ModContent.ProjectileType<MarbleStar>(), (int)(Projectile.damage * 1.5f), 4, p.whoAmI);
                int marstar2 = Projectile.NewProjectile(Projectile.GetSource_FromAI(), p.Center, outVec2, ModContent.ProjectileType<MarbleStar>(), (int)(Projectile.damage * 1.5f), 4, p.whoAmI);

                //Star dust
                for (int sd = 0; sd < 11; sd++)
                {
                    Color lineCol = Main.rand.NextBool() ? Color.Gold : new Color(255, 180, 0);

                    Dust d = Dust.NewDustPerfect(p.Center, ModContent.DustType<MuraLineBasic>(), 
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.75f, 2f), Alpha: Main.rand.Next(10, 15), lineCol, 0.23f);
                    d.velocity += outVec.SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length() * 0.45f;

                    Dust d2 = Dust.NewDustPerfect(p.Center, ModContent.DustType<MuraLineBasic>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.75f, 2f), Alpha: Main.rand.Next(10, 15), lineCol, 0.23f);
                    d2.velocity += outVec2.SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length() * 0.45f;
                }

                
                
                //Spawn dust
                Vector2 vel2 = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(120)) * 2f;
                Vector2 vel3 = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-120)) * 2f;

                Dust star2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                    vel2, newColor: Color.Goldenrod, Scale: 0.95f);

                star2.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.90f, shouldFadeColor: false);


                Dust star3 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                    vel3, newColor: Color.Goldenrod, Scale: 0.95f);

                star3.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.90f, shouldFadeColor: false);

                //Sound
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .55f, MaxInstances = -1, Volume = 0.45f }; 
                SoundEngine.PlaySound(style, Projectile.Center);
                p.GetModPlayer<MarbleMusketPlayer>().consecutiveHits = 0;
                
            }
            else if (p.GetModPlayer<MarbleMusketPlayer>().consecutiveHits == 1)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_crystal_impact_1") with { Volume = .4f, Pitch = 0.2f, MaxInstances = -1}; 
                SoundEngine.PlaySound(style, Projectile.Center);


                //Dust
                Vector2 vel2 = Projectile.rotation.ToRotationVector2() * -2f;

                Dust star2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                    vel2, newColor: Color.Goldenrod, Scale: 0.85f);

                star2.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.90f, shouldFadeColor: false);

                p.GetModPlayer<MarbleMusketPlayer>().consecutiveHits = 2;
            }
            else
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_crystal_impact_1") with { Pitch = -0.1f, MaxInstances = -1, Volume = 0.4f }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                p.GetModPlayer<MarbleMusketPlayer>().consecutiveHits += 1;
            }

        }

        public float starPower = 1f;
        public float starRot = Main.rand.NextFloat(6.28f);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D line = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Nightglow").Value;
            Texture2D star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Flare").Value;

            Vector2 vec2ScaleLine = new Vector2(0.75f, 1f) * Projectile.scale * 1f;

            Vector2 vec2ScaleStar = new Vector2(0.7f, 0.9f);

            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.Yellow with { A = 0 } * starPower, starRot, star.Size() / 2, vec2ScaleStar, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * starPower, starRot, star.Size() / 2, vec2ScaleStar * 0.5f, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.Yellow with { A = 0 } * starPower, starRot + MathHelper.PiOver2, star.Size() / 2, vec2ScaleStar, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * starPower, starRot + MathHelper.PiOver2, star.Size() / 2, vec2ScaleStar * 0.5f, SpriteEffects.None, 0.0f);


            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.Black with { A = 0 } * 0.4f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 1.2f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.Gold with { A = 0 } * 0.75f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 1.2f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(line, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 1f, Projectile.rotation - MathHelper.PiOver2, line.Size() / 2, vec2ScaleLine * 0.6f, SpriteEffects.None, 0.0f);
            
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Kill();
            return true;
        }
        
        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_40") with { Pitch = -.71f, PitchVariance = .28f, MaxInstances = 1, Volume = 0.5f };
            SoundEngine.PlaySound(style, Projectile.Center);

            
            Dust star = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                Projectile.rotation.ToRotationVector2() * 2f, newColor: Color.Goldenrod, Scale: 0.95f);

            star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                            rotPower: 0.05f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 4f, fadePower: 0.90f, shouldFadeColor: false);

            //Reset counter if we haven't hit anyone
            if (!hasHitNPCThisFrame)
            {
                Main.player[Projectile.owner].GetModPlayer<MarbleMusketPlayer>().consecutiveHits = 0;
            }
        }
    }
    public class MarbleStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        bool firstFrame = true;
        int timer = 0;
        float overallAlpha = 0f; 

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 160;

        }

        public bool randomRotBonus = false;

        public List<float> previousRotations;
        public List<Vector2> previousPostions;
        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();

                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();
            }

            int trailCount = 14;
            previousRotations.Add(Projectile.velocity.ToRotation());
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            //Homing
            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 1000f && (Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1) || Collision.CanHitLine(Main.player[Projectile.owner].Center, 1, 1, n.Center, 1, 1))).OrderBy(n => n.Distance(Projectile.Center)).FirstOrDefault();
            #region Photonic0 homing
            if (target != null && timer > 0 && true)
            {
                float homingStrength = RemapEased((timer - 0f) * 0.25f, 1, 20, 0, .3f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * Projectile.ai[0] * 2f, homingStrength);
                Projectile.velocity *= 1.01f;

            }
            else
            {
                //Kill faster if we dont have a target
                Projectile.timeLeft--;
            }
            #endregion
            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (timer % 2 == 0)
            {
                int dust2 = Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<GlowPixelCross>(), Scale: 0.2f + Main.rand.NextFloat(-0.2f, 0.1f),
                newColor: Main.rand.NextBool() ? Color.Gold : Color.Goldenrod);
                Main.dust[dust2].velocity *= 0.35f;
                Main.dust[dust2].velocity += Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length() * 0.25f;
                Main.dust[dust2].alpha = 5;
                Main.dust[dust2].noLight = false;
            }

            overallAlpha = MathHelper.Clamp(MathHelper.Lerp(overallAlpha, 1.25f, 0.11f), 0f, 1f);

            timer++;
        }

        //Photonic0 function 
        public float RemapEased(float fromValue, float fromMin, float fromMax, float toMin, float toMax, bool clamp = true)
        {
            return MathHelper.Lerp(toMin, toMax, Easings.easeInOutSine(Utils.GetLerpValue(fromMin, fromMax, fromValue, clamp)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/VanillaStar").Value;
            Texture2D StarBlack = Mod.Assets.Request<Texture2D>("Assets/TrailImages/VanillaStarBlackBG").Value;
            Texture2D Line = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Nightglow").Value;

            //Nightglow
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (1f - (progress * 0.5f)) * Projectile.scale;

                    float colVal = progress * overallAlpha;

                    Color col = Color.Lerp(Color.White * 0.75f, Color.Goldenrod, progress) * progress;

                    float size2 = (1f - (progress * 0.15f)) * Projectile.scale;
                    Vector2 vec2Scale = new Vector2(1f, 3f) * size;

                    //Black
                    Main.EntitySpriteDraw(Line, previousPostions[i] - Main.screenPosition, null, Color.Black * 0.25f * (colVal * colVal),
                            previousRotations[i] + MathHelper.PiOver2, Line.Size() / 2f, vec2Scale * size2, SpriteEffects.None);

                    Main.EntitySpriteDraw(StarBlack, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.7f * colVal,
                            Projectile.rotation, StarBlack.Size() / 2f, size2, SpriteEffects.None);

                    Main.EntitySpriteDraw(Line, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 1f * colVal,
                            previousRotations[i] + MathHelper.PiOver2, Line.Size() / 2f, vec2Scale * size2, SpriteEffects.None);

                }

            }

            for (int i = 0; i < 6; i++)
            {
                Color col = new Color(230, 185, 37);
                Main.EntitySpriteDraw(Star, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f), null, col with { A = 0 } * 0.8f * overallAlpha, Projectile.rotation, Star.Size() / 2f, Projectile.scale * 1.1f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Star, drawPos, null, Color.Goldenrod * overallAlpha, Projectile.rotation, Star.Size() / 2f, Projectile.scale * 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(StarBlack, drawPos, null, Color.White with { A = 0 } * 0.35f * overallAlpha, Projectile.rotation, StarBlack.Size() / 2f, Projectile.scale * 1.1f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dustStartPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 3f; 
            for (int i = 0; i < 10; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(5f, 5f);

                Color starCol = Color.Goldenrod;
                Color lineCol = Main.rand.NextBool() ? Color.Gold : new Color(255, 180, 0);

                if (Main.rand.NextBool())
                {
                    Dust star = Dust.NewDustPerfect(dustStartPos, ModContent.DustType<GlowPixelCross>(),
                        vel, newColor: starCol, Scale: 0.3f + Main.rand.NextFloat(0.0f, 0.07f));

                    star.velocity += Projectile.velocity * 0.3f;

                    star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                    rotPower: 0.07f, preSlowPower: 0.95f, timeBeforeSlow: 20, postSlowPower: 0.86f, velToBeginShrink: 3f, fadePower: 0.90f, shouldFadeColor: false);
                }
                else
                {
                    Dust d = Dust.NewDustPerfect(dustStartPos, ModContent.DustType<MuraLineBasic>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.75f, 2f), Alpha: Main.rand.Next(10, 15), lineCol, 0.23f);
                    d.velocity += Projectile.velocity * 0.35f;
                }

            }

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Pitch = 0.35f, PitchVariance = .12f, Volume = 0.3f, MaxInstances = -1 };
            SoundEngine.PlaySound(style2, Projectile.Center);


            //Dust on Trail
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i += 1)
                {
                    if (i % 3 == 0)
                    {
                        int dust2 = Dust.NewDust(previousPostions[i], 1, 1, ModContent.DustType<GlowPixelCross>(), Scale: 0.18f + Main.rand.NextFloat(-0.05f, 0.05f),
                            newColor: Main.rand.NextBool() ? Color.Gold : Color.Goldenrod);
                        Main.dust[dust2].velocity *= 0.5f;
                        Main.dust[dust2].velocity += previousRotations[i].ToRotationVector2() * Projectile.velocity.Length() * 0.25f;
                        Main.dust[dust2].alpha = 5;
                        Main.dust[dust2].noLight = false;
                    }
                }

            }

        }
    }
    public class MarbleMusketPlayer : ModPlayer
    {
        public int consecutiveHits = 0;

        public override void ResetEffects()
        {
            ResetVariables();
        }
        public override void UpdateDead()
        {
            ResetVariables();
        }
        private void ResetVariables()
        {
            //consecutiveHits = 0;
        }
    }
}