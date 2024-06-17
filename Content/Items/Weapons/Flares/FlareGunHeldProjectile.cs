using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareGunHeldProjectile : ModProjectile
    {

        int maxTime = 50;

        private float OFFSET = 15; //30
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public int upKnock = 10; //Bad name but it is how long the flare will move to the recoil location

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        public bool hasReachedDestination = false;

        bool flipMuzzle = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;

            maxTime = 90;
            Projectile.timeLeft = maxTime;
            Projectile.width = Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f; //1.15f
        }


        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        float muzzlewidth = 1f;
        public override void AI() 
        {
            Player Player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else if (maxTime < 30)
            {
                Projectile.active = false;
            }

            if (maxTime <= 35)
            {
                if (maxTime == 35)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Menu_Tick") with { Pitch = -.38f, PitchVariance = .26f };
                    SoundEngine.PlaySound(style);
                }
                if (maxTime >= 30)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            }
            if (maxTime <= 0)
            {
                Projectile.active = false;
            }

            if (maxTime <= 89)
            {
                if (hasReachedDestination == false)
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, 1f, 0.24f), 0, 0.4f);
                else 
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

                if (lerpToStuff == 0.4f)
                {
                    hasReachedDestination = true;
                }
            }
            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            if (maxTime == 87) OFFSET = 10f; 
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 17f, 0.07f), 0, 15);

            muzzlewidth = MathHelper.Lerp(muzzlewidth, 0, 0.04f);

            maxTime--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.Center - (0.5f * (direction * OFFSET * -0.25f)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
            }

            //var star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;
            //Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.Red * 0.7f, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            if (maxTime > 70)
            {
                if (maxTime == 90)
                    flipMuzzle = Main.rand.NextBool();

                Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/muzzle_05").Value;

                Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
                myEffect.Parameters["uColor"].SetValue(Color.OrangeRed.ToVector3() * (3 + (1 * 0.3f * (maxTime - 85))));
                myEffect.Parameters["uTime"].SetValue(2);
                myEffect.Parameters["uOpacity"].SetValue(0.5f); //0.6
                myEffect.Parameters["uSaturation"].SetValue(1.2f);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
                myEffect.CurrentTechnique.Passes[0].Apply();


                Main.spriteBatch.Draw(texture2, new Vector2(0,5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + position + direction * 38, texture2.Frame(1, 1, 0, 0), Color.Red, direction.ToRotation() + MathHelper.Pi / 2, 
                    texture2.Size() / 2, new Vector2(0.1f * muzzlewidth, 0.1f), flipMuzzle ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                //Reset again to fix tmod bug
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            


            return false;
        }
    }
}
