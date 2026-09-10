using System;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerIntroPlayer : ModPlayer
    {
        private bool ownsCamera;
        private float savedZoom;
        private Vector2 initialFocus;
        private int pushDirection;
        private int pushTicks;

        private bool TryIntro(out NPC boss)
        {
            boss = null;
            if (!ArenaData.Valid || !Player.active || Player.dead)
                return false;
            Rectangle bounds = ArenaData.WorldBounds;
            bounds.Inflate(240, 240);
            if (!bounds.Contains(Player.Center.ToPoint()))
                return false;
            int index = NPC.FindFirstNPC(ModContent.NPCType<CrystalTumbler>());
            if (index < 0)
                return false;
            boss = Main.npc[index];
            return boss.ai[0] == (float)TumblerState.Spawn && boss.ai[1] < CrystalTumbler.EntranceDuration;
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable) => TryIntro(out _);

        public override void PreUpdateMovement()
        {
            if (Player.whoAmI != Main.myPlayer || !TryIntro(out NPC boss))
            {
                pushDirection = 0;
                pushTicks = 0;
                return;
            }
            foreach (Projectile gate in Main.ActiveProjectiles)
            {
                if (gate.ModProjectile is not TumblerArenaGate)
                    continue;
                int start = gate.ai[1] < 0f ? 60 : 210;
                if (boss.ai[1] >= start - 15 && boss.ai[1] < start + 55 && Math.Abs(Player.Center.X - gate.Center.X) < 38f && Player.Bottom.Y > gate.Center.Y)
                    Player.velocity.X = -Math.Sign(gate.ai[1]) * 5f;
            }
            if (boss.ai[1] < 455)
            {
                pushDirection = 0;
                pushTicks = 0;
                return;
            }
            if (pushDirection == 0 && boss.ai[1] < 525 && boss.velocity.Y > 0f
                && Math.Abs(Player.Center.X - boss.Center.X) < 150f
                && boss.Bottom.Y + boss.velocity.Y * 2f >= Player.Top.Y - 64f
                && boss.Top.Y <= Player.Bottom.Y + 32f)
            {
                pushDirection = Player.Center.X < ArenaData.ArenaCenter.X ? -1 : Player.Center.X > ArenaData.ArenaCenter.X ? 1 : -Player.direction;
                foreach (Projectile hook in Main.ActiveProjectiles)
                {
                    if (hook.owner == Player.whoAmI && hook.aiStyle == ProjAIStyleID.Hook)
                        hook.Kill();
                }
                pushTicks = 12;
                Player.velocity.Y = -3.5f;
                Player.jump = 0;
            }
            if (pushTicks > 0)
            {
                pushTicks--;
                Player.velocity.X = pushDirection * 9f;
                Player.fallStart = (int)(Player.position.Y / 16f);
            }
        }

        public override void PostUpdate()
        {
            if (Main.dedServ || Player.whoAmI != Main.myPlayer)
                return;
            if (!TryIntro(out NPC boss))
            {
                ReleaseCamera();
                return;
            }
            ScreenPlayer screen = Player.GetModPlayer<ScreenPlayer>();
            if (!ownsCamera)
            {
                ownsCamera = true;
                savedZoom = Main.GameZoomTarget;
                initialFocus = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                screen.interpolant = 0f;
            }
            float time = boss.ai[1];
            Vector2 left = GateFocus(-1);
            Vector2 right = GateFocus(1);
            Vector2 gateway = CrystalTumbler.GatewayFocus;
            Vector2 reveal = ((CrystalTumbler)boss.ModNPC).EntranceStart + new Vector2(0f, 100f);
            Vector2 landingFocus = new(ArenaData.ArenaCenter.X, ArenaData.FloorY - 110f);
            float gateZoom = Math.Min(2f, savedZoom * 1.28f);
            float closeZoom = Math.Min(2f, savedZoom * 1.6f);
            Vector2 focus;
            float zoom;
            if (time < 150)
            {
                float pan = Smooth(time / 60f);
                focus = Vector2.Lerp(initialFocus, left, pan);
                zoom = MathHelper.Lerp(savedZoom, gateZoom, pan);
            }
            else if (time < 300)
            {
                focus = Vector2.Lerp(left, right, Smooth((time - 150f) / 60f));
                zoom = gateZoom;
            }
            else if (time < 430)
            {
                focus = Vector2.Lerp(right, gateway, Smooth((time - 300f) / 65f));
                zoom = MathHelper.Lerp(gateZoom, closeZoom, Smooth((time - 300f) / 100f));
            }
            else if (time < 455)
            {
                float pan = Smooth((time - 430f) / 25f);
                focus = Vector2.Lerp(gateway, reveal, pan);
                zoom = MathHelper.Lerp(closeZoom, gateZoom, pan);
            }
            else if (time < 525)
            {
                float fall = MathHelper.Clamp((time - 455f) / 70f, 0f, 1f);
                Vector2 following = boss.Center + new Vector2(0f, 100f);
                focus = Vector2.Lerp(following, landingFocus, Smooth(fall));
                zoom = MathHelper.Lerp(gateZoom, savedZoom, Smooth(fall));
            }
            else
            {
                focus = Vector2.Lerp(landingFocus, Player.Center, Smooth((time - 545f) / 55f));
                zoom = savedZoom;
            }
            screen.cutscene = true;
            screen.lerpBackToPlayer = false;
            screen.ScreenGoalPos = focus;
            Main.GameZoomTarget = zoom;
        }

        private static float Smooth(float progress) => MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(progress, 0f, 1f));

        private static Vector2 GateFocus(int side)
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is TumblerArenaGate && Math.Sign(projectile.ai[1]) == side)
                    return projectile.Center + new Vector2(-side * 65f, projectile.ai[2] * 0.5f);
            }
            float x = side < 0 ? ArenaData.OuterArenaBoundaryLeft.X : ArenaData.OuterArenaBoundaryRight.X;
            return new Vector2(x - side * 65f, ArenaData.FloorY - 130f);
        }

        internal void ReleaseCamera()
        {
            if (!ownsCamera)
                return;
            ownsCamera = false;
            Main.GameZoomTarget = savedZoom;
            ScreenPlayer screen = Player.GetModPlayer<ScreenPlayer>();
            screen.cutscene = false;
            screen.lerpBackToPlayer = true;
            pushDirection = 0;
        }
    }

    public class TumblerIntroSystem : ModSystem
    {
        public override void OnWorldUnload()
        {
            if (!Main.dedServ)
                Main.LocalPlayer.GetModPlayer<TumblerIntroPlayer>().ReleaseCamera();
        }
    }
}
