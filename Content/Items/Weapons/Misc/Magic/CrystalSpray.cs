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
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic
{
    public class CrystalSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hoses down enemies with homing water streams.");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.mana = 6;
            Item.autoReuse = true;
            Item.useStyle = 5;
            Item.knockBack = 5f;
            Item.width = 38;
            Item.height = 10;
            Item.useAnimation = 16;
            Item.useTime = 8;
            Item.damage = 38;
            Item.shootSpeed = 12.5f;
            Item.noMelee = true;
            Item.rare = 8;
            Item.value = 5400 * 5;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<CrystalSprayHeldProj>();
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AquaScepter).
                AddIngredient(ItemID.HallowedBar, 8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            //SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/CommonWaterFallLight00") with { Volume = .16f, Pitch = .54f, PitchVariance = .2f, MaxInstances = 1, };
            //SoundEngine.PlaySound(style2);

            //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_21") with { Pitch = .69f, PitchVariance = .27f, };
            //SoundEngine.PlaySound(style, player.Center);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class CrystalSprayHeldProj : ModProjectile
    {
        int timer = 0;
        public int OFFSET = 20; //30
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        float lerpToStuff = 0;
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Spray");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;


            if (Player.channel)
            {
                Projectile.timeLeft++;
                Player.itemTime = 2;
                Player.itemAnimation = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
                }
                direction = Angle.ToRotationVector2();

            } else
            {
                Projectile.active = false;
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

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (timer % 18 == 0 && timer != 0)
            {
                Vector2 vel = new Vector2(12.5f, 0).RotatedBy(direction.ToRotation());
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel * 1.2f, vel.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)), ModContent.ProjectileType<WaterTrailTest>(), 
                    Projectile.damage, 0, Main.myPlayer);

                //dust burst
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                for (int i = 0; i < 8; i++)
                {
                    Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + vel * 2.5f, ModContent.DustType<GlowCircleDust>(), Main.rand.NextVector2Circular(5,5), new Color(30,105,255), 0.35f, 0.7f, 0f, dustShader2);
                    gd.fadeIn = 2;
                    gd.scale *= Main.rand.NextFloat(0.9f, 1.3f);
                }

                //glow val max
                glowVal = 1;

                //sound
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/CommonWaterFallLight00") with { Volume = .16f, Pitch = .54f, PitchVariance = .2f, MaxInstances = 1, };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_21") with { Pitch = .69f, PitchVariance = .27f, };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            glowVal = MathHelper.Lerp(glowVal, 0, 0.13f);
            timer++;
        }

        float glowVal = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalSpray").Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalSprayGlow").Value;
            Texture2D glowMaskWhite = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalSprayGlowWhite").Value;

            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * -17)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();

            Vector2 newOffset = new Vector2(0, 2 * Player.direction).RotatedBy(Angle);

            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float bonusRot = Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver4 * -1; 
            Main.spriteBatch.Draw(texture, new Vector2((int)position.X, (int)position.Y) + newOffset, null, lightColor, direction.ToRotation() + bonusRot, origin, Projectile.scale, myEffect, 0.0f);
            Main.spriteBatch.Draw(glowMask, new Vector2((int)position.X, (int)position.Y) + newOffset, null, Color.White, direction.ToRotation() + bonusRot, origin, Projectile.scale, myEffect, 0.0f);
            Main.spriteBatch.Draw(glowMaskWhite, new Vector2((int)position.X, (int)position.Y) + newOffset, null, Color.DeepSkyBlue * glowVal, direction.ToRotation() + bonusRot, origin, Projectile.scale, myEffect, 0.0f);

            return false;
        }

    }
}
