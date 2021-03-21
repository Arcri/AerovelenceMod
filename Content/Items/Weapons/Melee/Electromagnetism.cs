using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
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
            item.value = Item.sellPrice(gold: 15);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.damage = 12;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
            tooltips.Add(line);

            line = new TooltipLine(mod, "Electromagnetism", "Artifact")
            {
                overrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float radius = 90f;
            float direction = Main.rand.NextFloat(0.25f, 1f) * Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(radius);
            Projectile projectile = Projectile.NewProjectileDirect(player.RotatedRelativePoint(player.MountedCenter), new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, direction);
            if (projectile.modProjectile is ElectromagnetismProjectile modItem)
            {
                modItem.firingSpeed = item.shootSpeed * 2f;
                modItem.firingAnimation = item.useAnimation;
                modItem.firingTime = item.useTime;
            }
            return false;
        }
    }


    public class ElectromagnetismProjectile : ModProjectile
    {
        public Vector2 chainHeadPosition;
        public float firingSpeed;
        public float firingAnimation;
        public float firingTime;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electromagnetism");
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
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = projectile.rotation;
            }
            float direction = (projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();
            Vector2 rotation = (direction * (projectile.ai[0] / firingAnimation * MathHelper.ToRadians(360f) + MathHelper.ToRadians(-90f))).ToRotationVector2();
            rotation.Y *= (float)Math.Sin(projectile.ai[1]);

            rotation = rotation.RotatedBy(projectile.localAI[0]);

            projectile.ai[0] += 1f;
            if (projectile.ai[0] < firingTime)
            {
                projectile.velocity += (firingSpeed * rotation).RotatedBy(MathHelper.ToRadians(90f));
            }
            else
            {
                projectile.Kill();
            }
            Vector2 offset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction == -1)
            {
                offset.X = player.bodyFrame.Width - offset.X;
            }
            if (player.gravDir == -1f)
            {
                offset.Y = player.bodyFrame.Height - offset.Y;
            }
            offset += new Vector2(4f, -6f) * new Vector2(player.direction, player.gravDir);
            offset -= new Vector2(player.bodyFrame.Width - projectile.width, player.bodyFrame.Height - 42) * 0.5f;
            projectile.Center = player.RotatedRelativePoint(player.position + offset) - projectile.velocity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 3;
            if (projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<ElectromagnetismExplosion>(), damage, 10f, projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                projectile.localAI[1] = cooldown;
            }
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
        }

        public override bool? CanCutTiles()
        {
            return true;
        }


        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity, (projectile.width + projectile.height) * 0.5f * projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
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

            Rectangle chainHandle = new Rectangle(0, 2, texture.Width, 40);
            Rectangle chainLinkEnd = new Rectangle(0, 68, texture.Width, 18);
            Rectangle chainLink = new Rectangle(0, 46, texture.Width, 18);
            Rectangle chainHead = new Rectangle(0, 90, texture.Width, 48);

            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            float chainDistance = projectile.velocity.Length() + 16f;
            bool distanceCheck = chainDistance < 100f;
            Vector2 direction = Vector2.Normalize(projectile.velocity);
            Rectangle rectangle = chainHandle;
            Vector2 yOffset = new Vector2(0f, Main.player[projectile.owner].gfxOffY);
            float rotation = direction.ToRotation() + MathHelper.ToRadians(-90f);
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
                    spriteBatch.Draw(texture, position - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), rotation, new Vector2(rectangle.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    chains += spacing;
                    position += direction * spacing;
                }
            }
            rectangle = chainHead;
            spriteBatch.Draw(texture, chainEnd - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)chainEnd.X / 16, (int)chainEnd.Y / 16), rotation, texture.Frame().Top(), projectile.scale, SpriteEffects.None, 0f);
            chainHeadPosition = chainEnd;

            return false;
        }
    }

    public class ElectromagnetismExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electromagnetic Explosion");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1];
            if (projectile.ai[0] == 0)
            {
                Main.PlaySound(SoundID.Item14, projectile.Center);
            }

            projectile.ai[0]++;
            if (projectile.ai[0] >= 3 * Main.projFrames[projectile.type])
            {
                projectile.Kill();
                return;
            }

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.hide = true;
                }
            }
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            projectile.Damage();

            int dusts = 5;
            for (int i = 0; i < dusts; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(projectile.Center, 59, velocity, 150, default, 1.5f);
                    dust1.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 4;
            projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[projectile.owner] = cooldown;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
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