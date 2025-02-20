using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class RockProjectile : ModProjectile
    {
        private NPC owner;
        private float offsetX;
        private bool initialized = false;

        private Color currentColor;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        bool isntThrown = true;

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.alpha = 0;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 0;
            currentColor = Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/CrystalTumbler/RockProjectileGlow").Value;
            Rectangle frame = texture.Frame(1, 3, 0, Projectile.frame);
            Color drawColor = isntThrown ? currentColor : Color.Aqua;
            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                drawColor,
                Projectile.rotation,
                frame.Size() / 2,
                Projectile.scale + 0.1f,
                SpriteEffects.None,
                0f
            );
            return true;
        }

        public override bool PreAI()
        {
            if (owner == null)
            {
                owner = Main.npc[(int)Projectile.ai[0]];
                if (owner == null || !owner.active || owner.type != ModContent.NPCType<CrystalTumbler>())
                {
                    Projectile.Kill();
                    return false;
                }
            }

            if (!initialized)
            {
                offsetX = Projectile.ai[1] switch
                {
                    0 => -80f,
                    1 => 0f,
                    2 => 80f,
                    _ => 0f,
                };
                initialized = true;
            }

            if (Projectile.localAI[1] < 300)
            {
                Projectile.localAI[1]++;
                Vector2 hoverPosition = new(owner.Center.X + offsetX, owner.position.Y - 100f + 10f * (float)Math.Sin(Main.GameUpdateCount / 30f));
                Vector2 hoverVelocity = (hoverPosition - Projectile.Center) * 0.05f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, hoverVelocity, 0.05f);
                float targetRotation = MathHelper.Clamp(Projectile.velocity.X * 0.05f, MathHelper.ToRadians(-15), MathHelper.ToRadians(15));
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.1f);
                currentColor = Color.Lerp(currentColor, Color.White, 0.05f);
            }
            else
            {
                if (isntThrown)
                {
                    isntThrown = false;

                    SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { Volume = .46f, Pitch = 1f, PitchVariance = 0f, };

                    CrystalTumbler.isAttacking = false;
                    SoundEngine.PlaySound(stylea, Projectile.Center);
                    Projectile.damage = 12;
                    if (Projectile.ai[1] == 2)
                    {
                        ThrowAroundPlayer(Main.player[owner.target]);
                    }
                    else
                    {
                        ThrowAtPlayer(Main.player[owner.target]);
                    }
                }
            }
            if (Projectile.localAI[1] >= 300)
                currentColor = Color.Lerp(currentColor, Color.Black, 0.05f);
            return false;
        }

        private void ThrowAtPlayer(Player player)
        {
            Vector2 direction = player.Center - Projectile.Center;
            direction.Normalize();
            Projectile.velocity = direction * 10f;
            Projectile.tileCollide = true;
            Projectile.netUpdate = true;
        }

        private void ThrowAroundPlayer(Player player)
        {
            Vector2 offset = new(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            offset.Normalize();
            offset *= 100f;

            Vector2 throwPosition = player.Center + offset;
            Vector2 direction = throwPosition - Projectile.Center;
            direction.Normalize();
            Projectile.velocity = direction * 10f;
            Projectile.tileCollide = true;
            Projectile.netUpdate = true;
        }
    }
}