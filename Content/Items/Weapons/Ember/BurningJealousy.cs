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
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class BurningJealousy : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Jealousy");
            Tooltip.SetDefault("Hold Right-Click to guard, gaining a large defense boost\nRetailiate with an explosive burst when hit while guarding");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 38;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Purple;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<BurningJealousyHeldProj>();
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            if (player.altFunctionUse == 2) 
                type = ModContent.ProjectileType<BurningJealousyGuard>(); 
            
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }

    public class BurningJealousyHeldProj : TrailProjBase
    {
        public bool bigSwing = false;
        
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Jealousy");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 8;
            //Projectile.scale = 1.5f;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f);
            return shouldDamage;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;

        float Angle = 0;

        bool playedSound = false;

        float storedDirection = 1;

        int timer = 0;

        int timerAfterEnd = 8;
        float mytrailWidth = 65;

        float amountsToSwing = 1f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            if (bigSwing) Projectile.scale = 1.5f;

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
            //if (easingProgress > 0.5)
            //player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - player.Center).ToRotation() - MathHelper.PiOver2);
            //player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {

                easingProgress = bigSwing ? 0f : 0.07f;
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


            if (timer >= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((320 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((320 * getProgress(easingProgress)));

                float advanceSpeed = bigSwing ? 0.0024f : 0.0019f; //0.0024 | 0.0019

                easingProgress = Math.Clamp(easingProgress + advanceSpeed * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 0.05f, 0.95f * amountsToSwing);
            }

            float centerOffset = bigSwing ? 70 : 60;

            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * centerOffset) + player.RotatedRelativePoint(player.MountedCenter);
            player.itemTime = 10;
            player.itemAnimation = 10;

            timer++;

            if (getProgress(easingProgress) >= 0.35f * amountsToSwing && getProgress(easingProgress) <= 0.78f * amountsToSwing)
            {
                Projectile.ai[1] = 1;
            }
            else
                Projectile.ai[1] = 0;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_bug_zap_1") with { Pitch = -.36f, Volume = 0.3f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.2f, PitchVariance = 0.15f, Volume = 0.37f }, Projectile.Center);


                if (bigSwing)
                {
                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/StampAirSwing1") with { Volume = .14f, Pitch = 0, PitchVariance = 0.2f };
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_flameburst_tower_shot_2") with { Pitch = -0.8f, PitchVariance = 0.3f };
                    SoundEngine.PlaySound(style, Projectile.Center);
                } 
                else
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_flameburst_tower_shot_2") with { Pitch = -.48f, PitchVariance = 0.3f };
                    SoundEngine.PlaySound(style, Projectile.Center);
                }


                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f * amountsToSwing)
            {
                if (timerAfterEnd == 0)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    Projectile.active = false;
                }
                timerAfterEnd--;
                
            }

            if (getProgress(easingProgress) >= 0.8)
            {
                mytrailWidth = MathHelper.Lerp(mytrailWidth, 0, 0.015f);
            }

            //Trail
            String assetLocation = bigSwing ? "Trail7" : "Trail5";
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/" + assetLocation).Value;
            trailColor = Color.OrangeRed * (getProgress(easingProgress) >= 0.7f ? (1.2f - getProgress(easingProgress * 0.7f)) : 1.2f);
            trailPointLimit = 800;
            trailMaxLength = 300;
            trailTime = timer * 0.007f;
            timesToDraw = bigSwing ? 1 : 2;

            trailRot = Projectile.rotation + MathHelper.PiOver4;
            trailPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.75f) * 12;
            TrailLogic();

            //Dust
            if (timer % 4 == 0 && getProgress(easingProgress) > 0.3f && getProgress(easingProgress) < 0.85f)
            {
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                int a = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleFlare>(),
                    Color.OrangeRed, 0.55f, 0.35f, 0f, dustShader2);
                Main.dust[a].noGravity = true;
                Main.dust[a].velocity *= 0.5f;
                Main.dust[a].velocity += Projectile.rotation.ToRotationVector2() * 4;
                Main.dust[a].fadeIn = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();

            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousy");
            Main.spriteBatch.Draw(Sword, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Sword.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyGlow");
            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Glow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


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
            /*
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            }
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            */
            #endregion

            #region easeOutBack
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            #endregion
        }

        public override float WidthFunction(float progress)
        {
            //return mytrailWidth;
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * (mytrailWidth / 28) * (bigSwing ? 0.6f : 1); // 0.3f
            
        }

    }

    public class BurningJealousyGuard : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Jealousy");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 4000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        float Angle = 0;

        bool fading = false;
        float fadeCounter = 0;
        float symbolIntensity_Sword = 0;

        Projectile symbol = null;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (symbol == null)
                symbol = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<BlockFX>(), 0, 0, Main.myPlayer);
            else if (symbol.ModProjectile is BlockFX fx) fx.symbolIntensity = symbolIntensity_Sword;

            player.heldProj = Projectile.whoAmI;
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - player.Center).ToRotation();
            }

            player.itemRotation = Angle;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            float xOffset = (player.velocity.X * 0.02f) * player.direction;
            float yOffset = (player.velocity.Y * 0.01f) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (0.2f - MathHelper.PiOver2 + xOffset + yOffset) * Projectile.direction);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;
                Projectile.Kill();
            }
            //Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center + new Vector2(20 * Projectile.direction,0);
            player.itemTime = 10;
            player.itemAnimation = 10;


            if (!Main.mouseRight)
            {
                fading = true;
            }

            if (fading)
            {
                symbolIntensity_Sword = Math.Clamp(MathHelper.Lerp(symbolIntensity_Sword, -0.5f, 0.12f), 0, 1);
                fadeCounter += 0.07f;

                if (fadeCounter >= 1)
                {
                    player.itemTime = 1;
                    player.itemAnimation = 1;
                    player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;

                    if (symbol != null)
                        symbol.active = false;
                    Projectile.active = false;

                }
            }
            else
            {
                if (timer > 7)
                {
                    player.statDefense = player.statDefense + 30;
                    player.GetModPlayer<BurningJealousyPlayer>().gaurding = true;
                }
                symbolIntensity_Sword = Math.Clamp(MathHelper.Lerp(symbolIntensity_Sword, 1.5f, 0.06f), 0, 1);
            }

            if (player.GetModPlayer<BurningJealousyPlayer>().justHit)
            {
                player.GetModPlayer<BurningJealousyPlayer>().justHit = false;
                player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 1.16f, };
                SoundEngine.PlaySound(style, player.Center);
                int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<BurningJealousyPulse>(), Projectile.damage * 2, 0, player.whoAmI);
                if (symbol != null)
                    symbol.active = false;
                Projectile.active = false;
            }
            timer++;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color glowMaskCol = Color.White;
            if (fading)
            {
                lightColor = Color.Lerp(lightColor, Color.Black * 0.3f, fadeCounter);
                glowMaskCol = Color.Lerp(Color.White, Color.Black * 0.3f, fadeCounter);
            }

            Player p = Main.player[Projectile.owner];
            Vector2 offset = p.direction == 1 ? new Vector2(-25f, 15f) : new Vector2(25f,15f);
            float rot = p.direction == 1 ? 2f : -5.1f;

            Vector2 place = p.Center - Main.screenPosition + offset + new Vector2(0, p.velocity.Y * -0.1f);
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousy");
            Main.spriteBatch.Draw(Sword, p.MountedCenter - Main.screenPosition + offset + new Vector2(0, p.velocity.Y * -0.1f), null, lightColor, rot + (p.velocity.X * 0.01f), Sword.Size() / 2, Projectile.scale, p.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);

            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyGlow");
            Main.spriteBatch.Draw(Glow, p.MountedCenter - Main.screenPosition + offset + new Vector2(p.velocity.X * 0.05f, p.velocity.Y * -0.1f), null, glowMaskCol, rot + (p.velocity.X * 0.01f), Glow.Size() / 2, Projectile.scale, p.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }

    public class BlockFX : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Block");
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 4000;
            Projectile.width = Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        public float fadeCounter = 0;
        public float symbolIntensity = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            Projectile.scale = (symbolIntensity);

            timer++;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            float offset = 0;

            if (symbolIntensity == 1)
                offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.15f;

            float offset2 = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.15f;

            Vector2 center = (Projectile.Center - Main.screenPosition);
            Texture2D Symbol = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/BlockSymbol");
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, Color.Black * 0.5f, offset2, Symbol.Size() / 2, Projectile.scale * 1.55f + offset, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //orange red = 255 165 0
            // orange = 255 69 0
            Color ReddishOrange = new Color(255, 100, 0);
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, ReddishOrange, offset2, Symbol.Size() / 2, Projectile.scale * 1.55f + offset, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, Color.Gold, offset2, Symbol.Size() / 2, Projectile.scale * 1.2f + offset, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyGlow");
            //Main.spriteBatch.Draw(Glow, p.Center - Main.screenPosition + offset + new Vector2(0, p.velocity.Y * -0.1f), null, Color.White, rot + (p.velocity.X * 0.01f), Glow.Size() / 2, Projectile.scale, p.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }

    public class BurningJealousyPlayer : ModPlayer
    {
        public bool gaurding = false;
        public bool justHit = false;
        public bool notTile = false;
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (gaurding)
            {
                Player player = Main.player[Main.myPlayer];

                int swingDir = Main.MouseWorld.X < player.Center.X ? 0 : 1;

                Projectile a = Projectile.NewProjectileDirect(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BurningJealousyHeldProj>(), 140, 0, Main.myPlayer, swingDir);
                if (a.ModProjectile is BurningJealousyHeldProj sword)
                {
                    sword.bigSwing = true;
                }
                player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);
                justHit = true;
                gaurding = false;
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (gaurding)
            {

                Player player = Main.player[Main.myPlayer];

                int swingDir = Main.MouseWorld.X > player.Center.X ? 0 : 1;

                Projectile a = Projectile.NewProjectileDirect(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BurningJealousyHeldProj>(), 120, 0, Main.myPlayer, swingDir);
                if (a.ModProjectile is BurningJealousyHeldProj sword)
                {
                    sword.bigSwing = true;
                }
                player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);
                justHit = true;
                gaurding = false;
            }
            
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (gaurding)
            {
                Player player = Main.player[Main.myPlayer];
                player.statDefense = player.statDefense + 30;

                /*
                Projectile a = Projectile.NewProjectileDirect(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BurningJealousyHeldProj>(), 120, 0, Main.myPlayer);
                if (a.ModProjectile is BurningJealousyHeldProj sword)
                {
                    sword.bigSwing = true;
                }
                justHit = true;
                gaurding = false;
                */
            }

            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);
        }
    }

    //Gotta replace/remove this later and move to a different weapon
    public class BurningJealousyExplode : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 0.1f;

            Projectile.timeLeft = 60; //30
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                //Projectile.rotation = Main.rand.NextFloat(6.28f);
            }
            //Projectile.scale = 1f;
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.6f, 0.2f); //1.51
            //Projectile.rotation += 0.06f;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Assets/EnergyBalls/energyball_4").Value;

            Vector2 tTex = new Vector2(Projectile.scale, Projectile.scale);
            //Use bigCircle (Cyvercry/Textures) for caustics and energyBall_9 for eye effect (3draws)
            //GlowConnectorGhost for double circle (3draws)

            //tstar caus, heartOriginal grad, eball4 tex (3draws)

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/tstar").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyBalls/energyball_9").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.08f); //0.04

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin1, tTex, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class BurningJealousyPulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override bool? CanDamage() { return false; }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;

            Projectile.timeLeft = 300;
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        bool firstFrame = true;
        float colorIntensity = 1f;
        float scale2 = 0;
        float scale3 = 0;

        float randomRot = 0;
        public override void AI()
        {
            if (firstFrame)
            {
                randomRot = Main.rand.NextFloat(6.28f);
                //AOE
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 170f)
                    {
                        int Direction = 0;
                        if (Projectile.position.X - Main.npc[i].position.X < 0)
                            Direction = 1;
                        else
                            Direction = -1;
                        Main.npc[i].StrikeNPC(Projectile.damage, Projectile.knockBack, Direction);
                        Main.npc[i].AddBuff(ModContent.BuffType<EmberFire>(), 300);

                    }
                }

                firstFrame = false;

                Projectile.rotation = Main.rand.NextFloat(6.28f);

                //Spawn Dust
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 30; i++)
                {
                    if (Main.rand.NextBool())
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(5, 5);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.OrangeRed, 0.15f, 0.2f, 0f, dustShader2);
                        gd.fadeIn = 45 + Main.rand.NextFloat(-3f, 14f);
                        gd.scale *= Main.rand.NextFloat(0.9f, 2.1f);
                    }
                    else
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(6, 6);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleFlare>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.7f, 0.1f, 0f, dustShader2);
                        gd.fadeIn = 1;
                    }
                }
                
            }

            Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, 2.1f, 0.15f), 0f, 2f);
            scale2 = Math.Clamp(MathHelper.Lerp(scale2, 2.1f, 0.05f), 0f, 2f);
            scale3 = Math.Clamp(MathHelper.Lerp(scale3, 2.1f, 0.10f), 0f, 2f);

            //Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.velocity = Vector2.Zero;

            if (timer > 10)
            {
                //Projectile.scale *= 0.9f;
                colorIntensity -= 0.12f;
                if (colorIntensity <= 0)
                    Projectile.active = false;
            }
            Projectile.rotation += 0.06f;

            timer++;
        }

        //OrangeRed, Orange, Gold, Gold, Wheat, White
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sixStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
            Texture2D circle = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
            Texture2D circle2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D color = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/color_burst_30");
            Texture2D fourStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_06");


            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.Black * 0.85f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.75f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.7f * colorIntensity, Projectile.rotation * 2f, sixStar.Size() / 2, Projectile.scale * 0.5f, 0, 0f);

            Main.spriteBatch.Draw(sixStar, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * colorIntensity, Projectile.rotation, sixStar.Size() / 2, Projectile.scale * 0.75f, 0, 0f);
            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.Orange * colorIntensity, Projectile.rotation, circle.Size() / 2, scale3 * 0.17f, 0, 0f);

            Main.spriteBatch.Draw(color, Projectile.Center - Main.screenPosition, null, Color.Gold * colorIntensity, randomRot, color.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.Wheat * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.White * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.25f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
