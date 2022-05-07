using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;

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
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.shootSpeed = 15f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<ElectromagnetismProjectile>();
            Item.value = Item.sellPrice(gold: 15);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 12;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Electromagnetism", "Artifact")
            {
                OverrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        float radius = 90f;
            float direction = Main.rand.NextFloat(0.25f, 1f) * Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(radius);
            Projectile projectile = Projectile.NewProjectileDirect(source, player.RotatedRelativePoint(player.MountedCenter), new Vector2(velocity.X, velocity.Y), type, damage, 2f, player.whoAmI, 0f, direction);
            if (projectile.ModProjectile is ElectromagnetismProjectile modItem)
            {
                modItem.firingSpeed = Item.shootSpeed * 2f;
                modItem.firingAnimation = Item.useAnimation;
                modItem.firingTime = Item.useTime;
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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-90f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = new Vector2(0f, -1f).RotatedByRandom(MathHelper.ToRadians(360f));
                Dust dust1 = Dust.NewDustPerfect(chainHeadPosition, 59, velocity, 150, default, 1.5f);
                dust1.noGravity = true;
            }
            Lighting.AddLight(chainHeadPosition, Color.White.ToVector3() * 0.4f);

            if (Projectile.localAI[1] > 0f)
            {
                Projectile.localAI[1] -= 1f;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Projectile.rotation;
            }
            float direction = (Projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();
            Vector2 rotation = (direction * (Projectile.ai[0] / firingAnimation * MathHelper.ToRadians(360f) + MathHelper.ToRadians(-90f))).ToRotationVector2();
            rotation.Y *= (float)Math.Sin(Projectile.ai[1]);

            rotation = rotation.RotatedBy(Projectile.localAI[0]);

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < firingTime)
            {
                Projectile.velocity += (firingSpeed * rotation).RotatedBy(MathHelper.ToRadians(90f));
            }
            else
            {
                Projectile.Kill();
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
            offset -= new Vector2(player.bodyFrame.Width - Projectile.width, player.bodyFrame.Height - 42) * 0.5f;
            Projectile.Center = player.RotatedRelativePoint(player.position + offset) - Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int cooldown = 3;
            if (Projectile.localAI[1] <= 0f)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<ElectromagnetismExplosion>(), damage, 10f, Projectile.owner, 0f, 1f);
                Vector2 radius = new Vector2(90f, 90f);
                explosion.Hitbox = new Rectangle(explosion.getRect().X - (int)(radius.X / 2), explosion.getRect().Y - (int)(radius.Y / 2), explosion.getRect().Width + (int)radius.X, explosion.getRect().Height + (int)radius.Y);
                Projectile.localAI[1] = cooldown;
            }
            Projectile.localNPCImmunity[target.whoAmI] = 6;
            target.immune[Projectile.owner] = cooldown;
        }

        public override bool? CanCutTiles()
        {
            return true;
        }


        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity, (Projectile.width + Projectile.height) * 0.5f * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, (Projectile.width + Projectile.height) * 0.5f * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            {
                Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
                Color color = lightColor;

                Rectangle chainHandle = new Rectangle(0, 2, texture.Width, 40);
                Rectangle chainLinkEnd = new Rectangle(0, 68, texture.Width, 18);
                Rectangle chainLink = new Rectangle(0, 46, texture.Width, 18);
                Rectangle chainHead = new Rectangle(0, 90, texture.Width, 48);

                if (Projectile.velocity == Vector2.Zero)
                {
                    return false;
                }

                float chainDistance = Projectile.velocity.Length() + 16f;
                bool distanceCheck = chainDistance < 100f;
                Vector2 direction = Vector2.Normalize(Projectile.velocity);
                Rectangle rectangle = chainHandle;
                Vector2 yOffset = new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
                float rotation = direction.ToRotation() + MathHelper.ToRadians(-90f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + yOffset, rectangle, color, rotation, rectangle.Size() / 2f - Vector2.UnitY * 4f, Projectile.scale, SpriteEffects.None, 0);
                chainDistance -= 40f * Projectile.scale;
                Vector2 position = Projectile.Center;
                position += direction * Projectile.scale * 24f;
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
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), rotation, new Vector2(rectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                        chains += rectangle.Height * Projectile.scale;
                        position += direction * rectangle.Height * Projectile.scale;
                    }
                }
                Vector2 chainEnd = position;
                position = Projectile.Center;
                position += direction * Projectile.scale * 24f;
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
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), rotation, new Vector2(rectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                        chains += spacing;
                        position += direction * spacing;
                    }
                }
                rectangle = chainHead;
                Main.spriteBatch.Draw(texture, chainEnd - Main.screenPosition + yOffset, rectangle, Lighting.GetColor((int)chainEnd.X / 16, (int)chainEnd.Y / 16), rotation, texture.Frame().Top(), Projectile.scale, SpriteEffects.None, 0);
                chainHeadPosition = chainEnd;

                return false;
            }
        }

        public class ElectromagnetismExplosion : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                DisplayName.SetDefault("Electromagnetic Explosion");
                Main.projFrames[Projectile.type] = 5;
            }

            public override void SetDefaults()
            {
                Projectile.width = 52;
                Projectile.height = 52;
                Projectile.friendly = true;
                Projectile.alpha = 255;
                Projectile.ignoreWater = true;
                Projectile.timeLeft = 60;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
            }

            public override void AI()
            {
                Player player = Main.player[Projectile.owner];
                Projectile.ai[1] += 0.01f;
                Projectile.scale = Projectile.ai[1];
                if (Projectile.ai[0] == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                }

                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 3 * Main.projFrames[Projectile.type])
                {
                    Projectile.Kill();
                    return;
                }

                if (++Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    {
                        Projectile.hide = true;
                    }
                }
                Projectile.alpha -= 63;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }

                Projectile.Damage();

                int dusts = 5;
                for (int i = 0; i < dusts; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        float speed = 6f;
                        Vector2 velocity = new Vector2(0f, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                        Dust dust1 = Dust.NewDustPerfect(Projectile.Center, 59, velocity, 150, default, 1.5f);
                        dust1.noGravity = true;
                    }
                }
            }

            public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
            {
                int cooldown = 4;
                Projectile.localNPCImmunity[target.whoAmI] = 6;
                target.immune[Projectile.owner] = cooldown;
            }

            public override bool PreDraw(ref Color lightColor)
            {
                Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
                Rectangle rectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Color color = Projectile.GetAlpha(lightColor);

                if (!Projectile.hide)
                {
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rectangle, color, Projectile.rotation, rectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
                return false;
            }
        }
    }
}