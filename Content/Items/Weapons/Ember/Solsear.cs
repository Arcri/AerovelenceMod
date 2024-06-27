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
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using System.Collections.Generic;

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


        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.knockBack = 1f; //Very weak
            Item.width = 92;
            Item.height = 30;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.shootSpeed = 4f; 
            Item.scale = 1.15f;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<SolsearHeldProj>();
            Item.rare = ItemRarities.EarlyHardmode;
            Item.value = Item.sellPrice(0, 4, 50, 0);

            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;

        }
        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Bomb Skill Strikes at maximum size [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ItemID.SoulofLight, 7).
                AddRecipeGroup("AerovelenceMod:MechSouls", 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                damage = (int)(damage * 0.5f);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {

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


                for (int i22 = 0; i22 < 10; i22++) //4 //2,2
                {
                    Color col = Main.rand.NextBool(2) ? new Color(255, 45, 0) : Color.OrangeRed;

                    Dust p = Dust.NewDustPerfect(position + velocity.SafeNormalize(Vector2.UnitX) * 5, ModContent.DustType<LineSpark>(),
                        velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-1.8f, 1.8f)) * Main.rand.Next(4, 12),
                        newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);
                    p.velocity += velocity * (2.45f + Main.rand.NextFloat(-0.1f, -0.2f));

                    p.customData = AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                        1f, 0.75f);

                }

                for (int i = 0; i < 2; i++)
                {
                    int b = Projectile.NewProjectile(null, position, velocity * 0.65f, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[b].rotation = velocity.ToRotation();
                    if (Main.projectile[b].ModProjectile is CirclePulse pulseb)
                    {
                        pulseb.color = new Color(255, 60, 5);
                        pulseb.size = 0.3f;
                    }
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
        public override string Texture => "Terraria/Images/Projectile_0";


        //This class exists literally to just draw the glowmask on m2
        private bool initialized = false;

        private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

        Player owner => Main.player[Projectile.owner];

        //public override void SetStaticDefaults() => DisplayName.SetDefault("Solsear");

        float justShotPower = 1f;
        float justShotPowerWeaker = 1f;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

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

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.25f, 0.2f), 0f, 1f);
            justShotPowerWeaker = Math.Clamp(MathHelper.Lerp(justShotPowerWeaker, -0.2f, 0.1f), 0f, 1f);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/Solsear").Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;

            Vector2 position = (owner.Center + (currentDirection * 22)) - Main.screenPosition;

            Vector2 scale = new Vector2(1f - (0.15f * justShotPower), 1f) * 1f;

            if (owner.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPowerWeaker * 2f, currentDirection.ToRotation(), glowMask.Size() / 2, scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation() - 3.14f, texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPowerWeaker * 2f, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

            }

            return false;
        }
    }
}