using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.GaussShotgun;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Launchers;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.AdamantitePulsar
{
    public class AdamantitePulsar : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TitaniumRocketLauncher>();
            this.ModifyLocalization("AdamantitePulsar", "Does not require ammo\nRight-Click to change modes")
            .AddName(Language.Default, "Adamantite Pulsar")
            .AddTooltip(Language.Default, "Does not require ammo\nRight-Click to change modes")

            .AddName(Language.Spanish, "Pulsar de Adamantita").AddTooltip(Language.Spanish, "No requiere munición\nHaz clic derecho para cambiar de modo")
            .AddName(Language.French, "Pulsar en Adamantite").AddTooltip(Language.French, "Ne nécessite pas de munitions\nClic droit pour changer de mode")
            .AddName(Language.German, "Adamantit-Pulsar").AddTooltip(Language.German, "Benötigt keine Munition\nRechtsklick, um den Modus zu wechseln")
            .AddName(Language.Italian, "Pulsar di Adamantite").AddTooltip(Language.Italian, "Non richiede munizioni\nTasto destro per cambiare modalità")
            //.AddName(Language.Polish, "Pulsar Adamantytowy").AddTooltip(Language.Polish, "Nie wymaga amunicji\nPrawy przycisk, aby zmienić tryb")
            //.AddName(Language.PortugueseBrazil, "Pulsar de Adamantita").AddTooltip(Language.PortugueseBrazil, "Não requer munição\nBotão direito para alterar modos")
            .AddName(Language.Russian, "Адамантитовый Пульсар").AddTooltip(Language.Russian, "Не требует боеприпасов\nПКМ, чтобы сменить режим");
            //.AddName(Language.ChineseTraditional, "堅鋼脈衝器").AddTooltip(Language.ChineseTraditional, "不需要彈藥\n右鍵切換模式")
            //.AddName(Language.ChineseSimplified, "精金脉冲器").AddTooltip(Language.ChineseSimplified, "不需要弹药\n右键切换模式");
        }

        public override void SetDefaults()
        {
            Item.damage = 66; 
            Item.knockBack = KnockbackTiers.Average;
            Item.DamageType = DamageClass.Ranged;

            Item.width = 82;
            Item.height = 30;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.shootSpeed = 2f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<AdamantitePulseShot>();

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23, 4);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (mode == 0)
            {
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useTime = 10;
                Item.useAnimation = 10 * 3;
                Item.noUseGraphic = true;
            }
        }

        //mode 0 = Single Shot
        //mode 1 = multi Shot
        int mode = 0;
        int currentShot = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Change Modes
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 0;
                player.itemTime = 0;
                player.itemAnimation = 0;

                mode = mode == 0 ? 1 : 0;

                if (mode == 0)
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Charge", false, true);
                else
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Burst", false, true);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_149") with { Pitch = .35f, Volume = 0.45f, MaxInstances = 1 }; 
                SoundEngine.PlaySound(style, player.Center);

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_3") with { Pitch = .15f, Volume = 0.45f, MaxInstances = 1 }; 
                SoundEngine.PlaySound(style3, player.Center);


                return false;
            }

            //Shoot
            if (mode == 0)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<AdamantitePulsarHeldProj>(), damage, knockback, Main.myPlayer);
            }
            else if (mode == 1)
            {
                #region dust
                Vector2 dustOffsetPos = position + velocity.SafeNormalize(Vector2.UnitX) * 50f;

                for (int i = 0; i < 2 + Main.rand.Next(1, 2); i++) //2 //0,3
                {
                    Dust dp = Dust.NewDustPerfect(dustOffsetPos, ModContent.DustType<LineSpark>(),
                        velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(6f, 22f),
                        newColor: Color.Red * 1f, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);

                    dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                        0.8f, 0.5f); //80

                }

                for (int i = 0; i < 3 + Main.rand.Next(0, 2); i++)
                {
                    Color col1 = Color.Lerp(Color.DeepPink, Color.HotPink, 0.65f);

                    Vector2 randomStart = Main.rand.NextVector2Circular(4f, 4f) * 1f;
                    Dust dust = Dust.NewDustPerfect(dustOffsetPos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: Color.Red, Scale: Main.rand.NextFloat(0.25f, 0.3f) * 1.15f);
                    dust.noLight = false;
                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                        velToBeginShrink: 10f, fadePower: 0.93f, shouldFadeColor: false);

                    dust.velocity += velocity.SafeNormalize(Vector2.UnitX) * 6f;
                }
                #endregion

                //HeldProj
                //Kill the current held proj if it exists
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active)
                        if (p.type == ModContent.ProjectileType<AdamantitePulsarRecoilBurst>())
                            if (p.owner == player.whoAmI)
                                p.active = false;
                }

                int gun = Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<AdamantitePulsarRecoilBurst>(), 0, 0, player.whoAmI);
                if (Main.projectile[gun].ModProjectile is AdamantitePulsarRecoilBurst held)
                {
                    held.SetProjInfo(
                        GunID: ModContent.ItemType<AdamantitePulsar>(),
                        AnimTime: 14,
                        NormalXOffset: 26f,
                        DestXOffset: 18f,
                        YRecoilAmount: 0.02f,
                        HoldOffset: new Vector2(0f, 5f),
                        TipPos: new Vector2(34f, 0f),
                        StarPos: new Vector2(28f, 0f)
                        );

                    held.timeToStartFade = 0;
                }

                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 16;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(source, position, velocity * 4, ModContent.ProjectileType<AdamSmallShot>(), (int)(damage * 1f), knockback, Main.myPlayer);


                //lol
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_92") with { Pitch = .80f, PitchVariance = 0.2f, Volume = 0.2f }; 
                SoundEngine.PlaySound(style, player.Center);
                SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .2f, PitchVariance = 0.1f, Volume = 0.4f };
                SoundEngine.PlaySound(style23, player.Center);
                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_2") with { Volume = .40f, Pitch = .8f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style3, player.Center);
                SoundStyle style4 = new SoundStyle("Terraria/Sounds/Research_3") with { Volume = .3f, Pitch = .55f, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style4, player.Center);
                SoundStyle style5 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .05f, Pitch = 1f, PitchVariance = 0.25f }; 
                SoundEngine.PlaySound(style5, player.Center);

                currentShot++;
                if (currentShot == 3)
                {
                    delayTimer = 45;
                    currentShot = 0;
                }
            }

            return false;
        }
        public override void HoldItem(Player player)
        {
            //yeah reuseDelay exists but doing it this way is so item speed does not equal more shots 
            delayTimer--;
        }

        int delayTimer;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            if (delayTimer > 0)
                return false;
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int itemID = ModContent.ItemType<AdamantitePulsar>();
            string currentLanguage = Terraria.Localization.LanguageManager.Instance.ActiveCulture.Name;

            string modeCharge, modeBurst, skillStrikeCharge, skillStrikeBurst;

            switch (currentLanguage)
            {
                case "es-ES": // Spanish
                    modeCharge = "Carga - Mantén presionado para cargar un disparo perforante, aumentando la precisión cuanto más tiempo cargues";
                    modeBurst = "Ráfaga - Dispara una ráfaga de tres balas";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Golpe de Habilidad al soltar con un tiempo perfecto [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] El tercer disparo realiza un Golpe de Habilidad si los otros aciertan al mismo objetivo [i:" + ItemID.FallenStar + "]";
                    break;

                case "fr-FR": // French
                    modeCharge = "Charge - Maintenez pour charger un tir perçant, augmentant la précision plus longtemps vous chargez";
                    modeBurst = "Rafale - Tire une rafale de trois balles";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Coup de Compétence en relâchant avec un timing parfait [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Le troisième tir déclenche un Coup de Compétence si les autres ont touché la même cible [i:" + ItemID.FallenStar + "]";
                    break;

                case "de-DE": // German
                    modeCharge = "Aufladen - Halte gedrückt, um einen durchdringenden Schuss aufzuladen, wobei die Genauigkeit mit der Ladezeit steigt";
                    modeBurst = "Salve - Feuert eine Salve von drei Kugeln";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Fähigkeitsschlag beim perfekten Timing beim Loslassen [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Der dritte Schuss führt einen Fähigkeitsschlag aus, wenn die anderen das gleiche Ziel treffen [i:" + ItemID.FallenStar + "]";
                    break;

                case "it-IT": // Italian
                    modeCharge = "Carica - Tieni premuto per caricare un colpo perforante, aumentando la precisione più a lungo carichi";
                    modeBurst = "Raffica - Spara una raffica di tre proiettili";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Colpo dell'Abilità rilasciando con tempismo perfetto [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Il terzo colpo esegue un Colpo dell'Abilità se gli altri colpiscono lo stesso bersaglio [i:" + ItemID.FallenStar + "]";
                    break;

                case "pl-PL": // Polish
                    modeCharge = "Ładunek - Przytrzymaj, aby naładować przeszywający strzał, zwiększając celność im dłużej ładujesz";
                    modeBurst = "Seria - Wystrzeliwuje serię trzech kul";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Cios Umiejętności przy wypuszczeniu z idealnym wyczuciem czasu [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Trzeci strzał wykonuje Cios Umiejętności, jeśli poprzednie trafiły w ten sam cel [i:" + ItemID.FallenStar + "]";
                    break;

                case "pt-BR": // Portuguese (Brazil)
                    modeCharge = "Carga - Segure para carregar um tiro perfurante, aumentando a precisão quanto mais tempo carregar";
                    modeBurst = "Rajada - Dispara uma rajada de três balas";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Golpe de Habilidade ao soltar com tempo perfeito [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] O terceiro tiro realiza um Golpe de Habilidade se os outros atingirem o mesmo alvo [i:" + ItemID.FallenStar + "]";
                    break;

                case "ru-RU": // Russian
                    modeCharge = "Заряд - Удерживайте, чтобы зарядить проникающий выстрел, увеличивая точность с увеличением заряда";
                    modeBurst = "Очередь - Выпускает очередь из трех пуль";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Навык Удара активируется при отпускании с идеальным таймингом [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Третий выстрел активирует Навык Удара, если предыдущие попали в ту же цель [i:" + ItemID.FallenStar + "]";
                    break;

                case "zh-Hant": // Chinese (Traditional)
                    modeCharge = "充能 - 按住蓄力發射貫穿射擊，充能時間越長準確度越高";
                    modeBurst = "連發 - 發射三發子彈";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] 以完美時機釋放觸發技能打擊 [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] 第三發擊中相同目標時觸發技能打擊 [i:" + ItemID.FallenStar + "]";
                    break;

                default: // English and fallback
                    modeCharge = "Charge - Hold to charge a piercing shot, accuracy increasing the longer you charge";
                    modeBurst = "Burst - Fires a burst of three bullets";
                    skillStrikeCharge = "[i:" + ItemID.FallenStar + "] Skill Strike by releasing with perfect timing [i:" + ItemID.FallenStar + "]";
                    skillStrikeBurst = "[i:" + ItemID.FallenStar + "] Third shot Skill Strikes if the other shots hit the same target [i:" + ItemID.FallenStar + "]";
                    break;
            }

            tooltips.Add(new TooltipLine(Mod, "mode", mode == 0 ? modeCharge : modeBurst) { OverrideColor = Color.Red });
            tooltips.Add(new TooltipLine(Mod, "SkillStrike", mode == 0 ? skillStrikeCharge : skillStrikeBurst) { OverrideColor = Color.Gold });
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteBar, 15).
                AddIngredient(ItemID.ChlorophyteBar, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    //3 shot burst held proj
    public class AdamantitePulsarRecoilBurst : BasicRecoilProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Item[gunID].Value;

            Player Player = Main.player[Projectile.owner];
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 heldOffset = new Vector2(HoldoutOffset.X, HoldoutOffset.Y * Player.direction).RotatedBy(Projectile.rotation);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY) + heldOffset;

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation, Texture.Size() / 2, Projectile.scale, mySE, 0f);

            //Glowmask
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar/AdamantitePulsar_Glow").Value;
            Main.spriteBatch.Draw(Glowmask, drawPos, null, Color.White, Projectile.rotation, Glowmask.Size() / 2, Projectile.scale, mySE, 0f);

            //Glowlayer
            Texture2D Glowlayer = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar/AdamantitePulsar_WhiteGlow").Value;
            Main.spriteBatch.Draw(Glowlayer, drawPos, null, Color.White with { A = 0 } * Easings.easeInQuad(bonusPower) * 3f, Projectile.rotation, Glowlayer.Size() / 2, Projectile.scale, mySE, 0f);


            return false;
        }
    }

    public class AdamantitePulsarHeldProj : ModProjectile
    {
        int timer = 0;
        public float offset = 10; 
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;
        public float skillCritWindow = 10;

        Vector2 reticleLocation = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 999999;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        bool hasLetGo = false;

        float reticleProgress = 0f;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2; 
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer)
                reticleLocation = Main.MouseWorld + Player.velocity;

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter + Player.velocity)).ToRotation();
                    reticleLocation = (Main.MouseWorld);
                }
                direction = Angle.ToRotationVector2();

            } 
            //Release Shot
            else
            {
                //Make we dont shoot another laser if the player spam clicks during the recoil
                if (Projectile.timeLeft > 100)
                {
                    hasLetGo = true;
                    Projectile.timeLeft = 20;

                    if (reticleProgress == 1)
                        Projectile.timeLeft = 30;

                    //Shoot Proj
                    float spread = 15f * (1 - reticleProgress);
                    Vector2 adjustedVel = new Vector2(2, 0).RotatedBy(Angle).RotatedByRandom(MathHelper.ToRadians(spread));

                    Angle = adjustedVel.ToRotation();


                    int damage = (int)(Projectile.damage * (1f + (2f * reticleProgress)));
                    int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + adjustedVel * 10, adjustedVel * 1.5f, ModContent.ProjectileType<AdamantitePulseShot>(), damage, Projectile.knockBack, Main.myPlayer);

                    if (Main.projectile[shot].ModProjectile is AdamantitePulseShot aps)
                        aps.big = reticleProgress == 1;


                    #region dust
                    Vector2 vel1 = adjustedVel * (reticleProgress == 1 ? 2.3f : 2.25f);
                    Vector2 vel2 = adjustedVel * (reticleProgress == 1 ? 2.8f : 2.75f);

                    Dust circA = Dust.NewDustPerfect(Projectile.Center + adjustedVel * 3, ModContent.DustType<Dusts.GlowDusts.CirclePulse>(), vel1, newColor: new Color(255, 10, 10) * 0.6f, Scale: 0.01f);
                    circA.customData = new CirclePulseBehavior((reticleProgress == 1 ? 0.65f : 0.55f), false, 2, 0.25f, 0.5f);                   

                    Dust circB = Dust.NewDustPerfect(Projectile.Center + adjustedVel * 3, ModContent.DustType<Dusts.GlowDusts.CirclePulse>(), vel2, newColor: new Color(255, 10, 10) * 0.7f, Scale: 0.01f);
                    circB.customData = new CirclePulseBehavior((reticleProgress == 1 ? 0.35f : 0.25f), false, 1, 0.25f, 0.5f);

                    Vector2 dustOffsetPos = Projectile.Center + adjustedVel * 10f;
                    for (int i = 220; i < 4 + Main.rand.Next(0, 2); i++)
                    {
                        Color col1 = Color.Lerp(Color.DeepPink, Color.HotPink, 0.65f);

                        Vector2 randomStart = Main.rand.NextVector2Circular(6f, 6f) * 1f;
                        Dust dust = Dust.NewDustPerfect(dustOffsetPos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: Color.Red, Scale: Main.rand.NextFloat(0.45f, 0.55f) * 1f);
                        dust.noLight = false;
                        dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                            velToBeginShrink: 10f, fadePower: 0.93f, shouldFadeColor: false);

                        dust.velocity += adjustedVel.SafeNormalize(Vector2.UnitX) * 12f;
                    }
                    #endregion

                    if (skillCritWindow > 0 && reticleProgress == 1)
                    {
                        SkillStrikeUtil.setSkillStrike(Main.projectile[shot], 1.3f, 2);
                    }

                    SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .10f, PitchVariance = 0.4f, Volume = 0.4f };
                    SoundEngine.PlaySound(style23, Projectile.Center);


                    SoundStyle style32;
                    if (reticleProgress == 1)
                        style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.2f, Pitch = -0.33f, MaxInstances = -1, PitchVariance = 0.15f };
                    else
                        style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.2f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style32, Projectile.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Research_3") with { Volume = .28f, Pitch = .6f, PitchVariance = 0.2f };
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    offset = 0;

                    if (reticleProgress == 1)
                    {
                        SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .13f, Pitch = .15f, PitchVariance = 0.1f }; 
                        SoundEngine.PlaySound(style, Projectile.Center);

                        Player.GetModPlayer<AeroPlayer>().ScreenShakePower = 18;
                        Player.velocity += Angle.ToRotationVector2() * -5.5f;

                        offset = -13;
                    }

                    glowAmount = 1f;
                }

            }

            offset = Math.Clamp(MathHelper.Lerp(offset, 15f, 0.1f), -10, 10);

            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            direction = Angle.ToRotationVector2();
            Projectile.Center = Player.MountedCenter + (direction * offset);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);

            if (Player.channel)
                reticleProgress = Math.Clamp(reticleProgress + 0.02f, 0f, 1f);
            else if (hasLetGo && reticleProgress != 1)
                reticleAlpha = Math.Clamp(MathHelper.Lerp(reticleAlpha, -1, 0.05f), 0, 1f);

            if (reticleProgress == 1)
                skillCritWindow--;

            if (skillCritWindow > 0 && reticleProgress == 1)
                goldPulseAmount = 1;


            if (hasLetGo && Projectile.timeLeft < (reticleProgress == 1 ? 12 : 8))
            {
                gunOpacity = Math.Clamp(MathHelper.Lerp(gunOpacity, -0.65f, 0.06f), 0, 1);
                reticleAlpha = Math.Clamp(MathHelper.Lerp(reticleAlpha, -1f, 0.12f), 0, 1); 
            }

            goldPulseAmount = Math.Clamp(MathHelper.Lerp(goldPulseAmount, -0.5f, 0.04f), 0, 1);
            glowAmount = Math.Clamp(MathHelper.Lerp(glowAmount, -0.5f, 0.06f), 0, 1);

            timer++;
        }

        float glowAmount = 0f;
        float goldPulseAmount = 0f;
        float reticleAlpha = 1f;
        float gunOpacity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar/AdamantitePulsar_WhiteGlow").Value;

            //Gun Drawing
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = (Projectile.Center - (0.5f * (direction * -17)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition).Floor();
            position += new Vector2(0, 3 * Player.direction).RotatedBy(Angle);

            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(texture, position, null, lightColor * gunOpacity, direction.ToRotation(), texture.Size() / 2f, Projectile.scale, myEffect, 0.0f);

            Color col1 = Color.Lerp(Color.White, Color.Gold, goldPulseAmount);

            Main.spriteBatch.Draw(Glow, position, null, col1 with { A = 0 } * glowAmount * gunOpacity, direction.ToRotation(), texture.Size() / 2f, Projectile.scale, myEffect, 0.0f);
            Main.spriteBatch.Draw(Glow, position, null, col1 with { A = 0 } * glowAmount * gunOpacity, direction.ToRotation(), texture.Size() / 2f, Projectile.scale, myEffect, 0.0f);


            #region Reticle Drawing
            Texture2D OuterL = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar/RedOuterL").Value;
            Texture2D InnerL = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar/WhiteInnerL").Value;

            float progress = Easings.easeInOutQuad(reticleProgress);
            float extraAngle = MathHelper.Lerp(MathF.PI * -0.25f, 2f * MathF.PI, progress);
            float opactity = MathHelper.Lerp(0f, 1f, Easings.easeInQuad(reticleProgress * 1.15f)) * reticleAlpha;
            float scale = reticleProgress * 0.9f;

            Color col = Color.Lerp(Color.Red, Color.Gold, goldPulseAmount);

            Vector2 reticlePosA = reticleLocation - Main.screenPosition + new Vector2(0, 100 * (1 - reticleProgress) + 10).RotatedBy(Angle + extraAngle);
            Vector2 reticlePosB = reticleLocation - Main.screenPosition + new Vector2(0, -100 * (1 - reticleProgress) - 10).RotatedBy(Angle + extraAngle);

            Main.spriteBatch.Draw(OuterL, reticlePosA, null, Color.White with { A = 0 } * (opactity * 0.75f), Angle - MathHelper.PiOver4, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(OuterL, reticlePosB, null, Color.White with { A = 0 } * (opactity * 0.75f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f);

            Main.spriteBatch.Draw(InnerL, reticlePosA, null, col * (opactity * 0.75f), Angle - MathHelper.PiOver4, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(InnerL, reticlePosB, null, col * (opactity * 0.75f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, OuterL.Size() / 2, scale, SpriteEffects.None, 0.0f);
            #endregion

            return false;
        }
    }
 
}