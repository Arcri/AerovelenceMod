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
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee.HandBlades
{
    public class TetraBlades : ModItem
    {
        private Color col = Color.LimeGreen;
        private bool trueFrontFalseBack = true;
        private bool dash = false;
        private int doubleAttackCount = 0;

        private int dashRefreshCounter = -1;
        private int dashes = 4;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.knockBack = KnockbackTiers.Weak;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.shootSpeed = 14f; //13f

            Item.width = 34;
            Item.height = 34;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarities.EarlyHardmode;
            Item.shoot = ModContent.ProjectileType<TetraBladeStrike>();
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f };

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NeonTetra, 4).
                AddIngredient(ItemID.SoulofLight, 4).
                AddIngredient(ItemID.SoulofNight, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SSline = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes when all 4 dashed are used [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SSline);
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (dashRefreshCounter == 0)
            {
                dashes = 4;

                int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<TetraBladeRefreshFX>(), 0, 0, Main.myPlayer);
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/dd2_phantom_phoenix_shot_2") with { Pitch = .76f, Volume = 0.8f}, player.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = .5f, Volume = 0.05f }; 
                SoundEngine.PlaySound(style, player.Center);
            }

            //Dust while dashing
            if (dashRefreshCounter > 80 && dashRefreshCounter % 2 == 0)
            {
                int p = Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<LineSpark>(), player.velocity.X * 0.1f, player.velocity.Y * 0.1f, newColor: Color.White, Scale: 0.25f);
                Main.dust[p].customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 40, 
                    0.9f, 0.35f);

                Main.dust[p].velocity = player.velocity * 0.15f;
            }

            if (player.itemAnimation == 0)
            {
                player.GetModPlayer<AeroPlayer>().useStyleData = null;
            }
            dashRefreshCounter--;
        }

        public override bool? UseItem(Player player)
        {
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            if (modPlayer.useStyleData == null)
            {
                modPlayer.useStyleInt++;
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (dashes > 0)
                {
                    Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0 };
                    doubleAttackCount = 10;
                    Item.useTime = 20;
                    Item.useAnimation = 20;
                }

            } else
            {
                Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0f, Pitch = 0.8f, PitchVariance = 0.1f };
                if (doubleAttackCount > 0)
                {
                    Item.useTime = 10;
                    Item.useAnimation = 10;
                }
                else
                {
                    Item.useTime = 20;
                    Item.useAnimation = 20;
                }
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                doubleAttackCount = 10;
                dash = true;
            }

            if (!dash)
            {
                float rotationOffset = Main.rand.NextFloat(-0.1f, 0.1f);

                velocity = velocity.RotatedBy(rotationOffset);
                int a = Projectile.NewProjectile(source, position + velocity * 3, Vector2.Zero, ModContent.ProjectileType<TetraBladeStrike>(), damage, knockback, Main.myPlayer);
                Main.projectile[a].rotation = velocity.ToRotation();
                Main.projectile[a].scale = 1f;

                if (Main.projectile[a].ModProjectile is TetraBladeStrike strike)
                    strike.strikeCol = col;

                if (dashes == 0)
                {
                    SkillStrikeUtil.setSkillStrike(Main.projectile[a], 1.3f);
                }

                for (int i = 0; i < 3 + (Main.rand.NextBool() ? 1 : 0); i++)
                {
                    Dust d = Dust.NewDustPerfect(player.Center + velocity * 1, ModContent.DustType<GlowPixelAlts>(), newColor: col, Scale: 0.4f + Main.rand.NextFloat(-0.1f, 0.2f));
                    d.alpha = 2;
                    d.velocity = velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(2f, 4f);
                    d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f));
                }

                if (col == Color.LimeGreen)
                    col = new Color(141, 13, 184);
                else if (col == new Color(141, 13, 184))
                    col = Color.LimeGreen;

                trueFrontFalseBack = !trueFrontFalseBack;
                AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();

                modPlayer.useStyleInt = 0;
                modPlayer.useStyleData = false;

                float soundPitch = -0.07f;

                if (doubleAttackCount > 0)
                {
                    soundPitch = 0.05f;
                    if (doubleAttackCount == 10)
                    {
                        player.itemAnimation = 10;
                        player.itemTime = 10;
                    }
                    doubleAttackCount--;
                }
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f }, player.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Metallic/joker_stab1") with { Volume = .25f, Pitch = soundPitch, PitchVariance = 0.1f, MaxInstances = -1 }; 
                SoundEngine.PlaySound(style, player.Center);

            }
            else
            {
                if (dashes > 0)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    player.velocity = (mousePos - player.Center).SafeNormalize(Vector2.UnitX) * 10; //12


                    int b = Projectile.NewProjectile(null, player.Center, player.velocity.SafeNormalize(Vector2.UnitX) * -0.5f, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[b].rotation = velocity.ToRotation();
                    if (Main.projectile[b].ModProjectile is CirclePulse pulseb)
                    {
                        pulseb.color = Color.White;
                        pulseb.size = 0.25f;
                    }

                    int afg = Projectile.NewProjectile(null, player.Center, player.velocity.SafeNormalize(Vector2.UnitX) * -1f, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);
                    Main.projectile[afg].timeLeft = 8;

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
                        distort.implode = false;
                        distort.scale = 0.2f;
                    }

                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GloogaSlide") with { Volume = 0.4f, Pitch = 0.3f, PitchVariance = 0.2f }, player.Center);


                    player.GiveImmuneTimeForCollisionAttack(15);

                    dashes--;
                    dashRefreshCounter = 100;


                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 vel = velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));

                        Dust p = Dust.NewDustPerfect(player.Center, ModContent.DustType<LineSpark>(), vel.SafeNormalize(Vector2.UnitX) * (-8f + Main.rand.NextFloat(-1f, 1f)),
                            newColor: Color.White, Scale: 0.45f);

                        p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.92f, preShrinkPower: 0.97f, postShrinkPower: 0.85f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 20,
                            0.5f, 0.35f);
                    }
                }

            }

            dash = false;
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.altFunctionUse == 2)
            {
                doubleAttackCount = 10;
                dash = true;                
            }

            if (!dash)
            {
                float itemAngle = (Main.MouseWorld - player.Center).ToRotation();


                TetraPlayer modPlayer = player.GetModPlayer<TetraPlayer>();
                modPlayer.frontArmRotation = itemAngle - MathHelper.PiOver2;

                player.direction = Main.MouseWorld.X > player.Center.X ? 1 : -1;
                


                if (player.itemAnimation > (player.itemAnimationMax / 1.2f))
                {
                    modPlayer.stretchAmount = (int)Player.CompositeArmStretchAmount.None;
                }
                else 
                {
                    if (trueFrontFalseBack)
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, itemAngle - MathHelper.PiOver2);
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.None, itemAngle - MathHelper.PiOver2);

                        modPlayer.stretchAmount = (int)Player.CompositeArmStretchAmount.Full;

                    }
                    else
                    {
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, itemAngle - MathHelper.PiOver2);
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, itemAngle - MathHelper.PiOver2);

                        modPlayer.stretchAmount = (int)Player.CompositeArmStretchAmount.None;

                    }
                }

                /*
                if (player.itemAnimation < (player.itemAnimationMax / 2))
                {
                    if (trueFrontFalseBack)
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, itemAngle - MathHelper.PiOver2);
                    }
                    else
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, 0);
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, itemAngle - MathHelper.PiOver2);

                    }
                }
                else
                {
                    if (trueFrontFalseBack)
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, itemAngle - MathHelper.PiOver2);
                    else
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.None, itemAngle - MathHelper.PiOver2);
                }
                */
            }

            dash = false;
        }

    }

    public class TetraBladeRefreshFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public override bool? CanDamage() { return false; }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.scale = 0.3f;
            Projectile.timeLeft = 1000;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        float alpha = 0f;
        float scale = 0f;
        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].MountedCenter;
            Projectile.rotation += 0.08f;
            if (timer > 10)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.01f), 0, 1);
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.2f, 0.03f), 0, 1f);

            }
            else
            {
                scale = Math.Clamp(MathHelper.Lerp(scale, 0.6f, 0.2f), 0, 0.5f);
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.1f), 0, 1f);

            }

            if (alpha <= 0f || scale <= 0f)
                Projectile.active = false;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0) return false;

            Player player = Main.player[Projectile.owner];


            if (player.HeldItem.type != ModContent.ItemType<TetraBlades>()) { Projectile.active = false; return false; }


            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;

            Vector2 vec2Scale = new Vector2(scale, scale) * 0.35f;

            Vector2 drawPos = Main.GetPlayerArmPosition(Projectile) - Main.screenPosition - new Vector2(0f, player.gfxOffY);


            //If the player is using the tetras we need to adjust the draw because ^ doesn't account for composite arms
            if (player.itemAnimation > 1)
            {
                TetraPlayer modPlayer = player.GetModPlayer<TetraPlayer>();

                drawPos = player.GetFrontHandPosition((Player.CompositeArmStretchAmount)modPlayer.stretchAmount, modPlayer.frontArmRotation) - Main.screenPosition + new Vector2(0f, player.gfxOffY);
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.Green * alpha, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.Green * alpha * 0.2f, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.Green * alpha, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.Green * alpha * 0.2f, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 6f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, Flare.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, Flare.Size() / 2, vec2Scale * Projectile.scale * 3f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class TetraPlayer : ModPlayer
    {
        //Literally just need to sync this info between the wep and the refresh effect
        public float frontArmRotation = 0f;
        public int stretchAmount = 0;
    }
}