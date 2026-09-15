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
    public class ConductorWand : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/ConductorWand/ConductorWand";
        private const string Description = "Channel a tethered electric bubble that gathers your minions\nThree captured minions and an additional 50 mana unleash a single plasma shock\nDrains 2 mana plus 2 per captured summon every half second\nRelease to free your summons; start a new bubble to shock again\nAlternatively carries one sentry; falling sentries release a landing shock";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Conductor Wand", Description).AddSkillStrike(Language.Default, "Charge the plasma globe with three minions");
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
            Item.damage = 30;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 6;
            Item.channel = true;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 3;
            Item.shootSpeed = 1;
            Item.shoot = ModContent.ProjectileType<ConductorBubble>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = SoundID.Item15 with { Volume = .4f };
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.MountedCenter + velocity.SafeNormalize(Vector2.UnitX * player.direction) * 40, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class ConductorBubble : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        private readonly List<int> captured = new();
        private bool sentry;
        private bool primed;
        private bool retiring;
        private int timer;
        private float radius = 34;
        public bool Holding => Projectile.active && !retiring;
        public bool Contains(int identity) => Holding && captured.Contains(identity);
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(sentry);
            writer.Write(primed);
            writer.Write(retiring);
            writer.Write((byte)captured.Count);
            foreach (int identity in captured) writer.Write(identity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            sentry = reader.ReadBoolean();
            bool incoming = reader.ReadBoolean();
            primed = incoming;
            retiring = reader.ReadBoolean();
            captured.Clear();
            int count = reader.ReadByte();
            for (int i = 0; i < count; i++) captured.Add(reader.ReadInt32());
        }
        private Projectile Resolve(int identity)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == Projectile.owner && p.identity == identity) return p;
            return null;
        }
        private void ReleaseAll()
        {
            bool armed = false;
            foreach (int identity in captured)
            {
                Projectile p = Resolve(identity);
                if (p == null) continue;
                ConductorCapturedProjectile state = p.GetGlobalProjectile<ConductorCapturedProjectile>();
                bool drop = sentry && !armed && state.HadTileCollision;
                state.Release(p, drop, Projectile.damage);
                armed |= drop;
            }
        }
        private void Retire()
        {
            if (retiring) return;
            retiring = true;
            ReleaseAll();
            Projectile.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            timer++;
            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<ConductorWand>() || player.CCed) Retire();
            if (Projectile.owner == Main.myPlayer && !player.channel) Retire();
            if (retiring) { ReleaseAll(); Projectile.velocity *= .85f; return; }
            Projectile.timeLeft = 20;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 offset = Main.MouseWorld - player.MountedCenter;
                if (offset.Length() > 260) offset = offset.SafeNormalize(Vector2.UnitX) * 260;
                Vector2 travel = player.MountedCenter + offset - Projectile.Center;
                Projectile.velocity = travel * .12f;
                if (Projectile.velocity.Length() > 9) Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 9;
                Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, true, true);
                if (timer % 6 == 0) Projectile.netUpdate = true;
                if (timer % 30 == 0 && !player.CheckMana(player.HeldItem, 2 + 2 * (sentry ? 1 : captured.Count), true)) { Retire(); return; }
                player.manaRegenDelay = player.maxRegenDelay;
                if (timer % 3 == 0)
                {
                    for (int i = captured.Count - 1; i >= 0; i--)
                        if (Resolve(captured[i]) == null) { captured.RemoveAt(i); Projectile.netUpdate = true; }
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.owner != Projectile.owner || p.whoAmI == Projectile.whoAmI || captured.Contains(p.identity) || p.GetGlobalProjectile<ConductorCapturedProjectile>().IsCaptured) continue;
                        bool eligible = p.minion && !p.sentry;
                        if (sentry || captured.Count >= 6 || (!eligible && !(p.sentry && captured.Count == 0))) continue;
                        if (Vector2.Distance(p.Center, Projectile.Center) > radius + Math.Min(p.width, p.height) / 2) continue;
                        if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, p.position, p.width, p.height)) continue;
                        sentry = p.sentry;
                        captured.Add(p.identity);
                        if (sentry && p.ModProjectile is global::AerovelenceMod.Content.Items.Weapons.CrystalCaverns.SaplingCluster)
                            foreach (Projectile child in Main.ActiveProjectiles)
                                if (child.owner == p.owner && child.ModProjectile is global::AerovelenceMod.Content.Items.Weapons.CrystalCaverns.BabySapper && child.ai[0] == p.identity) captured.Add(child.identity);
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item9 with { Volume = .3f, Pitch = .3f }, p.Center);
                    }
                }
                if (!sentry && captured.Count >= 3 && !primed && player.CheckMana(player.HeldItem, 50, true))
                {
                    primed = true;
                    Projectile.netUpdate = true;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ConductorShock>(), Projectile.damage * 2, 5, Projectile.owner, 180, 1);
                    SoundEngine.PlaySound(SoundID.Item122 with { Volume = .4f, Pitch = .2f }, Projectile.Center);
                }
            }
            radius = MathHelper.Lerp(radius, 34 + (sentry ? 14 : captured.Count * 10) + (primed ? 12 : 0), .08f);
            for (int i = 0; i < captured.Count; i++)
            {
                Projectile p = Resolve(captured[i]);
                if (p == null) continue;
                p.GetGlobalProjectile<ConductorCapturedProjectile>().Capture(p, Projectile, i, captured.Count, radius);
            }
            Vector2 aim = (Projectile.Center - player.MountedCenter).SafeNormalize(Vector2.UnitX * player.direction);
            player.direction = aim.X >= 0 ? 1 : -1;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, aim.ToRotation() - MathHelper.PiOver2);
            Lighting.AddLight(Projectile.Center, .1f, .35f, .5f);
            if (timer % 4 == 0) ConductorWandVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius), Main.rand.NextVector2Circular(1, 1), ConductorWandVFX.PhaseColor(0), .15f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center = Projectile.Center - Main.screenPosition;
            float fade = retiring ? Projectile.timeLeft / 20f : Math.Min(1, timer / 12f);
            int visualCount = sentry ? 1 : captured.Count;
            Color color = Color.Lerp(ConductorWandVFX.PhaseColor(0), new Color(255, 177, 45), Math.Clamp(visualCount / 4f, 0f, 1f));
            if (primed) color = Color.Lerp(color, Color.White, 0.5f);
            Vector2 hand = player.MountedCenter - Main.screenPosition;
            Vector2 aim = (center - hand).SafeNormalize(Vector2.UnitX * player.direction);
            if (!retiring)
            {
                Texture2D wand = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/ConductorWand/ConductorWand").Value;
                Texture2D wandGlow = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/ConductorWand/ConductorWand_Glowmask").Value;
                Vector2 grip = new(5f, wand.Height - 5f);
                float rotation = aim.ToRotation() + MathHelper.PiOver4;
                Vector2 wandTip = hand + ((new Vector2(wand.Width - 3f, 3f) - grip) * 0.85f).RotatedBy(rotation);
                Main.EntitySpriteDraw(wand, hand, null, lightColor * fade, rotation, grip, 0.85f, SpriteEffects.None);
                Main.EntitySpriteDraw(wandGlow, hand, null, ConductorWandVFX.Glow(color, (0.35f + Math.Min(4, visualCount) * 0.15f) * fade), rotation, grip, 0.85f, SpriteEffects.None);
                Main.EntitySpriteDraw(wandGlow, hand, null, ConductorWandVFX.Glow(Color.White, primed ? fade * 0.55f : 0f), rotation, grip, 0.85f, SpriteEffects.None);
                ConductorWandVFX.DrawCharge(Main.spriteBatch, wandTip, color, .5f, 7, timer * .04f, fade);
                ConductorWandVFX.DrawElectricLine(Main.spriteBatch, wandTip, center, color, (.15f + Math.Min(4, visualCount) * .13f) * fade, 18, Projectile.identity, 1 + Math.Min(4, visualCount) * .25f);
            }
            float breathe = 1 + MathF.Sin(timer * .09f) * .035f;
            ConductorWandVFX.DrawCorona(Main.spriteBatch, center, radius * breathe, color, fade * .65f, Projectile.identity, 1.7f);
            ConductorWandVFX.DrawCorona(Main.spriteBatch, center, radius * .94f / breathe, color, fade * .2f, Projectile.identity + 23);
            int branches = primed ? 12 : Math.Max(2, visualCount * 2);
            for (int i = 0; i < branches; i++)
            {
                float angle = i * MathHelper.TwoPi / branches + timer * .014f + MathF.Sin(timer * .045f + i * 13) * .3f;
                ConductorWandVFX.DrawElectricLine(Main.spriteBatch, center, center + angle.ToRotationVector2() * radius, color, fade * (primed ? .7f : .25f), 12, i * 27 + Projectile.identity, primed ? 1.8f : 1);
            }
            ConductorWandVFX.DrawCharge(Main.spriteBatch, center, color, primed ? 1 : Math.Min(1, visualCount / 3f), 13, timer * .035f, fade);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            ReleaseAll();
            for (int i = 0; i < 20; i++) ConductorWandVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius), Main.rand.NextVector2Circular(2, 2), ConductorWandVFX.PhaseColor(0), .18f);
        }
    }

    public class ConductorCapturedProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private int bubbleIndex = -1;
        private int bubbleIdentity;
        private bool tileCollision;
        private Vector2 velocity;
        private Vector2 offset;
        private Vector2 heldCenter;
        private bool dropping;
        private int fallTime;
        private int shockDamage;
        public bool IsCaptured => bubbleIndex >= 0;
        public bool HadTileCollision => IsCaptured && tileCollision;
        private Projectile Bubble(Projectile p)
        {
            if (bubbleIndex < 0 || bubbleIndex >= Main.maxProjectiles) return null;
            Projectile bubble = Main.projectile[bubbleIndex];
            return bubble.active && bubble.identity == bubbleIdentity && bubble.owner == p.owner && bubble.ModProjectile is ConductorBubble globe && globe.Contains(p.identity) ? bubble : null;
        }
        public void Capture(Projectile p, Projectile bubble, int slot, int count, float radius)
        {
            if (!IsCaptured)
            {
                tileCollision = p.tileCollide;
                velocity = p.velocity;
                dropping = false;
            }
            bubbleIndex = bubble.whoAmI;
            bubbleIdentity = bubble.identity;
            float angle = Main.GameUpdateCount * .012f + slot * MathHelper.TwoPi / Math.Max(1, count);
            offset = count == 1 ? Vector2.Zero : angle.ToRotationVector2() * radius * .48f;
            p.velocity = Vector2.Zero;
            p.Center = Vector2.Lerp(p.Center, bubble.Center + offset, .25f);
        }
        public void Release(Projectile p, bool drop = false, int damage = 0)
        {
            if (!IsCaptured) return;
            if (tileCollision && Collision.SolidCollision(p.position, p.width, p.height))
            {
                Vector2 origin = p.Center;
                bool found = false;
                for (int distance = 8; distance <= 128 && !found; distance += 8)
                    for (int direction = 0; direction < 8; direction++)
                    {
                        Vector2 candidate = origin + (direction * MathHelper.PiOver4 - MathHelper.PiOver2).ToRotationVector2() * distance;
                        if (Collision.SolidCollision(candidate - p.Size / 2, p.width, p.height)) continue;
                        p.Center = candidate;
                        found = true;
                        break;
                    }
            }
            p.velocity = drop ? new Vector2(0, 1) : velocity;
            bubbleIndex = -1;
            dropping = drop;
            fallTime = 0;
            shockDamage = damage;
            if (p.owner == Main.myPlayer) p.netUpdate = true;
        }
        public override bool PreAI(Projectile projectile)
        {
            if (!IsCaptured) return true;
            Projectile bubble = Bubble(projectile);
            if (bubble == null) { Release(projectile); return true; }
            heldCenter = projectile.Center;
            projectile.velocity = velocity;
            if (projectile.timeLeft < int.MaxValue) projectile.timeLeft++;
            return true;
        }
        public override bool ShouldUpdatePosition(Projectile projectile) => !IsCaptured;
        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => !IsCaptured;
        public override void PostAI(Projectile projectile)
        {
            if (IsCaptured)
            {
                velocity = projectile.velocity;
                projectile.Center = heldCenter;
                projectile.velocity = Vector2.Zero;
                projectile.tileCollide = tileCollision;
                return;
            }
            if (!dropping || projectile.numUpdates != 0) return;
            fallTime++;
            if (fallTime > 240) { dropping = false; return; }
            Vector2 probe = projectile.BottomLeft + new Vector2(0, 1);
            if (fallTime > 5 && projectile.velocity.Y >= 0 && Collision.SolidCollision(probe, projectile.width, 3)) Land(projectile);
        }
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (dropping && oldVelocity.Y > 0 && projectile.velocity.Y != oldVelocity.Y) Land(projectile);
            return true;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (dropping && fallTime > 5 && Collision.SolidCollision(projectile.BottomLeft, projectile.width, 8)) Land(projectile);
        }
        private void Land(Projectile p)
        {
            dropping = false;
            if (fallTime < 6 || p.owner != Main.myPlayer) return;
            float strength = MathHelper.Clamp(fallTime / 60f, .25f, 1.5f);
            Projectile.NewProjectile(p.GetSource_FromAI(), p.Bottom, Vector2.Zero, ModContent.ProjectileType<ConductorShock>(), (int)(shockDamage * strength), 3, p.owner, 64 + strength * 48, 0);
        }
    }

    public class ConductorShock : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        private int age;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        private float Radius => Projectile.ai[0] * MathF.Sin(Math.Min(1, age / 16f) * MathHelper.PiOver2);
        public override void AI()
        {
            age++;
            Lighting.AddLight(Projectile.Center, .3f, .5f, .65f);
            if (age % 2 == 0)
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(Radius, Radius);
                    ConductorWandVFX.SpawnSpark(Projectile.Center + offset, offset.SafeNormalize(Vector2.UnitY) * 2, Color.Lerp(ConductorWandVFX.PhaseColor(0), Color.White, .45f), .22f);
                }
        }
        public override bool? CanDamage() => age <= 22 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= Radius * Radius && Collision.CanHitLine(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] == 1) SkillStrikeUtil.setSkillStrike(Projectile, 1.7f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 180);
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;
            Color color = Color.Lerp(ConductorWandVFX.PhaseColor(0), Color.White, Math.Max(0, 1 - age / 12f));
            float fade = Math.Min(1, Projectile.timeLeft / 18f);
            ConductorWandVFX.DrawCorona(Main.spriteBatch, center, Radius, color, fade, Projectile.identity, 2.3f);
            for (int i = 0; i < 14; i++)
                ConductorWandVFX.DrawElectricLine(Main.spriteBatch, center, center + (i * MathHelper.TwoPi / 14 + age * .025f).ToRotationVector2() * Radius, color, fade * .8f, 16, i * 37 + Projectile.identity, 2);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 18; i++) ConductorWandVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2CircularEdge(Radius, Radius), Main.rand.NextVector2Circular(2, 2), ConductorWandVFX.PhaseColor(0), .15f);
        }
    }

    internal static class ConductorWandVFX
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
