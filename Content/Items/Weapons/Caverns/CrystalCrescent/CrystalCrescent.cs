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
using Mono.Cecil;
using static System.Net.Mime.MediaTypeNames;
using AerovelenceMod.Common.Systems.Language;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Caverns.CrystalCrescent
{
    //MOSTLY DONE
    public class CrystalCrescent : TranslatableModItem
    {
        bool tick = false;
        public static int attackCount = 0;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Crystal Crescent", "Throws out a returning quarterstaff after a rapid swing")
            .AddName(Language.Default, "Crystal Crescent")
            .AddTooltip(Language.Default, "Throws out a returning quarterstaff after a rapid swing")
            .AddSkillStrike(Language.Default, "Skill Strikes while in hand")

            .AddName(Language.Spanish, "Creciente de Cristal").AddTooltip(Language.Spanish, "Lanza una vara larga que regresa después de un golpe rápido").AddSkillStrike(Language.Spanish, "Golpes de Habilidad mientras está en mano")
            .AddName(Language.French, "Croissant de Cristal").AddTooltip(Language.French, "Lance un bâton de combat revenant après une frappe rapide").AddSkillStrike(Language.French, "Les Coups de Compétence se déclenchent tant que tenu en main")
            .AddName(Language.German, "Kristallhalbmond").AddTooltip(Language.German, "Wirft einen zurückkehrenden Kampfstab nach einem schnellen Schwung").AddSkillStrike(Language.German, "ähigkeitsschläge werden aktiviert, solange die Waffe in der Hand gehalten wird")
            .AddName(Language.Italian, "Crescente di Cristallo").AddTooltip(Language.Italian, "Lancia un bastone da combattimento che ritorna dopo un colpo rapido").AddSkillStrike(Language.Italian, "I Colpi dell'Abilità si attivano mentre è in mano")
            //.AddName(Language.Polish, "Kryształowy Półksiężyc").AddTooltip(Language.Polish, "Wyrzuca powracający kostur bojowy po szybkim zamachu").AddSkillStrike(Language.Polish, "Ciosy Umiejętności występują, gdy broń jest w dłoni")
            //.AddName(Language.PortugueseBrazil, "Meia-Lua de Cristal").AddTooltip(Language.PortugueseBrazil, "Arremessa um bastão de combate que retorna após um golpe rápido").AddSkillStrike(Language.PortugueseBrazil, "Os Golpes de Habilidade ocorrem enquanto estiver em mão")
            .AddName(Language.Russian, "Кристальный Полумесяц").AddTooltip(Language.Russian, "Бросает возвращающийся боевой посох после быстрого взмаха").AddSkillStrike(Language.Russian, "Навык Удара активируется, пока оружие в руке");
            //.AddName(Language.ChineseTraditional, "水晶新月").AddTooltip(Language.ChineseTraditional, "快速揮動後投擲一根回旋的長棍").AddSkillStrike(Language.ChineseTraditional, "技能打擊發生在持有時")
            //.AddName(Language.ChineseSimplified, "水晶新月").AddTooltip(Language.ChineseSimplified, "快速挥动后投掷一根回旋的长棍").AddSkillStrike(Language.ChineseSimplified, "技能打击发生在持有时");
        }

        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 12;
            Item.useAnimation = 20;
            Item.useTime = 20;
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

        public override bool CanUseItem(Player player)
        {
            int count = player.ownedProjectileCounts[ModContent.ProjectileType<CrystalCrescentThrowProj>()];
            return count < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackCount++;
            int p;
            tick = !tick;
            p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, tick ? 1 : 0);
            return false;
        }

    }

    public class CrystalCrescentThrowProj : ModProjectile
    {
        private float VelocityMult = 24;
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

                lightningData.CoreColorOverride = Color.White;
                lightningData.MidColorOverride = Color.SteelBlue;
                lightningData.OuterColorOverride = Color.MidnightBlue;
                lightningData.FlashColorOverride = Color.Black;
                lightningData.DistColorOverride = Color.White;

                lightningData.GlowIntensity = 1f;
                lightningData.GlowScale = 0.15f;
            }

            LightningUtils.InitializeBetweenPoints(lightningData, Projectile.Center, player.Center);
            LightningUtils.UpdateSegments(lightningData);
            LightningUtils.UpdateBranches(lightningData);
            LightningUtils.SpawnDust(lightningData);

            if (Projectile.ai[0] == 0f)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
                Projectile.velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld)) + player.velocity / VelocityMult / 2;
                initialVelocity = Projectile.velocity;

                //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Pitch = -0.3f, PitchVariance = .4f, Volume = 0.20f };
                //SoundEngine.PlaySound(style, Projectile.Center);
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
            SoundEngine.PlaySound(SoundID.NPCHit53 with { Volume = 0.35f, Pitch = 0.3f, PitchVariance = 0.4f});
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.25f, Pitch = -0.15f, PitchVariance = 0.4f});

            float currentShakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = currentShakePower > 1 ? Math.Clamp(currentShakePower, 3, 8) : 8;

            for (int i = 0; i < 3 + Main.rand.Next(0, 2); i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), newColor: Color.MidnightBlue, Scale: 0.4f + Main.rand.NextFloat(-0.2f, 0.2f));
                d.velocity = Projectile.velocity * Main.rand.NextFloat(1f, 3.5f) / 4;
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-2.05f, 2.05f));

                StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                d.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);

            }

            for (int i = 0; i < 4; i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<RoaParticle>(), newColor: Color.SteelBlue, Scale: 0.55f + Main.rand.NextFloat(-0.2f, 0.2f));
                d.velocity = Projectile.velocity * Main.rand.NextFloat(1f, 5f) / 4;
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-1.05f, 1.05f));
            }
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
        bool tick;
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
                //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.62f, PitchVariance = .3f, Volume = 0.20f };
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Pitch = -0.4f, PitchVariance = .4f, Volume = 0.20f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            if (Projectile.ai[0] == 1)
            {
                tick = true;
            }
            else
            {
                tick = false;
            }

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

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CrystalCrescentThrowProj>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 0, tick ? -1 : 1);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            relativeTrail.TrailDrawing(Main.spriteBatch);
            counterrelativeTrail.TrailDrawing(Main.spriteBatch);

            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/CrystalCrescent/CrystalCrescent");

            Vector2 origin = new Vector2(Blade.Width / 2, Blade.Height / 2);
            float rotationOffset = 0;

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);

            //Sprite is 64x64 so -0 to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + intensity * 0.5f, SpriteEffects.None, 0f);

            return false;
        }

        public override float getProgress(float x)
        {
            return Easings.easeInOutExpo(x);
        }
    }

}