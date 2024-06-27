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
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using System.Runtime.InteropServices;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class GraniteCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.shootSpeed = 5f;
            Item.knockBack = KnockbackTiers.Average;

            Item.width = 40;
            Item.height = 24;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<GraniteChunk>();

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarities.EarlyPHM;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] The Energy Cores Skill Strike [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Granite, 25)
                .AddRecipeGroup("AerovelenceMod:GoldOrPlatinum", 5)
                .AddIngredient(ItemID.FlintlockPistol, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_70") with { Pitch = .48f, };
            SoundEngine.PlaySound(style, player.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_62") with { Pitch = .68f, Volume = 0.5f };
            SoundEngine.PlaySound(style2, player.Center);

            //Spawn dust in oval pattern
            for (int i = 0; i < 16; ++i)
            {
                Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 16)) * new Vector2(1f, 4f);
                spinningpoint5 = spinningpoint5.RotatedBy(velocity.ToRotation());

                Dust dust = Dust.NewDustPerfect(position + spinningpoint5 + velocity * 10, 136, spinningpoint5, 0, Color.Blue, 1.3f);
                dust.noGravity = true;
            }

            //Adjust shot spawn position to be further away from player center
            //But make sure this won't make it clip through tiles
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            //Spawn projectile and 'held' projectile
            Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<GraniteCannonHeldProj>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<GraniteChunk>(), damage, knockback, player.whoAmI);

            return false;
        }
    }


    //A bit messy still but good enough
    public class GraniteCannonHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Projectile.type] = true;
        }

        int timer = 0;

        //How far away the held projectile is from player
        public float OFFSET = 20;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;

        //Remnants of other held projectiles (upward recoil)
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 30;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;
            Projectile.damage = 0;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() { return false; }
        public override bool? CanCutTiles() { return false; }

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            KillHeldProjIfPlayerDeadOrStunned(Projectile);


            //Basic Held projectile code
            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer && timer == 0)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            //On frame 2, recoil
            if (timer == 1)
            {
                OFFSET = 5f;
            }
            //Move back to original offset
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 20f, 0.07f), 0, 17);

            direction = Angle.ToRotationVector2();
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();

            glowStrength = Math.Clamp(MathHelper.Lerp(glowStrength, -0.3f, 0.08f), 0f, 1f);

            timer++;
        }

        float glowStrength = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteCannon");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteCannonOverglow");

            Vector2 drawPos = (Projectile.Center - Main.screenPosition) + new Vector2(0f, Player.gfxOffY);

            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(Weapon, drawPos, null, lightColor, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);

            Main.spriteBatch.Draw(Glow, drawPos, null, Color.White with { A = 0 } * 0.9f * glowStrength, Projectile.rotation, Glow.Size() / 2, Projectile.scale, mySE, 0f);


            return false;
        }
    }

    public class GraniteChunk : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();
            }

            //Check for collision with other granite chunks
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                //Do this check first as it thins the herd the most while also being simple (as opposed to checking collision first)
                if (p.type == Projectile.type)
                {
                    //Found nearby projectile, break both and spawn stars
                    if (p.active && p != null && p.whoAmI != Projectile.whoAmI && p.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        Vector2 projSpawnDir = ((p.Center - Projectile.Center) * 0.5f);

                        //Granite dust
                        for (float m = 0f; m < 6.28f; m += 0.5f)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center + projSpawnDir, DustID.Granite, new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f);
                            dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                            dust.noGravity = true;
                            dust.scale = 1.3f;
                        }

                        //Glow Dust
                        for (int t = 0; t < 8; t++)
                        {
                            Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);

                            Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                            gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5, 
                                preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
                        }

                        SoundStyle style = new SoundStyle("Terraria/Sounds/Item_14") with { Pitch = .27f, Volume = 0.7f, MaxInstances = -1 };
                        SoundEngine.PlaySound(style, Projectile.Center);

                        int core1 = Projectile.NewProjectile(Projectile.GetSource_FromAI(), projSpawnDir + Projectile.Center, projSpawnDir * 0.5f, ModContent.ProjectileType<GraniteCore>(), (int)(Projectile.damage * 1f), Projectile.knockBack * 0.5f, Main.myPlayer);
                        int core2 = Projectile.NewProjectile(Projectile.GetSource_FromAI(), projSpawnDir + Projectile.Center, projSpawnDir * -0.5f, ModContent.ProjectileType<GraniteCore>(), (int)(Projectile.damage * 1f), Projectile.knockBack * 0.5f, Main.myPlayer);

                        SkillStrikeUtil.setSkillStrike(Main.projectile[core1], 1.5f);
                        SkillStrikeUtil.setSkillStrike(Main.projectile[core2], 1.5f);

                        Projectile.active = false;
                        p.active = false;

                    }
                }

                
            }

            if (timer % 2 == 0)
            {
                int trailCount = 10;
                previousRotations.Add(Projectile.rotation);
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Chunk = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteChunk");

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (1f - ((1f - progress) * 0.5f)) * Projectile.scale;

                    Color col = Color.DeepSkyBlue * Easings.easeOutCirc(progress);

                    int reverseI = (previousPostions.Count - 1) - i;
                    float size1 = Math.Clamp(Projectile.scale - (reverseI * 0.05f), 0f, 1f);

                    Main.EntitySpriteDraw(Chunk, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * progress * 0.9f,
                            previousRotations[i], Chunk.Size() / 2f, size1, SpriteEffects.None);

                    Vector2 size2 = new Vector2(0.25f, 1.15f) * size;

                    Main.EntitySpriteDraw(Chunk, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.9f,
                        previousRotations[i], Chunk.Size() / 2f, size2 * 1.5f, SpriteEffects.None);

                    //Main.EntitySpriteDraw(Chunk, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 1.25f * colVal,
                        //previousRotations[i], Chunk.Size() / 2f, vec2Scale * 1.5f, SpriteEffects.None);
                }

            }
            #endregion

            for (int i = 0; i < 8; i++)
            {
                Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col * 1f, Projectile.rotation, Chunk.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Chunk.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.25f, Projectile.rotation, Chunk.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(2f, 2f) * 1.25f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(15, 80, 255), Scale: Main.rand.NextFloat(0.4f, 0.5f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }


            for (float i = 0; i < 6.28f; i += 0.3f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, new Vector2(MathF.Sin(i) * 1.3f, MathF.Cos(i)) * 2.4f);
                dust.velocity *= Main.rand.NextFloat(0.8f, 1.3f);
                dust.noGravity = true;
            }

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_89") with { Pitch = 0.2f, PitchVariance = 0.2f, Volume = 0.4f, MaxInstances = -1 };
            SoundEngine.PlaySound(style2, Projectile.Center);

            int gore1 = ModContent.GoreType<GraniteShard1>();
            int gore2 = ModContent.GoreType<GraniteShard2>();
            int gore3 = ModContent.GoreType<GraniteShard3>();

            for (int g = 1; g < 4; g++)
            {
                int type = g == 1 ? gore1 : (g == gore2 ? gore2 : gore3);
                Vector2 vel = Projectile.velocity * 0.4f + Main.rand.NextVector2Circular(1.5f, 1.5f);
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.UnitY * -2, vel, type, Scale: 0.75f);
            }
        }
    }

    public class GraniteCore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Core = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Twinkle");

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Color col = new Color(125, 198, 255) with { A = 0 } * 0.85f;
                    Main.spriteBatch.Draw(Core, Projectile.oldPos[j] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), null, col, Projectile.rotation + (i == 0 ? 1.57f / 2f : 0f), Core.Size() / 2, (1 - j * 0.15f) * scale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.Draw(Core, Projectile.Center - Main.screenPosition, null, Color.SkyBlue with { A = 0 }, Projectile.rotation, Core.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0f);
            return false;
        }

        float scale = 0f;
        int timer = 0;
        public override void AI()
        {
            if (timer > 25)
            {
                //Home in on nearest enemy
                if (FindNearestNPC(600f, true, false, true, out int index))
                {
                    NPC npc = Main.npc[index];
                    Projectile.velocity *= .98f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * 20f, .1f);

                    //Dont die if we have a target
                    Math.Clamp(Projectile.timeLeft, 2, 1000);
                }
                else
                {
                    Projectile.timeLeft -= 3;
                }

            } else
            {
                Projectile.velocity *= 0.98f;
            }

            scale = Math.Clamp(MathHelper.Lerp(scale, 1.25f, 0.08f), 0f, 1f);

            if (Projectile.velocity.X > 0)
                Projectile.rotation += 0.3f;
            else
                Projectile.rotation -= 0.3f;

            if (timer % 3 == 0)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(1.5f, 1.5f);
                dustVel += Projectile.velocity * 0.6f;

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 3f, fadePower: 0.89f, shouldFadeColor: false);
            }

            timer++;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_hurt_1") with { Pitch = .4f, MaxInstances = -1 }; 
            SoundEngine.PlaySound(style, Projectile.Center);

            SoundEngine.PlaySound(SoundID.Item93 with { Pitch = 0.4f, Volume = 0.2f, MaxInstances = -1 }, Projectile.Center);

            for (int i = 0; i < 6; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2.25f, 2.25f) * Main.rand.Next(1, 3);
                dustVel += Projectile.velocity * 0.3f;

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.35f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 1f, fadePower: 0.92f, shouldFadeColor: false);
            }
        }


        private bool FindNearestNPC(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
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
                    //Target and NPC friendliness are same
                    if (npc.friendly == targetIsFriendly)
                    {
                        //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                        if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                        {
                            //cache this
                            float compDist = Projectile.DistanceSQ(npc.Center);
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
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }

    }

    public class GraniteShard1 : ModGore
    {
        public override string Texture => "AerovelenceMod/Content/Gores/GraniteShard1";

        public override bool Update(Gore gore)
        {
            if (gore.timeLeft > 30)
                gore.alpha += 4;

            if (gore.alpha >= 250)
                gore.active = false;
            gore.timeLeft += 1;

            return base.Update(gore);
        }
    }
    public class GraniteShard2 : GraniteShard1 { public override string Texture => "AerovelenceMod/Content/Gores/GraniteShard2"; }
    public class GraniteShard3 : GraniteShard1 { public override string Texture => "AerovelenceMod/Content/Gores/GraniteShard3"; }


}