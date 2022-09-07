using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class ICERHeldProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ICER");
        }
        int maxTime = 50;

        int timer = 0;
        private const int OFFSET = 15; //30
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        public bool hasReachedDestination = false;

        int shotTimer = 0;

        public override void SetDefaults()
        {
            maxTime = 90;
            Projectile.timeLeft = maxTime;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1.15f;
        }


        public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI() 
        {
            Player Player = Main.player[Projectile.owner];


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
            else
            {
                Projectile.active = false;
            }

            if (maxTime <= 0)
            {
                Projectile.active = false;
            }

            lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

            /*
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
            */
            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();


            if (shotTimer % 15 == 0 && timer > 0) //10
            {
                Vector2 pos = (Projectile.position - (0.5f * (direction * OFFSET * -0.25f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f).Floor();


                //Vector2 position = new Vector2(0, 0);
                //4f
                //if (Player.direction == -1)
                    //position += new Vector2(0, 4).RotatedBy(Projectile.velocity.ToRotation());
                //else
                    //position += new Vector2(0, -4).RotatedBy(Projectile.velocity.ToRotation());
                //Projectile.NewProjectile(null, new Vector2(0, 5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + pos + direction * 24, new Vector2(2, 0).RotatedBy(direction.ToRotation()), ModContent.ProjectileType<ICERProj>(), 40, 3, Main.myPlayer);
                //Projectile.NewProjectile(null, new Vector2(0, 5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + pos + direction * 24, new Vector2(0.75f, 0).RotatedBy(direction.ToRotation()), ModContent.ProjectileType<ICERProj>(), 40, 3, Main.myPlayer);
                Projectile.NewProjectile(null, new Vector2(0, 5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + pos + direction * 24, new Vector2(2, 0).RotatedBy(direction.ToRotation()), ModContent.ProjectileType<ICERProj>(), 40, 3, Main.myPlayer);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Research_1") with { Volume = .56f, Pitch = .61f, };
                SoundEngine.PlaySound(style);

                //SoundStyle style = new SoundStyle("Terraria/Sounds/Research_3") with { Pitch = .79f, PitchVariance = .12f, };
                //SoundEngine.PlaySound(style, Player.Center);
            }


            //maxTime--;
            shotTimer++;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -0.25f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
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

            Texture2D texture2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Cyan.ToVector3() * 2);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.6f); //0.5
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();


            Main.spriteBatch.Draw(texture2, new Vector2(0, 5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + position + direction * 24, texture2.Frame(1, 1, 0, 0), Color.Cyan, MathHelper.ToRadians(timer),
                texture2.Size() / 2, 0.08f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture2, new Vector2(0, 5 * Player.direction * -1).RotatedBy(direction.ToRotation()) + position + direction * 24, texture2.Frame(1, 1, 0, 0), Color.Cyan, MathHelper.ToRadians(timer * -2),
                texture2.Size() / 2, 0.065f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
