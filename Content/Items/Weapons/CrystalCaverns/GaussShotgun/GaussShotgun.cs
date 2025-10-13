using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using static Terraria.NPC;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;
using Terraria.GameContent;
using AerovelenceMod.Content.Gores;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.GaussShotgun
{
    public class GaussShotgun : ModItem
    {
        private int shotCounter = 0;
        
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = KnockbackTiers.Weak;
            Item.width = 58;
            Item.height = 22;

            Item.useAnimation = 60;
            Item.useTime = 20;
            Item.shootSpeed = 7f;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarities.EarlyHardmode;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.value = Item.buyPrice(0, 20, 0, 0);

            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            float XDest = shotCounter == 2 ? 0f : 6f;
            float recoilPower = shotCounter == 2 ? 0.25f : 0.15f;

            switch (shotCounter)
            {
                //Triple Shot
                case 0:
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(0.1f), type, damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.1f), type, damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item38 with { Volume = 0.6f }, player.Center);

                    shotCounter++;
                    break;
                //Double Shot
                case 1:
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(0.05f), type, damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.05f), type, damage, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item38 with { Volume = 0.6f }, player.Center);

                    shotCounter++;
                    break;
                //Star
                case 2:
                    Projectile.NewProjectile(source, position, velocity * 1.4f, ModContent.ProjectileType<GaussianStar>(), damage * 2, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.6f }, player.Center);

                    Color col = Color.DeepSkyBlue;
                    for (int i = 0; i < 7 + Main.rand.Next(0, 4); i++) //2 //0,3
                    {
                        Dust dp = Dust.NewDustPerfect(position, ModContent.DustType<ElectricSparkGlow>(),
                            velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.Next(3, 20),
                            newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 1.75f);

                        ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.91f, FadeScalePower: 0.97f, FadeVelPower: 0.9f, Pixelize: true, XScale: 1f, YScale: 0.75f); //0.91

                        dp.customData = esb;
                    }

                    shotCounter = 0;
                    break;

            }

            //Bullet Casing
            Gore.NewGore(source, position + velocity, new Vector2(velocity.X * -0.25f, -0.75f), ModContent.GoreType<BlueCasing>());

            //Shot Dust
            #region dust
            int dir = velocity.X > 0 ? 1 : -1;
            Vector2 muzzlePos = position + new Vector2(18f, -3f * dir).RotatedBy(velocity.ToRotation());

            for (int i = 0; i < 7; i++) //16
            {
                Color col1 = Color.Lerp(Color.SkyBlue, Color.LightSkyBlue, 0f);

                float progress = (float)i / 6;
                Color col = Color.Lerp(Color.DodgerBlue * 0.5f, col1 with { A = 0 }, progress);

                Dust d = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<MediumSmoke>(), Velocity: Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.35f, 1f) * 1f,
                    newColor: col, Scale: Main.rand.NextFloat(0.9f, 1.5f) * 0.4f);
                d.customData = new MediumSmokeBehavior(Main.rand.Next(4, 18), 0.98f, 0.01f, 0.75f); //12 28

                d.rotation = Main.rand.NextFloat(6.28f);

                d.velocity += velocity.SafeNormalize(Vector2.UnitX) * 0.85f;
            }

            Dust softGlow = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: 0.1f);

            softGlow.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.9f, sizeChangeSpeed: 0.95f, timeToKill: 10,
                overallAlpha: 0.1f, DrawWhiteCore: true, 1f, 1f);

            for (int i = 0; i < 2 + Main.rand.Next(0, 3); i++)
            {

                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.25f, 0.5f) * 1.5f);
                dust.noLight = false;
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                    velToBeginShrink: 10f, fadePower: 0.9f, shouldFadeColor: false);

                dust.velocity += velocity.SafeNormalize(Vector2.UnitX) * 2f;
            }
            #endregion


            if (player.itemAnimation == (Item.useAnimation / 3) - 1)
            {
                shotCounter = 0; //Makes it so that in rare scenarious where the shot counter is misaligned, it will fix itself on the next shot 
            }

            //Kill the current held proj if it exists
            foreach (Projectile p in Main.projectile)
            {
                if (p.active)
                    if (p.type == ModContent.ProjectileType<GaussShotgunRecoil>())
                        if (p.owner == player.whoAmI)
                            p.active = false;
            }
            
            int gun = Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<GaussShotgunRecoil>(), 0, 0, player.whoAmI);
            if (Main.projectile[gun].ModProjectile is GaussShotgunRecoil held)
            {
                held.SetProjInfo(
                    GunID: ModContent.ItemType<GaussShotgun>(),
                    AnimTime: 16,
                    NormalXOffset: 18f,
                    DestXOffset: XDest,
                    YRecoilAmount: recoilPower,
                    HoldOffset: new Vector2(0f, 0f),
                    TipPos: new Vector2(34f, 0f),
                    StarPos: new Vector2(28f, 0f)
                    );

                held.timeToStartFade = 0;
            }

            return false;
        }

        public override bool OnPickup(Player player) //Another check for the problem mentioned above
        {
            shotCounter = 0;
            return base.OnPickup(player);
        }
    }

    public class GaussShotgunRecoil : BasicRecoilProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Item[gunID].Value;

            Player Player = Main.player[Projectile.owner];
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 heldOffset = new Vector2(HoldoutOffset.X, HoldoutOffset.Y * Player.direction).RotatedBy(Projectile.rotation);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY) + heldOffset;

            Color between = Color.Lerp(Color.Orange, Color.OrangeRed, 0.75f);
            Color[] colors = { between, Color.OrangeRed, Color.Orange, Color.White }; //between OrangeRed Orange White


            //Muzzle Flash
            Texture2D MuzzleFlash = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/Sprite/MiddleMuzzleFlashGray").Value;
            Texture2D MuzzleFlashGlow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/Sprite/MiddleMuzzleFlashGrayGlow").Value;

            int frameHeight = MuzzleFlash.Height / 3;
            Rectangle muzzleFlashSourceRect = new Rectangle(0, frameHeight * muzzleFlashNum, MuzzleFlash.Width, frameHeight);
            Vector2 muzzleFlashOrigin = muzzleFlashSourceRect.Size() / 2f;

            Vector2 muzzleFlashPos = drawPos + new Vector2(TipPosition.X, TipPosition.Y * Player.direction).RotatedBy(Projectile.rotation); //33 -3

            float easedMuzzleFlashAlpha = Easings.easeInSine(muzzleFlashPower);
            float muzzleFlashScale = Projectile.scale * 2f * Easings.easeOutSine(muzzleFlashPower);


            Main.spriteBatch.Draw(MuzzleFlashGlow, muzzleFlashPos + Main.rand.NextVector2Circular(3f, 3f), muzzleFlashSourceRect, Color.DeepSkyBlue with { A = 0 } * easedMuzzleFlashAlpha * 0.75f, Projectile.rotation, muzzleFlashOrigin, muzzleFlashScale, mySE, 0f);

            Main.spriteBatch.Draw(MuzzleFlash, muzzleFlashPos, muzzleFlashSourceRect, Color.White * easedMuzzleFlashAlpha * 0.35f, Projectile.rotation, muzzleFlashOrigin, muzzleFlashScale, mySE, 0f);

            float overglowAlpha = Easings.easeInQuad(bonusPower);

            Main.spriteBatch.Draw(MuzzleFlashGlow, muzzleFlashPos, muzzleFlashSourceRect, Color.DodgerBlue with { A = 0 } * overglowAlpha, Projectile.rotation, muzzleFlashOrigin, 3f * (1f - bonusPower), mySE, 0f);



            //Star on tip of gun
            Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA");

            Vector2 starPos = drawPos + new Vector2(StarPosition.X, StarPosition.Y * Player.direction).RotatedBy(Projectile.rotation);

            float starRot = (float)Main.timeForVisualEffects * 0.15f * Player.direction;

            float starAlpha = 0.65f * Easings.easeInSine(bonusPower);

            Main.spriteBatch.Draw(Star, starPos, null, Color.DeepSkyBlue with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.4f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, starPos, null, Color.SkyBlue with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, starPos, null, Color.White with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            
            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation, Texture.Size() / 2, Projectile.scale, mySE, 0f);

            //Glowmask
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/CrystalCaverns/GaussShotgun/GaussShotgunGlowmask").Value;
            Main.spriteBatch.Draw(Glowmask, drawPos, null, Color.White, Projectile.rotation, Glowmask.Size() / 2, Projectile.scale, mySE, 0f);

            //Glowlayer
            Texture2D Glowlayer = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/CrystalCaverns/GaussShotgun/GaussShotgunGlow").Value;
            Main.spriteBatch.Draw(Glowlayer, drawPos, null, Color.White with { A = 0 } * Easings.easeOutCubic(bonusPower), Projectile.rotation, Glowlayer.Size() / 2, Projectile.scale, mySE, 0f);


            return false;
        }
    }

    public class GaussianStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        int timer = 0;
        public override void AI()
        {
            int trailCount = 18;
            previousRotations.Add(Projectile.velocity.ToRotation());
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            if (timer > 15)
            {
                Vector2 move = Vector2.Zero;
                float distance = 350f;
                bool target = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && Main.npc[i].type != NPCID.TargetDummy)
                    {
                        Vector2 newMove = Main.npc[i].Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    float velLength = Projectile.velocity.Length();
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, move.SafeNormalize(Vector2.UnitX) * velLength, 0.1f);
                    Projectile.velocity += move.SafeNormalize(Vector2.UnitX) * 1f;
                }
            }

            if (Projectile.velocity.X > 0)
                Projectile.rotation += 0.25f;
            else
                Projectile.rotation -= 0.25f;


            //Dust
            if (timer % 2 == 0 && Main.rand.NextBool() && timer > 5)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(1.5f, 1.5f);

                Dust da = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkGlow>(), dustVel, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.4f, 0.6f) * 1.5f);
                da.velocity += Projectile.velocity.RotatedByRandom(0.25f) * -0.4f;

                ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.93f, FadeScalePower: 0.98f, FadeVelPower: 0.9f, Pixelize: true, XScale: 1f, YScale: 0.45f);
                esb.randomVelRotatePower = 0.15f;
                da.customData = esb;

            }

            float fadeInTime = Math.Clamp((timer + 3f) / 14f, 0f, 1f);
            overallScale = Easings.easeInOutHarsh(fadeInTime);

            timer++;
        }

        public override bool PreKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GaussExplosionVFX>(), 0, 0, Projectile.owner);

            Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: 0.2f);

            d.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.9f, sizeChangeSpeed: 0.95f, timeToKill: 20,
                overallAlpha: 0.15f, DrawWhiteCore: true, 1f, 1f); ;

            //Hit all enemies in a radius
            GeneralUtils.strikeNPCsInRadius(Projectile.Center, 85f, Projectile.damage * 0.5f, Projectile.knockBack * 0.5f);

            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);

            for (int i = 0; i < 13 + Main.rand.Next(0, 6); i++) //2 //0,3
            {
                Vector2 vel = Main.rand.NextVector2Circular(5f, 5f) * 2f;

                Dust dp = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkGlow>(), vel, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 1.5f);

                ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.98f, FadeVelPower: 0.92f, Pixelize: true, XScale: 1f, YScale: 0.75f); //0.91

                if (i < 8)
                    esb.randomVelRotatePower = 1f; //1f
                dp.customData = esb;
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2.5f, 7f);
                Color middleBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f + Main.rand.NextFloat(-0.15f, 0.15f));

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: middleBlue, Scale: Main.rand.NextFloat(0.15f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
            }

            Color between = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.15f);
            Dust d11 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<FeatheredGlowDust>(), Velocity: Vector2.Zero, newColor: between, Scale: 0.75f);

            FeatheredGlowBehavior fgb = new FeatheredGlowBehavior(AlphaChangeSpeed: 0.65f, timeToChangeAlpha: 6, ScaleChangeSpeed: 1.1f, timeToKill: 120, OverallAlpha: 0.5f);
            fgb.DrawWhiteCore = true;
            d11.customData = fgb;

            return base.PreKill(timeLeft);
        }

        float overallAlpha = 1f;
        float overallScale = 1.5f;
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPostions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Core = CommonTextures.Twinkle.Value;
            Texture2D Core2 = CommonTextures.CrispStarPMA.Value;
            Texture2D Orb = CommonTextures.SoftGlow.Value;

            Color thisCol = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.75f) * overallAlpha;

            Vector2 vec2Sccale = new Vector2(1.2f, 0.75f) * Projectile.scale * 1.5f * overallScale;
            Vector2 vec2Sccale2 = new Vector2(1.2f, 0.5f) * Projectile.scale * 1.5f * overallScale;

            float mainScale = Projectile.scale * 0.85f * overallScale;

            //AfterImage
            for (int i = 0; i < previousPostions.Count; i++)
            {
                float progress = (float)i / previousPostions.Count;

                Vector2 thisScale = vec2Sccale * Easings.easeInOutSine(progress);
                Vector2 thisScale2 = vec2Sccale2 * Easings.easeInOutSine(progress);

                Vector2 drawPos = previousPostions[i] - Main.screenPosition;
                Color col = thisCol with { A = 0 } * 0.85f * Easings.easeInSine(progress);

                float prevRot = previousRotations[i];

                Main.spriteBatch.Draw(Core, drawPos, null, col, prevRot, Core.Size() / 2, thisScale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(Core, drawPos, null, col, prevRot, Core.Size() / 2, thisScale2, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, null, thisCol with { A = 0 } * 0.06f, Projectile.rotation, Orb.Size() / 2, mainScale * 0.22f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Orb, Projectile.Center - Main.screenPosition, null, thisCol with { A = 0 } * 0.13f, Projectile.rotation, Orb.Size() / 2, mainScale * 0.18f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Core2, Projectile.Center - Main.screenPosition, null, thisCol with { A = 0 }, Projectile.rotation, Core2.Size() / 2, mainScale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Core2, Projectile.Center - Main.screenPosition, null, thisCol with { A = 0 }, Projectile.rotation, Core2.Size() / 2, mainScale * 0.75f, SpriteEffects.None, 0f);

            return false;

        }
    }

    public class GaussExplosionVFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;

            Projectile.timeLeft = 800;
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            int timeForPulse = 30;
            if (timer <= timeForPulse)
                overallScale = MathHelper.Lerp(0f, 1f, Easings.easeOutQuint((float)timer / (float)timeForPulse));

            if (timer >= 0)
            {
                if (timer >= (timeForPulse * 0.15f))
                    overallAlpha -= 0.055f;

                if (overallAlpha <= 0)
                    Projectile.active = false;
            }

            timer++;
        }

        float overallScale = 0f;
        float overallAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawShader(false);
            });
            DrawShader(true);

            return false;
        }

        public void DrawShader(bool giveUp = false)
        {
            if (giveUp)
                return;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/NewRadialScroll", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/WaterEnergyNoise").Value); //foam_mask_bloom
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/sparkNoiseloop").Value);
            myEffect.Parameters["flowSpeed"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);

            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["colorIntensity"].SetValue(2f * overallAlpha);

            Texture2D Tex2 = Mod.Assets.Request<Texture2D>("Assets/Orbs/circle_05").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float scale = overallScale * Projectile.scale * 0.65f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.EffectMatrix);

            Main.spriteBatch.Draw(Tex2, drawPos, null, Color.White, Projectile.rotation, Tex2.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex2, drawPos, null, Color.White, Projectile.rotation, Tex2.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}