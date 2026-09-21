using System;
using AerovelenceMod.Content.Items.Accessories.SmallAccessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class BouncyApparatus : ModItem
    {
        private const string EnglishTooltip = "Boing bong bing bong boing bouncy bouncy boing boing boing heheheheheheeehe";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BouncyApparatusPlayer>().Equipped = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShinyRedBalloon)
                .AddIngredient(ItemID.PortableStool)
                .AddIngredient(ModContent.ItemType<SpikesInABottle>())
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class BouncyApparatusPlayer : ModPlayer
    {
        public bool Equipped;
        private bool wasPressingUp;
        private int bounceCooldown;
        public override void ResetEffects()
        {
            Equipped = false;
            if (bounceCooldown > 0)
                bounceCooldown--;
        }

        public override void UpdateDead()
        {
            bounceCooldown = 0;
            wasPressingUp = false;
        }

        public override void PostUpdate()
        {
            bool pressingUp = Player.controlUp;
            if (!Equipped)
            {
                wasPressingUp = pressingUp;
                return;
            }
            if (Player.whoAmI == Main.myPlayer && pressingUp && !wasPressingUp && !Player.mount.Active && TryGetStandableSurface(out float groundY))
                SummonPad(groundY);
            wasPressingUp = pressingUp;
            if (bounceCooldown > 0 || Player.mount.Active || Player.grappling[0] != -1 || Player.velocity.Y < 0f)
                return;
            TryBounceOnPad();
        }

        private void SummonPad(float groundY)
        {
            int padType = ModContent.ProjectileType<BouncyApparatusBouncepad>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Player.whoAmI && projectile.type == padType) projectile.Kill();
            }
            Vector2 center = new((float)Math.Floor(Player.Bottom.X / 2f) * 2f, groundY - 8f);
            int index = Projectile.NewProjectile(Player.GetSource_FromThis(), center, Vector2.Zero, padType, 0, 0f, Player.whoAmI);
            if (index >= 0 && index < Main.maxProjectiles)
            {
                Projectile pad = Main.projectile[index];
                Player.Bottom = new Vector2(center.X, groundY - 14f);
                Player.velocity.Y = 0f;
                pad.netUpdate = true;
                Bounce(pad);
            }
        }

        private bool TryGetStandableSurface(out float groundY)
        {
            groundY = Player.Bottom.Y;
            if (Math.Abs(Player.velocity.Y) > 0.01f || Player.sliding || Player.mount.Active || Player.grappling[0] != -1) return false;
            float[] samples = [Player.Center.X, Player.Hitbox.Left + 5f, Player.Hitbox.Right - 5f];
            float bestDifference = float.MaxValue;
            bool found = false;
            for (int s = 0; s < samples.Length; s++)
                for (int yOffset = 0; yOffset <= 5; yOffset++)
                    if (TryGetStandableSurfaceAt(samples[s], Player.Bottom.Y + yOffset, out float surfaceY))
                    {
                        float difference = Math.Abs(Player.Bottom.Y - surfaceY);
                        if (difference <= 5f && difference < bestDifference) { bestDifference = difference; groundY = surfaceY; found = true; }
                    }
            return found;
        }

        private static bool TryGetStandableSurfaceAt(float worldX, float worldY, out float surfaceY)
        {
            surfaceY = 0f;
            Point point = new Vector2(worldX, worldY).ToTileCoordinates();
            if (!WorldGen.InWorld(point.X, point.Y, 2)) return false;
            Tile tile = Framing.GetTileSafely(point.X, point.Y);
            if (!tile.HasUnactuatedTile) return false;
            int type = tile.TileType;
            float tileTop = point.Y * 16f;
            if (Main.tileSolidTop[type]) { surfaceY = tileTop; return true; }
            if (!Main.tileSolid[type] || tile.IsHalfBlock || tile.Slope != SlopeType.Solid || Main.tileCut[type]) return false;
            surfaceY = tileTop;
            return true;
        }

        private void TryBounceOnPad()
        {
            int padType = ModContent.ProjectileType<BouncyApparatusBouncepad>();
            float oldBottom = Player.oldPosition.Y + Player.height;
            float currentBottom = Player.Bottom.Y;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.owner != Player.whoAmI || projectile.type != padType)
                    continue;
                float surfaceY = projectile.Center.Y - 6f;
                float halfWidth = 18f;
                bool horizontal = Player.Hitbox.Right > projectile.Center.X - halfWidth && Player.Hitbox.Left < projectile.Center.X + halfWidth;
                bool crossed = oldBottom <= surfaceY + 5f && currentBottom >= surfaceY - 5f;
                bool resting = Math.Abs(currentBottom - surfaceY) <= 7f;
                if (!horizontal || (!crossed && !resting))
                    continue;
                Bounce(projectile);
                return;
            }
        }

        private void Bounce(Projectile pad)
        {
            float impact = Math.Max(0f, Player.velocity.Y);
            Player.velocity.Y = -MathHelper.Clamp(10.5f + impact * 0.45f, 10.5f, 14f);
            Player.fallStart = (int)(Player.position.Y / 16f);
            Player.jump = 0;
            bounceCooldown = 14;
            if (pad.ModProjectile is BouncyApparatusBouncepad bouncepad)
                bouncepad.TriggerBounce();
            if (Player.whoAmI == Main.myPlayer)
                SpawnCaltrops(pad);
            Vector2 burstPosition = pad.Center - Vector2.UnitY * 12f;
            SpikesInABottleVFX.Burst(burstPosition, 10, 3f);
            SpikesInABottleVFX.Smoke(burstPosition + Vector2.UnitY * 2f, -Vector2.UnitY * 0.7f, 84f, SpikesInABottleVFX.Aqua);
            SoundEngine.PlaySound(SoundID.DoubleJump with { Volume = 0.45f, Pitch = 0.18f, PitchVariance = 0.08f }, Player.Center);
        }

        private void SpawnCaltrops(Projectile pad)
        {
            int count = 6;
            int active = Player.ownedProjectileCounts[ModContent.ProjectileType<BottleCaltrop>()];
            count = Math.Min(count, Math.Max(0, 24 - active));
            for (int i = 0; i < count; i++)
            {
                float spawnX = Main.rand.NextFloat(-15f, 15f);
                Vector2 origin = new(pad.Center.X + spawnX, pad.Center.Y - 6f);
                float outward = spawnX / 15f;
                float x = outward * Main.rand.NextFloat(1.5f, 2.3f) + Main.rand.NextFloat(-0.75f, 0.75f) + Player.velocity.X * 0.08f;
                float y = Player.velocity.Y * Main.rand.NextFloat(0.62f, 0.76f) - Main.rand.NextFloat(0.15f, 0.7f);
                Projectile.NewProjectile(Player.GetSource_FromThis(), origin, new Vector2(x, y), ModContent.ProjectileType<BottleCaltrop>(), 12, 2f, Player.whoAmI);
            }
        }
    }

    public class BouncyApparatusBouncepad : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Accessories/SmallAccessories/BouncyApparatusBouncepad";

        private float SpawnAge => Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 40;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead) { Projectile.Kill(); return; }
            Projectile.timeLeft = 2;
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[0]++;
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[1] == 0f)
            {
                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BouncyApparatusExtraStilts>(), 0, 0f, Projectile.owner, Projectile.identity);
                if (index >= 0 && index < Main.maxProjectiles) Main.projectile[index].netUpdate = true;
                Projectile.localAI[1] = 1f;
            }
            if (Projectile.ai[0] > 0f) { Projectile.ai[0]++; if (Projectile.ai[0] > 52f) Projectile.ai[0] = 0f; }
            Lighting.AddLight(Projectile.Center - Vector2.UnitY * 8f, new Vector3(0.18f, 0.55f, 0.9f) * 0.3f * Opacity());
        }

        public void TriggerBounce()
        {
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }

        public override void DrawBehind(int index, System.Collections.Generic.List<int> behindNPCsAndTiles, System.Collections.Generic.List<int> behindNPCs, System.Collections.Generic.List<int> behindProjectiles, System.Collections.Generic.List<int> overPlayers, System.Collections.Generic.List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        private float Opacity() => MathHelper.Clamp(SpawnAge / 5f, 0f, 1f);

        private float BandDisplacement()
        {
            if (Projectile.ai[0] <= 0f) return 1.5f;
            float age = Projectile.ai[0] - 1f;
            if (age < 3f) return MathHelper.Lerp(1.5f, 14f, age / 3f);
            float settle = age - 3f;
            float damping = (float)Math.Exp(-settle * 0.06f);
            return 14f * damping * (float)Math.Cos(settle * 0.56f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D body = TextureAssets.Projectile[Type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            Vector2 topLeft = Projectile.Center - body.Size() * 0.5f;
            float opacity = Opacity();
            DrawTileOccluded(body, topLeft, lightColor * opacity);
            DrawTileOccluded(glowmask, topLeft, Color.White * opacity);
            DrawStretchyBand(body, opacity);
            return false;
        }

        private static void DrawTileOccluded(Texture2D texture, Vector2 topLeft, Color color)
            => DrawTileOccluded(texture, new Rectangle(0, 0, texture.Width, texture.Height), topLeft, color);

        private static void DrawTileOccluded(Texture2D texture, Rectangle sourceRect, Vector2 worldTopLeft, Color color)
        {
            for (int sy = 0; sy < sourceRect.Height; sy++)
            {
                int runStart = -1;
                for (int sx = 0; sx <= sourceRect.Width; sx++)
                {
                    bool visible = sx < sourceRect.Width && !SolidTileAt(worldTopLeft + new Vector2(sx + 0.5f, sy + 0.5f));
                    if (visible && runStart < 0) runStart = sx;
                    if ((!visible || sx == sourceRect.Width) && runStart >= 0)
                    {
                        int width = sx - runStart;
                        Rectangle source = new(sourceRect.X + runStart, sourceRect.Y + sy, width, 1);
                        Main.EntitySpriteDraw(texture, worldTopLeft + new Vector2(runStart, sy) - Main.screenPosition, source, color, 0f, Vector2.Zero, 1f, SpriteEffects.None);
                        runStart = -1;
                    }
                }
            }
        }

        private static bool SolidTileAt(Vector2 worldPosition)
        {
            Point tilePoint = worldPosition.ToTileCoordinates();
            if (!WorldGen.InWorld(tilePoint.X, tilePoint.Y, 1)) return false;
            Tile tile = Framing.GetTileSafely(tilePoint.X, tilePoint.Y);
            if (!tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || Main.tileCut[tile.TileType] || Main.tileAxe[tile.TileType]) return false;
            return Collision.IsWorldPointSolid(worldPosition);
        }

        private void DrawStretchyBand(Texture2D body, float opacity)
        {
            Vector2 topLeft = Projectile.Center - body.Size() * 0.5f;
            float bandY = Projectile.Center.Y - 6f;
            Vector2 left = new(topLeft.X + 8f, bandY);
            Vector2 right = new(topLeft.X + body.Width - 6f, bandY);
            float displacement = BandDisplacement();
            float bounceAge = Projectile.ai[0] > 0f ? Projectile.ai[0] - 1f : 0f;
            float rebound = Projectile.ai[0] > 0f ? (float)Math.Exp(-bounceAge * 0.07f) : 0f;
            Vector2 previous = left;
            for (int i = 1; i <= 24; i++)
            {
                float t = i / 24f;
                float arch = (float)Math.Sin(t * MathHelper.Pi);
                float ripple = (float)Math.Sin(t * MathHelper.TwoPi) * rebound * 1.35f;
                Vector2 point = Vector2.Lerp(left, right, t) + Vector2.UnitY * (displacement * (float)Math.Pow(arch, 0.82f) + ripple * arch);
                DrawSegment(previous, point, new Color(55, 120, 220) * (opacity * 0.55f), 4f);
                DrawSegment(previous, point, new Color(160, 225, 255) * (opacity * 0.95f), 1.6f);
                previous = point;
            }
        }

        private static void DrawSegment(Vector2 startWorld, Vector2 endWorld, Color color, float width)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 start = startWorld - Main.screenPosition;
            Vector2 edge = endWorld - startWorld;
            float length = edge.Length();
            if (length <= 0.001f) return;
            Main.spriteBatch.Draw(pixel, start, new Rectangle(0, 0, 1, 1), color, edge.ToRotation(), new Vector2(0f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
        }
    }

    public class BouncyApparatusExtraStilts : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Accessories/SmallAccessories/BouncyApparatusExtra";
        private static Texture2D cachedBody;
        private static int cachedLeftPostBottom;
        private static int cachedRightPostBottom;

        public override void SetStaticDefaults() => ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 40;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.hide = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile parent = FindParent();
            if (parent == null) { Projectile.Kill(); return; }
            Projectile.Center = parent.Center;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
        }

        private Projectile FindParent()
        {
            int identity = (int)Projectile.ai[0];
            int type = ModContent.ProjectileType<BouncyApparatusBouncepad>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.type == type && projectile.identity == identity) return projectile;
            }
            return null;
        }

        public override void DrawBehind(int index, System.Collections.Generic.List<int> behindNPCsAndTiles, System.Collections.Generic.List<int> behindNPCs, System.Collections.Generic.List<int> behindProjectiles, System.Collections.Generic.List<int> overPlayers, System.Collections.Generic.List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile parent = FindParent();
            if (parent == null) return false;
            Texture2D body = TextureAssets.Projectile[ModContent.ProjectileType<BouncyApparatusBouncepad>()].Value;
            Texture2D extra = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 topLeft = parent.Center - body.Size() * 0.5f;
            float opacity = MathHelper.Clamp(parent.localAI[0] / 5f, 0f, 1f);
            CachePostBottoms(body);
            int split = extra.Width / 2;
            Rectangle leftSide = new(0, 0, split, extra.Height);
            Rectangle rightSide = new(split, 0, extra.Width - split, extra.Height);
            DrawStiltColumn(extra, leftSide, new Vector2(topLeft.X, topLeft.Y + cachedLeftPostBottom - 7f), lightColor * opacity);
            DrawStiltColumn(extra, rightSide, new Vector2(topLeft.X + body.Width - rightSide.Width, topLeft.Y + cachedRightPostBottom - 7f), lightColor * opacity);
            return false;
        }

        private static void CachePostBottoms(Texture2D body)
        {
            if (ReferenceEquals(cachedBody, body)) return;
            cachedBody = body;
            Color[] data = new Color[body.Width * body.Height];
            body.GetData(data);
            cachedLeftPostBottom = FindPostBottom(data, body.Width, body.Height, 3, Math.Min(body.Width, 14));
            cachedRightPostBottom = FindPostBottom(data, body.Width, body.Height, Math.Max(0, body.Width - 12), body.Width);
        }

        private static int FindPostBottom(Color[] data, int width, int height, int minX, int maxX)
        {
            for (int y = height - 1; y >= 0; y--)
                for (int x = minX; x < maxX; x++)
                    if (data[y * width + x].A > 16) return y + 1;
            return height;
        }

        private static void DrawStiltColumn(Texture2D texture, Rectangle source, Vector2 worldStart, Color color)
        {
            SupportInfo support = FindSupportSurface(worldStart.X + source.Width * 0.5f, worldStart.Y);
            if (support.IsTopSurfaceOnly) return;
            float step = Math.Max(1f, source.Height - 2f);
            float drawBottom = support.SurfaceY + 6f;
            int pieces = Math.Clamp((int)Math.Ceiling((drawBottom - worldStart.Y) / step), 1, 10);
            for (int i = 0; i < pieces; i++)
                Main.EntitySpriteDraw(texture, new Vector2(worldStart.X, worldStart.Y + i * step) - Main.screenPosition, source, color, 0f, Vector2.Zero, 1f, SpriteEffects.None);
        }

        private readonly struct SupportInfo
        {
            public readonly float SurfaceY;
            public readonly bool IsTopSurfaceOnly;
            public SupportInfo(float surfaceY, bool isTopSurfaceOnly) { SurfaceY = surfaceY; IsTopSurfaceOnly = isTopSurfaceOnly; }
        }

        private static SupportInfo FindSupportSurface(float worldX, float startY)
        {
            for (int y = (int)Math.Floor(startY); y <= startY + 96f; y++)
            {
                Vector2 point = new(worldX, y + 0.5f);
                Point tilePoint = point.ToTileCoordinates();
                if (!WorldGen.InWorld(tilePoint.X, tilePoint.Y, 1)) continue;
                Tile tile = Framing.GetTileSafely(tilePoint.X, tilePoint.Y);
                if (!tile.HasUnactuatedTile) continue;
                int type = tile.TileType;
                float tileTop = tilePoint.Y * 16f;
                if (Main.tileSolidTop[type] && y >= tileTop - 1f) return new SupportInfo(tileTop, true);
                if (Main.tileSolid[type] && !Main.tileCut[type] && !Main.tileAxe[type] && Collision.IsWorldPointSolid(point)) return new SupportInfo(y, false);
            }
            return new SupportInfo(startY + 24f, false);
        }
    }

}