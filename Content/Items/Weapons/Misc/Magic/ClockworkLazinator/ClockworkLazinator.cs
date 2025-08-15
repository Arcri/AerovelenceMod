using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.Ceroba;
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Interfaces;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.ClockworkLazinator
{
    public class ClockworkLazinator : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("ClockworkLazinator", "Right-Click to wind up the weapon, increasing how long it fires for\nCaps out at 4 winds")
            .AddName(Language.Default, "Clockwork Lazinator")
            .AddTooltip(Language.Default, "Right-Click to wind up the weapon, increasing how long it fires for\nCaps out at 4 winds")
            .AddSkillStrike(Language.Default, "Skill Strikes after firing for long enough")

            .AddName(Language.Spanish, "Lazinador Mecánico").AddTooltip(Language.Spanish, "Haz clic derecho para dar cuerda al arma, aumentando su duración de disparo\nSe detiene en 4 vueltas").AddSkillStrike(Language.Spanish, "Golpes de Habilidad después de disparar por suficiente tiempo")
            .AddName(Language.French, "Lazinator à Engrenages").AddTooltip(Language.French, "Clic droit pour remonter l'arme, augmentant la durée du tir\nSe limite à 4 remontées").AddSkillStrike(Language.French, "Les Coups de Compétence se déclenchent après un tir prolongé")
            .AddName(Language.German, "Uhrwerk-Lazinator").AddTooltip(Language.German, "Rechtsklick, um die Waffe aufzuziehen und die Feuerrate zu verlängern\nBegrenzt auf 4 Aufzüge").AddSkillStrike(Language.German, "Fähigkeitsschläge treten nach längerem Feuern auf")
            .AddName(Language.Italian, "Lazinator a Ingranaggi").AddTooltip(Language.Italian, "Tasto destro per caricare l'arma, aumentando la durata del fuoco\nSi ferma a 4 cariche").AddSkillStrike(Language.Italian, "I Colpi dell'Abilità si attivano dopo aver sparato abbastanza a lungo")
            //.AddName(Language.Polish, "Mechaniczny Lazinator").AddTooltip(Language.Polish, "Prawy przycisk, aby nakręcić broń, zwiększając czas strzału\nMaksymalnie 4 nakręcenia").AddSkillStrike(Language.Polish, "Ciosy Umiejętności występują po wystarczająco długim strzelaniu")
            //.AddName(Language.PortugueseBrazil, "Lazinator Mecânico").AddTooltip(Language.PortugueseBrazil, "Botão direito para dar corda na arma, aumentando o tempo de disparo\nLimite de 4 giros").AddSkillStrike(Language.PortugueseBrazil, "Os Golpes de Habilidade ocorrem após disparar por tempo suficiente")
            .AddName(Language.Russian, "Часовой Лазинатор").AddTooltip(Language.Russian, "ПКМ, чтобы заводить оружие, увеличивая время стрельбы\nМаксимум 4 завода").AddSkillStrike(Language.Russian, "Навык Удара активируется после достаточного времени стрельбы");
            //.AddName(Language.ChineseTraditional, "發條雷射器").AddTooltip(Language.ChineseTraditional, "右鍵為武器上發條，提高射擊時間\n最多可上發條 4 次").AddSkillStrike(Language.ChineseTraditional, "技能打擊發生在射擊足夠長時間後")
            //.AddName(Language.ChineseSimplified, "发条激光器").AddTooltip(Language.ChineseSimplified, "右键为武器上发条，提高射击时间\n最多可上发条 4 次").AddSkillStrike(Language.ChineseSimplified, "技能打击发生在射击足够长时间后");
        }

        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.knockBack = 4f; //Weak-Average Knockback
            Item.mana = 5;

            Item.width = 74;
            Item.height = 34;

            Item.useTime = 4;
            Item.useAnimation = 20;
            Item.reuseDelay = 10;
            Item.shootSpeed = 10f;

            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarities.PrePlantPostMech;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<LazinatorHeldProj>();
            Item.value = Item.sellPrice(0, 4, 10, 0);

            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Cog, 50).
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.Lens, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            //Dont consume mana on right-click (still pauses mana regen though but I don't think we can avoid that
            if (player.altFunctionUse == 2)
                mult *= 0;
        }

        public override bool CanUseItem(Player player)
        {

            if (player.altFunctionUse == 2)
            {
                int windNumber = Main.player[player.whoAmI].GetModPlayer<LazinatorPlayer>().winds;

                if (windNumber == Main.player[player.whoAmI].GetModPlayer<LazinatorPlayer>().WINDUP_MAX)
                {
                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Menu_Close") with { Volume = 0.75f, Pitch = -1f, MaxInstances = 0 };

                    SoundEngine.PlaySound(style3, Main.player[player.whoAmI].Center);
                    SoundEngine.PlaySound(style3, Main.player[player.whoAmI].Center);

                    return false;
                }
            }

            //Don't let them use if they don't have enough for a full burst
            return player.CheckMana(player.inventory[player.selectedItem], amount: player.inventory[player.selectedItem].mana * 5, pay: false); ;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<LazinatorWindUp>();

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class LazinatorShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2222;
        }

        public Vector2 endPoint = Vector2.Zero;
        float Rotation = 0;

        int timer = 0;

        bool collided = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.scale = 1f;

            Projectile.timeLeft = 3100;
            Projectile.extraUpdates = 100;
        }

        public override bool? CanDamage() { return !collided; }        

        public override void AI()
        {
            if (timer == 0)
            {
                Rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                endPoint = Projectile.Center;
            }

            if (collided)
            {
                Projectile.velocity = Vector2.Zero;
                if (timeAfterCollided > 300)
                    lineWidth = Math.Clamp(MathHelper.Lerp(lineWidth, -0.2f, 0.0015f), 0, 1f);

                if (lineWidth <= 0.1f) //0.4
                    Projectile.active = false;
                timeAfterCollided++;
            }

            if (timer == 200 && !collided) 
            {
                collided = true;
            }
            timer++;

        }

        int timeAfterCollided = 0;
        float uColorIntensity = 1.2f;
        float lineWidth = 1;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer > 0)
            {
                Texture2D texBeam = Mod.Assets.Request<Texture2D>("Assets/Trails/ThinGlowLine").Value;

                Vector2 beamOrigin = new Vector2(0, texBeam.Height / 2f);

                float height = 0.12f * Projectile.scale * lineWidth;

                if (height == 0f)
                    Projectile.active = false;

                float distance = (Projectile.Center - endPoint).Length() / 256f;

                Color beamCol = Color.Lerp(Color.DeepPink, Color.HotPink, 0.55f);
                Vector2 v2Scale1 = new Vector2(distance, height);
                Vector2 v2Scale2 = new Vector2(distance, height * 0.25f);

                Main.spriteBatch.Draw(texBeam, Projectile.Center - Main.screenPosition, null, beamCol with { A = 0 } * 1f, Rotation, beamOrigin, v2Scale1, 0, 0);
                Main.spriteBatch.Draw(texBeam, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Rotation, beamOrigin, v2Scale2, 0, 0);


                //End Points
                Texture2D star = Mod.Assets.Request<Texture2D>("Assets/Pixel/CrispStarPMA").Value;

                Vector2 starOrigin = star.Size() / 2f;
                float starScale = 0.4f * lineWidth * Projectile.scale;

                Color starCol = Color.Lerp(Color.DeepPink, Color.HotPink, 0.95f);

                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, starCol with { A = 0 }, Rotation, starOrigin, starScale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Rotation, starOrigin, starScale * 0.35f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(star, endPoint - Main.screenPosition, null, starCol with { A = 0 }, Rotation, starOrigin, starScale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, endPoint - Main.screenPosition, null, Color.White with { A = 0 }, Rotation, starOrigin, starScale * 0.35f, SpriteEffects.None, 0);

            }
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            collided = true;
            Projectile.velocity = Vector2.Zero;

            for (int ia = 0; ia < 1 + (Main.rand.NextBool() ? 1 : 0); ia++)
            {
                Vector2 speed = new Vector2(5, 0).RotatedBy(Rotation);
                int a = Dust.NewDust(Projectile.position, 5, 5, ModContent.DustType<ColorSpark>(), SpeedX: speed.X, SpeedY: speed.Y, newColor: Color.HotPink, Scale: 0.3f);
                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                extraInfo.gravityIntensity = 0.1f;
                Main.dust[a].fadeIn = 0.5f;
                Main.dust[a].alpha = 53;
                Main.dust[a].customData = extraInfo;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            for (int ia = 0; ia < 2; ia++)
            {
                Vector2 speed = new Vector2(-7, 0).RotatedBy(Rotation);
                int a = Dust.NewDust(Projectile.position, 5, 5, ModContent.DustType<ColorSpark>(), SpeedX: speed.X, SpeedY: speed.Y, newColor: Color.HotPink, Scale: 0.3f);
                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                extraInfo.gravityIntensity = 0.05f;
                Main.dust[a].fadeIn = 0.3f;
                Main.dust[a].alpha = 50;
                Main.dust[a].customData = extraInfo;
            }

            //Dust
            for (int i = 0; i < 3 - Main.rand.Next(0, 2); i++) //4 //2,2
            {
                Vector2 vel = new Vector2(-8, 0).RotatedBy(Rotation);

                Dust p = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineSpark>(),
                    vel.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.Next(7, 18),
                    newColor: Color.HotPink, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.3f);

                p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                    1f, 0.5f);
            }

            collided = true;
            Projectile.velocity = Vector2.Zero;
        }
    }

    //Why did i do it like this | actually nevermind this isn't that bad
    public class LazinatorHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        float OFFSET = 20; 

        ref float Angle => ref Projectile.ai[1];
        Vector2 direction = Vector2.Zero;
        float lerpToStuff = 0;
        bool ShouldFire = true;


        public override void SetDefaults()
        {
            Projectile.timeLeft = 50;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
        

        bool firstFrame = true;
        int shotCount = 0;
        public override void AI()
        {
            //Determine time left based on number of winds
            if (firstFrame)
            {
                Projectile.timeLeft = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds * 30 + 50;
                firstFrame = false;
            }

            HeldProjCode(false);
            
            Player owner = Main.player[Projectile.owner];

            //Fire weapon
            if (ShouldFire)
            {
                if (timer % 4 == 0 && timer > 15 && timer < 40 + (owner.GetModPlayer<LazinatorPlayer>().winds * 30))
                {
                    Vector2 vel = new Vector2(10, 0).RotatedBy(Angle);
                    Vector2 pos = Projectile.Center;

                    //Offset position to be at muzzle unless that would be through tiles
                    Vector2 muzzleOffset = Vector2.Normalize(vel) * 37f;
                    if (Collision.CanHit(Projectile.Center, 0, 0, Projectile.Center + muzzleOffset, 0, 0))
                        pos += muzzleOffset;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel.RotatedByRandom(0.05f), ModContent.ProjectileType<LazinatorShot>(), Projectile.damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);

                        if (Main.projectile[a].ModProjectile is LazinatorShot shot)
                            shot.endPoint = pos;

                        //Not sure if this will cause issue where it is only a skill strike for the owner
                        if (shotCount > 20)
                            SkillStrikeUtil.setSkillStrike(Main.projectile[a], 1.3f, 1, 0.5f, 0.15f);
                    }


                    SoundStyle style = new SoundStyle("Terraria/Sounds/Research_3") with { Volume = 0.3f, Pitch = .65f, PitchVariance = .2f };
                    SoundEngine.PlaySound(style, Main.player[Projectile.owner].Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_158") with { Pitch = .44f, };
                    SoundEngine.PlaySound(style2, Main.player[Projectile.owner].Center);


                    //Dust
                    for (int i = 0; i < 4 - Main.rand.Next(0, 2); i++) //4 //2,2
                    {
                        Dust dp = Dust.NewDustPerfect(pos, ModContent.DustType<LineSpark>(),
                            vel.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.Next(7, 18),
                            newColor: Color.HotPink, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.35f);

                        dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                            1f, 0.5f);
                    }

                    glowIntensity = 1f;
                    drawXScale = 0.85f;

                    if (!owner.CheckMana(owner.inventory[owner.selectedItem], pay: true))
                    {
                        owner.GetModPlayer<LazinatorPlayer>().winds = 0;
                        Projectile.active = false;
                    }

                    shotCount++;
                }
            }

            glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.75f, 0.08f), 0, 1);

            //Reset wind count when projectile is about to die
            if (Projectile.timeLeft == 2)
            {
                Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds = 0;
            }
        }

        //Generic held projectile code
        public void HeldProjCode(bool windup)
        {
            Player Player = Main.player[Projectile.owner];

            KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            if (timer == 0)
            {
                OFFSET = 0f;
            }
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 23f, 0.12f), 0, 20);
            drawXScale = Math.Clamp(MathHelper.Lerp(drawXScale, 1.2f, 0.2f), 0, 1);

            if (Projectile.timeLeft < 8 && !windup)
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.09f), 0, 1);
            else
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.2f), 0, 1);

            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();
            
            timer++;
        }

        float drawXScale = 1f;
        float alpha = 0f;
        public float glowIntensity = 0f;
        public float pinkGlowPower = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinator/ClockworkLazinator");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinator/ClockworkLazinatorGlow");
            Texture2D White = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/ClockworkLazinator/ClockworkLazinatorWhite");

            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawScale = new Vector2(drawXScale, 1f);

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);
            Vector2 drawOffset = new Vector2(0f, 2f * Player.direction).RotatedBy(Projectile.rotation); //This helps the bullets better align with the muzzle

            Main.spriteBatch.Draw(Weapon, drawPos + drawOffset, null, lightColor * alpha, Projectile.rotation, Weapon.Size() / 2, drawScale, mySE, 0f);
            Main.spriteBatch.Draw(Glow, drawPos + drawOffset, null, Color.White * alpha, Projectile.rotation, Weapon.Size() / 2, drawScale, mySE, 0f);
            Main.spriteBatch.Draw(White, drawPos + drawOffset, null, Color.White with { A = 0 } * alpha * glowIntensity, Projectile.rotation, White.Size() / 2, drawScale, mySE, 0f);

            //Over glow (fully charged winds)
            float pinkGlowSize = 1f + (pinkGlowPower * 0.2f);
            float pinkGlowAlpha = pinkGlowPower;
            if (pinkGlowPower > 0f)
            {
                Main.spriteBatch.Draw(Weapon, drawPos + drawOffset, null, Color.Pink with { A = 0 } * alpha * pinkGlowAlpha, Projectile.rotation, Weapon.Size() / 2, drawScale * pinkGlowSize, mySE, 0f);
                Main.spriteBatch.Draw(Weapon, drawPos + drawOffset, null, Color.Pink with { A = 0 } * alpha * pinkGlowAlpha, Projectile.rotation, Weapon.Size() / 2, drawScale * pinkGlowSize, mySE, 0f);
            }

            return false;
        }
    }

    public class LazinatorWindUp : LazinatorHeldProj
    {
        int windUpTimer = 0;
        float windUpPercent = 0;
        float windUpValue = 0;
        bool shouldKill = false;

        public override void AI()
        {
            HeldProjCode(true);

            Player owner = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;


            //Will kill proj at end of wind if they are not actively holding right clicker
            if (Main.myPlayer == owner.whoAmI && !Main.mouseRight)
                shouldKill = true;

            if (windUpTimer >= 20)
            {
                if (windUpTimer == 20)
                {

                    int windNumber = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds;

                    if (windNumber == Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().WINDUP_MAX)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Volume = 0.7f, Pitch = .6f, PitchVariance = 0.2f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = .75f, Pitch = .6f, }; 
                        SoundEngine.PlaySound(style2, Projectile.Center);

                        SoundStyle style4 = new SoundStyle("Terraria/Sounds/Item_149") with { Volume = 1f, Pitch = .7f };
                        SoundEngine.PlaySound(style4, Projectile.Center);

                        pinkGlowPower = 1f;
                    }
                    else
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_149") with { Volume = 1f, Pitch = .4f, PitchVariance = 0.1f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/TwinsDual_Union04") with { Volume = .2f, Pitch = -0.2f, PitchVariance = .25f, };
                        SoundEngine.PlaySound(style3, Projectile.Center);

                        glowIntensity = 1f;
                    }

                }

                windUpPercent = Math.Clamp((windUpTimer - 20) * 0.3f, 0, MathHelper.TwoPi); 

                if (windUpPercent == MathHelper.TwoPi)
                {
                    int windNumber = Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds;

                    bool maxWind = windNumber == 4;

                    Main.player[Projectile.owner].GetModPlayer<LazinatorPlayer>().winds = Math.Clamp(windNumber + 1, 0, 4);

                    windUpTimer = -10;
                    windUpValue = 0;
                    windUpPercent = 0;

                    if (shouldKill || maxWind)
                        Projectile.active = false;

                }
            }
            windUpValue = (float)Math.Sin(windUpPercent) * 0.4f;

            float armRot = (Projectile.Center - owner.Center).ToRotation() - MathHelper.PiOver2 + windUpValue;
            Main.player[Projectile.owner].SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

            pinkGlowPower = Math.Clamp(MathHelper.Lerp(pinkGlowPower, -0.25f, 0.12f), 0, 1);
            glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.75f, 0.08f), 0, 1);

            windUpTimer++;
        }
    }

    public class LazinatorPlayer : ModPlayer
    {
        public int winds = 0;
        public int WINDUP_MAX = 4;
    }

}
