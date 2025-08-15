using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common;
using Terraria.Graphics;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.WandOfExploding
{
    public class WandOfExploding : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;

            this.ModifyLocalization("WandOfExploding", "Inflicts Mana Burn, causing enemies to leak stars that restore mana")
            .AddName(Language.Default, "Wand of Exploding")
            .AddTooltip(Language.Default, "Inflicts Mana Burn, causing enemies to leak stars that restore mana")
            .AddSkillStrike(Language.Default, "Explosion Skill Strikes under 50% mana")

            .AddName(Language.Spanish, "Vara de Explosión").AddTooltip(Language.Spanish, "Inflige Quemadura de Maná, haciendo que los enemigos suelten estrellas que restauran maná").AddSkillStrike(Language.Spanish, "Golpes de Habilidad por debajo del 50% de maná")
            .AddName(Language.French, "Baguette Explosive").AddTooltip(Language.French, "Inflige Brûlure de Mana, faisant perdre des étoiles aux ennemis qui restaurent du mana").AddSkillStrike(Language.French, "Les Coups de Compétence se déclenchent sous 50% de mana")
            .AddName(Language.German, "Zauberstab der Explosionen").AddTooltip(Language.German, "Verursacht Manabrand, wodurch Feinde Sterne verlieren, die Mana wiederherstellen").AddSkillStrike(Language.German, "Fähigkeitsschläge treten bei unter 50% Mana auf")
            .AddName(Language.Italian, "Bacchetta delle Esplosioni").AddTooltip(Language.Italian, "Infligge Bruciatura di Mana, facendo perdere stelle ai nemici che ripristinano mana").AddSkillStrike(Language.Italian, "I Colpi dell'Abilità si attivano sotto il 50% di mana")
            //.AddName(Language.Polish, "Różdżka Eksplozji").AddTooltip(Language.Polish, "Nakłada Oparzenie Many, sprawiając, że wrogowie tracą gwiazdy przywracające manę").AddSkillStrike(Language.Polish, "Ciosy Umiejętności występują poniżej 50% many")
            //.AddName(Language.PortugueseBrazil, "Varinha Explosiva").AddTooltip(Language.PortugueseBrazil, "Inflige Queimadura de Mana, fazendo os inimigos soltarem estrelas que restauram mana").AddSkillStrike(Language.PortugueseBrazil, "Os Golpes de Habilidade ocorrem abaixo de 50% de mana")
            .AddName(Language.Russian, "Жезл Взрыва").AddTooltip(Language.Russian, "Накладывает Манапожог, заставляя врагов терять звезды, восстанавливающие ману").AddSkillStrike(Language.Russian, "Навык Удара активируется при мане ниже 50%");
            //.AddName(Language.ChineseTraditional, "爆炸魔杖").AddTooltip(Language.ChineseTraditional, "施加法力燃燒，使敵人洩漏恢復法力的星星").AddSkillStrike(Language.ChineseTraditional, "技能打擊發生在法力低於 50% 時")
            //.AddName(Language.ChineseSimplified, "爆炸魔杖").AddTooltip(Language.ChineseSimplified, "施加法力燃烧，使敌人泄漏恢复法力的星星").AddSkillStrike(Language.ChineseSimplified, "技能打击发生在法力低于 50% 时");
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.knockBack = KnockbackTiers.Average;
            Item.mana = 14;
            Item.shootSpeed = 17f;

            Item.width = 38;
            Item.height = 38;
            Item.useAnimation = 40;
            Item.useTime = 40;

            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<WandOfExplodingHeldProj>();
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.sellPrice(0, 0, 75, 0);
 
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        //We don't want just pulling out the staff to consume mana
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            //Costs zero mana with just holding the weapon out
            //This still causes natural mana regen to pause for some reason probably a tmod or vanilla bug //TODO: report this or find a way to fix it
            if (player.itemTime == 0)
                mult = 0f;
            base.ModifyManaCost(player, ref reduce, ref mult);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AerovelenceMod:EvilBars", 10).
                AddIngredient(ItemID.Sapphire, 5).
                AddIngredient(ItemID.ManaCrystal, 3).
                AddTile(TileID.Anvils).
                Register();
        }

    }

    public class WandOfExplodingHeldProj : ModProjectile
    {
        int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        //How far away the projectile will be held by the player
        float offsetAmount = 0f;

        //The progress of the fade in animation (1f = done)
        float fadeInProgress = 0f;

        float recoilProg = 0f; //1f = most recoil

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region startAnim
            //Starting Animation
            float rotationBonus = 0f;
            if (fadeInProgress < 1f)
            {
                int timeForFadeInAnim = 20;

                fadeInProgress = (float)timer / timeForFadeInAnim;

                float rotationEaseValue = Easings.easeOutCubic(fadeInProgress);
                rotationBonus = MathHelper.Lerp(MathHelper.TwoPi * -2.5f * player.direction, 0f, rotationEaseValue);

                float offsetEaseValue = Easings.easeOutSine(fadeInProgress);
                offsetAmount = MathHelper.Lerp(-10f, 28f, offsetEaseValue);

                float scaleEaseValue = Easings.easeInOutBack(fadeInProgress, 0f, 2f);
                overallScale = MathHelper.Lerp(0.6f, 1f, scaleEaseValue);

                float alphaEaseValue = Easings.easeOutQuart(fadeInProgress);
                overallAlpha = alphaEaseValue;

                //Play boomerang spin sound
                if (timer % 7 == 0)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Item_7") with { Pitch = .45f, PitchVariance = 0.2f }; 
                    SoundEngine.PlaySound(style, Projectile.Center);
                    SoundEngine.PlaySound(style, Projectile.Center);
                }

            }
            #endregion

            recoilProg = Math.Clamp(recoilProg- 0.05f, 0f, 1f);

            //Held Proj Code
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            if (!player.channel)
                Projectile.active = false;

            Projectile.velocity = Vector2.Zero;

            Vector2 mousePos = Vector2.Zero;
            if (Projectile.owner == Main.myPlayer)
                mousePos = Main.MouseWorld;

            float rotDir = (mousePos - player.Center).ToRotation();

            //Recoil = move back for 15% of the duration, ease back in for the other 85%
            float recoilOffset = 0f;
            if (recoilProg > 0.85f)
            {
                float recoilLerp = Utils.GetLerpValue(1f, 0.85f, recoilProg, true);
                recoilOffset = MathHelper.Lerp(0f, -14f, Easings.easeOutCubic(recoilLerp)); //-10f
            }
            else
            {
                float recoilLerp = Utils.GetLerpValue(0.85f, 0f, recoilProg, true);
                recoilOffset = MathHelper.Lerp(-14f, 0f, Easings.easeInCubic(recoilLerp)); //-10f
            }

            Projectile.Center = player.MountedCenter + rotDir.ToRotationVector2() * (offsetAmount + recoilOffset);
            Projectile.rotation = rotDir + rotationBonus;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(mousePos.X < player.Center.X ? -1 : 1);
            //player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);

            //Use this if you are not doing composite arms
            //player.itemRotation = MathHelper.WrapAngle(rotDir + (player.direction != 1 ? -3.14f : 0f));

            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.timeLeft = 2;


            float armEase = Easings.easeOutQuad(fadeInProgress);
            if (armEase > 0.75f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);
            else if (armEase > 0.5f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotDir - MathHelper.PiOver2);
            else if (armEase > 0.25f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, rotDir - MathHelper.PiOver2);
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, rotDir - MathHelper.PiOver2);

            //Fire Bolt
            if (timer % 40 == 0 && timer != 0 && fadeInProgress == 1f)
            {
                Vector2 normDir = rotDir.ToRotationVector2();

                //Sound
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_109") with { Volume = 0.5f, Pitch = 0.75f, PitchVariance = 0.15f };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_book_staff_cast_0") with { Volume = 0.3f, PitchVariance = 0.1f, };
                SoundEngine.PlaySound(style2, Projectile.Center);

                //Bolt Projectile
                Vector2 vel = new Vector2(17, 0).RotatedBy(rotDir);
                int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<WandOfExplodingBolt>(),
                    Projectile.damage, 0, Main.myPlayer);

                //GPA Dust
                for (int fg = 0; fg < 2 + Main.rand.Next(2); fg++)
                {
                    Vector2 dir = vel.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * Main.rand.NextFloat(0.3f, 1.35f) * 2.5f;

                    Dust gd = Dust.NewDustPerfect(Projectile.Center + normDir * 10f, ModContent.DustType<GlowPixelAlts>(), dir, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.6f) * 0.4f);
                    gd.velocity += vel * 0.1f;
                }

                //Cross Dust
                int crossCount = 3 + Main.rand.Next(2);
                for (int i = 0; i < crossCount; i++)
                {
                    float prog = (float)i / (float)crossCount;

                    Vector2 dustVel = rotDir.ToRotationVector2() * MathHelper.Lerp(2.5f, 7f, prog);
                    dustVel = dustVel.RotatedByRandom(0.5f);

                    Color middleBlue = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, 0.25f + Main.rand.NextFloat(-0.15f, 0.15f));

                    Dust gd = Dust.NewDustPerfect(Projectile.Center + normDir * 10f, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: middleBlue, Scale: Main.rand.NextFloat(0.25f, 0.45f));
                    gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.15f, timeBeforeSlow: 5,
                        preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
                }

                //Circle Pulse
                Dust d2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CirclePulse>(), normDir * 2f, newColor: Color.Lerp(Color.DodgerBlue, Color.Blue, 0.15f));
                CirclePulseBehavior b2 = new CirclePulseBehavior(0.25f, true, 6, 0.2f, 0.4f);
                b2.drawLayer = RenderLayer.UnderProjectiles;
                d2.customData = b2;
                d2.scale = 0.25f * 0.15f;

                //Deactivate Proj if we are out of mana
                if (!player.CheckMana(player.inventory[player.selectedItem], pay: true))
                    Projectile.active = false;

                //Start recoiling
                recoilProg = 1f;
            }

            Vector2 lightPos = Projectile.Center + (rotDir.ToRotationVector2() * offsetAmount);
            Lighting.AddLight(lightPos, Color.SkyBlue.ToVector3() * overallAlpha * 0.4f);

            timer++;
        }


        float overallAlpha = 1f;
        float overallScale = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/WandOfExploding").Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/WandOfExplodingGlowmask").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY);

            //Twirl
            if (fadeInProgress < 1f)
            {
                Texture2D Twirl = CommonTextures.PixelSwirl.Value;
                float twirlAlpha = Easings.easeInQuad(Utils.GetLerpValue(1f, 0.25f, fadeInProgress, true));

                Main.spriteBatch.Draw(Twirl, drawPos, null, Color.SaddleBrown * 0.5f * overallAlpha * twirlAlpha, Projectile.rotation + MathHelper.PiOver2, Twirl.Size() / 2, Projectile.scale * overallScale * 0.7f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Twirl, drawPos, null, Color.SaddleBrown * 0.5f * overallAlpha * twirlAlpha, Projectile.rotation, Twirl.Size() / 2, Projectile.scale * overallScale * 0.4f, SpriteEffects.None, 0f);
            }

            //Main Texture + Glowmask
            Vector2 origin = texture.Size() / 2f;
            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float extraRot = player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver4 * -1; //Sprite is diagonal, so make it straight

            Main.spriteBatch.Draw(texture, drawPos, null, lightColor * overallAlpha, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);
            Main.spriteBatch.Draw(glowMask, drawPos, null, Color.White * overallAlpha, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);

            //Star
            Texture2D star = CommonTextures.RainbowRod.Value;

            Vector2 starPos = drawPos + Projectile.rotation.ToRotationVector2() * 18f;
            float starRot = Projectile.rotation + MathHelper.Lerp(5f * player.direction, 0f, Easings.easeInCirc(recoilProg));
            float starScale = MathHelper.Lerp(0f, 1f, Easings.easeOutQuad(Utils.GetLerpValue(0f, 0.5f, recoilProg, true))) * 1.2f;
            float starAlpha = 1f * starScale;

            Main.spriteBatch.Draw(star, starPos, null, Color.DodgerBlue with { A = 0 } * starAlpha, starRot, star.Size() / 2f, Projectile.scale * overallScale * 0.5f * starScale, SE, 0.0f);
            Main.spriteBatch.Draw(star, starPos, null, Color.White with { A = 0 } * starAlpha, starRot, star.Size() / 2f, Projectile.scale * overallScale * 0.25f * starScale, SE, 0.0f);

            //Glorb
            Texture2D glorb = CommonTextures.feather_circle128PMA.Value;
            Main.spriteBatch.Draw(glorb, starPos, null, Color.Blue with { A = 0 } * starAlpha * 0.25f, 0f, glorb.Size() / 2f, Projectile.scale * overallScale * 0.35f * starScale, SE, 0.0f);
            Main.spriteBatch.Draw(glorb, starPos, null, Color.DodgerBlue with { A = 0 } * starAlpha * 0.5f, 0f, glorb.Size() / 2f, Projectile.scale * overallScale * 0.15f * starScale, SE, 0.0f);

            return false;
        }

    }

    public class WandOfExplodingBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (timer > 15)
                Projectile.velocity *= 0.8f;
            else if (timer > 10)
                Projectile.velocity *= 0.99f;
            
            if (timer == 30)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 0.5f * overallAlpha);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            if (timer % 5 == 0)
                Projectile.frame = (Projectile.frame + 1) % 4;


            //Alpha and scale
            overallAlpha = Math.Clamp(MathHelper.Lerp(overallAlpha, 1.25f, 0.06f), 0f, 1f);

            float timeForPopInAnim = 22; //33
            float animProgress = Math.Clamp((timer + 6) / timeForPopInAnim, 0f, 1f);

            overallScale = MathHelper.Lerp(0f, 1f, Easings.easeInOutBack(animProgress, 0f, 1.75f)) * 1f;

            //Trail
            int trailCount = 12; //12
            previousRotations.Add(Projectile.velocity.ToRotation());
            previousPositions.Add(Projectile.Center + Projectile.velocity);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);

            timer++;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Set timer to 20 if we are under it
            timer = Math.Max(timer, 20);
            Projectile.velocity *= 0.5f;
        }

        public override void OnKill(int timeLeft)
        {
            //Dust
            for (int fg = 0; fg < 20; fg++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.6f) * 0.4f);
            }

            for (int i = 0; i < 10; i++)
            {
                var v = Main.rand.NextVector2Unit();
                Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
                    Color.DeepSkyBlue, Main.rand.NextFloat(0.4f, 0.9f));
            }

            //Explosion
            int explo = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WandOfExplodingExplosion>(), (int)(Projectile.damage * 1.25f), 0, Projectile.owner);

            if (Main.player[Projectile.owner].statMana <= Main.player[Projectile.owner].statManaMax2 / 2f)
                SkillStrikeUtil.setSkillStrike(Main.projectile[explo], 1.3f, 100, 0.35f, 0f);

            //Sound
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 0.16f, Pitch = 0.5f };
            SoundEngine.PlaySound(style, Projectile.Center);

        }

        float overallScale = 0f;
        float overallAlpha = 0f;
        List<float> previousRotations = new List<float>();
        List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                DrawTrail(giveUp: false);
            });
            DrawTrail(giveUp: true);

            Texture2D fireball = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/ExplodingBolt").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos += Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3f;

            int frameHeight = fireball.Height / 4;
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, fireball.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            SpriteEffects se = Projectile.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

            float endPower = Utils.GetLerpValue(20, 30, timer, true);
            Vector2 randomOffset = Main.rand.NextVector2Circular(6f, 6f) * endPower;

            //Glowing Border
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(fireball, drawPos + Main.rand.NextVector2Circular(2f, 2f) + randomOffset, sourceRectangle, Color.White with { A = 0 } * overallAlpha, Projectile.rotation, origin, 1.05f * Projectile.scale * overallScale, se);
            }

            //Main Tex
            Main.EntitySpriteDraw(fireball, drawPos + randomOffset, sourceRectangle, Color.White * overallAlpha, Projectile.rotation, origin, Projectile.scale * overallScale, se);

            return false;
        }

        Effect myEffect = null;
        public void DrawTrail(bool giveUp)
        {
            if (giveUp)
                return;

            #region orb
            //Glorb
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos += Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3f;

            Texture2D orb = CommonTextures.feather_circle128PMA.Value;
            Color[] cols = { Color.DeepSkyBlue * 0.75f, Color.DodgerBlue * 0.525f, Color.Blue * 0.375f };
            float[] scales = { 1.15f, 1.6f, 2.5f };

            float orbRot = Projectile.velocity.ToRotation();
            float orbAlpha = 0.8f * overallAlpha;
            Vector2 orbScale = new Vector2(0.85f, 0.55f) * 0.3f * Projectile.scale * overallScale;
            Vector2 orbOrigin = orb.Size() / 2f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

            Main.EntitySpriteDraw(orb, drawPos, null, cols[0] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[0], SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, cols[1] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[1] * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, cols[2] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[2] * sineScale2, SpriteEffects.None);
            #endregion

            //Trail
            Texture2D trailTextureUnder = Mod.Assets.Request<Texture2D>("Assets/Trails/EvenThinnerGlowLine").Value;
            Texture2D trailTextureOver = Mod.Assets.Request<Texture2D>("Assets/Trails/EvenThinnerGlowLine").Value;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;

            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = previousPositions.ToArray();
            float[] rot_arr = previousRotations.ToArray();

            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.09f) * 0.15f;

            Color StripColor(float progress) => Color.White * (progress * progress * progress);
            float StripWidthUnder(float progress) => 40f * Easings.easeOutQuad(progress) * overallScale * sineWidthMult;
            float StripWidthOver(float progress) => 12f * Easings.easeOutQuad(progress) * overallScale * sineWidthMult;

            VertexStrip vertexStripUnder = new VertexStrip();
            vertexStripUnder.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidthUnder, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStripOver = new VertexStrip();
            vertexStripOver.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidthOver, -Main.screenPosition, includeBacksides: true);

            #region Trail Params + Drawing
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue(timer * 0.05f);
            myEffect.Parameters["reps"].SetValue(1f);

            //UnderLayer
            myEffect.Parameters["TrailTexture"].SetValue(trailTextureUnder);
            myEffect.Parameters["ColorOne"].SetValue(Color.Lerp(Color.DeepSkyBlue, Color.DodgerBlue, 0.3f).ToVector3() * 1f);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripUnder.DrawTrail();
            vertexStripUnder.DrawTrail();


            //Over layer
            Color overCol = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.75f);
            myEffect.Parameters["TrailTexture"].SetValue(trailTextureOver);
            myEffect.Parameters["ColorOne"].SetValue(overCol.ToVector3() * 1f);
            myEffect.Parameters["glowThreshold"].SetValue(0.7f); //0.6
            myEffect.Parameters["glowIntensity"].SetValue(2f); //2.25
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripOver.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            #endregion

            #region SolidTrail(good) use for fibber 
            /*
            //Trail
            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/EasySwipeTrail").Value;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("VFXPlus/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;

            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = previousPositions.ToArray();
            float[] rot_arr = previousRotations.ToArray();

            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.09f) * 0.15f;

            Color StripColor(float progress) => Color.White * (progress * progress);
            float StripWidthUnder(float progress) => 20f * Easings.easeOutCubic(progress) * overallScale * sineWidthMult;
            float StripWidthOver(float progress) => 8f * Easings.easeOutCubic(progress) * overallScale * sineWidthMult;

            VertexStrip vertexStripUnder = new VertexStrip();
            vertexStripUnder.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidthUnder, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStripOver = new VertexStrip();
            vertexStripOver.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidthOver, -Main.screenPosition, includeBacksides: true);



            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue(timer * 0.05f * 0f);
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["reps"].SetValue(1f);

            //UnderLayer
            myEffect.Parameters["ColorOne"].SetValue(Color.Lerp(Color.DodgerBlue, Color.Blue, 0.5f).ToVector3() * 1f);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripUnder.DrawTrail();


            //Over layer
            myEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 1f);
            myEffect.Parameters["glowThreshold"].SetValue(0.7f); //0.6
            myEffect.Parameters["glowIntensity"].SetValue(2f); //2.25
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripOver.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            */
            #endregion
        }
    }

    public class WandOfExplodingExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() { return timer < 4; }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ManaLeech>(), 300);
        }

        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 6)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 1f);

            int timeForFadeInAnim = 15;

            float fadeInProgress = Math.Clamp((float)timer / timeForFadeInAnim, 0f, 1f);

            float scaleEaseValue = Easings.easeInOutHarsh(fadeInProgress);
            overallScale = MathHelper.Lerp(0.5f, 1f, scaleEaseValue);

            timer++;
        }

        public float overallAlpha = 1f;
        public float overallScale = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            //Orb
            Texture2D orb = CommonTextures.feather_circle128PMA.Value;
            Color[] cols = { Color.DeepSkyBlue * 0.75f, Color.DodgerBlue * 0.525f, Color.Blue * 0.375f };
            float[] scales = { 1.15f, 1.6f, 2.5f };

            float orbRot = Projectile.velocity.ToRotation();
            float orbAlpha = 0.1f * overallAlpha;
            float orbScale = 1.5f * Projectile.scale * overallScale;
            Vector2 orbOrigin = orb.Size() / 2f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

            Main.EntitySpriteDraw(orb, drawPos + new Vector2(0f, 0f), null, cols[0] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[0], SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos + new Vector2(0f, 0f), null, cols[1] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[1] * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos + new Vector2(0f, 0f), null, cols[2] with { A = 0 } * orbAlpha, orbRot, orbOrigin, orbScale * scales[2] * sineScale2, SpriteEffects.None);


            //Explo
            Texture2D Explo = Mod.Assets.Request<Texture2D>("Assets/Anim/BlueFlareDarkGlowPMA").Value;
            int frameHeight = Explo.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Explo.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            float drawScale = Projectile.scale * overallScale * 1.25f;

            Main.spriteBatch.Draw(Explo, drawPos, sourceRectangle, Color.Black * 0.4f, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Explo, drawPos, sourceRectangle, Color.DeepSkyBlue with { A = 0 } * 0.2f, Projectile.rotation, origin, drawScale * 1.15f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Explo, drawPos, sourceRectangle, Color.White with { A = 0 }, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
