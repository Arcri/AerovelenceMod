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
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class Aliban : ModItem
    {
        int count = 0;
        bool tick = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aliban");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
            {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<AlibanSwing>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));


            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class AlibanSwing : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aliban");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12; //9
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 4; //2
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f);
            return shouldDamage;
        }

        public bool shouldShoot = false;
        bool hasShot = false;

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;
        float origAng = 0;

        float Angle = 0;

        bool playedSound = false;

        float storedDirection = 1;

        int timer = 0;
        float offset = 0;
        int timerAfterEnd = 20;
        float alpha = 0;
        public override void AI()
        { 
            Player player = Main.player[Projectile.owner];
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Projectile.Center - (player.Center)).ToRotation();
            }

            if (firstFrame)
                storedDirection = player.direction;

            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = Angle + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            //if (easingProgress > 0.5)
                //player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            player.heldProj = Projectile.whoAmI;

            //player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {

                easingProgress = 0.01f;

                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);

                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                origAng = startingAng;
                //we set Projectile.ai[0] in the wep. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    startingAng = startingAng - MathHelper.ToRadians(-135);
                }
                else
                {
                    startingAng = startingAng + MathHelper.ToRadians(-135);
                }

                currentAng = startingAng;
                firstFrame = false;
            }


            if (timer >= 8)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((270 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((270 * getProgress(easingProgress)));

                easingProgress = Math.Clamp(easingProgress + 0.004f * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 0.00f, 0.95f); // 0.008
            }

            offset = 40;
            alpha = 1;

            //offset = Math.Clamp(MathHelper.Lerp(offset, 42, 0.08f), 0, 40);
            //alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.08f), 0, 1);


            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * offset) + player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0,-5);
            player.itemTime = 10;
            player.itemAnimation = 10;

            timer++;

            if (getProgress(easingProgress) >= 0.35f && getProgress(easingProgress) <= 0.78f)
            {
                Projectile.ai[1] = 1;
            }
            else
                Projectile.ai[1] = 0;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.2f, PitchVariance = 0.15f, Volume = 0.67f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = 0.8f }, Projectile.Center);

                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f)
            {
                if (timerAfterEnd > 10)
                {
                    //offset = Math.Clamp(MathHelper.Lerp(offset, 0, 0.06f), 20, 40);

                }

                if (timerAfterEnd == 0)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    Projectile.active = false;
                }
                timerAfterEnd--;
                
            }

            if (timer % 10 == 0 && timer != 0 && timer < 50)
            {
                //int roA = Projectile.NewProjectile(null, Projectile.Center, currentAng.ToRotationVector2() * 2, ModContent.ProjectileType<RoAHit>(), 5, 0, player.whoAmI);

                //if (Main.projectile[roA].ModProjectile is RoAHit hit)
                //{
                    //hit.color = Color.LightBlue;
                //}
            }

            if (timer % 2 == 0 && timer < 30)
            {
                //Dust dust2 = Dust.NewDustPerfect(Projectile.Center + currentAng.ToRotationVector2() * 20, ModContent.DustType<StillDust>(),
                //currentAng.ToRotationVector2() * 2, newColor: Color.SkyBlue);
                //dust2.rotation = Main.rand.NextFloat(6.28f);
                //dust2.scale = 1.1f;
                //dust2.alpha = 50;
            }


            if (getProgress(easingProgress) >= 0.5f && !hasShot && shouldShoot)
            {
                hasShot = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, origAng.ToRotationVector2() * 5f, ModContent.ProjectileType<AlibanProj>(), 
                    Projectile.damage, 0, Projectile.owner);
            }

            if (swipeTimer % 10 == 0 && swipeTimer != 0)
            {
                trailFrame = Math.Clamp(trailFrame + 1, 0, 4);
            }
            swipeTimer++;
            swipeIntensity = Math.Clamp(MathHelper.Lerp(swipeIntensity, -0.2f, 0.025f), 0, 1);
        }

        float swipeIntensity = 0.65f;
        int swipeTimer = 0;
        int trailFrame = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Aliban");
            Texture2D Trail = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/SwordArcSwipe");
            Texture2D Trail2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/SwordFullSwipe");
            Texture2D Trail3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/SwordFullSwipeAnim");

            Rectangle sFrame = new Rectangle(0, Trail3.Height / 4 * trailFrame, Trail3.Width, (Trail3.Height / 4));

            float scaleVal = ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.2f);
            Vector2 scale2 = new Vector2(0.75f + scaleVal, 0.9f + scaleVal - (swipeTimer * 0.015f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Trail3, Main.player[Projectile.owner].MountedCenter - Main.screenPosition + new Vector2(10f + swipeTimer, 0).RotatedBy(origAng), sFrame, Color.Green * swipeIntensity * 0.5f, origAng, Trail2.Size() / 2, scale2 * 0.7f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Trail3, Main.player[Projectile.owner].MountedCenter - Main.screenPosition + new Vector2(10f + swipeTimer, 0).RotatedBy(origAng), sFrame, Color.LightGreen * swipeIntensity * 0.7f, origAng, Trail2.Size() / 2, scale2 * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Trail3, Main.player[Projectile.owner].MountedCenter - Main.screenPosition + new Vector2(10f + swipeTimer, 0).RotatedBy(origAng), sFrame, Color.LightBlue * swipeIntensity, origAng, Trail2.Size() / 2, scale2, SpriteEffects.None, 0f);

            Player p = Main.player[Projectile.owner];

            if (timer > 5)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    k++;
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(30f, 30f);
                    Color trailCol = Color.LightCyan * 0.45f;
                    Main.EntitySpriteDraw(Sword, drawPos, null, trailCol, Projectile.oldRot[k] + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Sword.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.1f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                    k++;
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);




            //Main.spriteBatch.Draw(Trail, Projectile.Center - Main.screenPosition + new Vector2(-30,0).RotatedBy(Projectile.rotation), null, Color.White * 0.5f, Projectile.rotation - MathHelper.PiOver4 + 0.05f, Sword.Size() / 2, 0.75f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


            Main.spriteBatch.Draw(Sword, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Sword.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.1f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            

            return false;
        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            return 1f - (float)Math.Pow(1 - x, 6);

            #region easeExpo


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


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            for (int i = 0; i < 2 + (Main.rand.NextBool() ? 1 : 0); i++)
            {
                Vector2 vel = (target.Center - Main.player[Projectile.owner].Center).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
                int roA = Projectile.NewProjectile(null, target.Center, vel.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1.5f,3.1f), ModContent.ProjectileType<RoAHit>(), 0, 0);

                if (Main.projectile[roA].ModProjectile is RoAHit hit)
                {
                    hit.color = Color.DeepSkyBlue;
                    hit.Projectile.frameCounter = Main.rand.Next(1, 3);
                }
            }

        }
    }

    public class AlibanProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            DisplayName.SetDefault("Aliban");
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 70;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 17;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
        }


        int timer = 0;
        float alpha = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer < 10)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.2f, 0.08f), 0, 1);
                Projectile.velocity *= 1.03f;

            }
            else
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.13f), 0, 1);
                Projectile.velocity *= 0.7f;

            }
            Projectile.rotation += 0.25f * Projectile.direction;

            timer++;

            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/AlibanProj");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/AlibanProj_Glow");
            Main.spriteBatch.Draw(Sword, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, Sword.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            if (timer > 5)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(15f, 15f);
                    Color trailCol = Color.LightCyan;
                    Main.EntitySpriteDraw(Glow, drawPos, null, trailCol, Projectile.oldRot[k], Sword.Size() / 2, Projectile.scale,SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            for (int i = 0; i < 2 + (Main.rand.NextBool() ? 1 : 0); i++)
            {
                Vector2 vel = (target.Center - Main.player[Projectile.owner].Center).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
                int roA = Projectile.NewProjectile(null, target.Center, vel.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1.5f, 3.1f), ModContent.ProjectileType<RoAHit>(), 0, 0);

                if (Main.projectile[roA].ModProjectile is RoAHit hit)
                {
                    hit.color = Color.DeepSkyBlue;
                    hit.Projectile.frameCounter = Main.rand.Next(1, 3);
                }
            }

        }
    }

}
