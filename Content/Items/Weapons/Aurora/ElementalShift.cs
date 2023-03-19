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
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;
using static System.Formats.Asn1.AsnWriter;

namespace AerovelenceMod.Content.Items.Weapons.Aurora
{
    public class ElementalShift : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Shift");
            Tooltip.SetDefault("");
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
            Item.rare = ItemRarityID.Blue;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<ElementalShiftProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));


            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class ElementalShiftProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Shift");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f);
            return shouldDamage;
        }

        public bool shouldShoot = false;
        bool hasShot = false;

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;
        float origAng = 0;

        float Angle = 0;

        bool playedSound = false;

        float storedDirection = 1;

        int timer = 0;
        float offset = 0;
        int timerAfterEnd = 4; //10
        float alpha = 0;
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
            //if (easingProgress > 0.5)
                //player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            player.heldProj = Projectile.whoAmI;

            //player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {
                startingPos = player.Center;

                easingProgress = 0.09f; //0.01

                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);

                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                origAng = startingAng;
                //we set Projectile.ai[0] in the wep. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    startingAng = startingAng - MathHelper.ToRadians(-145);
                }
                else
                {
                    startingAng = startingAng + MathHelper.ToRadians(-145);
                }

                currentAng = startingAng;
                firstFrame = false;

            }


            if (timer >= 8 && hitDelay <= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((290 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((290 * getProgress(easingProgress)));

                easingProgress = Math.Clamp(easingProgress + 0.0085f * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 0.00f, 1f); // 0.01 | 0.007
            }

            offset = 50;
            alpha = 1;

            //offset = Math.Clamp(MathHelper.Lerp(offset, 42, 0.08f), 0, 40);
            //alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.08f), 0, 1);


            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * offset) + player.RotatedRelativePoint(player.MountedCenter) + new Vector2(-5 * player.direction,-3);
            player.itemTime = 10;
            player.itemAnimation = 10;

            timer++;

            if (getProgress(easingProgress) >= 0.35f && getProgress(easingProgress) <= 0.78f)
            {
                Projectile.ai[1] = 1;
            }
            else
                Projectile.ai[1] = 0;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                /*
                int Mura = Projectile.NewProjectile(null, player.Center + origAng.ToRotationVector2() * 10, Vector2.Zero, ModContent.ProjectileType<MuraLineHandler>(), 5, 0, Projectile.owner);

                if (Main.projectile[Mura].ModProjectile is MuraLineHandler mlh)
                {
                    mlh.fadeMult = 2f;

                    for (int m = 0; m < 10; m++)
                    {
                        float xScaleMinus = Main.rand.NextFloat(0.2f, 1.6f);
                        MuraLine newWind = new MuraLine(Main.projectile[Mura].Center, origAng.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-2f, 21f)) * Main.rand.NextFloat(4f, 8f), 2 - xScaleMinus);
                        newWind.color = FetchRainbow(Main.rand.Next(0, 300));
                        mlh.lines.Add(newWind);
                    }
                }
                */

                int soundNumber = Main.rand.Next(4,7);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_0" + soundNumber) with { Pitch = -.2f, Volume = 0.3f }; //-.15
                SoundEngine.PlaySound(style2, Projectile.Center);

                int suffix = Main.rand.NextBool() ? 2 : 3;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.52f, PitchVariance = .16f, Volume = 0.25f };
                SoundEngine.PlaySound(style, Projectile.Center);
                

                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f)
            {
                if (timerAfterEnd > 10)
                {
                    //offset = Math.Clamp(MathHelper.Lerp(offset, 0, 0.06f), 20, 40);

                }

                if (timerAfterEnd == 0)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    Projectile.active = false;
                }
                timerAfterEnd--;
                
            }

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlowTrail").Value;
            trailColor = Color.SkyBlue * 0.5f;
            trailPointLimit = 800;
            trailMaxLength = 200;
            trailTime = timer * 0.007f;
            //timesToDraw = 2;

            if (timer < 35)
            {
                trailRot = Projectile.rotation + MathHelper.PiOver4;
                trailPos = startingPos + Projectile.rotation.ToRotationVector2().RotatedBy(-0.75f) * 90; //70
                TrailLogic();
            }



            if (timer > 20)
            {
                trailWidth = Math.Clamp(trailWidth - 0.02f, 0, 1);
            }

            hitDelay--;
        }

        Vector2 startingPos = Vector2.Zero;
        float trailWidth = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Trail = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/halfSwingGlow");
            //Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/GlowStar");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/ElementalShiftGlow");
            //Texture2D Edge = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/HalfSwingEdge");


            // 1 - 0.7 - 0.4
            //Main.spriteBatch.Draw(TrailEdge, Projectile.Center - Main.screenPosition + new Vector2(20f, 0).RotatedBy(currentAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(0) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI)), currentAng + MathHelper.PiOver2, Trail.Size() / 2, 1f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(TrailEdge, Projectile.Center - Main.screenPosition + new Vector2(20f, 0).RotatedBy(currentAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(0) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI)) * 1.2f, currentAng + MathHelper.PiOver2, Trail.Size() / 2, 0.8f, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(10f, 0).RotatedBy(currentAng), Trail.Frame(1, 1, 0, 0), Color.Black * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.75f), currentAng + MathHelper.Pi, Trail.Size() / 2, 1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            if (getProgress(easingProgress) > 0.25f && getProgress(easingProgress) < 0.8f)
            {
                //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(30f, 0).RotatedBy(currentAng), Star.Frame(1, 1, 0, 0), FetchRainbow(0) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI)) * 0.7f, currentAng, Star.Size() / 2, 0.7f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(10f, 0).RotatedBy(origAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(100) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.75f), currentAng + MathHelper.Pi, Trail.Size() / 2, 1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(10f, 0).RotatedBy(origAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(100) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.5f), currentAng + MathHelper.Pi, Trail.Size() / 2, 1.1f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(10f, 0).RotatedBy(origAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(100) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), currentAng + MathHelper.Pi, Trail.Size() / 2, 0.7f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), SpriteEffects.None, 0f);

                //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(30f, 0).RotatedBy(currentAng), Star.Frame(1, 1, 0, 0), Color.White, currentAng, Star.Size() / 2, new Vector2(1f, 1.5f), SpriteEffects.None, 0f);

            }


            //Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(5f, 0).RotatedBy(currentAng), Trail.Frame(1, 1, 0, 0), FetchRainbow(200) * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.35f), currentAng + MathHelper.PiOver2, Trail.Size() / 2, 0.7f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + Main.rand.NextVector2CircularEdge(1,1), null, FetchRainbow(100), Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Glow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, FetchRainbow(100), Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Glow.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Aurora/ElementalShift");

            Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            

            return false;
        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            //return 1f - (float)Math.Pow(1 - x, 6);

            #region easeExpo

            
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (16 * x) - 8)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-16 * x) + 8)))) / 2;
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


        int hitDelay = 0;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //hitDelay = 5;

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_impact_object_03") with { Volume = .17f, Pitch = .64f, };
            SoundEngine.PlaySound(style2, target.Center);

            Vector2 spawnPos = Main.rand.NextVector2FromRectangle(target.getRect());

            if (Projectile.Center.Distance(spawnPos) > 80)
            {
                spawnPos = Main.player[Projectile.owner].Center + currentAng.ToRotationVector2() * 60;
            }

            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, ModContent.ProjectileType<ElementalShiftImpact>(), 0, 0f);

            /*
            if (!target.HasBuff<AuroraFire>())
            {

                SoundStyle stylea = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_flame_breath") with { Volume = .4f, Pitch = .64f, PitchVariance = .22f, };
                SoundEngine.PlaySound(stylea, target.Center);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = .42f, PitchVariance = .42f, Volume = 0.4f, MaxInstances = 2 };
                SoundEngine.PlaySound(style, target.Center);

                for (int i = 0; i < 9; i++)
                {
                    //have to make new dustShader everytime so color is different
                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    Color c = new Color(
                        (byte)Main.rand.Next(0, 255),
                        (byte)Main.rand.Next(0, 255),
                        (byte)Main.rand.Next(0, 255));

                    int p = GlowDustHelper.DrawGlowDust(target.position, target.width, target.height, ModContent.DustType<GlowCircleQuadStar>(), c, 0.7f, 0.6f, 0f, dustShader);
                    Main.dust[p].noLight = true;
                    Main.dust[p].velocity *= 0.3f;

                    Main.dust[p].velocity = Vector2.Normalize(target.Center - Main.dust[p].position) * Main.rand.NextFloat(5, 10);

                    //int p = GlowDustHelper.DrawGlowDust(target.position, target.width, target.height, ModContent.DustType<GlowCircleDust>(), c * 2f, 0.75f, 0.8f, 0f, dustShader);
                    Main.dust[p].fadeIn = 35 + Main.rand.NextFloat(5, 15f);
                    //Main.dust[p].velocity *= 3f;
                }

            }

            target.AddBuff(ModContent.BuffType<AuroraFire>(), 120);
            */
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.4f * trailWidth; // 0.3f 
        }


        public Color FetchRainbow(int offset = 0)
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects + offset));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects + 120 + offset));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects + 240 + offset));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }

    }


    public class ElementalShiftBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 720;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Projectile_540").Value;
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/Glorb").Value;

            //Texture2D texture2D = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Projectile_540").Value;


            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);

            Vector2 scaleVec2 = new Vector2(Projectile.velocity.Length() * 0.05f + 1f, 1f - Projectile.velocity.Length() * 0.03f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, FetchRainbow() * 0.8f, Projectile.rotation, glow.Size() / 2, 2.3f, SpriteEffects.None, 0);


            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 scale = scaleVec2 * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition;
                Color color = FetchRainbow() * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                for (int i = 0; i < 1; i++)
                {
                    Main.EntitySpriteDraw(texture2D, drawPos + new Vector2(Projectile.width / 2, Projectile.height / 2), null, color * 0.8f, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture2D, drawPos + new Vector2(Projectile.width / 2, Projectile.height / 2), null, Color.White.MultiplyRGBA(color * 0.5f), Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0);

                    //Main.EntitySpriteDraw(texture2D, drawPos, null, Color.White.MultiplyRGBA(color * 0.75f), Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public Color FetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(timer));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(timer + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(timer + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }

        int timer = 0;
        int justHitCounter = 10;
        public override void AI()
        {

            if (justHitCounter <= 0)
                Projectile.velocity *= 0.9f;
            else
                Projectile.velocity *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            justHitCounter--;
            timer++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity *= -1;
            justHitCounter = 5;

            //int fx = Projectile.NewProjectile(null, target.Center, Vector2.Zero, ModContent.ProjectileType<ElementalShiftImpact>(), 0, 0f);
            //Main.projectile[fx].scale = 2.2f;

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_66") with { Pitch = .52f, }; SoundEngine.PlaySound(style2, Projectile.Center);

            int Mura = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MuraLineHandler>(), 0, 0, Projectile.owner);

            if (Main.projectile[Mura].ModProjectile is MuraLineHandler mlh)
            {
                mlh.fadeMult = 2f;

                for (int m = 0; m < 10; m++)
                {
                    float xScaleMinus = Main.rand.NextFloat(0.2f, 1.6f);
                    MuraLine newWind = new MuraLine(Main.projectile[Mura].Center, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-1f, 1f)) * -1 * Main.rand.NextFloat(4f, 8f), 2 - xScaleMinus);
                    newWind.color = FetchRainbow();
                    mlh.lines.Add(newWind);
                }
            }

        }
    }

    public class ElementalShiftPulse : ModProjectile
    {
        //Either scale to size, rotate and wait for a bit, then go back to zero
        //OR scale to size and then fade intensity


        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public override bool? CanDamage() { return false; }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.3f;
            Projectile.timeLeft = 1000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }


        float alpha = 1;
        float scale = 0f;
        bool rotDir = Main.rand.NextBool();
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            
            Projectile.rotation += rotDir ? 0.02f : -0.02f;


            if (timer > 15)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.12f), 0, 1);
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.1f, 0.02f), 0, 1f);

            }
            else
                scale = Math.Clamp(MathHelper.Lerp(scale, 0.6f, 0.12f), 0, 0.5f);

            if (alpha <= 0f)
                Projectile.active = false;

            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0) return false;

            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_10").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(FetchRainbow().ToVector3() * 2f * alpha);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, Flare.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), Color.White, Projectile.rotation + MathHelper.Pi, Flare.Size() / 2, scale * 0.4f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //Something something make a helper method for this later
        public Color FetchRainbow(int offset = 0)
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + offset));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + 120 + offset));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + 240 + offset));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }

    }

    public class ElementalShiftImpact : ModProjectile
    {
        
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public override bool? CanDamage() { return false; }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            //Projectile.scale = 0.3f;
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }


        float alpha = 1f;
        float scale = 0f;
        
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            if (timer > 15)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.1f), 0, 1);
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.1f, 0.05f), 0, 1f);

                

            }
            else
                scale = Math.Clamp(MathHelper.Lerp(scale, 1.4f, 0.12f), 0, 1f);


            if (timer == 8)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 8; i++)
                {
                    if (i != 4 && i != 0)
                    {
                        Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleFlare>(),
                            Projectile.rotation.ToRotationVector2().RotatedBy(i * MathHelper.PiOver4) * 2, FetchRainbow(), 0.8f, 0.3f, 0f, dustShader);
                        p.fadeIn = 2;
                        p.noLight = true;

                    }

                    //p.rotation = Main.rand.NextFloat(6.28f);
                }
            }

            if (alpha <= 0f)
                Projectile.active = false;

            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0) return false;

            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Assets/GlorbStrong").Value;
            Texture2D OuterGlow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;


            Vector2 vec2scale = new Vector2(scale * 0.5f, scale) * Projectile.scale; //1f scale

            //Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            //myEffect.Parameters["uColor"].SetValue(FetchRainbow().ToVector3() * 2f * alpha);
            //myEffect.Parameters["uTime"].SetValue(2);
            //myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.6
            //myEffect.Parameters["uSaturation"].SetValue(0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), FetchRainbow() * alpha * (alpha == 1 ? 2f : 1), Projectile.rotation, Flare.Size() / 2, vec2scale * 0.6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), FetchRainbow() * alpha, Projectile.rotation, Flare.Size() / 2, vec2scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), FetchRainbow() * alpha, Projectile.rotation + MathHelper.PiOver4, Flare.Size() / 2, scale * 0.3f + alpha * 0.3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, Glow.Frame(1, 1, 0, 0), FetchRainbow() * alpha, Projectile.rotation + MathHelper.PiOver4, Glow.Size() / 2, scale * 0.3f + alpha * 0.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(OuterGlow, Projectile.Center - Main.screenPosition, OuterGlow.Frame(1, 1, 0, 0), FetchRainbow() * alpha, Projectile.rotation + MathHelper.PiOver4, OuterGlow.Size() / 2, scale * 0.6f + alpha * 0.6f, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, Flare.Frame(1, 1, 0, 0), FetchRainbow() * 1.5f, Projectile.rotation + MathHelper.PiOver2, Flare.Size() / 2, scale * 0.7f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //Something something make a helper method for this later
        public Color FetchRainbow(int offset = 0)
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + offset));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + 120 + offset));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians((float)Main.timeForVisualEffects * 3 + 240 + offset));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }

    }

}
