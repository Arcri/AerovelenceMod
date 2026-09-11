using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerCascadeRail : ModProjectile
    {
        private readonly TumblerConjuredRail rail = new();
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1800;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.timeLeft = 450;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        internal static Vector2 Point(Vector2 start, int direction, float progress)
        {
            float t = MathHelper.Clamp(progress, 0f, 1f);
            float width = ArenaData.InnerArenaBoundaryRight.X - ArenaData.InnerArenaBoundaryLeft.X - 180f;
            float wave = MathF.Sin(t * MathHelper.TwoPi);
            float height = Math.Min(t < 0.5f ? 180f : 310f, start.Y - ArenaData.WorldBounds.Top - 100f);
            return start + new Vector2(direction * width * t, -height * wave * wave);
        }
        public override void AI()
        {
            int index = (int)Projectile.ai[0];
            int direction = Projectile.ai[1] >= 0f ? 1 : -1;
            if (index < 0 || index >= Main.maxNPCs || !Main.npc[index].active || Main.npc[index].ModNPC is not CrystalTumbler boss)
            {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 90);
                rail.Update(t => Point(Projectile.Center, direction, t), Projectile.ai[2], true);
                return;
            }
            if (boss.CascadeFinished)
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 90);
            else
            {
                Projectile.timeLeft = 450;
                Projectile.ai[2] = MathHelper.Clamp(boss.CascadeProgress, 0f, 1f);
            }
            rail.Update(t => Point(Projectile.Center, direction, t), Projectile.ai[2], boss.CascadeFinished);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int direction = Projectile.ai[1] >= 0f ? 1 : -1;
            rail.Draw(t => Point(Projectile.Center, direction, t), direction, TumblerProjectileRetirement.VisualOpacity(Projectile), true);
            return false;
        }
    }
}
