using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Desert
{
    public class StaffOfShiftingSandsHeldProj : ModProjectile
    {
        public override bool ShouldUpdatePosition() => Projectile.ai[1] == 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Shifting Sands");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; //WORKJS //WORKS !  1! ! !
        }
        int maxTime = 80;
        public override void SetDefaults()
        {
            Projectile.timeLeft = maxTime;
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float endingAng = 0;
        float currentAng = 0;

        public override void AI()
        {
            //Matenince 
            Player player = Main.player[Projectile.owner];

            //Make arm follow weapon
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - player.MountedCenter).ToRotation() - MathHelper.PiOver2);
            

            //Basic Held Proj stuffs
            player.heldProj = Projectile.whoAmI;
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }
            
            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {
                maxTime = 0;

                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                
                //This is the angle we will compare the mouseDir to 
                Vector2 angleToCompare = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);
                
                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 angToCompareNormalized = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = angleToCompare.AngleTo(angToCompareNormalized) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the weapon item. This is so the swing alternates direction
                if (Projectile.ai[0] == 1)
                {
                    endingAng = startingAng + MathHelper.ToRadians(-1 * AngleSpread / 2);
                    startingAng = startingAng - MathHelper.ToRadians(-1 * AngleSpread / 2);
                }
                else
                {
                    endingAng = startingAng - MathHelper.ToRadians(-1 * AngleSpread / 2);
                    startingAng = startingAng + MathHelper.ToRadians(-1 * AngleSpread / 2);
                }

                currentAng = startingAng;
                //Main.NewText(MathHelper.ToDegrees(sus1.AngleTo(sus2) * 2));
                firstFrame = false;
            }
            
            //wait 1 frame to give it a little more oomf
            if (maxTime >= 1)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians(getProgress());
                else
                    currentAng = startingAng + MathHelper.ToRadians(getProgress());

                angleTimer++;
            }

            if (maxTime % 5 == 0 && maxTime >= 60 && maxTime < 75)
            {
                glowIntensity = glowIntensityTime;
                ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");



                Vector2 projSpawnPos = (Projectile.Center - player.MountedCenter).SafeNormalize(Vector2.UnitX) * 45;

                for (int i = 0; i < 4; i++)
                {
                    int dust1 = GlowDustHelper.DrawGlowDust(projSpawnPos + player.Center, 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0,
                        Color.SandyBrown, Main.rand.NextFloat(0.15f, 0.2f), 0.5f, 0f, dustShader1);
                    Main.dust[dust1].velocity *= 1.25f;
                }

                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_101") with { Volume = .34f, Pitch = .28f, PitchVariance = .3f };
                SoundEngine.PlaySound(style, projSpawnPos + player.Center);

                Vector2 projVel = (Main.MouseWorld - (player.Center + projSpawnPos)).SafeNormalize(Vector2.UnitX) * 10;
                Projectile.NewProjectile(null, projSpawnPos + player.Center, projVel, ModContent.ProjectileType<SandBolt>(), 2, 1, Main.myPlayer);
            }


            Projectile.rotation = currentAng + MathHelper.PiOver4;

            //normal held proj stuff
            Projectile.Center = (currentAng.ToRotationVector2() * 32) + player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = currentAng * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;

            maxTime++;
            glowIntensity--;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack with { Pitch = 0.4f, Volume = 0.35f, PitchVariance = 0.1f }, Projectile.Center);
            //SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { Volume = .3f, PitchVariance = 0.3f, Pitch = -0.3f }, Projectile.Center);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (maxTime > 27 || maxTime < 5)
                return false;

            //Vector2 diff = target.position - Main.player[Projectile.owner].Center;
            //Vector2 toProj = Projectile.Center - Main.player[Projectile.owner].Center;

            return base.CanHitNPC(target);
        }

        float glowIntensityTime = 15f;
        float glowIntensity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            //Don't know why this check is here but I'm to scared to remove it
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 0)
            {

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value; 

                /*
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), new Rectangle(0, 0, texture.Width, texture.Height), Color.HotPink * (1f - progress), Projectile.oldRot[i] + (Projectile.ai[0] == -1 ? 0 : MathHelper.PiOver2 * 3), new Rectangle(0, 0, texture.Width, texture.Height).Size() / 2f, Math.Max(Projectile.scale * (1f - progress), 0.1f), Projectile.ai[0] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                */
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
                //Main.spriteBatch.Draw(glowMask, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            }

            Texture2D glowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Desert/SoSSWhite").Value;
            Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, Color.Gold * (glowIntensity / glowIntensityTime), Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), glowTexture.Size() / 2, Projectile.scale, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


            return false;
        }

        float angleTimer = 0;
        float AngleSpread = 180;
        public float getProgress()
        {
            float halfOfAngle = AngleSpread / 2;
            float stretch = 5; //Will last for about 3 times this

            //float valToReturn = (halfOfAngle * -1) * (float)Math.Cos(angleTimer / 10) + halfOfAngle;

            //float valTest = (240f) * (float)Math.Sqrt(1 - Math.Pow( (angleTimer / 30 * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee)) - 1, 2)); //semicircle
            float valTest = (AngleSpread * -1) * (float)Math.Pow(2 , (angleTimer /  7) * -1) + AngleSpread;

            //-60cos(x/10) + 60
            //The *60 and +60 come from it being half of 1/120

            return valTest;
        }

    }
}
