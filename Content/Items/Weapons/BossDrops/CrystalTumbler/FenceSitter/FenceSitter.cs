using AerovelenceMod.Common.Systems;
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
    public class FenceSitter : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/FenceSitter/FenceSitter";
        private const string Description = "Land four consecutive swings to unleash lighting beams on the fifth";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Fence Sitter", Description).AddSkillStrike(Language.Default, "The fifth swing's beams Skill Strike");
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
            Item.damage = 22;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.knockBack = 5;
            Item.shootSpeed = 1;
            Item.shoot = ModContent.ProjectileType<FenceSitterSwing>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D worldSprite = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            spriteBatch.Draw(worldSprite, Item.Center - Main.screenPosition, null, Color.White, rotation, worldSprite.Size() * .5f, scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            FenceSitterPlayer state = player.GetModPlayer<FenceSitterPlayer>();
            bool special = state.Hits >= 4;
            if (special) state.Hits = 0;
            Projectile.NewProjectile(source, player.MountedCenter, velocity.SafeNormalize(Vector2.UnitX * player.direction), type, damage, knockback, player.whoAmI, special ? 1 : 0, velocity.X >= 0f ? 1 : -1);
            return false;
        }
            public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.gameMenu)
                return;
            int charges = Main.LocalPlayer.GetModPlayer<FenceSitterPlayer>().Hits;
            if (charges <= 0)
                return;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            Rectangle lit = new(0, 0, glow.Width, FenceSitterVFX.ChargeHeight(glow, charges));
            spriteBatch.Draw(glow, position, lit, FenceSitterVFX.Glow(Color.White, 0.7f), 0f, origin, scale, SpriteEffects.None, 0f);
        }

    }

    public class FenceSitterPlayer : ModPlayer
    {
        public int Hits;
        public int Swing;
        public override void PostUpdate()
        {
            if (Player.dead || Player.HeldItem.type != ModContent.ItemType<FenceSitter>()) Hits = 0;
            if (Hits == 4 && Main.rand.NextBool(4)) FenceSitterVFX.SpawnSpark(Player.MountedCenter + new Vector2(Player.direction * 16, -8), new Vector2(0, -1), Color.White, .14f);
        }
    }

    public class FenceSitterSwing : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/FenceSitter/FenceSitterHeld";
        private float progress;
        private int pause;
        private int fieldTimer;
        private int displayedCharges;
        private bool hit;
        private bool fieldStarted;
        private Vector2 tip;
        private Vector2 previousTip;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(progress);
            writer.Write(pause);
            writer.Write(fieldTimer);
            writer.Write(displayedCharges);
            writer.Write(hit);
            writer.Write(fieldStarted);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            progress = reader.ReadSingle();
            pause = reader.ReadInt32();
            fieldTimer = reader.ReadInt32();
            displayedCharges = reader.ReadInt32();
            hit = reader.ReadBoolean();
            fieldStarted = reader.ReadBoolean();
        }
        private float Ease(float t) => t <= 0 ? 0 : t >= 1 ? 1 : t < .5f ? MathF.Pow(2, 16 * t - 8) / 2 : (2 - MathF.Pow(2, -16 * t + 8)) / 2;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 240;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active || player.HeldItem.type != ModContent.ItemType<FenceSitter>()) { Projectile.Kill(); return; }
            Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                int charges = player.GetModPlayer<FenceSitterPlayer>().Hits;
                if (displayedCharges != charges)
                {
                    displayedCharges = charges;
                    Projectile.netUpdate = true;
                }
            }
            float speed = player.GetTotalAttackSpeed(DamageClass.Melee) / 64f;
            if (Projectile.ai[0] == 1)
            {
                if (!fieldStarted && Projectile.owner == Main.myPlayer)
                {
                    Vector2 aim = GetSpecialAim(player);
                    if (Vector2.Dot(Projectile.velocity.SafeNormalize(aim), aim) < .999f) Projectile.netUpdate = true;
                    Projectile.velocity = aim;
                }

                int facing = Projectile.velocity.X < 0f ? -1 : 1;
                player.direction = facing;
                float finalRotation = Projectile.velocity.ToRotation() - facing * MathHelper.PiOver2;
                Vector2 finalCenter = player.MountedCenter + new Vector2(facing * 3f, -3f);

                if (!fieldStarted)
                {
                    progress = Math.Min(1f, progress + speed * 1.35f);
                    float pose = MathHelper.SmoothStep(0f, 1f, progress);
                    float startRotation = finalRotation + facing * .92f;
                    Projectile.rotation = startRotation + MathHelper.WrapAngle(finalRotation - startRotation) * pose;
                    Projectile.Center = Vector2.Lerp(player.MountedCenter + new Vector2(-facing * 11f, 19f), finalCenter, pose);

                    if (progress >= 1f)
                    {
                        Projectile.rotation = finalRotation;
                        Projectile.Center = finalCenter;
                        fieldStarted = true;
                        fieldTimer = 76;
                        SpawnElectricField(player);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.rotation = finalRotation;
                    Projectile.Center = finalCenter;
                    if (fieldTimer > 0) fieldTimer--;
                    else { Projectile.Kill(); return; }
                }
            }
            else
            {
                if (progress == 0) SoundEngine.PlaySound(SoundID.Item1 with { Volume = .7f }, player.Center);
                if (pause > 0) pause--;
                else progress = Math.Min(1f, progress + speed);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-2.35f, 2.35f, Ease(progress)) * Projectile.ai[1];
                Projectile.Center = player.MountedCenter;
                player.direction = Projectile.velocity.X >= 0 ? 1 : -1;
            }
            previousTip = tip == Vector2.Zero ? Projectile.Center : tip;
            tip = Projectile.Center + Projectile.rotation.ToRotationVector2() * 68;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            if (Projectile.ai[0] == 0 && progress >= 1) Projectile.Kill();
        }
        private Vector2 GetSpecialAim(Player player)
        {
            Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
            int facing = toMouse.X < -12f ? -1 : toMouse.X > 12f ? 1 : Projectile.ai[1] < 0f ? -1 : 1;
            float baseAngle = facing > 0 ? 0f : MathHelper.Pi;
            float relative = MathHelper.WrapAngle(toMouse.SafeNormalize(Vector2.UnitX * facing).ToRotation() - baseAngle);
            relative = MathHelper.Clamp(relative, -.7f, .7f);
            Projectile.ai[1] = facing;
            return (baseAngle + relative).ToRotationVector2();
        }
        private void SpawnElectricField(Player player)
        {
            Vector2 axis = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            if (Projectile.owner == Main.myPlayer)
                for (int i = 0; i < 4; i++)
                {
                    Vector2 fenceStart = CrystalWorldPosition(i);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), fenceStart, axis, ModContent.ProjectileType<FenceSitterBeam>(), (int)(Projectile.damage * .65f), 2f, Projectile.owner);
                    for (int k = 0; k < 4; k++) FenceSitterVFX.SpawnSpark(fenceStart, axis * Main.rand.NextFloat(.5f, 2.2f) + Main.rand.NextVector2Circular(1.2f, 1.2f), FenceSitterVFX.PhaseColor(0), .14f);
                }
            SoundEngine.PlaySound(SoundID.Item93 with { Volume = .5f, Pitch = .18f }, player.Center);
        }
        private Vector2 CrystalWorldPosition(int index)
        {
            Vector2 pixel = index switch
            {
                0 => new Vector2(31f, 6f),
                1 => new Vector2(33f, 19f),
                2 => new Vector2(31f, 31f),
                _ => new Vector2(29f, 45f)
            };
            const float width = 34f;
            const float height = 74f;
            Vector2 grip = new(width * .42f, height - 7f);
            float scale = 68f / (grip.Y - 4f);
            Vector2 local = new((pixel.X - grip.X) * (Projectile.velocity.X < 0f ? -1f : 1f), pixel.Y - grip.Y);
            return Projectile.Center + (local * scale).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
        }
        public override bool? CanDamage() => Projectile.ai[0] == 0 && !hit && Ease(progress) > .12f && Ease(progress) < .88f ? null : false;
        public override bool? CanHitNPC(NPC target) => hit || Projectile.ai[0] == 1 ? false : null;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, tip, 16, ref point) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), previousTip, tip, 16, ref point);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo info, int damageDone)
        {
            if (hit) return;
            hit = true;
            pause = 10;
            Projectile.netUpdate = true;
            if (Projectile.owner == Main.myPlayer) Main.player[Projectile.owner].GetModPlayer<FenceSitterPlayer>().Hits++;
            SoundEngine.PlaySound(SoundID.Item70 with { Volume = .4f }, target.Center);
            for (int i = 0; i < 12; i++) FenceSitterVFX.SpawnSpark(tip, Main.rand.NextVector2Circular(4, 4), FenceSitterVFX.PhaseColor(0));
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] == 0 && !hit) Main.player[Projectile.owner].GetModPlayer<FenceSitterPlayer>().Hits = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 start = Projectile.Center - Main.screenPosition;
            Texture2D sword = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            Vector2 grip = new(sword.Width * 0.42f, sword.Height - 7f);
            SpriteEffects flip = Projectile.velocity.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (flip != SpriteEffects.None) grip.X = sword.Width - grip.X;
            float scale = 68f / (grip.Y - 4f);
            float rotation = Projectile.rotation + MathHelper.PiOver2;
            float drawOpacity = Projectile.ai[0] == 1 && !fieldStarted ? MathHelper.SmoothStep(.25f, 1f, MathHelper.Clamp(progress * 3f, 0f, 1f)) : 1f;
            Main.EntitySpriteDraw(sword, start, null, lightColor * drawOpacity, rotation, grip, scale, flip);
            int charges = displayedCharges;
            if (Projectile.ai[0] == 1) charges = !fieldStarted ? 4 : fieldTimer > 0 ? Math.Clamp((int)Math.Ceiling(fieldTimer / 19f), 1, 4) : 0;
            int height = FenceSitterVFX.ChargeHeight(glow, charges);
            if (charges > 0)
            {
                Rectangle lit = new(0, 0, glow.Width, height);
                float pulse = 0.7f + 0.2f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
                Main.EntitySpriteDraw(glow, start, lit, FenceSitterVFX.Glow(FenceSitterVFX.PhaseColor(0), pulse * drawOpacity), rotation, grip, scale, flip);
                Main.EntitySpriteDraw(glow, start, lit, FenceSitterVFX.Glow(Color.White, 0.35f * drawOpacity), rotation, grip, scale, flip);
            }
            return false;
        }
    }

    public class FenceSitterBeam : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        private float length;
        private readonly byte[] hits = new byte[Main.maxNPCs];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 38;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            if (length != 0) return;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float[] samples = new float[3];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, 4, 540, samples);
            length = Math.Max(1, Math.Min(samples[0], Math.Min(samples[1], samples[2])));
        }
        public override bool? CanDamage() => Projectile.timeLeft > 8 ? null : false;
        public override bool? CanHitNPC(NPC target) => hits[target.whoAmI] < 2 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * length, 14, ref point);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => hits[target.whoAmI]++;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 end = Projectile.Center + Projectile.velocity * length;
            float fade = Math.Min(1, Projectile.timeLeft / 12f);
            FenceSitterVFX.DrawElectricLine(Main.spriteBatch, Projectile.Center - Main.screenPosition, end - Main.screenPosition, FenceSitterVFX.PhaseColor(0), fade, 24, Projectile.identity * 13, 2.2f);
            FenceSitterVFX.DrawCorona(Main.spriteBatch, Projectile.Center - Main.screenPosition, 10, FenceSitterVFX.PhaseColor(0), fade * .5f, Projectile.identity);
            FenceSitterVFX.DrawCorona(Main.spriteBatch, end - Main.screenPosition, 10, FenceSitterVFX.PhaseColor(0), fade * .5f, Projectile.identity);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++) FenceSitterVFX.SpawnSpark(Projectile.Center + Projectile.velocity * length * Main.rand.NextFloat(), Main.rand.NextVector2Circular(2, 2), FenceSitterVFX.PhaseColor(0), .18f);
        }
    }

    internal static class FenceSitterVFX
    {
        internal static int ChargeHeight(Texture2D texture, int charges) => Math.Clamp((int)MathF.Ceiling(texture.Height * Math.Clamp(charges, 0, 4) / 4f), 0, texture.Height);
        internal static Color Glow(Color color, float opacity = 1f)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);
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

        public static void DrawElectricLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float opacity, int segments = 18, float seed = 0f, float width = 2f)
        {
            Vector2 fullDelta = end - start;
            if (fullDelta.LengthSquared() < 0.01f || opacity <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.83f;
            Vector2 normal = fullDelta.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            segments = Math.Clamp(segments, 2, 64);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = start + Main.screenPosition;
            for (int i = 1; i <= segments; i++)
            {
                float progress = i / (float)segments;
                Vector2 current = Vector2.Lerp(start, end, progress);
                if (i < segments)
                {
                    float envelope = MathF.Sin(progress * MathHelper.Pi);
                    float wave = MathF.Sin(seed * 3.17f + i * 8.31f + time) + MathF.Sin(seed + i * 3.73f - time * 1.7f) * 0.45f;
                    current += normal * wave * 5f * envelope;
                }
                points[i] = current + Main.screenPosition;
                if (width > 1.5f && i > 2 && i < segments - 1 && i % 5 == 0)
                {
                    float branchSide = MathF.Sin(seed * 2.31f + i * 4.7f) >= 0f ? 1f : -1f;
                    Vector2 branch = (points[i] - points[i - 1]).SafeNormalize(Vector2.UnitX).RotatedBy(branchSide * 0.78f) * (12f + 5f * MathF.Abs(MathF.Sin(seed + i)));
                    DrawPath(new[] { points[i], points[i] + branch * 0.5f + normal * 2f, points[i] + branch }, color, opacity * 0.55f, Math.Max(1f, width * 0.55f));
                }
            }
            DrawPath(points, color, opacity, width);
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