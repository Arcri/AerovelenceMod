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
using AerovelenceMod.Common.Utilities;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Staff");
            Tooltip.SetDefault("Hold Right-click to charge a boulder \nPress Left-click to summon four close-ranged rocks");
            Item.staff[Item.type] = true;

        }
        public override bool AltFunctionUse(Player player) => true;
        bool tick = false;


        public override void SetDefaults()
        {
            //Item.UseSound = SoundID.Item5;
            Item.crit = 0;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 40; //5
            Item.UseSound = SoundID.Item88.WithPitchOffset(0.2f);
            Item.useAnimation = 40; //5
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.mana = 16;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlateStaffHeldProj>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;

        }

        public override void HoldItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = false;
                Item.mana = 4;
                Item.useTime = 10; //10
                Item.useAnimation = 40; //5
                Item.UseSound = SoundID.Item88.WithPitchOffset(0.2f).WithVolumeScale(0f);

            }
            else
            {
                Item.UseSound = SoundID.Item88.WithPitchOffset(0.2f);
                Item.noUseGraphic = true;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(SoundID.Item88.WithPitchOffset(0.2f).WithVolumeScale(0.5f), position);

                tick = !tick;

                int p = 0;
                if (tick)
                {

                    p = Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.UnitX).RotatedBy(-1 * MathHelper.TwoPi / 3) * 85, velocity.SafeNormalize(Vector2.UnitX).RotatedBy(-1 * MathHelper.PiOver2) * 85, ModContent.ProjectileType<StaffRock>(), damage / 2, knockback, Main.myPlayer);


                    for (int i = 0; i < 3; i++)
                    {
                        int d = GlowDustHelper.DrawGlowDust(position + velocity.SafeNormalize(Vector2.UnitX).RotatedBy(-1 * MathHelper.TwoPi / 3.6) * 85, 2, 2, ModContent.DustType<GlowCircleFlare>(),
                            new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X,
                            new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, Color.DeepPink, 0.3f, dustShader);
                        Main.dust[d].velocity *= 0.1f;
                    }
                }
                else
                {
                    p = Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.TwoPi / 3) * 85, velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 85, ModContent.ProjectileType<StaffRock>(), damage / 2, knockback, Main.myPlayer);

                    for (int i = 0; i < 3; i++)
                    {
                        int d = GlowDustHelper.DrawGlowDust(position + velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.TwoPi / 3.6) * 85, 2, 2, ModContent.DustType<GlowCircleFlare>(),
                            new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).X,
                            new Vector2(Main.rand.NextFloat(8, 13), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))).Y, Color.DeepPink, 0.3f, dustShader);
                        Main.dust[d].velocity *= 0.1f;
                    }

                }

                if (Main.projectile[p].ModProjectile is StaffRock rock){
                    rock.setDestination(velocity.SafeNormalize(Vector2.UnitX) * 85);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
            }

            return false;
        }
    }
}
