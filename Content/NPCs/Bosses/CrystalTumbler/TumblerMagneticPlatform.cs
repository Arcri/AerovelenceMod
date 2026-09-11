using System;
using System.IO;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerMagneticPlatform : ModProjectile
    {
        private int age;
        private int crushTimer = -1;
        private int warningTicks = 90;
        private int holdTicks = 45;
        private float homeX;
        private float spawnY;
        private float ceilingY;
        private float compression;
        private float springVelocity;
        private Vector2 returnPosition;
        private float colorCharge;
        private float proximitySink;
        private int occupiedTicks;
        private int collapseTimer = -1;
        public bool Collapsing => collapseTimer >= 0;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RockProjectile";
        public float SurfaceY => Projectile.Top.Y;
        public Vector2 SurfaceStart => Projectile.TopLeft;
        public Vector2 SurfaceEnd => Projectile.TopRight;
        public bool CanStand => TumblerProjectileRetirement.VisualOpacity(Projectile) >= 1f && age >= Projectile.ai[2] + 35f && Projectile.Opacity >= 0.85f && collapseTimer < 60;
        public bool Crushing => crushTimer >= warningTicks && crushTimer < warningTicks + 140 + holdTicks;

        public static bool IsArenaPlatform(Projectile projectile)
        {
            return projectile.active && projectile.type == ModContent.ProjectileType<TumblerMagneticPlatform>();
        }

        public static int Spawn(NPC boss, Vector2 restingCenter, int delayTicks = 0)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return -1;
            Vector2 origin = new(restingCenter.X, ArenaData.FloorY + 22f);
            return Projectile.NewProjectile(boss.GetSource_FromAI(), origin, Vector2.Zero, ModContent.ProjectileType<TumblerMagneticPlatform>(), 0, 0f, Main.myPlayer, boss.whoAmI, restingCenter.Y, Math.Max(0, delayTicks));
        }

        public static void BeginCrush(NPC boss, int warningTicks = 90, int holdTicks = 45)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!IsArenaPlatform(projectile) || projectile.ai[0] != boss.whoAmI || ((TumblerMagneticPlatform)projectile.ModProjectile).Collapsing)
                    continue;
                TumblerMagneticPlatform platform = (TumblerMagneticPlatform)projectile.ModProjectile;
                if (platform.crushTimer >= 0)
                    continue;
                platform.warningTicks = Math.Max(60, warningTicks);
                platform.holdTicks = Math.Max(15, holdTicks);
                platform.crushTimer = 0;
                projectile.netUpdate = true;
            }
        }

        public static bool CollapseAll(NPC boss)
        {
            bool found = false;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (!IsArenaPlatform(projectile) || projectile.ai[0] != boss.whoAmI)
                    continue;
                found = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    ((TumblerMagneticPlatform)projectile.ModProjectile).BeginCollapse();
            }
            return found;
        }

        private void BeginCollapse()
        {
            if (collapseTimer >= 0)
                return;
            collapseTimer = 0;
            Projectile.netUpdate = true;
        }

        public static void Release(NPC boss)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!IsArenaPlatform(projectile) || projectile.ai[0] != boss.whoAmI)
                    continue;
                TumblerMagneticPlatform platform = (TumblerMagneticPlatform)projectile.ModProjectile;
                if (platform.Collapsing || platform.crushTimer < 0 || platform.crushTimer >= platform.warningTicks + 140 + platform.holdTicks)
                    continue;
                platform.returnPosition = projectile.Center;
                platform.crushTimer = platform.warningTicks + 140 + platform.holdTicks;
                projectile.netUpdate = true;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 30;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            homeX = Projectile.Center.X;
            spawnY = Projectile.Center.Y;
            ceilingY = Projectile.ai[1] - 160f;
            if (!ArenaData.Valid)
                return;
            for (float y = Projectile.ai[1] - 60f; y >= ArenaData.WorldBounds.Top + 16f; y -= 8f)
            {
                if (!Collision.SolidCollision(new Vector2(homeX - Projectile.width * 0.5f, y), Projectile.width, 4))
                    continue;
                ceilingY = Math.Min(Projectile.ai[1] - 40f, y + 40f);
                break;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            int bossIndex = (int)Projectile.ai[0];
            if (bossIndex < 0 || bossIndex >= Main.maxNPCs || !Main.npc[bossIndex].active || Main.npc[bossIndex].type != ModContent.NPCType<CrystalTumbler>() || Main.npc[bossIndex].ai[0] == (float)TumblerState.Despawn)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 3600;
            age++;
            float targetColor = Main.npc[bossIndex].ai[2] >= 1f ? 1f : crushTimer < 0 ? 0f : MathHelper.Clamp(crushTimer / (float)warningTicks, 0f, 1f);
            colorCharge = MathHelper.Lerp(colorCharge, targetColor, 0.035f);
            if (collapseTimer >= 0)
            {
                collapseTimer++;
                if (collapseTimer == 1)
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.45f, Pitch = -0.5f }, Projectile.Center);
                if (collapseTimer is 24 or 42 or 54)
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.35f, Pitch = collapseTimer / 70f }, Projectile.Center);
                if (collapseTimer == 60 && !Main.dedServ)
                {
                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollideBetter") with { Volume = 0.65f, Pitch = -0.2f }, Projectile.Center);
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 direction = (i * MathHelper.TwoPi / 18f).ToRotationVector2();
                        TumblerVFX.SpawnSpark(Projectile.Center + direction * 30f, direction * 4f, Color.White, 0.3f);
                    }
                    for (int i = 0; i < 8; i++)
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(55f, 12f), DustID.Stone, Main.rand.NextVector2Circular(3f, 2f), 0, new Color(135, 145, 155), 1.2f);
                }
                if (collapseTimer < 60)
                {
                    Projectile.position.X += MathF.Sin(collapseTimer * 1.7f) * collapseTimer / 50f;
                    if (!Main.dedServ && collapseTimer % 6 == 0)
                        TumblerVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2Circular(55f, 12f), new Vector2(0f, 1f), Color.White, 0.23f);
                }
                else
                {
                    Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.5f, 13f);
                    Projectile.position += Projectile.velocity;
                    if (Projectile.Bottom.Y >= ArenaData.FloorY)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Left.X - 20f, ArenaData.FloorY), new Vector2(Projectile.width + 40f, 0f), ModContent.ProjectileType<TumblerResidualField>(), 14, 0f, Main.myPlayer, 180f, Main.npc[bossIndex].ai[2]);
                        Projectile.Kill();
                    }
                }
                return;
            }
            if (crushTimer >= 0)
                crushTimer++;
            float emergence = MathHelper.Clamp((age - Projectile.ai[2]) / 100f, 0f, 1f);
            Projectile.Opacity = MathHelper.Clamp((age - Projectile.ai[2]) / 28f, 0f, 1f);
            float load = 0f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && player.velocity.Y >= 0f && !player.controlDown && player.Right.X > Projectile.Left.X && player.Left.X < Projectile.Right.X && Math.Abs(player.Bottom.Y - SurfaceY) < 10f)
                    load = 7f;
            }
            foreach (Projectile hook in Main.ActiveProjectiles)
            {
                if (hook.aiStyle == ProjAIStyleID.Hook && hook.ai[0] == 2f && hook.Hitbox.Intersects(Projectile.Hitbox))
                    load = 7f;
            }
            if (load > 0f && Main.netMode != NetmodeID.MultiplayerClient && ++occupiedTicks >= 240)
                BeginCollapse();
            float proximity = MathHelper.Clamp(1f - Math.Abs(Main.npc[bossIndex].Center.X - Projectile.Center.X) / 240f, 0f, 1f);
            proximitySink = MathHelper.Lerp(proximitySink, proximity * 46f, 0.035f);
            springVelocity = (springVelocity + (load - compression) * 0.07f) * 0.78f;
            compression = MathHelper.Clamp(compression + springVelocity, -2f, 10f);
            float lift = crushTimer < 0 ? 0f : MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((crushTimer - warningTicks) / 140f, 0f, 1f));
            int releaseTime = warningTicks + 140 + holdTicks;
            if (crushTimer == releaseTime)
                returnPosition = Projectile.Center;
            float drift = MathF.Sin(age * 0.009f + Projectile.identity * 1.9f) * 15f;
            float bob = MathF.Sin(age * 0.019f + Projectile.identity * 1.9f) * 3f;
            Vector2 rest = new(homeX + drift, Projectile.ai[1] + bob + compression + proximitySink);
            rest = Vector2.Lerp(new Vector2(homeX, spawnY), rest, MathHelper.SmoothStep(0f, 1f, emergence));
            Vector2 next;
            if (crushTimer >= releaseTime)
            {
                float progress = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((crushTimer - releaseTime) / 120f, 0f, 1f));
                lift = 1f - progress;
                next = Vector2.Lerp(returnPosition, rest, progress);
                if (crushTimer >= releaseTime + 120)
                    crushTimer = -1;
            }
            else
                next = Vector2.Lerp(rest, new Vector2(homeX, ceilingY), lift * emergence);
            Projectile.Center = next;
            if (Main.netMode == NetmodeID.Server && age % 90 == 0)
                Projectile.netUpdate = true;
            Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(Main.npc[bossIndex].ai[2]).ToVector3() * (0.25f + lift * 0.3f) * Projectile.Opacity);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(age);
            writer.Write(crushTimer);
            writer.Write(warningTicks);
            writer.Write(holdTicks);
            writer.Write(homeX);
            writer.Write(spawnY);
            writer.Write(ceilingY);
            writer.Write(compression);
            writer.Write(springVelocity);
            writer.WriteVector2(returnPosition);
            writer.Write(proximitySink);
            writer.Write(collapseTimer);
            writer.Write(occupiedTicks);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            age = reader.ReadInt32();
            crushTimer = reader.ReadInt32();
            warningTicks = reader.ReadInt32();
            holdTicks = reader.ReadInt32();
            homeX = reader.ReadSingle();
            spawnY = reader.ReadSingle();
            ceilingY = reader.ReadSingle();
            compression = reader.ReadSingle();
            springVelocity = reader.ReadSingle();
            returnPosition = reader.ReadVector2();
            proximitySink = reader.ReadSingle();
            collapseTimer = reader.ReadInt32();
            occupiedTicks = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D rock = TextureAssets.Projectile[Type].Value;
            Rectangle frame = rock.Frame(1, 3, 0, 0);
            frame.Inflate(-3, -3);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 center = Projectile.Center - Main.screenPosition;
            int bossIndex = (int)Projectile.ai[0];
            Color color = Color.Lerp(TumblerVFX.PhaseColor(0f), TumblerVFX.PhaseColor(1f), colorCharge);
            float warning = crushTimer < 0 ? 0f : MathHelper.Clamp(crushTimer / (float)warningTicks, 0f, 1f);
            float opacity = Projectile.Opacity * TumblerProjectileRetirement.VisualOpacity(Projectile);
            float demagnetizing = collapseTimer < 0 ? 0f : MathHelper.Clamp(collapseTimer / 60f, 0f, 1f);
            float magneticOpacity = opacity * (1f - demagnetizing) * (collapseTimer < 0 ? 1f : 0.45f + 0.55f * Math.Abs(MathF.Sin(collapseTimer * 0.65f)));
            Color charged = Color.Lerp(color, Color.White, collapseTimer < 0 ? 0f : 0.5f + MathF.Sin(age * 0.5f) * 0.3f);
            spriteBatch.Draw(bloom, center + new Vector2(0f, 14f), null, TumblerVFX.Glow(charged, magneticOpacity * (0.16f + warning * 0.18f)), 0f, bloom.Size() * 0.5f, new Vector2(154f, 56f) / bloom.Size(), SpriteEffects.None, 0f);
            for (int i = -1; i <= 1; i++)
            {
                Vector2 piece = center + new Vector2(i * (40f + demagnetizing * 8f), (i == 0 ? 2f : 4f) + Math.Abs(i) * demagnetizing * 6f);
                Vector2 size = new(i == 0 ? 62f : 54f, i == 0 ? 39f : 32f);
                spriteBatch.Draw(rock, piece, frame, Color.Lerp(lightColor, Color.White, 0.2f) * opacity, i * 0.06f, frame.Size() * 0.5f, size / frame.Size(), i < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                spriteBatch.Draw(rock, piece, frame, TumblerVFX.Glow(charged, magneticOpacity * 0.12f), i * 0.06f, frame.Size() * 0.5f, size / frame.Size(), i < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            Vector2 left = SurfaceStart - Main.screenPosition;
            Vector2 right = SurfaceEnd - Main.screenPosition;
            if (collapseTimer >= 0 && collapseTimer < 86)
            {
                float discharge = (collapseTimer < 60 ? 0.45f + demagnetizing * 0.55f : (86f - collapseTimer) / 26f) * opacity;
                float separation = collapseTimer < 60 ? demagnetizing * 18f : 18f + (collapseTimer - 60f) * 3f;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 anchor = Vector2.Lerp(left, right, (i + 0.5f) / 6f) + new Vector2(0f, 18f);
                    Vector2 end = anchor + new Vector2(MathF.Sin(age * 0.13f + i) * (15f + separation), 30f + separation);
                    TumblerVFX.DrawElectricLine(spriteBatch, anchor, end, Color.Lerp(color, Color.White, 0.7f), discharge, 12, Projectile.identity + i * 19 + age / 3, 2.2f);
                }
                for (int i = -1; i <= 1; i += 2)
                    TumblerVFX.DrawElectricLine(spriteBatch, center + new Vector2(i * 12f, 0f), center + new Vector2(i * (55f + separation), 6f), Color.White, discharge * 0.8f, 12, age / 2 + i, 2f);
                if (collapseTimer >= 60)
                    TumblerVFX.DrawCorona(spriteBatch, center, 65f + (collapseTimer - 60f) * 3f, Color.White, discharge * 0.9f, Projectile.identity, 3f);
            }
            TumblerVFX.DrawLine(spriteBatch, left + new Vector2(5f, 1f), right - new Vector2(5f, -1f), TumblerVFX.Glow(charged, magneticOpacity * 0.65f), 2f);
            for (int i = 0; i < 5; i++)
            {
                Vector2 anchor = Vector2.Lerp(left, right, (i + 0.5f) / 5f) + new Vector2(0f, 29f);
                float swing = MathF.Sin(age * 0.045f + i * 1.8f) * 5f;
                Vector2 tip = anchor + new Vector2(swing, 9f + MathF.Sin(age * 0.025f + i) * 3f);
                TumblerVFX.DrawElectricLine(spriteBatch, anchor, tip, charged, magneticOpacity * 0.45f, 4, Projectile.identity + i, 1f);
                spriteBatch.Draw(star, tip, null, TumblerVFX.Glow(charged, magneticOpacity * 0.65f), 0f, star.Size() * 0.5f, 12f / star.Width, SpriteEffects.None, 0f);
            }
            if (crushTimer >= 0 && crushTimer < warningTicks)
            {
                Vector2 target = new(center.X, ceilingY - Projectile.height * 0.5f - Main.screenPosition.Y);
                TumblerVFX.DrawTelegraph(spriteBatch, new Vector2(center.X, SurfaceY - Main.screenPosition.Y - 12f), target, charged, (0.24f + warning * 0.45f) * opacity, 32f);
                TumblerVFX.DrawCorona(spriteBatch, center, 74f, charged, warning * opacity * 0.25f, Projectile.identity, 1f);
                for (int i = 0; i < 3; i++)
                {
                    float step = (age * 0.02f + i / 3f) % 1f;
                    Vector2 arrow = center + new Vector2(0f, -30f - step * 44f);
                    Color arrowColor = TumblerVFX.Glow(charged, (1f - step) * (0.4f + warning * 0.4f) * opacity);
                    TumblerVFX.DrawLine(spriteBatch, arrow + new Vector2(-7f, 7f), arrow, arrowColor, 1.6f);
                    TumblerVFX.DrawLine(spriteBatch, arrow + new Vector2(7f, 7f), arrow, arrowColor, 1.6f);
                }
            }
            return false;
        }
    }

    public class TumblerPlatformCollision : ModSystem
    {
        public override void Load()
        {
            On_Player.SlopingCollision += ResolvePlatforms;
        }

        public override void Unload()
        {
            On_Player.SlopingCollision -= ResolvePlatforms;
        }

        private static void ResolvePlatforms(On_Player.orig_SlopingCollision orig, Player player, bool fallThrough, bool ignorePlats)
        {
            orig(player, fallThrough, ignorePlats);
            player.GetModPlayer<TumblerPlatformPlayer>().ResolveStanding(fallThrough || ignorePlats);
            player.GetModPlayer<TumblerRipplePlayer>().Resolve();
        }
    }

    public class TumblerPlatformPlayer : ModPlayer
    {
        private int platformIndex = -1;
        private int platformIdentity = -1;
        private int detachTimer;
        private Vector2 previousBottom;
        private Vector2 previousPlatformCenter;

        public override void PreUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
                return;
            previousBottom = Player.Bottom;
            if (detachTimer > 0)
                detachTimer--;
            if (Player.dead || !Player.active || Player.controlDown || Player.gravDir < 0f || Player.pulley || Player.GoingDownWithGrapple)
            {
                Detach(Player.controlDown ? 16 : 3);
                return;
            }
            if (!TryGetPlatform(out TumblerMagneticPlatform platform))
                return;
            if (Player.velocity.Y < -0.5f || Player.justJumped || Player.Right.X <= platform.SurfaceStart.X || Player.Left.X >= platform.SurfaceEnd.X)
            {
                Detach(3);
                return;
            }
            Vector2 carry = platform.Projectile.Center - previousPlatformCenter;
            if (carry.LengthSquared() > 400f)
            {
                Detach(6);
                return;
            }
            Vector2 allowed = Collision.TileCollision(Player.position, carry, Player.width, Player.height, true, true, (int)Player.gravDir);
            if (carry.Y < -0.01f && allowed.Y > carry.Y + 0.5f)
            {
                int escapeDirection = Player.Center.X < platform.Projectile.Center.X ? -1 : 1;
                if (platform.Crushing && Player.whoAmI == Main.myPlayer)
                    Player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(Player.name + " was caught between a magnetic boulder and the cavern ceiling.")), 18, escapeDirection);
                Player.velocity = new Vector2(escapeDirection * 5f, 2f);
                Detach(30);
                return;
            }
            Player.position += allowed;
            previousBottom += allowed;
            previousPlatformCenter = platform.Projectile.Center;
            Player.velocity.Y = 0f;
            Player.fallStart = (int)(Player.position.Y / 16f);
        }

        public override void PreUpdateMovement()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
                return;
            if (Player.velocity.Y < 0f || Player.justJumped || Player.controlDown)
            {
                Detach(Player.controlDown ? 16 : 3);
                return;
            }
            if (TryGetPlatform(out TumblerMagneticPlatform platform) && Math.Abs(Player.Bottom.Y - platform.SurfaceY) <= 8f && Player.Right.X > platform.SurfaceStart.X && Player.Left.X < platform.SurfaceEnd.X)
                Player.velocity.Y = 0f;
        }

        internal void ResolveStanding(bool fallThrough)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
                return;
            if (fallThrough || !Player.active || Player.dead || detachTimer > 0 || Player.controlDown || Player.gravDir < 0f || Player.velocity.Y < 0f || Player.justJumped || Player.GoingDownWithGrapple)
                return;
            TumblerMagneticPlatform landing = null;
            float highest = float.MaxValue;
            if (TryGetPlatform(out TumblerMagneticPlatform current) && Math.Abs(Player.Bottom.Y - current.SurfaceY) <= 8f && Player.Right.X > current.SurfaceStart.X + 3f && Player.Left.X < current.SurfaceEnd.X - 3f)
            {
                landing = current;
                highest = current.SurfaceY;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!TumblerMagneticPlatform.IsArenaPlatform(projectile))
                    continue;
                TumblerMagneticPlatform platform = (TumblerMagneticPlatform)projectile.ModProjectile;
                if (!platform.CanStand || Player.Right.X <= platform.SurfaceStart.X + 3f || Player.Left.X >= platform.SurfaceEnd.X - 3f)
                    continue;
                float top = platform.SurfaceY;
                if (previousBottom.Y > top + 8f || Player.Bottom.Y < top || top >= highest)
                    continue;
                landing = platform;
                highest = top;
            }
            if (landing == null)
            {
                platformIndex = -1;
                return;
            }
            Vector2 destination = new(Player.position.X, highest - Player.height);
            if (Collision.SolidCollision(destination, Player.width, Player.height))
            {
                Detach(12);
                return;
            }
            Player.position = destination;
            Player.velocity.Y = 0f;
            Player.jump = 0;
            Player.fallStart = (int)(Player.position.Y / 16f);
            Player.gfxOffY = 0f;
            platformIndex = landing.Projectile.whoAmI;
            platformIdentity = landing.Projectile.identity;
            previousPlatformCenter = landing.Projectile.Center;
        }

        private bool TryGetPlatform(out TumblerMagneticPlatform platform)
        {
            platform = null;
            if (platformIndex < 0 || platformIndex >= Main.maxProjectiles)
                return false;
            Projectile projectile = Main.projectile[platformIndex];
            if (!TumblerMagneticPlatform.IsArenaPlatform(projectile) || projectile.identity != platformIdentity)
            {
                platformIndex = -1;
                return false;
            }
            platform = (TumblerMagneticPlatform)projectile.ModProjectile;
            return platform.CanStand;
        }

        private void Detach(int ticks)
        {
            platformIndex = -1;
            platformIdentity = -1;
            detachTimer = Math.Max(detachTimer, ticks);
        }
    }
}
