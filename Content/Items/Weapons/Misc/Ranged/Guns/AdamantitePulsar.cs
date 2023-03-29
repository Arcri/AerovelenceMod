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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{
    public class AdamantitePulsar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Pulsar");
            Tooltip.SetDefault("Right-click to switch between 2 modes");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.damage = 95;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AdamantitePulseShot>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.channel = true;
            Item.shootSpeed = 2f;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23, 4);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (mode == 0)
            {
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useTime = 10;
                Item.useAnimation = 10 * 3;
                Item.noUseGraphic = false;
            }
            /*
            if (player.altFunctionUse == 2)
            {
                mode = mode == 0 ? 1: 0;
                Item.useTime = 1;
                Item.useAnimation = 1;
            } 
            else
            {
                if (mode == 0)
                    Item.noUseGraphic = true;
                else
                    Item.noUseGraphic = false;

                if (currentShot == 3)
                    player.itemTime = 100;
                else
                    player.itemTime = 10;

                Item.useAnimation = 30;
            }
            */
        }

        //mode 0 = Single Shot
        //mode 1 = multi Shot
        int mode = 0;
        int currentShot = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 0;
                player.itemTime = 0;
                player.itemAnimation = 0;

                mode = mode == 0 ? 1 : 0;

                if (mode == 0)
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Charge", false, true);
                else
                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Burst", false, true);


                return false;
            }

            if (mode == 0)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<AdamantitePulsarHeldProj>(), damage, knockback, Main.myPlayer);
            }
            else if (mode == 1)
            {
                //player.itemAnimationMax = Item.useTime * 3;
                //player.itemTime = Item.useTime * 3;
                //player.itemAnimation = Item.useTime * 3;

                int a = Projectile.NewProjectile(null, position + velocity * (20), velocity * 2.5f, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                Main.projectile[a].rotation = velocity.ToRotation();
                if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                {
                    pulse.color = Color.Crimson;
                    pulse.oval = true;
                    pulse.size = 0.65f;
                }

                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 16;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(source, position, velocity * 4, ModContent.ProjectileType<AdamSmallShot>(), (int)(damage * 1.3f), knockback, Main.myPlayer);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_92") with { Pitch = .80f, PitchVariance = .2f, Volume = 0.4f }; 
                SoundEngine.PlaySound(style, player.Center);
                SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .47f, PitchVariance = 0.1f, Volume = 0.6f };
                SoundEngine.PlaySound(style23, player.Center);


                currentShot++;
                if (currentShot == 3)
                {
                    delayTimer = 60;
                    currentShot = 0;

                }
            }

            return false;
        }
        public override void HoldItem(Player player)
        {
            delayTimer--;

        }

        int delayTimer;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            if (delayTimer > 0)
                return false;
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int itemID = ModContent.ItemType<AdamantitePulsar>();

            if (mode == 0)
            {

                TooltipLine modeDesc = new(Mod, "mode", "Charge - Hold to charge a piercing shot, accuracy increasing the longer you charge")
                {
                    
                    OverrideColor = Color.Red,
                };
                tooltips.Add(modeDesc);

                TooltipLine SSline = new(Mod, "SS", "[i:" + ItemID.FallenStar + "] Skill Strike by releasing with perfect timing [i:" + ItemID.FallenStar + "]")
                {
                    OverrideColor = Color.Gold,
                };
                tooltips.Add(SSline);
            }
            else if (mode == 1)
            {
                TooltipLine modeDesc = new(Mod, "mode", "Burst - Fires a burst of three bullets")
                {
                    OverrideColor = Color.Red,
                };
                tooltips.Add(modeDesc);

                TooltipLine SSline = new(Mod, "SS", "[i:" + ItemID.FallenStar + "] Third shot Skill Strikes if the other shots hit the same target [i:" + ItemID.FallenStar + "]")
                {
                    OverrideColor = Color.Gold,
                };
                tooltips.Add(SSline);
            }
        }
    }

    public class AdamantitePulsarHeldProj : ModProjectile
    {
        int timer = 0;
        public int OFFSET = 10; 
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;
        public float skillCritWindow = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Pulsar");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 999999;
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

        float reticleProgress = 0f;
        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];


            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
                }
                direction = Angle.ToRotationVector2();

            } else
            {
                
                if (timer > 0 && Projectile.timeLeft > 100)
                {
                    Projectile.timeLeft = 20;


                    //Shoot Proj
                    float velRot = Angle + (Main.rand.NextFloat(1 - reticleProgress, (1 - reticleProgress) * -1) * 0.5f);
                    Vector2 vel = new Vector2(2, 0).RotatedBy(velRot);
                    int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel * 10, vel * 1.5f, ModContent.ProjectileType<AdamantitePulseShot>(), Projectile.damage * 3, Projectile.knockBack, Main.myPlayer);

                    int a = Projectile.NewProjectile(null, Projectile.Center + vel * 20, vel, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[a].rotation = velRot;
                    if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                    {
                        pulse.color = (skillCritWindow > 0 && reticleProgress == 1) ? Color.Gold : Color.Crimson;
                        pulse.oval = true;
                        pulse.size = 1.4f;
                    }

                    if (skillCritWindow > 0 && reticleProgress == 1)
                    {

                        SoundStyle style23 = new SoundStyle("Terraria/Sounds/Custom/dd2_sky_dragons_fury_shot_0") with { Pitch = .47f, PitchVariance = 0.1f, Volume = 0.6f };
                        SoundEngine.PlaySound(style23, Projectile.Center);

                        SoundStyle styl23e = new SoundStyle("Terraria/Sounds/Item_68") with { Pitch = .65f, PitchVariance = .15f, Volume = 0.3f }; SoundEngine.PlaySound(styl23e);


                        //Main.NewText("skillcrit");
                        Main.projectile[shot].GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                        Main.projectile[shot].GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.None;
                        Main.projectile[shot].GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.glowTargetCenter;
                        Main.projectile[shot].GetGlobalProjectile<SkillStrikeGProj>().impactScale = 0.4f;
                        Main.projectile[shot].GetGlobalProjectile<SkillStrikeGProj>().hitSoundVolume = 0.1f;

                    } else
                    {
                        SoundStyle styl23e = new SoundStyle("Terraria/Sounds/Item_68") with { Pitch = .65f, PitchVariance = .15f, Volume = 0.2f }; SoundEngine.PlaySound(styl23e);

                    }

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Item_92") with { Pitch = .68f, PitchVariance = .15f, Volume = 0.8f }; SoundEngine.PlaySound(style);



                    //Projectile.active = false;
                }

            }


            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);



            reticleFrameCounter++;
            if (reticleFrameCounter >= 2 && reticleFrame != 26)
            {
                reticleFrameCounter = 0;
                reticleFrame = (reticleFrame + 1) % 27;
            }

            if (Player.channel)
                reticleProgress = Math.Clamp(reticleProgress + 0.02f, 0f, 1f);

            if (reticleProgress == 1)
                skillCritWindow--;

            timer++;
        }


        int reticleFrame = 0;
        int reticleFrameCounter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * -17)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();

            Vector2 newOffset = new Vector2(0,2 * Player.direction).RotatedBy(Angle);

            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(texture, new Vector2((int)position.X, (int)position.Y) + newOffset, null, lightColor, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            
            Texture2D ReticleTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/RedL").Value;
            Texture2D ReticleTexRed = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/RedLRed").Value;

            Color col = (skillCritWindow > 0 && reticleProgress == 1) ? Color.Gold : Color.White;

            Main.spriteBatch.Draw(ReticleTex, Main.MouseWorld - Main.screenPosition + new Vector2(0, 100 * (1 - reticleProgress) + 10).RotatedBy(Angle), null, col * (reticleProgress * 0.9f), Angle - MathHelper.PiOver4, ReticleTex.Size() / 2, reticleProgress, SpriteEffects.None, 0.0f); ;
            Main.spriteBatch.Draw(ReticleTex, Main.MouseWorld - Main.screenPosition + new Vector2(0, -100 * (1 - reticleProgress) - 10).RotatedBy(Angle), null, col * (reticleProgress * 0.9f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, ReticleTex.Size() / 2, reticleProgress, SpriteEffects.None, 0.0f); ;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Looks beter but removed for consistency
            //Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Guns/AdamantitePulsar_Glow").Value;
            //Main.spriteBatch.Draw(Glowmask, new Vector2((int)position.X, (int)position.Y) + newOffset, null, Color.White, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);

            Main.spriteBatch.Draw(ReticleTexRed, Main.MouseWorld - Main.screenPosition + new Vector2(0, 100 * (1 - reticleProgress) + 10).RotatedBy(Angle), null, Color.White * (reticleProgress * 0.9f), Angle - MathHelper.PiOver4, ReticleTex.Size() / 2, reticleProgress, SpriteEffects.None, 0.0f); ;
            Main.spriteBatch.Draw(ReticleTexRed, Main.MouseWorld - Main.screenPosition + new Vector2(0, -100 * (1 - reticleProgress) - 10).RotatedBy(Angle), null, Color.White * (reticleProgress * 0.9f), Angle + MathHelper.PiOver4 + MathHelper.PiOver2, ReticleTex.Size() / 2, reticleProgress, SpriteEffects.None, 0.0f); ;


            return false;
        }
    }
 
}