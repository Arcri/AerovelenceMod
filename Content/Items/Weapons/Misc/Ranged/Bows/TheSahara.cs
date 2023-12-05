using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Bows
{
	public class TheSahara : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sahara");
			// Tooltip.SetDefault("TODO");
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.rare = ItemRarityID.Orange;

			Item.width = 58;
			Item.height = 20;

			Item.useAnimation = 20;
			Item.useTime = 20;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 15f;
			Item.knockBack = 2f;

			Item.DamageType = DamageClass.Ranged;

			Item.autoReuse = true;
			Item.noMelee = true;

			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.WoodenArrowFriendly;

			Item.useAmmo = AmmoID.Arrow;
			Item.channel = true;
			Item.noUseGraphic = true;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(8, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile proj2 = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ModContent.ProjectileType<TheSaharaHeldProj>(), damage, 0, player.whoAmI);

			if (proj2.ModProjectile is TheSaharaHeldProj wb)
            {
				wb.projToShootID = type;
            }

			return false;
        }

    }

    public class TheSaharaHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sahara");
        }

        public int OFFSET = 10; //15
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        int timer = 0;

        int timeBeforeKill = 0; //once the player lets go, this will start ticking up and the proj will die after a certain time

        float percentDrawnBack = 0;

        public bool runOnceCharge = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }


        public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public int projToShootID = ProjectileID.WoodenArrowFriendly;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

                percentDrawnBack = MathHelper.Clamp(percentDrawnBack + 0.04f, 0f, 1f);
                if (timer < 10)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                else if (timer < 20)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (percentDrawnBack >= 1f && runOnceCharge)
                {
                    runOnceCharge = false;
                }
            }
            else
            {
                Vector2 projPosition = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY).Floor();


                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (timeBeforeKill == 0)
                {
                    float vel = MathHelper.Clamp(18f * percentDrawnBack, 3.5f, 18f);

                    float knockBack = 4 * percentDrawnBack;
                    if (timer > 55)
                    {
                        projToShootID = ModContent.ProjectileType<SaharaFireVortex>();
                        vel = 20;
                        knockBack = 0;
                    }

                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, direction.SafeNormalize(Vector2.UnitX) * vel, projToShootID, Projectile.damage / 2 + (int)(Projectile.damage * percentDrawnBack), knockBack, Player.whoAmI);

                    if (projToShootID != ProjectileType<SaharaFireVortex>())
                    {
                        SaharaFlameEffect globalProjectile = proj.GetGlobalProjectile<SaharaFlameEffect>();
                        globalProjectile.trailActive = true;

                    }


                    for (int j = 0; j < 7; j++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * 10, ModContent.DustType<StillDust>(),
                            direction.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(1.5f, 3.2f), newColor: Color.Orange);
                        dust2.scale = 2f;
                        dust2.rotation = Main.rand.NextFloat(6.28f);
                        //Dust dust1 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * vel, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 3),
                        //Color.Orange, Main.rand.NextFloat(0.3f, 0.5f), 0.6f, 0f, dustShader2);
                        //dust1.velocity *= 1f;

                    }

                    int Mura = Projectile.NewProjectile(null, Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * 10, Vector2.Zero, ModContent.ProjectileType<MuraLineHandler>(), 5, 0, Projectile.owner);

                    if (Main.projectile[Mura].ModProjectile is MuraLineHandler mlh)
                    {
                        //mlh.fadeMult = 2f;

                        for (int m = 0; m < 5; m++)
                        {
                            float xScaleMinus = Main.rand.NextFloat(0f, 1.1f);
                            MuraLine newWind = new MuraLine(Main.projectile[Mura].Center, direction.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(2f, 4.2f), 2 - xScaleMinus);
                            newWind.color = Color.OrangeRed;
                            mlh.lines.Add(newWind);
                        }
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 0.75f;
                        Dust dust1 = Dust.NewDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * 10, DustID.Torch, randomStart);
                        dust1.rotation = Main.rand.NextFloat(6.28f);
                        dust1.alpha = 65;
                        dust1.scale = 1.3f;
                        dust1.noGravity = true;
                    }

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_5") with { Pitch = -.53f, };
                    SoundEngine.PlaySound(style2, Player.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .24f, Volume = 0.5f };
                    SoundEngine.PlaySound(style3, Player.Center);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_javelin_throwers_attack_1") with { Pitch = .45f, PitchVariance = 0.1f, Volume = 0.5f + (0.5f * percentDrawnBack) };
                    SoundEngine.PlaySound(style, Player.Center);
                }

                if (timeBeforeKill >= 40)
                    Projectile.active = false;
                timeBeforeKill++;
            }

            //lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);


            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);


            if (timer == 53 && Player.channel)
            {
                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Volume = 0.65f, Pitch = 0.3f }, Projectile.Center);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 0.75f;
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * 14, ModContent.DustType<StillDust>(), randomStart);
                    dust1.rotation = Main.rand.NextFloat(6.28f);
                    dust1.scale = 1.3f;
                    dust1.color = Color.Orange;
                }
            }

            if (timer > 53)
                starScale = Math.Clamp(MathHelper.Lerp(starScale, 1.5f, 0.06f), 0, 1);

            timer++;
            starRotation += 0.04f * Player.direction;
            
        }

        float starRotation = 0;
        float starScale = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Bows/TheSaharaNoString").Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);
            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
            }

            if (Player.channel)
            {
                float extraRot = Player.direction == 1 ? 0 : -3.14f;
                SpriteEffects ef = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                
                //0.5f
                Main.spriteBatch.Draw(texture, position + Main.rand.NextVector2Circular(1f, 1f), null, Color.OrangeRed with { A = 0 } * 1f * percentDrawnBack, direction.ToRotation() + extraRot, origin, Projectile.scale * 1f, ef, 0.0f);

            }


            //Arrow Drawing
            Texture2D arrowTexture = TextureAssets.Projectile[projToShootID].Value;
            //Texture2D arrowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Bows/Wooden_Arrow").Value;
            Vector2 origin2 = new Vector2((float)arrowTexture.Width / 2f, (float)arrowTexture.Height / 2f);
            Vector2 position2 = (Projectile.position - (0.5f * (direction * OFFSET * -1.5f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 chargeOffset = new Vector2(-5 * percentDrawnBack, 0).RotatedBy(direction.ToRotation());

            Vector2 lineOffsetRot1 = new Vector2(-3f, -15f).RotatedBy(Projectile.rotation);
            Vector2 lineOffsetRot2 = new Vector2(-3f, 15f).RotatedBy(Projectile.rotation);

            Vector2 arrowButtPos = position2 + chargeOffset + new Vector2(0, -13).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);

            Color lineColor = Blend(lightColor, Color.SaddleBrown, 0.7f);
            if (Player.channel)
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, arrowButtPos + Main.screenPosition, lineColor, lineColor, 1.8f);
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, arrowButtPos + Main.screenPosition, lineColor, lineColor, 1.8f);
            }
            else
            {
                Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, Projectile.Center + lineOffsetRot2, lineColor, lineColor, 1.3f);
            }

            if (Player.channel)
            {
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, lightColor, direction.ToRotation() - MathHelper.PiOver2 + 3.14f, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, Color.Orange with { A = 0 } * percentDrawnBack * 1f, direction.ToRotation() - MathHelper.PiOver2 + 3.14f, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            }
            if (Player.channel && timer > 55)
            {
                Texture2D shineTexture = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Starlight").Value;

                //Main.spriteBatch.Draw(shineTexture, position + direction * 9, null, new Color(0, 0, 0) * 0.4f, starRotation, shineTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(shineTexture, position + direction * 9, null, new Color(200, 100, 20, 0) * 1f, starRotation + MathHelper.PiOver2, shineTexture.Size() / 2, Projectile.scale * 0.6f * starScale, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(shineTexture, position + direction * 9, null, new Color(200, 100, 20, 0) * 1f, starRotation, shineTexture.Size() / 2, Projectile.scale * 0.6f * starScale, SpriteEffects.None, 0.0f);

            }


            return false;
        }

        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// �on top of� <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public Color Blend(Color myColor, Color backColor, double amount)
        {
            byte r = (byte)(myColor.R * amount + backColor.R * (1 - amount));
            byte g = (byte)(myColor.G * amount + backColor.G * (1 - amount));
            byte b = (byte)(myColor.B * amount + backColor.B * (1 - amount));
            return new Color(r, g, b);
        }

    }

    public class SaharaFireVortex : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FireWhirl");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.timeLeft = 135; //150
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor) { return Color.White; }
        int timer = 0;
        float justPulsedVal = 0f; 

        public override void AI()
        {
            Projectile.rotation = 0;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            Projectile.velocity *= timer > 50 ? 0.9f : 0.95f;

            if (timer == 65 || timer == 77 || timer == 89)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SaharaFirePulse>(), Projectile.damage, 0, Main.myPlayer);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_0") with { Pitch = -.33f, MaxInstances = -1, Volume = 0.8f };
                SoundEngine.PlaySound(style, Projectile.Center);
                justPulsedVal = 1f;

                for (int i = 0; i < 8; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2Circular(3f, 3f) * 2f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<GlowPixelCross>(), randomStart, newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.55f, 0.85f));

                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: false);
                }
            }
            justPulsedVal *= 0.8f;

            if (Projectile.timeLeft <= 20)
            {
                float progress = 1f - (Projectile.timeLeft / 20f);
                Projectile.scale = MathHelper.Lerp(1f, 0f, Easings.easeInBack(progress));
            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/feather_circle128PMA");
            Texture2D Tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Bows/SaharaFireVortex");
            Texture2D White = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Bows/SaharaFireVortexWhite");

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * 0.7f, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.7f, 0, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, 0, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White with { A = 0 } * 0.25f, Projectile.rotation, origin, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(White, Projectile.Center - Main.screenPosition, sourceRectangle, Color.OrangeRed with { A = 0 } * ((2f * justPulsedVal) + 0f), Projectile.rotation, origin, 1.5f * (1f - justPulsedVal), 0, 0f);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_book_staff_cast_2") with { Pitch = -0.2f, PitchVariance = 0.35f, Volume = 0.6f };
            SoundEngine.PlaySound(style, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 2f;
                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, randomStart * Main.rand.NextFloat(0.9f, 1.5f));
                dust1.rotation = Main.rand.NextFloat(6.28f);
                dust1.scale = 1.5f;
                dust1.color = Color.Orange;
                dust1.noGravity = true;
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(3f, 3f) * 1.5f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<GlowPixelCross>(), randomStart, newColor: new Color(255, 100, 5), Scale: Main.rand.NextFloat(0.55f, 0.85f));

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: false);
            }
        }
    }

    public class SaharaFirePulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        float opacity = 1f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hollow Pulse");

        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        Vector2 startingCenter;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer == 0)
                startingCenter = Projectile.Center;

            timer++;

            Projectile.scale = MathHelper.Clamp(MathHelper.Lerp(Projectile.scale, 1.25f, 0.08f), 0f, 1f);

            if (Projectile.scale >= 0.8f)
                opacity = MathHelper.Clamp(MathHelper.Lerp(opacity, -0.2f, 0.15f), 0, 2);

            if (opacity <= 0)
                Projectile.active = false;

            Projectile.width = (int)(190 * Projectile.scale);
            Projectile.height = (int)(190 * Projectile.scale);
            Projectile.Center = startingCenter;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/Royal_Resonance").Value;

            
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1,1,0,0), new Color(255, 130, 0) with { A = 0 } * opacity, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.OrangeRed with { A = 0 } * opacity, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 1.5f + 0.15f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), new Color(255, 130, 0) with { A = 0 } * opacity, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.OrangeRed with { A = 0 } * opacity, Projectile.rotation, Tex.Size() / 2, Projectile.scale * 1.5f + 0.15f, SpriteEffects.None, 0f);

            
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 100);
        }
    }

    //Special VFX for arrows show by the weapon
    public class SaharaFlameEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool trailActive = false;
        public float trailType = 1;
        public float trailIntensity = 1f;

        private BaseTrailInfo fireTrail = new BaseTrailInfo();
        public List<Vector2> previousPositions = new List<Vector2>();
        int timer = 0;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!trailActive)
            {
                base.OnHitNPC(projectile, target, hit, damageDone);
                return;
            }

            for (int i = 0; i < 6; i++)
            {
                float arrowVel = Math.Clamp(projectile.velocity.Length(), 5f, 11f);
                Vector2 randomStart = Main.rand.NextVector2Circular(3f, 3f) * 1f;
                Dust dust = Dust.NewDustPerfect(projectile.Center, DustType<GlowPixelCross>(), randomStart, newColor: new Color(255, 100, 5), Scale: Main.rand.NextFloat(0.6f, 0.7f));
                dust.velocity += projectile.velocity.SafeNormalize(Vector2.UnitX) * arrowVel * 0.5f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.97f, timeBeforeSlow: 6, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: false);
            }
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void PostAI(Projectile projectile)
        {
            if (!trailActive) return;
            fireTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            fireTrail.trailColor = new Color(255, 100, 5);
            fireTrail.trailPointLimit = (int)(120 * projectile.scale);
            fireTrail.trailWidth = (int)(20 * projectile.scale);
            fireTrail.trailMaxLength = (int)(120 * projectile.scale); //225
            fireTrail.timesToDraw = 0;
            fireTrail.pinch = true;
            fireTrail.pinchAmount = 0.4f;
            
            fireTrail.trailTime = (float)Main.timeForVisualEffects * 0.05f;
            fireTrail.trailRot = projectile.velocity.ToRotation();
            fireTrail.trailPos = projectile.Center + projectile.velocity;
            fireTrail.TrailLogic();

            if (timer % 1 == 0)
            {
                previousPositions.Add(projectile.Center);

                int trailCount = 10;
                if (previousPositions.Count > trailCount)
                {
                    previousPositions.RemoveAt(0);
                }
            }

            timer++;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!trailActive) return base.PreDraw(projectile, ref lightColor);

            fireTrail.TrailDrawing(Main.spriteBatch);

            Vector2 scale = new Vector2(0.5f, 1f);

            return false;
            //return base.PreDraw(projectile, ref lightColor);
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (!trailActive)
            {
                base.PostDraw(projectile, lightColor);
                return;
            }
            Vector2 scale = new Vector2(0.25f, 0.5f) * 0.5f;

            Texture2D arrowTex = TextureAssets.Projectile[projectile.type].Value;
            Texture2D arrowWhite = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Bows/WoodenArrowWhiteGlow").Value;

            for (int i = 1; i < previousPositions.Count; i++)
            {
                float progress = (float)i / previousPositions.Count;
                float size = projectile.scale * progress;
                //size *= (0.75f + (progress * 0.25f));

                Vector2 vec2Scale = new Vector2(1f * progress, 1f) * projectile.scale;
                Vector2 vec2Scale2 = new Vector2(0.5f * progress, 1f) * projectile.scale;

                Main.EntitySpriteDraw(arrowWhite, previousPositions[i] - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * progress * 0.35f, projectile.rotation + 3.14f, arrowWhite.Size() / 2, vec2Scale, SpriteEffects.None);
                //Main.EntitySpriteDraw(arrowWhite, previousPositions[i] - Main.screenPosition, null, Color.Orange with { A = 0 } * Easings.easeInSine(progress) * 0.15f, projectile.rotation + 3.14f, arrowWhite.Size() / 2, vec2Scale2, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(arrowTex, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, projectile.rotation, arrowTex.Size() / 2, projectile.scale * 1.1f, SpriteEffects.None);
            Main.EntitySpriteDraw(arrowTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, arrowTex.Size() / 2, projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(arrowTex, projectile.Center - Main.screenPosition, null, Color.Orange with { A = 0 }, projectile.rotation, arrowTex.Size() / 2, projectile.scale, SpriteEffects.None);



            Texture2D spike = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/DiamondGlowPMA").Value;
            Main.EntitySpriteDraw(spike, projectile.Center - Main.screenPosition + projectile.velocity.SafeNormalize(Vector2.UnitX) * -2f, null, Color.Orange with { A = 0 }, projectile.rotation, spike.Size() / 2, projectile.scale * scale, SpriteEffects.None);
            Main.EntitySpriteDraw(spike, projectile.Center - Main.screenPosition + projectile.velocity.SafeNormalize(Vector2.UnitX) * -2f, null, Color.White with { A = 0 }, projectile.rotation, spike.Size() / 2, projectile.scale * 0.5f * scale, SpriteEffects.None);
            
            
            base.PostDraw(projectile, lightColor);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (trailActive)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2Circular(2f, 2f) * 1f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center, DustType<GlowPixelCross>(), randomStart, newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.55f, 0.7f));
                    dust.velocity += projectile.velocity * 0.15f;

                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
                }
            }
            base.OnKill(projectile, timeLeft);
        }
    }
}