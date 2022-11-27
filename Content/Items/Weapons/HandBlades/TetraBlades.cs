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

namespace AerovelenceMod.Content.Items.Weapons.HandBlades
{
    public class TetraBlades : ModItem
    {
        private Color col = Color.LimeGreen;
        private bool trueFrontFalseBack = true;
        private bool dash = false;
        private int doubleAttackCount = 0;

        private int dashRefreshCounter = 0;
        private int dashes = 4;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tetra Blades");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.crit = 4;
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<TetraBladeStrike>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f };
            Item.channel = false;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
         {
            if (dashRefreshCounter == 0)
            {
                dashes = 4;
                CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Green, "Dashes Refreshed!", false, true);
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
            }
            else
            {
                if (dashes > 0)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    player.velocity = (mousePos - player.Center).SafeNormalize(Vector2.UnitX) * 10; //12


                    int a = Projectile.NewProjectile(null, player.Center, player.velocity.SafeNormalize(Vector2.UnitX) * -0.5f, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[a].rotation = player.velocity.ToRotation();


                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/TetraSlide") with { Volume = 0.2f, Pitch = 0.4f, PitchVariance = 0.2f }, player.Center);

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
                AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();

                float itemAngle = (Main.MouseWorld - player.Center).ToRotation();
                player.direction = Main.MouseWorld.X > player.Center.X ? 1 : -1;

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
            }


        }


        public override bool CanUseItem(Player player)
        {

            return true;
        }
    }
}