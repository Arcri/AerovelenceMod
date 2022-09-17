using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateSwordHeldProj : ModProjectile
    {
        public override bool ShouldUpdatePosition() => Projectile.ai[1] == 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Sword");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; //WORKJS //WORKS !  1! ! !
        }
        int maxTime = 25;
        public override void SetDefaults()
        {
            maxTime = 30;
            Projectile.timeLeft = maxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1.15f;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float endingAng = 0;
        float currentAng = 0;

        public override void AI()
        {
            //Matenince 
            Player player = Main.player[Projectile.owner];
            //This make the arm point to the sword but apparently isn't completely accurate so ill do that later
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

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
                
                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);
                
                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the slate set. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    endingAng = startingAng + MathHelper.ToRadians(-90);
                    startingAng = startingAng - MathHelper.ToRadians(-90);
                }
                else
                {
                    endingAng = startingAng - MathHelper.ToRadians(-90);
                    startingAng = startingAng + MathHelper.ToRadians(-90);
                }

                currentAng = startingAng;

                //Main.NewText(MathHelper.ToDegrees(sus1.AngleTo(sus2) * 2));
                firstFrame = false;
            }
            
            //wait 2 frames to give it a little more oomf
            if (maxTime > 2)
            {
                //the actual lerping
                float sussybaka = (maxTime * 0.02f);
                currentAng = MathHelper.Lerp(currentAng, endingAng, sussybaka);
            }
            Projectile.rotation = currentAng + MathHelper.PiOver4;

            //normal held proj stuff
            Projectile.Center = (currentAng.ToRotationVector2() * 30) + player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = currentAng * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;

            float offset = 30;
            float start;
            float end;

            if (maxTime < 3)
                Projectile.scale = 1.15f; //12

            if (maxTime > 3)
                Projectile.scale = 1.2f; //345

            if (maxTime >= 6)
                Projectile.scale = 1.55f; //678

            if (maxTime >= 9)
                Projectile.scale = 1.65f; //91011

            if (maxTime >= 12)
                Projectile.scale = 1.55f; //121314

            if (maxTime >= 15)
                Projectile.scale = 1.2f;

            if (maxTime >= 18)
                Projectile.scale = 1.15f;


            if (maxTime == 9)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 5, ModContent.ProjectileType<SlateChunk>(), Projectile.damage / 2, 1, Main.myPlayer);
            }

            if (maxTime > 15)
                Projectile.damage = 0;

            maxTime++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*for (int i = 0; i < 20; i++)
            {
                Vector2 dirToTarger = Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.UnitX);

                Vector2 spawnOffset = dirToTarger.RotatedBy(MathHelper.ToRadians(180)) * (target.width / 2) * (target.height/2);



                for (int ar = 0; ar < 10; ar++)
                {
                    Vector2 rawprojdir = dirToTarger;
                    Vector2 dustydiry = dirToTarger.RotatedBy(MathHelper.ToRadians(180));
                    Dust m = Dust.NewDustPerfect(target.Center, DustID.Stone, rawprojdir * (1.1f + (ar * 1.5f) / 10), Alpha: 100, Scale: 1.25f);
                    Dust n = Dust.NewDustPerfect(target.Center, DustID.Stone, dustydiry * (1.1f + (ar * 1.5f) / 10), Alpha: 100, Scale: 1.25f);
                    m.noGravity = true;
                    n.noGravity = true;
                }
                //Dust d = Dust.NewDustPerfect()

                //Vector2 dirToProj = (Projectile.Center - target.Center).SafeNormalize(Vector2.UnitX);
                //Dust dust = Dust.NewDustDirect(target.Center, target.width, target.height, DustID.Stone, 0, 0, Projectile.alpha);
                //dust.velocity *= 0.55f;
                //dust.velocity += Projectile.velocity * 0.5f;
                //dust.scale *= 1.25f;
                //dust.noGravity = true;
            }*/
            //SoundEngine.PlaySound(new SoundStyle("Redux/Sounds/Item/RockCollide") with { Volume = .1f, PitchVariance = 0.3f, Pitch = -0.3f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/SlateSlice1").Value;

            float toAlphaMult = 0.0f;

            switch (Projectile.scale)
            {
                case 1.2f:
                    toAlphaMult = 0.2f;
                    break;
                case 1.55f:
                    toAlphaMult = 0.3f;
                    break;
                case 1.65f:
                    toAlphaMult = 0.5f;
                    break;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition + (new Vector2(1, -1) * Projectile.scale), null, lightColor * toAlphaMult, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 0)
            {
                Player player = Main.player[Projectile.owner];

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                //{
                 //   float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                 //   Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + new Vector2(16,18), new Rectangle(0, 0, texture.Width, texture.Height), lightColor * (1f - progress), Projectile.rotation, new Rectangle(0, 0, texture.Width, texture.Height).Size() / 2f, Math.Max(Projectile.scale * (1f - progress), 0.1f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                //}
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] == -1 ? 0 : MathHelper.PiOver2 * 3), texture.Size() / 2, Projectile.scale, Projectile.ai[0] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }
            
            return false;
        }
    }
}
