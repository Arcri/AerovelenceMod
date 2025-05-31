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
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using System.Reflection.PortableExecutable;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using System.Threading;
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;

namespace AerovelenceMod.Content.Items.Weapons.Ocean
{
    public class OceanMist : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("OceanMist", "Casts a water burst")
            .AddName(Language.Default, "Ocean Mist")
            .AddTooltip(Language.Default, "Casts a water burst")
            .AddSkillStrike(Language.Default, "Skill Strikes at Full Mana")

            .AddName(Language.Spanish, "Niebla Oceánica").AddTooltip(Language.Spanish, "Lanza una ráfaga de agua").AddSkillStrike(Language.Spanish, "Realiza Golpes de Habilidad con maná completo")
            .AddName(Language.French, "Brume Océanique").AddTooltip(Language.French, "Lance une explosion d'eau").AddSkillStrike(Language.French, "Déclenche un Coup de Compétence à mana plein")
            .AddName(Language.German, "Ozeannebel").AddTooltip(Language.German, "Wirft eine Wasserexplosion").AddSkillStrike(Language.German, "Führt Fähigkeitsschläge bei vollem Mana aus")
            .AddName(Language.Italian, "Nebbia Oceanica").AddTooltip(Language.Italian, "Scaglia un'esplosione d'acqua").AddSkillStrike(Language.Italian, "Esegue Colpi dell'Abilità a mana pieno")
            .AddName(Language.Polish, "Morska Mgła").AddTooltip(Language.Polish, "Wystrzeliwuje wodną eksplozję").AddSkillStrike(Language.Polish, "Ciosy Umiejętności przy pełnej manie")
            .AddName(Language.PortugueseBrazil, "Névoa do Oceano").AddTooltip(Language.PortugueseBrazil, "Lança uma explosão de água").AddSkillStrike(Language.PortugueseBrazil, "Realiza Golpes de Habilidade com mana cheio")
            .AddName(Language.Russian, "Океанский Туман").AddTooltip(Language.Russian, "Выпускает водяной взрыв").AddSkillStrike(Language.Russian, "Навык Удара активируется при полном запасе маны")
            .AddName(Language.ChineseTraditional, "海霧").AddTooltip(Language.ChineseTraditional, "釋放水爆").AddSkillStrike(Language.ChineseTraditional, "滿法力時觸發技能打擊")
            .AddName(Language.ChineseSimplified, "海雾").AddTooltip(Language.ChineseSimplified, "释放水爆").AddSkillStrike(Language.ChineseSimplified, "满法力时触发技能打击");
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.knockBack = KnockbackTiers.Weak;
            Item.mana = 9;
            Item.shootSpeed = 8f;

            Item.width = 40;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 30;

            Item.shoot = ModContent.ProjectileType<OceanMistHeldProj>();
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(0, 0, 15, 0);

