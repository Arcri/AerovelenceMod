﻿using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.TempVFX;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Sky
{
    public class PoundOfFeathers : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.knockBack = KnockbackTiers.ExtremelyWeak;
            Item.mana = 3;
            
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.shootSpeed = 10f;


            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<PoundOfFeathersProj>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.sellPrice(0, 0, 50, 0);

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes at close range [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int feather = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1f) * 1f, ModContent.ProjectileType<PoundOfFeathersProj>(), damage, knockback, Main.myPlayer);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_1") with { Pitch = .89f, PitchVariance = .33f, }; 
            SoundEngine.PlaySound(style, player.Center);
            
            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShadowScale, 5).
                AddIngredient(ItemID.Silk, 20).
                AddIngredient(ItemID.Feather, 15).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.TissueSample, 5).
                AddIngredient(ItemID.Silk, 20).
                AddIngredient(ItemID.Feather, 15).
                AddTile(TileID.Anvils).
                Register();
        }

    }

    public class PoundOfFeathersProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 75;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? CanDamage() { return !stuckIn; }
        public override void AI()
        {
            //Initialize lists
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();
            }

            //Normal behavior
            if (!stuckIn)
            {
                //Home towards cursor 
                if (timer < 70)
                {
                    //Home a little stronger after half a second
                    float turnPower = 25f;
                    int turn2 = timer < 50f ? 30 : 35;

                    Vector2 mousePos = Vector2.Zero;

                    if (Main.myPlayer == Projectile.owner)
                        mousePos = Main.MouseWorld;

                    Vector2 toMouse = (mousePos - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    toMouse *= turnPower;

                    Projectile.velocity = (Projectile.velocity * (turn2 - 1) + toMouse) / turn2;
                    if (Projectile.velocity.Length() < 10f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 10f;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();

                fadeAlpha = Math.Clamp(MathHelper.Lerp(fadeAlpha, 1.5f, 0.15f), 0f, 1f);

                if (timer <= 10)
                    SkillStrikeUtil.setSkillStrike(Projectile, 1.3f, 1, 0.25f, 0f);
                else
                    Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            }
            else
            {
                //Fade out for a little bit instead of immediately dying on hit
                fadeAlpha = Math.Clamp(MathHelper.Lerp(fadeAlpha, -0.5f, 0.15f), 0f, 1f);
            }

            int trailCount = 10;
            if (!stuckIn)
            {
                previousRotations.Add(Projectile.rotation);
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }

            timer++;
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        float fadeAlpha = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Sky/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Sky/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Sky/FeatherWhite").Value;

            Vector2 featherScale = new Vector2(1f, 1f * fadeAlpha) * Projectile.scale;

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;
                    float size = (0.75f + (progress * 0.25f)) * Projectile.scale;

                    Color col = Color.Lerp(Color.Blue, Color.DeepSkyBlue, progress) * progress * fadeAlpha;

                    float size2 = (1f + (progress * 0.25f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FeatherGray, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.55f,
                            previousRotations[i], FeatherGray.Size() / 2f, size2 * featherScale, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(1.5f, 0.25f) * size;
                    Main.EntitySpriteDraw(FeatherWhite, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f,
                        previousRotations[i], FeatherGray.Size() / 2f, vec2Scale, SpriteEffects.None);
                }

            }
            #endregion


            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(FeatherWhite, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, Color.DeepSkyBlue * 0.5f * (fadeAlpha * fadeAlpha), Projectile.rotation, Feather.Size() / 2f, featherScale * 1.05f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, lightColor * fadeAlpha, Projectile.rotation, Feather.Size() / 2f, featherScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Feather, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * fadeAlpha, Projectile.rotation, Feather.Size() / 2f, featherScale, SpriteEffects.None);

            return false;
        }

        bool stuckIn = false;
        public override void OnKill(int timeLeft)
        {
            if (!stuckIn)
                hitFX();  
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitFX();
            stuckIn = true;
            Projectile.timeLeft = 10;
            Projectile.velocity = Vector2.Zero;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (stuckIn) return false;

            hitFX();
            stuckIn = true;
            Projectile.timeLeft = 10;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = previousPostions[previousPostions.Count - 1]; //To make sure it doesn't break on slopes because terraria sucks dick

            return false;
        }

        public void hitFX()
        {
            for (int i = 0; i < 3 + Main.rand.Next(-1, 2); i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(1.5f, 1.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: new Color(0, 100, 255), Scale: Main.rand.NextFloat(0.35f, 0.45f));
                dust.velocity += Projectile.velocity * 0.25f;

                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.88f, shouldFadeColor: false);
            }
            SoundStyle style = new SoundStyle("Terraria/Sounds/Grab") with { Pitch = .89f, PitchVariance = .33f, Volume = 0.35f };
            SoundEngine.PlaySound(style, Projectile.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Hit_11") with { Pitch = .61f, PitchVariance = .26f, MaxInstances = -1, Volume = 0.35f };
            SoundEngine.PlaySound(style2, Projectile.Center);
        }
    }

}