using System;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Mounts
{
    public class TumblingMountPlayer : ModPlayer
    {
        internal bool Mounted => Player.mount.Active && Player.mount.Type == ModContent.MountType<TumblingMount>();
        internal Vector2 BallCenter => Player.Bottom - new Vector2(0f, TumblingRampMotion.Radius);
        internal bool Riding { get; private set; }
        internal float Angle { get; private set; }
        internal float Speed { get; private set; }
        internal int Direction { get; private set; } = 1;
        internal float Rotation { get; private set; }
        internal float Charge { get; private set; }
        private int rampAge;
        internal int RampAge => rampAge;
        private int launchCharge;
        private bool upReleased = true;
        private Vector2 movementStart;
        private Vector2 intendedVelocity;
        private Vector2 lastCenter;
        private bool hadPosition;

        internal void ResetRide()
        {
            Riding = false;
            Angle = Speed = Charge = 0f;
            rampAge = launchCharge = 0;
            upReleased = !Player.controlUp;
            hadPosition = false;
        }

        internal void ReceiveRide(bool riding, float angle, int direction, float charge)
        {
            if (Player.whoAmI == Main.myPlayer)
                return;
            Riding = riding;
            Angle = angle;
            Direction = direction;
            Charge = charge;
        }

        internal void ReceiveRotation(float rotation)
        {
            if (Player.whoAmI != Main.myPlayer)
                Rotation = MathHelper.WrapAngle(Rotation + MathHelper.WrapAngle(rotation - Rotation) * 0.25f);
        }

        public override void PreUpdateMovement()
        {
            if (!Mounted || Player.dead || Player.whoAmI != Main.myPlayer)
                return;
            Player.velocity.X = MathHelper.Clamp(Player.velocity.X, -TumblingRampMotion.MaximumSpeed, TumblingRampMotion.MaximumSpeed);
            if (!Player.controlUp)
                upReleased = true;
            bool canRide = !Player.CCed && !Player.pulley && !Player.tongued && !Player.shimmering && Player.grappling[0] == -1 && Player.gravDir == 1f;
            if (hadPosition && Vector2.DistanceSquared(Player.Center, lastCenter) > 160f * 160f)
            {
                ResetRide();
                upReleased = false;
                return;
            }
            if (Riding && (!Player.controlUp || !canRide))
                EndRamp(canRide);
            if (!Riding && canRide && Player.controlUp && upReleased && HasRampFooting())
            {
                Riding = true;
                upReleased = false;
                rampAge = 0;
                Angle = 0f;
                Direction = Math.Abs(Player.velocity.X) > 1f ? Math.Sign(Player.velocity.X) : Player.direction;
                Speed = MathHelper.Clamp(Math.Abs(Player.velocity.X), TumblingRampMotion.MinimumSpeed, TumblingRampMotion.MaximumSpeed);
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.45f, Pitch = 0.3f }, BallCenter);
            }
            if (!Riding)
                return;
            float angle = Angle;
            float speed = Speed;
            intendedVelocity = TumblingRampMotion.Advance(ref angle, ref speed, Direction, ++rampAge);
            Angle = angle;
            Speed = speed;
            movementStart = Player.position;
            Player.velocity = intendedVelocity;
            Player.jump = 0;
            Player.fallStart = (int)(Player.position.Y / 16f);
        }

        private bool HasRampFooting()
        {
            if (Player.velocity.Y < 0f || Player.velocity.Y > 1f || Player.justJumped || Player.controlJump || Player.controlDown)
                return false;
            Vector2 probe = Collision.TileCollision(Player.position, new Vector2(0f, 2f), Player.width, Player.height);
            if (probe.Y < 0.5f)
                return true;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is TumblerMagneticPlatform platform && platform.CanStand &&
                    Math.Abs(Player.Bottom.Y - platform.SurfaceY) <= 2f &&
                    Player.Right.X > platform.SurfaceStart.X + 3f && Player.Left.X < platform.SurfaceEnd.X - 3f)
                    return true;
            }
            return false;
        }

        private void EndRamp(bool launch)
        {
            Riding = false;
            if (launch)
            {
                Player.velocity = TumblingRampMotion.Launch(Angle, Speed, Direction);
                launchCharge = 60;
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.5f, Pitch = 0.4f }, BallCenter);
            }
            else
                launchCharge = 0;
            if (!Main.dedServ)
                for (int i = 0; i < 8; i++)
                    TumblerVFX.SpawnSpark(BallCenter, Main.rand.NextVector2Circular(3f, 3f), Color.LightCyan, 0.23f);
        }

        public override void PostUpdate()
        {
            if (!Mounted || Player.dead)
            {
                ResetRide();
                return;
            }
            Player.legFrame.Y = 0;
            if (Player.whoAmI != Main.myPlayer)
            {
                float distance = hadPosition ? Vector2.Distance(Player.Center, lastCenter) : 0f;
                if (distance < 40f)
                    Rotation = MathHelper.WrapAngle(Rotation + (Riding ? Direction * distance : Player.velocity.X) / TumblingRampMotion.Radius);
                lastCenter = Player.Center;
                hadPosition = true;
                return;
            }
            if (Riding)
            {
                Vector2 moved = Player.position - movementStart;
                if (Vector2.DistanceSquared(moved, intendedVelocity) > 9f)
                    EndRamp(false);
            }
            if (launchCharge > 0)
                launchCharge--;
            Charge = MathHelper.Lerp(Charge, Riding || launchCharge > 0 ? 1f : 0f, 0.15f);
            float traveled = hadPosition ? Vector2.Distance(Player.Center, lastCenter) : 0f;
            if (traveled < 40f)
                Rotation = MathHelper.WrapAngle(Rotation + (Riding || launchCharge > 0 ? Direction * traveled : Player.velocity.X) / TumblingRampMotion.Radius);
            lastCenter = Player.Center;
            hadPosition = true;
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<TumblingMountTrail>()] == 0)
                Projectile.NewProjectile(Player.GetSource_Misc("TumblingHarness"), BallCenter, Vector2.Zero, ModContent.ProjectileType<TumblingMountTrail>(), 24, 5f, Player.whoAmI);
        }
    }
}
