using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Terraria.Audio;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using Terraria.GameContent;
using AerovelenceMod.Common;
using AerovelenceMod.Content.Dusts;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareGun : TranslatableModItem
    {
        public int lockOutTimer;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("CombatFlareGun", "Does not require ammo\n5% summon tag crit chance\nClick again with good timing to fire faster")
            .AddName(Language.Default, "Combat Flare Gun").AddTooltip(Language.Default, "Does not require ammo\n5% summon tag crit chance\nClick again with good timing to fire faster")
            .AddSkillStrike(Language.Default, "Skill Strikes at long range")

            .AddName(Language.Spanish, "Pistola de Bengalas de Combate").AddTooltip(Language.Spanish, "No requiere munición\n5% de probabilidad de crítico con etiqueta de invocador\nHaz clic de nuevo con buen tiempo para disparar más rápido").AddSkillStrike(Language.Spanish, "Golpes de Habilidad a larga distancia")
            .AddName(Language.French, "Pistolet de Détresse de Combat").AddTooltip(Language.French, "Ne nécessite pas de munitions\n5% de chance de coup critique pour les sbires\nCliquez à nouveau avec un bon timing pour tirer plus vite").AddSkillStrike(Language.French, "Les Coups de Compétence se déclenchent à longue portée")
            .AddName(Language.German, "Kampfsignalpistole").AddTooltip(Language.German, "Benötigt keine Munition\n5% kritische Trefferchance für Beschwörer\nKlicke erneut mit gutem Timing, um schneller zu feuern").AddSkillStrike(Language.German, "Fähigkeitsschläge treten auf große Entfernung auf")
            .AddName(Language.Italian, "Pistola Razzo da Combattimento").AddTooltip(Language.Italian, "Non richiede munizioni\n5% di probabilità di critico per tag degli evocatori\nClicca di nuovo con il giusto tempismo per sparare più velocemente").AddSkillStrike(Language.Italian, "I Colpi dell'Abilità si attivano a lunga distanza")
            //.AddName(Language.Polish, "Bojowa Raca").AddTooltip(Language.Polish, "Nie wymaga amunicji\n5% szans na krytyk dla tagów przywołańców\nKliknij ponownie we właściwym momencie, aby strzelać szybciej").AddSkillStrike(Language.Polish, "Ciosy Umiejętności występują na dalekim zasięgu")
            //.AddName(Language.PortugueseBrazil, "Sinalizador de Combate").AddTooltip(Language.PortugueseBrazil, "Não requer munição\n5% de chance de crítico para invocadores\nClique novamente com bom tempo para disparar mais rápido").AddSkillStrike(Language.PortugueseBrazil, "Os Golpes de Habilidade ocorrem a longa distância")
            .AddName(Language.Russian, "Боевой сигнальный пистолет").AddTooltip(Language.Russian, "Не требует боеприпасов\n5% шанс критического удара для призывателей\nНажмите снова с хорошим таймингом, чтобы стрелять быстрее").AddSkillStrike(Language.Russian, "Навык Удара активируется на дальнем расстоянии");
            //.AddName(Language.ChineseTraditional, "戰鬥照明槍").AddTooltip(Language.ChineseTraditional, "不需要彈藥\n5%召喚標籤暴擊率\n適時點擊可更快開火").AddSkillStrike(Language.ChineseTraditional, "技能打擊發生在遠距離")
            //.AddName(Language.ChineseSimplified, "战斗信号枪").AddTooltip(Language.ChineseSimplified, "不需要弹药\n5%召唤标记暴击率\n适时点击可更快开火").AddSkillStrike(Language.ChineseSimplified, "技能打击发生在远距离");
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.knockBack = KnockbackTiers.ExtremelyWeak;

            Item.width = 46;
            Item.height = 28;
            Item.useTime = 70;
            Item.useAnimation = 70;

            Item.UseSound = SoundID.Item110;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarities.MidPHM;

            Item.shoot = ModContent.ProjectileType<FireFlare>();
            Item.shootSpeed = 17f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FlareGun).
                AddIngredient(ItemID.DemoniteBar, 5).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int heldProj = Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<FlareGunRecoilProjectile>(), 0, 0, player.whoAmI);

            if (Main.projectile[heldProj].ModProjectile is FlareGunRecoilProjectile held)
            {
                held.SetProjInfo(
                    GunID: ModContent.ItemType<FlareGun>(),
                    AnimTime: 24,
                    NormalXOffset: 16f,
                    DestXOffset: -1f,
                    YRecoilAmount: 0.35f,
                    HoldOffset: new Vector2(0f, 2f),
                    TipPos: new Vector2(31f, -5f),
                    StarPos: new Vector2(22f, -5f)
                    );

                held.timeToStartFade = 1;
                held.quickFade = true; //Recommended for slower firing guns with large YRecoil
            }

            //Explosion
            int dir = velocity.X > 0 ? 1 : -1;
            Vector2 muzzlePos = position + new Vector2(32f, -3f * dir).RotatedBy(velocity.ToRotation());

            for (int i = 0; i < 11; i++) //16
            {
                Color col1 = Color.Lerp(Color.OrangeRed, Color.Orange, 0.35f);

                float progress = (float)i / 10;
                Color col = Color.Lerp(Color.Brown * 0.5f, col1 with { A = 0 }, progress);

                Dust d = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<MediumSmoke>(), Velocity: Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.35f, 1f) * 1f,
                    newColor: col, Scale: Main.rand.NextFloat(0.9f, 1.5f) * 0.4f);
                d.customData = new MediumSmokeBehavior(Main.rand.Next(4, 18), 0.98f, 0.01f, 0.75f); //12 28

                d.rotation = Main.rand.NextFloat(6.28f);

                d.velocity += velocity.SafeNormalize(Vector2.UnitX) * 0.85f;
            }

            //Light Dust
            Dust softGlow = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.OrangeRed, Scale: 0.1f);

            softGlow.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.9f, sizeChangeSpeed: 0.95f, timeToKill: 10,
                overallAlpha: 0.1f, DrawWhiteCore: true, 1f, 1f);

            for (int i = 0; i < 2 + Main.rand.Next(0, 3); i++)
            {
                Color col1 = Color.Lerp(Color.OrangeRed, Color.Orange, 0.15f);


                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(muzzlePos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: col1, Scale: Main.rand.NextFloat(0.25f, 0.5f) * 1.5f);
                dust.noLight = false;
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                    velToBeginShrink: 10f, fadePower: 0.9f, shouldFadeColor: false);

                dust.velocity += velocity.SafeNormalize(Vector2.UnitX) * 2f;
            }

            return true;
        }
        
    }


    public class FlareGunRecoilProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        //How long should the recoil take
        public int AnimationTime = 25;

        //The item ID of the weapon we are using
        public int gunID = 1;

        //Stats for the recoil
        public float BaseXOffset = 18f;
        public float GoalXOffset = 4f;
        public float yRecoilPower = 0.075f;

        public Vector2 HoldoutOffset = Vector2.Zero; 
        public Vector2 TipPosition = Vector2.Zero; //Muzzle Flash Position
        public Vector2 StarPosition = Vector2.Zero; //Postion of the little star vfx (generally advised to be about 6 less than TipPos)

        public int timeToStartFade = 1;

        //Makes the glow on MuzzleFlash fade faster
        public bool quickFade = false;

        //Whether composite arms should always be at max stretch 
        public bool compositeArmAlwaysFull = false;

        //TODO: Add summary for this function here and in BasicRecoilProjectile
        public void SetProjInfo(int GunID, int AnimTime, float NormalXOffset, float DestXOffset, float YRecoilAmount,
            Vector2 HoldOffset, Vector2 TipPos, Vector2 StarPos)
        {
            gunID = GunID;
            AnimationTime = AnimTime;
            BaseXOffset = NormalXOffset;
            GoalXOffset = DestXOffset;
            yRecoilPower = YRecoilAmount;
            HoldoutOffset = HoldOffset;
            TipPosition = TipPos;
            StarPosition = StarPos;
        }


        //The angle of the gun shot
        float shotAngle = 0f;

        //Which muzzle flash texture to use
        int muzzleFlashFrame = Main.rand.Next(0, 3);

        float pullBackRotOffsetAmount = 0f;
        bool hasDoneClickSound = false;

        int timer = 0;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;

            //Kill proj if player is done with item use
            if (Player.itemAnimation <= 1)
                Projectile.Kill();

            //Do this instead of ^ if you are using reuseDelay...
            //if (Player.itemTime + Player.reuseDelay == 0)
            //    Projectile.active = false;


            if (timer == 0)
            {
                bonusPower = 1f;
                XOffset = BaseXOffset;
            }

            //Only get MousePos if we are the projectile owner
            if (Projectile.owner == Main.myPlayer)
                shotAngle = (Main.MouseWorld - Player.Center).ToRotation();


            GunDirection = shotAngle.ToRotationVector2();
            Player.ChangeDir(GunDirection.X > 0 ? 1 : -1);

            #region XRecoil
            int XAnimTime = AnimationTime;
            float goalX = GoalXOffset; //The furthest we will recoil back
            float baseX = BaseXOffset; //The Normal XOffset of the gun

            //Should add up to 1 (but does not need to)
            Vector2 animRatioX = new Vector2(0.25f, 0.75f);

            float xAnimProgress = (float)(Math.Clamp(timer, 0f, XAnimTime) / XAnimTime);

            //Move Out
            if (xAnimProgress < animRatioX.X)
            {
                float prog = xAnimProgress / animRatioX.X;
                XOffset = MathHelper.Lerp(baseX, goalX, Easings.easeInOutQuad(prog));
            }
            //Move back in
            else
            {
                float prog = (xAnimProgress - animRatioX.X) / animRatioX.Y;
                XOffset = MathHelper.Lerp(goalX, baseX, Easings.easeOutQuad(prog));
            }
            #endregion

            #region YRecoil
            int timeToStartYAnim = 3;
            int YAnimTime = AnimationTime;
            float goalY = yRecoilPower; //The amount of recoil (radians) of the shot
            float baseY = 0f;

            //Should add up to 1 (but does not need to)
            Vector2 animRatioY = new Vector2(0.15f, 0.85f);

            if (timer >= timeToStartYAnim)
            {
                float yAnimProgress = (float)(Math.Clamp((timer - timeToStartYAnim), 0f, YAnimTime) / YAnimTime);

                //RecoilUp
                if (yAnimProgress < animRatioY.X)
                {
                    float prog = yAnimProgress / animRatioY.X;
                    YRecoil = MathHelper.Lerp(baseY, goalY, Easings.easeOutCubic(prog));
                }
                //RecoilDown
                else
                {
                    float prog = (yAnimProgress - animRatioY.X) / animRatioY.Y;
                    YRecoil = MathHelper.Lerp(goalY, baseY, Easings.easeInOutBack(prog, 0f, 1f)); //
                }
            }
            #endregion

            //StandardHeldProjCode
            GunDirection = shotAngle.ToRotationVector2().RotatedBy(YRecoil * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (GunDirection * XOffset);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = shotAngle;

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            #region compositeArms

            float totalProgress = ((float)timer / (float)Player.itemAnimationMax);
            bool doPullClickAnim = (totalProgress >= 0.6f && totalProgress <= 0.8f);


            float armRot = GunDirection.ToRotation() - MathHelper.PiOver2;
            armRot += (-0.35f * pullBackRotOffsetAmount) * Player.direction;

            if (doPullClickAnim)
            {
                float pullProg = Utils.GetLerpValue(0.6f, 0.8f, totalProgress, true);
                pullProg = Easings.easeOutSine(pullProg);

                if (pullProg > 0.75f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, armRot);
                else if (pullProg > 0.5f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armRot);
                else if (pullProg > 0.25f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRot);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                if (!hasDoneClickSound && pullProg > 0.5f)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Menu_Tick") with { Volume = 0.66f, Pitch = -.4f, PitchVariance = .25f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style, Player.Center);
                    hasDoneClickSound = true;
                }


                pullBackRotOffsetAmount = 1f;
                //pullBackRotOffsetAmount = Math.Clamp(MathHelper.Lerp(pullBackRotOffsetAmount, 2f, 0.12f), 0f, 1f);
            }
            else
            {
                float Xprog = Utils.GetLerpValue(goalX, baseX, XOffset, true);

                if (Xprog > 0.75f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
                else if (Xprog > 0.5f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRot);
                else if (Xprog > 0.25f)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armRot);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, armRot);

                pullBackRotOffsetAmount = Math.Clamp(MathHelper.Lerp(pullBackRotOffsetAmount, -0.75f, 0.12f), 0f, 1f);
            }
            #endregion

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = GunDirection.ToRotation();


            if (timer > timeToStartFade)
                muzzleFlashPower = Math.Clamp(MathHelper.Lerp(muzzleFlashPower, -0.5f, 0.15f), 0f, 1f);
            bonusPower *= 0.8f;

            timer++;
        }

        public Vector2 GunDirection = Vector2.Zero;

        float XOffset = 0f;
        float YRecoil = 0f;

        float bonusPower = 0f;
        float muzzleFlashPower = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Item[gunID].Value;

            Player Player = Main.player[Projectile.owner];
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 heldOffset = new Vector2(HoldoutOffset.X, HoldoutOffset.Y * Player.direction).RotatedBy(Projectile.rotation);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY) + heldOffset;

            Color between = Color.Lerp(Color.Orange, Color.OrangeRed, 0.75f);
            Color[] colors = { between, Color.OrangeRed, Color.Orange, Color.White };

            //Muzzle Flash
            #region Muzzle Flash
            Texture2D MuzzleFlash = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/Sprite/MiddleMuzzleFlash").Value;
            Texture2D MuzzleFlashGlow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/Sprite/MiddleMuzzleFlashGlow").Value;

            int frameHeight = MuzzleFlash.Height / 3;
            Rectangle muzzleFlashSourceRect = new Rectangle(0, frameHeight * muzzleFlashFrame, MuzzleFlash.Width, frameHeight);
            Vector2 muzzleFlashOrigin = muzzleFlashSourceRect.Size() / 2f;

            Vector2 muzzleFlashPos = drawPos + new Vector2(TipPosition.X, TipPosition.Y * Player.direction).RotatedBy(Projectile.rotation); //33 -3

            float easedMuzzleFlashAlpha = Easings.easeInSine(muzzleFlashPower);
            float muzzleFlashScale = Projectile.scale * 2f * Easings.easeOutSine(muzzleFlashPower);


            Main.spriteBatch.Draw(MuzzleFlashGlow, muzzleFlashPos + Main.rand.NextVector2Circular(3f, 3f), muzzleFlashSourceRect, colors[0] with { A = 0 } * easedMuzzleFlashAlpha * 0.75f, Projectile.rotation, muzzleFlashOrigin, muzzleFlashScale, mySE, 0f);

            Main.spriteBatch.Draw(MuzzleFlash, muzzleFlashPos, muzzleFlashSourceRect, colors[3] * easedMuzzleFlashAlpha * 1f, Projectile.rotation, muzzleFlashOrigin, muzzleFlashScale, mySE, 0f);

            float overglowAlpha = (1f * bonusPower);

            if (quickFade)
                overglowAlpha = Easings.easeInQuad(overglowAlpha);
            Main.spriteBatch.Draw(MuzzleFlashGlow, muzzleFlashPos, muzzleFlashSourceRect, colors[0] with { A = 0 } * overglowAlpha, Projectile.rotation, muzzleFlashOrigin, 3f * (1f - bonusPower), mySE, 0f);
            #endregion


            //Star on tip of gun
            Texture2D Star = CommonTextures.CrispStarPMA.Value;

            Vector2 starPos = drawPos + new Vector2(StarPosition.X, StarPosition.Y * Player.direction).RotatedBy(Projectile.rotation);

            float starRot = (float)Main.timeForVisualEffects * 0.15f * Player.direction;

            float starAlpha = 0.65f * Easings.easeInSine(bonusPower);

            Main.spriteBatch.Draw(Star, starPos, null, colors[1] with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.4f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, starPos, null, colors[2] with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, starPos, null, Color.White with { A = 0 } * starAlpha, starRot, Star.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            

            //Gun Texture
            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation, Texture.Size() / 2, Projectile.scale, mySE, 0f);

            return false;
        }

    }

}