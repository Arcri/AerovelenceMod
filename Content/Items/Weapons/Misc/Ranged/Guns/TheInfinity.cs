using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{
    public class TheInfinity : ModItem
    {
        public static int CurrentElementIndex = 0;

        public static readonly int[] ElementFrameIndices =
        {
            0, //Caustic (first frame)
            1, //Ice (second frame)
            2, //Fire (third frame) 
            3, //Explosive (fourth frame)
            4  //Electric (fifth frame)
        };

        public static readonly string[] ElementTypes =
        {
            "Caustic",   // Green, corrosive
            "Ice",       // Blue, slow
            "Fire",      // Red, burning
            "Explosive", // Dark blue, electrified
            "Electric"   // Yellow, explosion
        };

        public static readonly Color[] ElementColors =
        [
            new Color(32, 178, 73),    //Caustic green
            new Color(0, 242, 255),    //Ice blue
            new Color(255, 69, 0),     //Fire orange-red
            new Color(255, 215, 0),    //Explosive yellow
            new Color(0, 90, 255)      //Electric dark blue
            
        ];

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 32;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 42;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarities.RarePrePlant;
            Item.shoot = ModContent.ProjectileType<TheInfinityHeldProj>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes based on element (?) [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) { return false; }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle shootSound = new SoundStyle("Terraria/Sounds/Item_11") with { Pitch = -0.2f, MaxInstances = 3, PitchVariance = .1f };
            SoundEngine.PlaySound(shootSound, position);
            bool foundExistingProj = false;
            int existingProjIndex = -1;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<TheInfinityHeldProj>())
                {
                    foundExistingProj = true;
                    existingProjIndex = i;
                    break;
                }
            }
            if (foundExistingProj && existingProjIndex != -1)
            {
                if (Main.projectile[existingProjIndex].ModProjectile is TheInfinityHeldProj gunProj)
                    gunProj.TriggerShoot();
            }
            else
            {
                int heldProj = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<TheInfinityHeldProj>(), 0, 0, player.whoAmI);
                if (Main.projectile[heldProj].ModProjectile is TheInfinityHeldProj gunProj)
                    gunProj.TriggerShoot();
            }

            return false;
        }
    }

    public class TheInfinityHeldProj : ModProjectile
    {

        private bool needToShoot = false;
        private Player Owner => Main.player[Projectile.owner];
        private int inactiveCounter = 0;
        private const int MAX_INACTIVE_TIME = 60;
        private int timeSinceLastShot = 0;
        private const int INACTIVE_TIMEOUT = 9;
        private int lastSelectedItem = -1;

        //Recoil
        private Vector2 recoilOffset = Vector2.Zero;
        private float recoilStrength = 6f;
        private float recoilRecoverySpeed = 0.4f;
        private bool hasRecoil = false;
        private float recoilRotation = 0f;
        private float maxRecoilRotation = -0.2f;
        private float rotationalRecoilRecoverySpeed = 0.4f;

        //Infinity symbol
        private List<Vector2> infinityPoints = [];
        private int maxInfinityPoints = 100;
        private float infinityTime = 0f;
        private float infinitySpeed = 0.15f;
        private float infinityScale = 24f;
        private int infinityLifetime = 30;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpVal = 0;

        public void TriggerShoot()
        {
            needToShoot = true;
            timeSinceLastShot = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
        }

        public override bool? CanDamage() => false;

        private void ApplyRecoil()
        {
            float backwardRecoil = -recoilStrength;
            float upwardRecoil = recoilStrength * 0.5f;

            if (Owner.direction == -1)
                recoilOffset = new Vector2(backwardRecoil, upwardRecoil);
            else
                recoilOffset = new Vector2(backwardRecoil, -upwardRecoil);

            recoilRotation = maxRecoilRotation;
            hasRecoil = true;
        }

        private float glowPulse = 0f;
        private float infinityGlowPulse = 0f;
        private float whiteGlowPulse = 0f;
        private bool rightClickHandled = false;

        public override void AI()
        {
            if (Owner.dead || Owner.stoned || Owner.frozen)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
                Angle = (Main.MouseWorld - (Owner.MountedCenter)).ToRotation();
            direction = Angle.ToRotationVector2();
            Owner.ChangeDir(direction.X > 0 ? 1 : -1);
            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);
            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * Owner.direction * -1f);
            float armRotation = Projectile.rotation - MathHelper.PiOver2;
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float offsetForward = 20f;
            float offsetSide = -4f;
            if (Owner.direction < 0)
                offsetSide *= -1;
            Vector2 offsetVector = new(
                offsetForward * (float)Math.Cos(direction.ToRotation()) - offsetSide * (float)Math.Sin(direction.ToRotation()),
                offsetForward * (float)Math.Sin(direction.ToRotation()) + offsetSide * (float)Math.Cos(direction.ToRotation())
            );
            if (hasRecoil)
            {
                recoilOffset = Vector2.Lerp(recoilOffset, Vector2.Zero, recoilRecoverySpeed);
                recoilRotation = MathHelper.Lerp(recoilRotation, 0f, rotationalRecoilRecoverySpeed);

                if (recoilOffset.Length() < 0.1f && Math.Abs(recoilRotation) < 0.01f)
                {
                    hasRecoil = false;
                    recoilOffset = Vector2.Zero;
                    recoilRotation = 0f;
                }
            }
            Vector2 recoilVector = Vector2.Zero;
            if (hasRecoil)
            {
                recoilVector = new Vector2(
                    recoilOffset.X * (float)Math.Cos(direction.ToRotation()) - recoilOffset.Y * (float)Math.Sin(direction.ToRotation()),
                    recoilOffset.X * (float)Math.Sin(direction.ToRotation()) + recoilOffset.Y * (float)Math.Cos(direction.ToRotation())
                );
            }
            Projectile.Center = armPosition + offsetVector + recoilVector;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float finalRotation = direction.ToRotation();
            if (hasRecoil)
                finalRotation += recoilRotation * Owner.direction;
            Projectile.rotation = finalRotation;
            Owner.heldProj = Projectile.whoAmI;
            if (Main.mouseRight && Main.myPlayer == Owner.whoAmI)
            {
                if (!rightClickHandled)
                {
                    rightClickHandled = true;
                    TheInfinity.CurrentElementIndex = (TheInfinity.CurrentElementIndex + 1) % TheInfinity.ElementTypes.Length;
                    SoundStyle switchSound = new SoundStyle("Terraria/Sounds/Item_30") with { Pitch = 0.2f, Volume = 0.5f };
                    SoundEngine.PlaySound(switchSound, Projectile.Center);
                    Color elementColor = TheInfinity.ElementColors[TheInfinity.CurrentElementIndex];
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 dustVel = Main.rand.NextVector2Circular(5f, 5f);
                        Dust d = Dust.NewDustDirect(Projectile.Center, 10, 10, GetDustTypeForElement(TheInfinity.CurrentElementIndex), dustVel.X, dustVel.Y, 0, elementColor, 1.2f);
                        d.noGravity = true;
                        d.fadeIn = 1.5f;
                    }
                }
            }
            else
                rightClickHandled = false;
            if (!Owner.channel)
            {
                inactiveCounter++;
                if (inactiveCounter >= MAX_INACTIVE_TIME)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
                inactiveCounter = 0;
            bool holdingCorrectItem = Owner.HeldItem.type == ModContent.ItemType<TheInfinity>();
            bool selectedItemChanged = lastSelectedItem != Owner.selectedItem;
            if (selectedItemChanged)
                lastSelectedItem = Owner.selectedItem;
            if (!holdingCorrectItem || Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (Owner.selectedItem != lastSelectedItem)
            {
                Projectile.Kill();
                return;
            }
            timeSinceLastShot++;
            if (timeSinceLastShot >= INACTIVE_TIMEOUT)
            {
                Projectile.Kill();
                return;
            }
            if (needToShoot)
            {
                needToShoot = false;
                Vector2 aimDirection = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
                int bulletDamage = (int)(Owner.HeldItem.damage * Owner.GetDamage(DamageClass.Ranged).Multiplicative);
                FireElementalBullet(aimDirection, bulletDamage);
                ApplyRecoil();
                glowPulse = 1f;
                infinityGlowPulse = 1f;
                whiteGlowPulse = 1f;
            }
            glowPulse = Math.Max(0f, glowPulse - 0.05f);
            infinityGlowPulse = Math.Max(0f, infinityGlowPulse - 0.3f);
            whiteGlowPulse = Math.Max(0f, whiteGlowPulse - 0.03f);
            UpdateInfinitySymbol();
        }

        private int GetDustTypeForElement(int elementIndex)
        {
            return elementIndex switch
            {
                0 => 163, //Caustic
                1 => 135, //Ice
                2 => 174, //Fire
                3 => 6,   //Explosive
                4 => 226, //Electric
                _ => DustID.WhiteTorch
            };
        }

        private void FireElementalBullet(Vector2 aimDirection, int damage)
        {
            int elementIndex = TheInfinity.CurrentElementIndex;
            int projType = ModContent.ProjectileType<InfinityBullet>();
            Vector2 velocity = aimDirection * 16f;
            Vector2 muzzleOffset = new Vector2(-50, -10);
            muzzleOffset = muzzleOffset.RotatedBy(aimDirection.ToRotation());
            if (Owner.direction == -1)
                muzzleOffset.Y *= -1;
            Vector2 spawnPosition = Projectile.Center + aimDirection * 36f + muzzleOffset;
            int bulletProj = Projectile.NewProjectile(Owner.GetSource_ItemUse(Owner.HeldItem), spawnPosition, velocity, projType, damage, Owner.HeldItem.knockBack, Owner.whoAmI, elementIndex);
            Color dustColor = TheInfinity.ElementColors[elementIndex];
            int dustType = GetDustTypeForElement(elementIndex);
        }

        private float progressiveDrawProgress = 0f;
        private float progressiveDrawSpeed = 0.002f;
        private float progressiveAcceleration = 0.00001f;
        private float progressiveMaxSpeed = 0.02f;
        private List<Vector2> movingDotAfterimages = [];
        private List<float> movingDotAfterimageTimers = [];
        private float afterimageLifetime = 30f;

        private void UpdateInfinitySymbol()
        {
            int segments = 20; 
            infinityPoints.Clear();
            infinityTime += infinitySpeed;
            if (infinityTime > MathHelper.TwoPi)
                infinityTime -= MathHelper.TwoPi;
            for (int i = 0; i < segments; i++)
            {
                float t = ((float)i / segments) * MathHelper.TwoPi + infinityTime;
                //Call me Gerono the way I be lemniscating (Lemniscate of Gerono formula)
                float x = (float)Math.Cos(t) * infinityScale;
                float y = (float)Math.Sin(t) * (float)Math.Cos(t) * infinityScale;
                Vector2 offset = new Vector2(x, y);
                offset = offset.RotatedBy(direction.ToRotation());
                Vector2 point = Projectile.Center + direction * 40f + offset;
                infinityPoints.Add(point);
            }
            progressiveDrawSpeed = Math.Min(progressiveDrawSpeed + progressiveAcceleration, progressiveMaxSpeed);
            progressiveDrawProgress += progressiveDrawSpeed;
            if (progressiveDrawProgress > 1f)
                progressiveDrawProgress = 1f;
            int segmentsToDraw = (int)(infinityPoints.Count * progressiveDrawProgress);
            if (segmentsToDraw > 0 && segmentsToDraw < infinityPoints.Count)
            {
                Vector2 movingPoint = infinityPoints[segmentsToDraw];
                movingDotAfterimages.Add(movingPoint);
                movingDotAfterimageTimers.Add(0f);
            }

            for (int i = 0; i < movingDotAfterimageTimers.Count; i++)
            {
                movingDotAfterimageTimers[i] += 1f;
            }
            while (movingDotAfterimageTimers.Count > 0 && movingDotAfterimageTimers[0] > afterimageLifetime)
            {
                movingDotAfterimageTimers.RemoveAt(0);
                movingDotAfterimages.RemoveAt(0);
            }
        }


        private void DrawInfinityLoop()
        {
            Texture2D magicPixel = ModContent.Request<Texture2D>("AerovelenceMod/Assets/GlorbStrong").Value;
            Color loopColor = TheInfinity.ElementColors[TheInfinity.CurrentElementIndex] * 2f;
            float thickness = 8f;
            Vector2 origin = new(magicPixel.Width / 2f, magicPixel.Height / 2f);
            int segmentsToDraw = (int)(infinityPoints.Count * progressiveDrawProgress);

            //i finally understand why zoolander cant look left
            float effectiveRotation = Projectile.rotation;
            if (Owner.direction == -1)
                effectiveRotation += MathHelper.Pi;
            Vector2 additionalOffset = new Vector2(0, -6).RotatedBy(effectiveRotation);
            for (int i = 0; i < segmentsToDraw; i++)
            {
                Vector2 start = infinityPoints[i] - Main.screenPosition + additionalOffset;

                Vector2 end = infinityPoints[(i + 1) % infinityPoints.Count] - Main.screenPosition;
                Vector2 edge = end - start;
                float length = edge.Length();
                float rotation = edge.ToRotation();
                Vector2 scale = new(length / magicPixel.Width, thickness / magicPixel.Height);
                Main.spriteBatch.Draw(magicPixel, start, null, loopColor, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (infinityPoints.Count > 1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                DrawInfinityLoop();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }


        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/TheInfinityHeldProj").Value;
            int frameIndex = TheInfinity.ElementFrameIndices[TheInfinity.CurrentElementIndex];
            Rectangle sourceRect = new(0, frameIndex * 34, 62, 32);
            SpriteEffects spriteEffects = (Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            float rotation = Projectile.rotation;
            if (Owner.direction == -1)
                rotation += MathHelper.Pi;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(sourceRect.Width / 2, sourceRect.Height / 2);
            Main.spriteBatch.Draw(texture, drawPos, sourceRect, lightColor, rotation, origin, 1f, spriteEffects, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //1) Draw glowmask but make it cool and pulse
            if (glowPulse > 0f)
            {
                Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/TheInfinityProjGlow").Value;
                float glowAlpha = MathHelper.Lerp(0.8f, 1f, glowPulse);
                Main.spriteBatch.Draw(glowTexture, drawPos, sourceRect, Color.White * glowAlpha, rotation, origin, 1f, spriteEffects, 0f);
            }

            //2) Draw colored infinity symbol
            if (infinityGlowPulse > 0f)
            {
                Texture2D infinityGlowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/TheInfinityInfinityGlow").Value;
                Color elementColor = TheInfinity.ElementColors[TheInfinity.CurrentElementIndex];
                Main.spriteBatch.Draw(infinityGlowTexture, drawPos, sourceRect, elementColor * infinityGlowPulse, rotation, origin, 1f, spriteEffects, 0f);
            }

            //3) Draw white infinity symbol
            if (whiteGlowPulse > 0f)
            {
                Texture2D whiteGlowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/TheInfinityInfinityWhite").Value;
                Main.spriteBatch.Draw(whiteGlowTexture, drawPos, null, Color.White * whiteGlowPulse, rotation, new Vector2(whiteGlowTexture.Width / 2, whiteGlowTexture.Height / 2), 1f, spriteEffects, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class InfinityBullet : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private int elementIndex;
        private bool impacted = false;
        private int impactTimer = 0;
        private int impactDuration = 5;

        private int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            elementIndex = (int)Projectile.ai[0];
        }

        public List<float> previousRotations = [];
        public List<Vector2> previousPostions = [];

        public void DrawTrail()
        {
            //basically just OceanMist projectile by Linty
            Texture2D line = CommonTextures.GlowCircleFlare.Value;
            if (previousRotations != null && previousPostions != null)
            {
                Color baseColor = trailColor;

                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;
                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.25f) * 0.1f;
                    Vector2 AfterImagePos = previousPostions[i] - Main.screenPosition;
                    float startScale = Projectile.scale + sineScale;
                    float easedFadeValue = Easings.easeInSine(progress);
                    float hueShift = progress * 0.1f;

                    //1) Subtle hue shift effect
                    Color shiftedColor = ColorUtils.ShiftHue(baseColor, hueShift);
                    Color displayColor = shiftedColor with { A = 0 } * easedFadeValue;

                    Vector2 lineScale = new Vector2(1.25f, 0.3f + 0.3f * progress);
                    Vector2 lineScale2 = new Vector2(1.25f, 0.06f + 0.04f * progress);

                    //2) Main colored trail
                    Main.EntitySpriteDraw(line, AfterImagePos / 2, null, displayColor,
                        previousRotations[i], line.Size() / 2f, lineScale * startScale * 0.5f, SpriteEffects.None);

                    //3) White core
                    Main.EntitySpriteDraw(line, AfterImagePos / 2, null, Color.White with { A = 0 } * 0.9f * easedFadeValue,
                        previousRotations[i], line.Size() / 2f, lineScale2 * startScale * 0.5f, SpriteEffects.None);
                }
            }
        }



        public override void AI()
        {
            int trailCount = 40;

            if (timer % 2 == 0)
            {
                previousRotations.Add(Projectile.velocity.ToRotation());
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }

            if (impacted)
            {
                impactTimer++;
                if (impactTimer >= impactDuration)
                    Projectile.Kill();
                return;
            }

            Color elementColor = TheInfinity.ElementColors[elementIndex];

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/s06sBloom").Value;
            trailColor = elementColor;
            trailTime = 1f;
            trailPointLimit = 10;
            trailWidth = 1;
            trailMaxLength = 100;
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center + Projectile.velocity;
            TrailLogic();

            Lighting.AddLight(Projectile.Center, elementColor.ToVector3() * 0.5f);
            if (Main.rand.NextBool(5))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, GetDustType(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, elementColor, 1f);
                d.noGravity = true;
                d.velocity *= 0.3f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            HandleImpact();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (elementIndex)
            {
                case 0: //Caustic
                    target.AddBuff(BuffID.Venom, 180);
                    break;
                case 1: //Ice
                    target.AddBuff(BuffID.Frostburn, 180);
                    target.AddBuff(BuffID.Slow, 120);
                    break;
                case 2: //Fire
                    target.AddBuff(BuffID.OnFire, 240);
                    break;
                case 3: //Explosive
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC nearbyNPC = Main.npc[i];
                            if (nearbyNPC.active && !nearbyNPC.friendly && !nearbyNPC.dontTakeDamage &&Vector2.Distance(nearbyNPC.Center, Projectile.Center) < 120f &&nearbyNPC.whoAmI != target.whoAmI)
                            {
                                int explosionDamage = Projectile.damage / 2;
                                nearbyNPC.StrikeNPC(new NPC.HitInfo
                                {
                                    Damage = explosionDamage,
                                    Knockback = Projectile.knockBack / 2,
                                    HitDirection = (nearbyNPC.Center.X < Projectile.Center.X) ? -1 : 1
                                });
                            }
                        }
                    }
                    break;
                case 4: //Electric
                    target.AddBuff(BuffID.Electrified, 180);
                    break;
            }
            HandleImpact();
        }

        private void HandleImpact()
        {
            if (impacted) return;

            impacted = true;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.position = Projectile.Center;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.Center = Projectile.position;
            switch (elementIndex)
            {
                case 0: //Caustic
                    CreateAcidSplash();
                    break;
                case 1: //Ice
                    CreateIceShard();
                    break;
                case 2: //Fire
                    CreateFireBurst();
                    break;
                case 3: //Explosive
                    CreateExplosion();
                    break;
                case 4: //Electric
                    CreateLightningArcs();
                    break;
            }
        }

        private void CreateAcidSplash()
        {
            //SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(0.5f, 1f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.PoisonStaff, velocity.X, velocity.Y, 0, TheInfinity.ElementColors[0], Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
                d.fadeIn = 1.2f;
            }
        }

        private void CreateIceShard()
        {
            //SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.IceTorch, velocity.X, velocity.Y, 0, TheInfinity.ElementColors[1], Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
                d.fadeIn = 1.2f;
            }
        }

        private void CreateFireBurst()
        {
            //SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.InfernoFork, velocity.X, velocity.Y, 0, TheInfinity.ElementColors[2], Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
                d.fadeIn = 1.5f;
            }
        }

        private void CreateExplosion()
        {
            //SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12f, 12f);
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Torch, velocity.X, velocity.Y, 0, TheInfinity.ElementColors[3], Main.rand.NextFloat(1.5f, 2.5f));
                d.noGravity = true;
                Dust smoke = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Smoke, velocity.X * 0.5f, velocity.Y * 0.5f, 0, Color.Gray, Main.rand.NextFloat(0.8f, 1f));
                smoke.noGravity = true;
                smoke.fadeIn = 1.5f;
            }
        }

        private void CreateLightningArcs()
        {
            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                float distance = Main.rand.Next(50, 100);
                Vector2 target = Projectile.Center + direction * distance;
                Vector2 current = Projectile.Center;
                int segments = (int)(distance / 10f);
                for (int j = 0; j < segments; j++)
                {
                    Vector2 next = Vector2.Lerp(current, target, (float)(j + 1) / segments);
                    next += Main.rand.NextVector2Circular(5f, 5f);
                    for (int d = 0; d < 2; d++)
                    {
                        Dust dust = Dust.NewDustPerfect(Vector2.Lerp(current, next, d / 2f), 226, Vector2.Zero, 0, TheInfinity.ElementColors[4], 1.2f);
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                    }
                    current = next;
                }
            }
        }

        private int GetDustType()
        {
            return elementIndex switch
            {
                0 => 163, //Caustic
                1 => 135, //Ice
                2 => 174, //Fire
                3 => 6,   //Explosive
                4 => 226, //Electric
                _ => DustID.WhiteTorch
            };
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (impacted) return false;
            PixellationSystem.QueuePixelationAction(() =>
            {
                DrawTrail();
            }, PixellationSystem.RenderType.AlphaBlend);
            Texture2D glowTex = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Nightglow").Value;
            Color elementColor = TheInfinity.ElementColors[elementIndex];
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, elementColor * 0.7f, Projectile.rotation, glowTex.Size() / 2f, 1f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, elementColor * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 1f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, trailColor, Projectile.rotation, glowTex.Size() / 2f, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            return false;
        }
    }
}