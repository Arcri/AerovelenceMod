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
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight
{
    public class TheFlashlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 0f;
            Item.mana = 5;
            Item.width = Item.height = 26;

            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.shootSpeed = 12f;

            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<TheFlashlightProj>();

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes after locking onto the same enemy for a while [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 10).
                AddIngredient(ItemID.Topaz, 5).
                AddIngredient(ItemID.Lens, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        public override bool CanUseItem(Player player)
        {
            //Do not let the player use item if they have less than 20 mana so it doesn't flicker weird when they are out of mana
            return player.CheckMana(player.inventory[player.selectedItem], amount: 20, pay: false);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, 0f);
        }
    }
    public class TheFlashlightProj : ModProjectile
    {
        public int OFFSET = 15;
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpVal = 0;

        public Vector2 endPoint;
        public float LaserRotation = 0;
        public float laserWidth = 20;

        int timeUntilSkillStrike = 140;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        int timer = 0;
        int laserTimer = 0;
        int lockedOnTimer = 0;
        NPC prevLockOn = Main.npc[0];
        public bool lockedOn = false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            #region heldProjStuff
            KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (player.MountedCenter)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else
            {
                Projectile.active = false;
            }

            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * player.direction * -1f);
            Projectile.Center = player.MountedCenter + (direction * OFFSET) + new Vector2(0f, player.gfxOffY);
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

            if (FindNearestNPCMouse(150f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                if (npc != prevLockOn && timer != 0)
                {
                    laserWidth = 0;
                    laserTimer = 0;
                    lockedOnTimer = 0;
                    timer = -1;
                    lockedOn = false;
                }
                else
                {
                    if (!lockedOn)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_108") with { Pitch = .78f, PitchVariance = 0.1f, Volume = 0.3f };
                        SoundEngine.PlaySound(style, Projectile.Center);
                    }

                    lockedOn = true;
                    endPoint = npc.Center + new Vector2(0f, player.gfxOffY);
                    LaserRotation = (npc.Center - (Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 25))).ToRotation();

                    laserWidth = MathHelper.Clamp(0 + (laserTimer * 0.4f), 0, 15);
                    prevLockOn = npc;

                    if (timer % 2 == 0)
                    {
                        if (Main.rand.NextBool())
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.15f, laserWidth * 0.15f);

                            Dust d = Dust.NewDustPerfect(npc.Center, ModContent.DustType<MuraLineBasic>(),
                                randomStart * 1.5f, Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.15f);
                        }
                        else
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.25f, laserWidth * 0.25f);

                            Dust orb = Dust.NewDustPerfect(npc.Center, ModContent.DustType<PixelGlowOrb>(),
                                randomStart * Main.rand.NextFloat(0.65f, 1.35f), newColor: Color.Orange, Scale: Main.rand.NextFloat(0.35f, 0.45f) * 1f);

                            orb.customData = DustBehaviorUtil.AssignBehavior_PGOBase(
                                rotPower: 0.1f, killEarlyTime: -1, timeBeforeSlow: 2, preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1.55f, fadePower: 0.9f,
                                dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.85f);
                        }
                    }

                    if (laserTimer % 15 == 0)
                    {
                        player.CheckMana(player.inventory[player.selectedItem], pay: true);
                    }

                    lockedOnTimer++;
                }

            }
            else
            {
                lockedOn = false;
                laserWidth = 0;
                laserTimer = 0;
                lockedOnTimer = 0;
            }
            #endregion

            if (lockedOnTimer > timeUntilSkillStrike)
                SkillStrikeUtil.setSkillStrike(Projectile, 1.3f, 10000, 0.5f, 0f);
            else
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;

            //Store direction that star should turn
            if (timer == 0)
                Projectile.ai[0] = player.direction;

            if (!player.CheckMana(player.inventory[player.selectedItem], pay: false))
                Projectile.active = false;

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
                                    if (!scanTiles || (scanTiles && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1)))
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

            #region Flashlight
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightGlow").Value;

            SpriteEffects spriteEffects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height1 = texture.Height;
            Vector2 origin = new Vector2(texture.Width / 2f, height1 / 2f);

            Vector2 actualPos = Projectile.Center - Main.screenPosition;// + new Vector2(0f, Player.gfxOffY);
            Main.spriteBatch.Draw(texture, actualPos, null, lightColor, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, actualPos, null, Color.White * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(glowMask, actualPos, null, Color.White * 0.1f * (laserWidth * 0.1f), direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale + 0.1f, spriteEffects, 0.0f);

            #endregion

            #region Laser

            Texture2D LaserTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeam").Value;
            Texture2D LaserTextureBlack = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/FlashLightBeamBlack").Value;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

            Color col1 = new Color(255, 170, 20) with { A = 0 };
            Color col2 = new Color(200, 100, 20) with { A = 0 };

            myEffect.Parameters["Color1"].SetValue(col1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(col2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(0.75f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);

            myEffect.Parameters["tex1reps"].SetValue(0.25f);
            myEffect.Parameters["tex2reps"].SetValue(0.03f);
            myEffect.Parameters["satPower"].SetValue(0.8f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.013f);
            #endregion


            float height = laserWidth * 5f;

            int width = (int)(Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 23) - endPoint).Length();

            Vector2 pos = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 23) - Main.screenPosition;
            Rectangle target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.7f));
            Rectangle targetSmaller = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 0.3f));

            Vector2 origin2 = new Vector2(0, LaserTexture.Height / 2);
            Main.spriteBatch.Draw(LaserTexture, targetSmaller, null, Color.Black * 0.25f, LaserRotation, origin2, 0, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);
            Main.spriteBatch.Draw(LaserTexture, target, null, Color.White, LaserRotation, origin2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(LaserTextureBlack, targetSmaller, null, Color.Gold with { A = 0 } * 0.2f, LaserRotation, origin2, 0, 0);


            //Flares

            Texture2D flare1 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_01").Value;
            Texture2D flare2 = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStar").Value;
            Texture2D flare12 = Mod.Assets.Request<Texture2D>("Assets/TrailImages/PartiGlow").Value;

            Vector2 startDrawPoint = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * 23) - Main.screenPosition;
            float sinScale = laserWidth < 1f ? 0f : MathF.Sin((float)Main.timeForVisualEffects * 0.06f) * 0.025f;

            //Start
            Main.spriteBatch.Draw(flare12, startDrawPoint, null, Color.Orange with { A = 0 }, timer * 0.05f * Projectile.ai[0], flare12.Size() / 2, 0.02f * laserWidth + sinScale, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare12, startDrawPoint, null, Color.LightGoldenrodYellow with { A = 0 }, timer * 0.075f * Projectile.ai[0], flare12.Size() / 2, 0.013f * laserWidth, spriteEffects, 0.0f);

            //End
            Main.spriteBatch.Draw(flare1, endPoint - Main.screenPosition - new Vector2(0f, Player.gfxOffY), null, Color.Orange with { A = 0 }, timer * 0.07f * Projectile.ai[0], flare1.Size() / 2, 0.007f * laserWidth, spriteEffects, 0.0f);
            Main.spriteBatch.Draw(flare2, endPoint - Main.screenPosition - new Vector2(0f, Player.gfxOffY), null, Color.White with { A = 0 }, timer * 0.07f * Projectile.ai[0], flare2.Size() / 2, 0.03f * laserWidth, spriteEffects, 0.0f);

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
                SoundStyle stylees = new SoundStyle("Terraria/Sounds/Item_117") with { Pitch = .72f, PitchVariance = .11f, Volume = 0.015f * (laserWidth) };
                SoundEngine.PlaySound(stylees, target.Center);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0 || Main.rand.NextBool())
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(laserWidth * 0.15f, laserWidth * 0.15f);

                        Dust orb = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(),
                            randomStart * Main.rand.NextFloat(0.65f, 1.1f), newColor: Color.Goldenrod, Scale: Main.rand.NextFloat(0.45f, 0.55f) * 1f);

                        orb.customData = DustBehaviorUtil.AssignBehavior_PGOBase(
                            rotPower: 0.1f, killEarlyTime: -1, timeBeforeSlow: 2, preSlowPower: 0.90f, postSlowPower: 0.89f, velToBeginShrink: 2f, fadePower: 0.9f,
                            dontDrawOrb: false, glowIntensity: 0.3f, colorFadePower: 0.85f);
                    }
                }

            }
        }

        //Basic line collision
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            if (laserWidth > 5)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * 12,
                    endPoint, laserWidth * 0.5f, ref point);
            }

            return false;
        }
    }

}
