using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
//ExampleSolarEruption

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Electromagnetism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electromagnetism");
            Tooltip.SetDefault("Causes chains of electricity that can hit other enemies, and explodes");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 40;
            item.useTime = 40;
            item.shootSpeed = 15f;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item116;
            item.shoot = ModContent.ProjectileType<ElectromagnetismProjectile>();
            item.value = Item.sellPrice(silver: 5);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.damage = 12;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // How far out the inaccuracy of the shot chain can be.
            float radius = 90f;
            // Sets ai[1] to the following value to determine the firing direction.
            // The smaller the value of NextFloat(), the more accurate the shot will be. The larger, the less accurate. This changes depending on your radius.
            // NextBool().ToDirectionInt() will have a 50% chance to make it negative instead of positive.
            // The Solar Eruption uses this calculation: Main.rand.NextFloat(0f, 0.5f) * Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(45f);
            float direction = Main.rand.NextFloat(0.25f, 1f) * Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(radius);
            Projectile projectile = Projectile.NewProjectileDirect(player.RotatedRelativePoint(player.MountedCenter), new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, direction);
            // Extra logic for the chain to adjust to item stats, unlike the Solar Eruption.
            if (projectile.modProjectile is ElectromagnetismProjectile modItem)
            {
                modItem.firingSpeed = item.shootSpeed * 2f;
                modItem.firingAnimation = item.useAnimation;
                modItem.firingTime = item.useTime;
            }
            return false;
        }
    }
}



namespace AerovelenceMod.Items.Weapons.Melee
{
    public class ElectromagnetismProjectile : ModProjectile
    {
        public Vector2 chainHeadPosition;
        public float firingSpeed;
        public float firingAnimation;
        public float firingTime;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Solar Eruption");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(-90f);
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();

            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = new Vector2(0f, -1f).RotatedByRandom(MathHelper.ToRadians(360f));
                Dust dust1 = Dust.NewDustPerfect(chainHeadPosition, 59, velocity, 150, default, 1.5f);
                dust1.noGravity = true;
            }
            Lighting.AddLight(chainHeadPosition, Color.White.ToVector3() * 0.4f);

            if (projectile.localAI[1] > 0f)
            {
                projectile.localAI[1] -= 1f;
            }

            // The projectile's swerving motion.

