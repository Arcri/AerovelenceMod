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

namespace AerovelenceMod.Content.Items.Weapons.HandBlades
{
    public class TetraBlades : ModItem
    {
        private Color col = Color.Green;
        private bool trueFrontFalseBack = true;

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
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<TetraBladeStrike>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.35f, Pitch = 0.8f, PitchVariance = 0.1f };
            Item.channel = false;
        }

        public override void HoldItem(Player player)
        {
            if (player.itemAnimation == 0)
            {
                player.GetModPlayer<AeroPlayer>().useStyleData = null;
            }
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
            /*
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            if (player.direction == -1)
                position += new Vector2(0, 4).RotatedBy(velocity.ToRotation());
            else
                position += new Vector2(0, -4).RotatedBy(velocity.ToRotation());
            Main.NewText(player.direction);
            */
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.4f));
            int a = Projectile.NewProjectile(source, position + velocity * 3, Vector2.Zero, ModContent.ProjectileType<TetraBladeStrike>(), damage, knockback, Main.myPlayer);
            Main.projectile[a].rotation = velocity.ToRotation();
            Main.projectile[a].scale = 1f;

            if (Main.projectile[a].ModProjectile is TetraBladeStrike strike)
            {
                strike.strikeCol = col;
            }

            if (col == Color.Green)
                col = Color.Purple;
            else if (col == Color.Purple)
                col = Color.Green;

            trueFrontFalseBack = !trueFrontFalseBack;
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            //Main.NewText(modPlayer.useStyleInt);

            modPlayer.useStyleInt = 0;
            modPlayer.useStyleData = false;
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();

            if (modPlayer.useStyleInt > 0)
            {
                if (player.controlUseItem && (modPlayer.useStyleData == null || (bool)modPlayer.useStyleData == true))
                {
                    modPlayer.useStyleInt++;
                    if (player.itemAnimation < 2)
                    {
                        if (player.CheckMana(Item, pay: true))
                        {
                            player.itemAnimation = player.itemAnimationMax;
                            //Not waiting to fire..
                            modPlayer.useStyleData = false;
                        }
                        else
                        {
                            player.itemAnimation = player.itemAnimationMax;
                            //Waiting to fire..
                            modPlayer.useStyleData = true;
                        }
                    }
                }
                else
                {
                    Main.NewText("apple");
                    //Waiting to fire..
                    modPlayer.useStyleData = true;
                    if (player.itemAnimation < (player.itemAnimationMax / 2) - 1)
                    {
                        player.itemAnimation = player.itemAnimationMax;
                    }
                }
            }

            float itemAngle = (Main.MouseWorld - player.Center).ToRotation();
            player.direction = Main.MouseWorld.X > player.Center.X ? 1 : -1;
            /*
            if (player.direction > 0)
                player.itemRotation = itemAngle;
            else
                player.itemRotation = itemAngle + MathHelper.Pi;
            */
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

        public override bool CanUseItem(Player player)
        {

            return true;
        }
    }
}