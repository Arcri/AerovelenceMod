using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet.SlateAxe
{
    public class SlateAxeRangedHeldProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slate Axe");
        }
        int maxTime = 50;

        private const int OFFSET = 10; //15
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public int upKnock = 10; //Bad name but it is how long the "gun" will move to the recoil location

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            maxTime = 90;
            Projectile.timeLeft = maxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
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
            else if (maxTime < 30)
            {
                Projectile.active = false;
            }

            if (maxTime <= 30)
            {
                Projectile.active = false;
            }

            if (maxTime <= 89)
            {
                if (hasReachedDestination == false)
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, 1.2f, 0.24f), 0, 0.6f);
                else 
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

                if (lerpToStuff == 0.6f)
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

            maxTime--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1.5f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale * 0.8f, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale * 0.8f, effects1, 0.0f);
            }

            return false;
        }
    }
}
