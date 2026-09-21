using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.CrystalTumbler
{
    public class TumblerAccelerator : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/TumblerAccelerator/TumblerAccelerator";
        private const string Description = "Catapults tiny tumblers\nRight click to launch tumblers toward your cursor";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Tumbler Accelerator", Description).AddSkillStrike(Language.Default, "Hitting the same enemy with two different tumblers quickly Skill Strikes");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.knockBack = 4;
            Item.shootSpeed = 9;
            Item.shoot = ModContent.ProjectileType<AcceleratorTumbler>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] < 12;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            int magnetized = 0;
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type && p.ai[0] == 0)
                    {
                        p.ai[0] = 1;
                        p.ai[1] = Main.MouseWorld.X;
                        p.ai[2] = Main.MouseWorld.Y;
                        p.netUpdate = true;
                        magnetized++;
                    }
                SoundEngine.PlaySound(SoundID.Item15 with { Volume = .45f }, player.Center);
            }
            else
            {
                Projectile.NewProjectile(source, player.MountedCenter, aim * 7 + new Vector2(0, -3), type, damage, knockback, player.whoAmI);
                SoundEngine.PlaySound(SoundID.Item61 with { Volume = .6f, Pitch = -.3f }, player.Center);
            }
            Projectile.NewProjectile(source, player.MountedCenter, aim, ModContent.ProjectileType<AcceleratorHeld>(), 0, 0, player.whoAmI, player.altFunctionUse == 2 ? 1 : 0, magnetized);
            return false;
        }
    }

    public class AcceleratorPairTracker : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private readonly Dictionary<int, (ulong tick, int identity)> impacts = new();
        public bool IsPair(Projectile projectile) => impacts.TryGetValue(projectile.owner, out var last) && last.identity != projectile.identity && Main.GameUpdateCount - last.tick <= 20;
        public void Record(Projectile projectile)
        {
            if (IsPair(projectile)) impacts.Remove(projectile.owner);
            else impacts[projectile.owner] = (Main.GameUpdateCount, projectile.identity);
        }
    }

    public class AcceleratorTumbler : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/CrystalCaverns/TumblerockMedium";
        private int charge;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity *= .88f;
                Projectile.rotation += .2f + charge * .025f;
                charge++;
                if (Projectile.owner == Main.myPlayer && charge % 6 == 0)
                {
                    Projectile.ai[1] = Main.MouseWorld.X;
                    Projectile.ai[2] = Main.MouseWorld.Y;
                    Projectile.netUpdate = true;
                }
                if (charge >= 32)
                {
                    Projectile.ai[0] = 2;
                    Projectile.velocity = (new Vector2(Projectile.ai[1], Projectile.ai[2]) - Projectile.Center).SafeNormalize(Vector2.UnitX) * 12;
                    Projectile.extraUpdates = 1;
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 150);
                    Projectile.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = .35f, Pitch = .4f }, Projectile.Center);
                    for (int i = 0; i < 12; i++) TumblerAcceleratorVFX.SpawnSpark(Projectile.Center, Main.rand.NextVector2Circular(4, 4), Color.Gold);
                }
            }
            else
            {
                Projectile.extraUpdates = Projectile.ai[0] == 2 ? 1 : 0;
                Projectile.velocity.Y += Projectile.ai[0] == 2 ? .04f : .23f;
                Projectile.rotation += Projectile.velocity.X / 11;
                if (Projectile.ai[0] == 3) Projectile.velocity.X *= .985f;
            }
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
            {
                Lighting.AddLight(Projectile.Center, .5f, .3f, .05f);
                if (Main.rand.NextBool(3)) TumblerAcceleratorVFX.SpawnSpark(Projectile.Center, -Projectile.velocity * .1f, Color.Gold, .16f);
            }
        }
        public override bool? CanDamage() => Projectile.ai[0] == 1 || Projectile.timeLeft < 20 ? false : null;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 2) return true;
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X * .65f;
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = oldVelocity.Y > 2 ? -oldVelocity.Y * .45f : 0;
                if (oldVelocity.Y > 0) { Projectile.ai[0] = 3; Projectile.timeLeft = Math.Min(Projectile.timeLeft, 90); }
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (target.GetGlobalNPC<AcceleratorPairTracker>().IsPair(Projectile)) SkillStrikeUtil.setSkillStrike(Projectile, 1.65f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            AcceleratorPairTracker tracker = target.GetGlobalNPC<AcceleratorPairTracker>();
            if (tracker.IsPair(Projectile)) target.AddBuff(BuffID.Electrified, 180);
            tracker.Record(Projectile);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/CrystalCaverns/TumblerockMedium").Value;
            Rectangle frame = texture.Frame(1, 2);
            float fade = Math.Min(1, Projectile.timeLeft / 20f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor * fade, Projectile.rotation, frame.Size() / 2, 24f / frame.Width, SpriteEffects.None);
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
                TumblerAcceleratorVFX.DrawCharge(Main.spriteBatch, Projectile.Center - Main.screenPosition, Color.Gold, Math.Min(1, charge / 32f), 17, Projectile.rotation, fade * .85f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++) TumblerAcceleratorVFX.SpawnSpark(Projectile.Center, Main.rand.NextVector2Circular(4, 4), Projectile.ai[0] == 2 ? Color.Gold : TumblerAcceleratorVFX.PhaseColor(0));
            if (!Main.dedServ) for (int i = 0; i < 7; i++) Dust.NewDust(Projectile.position, 22, 22, DustID.Stone, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 1));
        }
    }

    public class AcceleratorHeld : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/TumblerAccelerator/TumblerAccelerator";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 24;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || player.HeldItem.type != ModContent.ItemType<TumblerAccelerator>()) { Projectile.Kill(); return; }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.MountedCenter + Projectile.velocity * (16 - Projectile.timeLeft / 3f);
            player.direction = Projectile.velocity.X >= 0 ? 1 : -1;
            player.heldProj = Projectile.whoAmI;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            if (Projectile.ai[0] == 1 && Projectile.ai[1] > 0f)
            {
                float strength = MathHelper.Clamp(0.45f + Projectile.ai[1] * 0.08f, 0.45f, 1f);
                Lighting.AddLight(Projectile.Center, new Vector3(1f, .58f, .12f) * strength * .55f);
                if (Main.rand.NextBool(3)) TumblerAcceleratorVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f), Main.rand.NextVector2Circular(1.8f, 1.8f), Color.Gold, .13f + strength * .05f);
            }
            if (player.itemTime <= 1) Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 axis = Projectile.rotation.ToRotationVector2();
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            Texture2D magnetGlow = ModContent.Request<Texture2D>(Texture + "_Glowmask1").Value;
            SpriteEffects flip = axis.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 origin = texture.Size() * .5f;
            float age = 24f - Projectile.timeLeft;
            float shotFlash = MathF.Exp(-age * .28f);
            Main.EntitySpriteDraw(texture, center, null, lightColor, Projectile.rotation, origin, 1f, flip);
            Main.EntitySpriteDraw(glow, center, null, Color.White, Projectile.rotation, glow.Size() * .5f, 1f, flip);
            Main.EntitySpriteDraw(glow, center, null, TumblerAcceleratorVFX.Glow(Color.White, shotFlash * .9f), Projectile.rotation, glow.Size() * .5f, 1f + shotFlash * .025f, flip);
            if (Projectile.ai[0] == 1 && Projectile.ai[1] > 0f)
            {
                float magnetStrength = MathHelper.Clamp(.5f + Projectile.ai[1] * .08f, .5f, 1f);
                float pulse = .72f + .28f * MathF.Sin(Main.GlobalTimeWrappedHourly * 11f + Projectile.identity);
                Color magnetColor = Color.Lerp(new Color(255, 155, 35), Color.White, .2f + pulse * .18f);
                Main.EntitySpriteDraw(magnetGlow, center, null, TumblerAcceleratorVFX.Glow(magnetColor, magnetStrength * (.62f + pulse * .22f)), Projectile.rotation, magnetGlow.Size() * .5f, 1f, flip);
                Main.EntitySpriteDraw(magnetGlow, center, null, TumblerAcceleratorVFX.Glow(Color.White, magnetStrength * pulse * .42f), Projectile.rotation, magnetGlow.Size() * .5f, 1.025f + pulse * .018f, flip);
                TumblerAcceleratorVFX.DrawCorona(Main.spriteBatch, center, 12f + pulse * 3f, Color.Gold, magnetStrength * .32f, Projectile.identity, 1.6f);
            }
            TumblerAcceleratorVFX.DrawCorona(Main.spriteBatch, center + axis * 22f, 8f, TumblerAcceleratorVFX.PhaseColor(0), Projectile.timeLeft / 40f, Projectile.identity);
            return false;
        }
    }

    internal static class TumblerAcceleratorVFX
    {
        public static void SpawnSpark(Vector2 position, Vector2 velocity, Color color, float scale = 0.2f)
        {
            if (Main.dedServ)
                return;
            Dust spark = Dust.NewDustPerfect(position, ModContent.DustType<ElectricSparkGlow>(), velocity, 0, color, scale);
            spark.customData = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.98f, FadeVelPower: 0.92f, TimeBetweenFrames: 3, KillEarlyTime: 24, Pixelize: true, UnderGlowPower: 1.4f, WhiteLayerPower: 0.8f);
        }

        public static Color PhaseColor(float phase)
        {
            return phase >= 1f ? new Color(255, 177, 45) : new Color(45, 226, 255);
        }

        public static Color Glow(Color color, float opacity = 1f)
        {
            color *= MathHelper.Clamp(opacity, 0f, 1f);
            color.A = 0;
            return color;
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            Vector2 delta = end - start;
            if (delta.LengthSquared() < 0.01f)
                return;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, delta.ToRotation(), new Vector2(0f, 0.5f), new Vector2(delta.Length(), Math.Max(0.5f, width)), SpriteEffects.None, 0f);
        }

        public static void DrawCorona(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, float opacity, float seed = 0f, float width = 1.5f)
        {
            if (opacity <= 0f || radius <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.65f;
            int segments = Math.Clamp((int)(radius * 0.8f), 12, 48);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = center + Main.screenPosition + new Vector2(radius, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * MathHelper.TwoPi;
                float jitter = MathF.Sin(i * 8.17f + seed + time) * 2.4f;
                Vector2 current = center + angle.ToRotationVector2() * (radius + (i == segments ? 0f : jitter));
                points[i] = current + Main.screenPosition;
            }
            DrawPath(points, color, opacity * 0.8f, width);
        }

        public static void DrawCharge(SpriteBatch spriteBatch, Vector2 center, Color color, float progress, float radius, float rotation = 0f, float opacity = 1f)
        {
            if (radius <= 0f || opacity <= 0f)
                return;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            spriteBatch.Draw(glow, center, null, Glow(color, (0.12f + progress * 0.22f) * opacity), rotation, glow.Size() * 0.5f, radius * 3f / glow.Width, SpriteEffects.None, 0f);
            DrawCorona(spriteBatch, center, radius * (1.5f - progress * 0.5f), color, (0.2f + progress * 0.55f) * opacity, rotation);
            spriteBatch.Draw(star, center, null, Glow(color, (0.6f + progress * 0.4f) * opacity), rotation, star.Size() * 0.5f, radius * 2f / star.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, center, null, Glow(Color.White, (0.55f + progress * 0.45f) * opacity), -rotation, star.Size() * 0.5f, radius / star.Width, SpriteEffects.None, 0f);
            for (int i = 0; i < 3; i++)
            {
                float angle = rotation + i * MathHelper.TwoPi / 3f;
                Vector2 tip = center + angle.ToRotationVector2() * radius * (1.5f - progress * 0.5f);
                spriteBatch.Draw(star, tip, null, Glow(color, (0.45f + progress * 0.4f) * opacity), -angle, star.Size() * 0.5f, Math.Min(0.16f, radius / 75f), SpriteEffects.None, 0f);
            }
        }
            private static ulong dustTick;
        private static int dustBudget;
        private static void DrawPath(Vector2[] points, Color color, float opacity, float width)
        {
            if (Main.dedServ || points.Length < 2 || opacity <= 0f)
                return;
            Vector2[] snapshot = (Vector2[])points.Clone();
            float alpha = MathHelper.Clamp(opacity, 0f, 1f);
            PixellationSystem.QueuePixelationAction(() =>
            {
                float thickness = Math.Max(2f, width) * 0.5f;
                Color core = Color.Lerp(color, Color.White, 0.9f) * alpha;
                Color middle = color * (alpha * 0.55f);
                Color outer = color * (alpha * 0.26f);
                float pulse = 0.85f + 0.15f * MathF.Sin(Main.GameUpdateCount * 0.2f);
                Color bloom = color * (alpha * 0.1f * pulse);
                core.A = middle.A = outer.A = bloom.A = 255;
                for (int i = 1; i < snapshot.Length; i++)
                {
                    Vector2 start = (snapshot[i - 1] - Main.screenPosition) * 0.5f;
                    Vector2 end = (snapshot[i] - Main.screenPosition) * 0.5f;
                    for (int halo = 4; halo >= 1; halo--)
                    {
                        Color tint = bloom * ((5f - halo) / 5f);
                        tint.A = 255;
                        DrawLine(Main.spriteBatch, start, end, tint, thickness + halo * 2f);
                    }
                    DrawLine(Main.spriteBatch, start, end, outer, thickness + 3f);
                    DrawLine(Main.spriteBatch, start, end, middle, thickness + 1.5f);
                    DrawLine(Main.spriteBatch, start, end, core, thickness);
                }
            }, PixellationSystem.RenderType.Additive);
            if (dustTick != Main.GameUpdateCount)
            {
                dustTick = Main.GameUpdateCount;
                dustBudget = 6;
            }
            if (!Main.gamePaused && dustBudget > 0 && Main.rand.NextBool(3))
            {
                int segment = Main.rand.Next(1, snapshot.Length);
                Vector2 point = Vector2.Lerp(snapshot[segment - 1], snapshot[segment], Main.rand.NextFloat());
                SpawnSpark(point, Main.rand.NextVector2Circular(1.5f, 1.5f), Color.Lerp(color, Color.White, 0.5f), 0.18f * alpha);
                dustBudget--;
            }
        }

    }
}