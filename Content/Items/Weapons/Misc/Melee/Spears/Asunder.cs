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
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles;
using ReLogic.Content;
using Terraria.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee.Spears
{
    public class Asunder : ModItem
    {
        //bool tick = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asunder");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 38;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Purple;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<AsunderHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           //tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
           return false;
        }

        public override void AddRecipes()
        {
            /*
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
            */
        }
    }

    public class AsunderHeldProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asunder");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 40;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
        }

        public override bool? CanDamage()
        {
            //bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f);
            //return shouldDamage;
            return true;
        }

        bool playedSound = false;

        int timer = 0;


        public float offset = 0;
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;

        public bool hasReachedGoal = false;
        public override void AI()
        { 
            Player player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer && timer <= 5)
            {
                Angle = (Main.MouseWorld - player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            player.ChangeDir(direction.X > 0 ? 1 : -1);

            if (timer > 5)
            {
                if (!hasReachedGoal)
                    offset = Math.Clamp(MathHelper.Lerp(offset, 65f, 0.3f), 0, 55);
                else
                    offset = Math.Clamp(MathHelper.Lerp(offset, 15f, 0.1f), 15, 55);

                if (offset == 55)
                    hasReachedGoal = true;
            }


            direction = Angle.ToRotationVector2();
            Projectile.Center = player.MountedCenter + (direction * offset);
            Projectile.velocity = Vector2.Zero;
            player.itemRotation = direction.ToRotation();

            if (player.direction != 1)
                player.itemRotation -= 3.14f;

            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();

            if (!player.active || player.dead || player.CCed || player.noItems || (hasReachedGoal && offset <= 15)) //15
            {
                Projectile.Kill();
            }

            timer++;

            if (offset >= 35 && !playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.2f, PitchVariance = 0.15f, Volume = 1f }, Projectile.Center);
                //SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = 0.8f }, Projectile.Center);


                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Volume = .1f, Pitch = -.33f, PitchVariance = .15f, };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Heavy_M_a") with { Pitch = .14f, PitchVariance = 0.2f, Volume = 0.3f }; 
                SoundEngine.PlaySound(style2, Projectile.Center);
                //String extra = Main.rand.NextBool() ? "M_a" : "L_a";
                //String soundLocation = Main.rand.NextBool() ? "AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_L_a" : "AerovelenceMod/Sounds/Effects/GGS/Sword_Sharp_L_b";
                //SoundStyle slash = new SoundStyle(soundLocation) with { Pitch = -0.25f, PitchVariance = .3f, Volume = 0.1f };
                //SoundEngine.PlaySound(slash, Projectile.Center);
                playedSound = true;
            }

            

            //Trail
            //trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value;
            //trailColor = Color.WhiteSmoke * 0f;
            //trailPointLimit = 200;
            //trailMaxLength = 300;
            //trailTime = timer * 0.007f;
            //timesToDraw = 2;

            //trailRot = Projectile.rotation + MathHelper.PiOver4;
            //trailPos = Projectile.Center;
            //TrailLogic();

            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //TrailDrawing();

            

            Texture2D Spear = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Spears/AsunderProj");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Spears/AsunderProjAura");

            if (offset > 30)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, Projectile.rotation + MathHelper.PiOver4, Glow.Size() / 2, Projectile.scale * 0.95f, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, Projectile.rotation + MathHelper.PiOver4, Glow.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            }

            Main.spriteBatch.Draw(Spear, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, Spear.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            //Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusherBlade");
            //Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, Color.MediumPurple, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            
            return false;
        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            #region easeExpo
            /*
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;
            */

            #endregion;

            #region easeCircle
            /*
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            }
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            */
            #endregion

            #region easeOutBack
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            #endregion
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num); // 0.3f
            
        }


        bool hasHit = false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

        }
    }
   
}
