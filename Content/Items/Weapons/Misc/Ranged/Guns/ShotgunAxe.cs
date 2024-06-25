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
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;
using Terraria.Map;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{
    public class ShotgunAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.knockBack = 6f; //A bit above average

            Item.width = 78;
            Item.height = 32;

            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarities.EarlyHardmode;
            Item.value = Item.sellPrice(0, 5, 0, 0);

            Item.shootSpeed = 20f;
            Item.shoot = ProjectileID.Ale; //Need this for shoot() to activate
            Item.useAmmo = AmmoID.Bullet;

            Item.autoReuse = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

        }
        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Bullets Skill Strike gouged enemies [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                float ai0 = player.direction == 1 ? 0 : 1;
                Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<ShotgunAxeMeleeProj>(), (int)(damage * 1.5f), (int)(knockback * 1.5f), player.whoAmI, ai0: ai0);
                return false;
            }

            player.GetModPlayer<AeroPlayer>().ScreenShakePower = 1.5f;

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40f;
            muzzleOffset += new Vector2(0, -10 * player.direction).RotatedBy(velocity.ToRotation());
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            for (int m = 0; m < 8; m++) // m < 9
            {
                Dust d = Dust.NewDustPerfect(position, ModContent.DustType<GlowStrong>(),
                    velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 0.1f * (Main.rand.NextFloat(4) + 1), newColor: Color.Crimson, Scale: 0.2f + Main.rand.NextFloat(0, 0.2f));
                d.alpha = 2; //Dust color will fade out
            }


            //lol
            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/back_scatter") with { Volume = .055f, Pitch = .6f, PitchVariance = 0.25f };
            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/rescue_ranger_fire") with { Volume = .12f, Pitch = .4f, };
            SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Impact_Sword_L_a") with { Volume = .18f, Pitch = .6f, PitchVariance = 0.25f };
            SoundStyle style4 = new SoundStyle("Terraria/Sounds/Item_71") with { Pitch = -.51f, PitchVariance = .25f, Volume = 0.3f };
            SoundEngine.PlaySound(style, player.Center);
            SoundEngine.PlaySound(style2, player.Center);
            SoundEngine.PlaySound(style3, player.Center);
            SoundEngine.PlaySound(style4, player.Center);

            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShotgunAxeHeldProj>(), 0, 0, player.whoAmI);

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ShotgunAxeBullet>(), (int)(damage * 0.35f), 0, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<ShotgunAxeBullet>(), (int)(damage * 0.35f), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<ShotgunAxeBullet>(), (int)(damage * 0.35f), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.85f, 1f), ModContent.ProjectileType<ShotgunAxeBullet>(), (int)(damage * 0.35f), knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Shotgun, 1).
                AddIngredient(ItemID.AdamantiteWaraxe, 1).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.Shotgun, 1).
                AddIngredient(ItemID.TitaniumWaraxe, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }

    public class ShotgunAxeHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public float OFFSET = 20; //30

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 30;

            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer && timer == 0)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            if (timer > 1)
            {
                if (hasReachedDestination == false)
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, 1f, 0.24f), 0, 0.6f);
                else
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.6f);

            }

            if (lerpToStuff == 0.6f)
            {
                hasReachedDestination = true;
            }

            if (timer == 0)
            {
                OFFSET = 0f;
            }
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 25f, 0.07f), 0, 20);

            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();

            if (timer == 15)
                muzzleFlashAlpha = 0;

            muzzleFlashAlpha = Math.Clamp(MathHelper.Lerp(muzzleFlashAlpha, -0.2f, 0.1f) , 0, 1);

            timer++;
        }

        float muzzleFlashAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/ShotgunAxe");
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(Weapon, Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY), null, lightColor, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);

            //MuzzleFlash
            Texture2D Flash = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/CrispStar");

            Vector2 offset = new Vector2(Weapon.Width / 1.25f * Player.direction, (-Weapon.Height / 2)).RotatedBy(Projectile.rotation) * Player.direction;
            Vector2 offsetOffset = new Vector2(-22,8 * Player.direction).RotatedBy(Projectile.rotation) + new Vector2(0f, Player.gfxOffY);

            Vector2 vec2Scale = new Vector2(0.45f, 0.45f) * Projectile.scale;
            Vector2 origin = new Vector2(0f, Flash.Height / 2);


            float starRot = (float)Main.timeForVisualEffects * 0.1f * Player.direction;

            Main.spriteBatch.Draw(Flash, Projectile.Center - Main.screenPosition + offset + offsetOffset, null, Color.Red with { A = 0 } * 1f * muzzleFlashAlpha, starRot, Flash.Size() / 2, vec2Scale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flash, Projectile.Center - Main.screenPosition + offset + offsetOffset, null, Color.Red with { A = 0 } * 1f * muzzleFlashAlpha, starRot, Flash.Size() / 2, vec2Scale * 0.6f, SpriteEffects.FlipVertically, 0f);

            Main.spriteBatch.Draw(Flash, Projectile.Center - Main.screenPosition + offset + offsetOffset, null, Color.White with { A = 0 } * muzzleFlashAlpha, starRot, Flash.Size() / 2, vec2Scale * 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flash, Projectile.Center - Main.screenPosition + offset + offsetOffset, null, Color.White with { A = 0 } * muzzleFlashAlpha, starRot, Flash.Size() / 2, vec2Scale * 0.25f, SpriteEffects.FlipVertically, 0f);


            return false;
        }
    }

    public class ShotgunAxeBullet : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;

            Projectile.extraUpdates = 2;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; 
        }

        int timer = 0;
        int trailPoints = 300;

        float alpha = 1;
        public override bool? CanDamage()
        {
            return timer < 50 && !justHit;
        }

        float storedRot = 0f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            justHit = true;
        }

        bool justHit = false;
        float justHitTimer = 4;

        public override void AI()
        {

            if (!justHit)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                trailColor = new Color(255, 10, 10);
                trailTime = timer * 0.02f;

                trailPointLimit = 120;
                trailWidth = 20;
                trailMaxLength = trailPoints;

                trailRot = Projectile.velocity.ToRotation();
                trailPos = Projectile.Center + Projectile.velocity;

                TrailLogic();

                Lighting.AddLight(Projectile.position, Color.Red.ToVector3() * 0.45f);

                if (timer > 10)
                {
                    Projectile.velocity *= 0.96f;
                    trailPoints = (int)Math.Clamp(MathHelper.Lerp(trailPoints, -0.2f, 0.07f), 0f, 300f);

                    if (timer > 40)
                    {
                        alpha = MathHelper.Lerp(alpha, 0, 0.08f);
                    }
                }
                if (trailPoints <= 5)
                    Projectile.active = false;

                timer++;
            }
            else
            {
                justHitTimer--;

                trailColor = Color.Lerp(Color.White, Color.Crimson, 0.8f);

                if (justHitTimer == 0)
                {
                    Projectile.Kill();
                    Projectile.active = false;
                }
            }

            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Nightglow").Value;
            Vector2 scale = new Vector2(Projectile.scale * 2, Projectile.scale) * 0.5f;

            Color col = justHit ? Color.White : Color.Red;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), col * 2f * alpha, Projectile.rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition, Tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Tex.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.5f;

        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = -0.5f, PitchVariance = .28f, MaxInstances = 4, Volume = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            Collision.HitTiles(Projectile.position + (Projectile.velocity * 0.5f), Projectile.velocity * 0.5f, Projectile.width, Projectile.height);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 2; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    Color.Red, Main.rand.NextFloat(0.35f, 0.55f), 0.4f, 0f, dustShader);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<ShotgunAxeDebuff>()))
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 2 + (Main.rand.NextBool() ? 1 : 0); i++)
                {
                    int p = GlowDustHelper.DrawGlowDust(target.position, target.width, target.height, ModContent.DustType<GlowCircleQuadStar>(), Color.Crimson,
                        Main.rand.NextFloat(0.4f, 0.9f), 0.4f, 0f, dustShader);
                    Main.dust[p].velocity *= 2.6f;
                    Main.dust[p].fadeIn = 2f;
                }

                for (int i = 0; i < 7; i++)
                {
                    int p = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 0, new Color(255, 255, 255), 1.15f);
                    Main.dust[p].velocity *= 1.5f;
                }

                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, impactVolume: 0.35f);
            }

        }
    }

    public class ShotgunAxeMeleeProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shotgun Axe");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            
            Projectile.width = Projectile.height = 70;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.ownerHitCheck = true;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;

        float Angle = 0;

        bool playedSound = false;

        float storedDirection = 1;

        int timer = 0;

        bool extraDelay = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Projectile.Center - (player.Center)).ToRotation();
            }

            if (firstFrame)
                storedDirection = player.direction;

            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = Angle + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion
            player.heldProj = Projectile.whoAmI;
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {

                //easingProgress = 0.15f;
                timer = 0;
                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);

                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the slate set. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    startingAng = startingAng - MathHelper.ToRadians(-170);
                }
                else
                {
                    startingAng = startingAng + MathHelper.ToRadians(-170);
                }

                currentAng = startingAng;
                firstFrame = false;
            }


            if (timer >= 5 && justHitTime <= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians(340 * getProgress(easingProgress));
                else
                    currentAng = startingAng + MathHelper.ToRadians(340 * getProgress(easingProgress));

                easingProgress = Math.Clamp(easingProgress + 0.015f * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 0.05f, 0.95f);
            }


            Projectile.rotation = currentAng;
            Projectile.Center = (currentAng.ToRotationVector2() * 50) + player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0,-10);
            player.itemTime = 10;
            player.itemAnimation = 10;

            justHitTime--;
            timer++;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Heavy_M_a") with { Pitch = - 0.25f, PitchVariance = 0.3f, Volume = 0.37f }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = -.42f, Volume = 0.27f };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundEngine.PlaySound(SoundID.Item71 with { Volume = 0.87f, Pitch = -0.35f, PitchVariance = 0.45f }, Projectile.Center);

                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f)
            {
                player.itemTime = 0;
                player.itemAnimation = 0;
                Projectile.active = false;

            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/ShotgunAxe");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/ShotgunAxeGlow");
            Texture2D BladeGlow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/ShotgunAxeBladeGlow");


            Vector2 drawPos = Projectile.Center - Main.screenPosition;// - new Vector2(0f, -Main.player[Projectile.owner].gfxOffY);

            if (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f)
            {
                Main.spriteBatch.Draw(BladeGlow, drawPos, null, Color.Red with { A = 0 } * 0.8f, Projectile.rotation, BladeGlow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f) + 0f, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                
                if (justHitTime > 0)
                    Main.spriteBatch.Draw(BladeGlow, drawPos, null, Color.Crimson with { A = 0 } * 0.3f, Projectile.rotation, BladeGlow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f) + 0f, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            if (justHitTime > 1)
            {
                Main.spriteBatch.Draw(Glow, drawPos, null, Color.Red with { A = 0 } * 0.4f, Projectile.rotation, Glow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f) + 0f, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            Main.spriteBatch.Draw(Sword, drawPos, null, lightColor, Projectile.rotation, Sword.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            #region easeExpo
            
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;
            

            #endregion;

            #region easeCircle
            
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            }
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            
            #endregion

            #region easeOutBack
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            #endregion
        }

        int justHitTime = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 1; i++)
            {
                int b = Projectile.NewProjectile(null, target.Center, (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(1f, 1.5f), ModContent.ProjectileType<FadeExplosionHighRes>(), 0, 0);
                Main.projectile[b].rotation = Main.rand.NextFloat(6.28f);
                if (Main.projectile[b].ModProjectile is FadeExplosionHighRes explo)
                {
                    explo.color = Color.Crimson;
                    explo.size = 0.5f;
                    explo.multiplier = 10f;
                    explo.colorIntensity = 0.25f; //0.5
                }
            }
            int a = Projectile.NewProjectile(null, target.Center, Vector2.Zero, ModContent.ProjectileType<ShotgunAxeBlood>(), 0, 0, Main.myPlayer);
            Main.projectile[a].rotation = Projectile.rotation;

            for (int i = 0; i < 20; i++)
                Dust.NewDust(target.position, 30, 30, DustID.Blood, 0f, 0f, 0, new Color(255,255,255), 1f);

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/cleaver_hit_06") with { Pitch = 0.36f, PitchVariance = .35f, Volume = 0.3f };
            SoundEngine.PlaySound(style2, target.Center);
            justHitTime = 7;

            target.AddBuff(ModContent.BuffType<ShotgunAxeDebuff>(), 500);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAng.ToRotationVector2() * ((Projectile.Size.Length() * 1.1f) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 50f * Projectile.scale, ref collisionPoint);
        }
    }

    public class ShotgunAxeBlood : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 26;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        

        public override void AI()
        {
            if (Projectile.frame == 0 && Projectile.timeLeft == 200)
            {
                //Hold the first frame for a little bit longer to add more oomf //OMG OOMFIE
                Projectile.frameCounter = 1;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 1)
            {
                if (Projectile.frame == 9)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/BloodHit").Value;
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/SoftGlow64").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + new Vector2(-10, -40).RotatedBy(Projectile.rotation + MathHelper.PiOver2), null, Color.Red with { A = 0 } * 0.3f, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(-10, -40).RotatedBy(Projectile.rotation + MathHelper.PiOver2), sourceRectangle, Color.Red * 0.2f, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(-10, -40).RotatedBy(Projectile.rotation + MathHelper.PiOver2), sourceRectangle, Color.Red, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class ShotgunAxeDebuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gouged");
            // Description.SetDefault("Losing this much blood can't be good, right?");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;

        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ShotgunAxeDebuffGlobalNPC>().BloodDebuff = true;
            timer++;
        }

       
    }

    public class ShotgunAxeDebuffGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool BloodDebuff = false;
        public float DebuffTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<ShotgunAxeDebuff>()))
            {
                BloodDebuff = false;
                DebuffTime = 0;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (BloodDebuff)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                if (DebuffTime % 1 == 0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 0f, 0f, 0, new Color(255, 255, 255), 1f);
                }
                if (DebuffTime % 7 == 0) //else if is intentional
                {
                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleFlare>(), Color.Red, 
                        Main.rand.NextFloat(0.3f, 0.5f), 0.4f, 0f, dustShader);
                }
                DebuffTime++;

            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            /*
            if (BloodDebuff)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                if (projectile.type == ModContent.ProjectileType<ShotgunAxeBullet>())
                {
                    for (int i = 0; i < 2 + (Main.rand.NextBool() ? 1 : 0); i++)
                    {
                        int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleQuadStar>(), Color.Crimson,
                            Main.rand.NextFloat(0.4f, 0.9f), 0.4f, 0f, dustShader);
                        Main.dust[p].velocity *= 2.6f;
                        Main.dust[p].fadeIn = 2f;
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        int p = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 0f, 0f, 0, new Color(255, 255, 255), 1.15f);
                        Main.dust[p].velocity *= 1.5f;
                    }

                    SkillStrikeUtil.setSkillStrike(projectile, 1.3f);

                }
            }
            */
        }
    }
}