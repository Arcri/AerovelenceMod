/*
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
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using System.Runtime.InteropServices;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Launchers
{
    public class BunnyCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 220;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.shootSpeed = 15f;
            Item.knockBack = KnockbackTiers.Strong;

            Item.width = 58;
            Item.height = 40;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<BunnyCannonShot>();

            Item.value = Item.sellPrice(0, 7, 5, 0);
            Item.rare = ItemRarities.PostPlantDungeon;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] The Energy Cores Skill Strike [i:" + ItemID.FallenStar + "]")
            //{
            //    OverrideColor = Color.Gold,
            //};
            //tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
             //   .AddIngredient(ItemID.Granite, 25)
              //  .AddRecipeGroup("AerovelenceMod:GoldOrPlatinum", 5)
               // .AddIngredient(ItemID.FlintlockPistol, 1)
                //.AddTile(TileID.Anvils)
                //.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            

            //Adjust shot spawn position to be further away from player center
            //But make sure this won't make it clip through tiles
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            //Spawn projectile and 'held' projectile
            Projectile.NewProjectile(null, position, velocity * 0.5f, ModContent.ProjectileType<BunnyCannonShot>(), 0, 0, player.whoAmI);
            //Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<GraniteChunk>(), damage, knockback, player.whoAmI);

            return false;
        }
    }


    public class BunnyCannonHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        int timer = 0;

        //How far away the held projectile is from player
        public float OFFSET = 20;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;

        //Remnants of other held projectiles (upward recoil)
        public float upwardsRecoil = 0;
        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 30;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;
            Projectile.damage = 0;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false; 
        public override bool? CanCutTiles() => false; 

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            KillHeldProjIfPlayerDeadOrStunned(Projectile);


            //Basic Held projectile code
            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer && timer == 0)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            //On frame 2, recoil
            if (timer == 1)
            {
                OFFSET = 5f;
            }
            //Move back to original offset
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 20f, 0.07f), 0, 17);

            direction = Angle.ToRotationVector2();
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();

            glowStrength = Math.Clamp(MathHelper.Lerp(glowStrength, -0.3f, 0.08f), 0f, 1f);

            timer++;
        }

        float glowStrength = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Launchers/BunnyCannon");
            //Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteCannonOverglow");

            Vector2 drawPos = (Projectile.Center - Main.screenPosition) + new Vector2(0f, Player.gfxOffY);

            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(Weapon, drawPos, null, lightColor, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);

            //Main.spriteBatch.Draw(Glow, drawPos, null, Color.White with { A = 0 } * 0.9f * glowStrength, Projectile.rotation, Glow.Size() / 2, Projectile.scale, mySE, 0f);


            return false;
        }
    }

    public class BunnyCannonShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();
        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();

                #region trailInfo

                //Trail1 
                trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/LavaTrailZetta").Value;
                trail1.trailColor = Color.OrangeRed;
                trail1.trailPointLimit = 255;
                trail1.trailWidth = 45;
                trail1.trailMaxLength = 255;
                trail1.timesToDraw = 2;
                trail1.pinch = false;
                trail1.pinchAmount = 0.4f;


                //trail1.gradient = false;
                //trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
                //trail1.shouldScrollColor = true;
                //trail1.gradientTime = (float)Main.timeForVisualEffects * 0.03f;

                //Trail2
                trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                trail2.trailColor = Color.White;
                trail2.trailPointLimit = 175;
                trail2.trailWidth = 25;
                trail2.trailMaxLength = 175;
                trail2.timesToDraw = 2;
                trail2.pinch = false;
                trail2.pinchAmount = 0.4f;

                //trail2.gradient = true;
                //trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
                //trail2.shouldScrollColor = true;
                //trail2.gradientTime = ((float)Main.timeForVisualEffects * 0.02f) + 0.3f;


                #endregion
            }

            trail1.trailTime = timer * 0.03f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            trail2.trailTime = timer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            trail2.TrailLogic();

            if (timer % 2 == 0)
            {
                int trailCount = 10;
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
        public override bool PreDraw(ref Color lightColor)
        {

            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Bunny = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Launchers/BunnyCannonShot");

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (1f - ((1f - progress) * 0.5f)) * Projectile.scale;

                    Color col = Color.DeepSkyBlue * Easings.easeOutCirc(progress);

                    int reverseI = (previousPostions.Count - 1) - i;
                    float size1 = Math.Clamp(Projectile.scale - (reverseI * 0.05f), 0f, 1f);

                    Vector2 size2 = new Vector2(0.25f, 1.15f) * size;


                }

            }
            #endregion

            for (int i = 0; i < 8; i++)
            {
                Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                //Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col * 1f, Projectile.rotation, Chunk.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }

            //Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Chunk.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Bunny, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Bunny.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            
        }
    }

}
*/