            Item.autoReuse = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int a = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<OceanMistHeldProj>(), damage, knockback, player.whoAmI);
            
            if (player.statMana + player.GetManaCost(player.inventory[player.selectedItem]) == player.statManaMax2)
                (Main.projectile[a].ModProjectile as OceanMistHeldProj).shouldSkillStrike = true;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PalmWood, 25).
                AddIngredient(ItemID.Coral, 5).
                AddIngredient(ItemID.FallenStar, 3).
                AddTile(TileID.Anvils).
                Register();
        }

    }

    public class OceanMistHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public float OFFSET = -15; //30
        public float alphaPercent = 0;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 100;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        
        public bool shouldSkillStrike = false;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            #region held proj stuff

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2; 
            Player.itemAnimation = 2;

            KillHeldProjIfPlayerDeadOrStunned(Projectile);

            //Get angle to mouse
            if (Projectile.owner == Main.myPlayer)
                Angle = (Main.MouseWorld - Player.Center).ToRotation();

            //Have player turn whichever direction they point
            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            float goalRotation = direction.ToRotation() + (MathHelper.PiOver4 * Player.direction);
            float startRotation = goalRotation + (MathHelper.TwoPi * -1.25f) * Player.direction;

            float spinInProgress = Math.Clamp((float)timer / 20f, 0f, 1f);
            Projectile.rotation = MathHelper.Lerp(startRotation, goalRotation, Easings.easeInOutHarsh(spinInProgress)); //InOutSine

            //Like how this looks without composite arms better
            //Player.SetCompositeArmFront(true, stretch: Player.CompositeArmStretchAmount.Full, Angle - MathHelper.PiOver2);

            #endregion

            float maxOffset = 16f; //14f

            if (timer >= 42)
            {
                if (timer == 42)
                {
                    //Start fadeout
                    Projectile.timeLeft = 15;
                }
                else
                {
                    //Fade out projectile
                    OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, -15f, 0.03f), -20, maxOffset);
                    alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, -0.25f, 0.15f), 0, 1);
                }
            }
            else
            {
                //Fade in
                OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, maxOffset, 0.2f), -100, maxOffset);
                alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, 1f, 0.065f), 0, 1f); //0.08
            }

            //Shoot shot
            if (timer == 20)
            {
                //FX
                glowAlpha = 1f;
                justShotPower = 1f;
                
                Vector2 vel = new Vector2(12.5f, 0).RotatedBy(direction.ToRotation());
                
                for (int i = 0; i < 4; i++)
                {
                    if (i < 4)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center + vel, ModContent.DustType<GlowFlare>(), Main.rand.NextVector2Circular(3, 3),
                            newColor: new Color(30, 105, 255), Scale: 0.7f);
                        d.customData = new GlowFlareBehavior(0.4f, 2.5f, 1f);
                        d.scale *= Main.rand.NextFloat(0.9f, 1.3f);
                    }

                    int a = Dust.NewDust(Projectile.Center + vel * 2, 1, 1, DustID.BlueTorch, Scale: 2f);
                    Main.dust[a].noGravity = true;
                }

                //Sounds
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/CommonWaterFallLight00") with { Volume = .23f, Pitch = .54f, PitchVariance = .4f, MaxInstances = -1, };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ENV_water_splash_01") with { Pitch = 0.1f, PitchVariance = 0.1f, Volume = 0.75f, MaxInstances = -1 }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                //Spawn Proj
                int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Angle.ToRotationVector2() * 8f, ModContent.ProjectileType<OceanMistShot>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

                if (shouldSkillStrike)
                    SkillStrikeUtil.setSkillStrike(Main.projectile[shot], 1.3f, 100, 0.35f, 0f); //1
            }

            //Swoosh Sound 
            if (timer % 7 == 0 && timer <= 20)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_7") with { Pitch = .45f, PitchVariance = 0.2f }; SoundEngine.PlaySound(style, Projectile.Center);
            }

            //Vfx values
            glowAlpha = Math.Clamp(MathHelper.Lerp(glowAlpha, -0.5f, 0.05f), 0f, 1f);

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.75f, 0.08f), 0f, 1f);

            // For having the spin always rotate away from the player
            Projectile.ai[0] = Player.direction;

            timer++;
        }

        float justShotPower = 0f;
        float glowAlpha = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMist");
            Texture2D Twirl = CommonTextures.PixelSwirl.Value;
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMistGlowy");
            Texture2D White = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMistWhite");


            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);

            Color SwirlCol = shouldSkillStrike ? Color.Yellow : Color.LightSkyBlue * 0.75f;

            if (timer <= 20)
            {
                Main.spriteBatch.Draw(Twirl, pos, null, SwirlCol with { A = 0 } * 0.4f * alphaPercent, Projectile.rotation, Twirl.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            }

            float weaponScale = Projectile.scale + (0.35f * justShotPower);


            Main.spriteBatch.Draw(Glow, pos, null, Color.DeepSkyBlue with { A = 0 } * glowAlpha * 1.5f, Projectile.rotation, Glow.Size() / 2, weaponScale, mySE, 0f);
            Main.spriteBatch.Draw(Weapon, pos, null, lightColor * alphaPercent, Projectile.rotation, Weapon.Size() / 2, weaponScale, mySE, 0f);


            //Glow overlay
            float overlayAlpha = Easings.easeInSine(glowAlpha);
            Main.spriteBatch.Draw(White, pos, null, Color.LightSkyBlue with { A = 0 } * overlayAlpha, Projectile.rotation, Weapon.Size() / 2, weaponScale, mySE, 0f);

            return false;
        }

    }

    public class OceanMistShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
 
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 100;
            Projectile.penetrate = 10;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

        }
        int maximumPierce = 10;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (maximumPierce <= 0)
                return false;

            //Check collision in a radius for every 2 positions
            int i = 0;
            foreach (Vector2 vec in previousPostions)
            {
                if (i % 2 == 0 && targetHitbox.Distance(vec) < 10)
                    return true;
                i++;

            }
            return false;
        }

        int timer = 0;
        public override void AI()
        {
            int trailCount = 20;

            if (timer % 2 == 0)
            {
                previousRotations.Add(Projectile.velocity.ToRotation());
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }


            Projectile.velocity.Y += 0.09f;

            //Dust
            if (timer % 2 == 0 && timer > 3 && Main.rand.NextBool(2))
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(3f, 3f);

                Dust da = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), dustVel, newColor: Color.DeepSkyBlue * 0.65f, Scale: Main.rand.NextFloat(0.15f, 0.25f) * 1.75f);
                da.velocity -= Projectile.velocity.RotatedByRandom(0.2f) * 0.65f;
                da.alpha = 12;
            }

            if (timer % 3 == 0 && Main.rand.NextBool(5) && timer > 3)
            {
                Vector2 vel = Main.rand.NextVector2Circular(7f, 7f);
                Dust de = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFlare>(), vel, newColor: Color.DodgerBlue, Scale: 0.5f);
                de.customData = new GlowFlareBehavior(0.4f, 2.5f, 1f);

                de.velocity *= 0.45f;
                de.velocity += Projectile.velocity * 0.5f;
            }

            starPower = Math.Clamp(MathHelper.Lerp(starPower, 1.25f, 0.04f), 0f, 1f);

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 0.7f);

            timer++;
        }


        float starPower = 0f;

        float overallAlpha = 1f;
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPostions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            //Star
            if (starPower < 1)
            {
                Texture2D star = Mod.Assets.Request<Texture2D>("Assets/Pixel/CrispStarPMA").Value;

                Vector2 posOffset = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 3f;

                Vector2 drawPos = Projectile.Center + posOffset - Main.screenPosition;

                float dir = Projectile.velocity.X > 0 ? 1 : -1;

                float starRotation = MathHelper.Lerp(0f, MathHelper.Pi * 2f * dir, Easings.easeInOutQuad(starPower)) + Projectile.velocity.ToRotation();
                float starScale = Easings.easeOutQuint(1f - starPower) * Projectile.scale * 1.3f;

                Vector2 starScaleVec2 = new Vector2(1f, 0.5f) * starScale;

                Main.EntitySpriteDraw(star, drawPos, null, Color.DeepSkyBlue with { A = 0 } * starPower, starRotation, star.Size() / 2f, starScale, SpriteEffects.None);
                Main.EntitySpriteDraw(star, drawPos, null, Color.White with { A = 0 } * starPower, starRotation, star.Size() / 2f, starScale * 0.55f, SpriteEffects.None);
            }

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawTrail();
            });

            return false;
        }

        public void DrawTrail()
        {
            Texture2D line = CommonTextures.Flare.Value;

            //After-Image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.25f) * 0.1f;

                    Vector2 AfterImagePos = previousPostions[i] - Main.screenPosition + Main.rand.NextVector2Circular(4f, 4f); //3f

                    float startScale = Projectile.scale + sineScale;

                    Color between = Color.Lerp(Color.DeepSkyBlue, Color.DodgerBlue, 0.8f);
                    Color col = Color.Lerp(between, Color.DodgerBlue, 1f - progress);

                    float easedFadeValue = Easings.easeInSine(progress);


                    Vector2 lineScale = new Vector2(1.25f, 0.5f + 0.4f * progress); //
                    Vector2 lineScale2 = new Vector2(1.25f, 0.08f + 0.05f * progress); //0.1f 0.2f

                    //Main
                    Main.EntitySpriteDraw(line, AfterImagePos, null, col with { A = 0 } * 1f * easedFadeValue,
                        previousRotations[i], line.Size() / 2f, lineScale * startScale, SpriteEffects.None);

                    //White
                    Main.EntitySpriteDraw(line, AfterImagePos, null, Color.White with { A = 0 } * 1f * easedFadeValue,
                        previousRotations[i], line.Size() / 2f, lineScale2 * startScale, SpriteEffects.None);

                }

            }



        }

        public override void OnKill(int timeLeft)
        {
            Color col = Color.Lerp(Color.DeepSkyBlue, Color.DodgerBlue, 0.5f);

            //Dust On Trail
            int i = 0;
            foreach (Vector2 pos in previousPostions)
            {
                i++;
                if (Main.rand.NextBool(2))
                {
                    int a = Dust.NewDust(pos, 0, 0, ModContent.DustType<GlowFlare>(), 0, 0, newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.55f));
                    Main.dust[a].customData = new GlowFlareBehavior(0.4f, 2.5f, 1f);
                    Main.dust[a].velocity *= 0.55f + ((i * 0.04f));
                    Main.dust[a].velocity += Projectile.velocity * 0.2f;
                }
            }

            //Dust on tip
            for (int j = 0; j < Main.rand.Next(4, 7);  j++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 5f);

                float dustScale = Main.rand.NextFloat(0.5f, 0.65f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFlare>(), dustVel, newColor: col, Scale: dustScale);
                d.customData = new GlowFlareBehavior(0.4f, 2.5f, 1f);
                d.velocity += Projectile.velocity * 0.1f;
            }

            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ENV_water_splash_01") with { Volume = 0.5f, Pitch = 0.5f, MaxInstances = -1 }; 
            SoundEngine.PlaySound(style, Projectile.Center);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Color dustCol = Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike ? Color.Orange : Color.DeepSkyBlue;
            for (int i = 0; i < 2 + Main.rand.Next(0,3); i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2f, 2f);

                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: dustCol, Scale: Main.rand.NextFloat(0.2f, 0.3f));
            }

            
            if (maximumPierce % 2 == 0)
                Projectile.damage = (int)(Projectile.damage * 0.95f);
            maximumPierce--;
        }
    }

}