            // If this localAI slot is 0, meaning it doesn't have an assigned value, then set it to the projectile's rotation so that we can get the rotation it had on its first tick of being spawned.
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = projectile.rotation;
            }

            // If localAI[0] (the localAI slot we use to store initial rotation)'s X value is greater than 0, then direction is 1. Otherwise, -1.
            float direction = (projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();

            // Use a sine calculation to rotate the Solar Eruption around to form an ovular motion.
            Vector2 rotation = (direction * (projectile.ai[0] / firingAnimation * MathHelper.ToRadians(360f) + MathHelper.ToRadians(-90f))).ToRotationVector2();
            rotation.Y *= (float)Math.Sin(projectile.ai[1]);

            rotation = rotation.RotatedBy(projectile.localAI[0]);

            // Use the ai[0] slot as a timer to increment how long the projectile has been alive.
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < firingTime)
            {
                projectile.velocity += (firingSpeed * rotation).RotatedBy(MathHelper.ToRadians(90f));
            }
            else
            {
                // If past the firingTime variable we set in the item's Shoot() hook, kill it.
                projectile.Kill();
            }

            // Manages the positioning for the chain's handle.
            Vector2 offset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;

            // Flip the offset horizontally if the player is facing left instead of right.
            if (player.direction == -1)
            {
                offset.X = player.bodyFrame.Width - offset.X;
            }
            // Flip the offset vetically if the player is using gravity (such as a Gravity Globe or Gravitation Potion.)
            if (player.gravDir == -1f)
            {
                offset.Y = player.bodyFrame.Height - offset.Y;
            }
            // This line is a custom offset that you can change to move the handle around. Default is 0f, 0f. This projectile uses 4f, -6f.
            offset += new Vector2(4f, -6f) * new Vector2(player.direction, player.gravDir);
            offset -= new Vector2(player.bodyFrame.Width - projectile.width, player.bodyFrame.Height - 42) * 0.5f;
            projectile.Center = player.RotatedRelativePoint(player.position + offset) - projectile.velocity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Spawns an explosion when it hits an NPC.
            int cooldown = 3;
            if (projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<ElectromagnetismExplosion>(), damage, 10f, projectile.owner, 0f, 1f);
                // Expand the hitbox of the explosion when it spawns.
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                projectile.localAI[1] = cooldown;
            }
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
        }

        // Set to true so the projectile can break tiles like grass, pots, vines, etc.
        public override bool? CanCutTiles()
        {
            return true;
        }

        // Plot a line from the start of the Solar Eruption to the end of it, to change the tile-cutting collision logic. (Don't change this.)
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity, (projectile.width + projectile.height) * 0.5f * projectile.scale, DelegateMethods.CutTiles);
        }

        // Plot a line from the start of the Solar Eruption to the end of it, and check if any hitboxes are intersected by it for the entity collision logic. (Don't change this.)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Custom collision so all chains across the flail can cause impact.
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity, (projectile.width + projectile.height) * 0.5f * projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Color color = lightColor;

            // Some rectangle presets for different parts of the chain.
            Rectangle chainHandle = new Rectangle(0, 2, texture.Width, 40);
            Rectangle chainLinkEnd = new Rectangle(0, 68, texture.Width, 18);
            Rectangle chainLink = new Rectangle(0, 46, texture.Width, 18);
            Rectangle chainHead = new Rectangle(0, 90, texture.Width, 48);

            // If the chain isn't moving, stop drawing all of its components.
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            // These fields / pre-draw logic have been taken from the vanilla source code for the Solar Eruption.
            // They setup distances, directions, offsets, and rotations all so the chain faces correctly.
            float chainDistance = projectile.velocity.Length() + 16f;
            bool distanceCheck = chainDistance < 100f;
            Vector2 direction = Vector2.Normalize(projectile.velocity);
            Rectangle rectangle = chainHandle;
            Vector2 yOffset = new Vector2(0f, Main.player[projectile.owner].gfxOffY);
            float rotation = direction.ToRotation() + MathHelper.ToRadians(-90f);
            // Draw the chain handle. This is the first piece in the sprite.
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + yOffset, rectangle, color, rotation, rectangle.Size() / 2f - Vector2.UnitY * 4f, projectile.scale, SpriteEffects.None, 0f);
            chainDistance -= 40f * projectile.scale;
            Vector2 position = projectile.Center;
            position += direction * projectile.scale * 24f;
            rectangle = chainLinkEnd;
            if (chainDistance > 0f)
            {
                float chains = 0f;
                while (chains + 1f < chainDistance)
                {
                    if (chainDistance - chains < rectangle.Height)
                    {
                        rectangle.Height = (int)(chainDistance - chains);
                    }
                    // Draws the chain links between the handle and the head. This is the "line," or the third piece in the sprite.
                    spriteBatch.Draw(texture, position - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), rotation, new Vector2(rectangle.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    chains += rectangle.Height * projectile.scale;
                    position += direction * rectangle.Height * projectile.scale;
                }
            }
            Vector2 chainEnd = position;
            position = projectile.Center;
            position += direction * projectile.scale * 24f;
            rectangle = chainLink;
            int offset = distanceCheck ? 9 : 18;
            float chainLinkDistance = chainDistance;
            if (chainDistance > 0f)
            {
                float chains = 0f;
                float increment = chainLinkDistance / offset;
                chains += increment * 0.25f;
                position += direction * increment * 0.25f;
                for (int i = 0; i < offset; i++)
                {
                    float spacing = increment;
                    if (i == 0)
                    {
                        spacing *= 0.75f;
                    }
                    // Draws the actual chain link spikes between the handle and the head. These are the "spikes," or the second piece in the sprite.
                    spriteBatch.Draw(texture, position - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), rotation, new Vector2(rectangle.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    chains += spacing;
                    position += direction * spacing;
                }
            }
            rectangle = chainHead;
            // Draw the chain head. This is the fourth piece in the sprite.
            spriteBatch.Draw(texture, chainEnd - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)chainEnd.X / 16, (int)chainEnd.Y / 16), rotation, texture.Frame().Top(), projectile.scale, SpriteEffects.None, 0f);
            // Because the chain head's draw position isn't determined in AI, it is set in PreDraw.
            // This is so the smoke-spawning dust and white light are at the proper location.
            chainHeadPosition = chainEnd;

            return false;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class ElectromagnetismExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The English name of this projectile.
            DisplayName.SetDefault("Example Solar Eruption Explosion");
            // How many vertical animation frames its spritesheet has.
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            // Initial hitbox size. When we manually spawn it in OnHitNPC of ExampleSolarEruptionProjectile, we increase this as necessary.
            projectile.width = 52;
            projectile.height = 52;
            // The projectile is spawned by a player.
            projectile.friendly = true;
            // The projectile starts at alpha 255, meaning its invisible. 0 alpha is visible, 255 is invisible.
            projectile.alpha = 255;
            // The projectile doesn't move anyways, but ignore water physics as necessary.
            projectile.ignoreWater = true;
            // Spawn and exist for 60 ticks, or 1 full second.
            projectile.timeLeft = 60;
            // Don't despawn upon tile collision
            projectile.tileCollide = false;
            // Never despawn upon hurting an enemy
            projectile.penetrate = -1;
            // Important for changing cooldowns of hit enemies.
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            // Add light in place of the explosion. Color.White can be any color you want, and 1f at the end is a radius multiplier.
            //Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 1f);

            // Make the explosion larger overtime. By the time of death (60 ticks) it grows by 0.6f.
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1];

            // When it spawns, play an explosion sound.
            if (projectile.ai[0] == 0)
            {
                Main.PlaySound(SoundID.Item14, projectile.Center);
            }

            projectile.ai[0]++;
            // The larger amount of frames the explosion has, the longer it takes to die (still up to 60 ticks.)
            if (projectile.ai[0] >= 3 * Main.projFrames[projectile.type])
            {
                projectile.Kill();
                return;
            }

            // Animates the explosion.
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    // If greater than the max frame count, hide the projectile so it creates the illusion of disappearing like an explosion.
                    projectile.hide = true;
                }
            }

            // Fades in the explosion.
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            projectile.Damage();

            // Basic explosion dust
            int dusts = 5;
            for (int i = 0; i < dusts; i++)
            {
                // If a random number including 0, 1, 2, and 3, lands 0, (basically 25% chance), spawn the dust.
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    // This velocity takes the speed, multiplies it by a random number from 0.5, to 1.2, then rotates it to be evenly spread like a circle based on what dusts is set to, and then randomly offsets it.
                    Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(projectile.Center, 59, velocity, 150, default, 1.5f);
                    dust1.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // The Solar Eruption's explosions make use of NPC hit-cooldown timers.
            int cooldown = 4;
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Redraw the projectile with its origin on the center of the hitbox, to compensate for hitbox inflation for accurate explosions.
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color color = projectile.GetAlpha(lightColor);

            if (!projectile.hide)
            {
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, rectangle, color, projectile.rotation, rectangle.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}