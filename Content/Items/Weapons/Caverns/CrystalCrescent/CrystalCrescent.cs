using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles;
using System;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Caverns.CrystalCrescent
{
    //MOSTLY DONE
    public class CrystalCrescent : ModItem
    {
        bool tick = false;
        public static int attackCount = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Example Swing Sword");
            // Tooltip.SetDefault("Debug/Example Item");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;

            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<CrystalCrescentSwingProj>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes while in hand [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<CrystalCrescentThrowProj>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackCount++;
            int p;

            if (attackCount % 2 == 0)
            {
                p = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalCrescentThrowProj>(), damage, knockback, player.whoAmI, 0, tick ? -1 : 1);
            }
            else
            {
                tick = !tick;
                p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, tick ? 1 : 0);
            }


            return false;
        }

    }

    public class CrystalCrescentThrowProj : ModProjectile
    {
        private float VelocityMult = 18;
        private float ReboundTicks = 60;

        public override string Texture => "Terraria/Images/Projectile_0";

        //public override string Texture => "AerovelenceMod/Content/Items/Weapons/Caverns/CrystalCrescent/CrystalCrescent";

        BaseTrailInfo relativeTrail = new BaseTrailInfo();
        BaseTrailInfo counterrelativeTrail = new BaseTrailInfo();

        private LightningUtils.LightningData lightningData;

        private Vector2 initialVelocity;
        private Vector2 returnVelocity;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 78;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.light = 0.75f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region Trails

            relativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value;
            relativeTrail.trailColor = Color.MidnightBlue;
            relativeTrail.trailPointLimit = 75;
            relativeTrail.trailWidth = 30;
            relativeTrail.trailMaxLength = 100;
            relativeTrail.timesToDraw = 3;
            relativeTrail.relativeToPlayer = true;
            relativeTrail.myPlayer = Main.player[Projectile.owner];
            relativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            relativeTrail.trailPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-1f) * (60) - Main.player[Projectile.owner].Center;

            counterrelativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value;
            counterrelativeTrail.trailColor = Color.SteelBlue;
            counterrelativeTrail.trailPointLimit = 75;
            counterrelativeTrail.trailWidth = 30;
            counterrelativeTrail.trailMaxLength = 100;
            counterrelativeTrail.timesToDraw = 3;
            counterrelativeTrail.relativeToPlayer = true;
            counterrelativeTrail.myPlayer = Main.player[Projectile.owner];
            counterrelativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            counterrelativeTrail.trailPos = (Projectile.Center - Projectile.rotation.ToRotationVector2().RotatedBy(-1f) * (60) - Main.player[Projectile.owner].Center);

            relativeTrail.TrailLogic();
            counterrelativeTrail.TrailLogic();
            #endregion

            if (lightningData == null || !lightningData.Initialized)
            {
                lightningData = new LightningUtils.LightningData(Projectile, LightningUtils.LightningStyle.Default);
                lightningData.NoiseFrequency = 3f;
                lightningData.CoreColorOverride = Color.Indigo;
                lightningData.MidColorOverride = Color.Purple;
                lightningData.OuterColorOverride = Color.Pink;
                lightningData.FlashColorOverride = Color.Yellow;

                lightningData.GlowIntensity = 0.6f;
            }

            LightningUtils.InitializeBetweenPoints(lightningData, Projectile.Center, player.Center);
            LightningUtils.UpdateSegments(lightningData);
            LightningUtils.UpdateBranches(lightningData);
            LightningUtils.SpawnDust(lightningData);

            if (Projectile.ai[0] == 0f)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
                Projectile.velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld)) + player.velocity / VelocityMult;
                initialVelocity = Projectile.velocity;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Pitch = -0.3f, PitchVariance = .4f, Volume = 0.20f };
                SoundEngine.PlaySound(style, Projectile.Center);
            }

            Projectile.rotation += 0.3f * Projectile.ai[1];

            returnVelocity = Vector2.Normalize(Projectile.Center.DirectionTo(player.position));

            if (Projectile.ai[0] >= ReboundTicks)
            {
                Projectile.velocity = returnVelocity * VelocityMult;
            }
            else
            {
                Projectile.velocity = (initialVelocity * ((ReboundTicks - Projectile.ai[0]) / ReboundTicks) + returnVelocity * (Projectile.ai[0] / ReboundTicks)) * VelocityMult;

            }

            if (Vector2.Distance(Projectile.Center, player.Center) < 2 * 16 && Projectile.ai[0] > ReboundTicks / 2)
            {
                Projectile.active = false;
                return;
            }

            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Main.player[Projectile.owner].whoAmI] = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (lightningData != null && lightningData.Initialized)
            {
                LightningUtils.DrawLightning(lightningData, Main.spriteBatch);
            }

            relativeTrail.TrailDrawing(Main.spriteBatch);
            counterrelativeTrail.TrailDrawing(Main.spriteBatch);

            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/CrystalCrescent/CrystalCrescent");

            Vector2 origin = new Vector2(Blade.Width / 2, Blade.Height / 2);
            float rotationOffset = 0;

            Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class CrystalCrescentSwingProj : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 7;
            Projectile.light = 0.75f;
        }

        bool playedSound = false;
        BaseTrailInfo relativeTrail = new BaseTrailInfo();
        BaseTrailInfo counterrelativeTrail = new BaseTrailInfo();

        public override void AI()
        {
            SwingHalfAngle = 190; // 190
            easingAdditionAmount = 0.02f / Projectile.extraUpdates; //0.015f
            offset = 50;
            frameToStartSwing = 0;
            timeAfterEnd = 0;
            startingProgress = 0.1f;
            progressToKill = 0.9f;

            StandardSwingUpdate();
            StandardHeldProjCode();

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.62f, PitchVariance = .3f, Volume = 0.20f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);


            //Trail
            relativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value;
            relativeTrail.trailColor = Color.MidnightBlue;
            relativeTrail.trailPointLimit = 75;
            relativeTrail.trailWidth = 20;
            relativeTrail.trailMaxLength = 150;
            relativeTrail.timesToDraw = 3;
            relativeTrail.relativeToPlayer = true;
            relativeTrail.myPlayer = Main.player[Projectile.owner];
            relativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            relativeTrail.trailPos = Projectile.Center / 2 + Projectile.rotation.ToRotationVector2().RotatedBy(-1f) * (60 + intensity * 30) / 2 - Main.player[Projectile.owner].Center / 2;

            counterrelativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightningBloom").Value;
            counterrelativeTrail.trailColor = Color.SteelBlue;
            counterrelativeTrail.trailPointLimit = 75;
            counterrelativeTrail.trailWidth = 20;
            counterrelativeTrail.trailMaxLength = 150;
            counterrelativeTrail.timesToDraw = 3;
            counterrelativeTrail.relativeToPlayer = true;
            counterrelativeTrail.myPlayer = Main.player[Projectile.owner];
            counterrelativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            counterrelativeTrail.trailPos = (Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-1f) * (60 + intensity * 30) - Main.player[Projectile.owner].Center) / -2;

            if (getProgress(easingProgress) >= 0.03f)
            {
                relativeTrail.TrailLogic();
                counterrelativeTrail.TrailLogic();
            }

            SkillStrikeUtil.setSkillStrike(Projectile, 2f, 10000, 1f, 0f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Main.player[Projectile.owner].whoAmI] = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            relativeTrail.TrailDrawing(Main.spriteBatch);
            counterrelativeTrail.TrailDrawing(Main.spriteBatch);

            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/CrystalCrescent/CrystalCrescent");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(Blade.Width / 2, Blade.Height / 2);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Blade.Width / 2, Blade.Height / 2);
                //rotationOffset = MathHelper.ToRadians(90f);
                //effects = SpriteEffects.FlipHorizontally;
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);

            //Sprite is 64x64 so -0 to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + intensity * 0.5f, effects, 0f);

            return false;
        }

        public override float getProgress(float x)
        {
            return Easings.easeInOutExpo(x);
        }
    }

}