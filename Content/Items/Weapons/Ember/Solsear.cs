using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class Solsear : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Solsear");
            /* Tooltip.SetDefault("'With great firepower comes great irresponsibility'\n" +
                "Right-click to shoot an exploding magma flare\n" +
                "Aim the tip of the laser into the flare to increase its size\n" +
                "The tip of the laser also deals more damage"); */
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {
            //item.UseSound = SoundID.Item11;
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.crit = 4;
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SolsearHeldProj>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.channel = true;
            Item.shootSpeed = 4f; //2
            Item.scale = 1.15f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25, 0); //-4
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                damage = (int)damage / 2;
            }
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
            if (player.altFunctionUse == 2)
            {
                /*
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MagmaBall>()] < 3)
                {
                    return false;
                }
                */

                Vector2 offset = Vector2.Normalize(velocity).RotatedBy(-1.57f * player.direction) * 10;
                offset += position;
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SolsearHeld2>(), 0, 0, player.whoAmI);

                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SolsearBomb>(), damage, 0, player.whoAmI);
                player.velocity += velocity * -1;

                SoundStyle styleba = new SoundStyle("AerovelenceMod/Sounds/Effects/fireLoopBad") with { Volume = .12f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleba, player.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .45f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, player.Center);

                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .44f, Volume = 0.9f, PitchVariance = 0.11f};
                SoundEngine.PlaySound(styla, player.Center);

                for (int i = 0; i < 5; i++)
                {
                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                    Vector2 vel = velocity.RotatedBy(0);

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(position + muzzleOffset * 0.2f, ModContent.DustType<GlowCircleQuadStar>(), vel * (3f + Main.rand.NextFloat(-1f, 1f) + i),
                        Color.OrangeRed, Main.rand.NextFloat(0.1f * 5, 0.3f * 5), 0.4f, 0f, dustShader);
                    p.noLight = false;
                    //p.velocity += NPC.velocity * (0.8f + Main.rand.NextFloat(-0.1f, -0.2f));
                    p.fadeIn = 30 + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.3f;
                }

                for (int i = 0; i < 12; i++)
                {
                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                    Vector2 vel = velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(position + muzzleOffset * 0.2f, ModContent.DustType<GlowLine1Fast>(), vel * (3f + Main.rand.NextFloat(-1f, 1f)),
                        Color.OrangeRed, Main.rand.NextFloat(0.07f * 0.75f, 0.2f * 0.75f), 0.5f, 0f, dustShader);
                    p.noLight = false;
                    p.fadeIn = 40 + Main.rand.NextFloat(-5, 10);
                    p.velocity *= 0.3f;
                }

                return false;
            }
            else
            {
                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, PitchVariance = 0.11f};
                SoundEngine.PlaySound(styla, player.Center);
                return true;
            }
        }
        public override void HoldItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
                Item.useTime = 40; //10
                Item.useAnimation = 40; //5
            }
            else
            {
                Item.noUseGraphic = true;
            }
        }

        public override bool CanUseItem(Player player)
        {

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;
            GlowmaskUtilities.DrawItemGlowmask(spriteBatch, glowMask, this.Item, rotation, scale);
        }
    }

    public class SolsearRiseDust : GlowCircleRiseFlare
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Flare";

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color black = Color.Black;
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Black, Color.Gray, dust.alpha / 80f * 0.5f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Gray, Color.LightGray * 0.5f, (dust.alpha - 80) / 80f * 0.5f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.color = dust.GetAlpha(Color.Black);           
            dust.alpha += 2;
            dust.velocity.Y += -0.02f;

            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }
    }

    public class SolsearHeld2 : ModProjectile
    {
        //This class exists literally to just draw the glowmask, also this is pmuch copied from SLR
        private bool initialized = false;

        private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

        Player owner => Main.player[Projectile.owner];

        //public override void SetStaticDefaults() => DisplayName.SetDefault("Solsear");

        float justShotPower = 1f;
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 999999;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            owner.heldProj = Projectile.whoAmI;

            if (owner.itemTime <= 1)
                Projectile.active = false;

            Projectile.Center = owner.Center;

            if (!initialized)
            {
                initialized = true;
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.15f, 0.1f), 0f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;

            Vector2 position = (owner.Center + (currentDirection * 22)) - Main.screenPosition;

            Vector2 scale = new Vector2(1f - (0.1f * justShotPower), 1f) * 1f;

            if (owner.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPower * 2f, currentDirection.ToRotation(), glowMask.Size() / 2, scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation() - 3.14f, texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPower * 2f, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

            }

            return false;
        }
    }
}