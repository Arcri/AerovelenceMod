using System;
using System.Collections.Generic;
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
    public class TumblerArenaGate : ModProjectile
    {
        private bool impactPlayed;
        private float phase;
        private int DropStart => Projectile.ai[1] < 0f ? 60 : 210;
        private float Height => Projectile.ai[2];
        private int TileX => (int)(Projectile.Center.X / 16f);
        private float IntroTime => TryBoss(out NPC boss) && boss.ai[0] == (float)TumblerState.Spawn ? boss.ai[1] : 600f;
        private float Closure => MathF.Pow(MathHelper.Clamp((IntroTime - DropStart) / 55f, 0f, 1f), 2f);
        public override string Texture => "Terraria/Images/Projectile_0";

        public static float FindTop(int x)
        {
            int floor = (int)(ArenaData.FloorY / 16f);
            for (int y = floor - 1; y >= ArenaData.TileBounds.Top; y--)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && tile.TileType != TileID.SapphireGemspark)
                    return (y + 1) * 16f;
            }
            return ArenaData.WorldBounds.Top + 16f;
        }

        public static void SpawnGates(NPC boss)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !ArenaData.Valid)
                return;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is TumblerArenaGate)
                    projectile.Kill();
            }
            ArenaData.ClearTemporaryBarriers();
            float leftX = ArenaData.OuterArenaBoundaryLeft.X;
            float rightX = ArenaData.OuterArenaBoundaryRight.X;
            float leftTop = FindTop((int)(leftX / 16f));
            float rightTop = FindTop((int)(rightX / 16f));
            ArenaData.CreateTemporaryBarriers();
            ArenaData.SetGateClosure((int)(leftX / 16f), leftTop);
            ArenaData.SetGateClosure((int)(rightX / 16f), rightTop);
            Projectile.NewProjectile(boss.GetSource_FromAI(), new Vector2(leftX + 8f, leftTop), Vector2.Zero, ModContent.ProjectileType<TumblerArenaGate>(), 18, 0f, Main.myPlayer, boss.whoAmI, -1f, ArenaData.FloorY - leftTop);
            Projectile.NewProjectile(boss.GetSource_FromAI(), new Vector2(rightX + 8f, rightTop), Vector2.Zero, ModContent.ProjectileType<TumblerArenaGate>(), 18, 0f, Main.myPlayer, boss.whoAmI, 1f, ArenaData.FloorY - rightTop);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 3600;
        }

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        public override bool ShouldUpdatePosition() => false;

        private bool TryBoss(out NPC boss)
        {
            int index = (int)Projectile.ai[0];
            boss = index >= 0 && index < Main.maxNPCs ? Main.npc[index] : null;
            return boss != null && boss.active && boss.ModNPC is CrystalTumbler && boss.ai[0] != (float)TumblerState.Despawn;
        }

        public override void AI()
        {
            if (!TryBoss(out NPC boss))
            {
                ArenaData.ClearTemporaryBarriers();
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 3600;
            float closure = Closure;
            ArenaData.SetGateClosure(TileX, Projectile.Center.Y + Height * closure);
            phase = MathHelper.Lerp(phase, boss.ai[2] >= 1f ? 1f : 0f, 0.025f);
            if (closure >= 1f && !impactPlayed)
            {
                impactPlayed = true;
                if (IntroTime < DropStart + 70)
                {
                    SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam") with { Volume = 0.65f, Pitch = -0.3f }, new Vector2(Projectile.Center.X, ArenaData.FloorY));
                    if (!Main.dedServ)
                    {
                        Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Max(Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower, 6f);
                        for (int i = 0; i < 10; i++)
                            Dust.NewDustPerfect(new Vector2(Projectile.Center.X, ArenaData.FloorY - 5f), ModContent.DustType<TumblerRollDust>(), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2.5f, -0.5f)), 50, new Color(100, 110, 125), Main.rand.NextFloat(0.12f, 0.2f));
                    }
                }
            }
            if (!Main.dedServ && closure >= 1f)
            {
                Color color = Color.Lerp(TumblerVFX.PhaseColor(0f), TumblerVFX.PhaseColor(1f), phase);
                Vector2 point = Projectile.Center + new Vector2(0f, Main.rand.NextFloat(Height));
                Lighting.AddLight(point, color.ToVector3() * 0.6f);
                if (Main.GameUpdateCount % 7 == 0)
                    TumblerVFX.SpawnSpark(point, Main.rand.NextVector2Circular(1.3f, 1.3f), color, 0.22f);
            }
        }

        public override bool? CanDamage() => Closure >= 1f && TryBoss(out NPC boss) && boss.ai[0] != (float)TumblerState.Spawn;

        public override void OnKill(int timeLeft)
        {
            if (!TryBoss(out _))
                ArenaData.ClearTemporaryBarriers();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collision = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0f, Height), 26f, ref collision);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 75);

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadTiles(TileID.SapphireGemspark);
            Texture2D texture = TextureAssets.Tile[TileID.SapphireGemspark].Value;
            float closure = Closure;
            float displacement = Height * (1f - closure);
            float opacity = TumblerProjectileRetirement.VisualOpacity(Projectile);
            Color color = Color.Lerp(TumblerVFX.PhaseColor(0f), TumblerVFX.PhaseColor(1f), phase);
            float flash = MathHelper.Clamp(1f - (IntroTime - DropStart - 55f) / 18f, 0f, 1f);
            for (int row = 0; row < (int)(Height / 16f); row++)
            {
                float y = Projectile.Center.Y + row * 16f - displacement;
                int clip = (int)MathF.Ceiling(Math.Max(0f, Projectile.Center.Y - y));
                if (clip >= 16)
                    continue;
                Tile tile = Framing.GetTileSafely(TileX, (int)(Projectile.Center.Y / 16f) + row);
                Rectangle source = new(tile.TileFrameX, tile.TileFrameY + clip, 16, 16 - clip);
                Vector2 position = new(Projectile.Center.X - 8f, y + clip);
                Main.spriteBatch.Draw(texture, position - Main.screenPosition, source, Color.Lerp(Color.White, color, 0.25f) * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            if (closure >= 1f)
            {
                int count = Math.Max(2, (int)(Height / 16f));
                Vector2[] points = new Vector2[count + 1];
                for (int i = 0; i <= count; i++)
                {
                    float jitter = i == 0 || i == count ? 0f : MathF.Sin(i * 8.13f + Main.GameUpdateCount / 3f) * 6f;
                    points[i] = Projectile.Center + new Vector2(jitter, Height * i / count);
                }
                TumblerLightningSystem.DrawPath(points, Color.Lerp(color, Color.White, flash), opacity * 0.8f, 2.5f + flash * 2f, true, RenderLayer.UnderTiles, 0f);
            }
            return false;
        }
    }

    public class TumblerGateTileDrawing : GlobalTile
    {
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (type != TileID.SapphireGemspark || !ArenaData.Valid)
                return true;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is TumblerArenaGate && (int)(projectile.Center.X / 16f) == i && j * 16f >= projectile.Center.Y && j * 16f < projectile.Center.Y + projectile.ai[2])
                    return false;
            }
            return true;
        }
    }
}
