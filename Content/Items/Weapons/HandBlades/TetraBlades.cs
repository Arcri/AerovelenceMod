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
using AerovelenceMod.Content.Items.Weapons.SlateSet;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.HandBlades
{
    public class TetraBlades : ModItem
    {
        private Color col = Color.LimeGreen;
        private bool trueFrontFalseBack = true;
        private bool dash = false;
        private int doubleAttackCount = 0;

        private int dashRefreshCounter = -1;
        private int dashes = 4;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tetra Blades");
            // Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;

            Item.width = 28;
            Item.height = 28;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = Common.ItemStatHelper.RarityPreMechs;

            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<TetraBladeStrike>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f };
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SSline = new(Mod, "SS", "[i:" + ItemID.FallenStar + "] Skill Strikes when all 4 dashed are used [i:" + ItemID.FallenStar + "]")
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
                //CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Green, "Dashes Refreshed!", false, true);

                int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<TetraBladeRefreshFX>(), 0, 0, Main.myPlayer);
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Custom/dd2_phantom_phoenix_shot_2") with { Pitch = .76f, Volume = 0.8f}, player.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = .5f, Volume = 0.05f }; 
                SoundEngine.PlaySound(style, player.Center);
                //dashRefreshCounter = -1;
            }

            //dust while dashing
            if (dashRefreshCounter > 80 && dashRefreshCounter % 2 == 0)
            {
                int a = Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<DashTrailDust>(), player.velocity.X * 0.1f, player.velocity.Y * 0.1f,  newColor: Color.White, Scale: 1f);
                Main.dust[a].velocity = player.velocity * 0.1f;
                Main.dust[a].scale = 1f;

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

        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);// && player.GetModPlayer<AeroPlayer>().useStyleInt != 0 && !player.controlUseItem && player.itemAnimation < player.itemAnimationMax && player.GetModPlayer<AeroPlayer>().useStyleData.Equals(true);
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
            //col = new Color(141, 13, 184);
            //col = new Color(210, 101, 255);
            if (!dash)
            {
                float rotationOffset = doubleAttackCount > 0 ? Main.rand.NextFloat(-0.1f, 0.2f) : Main.rand.NextFloat(-0.3f, 0.4f);

                velocity = velocity.RotatedBy(rotationOffset);
                int a = Projectile.NewProjectile(source, position + velocity * 3, Vector2.Zero, ModContent.ProjectileType<TetraBladeStrike>(), damage, knockback, Main.myPlayer);
                Main.projectile[a].rotation = velocity.ToRotation();
                Main.projectile[a].scale = 1f;

                if (Main.projectile[a].ModProjectile is TetraBladeStrike strike)
                {
                    strike.strikeCol = col;
                }

                if (dashes == 0)
                {
                    Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                    Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.None;
                    Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.glowTargetCenter;
                    Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().impactScale = 0.35f;
                    Main.projectile[a].GetGlobalProjectile<SkillStrikeGProj>().hitSoundVolume = 0.9f;

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

                if (doubleAttackCount > 0)
                {
                    if (doubleAttackCount == 10)
                    {
                        player.itemAnimation = 10;
                        player.itemTime = 10;
                    }
                    doubleAttackCount--;
                }
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f }, player.Center);

                SoundStyle styl1e = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_06") with { Pitch = .1f, PitchVariance = .42f, MaxInstances = 0, Volume = 0.1f}; 
                SoundEngine.PlaySound(styl1e, player.Center);


                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/TF2/katana_impact_object_03") with { Volume = .17f, Pitch = .2f, PitchVariance = 0.2f, MaxInstances = 0 };
                SoundEngine.PlaySound(style2, player.Center);

                //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Heavy_M_a") with { Pitch = .14f, PitchVariance = .16f, Volume = 0.25f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style, player.Center);


            }
            else
            {
                if (dashes > 0)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    player.velocity = (mousePos - player.Center).SafeNormalize(Vector2.UnitX) * 10; //12

                    for (int m = 0; m < 2; m++)
                    {
                        int a = Projectile.NewProjectile(null, player.Center, player.velocity.SafeNormalize(Vector2.UnitX) * -0.5f, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                        Main.projectile[a].rotation = player.velocity.ToRotation();
                        if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                        {
                            pulse.color = Color.White;
                            pulse.oval = true;
                            pulse.size = 1f;
                        }
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

                    for (int i = 0; i < 1; i++) //4 //2,2
                    {
                        ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                        Vector2 vel = velocity.RotatedBy(0);

                        Dust p = GlowDustHelper.DrawGlowDustPerfect(player.Center, ModContent.DustType<GlowCircleQuadStar>(), vel.SafeNormalize(Vector2.UnitX) * (-8f + Main.rand.NextFloat(-1f, 1f) + i),
                            Color.White, Main.rand.NextFloat(0.1f * 3, 0.3f * 3), 0.4f, 0f, dustShader);
                        p.noLight = false;
                        //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                        p.fadeIn = 10 + Main.rand.NextFloat(-5, 10);
                        p.velocity *= 0.4f;
                    }

                    for (int i = 0; i < 7; i++) //4 //2,2
                    {
                        ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                        Vector2 vel = velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));

                        Dust p = GlowDustHelper.DrawGlowDustPerfect(player.Center, ModContent.DustType<GlowLine1Fast>(), vel.SafeNormalize(Vector2.UnitX) * (-8f + Main.rand.NextFloat(-1f, 1f)),
                            Color.White, Main.rand.NextFloat(0.07f * 0.75f, 0.2f * 0.75f), 0.5f, 0f, dustShader);
                        p.noLight = false;
                        p.fadeIn = 10 + Main.rand.NextFloat(-10, 15);
                        p.velocity *= 0.4f;
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


        }


        public override bool CanUseItem(Player player)
        {

            return true;
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
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.3f;
            Projectile.timeLeft = 1000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
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

            Vector2 drawPos = Main.GetPlayerArmPosition(Projectile) - Main.screenPosition;


            //If the player is using the tetras we need to adjust the draw because ^ doesn't account for composite arms


            if (player.itemAnimation > 1)
            {
                TetraPlayer modPlayer = player.GetModPlayer<TetraPlayer>();

                drawPos = player.GetFrontHandPosition((Player.CompositeArmStretchAmount)modPlayer.stretchAmount, modPlayer.frontArmRotation) - Main.screenPosition;
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