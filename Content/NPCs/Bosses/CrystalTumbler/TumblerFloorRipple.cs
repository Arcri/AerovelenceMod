using System;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerFloorRipple : ModProjectile
    {
        internal static TumblerFloorRipple Active;
        private FloorTile[] tiles;
        private int leftTile;
        private readonly record struct FloorTile(ushort Type, short X, short Y, ushort FillType, short FillX, short FillY, bool Solid);
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2200;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.timeLeft = 245;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        internal void Retire()
        {
            if (Projectile.ai[2] > 0f)
                return;
            Projectile.ai[2] = 1f;
            Projectile.netUpdate = true;
        }
        public override void AI()
        {
            if (!ArenaData.Valid)
            {
                Projectile.Kill();
                return;
            }
            Active = this;
            Projectile.ai[1]++;
            int owner = (int)Projectile.ai[0];
            if (owner < 0 || owner >= Main.maxNPCs || !Main.npc[owner].active || Main.npc[owner].ModNPC is not CrystalTumbler)
                Retire();
            if (Projectile.ai[2] > 0f && ++Projectile.ai[2] >= 36f)
            {
                Projectile.Kill();
                return;
            }
            if (tiles == null)
                CaptureFloor();
            if (!Main.dedServ && Projectile.ai[1] % 4f == 0f)
            {
                for (int side = -1; side <= 1; side += 2)
                {
                    float x = Projectile.Center.X + side * (Projectile.ai[1] * 7f - 100f);
                    float height = Height(x);
                    if (height > 5f)
                    {
                        Vector2 position = new(x, ArenaData.FloorY - height);
                        TumblerVFX.SpawnSpark(position, new Vector2(side, -1.5f), TumblerVFX.PhaseColor(owner >= 0 && owner < Main.maxNPCs ? Main.npc[owner].ai[2] : 0f), 0.2f);
                        Dust.NewDustPerfect(position, DustID.Stone, new Vector2(side * 1.5f, -1f), 0, default, 0.8f);
                    }
                }
            }
        }
        private void CaptureFloor()
        {
            leftTile = (int)(ArenaData.OuterArenaBoundaryLeft.X / 16f) + 1;
            int rightTile = (int)(ArenaData.OuterArenaBoundaryRight.X / 16f) - 1;
            tiles = new FloorTile[Math.Max(0, rightTile - leftTile + 1)];
            int y = (int)(ArenaData.FloorY / 16f);
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile tile = Framing.GetTileSafely(leftTile + i, y);
                Tile fill = Framing.GetTileSafely(leftTile + i, y + 1);
                if (!fill.HasTile)
                    fill = tile;
                tiles[i] = new FloorTile(tile.TileType, tile.TileFrameX, tile.TileFrameY, fill.TileType, fill.TileFrameX, fill.TileFrameY, tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]);
            }
        }
        internal float Height(float x, float ageOffset = 0f)
        {
            if (tiles == null || !Projectile.active || !ArenaData.Valid)
                return 0f;
            int index = (int)MathF.Floor(x / 16f) - leftTile;
            if (index < 0 || index >= tiles.Length || !tiles[index].Solid)
                return 0f;
            float age = Projectile.ai[1] + ageOffset;
            float distance = Math.Abs(x - Projectile.Center.X);
            float height = 0f;
            for (int wave = 0; wave < 2; wave++)
            {
                float time = age - wave * 64f;
                if (time < 0f)
                    continue;
                float q = (distance - (time * 7f - 100f)) / 170f;
                if (Math.Abs(q) >= 1f)
                    continue;
                float crest = MathF.Cos(q * MathHelper.PiOver2);
                height = Math.Max(height, crest * crest * 64f * MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(time / 18f, 0f, 1f)));
            }
            float edge = Math.Min(x - ArenaData.OuterArenaBoundaryLeft.X - 16f, ArenaData.OuterArenaBoundaryRight.X - 16f - x);
            float fade = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((245f - age) / 40f, 0f, 1f));
            if (Projectile.ai[2] > 0f)
                fade *= MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((36f - Projectile.ai[2] - ageOffset) / 35f, 0f, 1f));
            return MathF.Round(height * fade * MathHelper.Clamp(edge / 70f, 0f, 1f));
        }
        internal static bool Surface(Entity entity, out float surface, float ageOffset = 0f)
        {
            surface = ArenaData.FloorY;
            TumblerFloorRipple ripple = Active;
            if (ripple == null || !ripple.Projectile.active || ripple.Projectile.ModProjectile != ripple || !ArenaData.Valid)
                return false;
            float height = 0f;
            for (float x = entity.position.X + 2f; x <= entity.position.X + entity.width - 2f; x += 4f)
                height = Math.Max(height, ripple.Height(x, ageOffset));
            surface -= height;
            return height > 0f;
        }
        public override void OnKill(int timeLeft)
        {
            if (Active == this)
                Active = null;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            if (tiles == null)
                return false;
            int floorTile = (int)(ArenaData.FloorY / 16f);
            for (int i = 0; i < tiles.Length; i++)
            {
                FloorTile tile = tiles[i];
                if (!tile.Solid)
                    continue;
                Main.instance.LoadTiles(tile.Type);
                Main.instance.LoadTiles(tile.FillType);
                Texture2D texture = TextureAssets.Tile[tile.Type].Value;
                Texture2D fill = TextureAssets.Tile[tile.FillType].Value;
                Color color = Lighting.GetColor(leftTile + i, floorTile);
                for (int strip = 0; strip < 16; strip += 2)
                {
                    float x = (leftTile + i) * 16f + strip;
                    int height = (int)Height(x + 1f);
                    if (height <= 0)
                        continue;
                    Vector2 top = new Vector2(x, ArenaData.FloorY - height) - Main.screenPosition;
                    Main.spriteBatch.Draw(texture, top, new Rectangle(tile.X + strip, tile.Y, 2, 16), color);
                    for (int y = 16; y < height; y += 16)
                        Main.spriteBatch.Draw(fill, top + new Vector2(0f, y), new Rectangle(tile.FillX + strip, tile.FillY, 2, Math.Min(16, height - y)), color);
                }
            }
            return false;
        }
    }

    internal sealed class TumblerRippleSupport
    {
        private bool standing;
        private float lastSurface;
        private float previousBottom;
        internal void Begin(Entity entity, ref Vector2 velocity, bool enabled)
        {
            previousBottom = entity.Bottom.Y;
            if (!enabled || velocity.Y < -0.1f || !TumblerFloorRipple.Surface(entity, out float surface))
            {
                standing = false;
                return;
            }
            if (!standing || Math.Abs(previousBottom - lastSurface) > 10f)
                return;
            float delta = surface - previousBottom;
            Vector2 allowed = Collision.TileCollision(entity.position, new Vector2(0f, delta), entity.width, entity.height, true, true);
            entity.position += allowed;
            previousBottom = entity.Bottom.Y;
            velocity.Y = 0f;
        }
        internal bool Resolve(Entity entity, ref Vector2 velocity, bool enabled)
        {
            if (!enabled || velocity.Y < -0.1f || !TumblerFloorRipple.Surface(entity, out float surface))
            {
                standing = false;
                return false;
            }
            TumblerFloorRipple.Surface(entity, out float oldSurface, -1f);
            bool carried = standing && Math.Abs(entity.Bottom.Y - surface) < 14f + Math.Abs(velocity.X) * 0.65f;
            bool landed = previousBottom <= Math.Max(surface, oldSurface) + 8f && entity.Bottom.Y >= surface;
            if (!carried && !landed)
            {
                standing = false;
                return false;
            }
            Vector2 destination = new(entity.position.X, surface - entity.height);
            if (Collision.SolidCollision(destination, entity.width, entity.height))
            {
                standing = false;
                return false;
            }
            entity.position = destination;
            velocity.Y = 0f;
            lastSurface = surface;
            standing = true;
            return true;
        }
    }

    public class TumblerRipplePlayer : ModPlayer
    {
        private readonly TumblerRippleSupport support = new();
        private bool Enabled => Player.active && !Player.dead && Player.gravDir > 0f && !Player.justJumped && !Player.pulley && !Player.GoingDownWithGrapple && (Main.netMode != NetmodeID.MultiplayerClient || Player.whoAmI == Main.myPlayer);
        public override void PreUpdate() => support.Begin(Player, ref Player.velocity, Enabled);
        internal void Resolve()
        {
            if (!support.Resolve(Player, ref Player.velocity, Enabled))
                return;
            Player.jump = 0;
            Player.fallStart = (int)(Player.position.Y / 16f);
            Player.gfxOffY = 0f;
        }
    }

    public class TumblerRippleNPC : GlobalNPC
    {
        private readonly TumblerRippleSupport support = new();
        public override bool InstancePerEntity => true;
        private static bool Enabled(NPC npc) => !npc.noGravity && !npc.noTileCollide && npc.ModNPC is not CrystalTumbler;
        public override bool PreAI(NPC npc)
        {
            support.Begin(npc, ref npc.velocity, Enabled(npc));
            return true;
        }
        internal void Resolve(NPC npc)
        {
            if (support.Resolve(npc, ref npc.velocity, Enabled(npc)))
                npc.collideY = true;
        }
    }

    public class TumblerRippleMinion : GlobalProjectile
    {
        private readonly TumblerRippleSupport support = new();
        public override bool InstancePerEntity => true;
        private static bool Enabled(Projectile projectile) => (projectile.minion || projectile.sentry) && projectile.tileCollide;
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.minion || projectile.sentry)
                support.Begin(projectile, ref projectile.velocity, Enabled(projectile));
            return true;
        }
        internal void Resolve(Projectile projectile) => support.Resolve(projectile, ref projectile.velocity, Enabled(projectile));
    }

    public class TumblerRippleCollision : ModSystem
    {
        public override void OnWorldUnload() => TumblerFloorRipple.Active = null;
        public override void PostUpdateNPCs()
        {
            if (TumblerFloorRipple.Active == null)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
                npc.GetGlobalNPC<TumblerRippleNPC>().Resolve(npc);
        }
        public override void PostUpdateProjectiles()
        {
            if (TumblerFloorRipple.Active == null)
                return;
            foreach (Projectile projectile in Main.ActiveProjectiles)
                if (projectile.minion || projectile.sentry)
                    projectile.GetGlobalProjectile<TumblerRippleMinion>().Resolve(projectile);
        }
    }
}
