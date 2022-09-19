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

namespace AerovelenceMod.Content.Items.Weapons.SlateSet.SlateAxe
{
    public class SlateAxeRanged : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Axe");
            Tooltip.SetDefault("You shouldn't see this... ");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item34;
            Item.crit = 4;
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 28;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SlateAxeRangedHeldProj>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 10f;
            Item.channel = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SlateChunk>(), damage, knockback, Main.myPlayer);
            
            float aim = velocity.ToRotation() + MathHelper.Pi;


            for (int l = 0; l < 3; l++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<SlateChunk>(), damage, knockback, Main.myPlayer);

            }

            for (int m = 0; m < 10; m++) // m < 9
            {
                float dustRot = aim + 1.57f * 1.5f + Main.rand.NextFloat(-0.6f, 0.6f);


                Dust d = GlowDustHelper.DrawGlowDustPerfect(player.Center - aim.ToRotationVector2() * 35, ModContent.DustType<GlowCircleDust>(), Vector2.One.RotatedBy(dustRot) * (Main.rand.NextFloat(3) + 1),
                    Color.HotPink, 0.40f + Main.rand.NextFloat(0,0.2f), 0.7f, 0f, dustShader); // 0.6
                d.velocity *= 0.45f;
            }
            
            return true;
        }

    }
}