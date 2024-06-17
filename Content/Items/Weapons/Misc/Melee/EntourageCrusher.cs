using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles;
using ReLogic.Content;
using Terraria.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Globals.Players;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class EntourageCrusher : ModItem
    {
        bool tick = false;

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.knockBack = 4f; //Slightly below average

            Item.width = 56;
            Item.height = 56;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.shootSpeed = 1f;

            Item.rare = ItemRarities.EarlyHardmode;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<NewEntourageCrusherHeldProj>();
            Item.value = Item.sellPrice(0, 3, 50, 0);

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
           return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class NewEntourageCrusherHeldProj : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;


            Projectile.timeLeft = 10000;
            Projectile.width = Projectile.height = 70; //70
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.extraUpdates = 14;

            //Stops stuff like Fire Gauntlet dust from appearing
            Projectile.noEnchantmentVisuals = true;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            if (justHitTime > -1 * Projectile.extraUpdates) //-5 so the sword continues to swing a bit and won't get stuck on a bunch of npcs
                return false;

            return getProgress(easingProgress) > 0.3f && getProgress(easingProgress) <= 0.8f; //.2 8f

        } 

        bool skillStrike = false;
        bool playedSound = false;
        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int afterImageTimer = 0;

        public override void AI()
        {

            if (timer == 0)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
                previousRotations = new List<float>();

                if (Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits >= Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().hitsForCrit)
                {
                    SkillStrikeUtil.setSkillStrike(Projectile, 1.3f);

                    skillStrike = true;
                }
            }

            SwingHalfAngle = 190; //190
            easingAdditionAmount = (skillStrike ? 0.022f : 0.018f) / Projectile.extraUpdates; //0.018
            offset = 65; //60
            frameToStartSwing = 1 * Projectile.extraUpdates;
            timeAfterEnd = 2 * Projectile.extraUpdates;
            startingProgress = 0.09f;
            progressToKill = 0.98f;

            StandardSwingUpdate();
            StandardHeldProjCode();

            if (justHitTime <= 0 && getProgress(easingProgress) > 0.1f)
                Trail();

            int trailMod = skillStrike ? 7 : 8;
            if (afterImageTimer % trailMod == 0 && justHitTime <= 0)
            {
                previousRotations.Add(Projectile.rotation);

                int trailCount = skillStrike ? 12 : 10;

                if (previousRotations.Count > trailCount) //50
                {
                    previousRotations.RemoveAt(0);
                }
            }

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                String soundLocation = Main.rand.NextBool() ? "AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_L_a" : "AerovelenceMod/Sounds/Effects/GGS/Sword_Sharp_L_b";
                SoundStyle slash = new SoundStyle(soundLocation) with { Pitch = -0.1f, PitchVariance = .2f, Volume = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(slash, Projectile.Center);

                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/demo_charge_hit_world1") with { Volume = .11f, Pitch = .9f, PitchVariance = .25f, MaxInstances = -1 }; 
                SoundEngine.PlaySound(stylea, Projectile.Center);

                playedSound = true;
            }

            int dustMod = (int)Math.Clamp(7f - (2f * (Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1f)), 0f, 8f);
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.8f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(50f, 100f), ModContent.DustType<GlowStrong>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: Color.Purple, 0.2f + Main.rand.NextFloat(-0.05f, 0.05f));
                d.scale *= Projectile.scale;
            }

            justHitVFXPower = Math.Clamp(MathHelper.Lerp(justHitVFXPower, -0.25f, 0.01f), 0f, 1f);

            if (justHitTime <= 0)
                afterImageTimer++;

            //Reset if swing didn't hit anybody
            if (getProgress(easingProgress) > 0.8f && !hasHit)
                Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits = 1;
            
        }

        public void Trail()
        { 
            Vector2 gfxOffset = new Vector2(0, Main.player[Projectile.owner].gfxOffY);

            float width = 0f;
            if (getProgress(easingProgress) <= 0.2f)
                width = getProgress(easingProgress) / 0.2f;
            else if (getProgress(easingProgress) >= 0.8f)
                width = 1f - ((getProgress(easingProgress) - 0.8f) / 0.2f);
            else
                width = 1f;

            if (width < 0.15)
                width = 0;

            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail1.trailColor = Color.Purple * width;
            trail1.trailPointLimit = 800;
            trail1.trailWidth = (int)(50 * width * 1.5f);
            trail1.trailMaxLength = 300;
            trail1.timesToDraw = 2;
            trail1.relativeToPlayer = true;
            trail1.myPlayer = Main.player[Projectile.owner];
            trail1.pinch = true;
            trail1.pinchAmount = 0.95f;

            //trail1.trailTime = (float)Main.timeForVisualEffects;
            trail1.trailRot = Projectile.rotation + MathHelper.PiOver4;


            Vector2 relativePosition = (Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.93f) * 30) - Main.player[Projectile.owner].Center;
            trail1.trailPos = relativePosition - gfxOffset;
            trail1.TrailLogic();

            //Trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value;
            trail2.trailColor = Color.White * width;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = (int)(8);
            trail2.trailMaxLength = 300;
            trail2.timesToDraw = skillStrike ? 2 : 1;
            trail2.relativeToPlayer = true;
            trail2.myPlayer = Main.player[Projectile.owner];
            trail2.pinch = true;
            trail2.pinchAmount = 0.85f;

            trail2.trailTime = (float)Main.timeForVisualEffects;
            trail2.trailRot = Projectile.rotation + MathHelper.PiOver4;

            trail2.trailPos = relativePosition - gfxOffset;
            trail2.TrailLogic();
        }

        public List<float> previousRotations;
        float justHitVFXPower = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            trail1.trailTime = (float)Main.timeForVisualEffects * 0.04f;
            trail2.trailTime = (float)Main.timeForVisualEffects * 0.02f;

            Texture2D JustBladeWhite = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusherBladeWhite");
            Texture2D JustBlade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusherBlade");
            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusher");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(0, Blade.Height);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Blade.Width, Blade.Height);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle); // get position of hand

            //Sprite is 58x72 so -14y to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);

            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);
            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);
            Color AfterImageCol = Color.Lerp(Color.Purple, Color.MediumPurple, justHitVFXPower) with { A = 0 } * 0.5f;


            #region pulse
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f) * 0.3f + 1.3f;
            
            if (skillStrike)
                AfterImageCol *= 1.5f;

            AfterImageCol *= scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                Main.spriteBatch.Draw(JustBlade, armPosition - Main.screenPosition + otherOffset - gfxOffset + (TwoPi * num5).ToRotationVector2() * (4f + offset * 1f), null, AfterImageCol,
                    Projectile.rotation + rotationOffset, origin,
                    Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);
            }
            #endregion

            #region after image
            if (previousRotations != null)
            {
                for (int afterI = 0; afterI < previousRotations.Count; afterI++)
                {
                    float progress = (float)afterI / previousRotations.Count;

                    float size = (Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f));
                    //size *= (0.75f + (progress * 0.25f));

                    Main.spriteBatch.Draw(JustBladeWhite, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, AfterImageCol * progress * intensity * 0.35f, previousRotations[afterI] + rotationOffset, origin, size, effects, 0f);
                }

            }
            #endregion

            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);

            //Just Hit Glow
            Main.spriteBatch.Draw(JustBladeWhite, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.MediumPurple with { A = 0 } * justHitVFXPower, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);

            if (skillStrike)
                Main.spriteBatch.Draw(JustBladeWhite, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.MediumPurple with { A = 0 } * justHitVFXPower, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), effects, 0f);

            if (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.7f)
            {

                Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/CrispStarPMA");
                Color colToUse = Color.Purple;
                colToUse.A = 0;
                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(25f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, colToUse * justHitVFXPower * 2f,
                    (float)Main.timeForVisualEffects * 0.15f, Star.Size() / 2,
                    1f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(25f * (1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)), 0).RotatedBy(currentAngle),
                    null, Color.White with { A = 0 } * justHitVFXPower * 2f,
                    (float)Main.timeForVisualEffects * -0.15f, Star.Size() / 2,
                    0.5f, SpriteEffects.None, 0f);
            }

            trail2.TrailDrawing(Main.spriteBatch);
            trail1.TrailDrawing(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        bool hasHit = false; 
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player pl = Main.player[Projectile.owner];
            
            if (!hasHit)
                pl.GetModPlayer<EntourageCounter>().successiveHits = pl.GetModPlayer<EntourageCounter>().successiveHits + 1;
            hasHit = true;

            Vector2 orthToSwing = (MathHelper.PiOver2 + currentAngle).ToRotationVector2() * (Projectile.ai[0] == 1 ? -1 : 1f);

            justHitVFXPower = 1f;

            //Want less hitpause at higher attack speeds
            justHitTime = (5 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 5f)) * Projectile.extraUpdates; //6

            float currentShakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = currentShakePower > 1 ? Math.Clamp(currentShakePower, 5, 10) : 10;

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_hurt_2") with { Pitch = .25f, PitchVariance = .35f, Volume = 1f, MaxInstances = 1 };
            SoundEngine.PlaySound(style, target.Center);

            SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Pitch = -.22f, PitchVariance = .25f, Volume = 0.5f, MaxInstances = 1 }; 
            SoundEngine.PlaySound(style3, target.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 0; i++)
            {
                Vector2 vel = (target.Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));

                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowSpark>(), vel.SafeNormalize(Vector2.UnitX) * (14f + Main.rand.NextFloat(-2f, 2f)),
                    Color.Purple, Main.rand.NextFloat(0.1f, 0.3f), 0.9f, 0f, dustShader);
                p.fadeIn = 50 + Main.rand.NextFloat(-10, 15);
                p.velocity *= 0.3f;
            }

            for (int i = 0; i < 6 + Main.rand.Next(0, 5) + (skillStrike ? 3 : 0); i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), newColor: Color.Purple, Scale: 0.4f + Main.rand.NextFloat(-0.2f, 0.2f));
                d.velocity = orthToSwing * Main.rand.NextFloat(1f, skillStrike ? 4.5f : 3.5f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-2.05f, 2.05f));

                StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                d.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);

            }

            for (int i = 0; i < 11; i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<RoaParticle>(), newColor: new Color(100, 10, 255), Scale: 0.55f + Main.rand.NextFloat(-0.2f, 0.2f));
                d.velocity = orthToSwing * Main.rand.NextFloat(1f, 5f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-1.05f, 1.05f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAngle.ToRotationVector2() * (90f * Projectile.scale);

            Vector2 newStart = Main.player[Projectile.owner].MountedCenter + currentAngle.ToRotationVector2() * (20f * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), newStart, end, 40f * Projectile.scale, ref collisionPoint);
        }

        public override float getProgress(float x)
        {
            float toReturn = 0f;
            #region easeExpo

            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2.2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - Math.Pow(2.2, (-20 * x) + 10)) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;


            #endregion;
        }

    }
    public class EntourageCounter : ModPlayer
    {
        public int successiveHits = 1;
        public int hitsForCrit = 4;
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
            //successiveHits = 1;
        }

    }
}
