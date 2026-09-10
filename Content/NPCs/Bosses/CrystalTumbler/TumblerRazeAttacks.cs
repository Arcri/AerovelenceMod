using System;
using System.IO;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerResidualField : ModProjectile
    {
        internal const int LaserFieldDuration = 20 * 60;
        private int age;
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ElectricSpikeField";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => age >= 15 && Projectile.timeLeft > 15;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(age);
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            age = reader.ReadInt32();
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override void AI()
        {
            if (age++ == 0)
                Projectile.timeLeft = Math.Max(30, (int)Projectile.ai[0]);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y - 46, (int)Math.Abs(Projectile.velocity.X), 46).Intersects(targetHitbox);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 60);

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = MathHelper.Clamp(age / 15f, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / 25f, 0f, 1f);
            DrawField(Projectile.Center, Projectile.Center.X + Math.Abs(Projectile.velocity.X), Projectile.ai[1], opacity);
            return false;
        }

        internal static void DrawField(Vector2 start, float right, float phase, float opacity)
        {
            if (right <= start.X)
                return;
            Color color = TumblerVFX.PhaseColor(phase);
            TumblerVFX.DrawElectricLine(Main.spriteBatch, start - Main.screenPosition - new Vector2(0f, 18f), new Vector2(right, start.Y - 18f) - Main.screenPosition, color, opacity * 0.75f, Math.Clamp((int)((right - start.X) / 18f), 4, 64), start.X, 2f);
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                Texture2D texture = TumblerPhaseTextures.Get("ElectricSpikeField", phase > 0.5f);
                int count = Math.Max(2, (int)MathF.Ceiling((right - start.X) / 16f));
                int frameY = (int)(Main.GameUpdateCount / 5 % 3) * 26;
                for (int i = 0; i < count; i++)
                {
                    float tileLeft = MathF.Round(MathHelper.Lerp(start.X, right, i / (float)count));
                    float tileRight = MathF.Round(MathHelper.Lerp(start.X, right, (i + 1f) / count));
                    Vector2 position = new(tileLeft, start.Y);
                    float pulse = 0.8f + MathF.Sin(Main.GlobalTimeWrappedHourly * 9f + i) * 0.2f;
                    Rectangle frame = new(i == 0 ? 0 : i == count - 1 ? 54 : 18 + (i % 2) * 18, frameY, 16, 24);
                    Vector2 scale = new((tileRight - tileLeft) / frame.Width, 46f / frame.Height);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, TumblerVFX.Glow(Color.White, opacity * pulse), 0f, new Vector2(0f, frame.Height), scale, SpriteEffects.None, 0f);
                }
            });
        }
    }

    public class TumblerRazeBeam : ModProjectile
    {
        private const int BeamEnd = 390;
        private const int FieldEnd = BeamEnd + TumblerResidualField.LaserFieldDuration;
        private int timer;
        private readonly TumblerLightningVisual lightning = new();
        private bool Active => timer >= 120 && timer < BeamEnd;
        private float Edge => Projectile.ai[1] < 0f ? ArenaData.OuterArenaBoundaryLeft.X + 16f : ArenaData.OuterArenaBoundaryRight.X - 16f;
        private float SweepX => MathHelper.Lerp(Edge, ArenaData.ArenaCenter.X + Math.Sign(Projectile.ai[1]) * 65f, MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((timer - 120f) / 180f, 0f, 1f)));
        private Vector2 BeamTop => new(SweepX, ArenaData.WorldBounds.Top + 32f);
        private Vector2 BeamBottom => new(SweepX, ArenaData.FloorY);
        private float FieldTop => ArenaData.WorldBounds.Top + 32f;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.timeLeft = FieldEnd;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        public override bool? CanDamage() => timer >= 120 && timer < FieldEnd - 15;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            timer++;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, Math.Max(1, FieldEnd - timer));
            int index = (int)Projectile.ai[0];
            if (index < 0 || index >= Main.maxNPCs || !Main.npc[index].active || Main.npc[index].ModNPC is not CrystalTumbler)
            {
                Projectile.Kill();
                return;
            }
            if (timer >= 120 && timer < BeamEnd + 25)
                lightning.Update(Projectile, BeamTop, BeamBottom, 2f, true);
            if (timer == 120)
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.85f, Pitch = -0.3f }, BeamBottom);
            if (Active && timer % 18 == 0)
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.15f, Pitch = -0.5f, MaxInstances = 1 }, BeamBottom);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collision = 0f;
            if (Active && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeamTop, BeamBottom, 22f, ref collision))
                return true;
            float left = Math.Min(Edge, SweepX);
            float right = Math.Max(Edge, SweepX);
            float top = FieldTop;
            return new Rectangle((int)left, (int)top, (int)(right - left), (int)(ArenaData.FloorY - top)).Intersects(targetHitbox);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 75);
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(timer);
        public override void ReceiveExtraAI(BinaryReader reader) => timer = reader.ReadInt32();

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = TumblerVFX.PhaseColor(1f);
            float left = Math.Min(Edge, ArenaData.ArenaCenter.X + Math.Sign(Projectile.ai[1]) * 65f);
            float right = Math.Max(Edge, ArenaData.ArenaCenter.X + Math.Sign(Projectile.ai[1]) * 65f);
            if (timer < 120)
            {
                float charge = timer / 120f;
                for (int row = 0; row < 5; row++)
                {
                    float y = MathHelper.Lerp(ArenaData.FloorY, FieldTop, row / 4f);
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, new Vector2(left, y) - Main.screenPosition, new Vector2(right, y) - Main.screenPosition, color, 0.3f + charge * 0.45f);
                }
                TumblerVFX.DrawTelegraph(Main.spriteBatch, BeamTop - Main.screenPosition, BeamBottom - Main.screenPosition, color, 0.35f + charge * 0.45f);
                TumblerVFX.DrawCharge(Main.spriteBatch, BeamTop - Main.screenPosition, color, charge, 48f, timer * 0.02f);
            }
            else
            {
                float opacity = MathHelper.Clamp((FieldEnd - timer) / 60f, 0f, 1f);
                float beamOpacity = MathHelper.Clamp((BeamEnd + 25f - timer) / 25f, 0f, 1f);
                lightning.Draw(Main.spriteBatch, Color.Lerp(color, Color.White, MathHelper.Clamp((134f - timer) / 14f, 0f, 1f)), beamOpacity, 3f);
                left = Math.Min(Edge, SweepX);
                right = Math.Max(Edge, SweepX);
                TumblerResidualField.DrawField(new Vector2(left, ArenaData.FloorY), right, 1f, opacity);
                TumblerVFX.DrawElectricLine(Main.spriteBatch, new Vector2(left, FieldTop) - Main.screenPosition, new Vector2(right, FieldTop) - Main.screenPosition, color, opacity * 0.65f, 32, Projectile.identity, 2f);
                for (float x = left; x < right; x += 95f)
                    TumblerVFX.DrawElectricLine(Main.spriteBatch, new Vector2(x, ArenaData.FloorY) - Main.screenPosition, new Vector2(x, FieldTop) - Main.screenPosition, color, opacity * 0.4f, 32, x, 1.5f);
            }
            return false;
        }
    }

    public class TumblerConvergenceOrb : ModProjectile
    {
        private int timer;
        private float groundX;
        private readonly TumblerLightningVisual channel = new();
        private readonly TumblerLightningVisual raze = new();
        private bool Active => timer >= 210 && timer < 510;
        private float Growth => MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((timer - 40f) / 170f, 0f, 1f));
        private float CoreRadius => 12f + Growth * 78f + MathHelper.Clamp((timer - 210f) / 300f, 0f, 1f) * 18f;
        internal static Vector2 Anchor => new(ArenaData.ArenaCenter.X, ArenaData.FloorY - 260f);
        private Vector2 Crystal(int index) => ArenaData.CrystalPositions.Length >= 3 ? ArenaData.CrystalPositions[index] : ArenaData.ArenaCenter - new Vector2(0f, 250f);
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumblerOrb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 68;
            Projectile.timeLeft = 550;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2800;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Active;

        public override void AI()
        {
            timer++;
            int bossIndex = (int)Projectile.ai[0];
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, Math.Max(1, 550 - timer));
            if (bossIndex < 0 || bossIndex >= Main.maxNPCs || !Main.npc[bossIndex].active || Main.npc[bossIndex].ai[0] != (float)TumblerState.CrystalConvergence)
            {
                Projectile.Kill();
                return;
            }
            Projectile.frame = timer / 5 % 4;
            Projectile.rotation += 0.035f;
            Projectile.Center = Anchor;
            if (timer == 1)
                groundX = ArenaData.ArenaCenter.X;
            if (timer == 40 || timer == 80 || timer == 120 || timer == 210)
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.45f + timer / 800f, Pitch = 0.3f - timer / 400f }, Projectile.Center);
            if (timer >= 120)
                channel.Update(Projectile, Crystal(1), Projectile.Center, 2f, true);
            if (Active)
            {
                Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                groundX += MathHelper.Clamp(player.Center.X - groundX, -2.2f, 2.2f);
                groundX = MathHelper.Clamp(groundX, ArenaData.OuterArenaBoundaryLeft.X + 60f, ArenaData.OuterArenaBoundaryRight.X - 60f);
                raze.Update(Projectile, Projectile.Center, new Vector2(groundX, ArenaData.FloorY), 2f, true);
                if (timer % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LeaveGroundCurrent();
                    Projectile.netUpdate = true;
                }
            }
            if (!Main.dedServ && timer >= 40 && timer < 510)
            {
                Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(1f).ToVector3() * (0.45f + Growth));
                if (timer % 3 == 0)
                {
                    Vector2 direction = Main.rand.NextVector2Unit();
                    Vector2 position = Projectile.Center + direction * CoreRadius * (Active ? 0.95f : 1.65f);
                    Vector2 velocity = direction * (Active ? 2.6f : -2.8f) + direction.RotatedBy(MathHelper.PiOver2) * 1.3f;
                    TumblerVFX.SpawnSpark(position, velocity, Color.Lerp(TumblerVFX.PhaseColor(1f), Color.White, 0.6f), 0.2f + Growth * 0.12f);
                }
                if (timer == 210)
                {
                    for (int i = 0; i < 28; i++)
                    {
                        Vector2 direction = (i * MathHelper.TwoPi / 28f).ToRotationVector2();
                        TumblerVFX.SpawnSpark(Projectile.Center + direction * CoreRadius, direction * 5f, Color.White, 0.38f);
                    }
                }
            }
        }

        private void LeaveGroundCurrent()
        {
            int type = ModContent.ProjectileType<TumblerResidualField>();
            foreach (Projectile field in Main.ActiveProjectiles)
            {
                if (field.type != type || field.owner != Projectile.owner || Math.Abs(field.Center.X + field.velocity.X * 0.5f - groundX) > 26f)
                    continue;
                field.timeLeft = TumblerResidualField.LaserFieldDuration;
                field.netUpdate = true;
                return;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(groundX - 30f, ArenaData.FloorY), new Vector2(60f, 0f), type, Projectile.damage, 0f, Projectile.owner, TumblerResidualField.LaserFieldDuration, 1f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collision = 0f;
            Vector2 closest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(closest, Projectile.Center) <= MathF.Pow(CoreRadius * 0.8f, 2f)
                || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, new Vector2(groundX, ArenaData.FloorY), 24f, ref collision);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 75);
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(groundX);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            groundX = reader.ReadSingle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color color = TumblerVFX.PhaseColor(1f);
            float fade = MathHelper.Clamp((550f - timer) / 40f, 0f, 1f);
            for (int i = 0; i < 3; i++)
            {
                float charge = MathHelper.Clamp((timer - i * 40f) / 40f, 0f, 1f);
                Vector2 start = Crystal(i) - Main.screenPosition;
                Vector2 end = Projectile.Center - Main.screenPosition;
                TumblerVFX.DrawCharge(spriteBatch, start, color, charge, 26f, timer * 0.02f + i, fade);
                if (charge >= 1f && (i != 1 || timer < 120))
                    TumblerVFX.DrawElectricLine(spriteBatch, start, end, color, fade * 0.7f, 24, i * 7f, 3f);
                else if (charge > 0f && charge < 1f)
                    TumblerVFX.DrawTelegraph(spriteBatch, start, end, color, charge * 0.65f);
            }
            float growth = Growth;
            Vector2 position = Projectile.Center - Main.screenPosition;
            if (timer >= 120)
                channel.Draw(spriteBatch, color, fade * growth, 2.5f);
            float coreOpacity = fade * MathHelper.Clamp((timer - 40f) / 35f, 0f, 1f);
            TumblerStormCore.Draw(Projectile.Center, CoreRadius, coreOpacity, timer, growth, Projectile.identity);
            if (timer < 210 && timer >= 120)
            {
                Vector2 ground = new(groundX, ArenaData.FloorY);
                TumblerVFX.DrawTelegraph(spriteBatch, position, ground - Main.screenPosition, color, growth * 0.85f);
                TumblerVFX.DrawCharge(spriteBatch, ground - Main.screenPosition, color, growth, 32f, -timer * 0.03f);
            }
            if (timer >= 210)
            {
                raze.Draw(spriteBatch, color, fade, 3f);
                TumblerVFX.DrawCharge(spriteBatch, new Vector2(groundX, ArenaData.FloorY) - Main.screenPosition, color, 1f, 25f, timer * 0.04f, fade);
            }
            return false;
        }
    }
}
