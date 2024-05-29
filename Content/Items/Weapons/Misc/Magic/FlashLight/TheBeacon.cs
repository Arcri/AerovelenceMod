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
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using AerovelenceMod.Content.Projectiles.TempVFX;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight
{
    public class TheBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.mana = 7;
            Item.width = 46;
            Item.height = 46;
            Item.knockBack = 5f;
            
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.shootSpeed = 12f;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<TheBeaconProj>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarities.PlanteraGolemTier;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<TheLaserPointer>()).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddIngredient(ItemID.LunarTabletFragment, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool CanUseItem(Player player)
        {
            //Do not let the player use item if they have less than 40 mana so it doesn't flicker weird when they are out of mana
            return player.CheckMana(player.inventory[player.selectedItem], amount: 40, pay: false);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, 0f);
        }
    }

    public class TheBeaconProj : ModProjectile
    {
        public int OFFSET = 5;
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpVal = 0;

        public Vector2 endPoint;
        public float LaserRotation = 0;
        public float laserWidth = 20;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        int timer = 0;
        int laserTimer = 0;
        NPC prevLockOn = Main.npc[0];
        public bool lockedOn = false;
        
        List<int> tendrils = new List<int>();

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer == 0)
                Projectile.ai[0] = player.direction;

            Projectile.knockBack = 0f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            #region heldProjStuff

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - player.MountedCenter).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else
            {
                //Delete all tendrils
                foreach (int k in tendrils)
                {
                    (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                }
                tendrils.Clear();
                Projectile.active = false;
            }

            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * player.direction * -1f);
            Projectile.Center = player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            player.itemRotation = direction.ToRotation();

            if (player.direction != 1)
                player.itemRotation -= 3.14f;

            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);

            #endregion

            #region Laser

            if (FindNearestNPCMouse(500f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                if (npc != prevLockOn && timer != 0)
                {
                    //Delete all tendrils
                    foreach (int k in tendrils)
                    {
                        (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                    }
                    tendrils.Clear();

                    laserWidth = 0;
                    laserTimer = 0;
                    timer = -1;
                    lockedOn = false;
                }
                else
                {
                    if (!lockedOn)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .78f, PitchVariance = 0.1f, Volume = 0.3f };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/rescue_ranger_fire") with { Volume = .1f, Pitch = .9f, PitchVariance = .1f }; 
                        SoundEngine.PlaySound(style2, npc.Center);

                        //Store direction that star should turn
                        Projectile.ai[0] = player.MountedCenter.X > npc.Center.X ? -1 : 1; 
                        
                        //Delete tendrils incase they were still alive
                        foreach (int k in tendrils)
                        {
                            (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                        }
                        tendrils.Clear();

                        //Spawn all 3 tendrils
                        for (int i = 0; i < 3; i++)
                        {
                            int tendril = Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero * 1f, ModContent.ProjectileType<BeaconTendril>(), Projectile.damage, 0f, Main.myPlayer);
                            Main.projectile[tendril].ai[0] = Projectile.ai[0];
                            Main.projectile[tendril].rotation = MathHelper.ToRadians(120 + (120 * i));
                            tendrils.Add(tendril);
                        }
                    }
                    lockedOn = true;
                    endPoint = npc.Center;
                    LaserRotation = (npc.Center - (Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 40))).ToRotation();

                    laserWidth = MathHelper.Clamp(laserTimer, 0, 45);
                    prevLockOn = npc;

                    foreach (int tendril in tendrils)
                    {
                        (Main.projectile[tendril].ModProjectile as BeaconTendril).anchor = endPoint;
                    }

                    if (timer % 2 == 0)
                    {
                        if (Main.rand.NextBool())
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.06f, laserWidth * 0.06f);

                            Dust d = Dust.NewDustPerfect(npc.Center, ModContent.DustType<MuraLineBasic>(),
                                randomStart * 2f, Alpha: Main.rand.Next(10, 15), Color.Firebrick, 0.3f);
                        }
                        else
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.25f, laserWidth * 0.25f);

                            Dust orb = Dust.NewDustPerfect(npc.Center, ModContent.DustType<PixelGlowOrb>(),
                                randomStart * Main.rand.NextFloat(0.65f, 1.1f), newColor: Color.Firebrick, Scale: Main.rand.NextFloat(0.45f, 0.55f) * 1f);

                            orb.customData = DustBehaviorUtil.AssignBehavior_PGOBase(
                                rotPower: 0.1f, killEarlyTime: -1, timeBeforeSlow: 2, preSlowPower: 0.90f, postSlowPower: 0.89f, velToBeginShrink: 2f, fadePower: 0.9f,
                                dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.85f);
                        }
                    }

                    //Play hitsound
                    if (timer % 10 == 0)
                    {
                        SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .72f, PitchVariance = .11f, Volume = 0.01f * (laserWidth) };
                        SoundEngine.PlaySound(stylees, npc.Center);
                    }

                    //Mana
                    if (laserTimer % 10 == 0)
                    {
                        player.CheckMana(player.inventory[player.selectedItem], pay: true);
                    }
                }

            }
            else
            {
                //Delete tendrils 
                foreach (int k in tendrils)
                {
                    (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                }
                tendrils.Clear();

                lockedOn = false;
                laserWidth = 0;
                laserTimer = 0;
            }
            #endregion

            if (Projectile.timeLeft == 1)
            {
                //Delete tendrils 
                foreach (int k in tendrils)
                {
                    (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                }
                tendrils.Clear();
            }

            if (!player.CheckMana(player.inventory[player.selectedItem], pay: false))
            {
                //Delete all tendrils
                foreach (int k in tendrils)
                {
                    (Main.projectile[k].ModProjectile as BeaconTendril).fadeOut = true;
                }
                tendrils.Clear();
                Projectile.active = false;
            }

            laserTimer++;
            timer++;
        }
        private bool FindNearestNPCMouse(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
        {
            npcIndex = -1;
            bool foundNPC = false;
            double dist = range * range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                //Make sure NPC is valid anyway
                if (npc.active && npc.life > 0)
                {
                    if (!npc.hide && !npc.dontTakeDamage)
                    {
                        //Target and NPC friendliness are same
                        if (npc.friendly == targetIsFriendly)
                        {
                            //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                            if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                            {
                                //cache this
                                float compDist = Main.MouseWorld.DistanceSQ(npc.Center);
                                //Distance is shorter than current distance, but did not overflow (underflow)
                                if (compDist < dist && compDist > 0)
                                {
                                    //ignore tiles, OR scan tiles and can hit anyway
                                    if (!scanTiles || (scanTiles && Collision.CanHit(Projectile, new NPCAimedTarget(npc))))
                                    {
                                        npcIndex = i;
                                        dist = compDist;
                                        foundNPC = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            float ballDistance = 30;
            Vector2 startDrawPoint = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * ballDistance) - Main.screenPosition;

            #region Flashlight
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/TheBeaconProjGlow").Value;

            //Anim
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            SpriteEffects spriteEffects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin332 = sourceRectangle.Size() / 2f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);

            Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, lightColor, direction.ToRotation() + MathHelper.PiOver2, origin332, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, drawPos, sourceRectangle, Color.White * 0.3f, direction.ToRotation() + MathHelper.PiOver2, origin332, Projectile.scale, spriteEffects, 0.0f);

            #endregion

            #region Laser

            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;
            Texture2D LaserTextureBlack = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeamBlack").Value;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

            Color col1 = new Color(255, 75, 0) with { A = 0 };
            Color col2 = new Color(200, 135, 15) with { A = 0 };

            myEffect.Parameters["Color1"].SetValue(col1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(col2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(0.9f);

            myEffect.Parameters["tex1reps"].SetValue(0.15f);
            myEffect.Parameters["tex2reps"].SetValue(0.03f);
            myEffect.Parameters["satPower"].SetValue(0.75f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.013f);
            #endregion


            float height = laserWidth * 4f;

            int width = (int)((startDrawPoint + Main.screenPosition) - endPoint).Length();

            Vector2 pos = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * ballDistance) - Main.screenPosition;
            Rectangle target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));
            Rectangle targetSmaller = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.3f));

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);
            //Main.spriteBatch.Draw(LaserTexture, targetSmaller, null, Color.Black * 0.15f, LaserRotation, origin2, 0, 0);

            float laserDrawRot = (endPoint - (startDrawPoint + Main.screenPosition)).ToRotation();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, laserDrawRot, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, laserDrawRot, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(LaserTextureBlack, targetSmaller, null, new Color(255, 185, 0) with { A = 0 } * 0.2f, laserDrawRot, origin2, 0, 0);


            //Flares

            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_01").Value;
            Texture2D flare2 = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStar").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/TrailImages/PartiGlow").Value;

            float sinScale = laserWidth < 1f ? 0f : MathF.Sin((float)Main.timeForVisualEffects * 0.06f) * 0.025f;

            //Start
            Main.spriteBatch.Draw(flare12, startDrawPoint, null, Color.OrangeRed with { A = 0 }, timer * 0.12f * Projectile.ai[0], flare12.Size() / 2, 0.02f * laserWidth + sinScale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare12, startDrawPoint, null, Color.White with { A = 0 }, timer * 0.12f * Projectile.ai[0], flare12.Size() / 2, 0.013f * laserWidth, spriteEffects, 0.0f);

            //End
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition, null, new Color(255, 90, 15) with { A = 0 }, timer * 0.12f * Projectile.ai[0], flare1.Size() / 2, 0.004f * laserWidth, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare2, endPoint - Main.screenPosition, null, Color.White with { A = 0 }, timer * 0.12f * Projectile.ai[0], flare2.Size() / 2, 0.018f * laserWidth, spriteEffects, 0.0f);

            #endregion

            //Reset again cause arm glow thing that hasn't been fixed for like a year
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            if (target.whoAmI == prevLockOn.whoAmI)
            {

            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0 || Main.rand.NextBool())
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.15f, laserWidth * 0.15f);

                        Dust orb = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(),
                            randomStart * Main.rand.NextFloat(0.65f, 1.1f), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.45f, 0.55f) * 1f);

                        orb.customData = DustBehaviorUtil.AssignBehavior_PGOBase(
                            rotPower: 0.1f, killEarlyTime: -1, timeBeforeSlow: 2, preSlowPower: 0.90f, postSlowPower: 0.89f, velToBeginShrink: 2f, fadePower: 0.9f,
                            dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.85f);
                    }
                }

            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = LaserRotation.ToRotationVector2();
            float point = 0f;
            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 12,
                    endPoint, laserWidth * 0.5f, ref point);
            }

            return false;
        }

    }

    public class BeaconTendril : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 4400;
            Projectile.extraUpdates = 0;

            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() { return !fadeOut; }
    
        int timer = 0;
        float width = 0f;

        public List<Vector2> l_positions = new List<Vector2>();
        public List<float> l_rotations = new List<float>();

        Vector2[] arr_positions = new Vector2[150];
        float[] arr_rotations = new float[150];

        float[] draw_rotations = new float[150];
        Vector2[] draw_positions = new Vector2[150];

        int TotalPoints = 150;

        public Vector2 anchor = Vector2.Zero;

        public bool fadeOut = false;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.channel == false)
                fadeOut = true;

            if (timer == 0)
            {
                anchor = Projectile.Center;

                //Create all of the points and set all the rotations to be the same
                for (int i = 0; i < TotalPoints; i++)
                {
                    arr_positions[i] = Vector2.Zero + Projectile.rotation.ToRotationVector2() * (2.5f * i);
                    arr_rotations[i] = Projectile.rotation;
                }
            }

            anchor += Projectile.velocity;

            //Rotate the anchor
            Projectile.rotation += 0.07f * Easings.easeInSine(width) * Projectile.ai[0];

            //Have all points try to rotate towards the acnhor
            for (int j = 0; j < TotalPoints; j++)
            {
                float progress = (j / (float)TotalPoints);

                //The further along the trail, the weaker the turning
                float lerpValue = MathHelper.Lerp(1f, 0.3f, progress);

                //Keep angle within 2pi 
                float NormalizedGoalRotation = Projectile.rotation;

                float newRotation = MathHelper.Lerp(arr_rotations[j], NormalizedGoalRotation, lerpValue * 0.175f);

                arr_rotations[j] = newRotation;
                arr_positions[j] = Vector2.Zero + newRotation.ToRotationVector2() * (2.5f * j);
            }

            for (int k = 0; k < TotalPoints - 1 * width; k++)
            {
                //We have to flip the first point over for some reason or else we get a weird tear.
                if (k == 0)
                    draw_rotations[k] = arr_rotations[k] + MathHelper.Pi;
                else
                    draw_rotations[k] = (arr_positions[k - 1] - arr_positions[k]).ToRotation();
                draw_positions[k] = arr_positions[k] + anchor;
            }

            //Steadily increase the number of points used
            //Used to create the effect of the tendril growing outward
            l_positions.Clear();
            l_rotations.Clear();
            for (int m = 0; m < TotalPoints * Easings.easeInQuad(width); m++)
            {
                l_positions.Add(draw_positions[m]);
                l_rotations.Add(draw_rotations[m]);
            }

            if (!fadeOut)
                width = Math.Clamp(MathHelper.Lerp(width, 1.25f, 0.05f), 0f, 1f);
            else
                width = Math.Clamp(MathHelper.Lerp(width, -0.25f, 0.1f), 0f, 1f);

            if (width == 0f && fadeOut)
                Projectile.active = false;

            if (width >= 0.75 && timer % 4 == 0 && Main.rand.NextBool())
            {
                int randomPoint = Main.rand.Next(10, 70);

                float rot = arr_rotations[randomPoint];
                Vector2 pos = arr_positions[randomPoint] + anchor;

                Vector2 vel = rot.ToRotationVector2().RotatedByRandom(0.25f) * Main.rand.NextFloat(4f, 6f);
                vel += rot.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * Projectile.ai[0]) * 9;
                
                Dust smoke = Dust.NewDustPerfect(pos, ModContent.DustType<GlowStrong>(), vel, newColor: new Color(255, 100, 15), Scale: 0.2f);
            }

            timer++;
        }

        //Need ref for CheckAABBvLineCollision even if it doesn't do anything
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Do a line collision check between every 3 points

            float maxPoints = Math.Clamp(l_positions.Count(), 0f, 100f);
            for (int m = 4; m < maxPoints; m += 3)
            {
                float discarda = 0f;
                bool collided = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), l_positions[m], l_positions[m - 3], 15f, ref discarda);

                if (collided)
                    return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 0 || Main.rand.NextBool())
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(3f, 3f);

                    Dust orb = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(),
                        randomStart * Main.rand.NextFloat(0.65f, 1.1f), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.45f, 0.55f) * 1f);

                    orb.customData = DustBehaviorUtil.AssignBehavior_PGOBase(
                        rotPower: 0.1f, killEarlyTime: -1, timeBeforeSlow: 2, preSlowPower: 0.90f, postSlowPower: 0.89f, velToBeginShrink: 2f, fadePower: 0.9f,
                        dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.85f);
                }
            }
        }
        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/spark_07_Black").Value;
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Extra_196_Black").Value;

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/TrailImages/VanillaStar").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            Vector2[] pos_arr = l_positions.ToArray();
            float[] rot_arr = l_rotations.ToArray();

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStripBlack = new VertexStrip();
            vertexStripBlack.PrepareStrip(pos_arr, rot_arr, StripColor2, StripWidth2, -Main.screenPosition, includeBacksides: true);


            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue(timer * -0.04f);

            //Main layer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.OrangeRed.ToVector3() * 4f * width);

            myEffect.Parameters["glowThreshold"].SetValue(0.8f);
            myEffect.Parameters["glowIntensity"].SetValue(1.4f);


            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();
            vertexStrip.DrawTrail();

            //Layer 2
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            myEffect.Parameters["ColorOne"].SetValue(Color.OrangeRed.ToVector3() * 4f * width);

            myEffect.Parameters["glowThreshold"].SetValue(0.5f);
            myEffect.Parameters["glowIntensity"].SetValue(1.8f);


            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripBlack.DrawTrail();
            vertexStripBlack.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            //Visualizes hitbox
            /*
            float maxPoints = Math.Clamp(l_positions.Count(), 0f, 100f);
            for (int m = 6; m < maxPoints; m += 5)
            {
                Utils.DrawLine(Main.spriteBatch, l_positions[m], l_positions[m - 5], Color.Black, Color.Black, 15f);
            }
            */

            return false;
        }

        public Color StripColor(float progress)
        {
            float alpha = 1f;
            alpha = 1f - Easings.easeOutQuad(progress);
            Color color = new Color(0f, 0f, 0f, alpha);
            return color;
        }
        public float StripWidth(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1f - progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 140f, Easings.easeInCirc(num)) * 1.15f * Easings.easeInSine(width); // 0.3f 
        }

        public Color StripColor2(float progress)
        {
            float alpha = 1f;
            alpha = 1f - Easings.easeOutQuad(progress);
            Color color = new Color(0f, 0f, 0f, alpha);
            return color;
        }
        public float StripWidth2(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1f - progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 60f, Easings.easeInCirc(num)) * Easings.easeInSine(width); // 0.3f 
        }
    }



}
