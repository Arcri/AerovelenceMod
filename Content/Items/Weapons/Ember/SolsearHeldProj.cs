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
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class SolsearHeldProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solsear");
        }
        int maxTime = 50;

        int timer = 0;
        private const int OFFSET = 15; //30
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        public bool hasReachedDestination = false;

        Vector2 storedMousePos = Vector2.Zero;

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

            //storedMousePos = Vector2.SmoothStep(storedMousePos, Main.MouseWorld, 0.12f);
            storedMousePos = Vector2.Lerp(storedMousePos, Main.MouseWorld, 0.42f); //0.08
            Main.NewText(Player.direction);

            Vector2 exhaustLocation;
            if (Player.direction == 1)
                exhaustLocation = new Vector2(-18, -16).RotatedBy(Projectile.rotation) + Player.Center;
            else
                exhaustLocation = new Vector2(-18, 16).RotatedBy(Projectile.rotation) + Player.Center;

            //Dust.NewDustPerfect(exhaustLocation, DustID.AmberBolt, Velocity: Vector2.Zero);

            if (Main.rand.NextBool(1, 40))
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                if (Main.rand.NextBool())
                {
                    Dust m = GlowDustHelper.DrawGlowDustPerfect(exhaustLocation, ModContent.DustType<GlowCircleRise>(),
                        Main.rand.NextVector2CircularEdge(2, 2), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader);
                    m.velocity.Y = Math.Abs(Projectile.velocity.Y) * -1;
                }
                else
                {
                    Dust p = GlowDustHelper.DrawGlowDustPerfect(exhaustLocation, ModContent.DustType<GlowCircleRise>(),
                        Main.rand.NextVector2CircularEdge(2, 2), Color.OrangeRed, Main.rand.NextFloat(0.3f, 0.5f), 0.4f, 0f, dustShader);
                    p.velocity.Y = Math.Abs(Projectile.velocity.Y) * -1;
                }

            }

            if (timer == 0)
            {
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/movingshield_sound") { MaxInstances = 2};
                SoundEngine.PlaySound(style2, Projectile.Center);
                SoundEngine.PlaySound(style2, Projectile.Center);
                SoundEngine.PlaySound(style2, Projectile.Center);
                //MORE POWER AHAHAHAHHA
 

                storedMousePos = Main.MouseWorld;
                Projectile.NewProjectile(null, Player.Center, (storedMousePos - Player.Center).SafeNormalize(Vector2.UnitX) * 2, ModContent.ProjectileType<SolsearLaser>(),
                    80, 1, Main.myPlayer);
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (storedMousePos - (Player.Center)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else
            {
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/movingshield_sound") { Volume = 0.01f };
                SoundEngine.PlaySound(style2, Projectile.Center);
                Projectile.active = false;
            }

            if (maxTime <= 0)
            {
                Projectile.active = false;
            }

            lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.002f), 0, 0.4f);


            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            if (timer < 150)
            {
                ColorLerpValue = Color.Lerp(ColorLerpValue, Color.White * 0f, 0.02f);
            }
            else
            {
                ColorLerpValue = Color.Lerp(ColorLerpValue, Color.White * 0.5f, 0.04f);
                if (timer == 190)
                    timer = 0;
            }

            timer++;
        }

        Color ColorLerpValue = Color.White * 0.5f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;
            Texture2D glowMaskWhite = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMaskWhite").Value;


            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMaskWhite, position, null, ColorLerpValue, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMaskWhite, position, null, ColorLerpValue, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);

            }

            return false;
        }
    }
}
