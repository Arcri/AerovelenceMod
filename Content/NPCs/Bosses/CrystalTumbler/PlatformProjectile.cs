using AerovelenceMod.Content.Tiles.CrystalCaverns.Building;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using AerovelenceMod.Content.Tiles.Citadel;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.DataStructures;
using static AerovelenceMod.Content.Items.BossSummons.LargeGeode;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class PlatformProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.position.X = ArenaBoundaries.leftBoundary.X + Projectile.ai[0] * Projectile.width;
            Projectile.position.Y = ArenaBoundaries.leftBoundary.Y - 200f;
            Projectile.localAI[0] = Projectile.position.X;
            Projectile.localAI[1] = Projectile.position.Y;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 120f)
            {
                Projectile.position.Y = Projectile.localAI[1];
                Projectile.ai[0]++;
            }
            else
            {
                Projectile.position.X = Projectile.localAI[0];
                Projectile foundMiddleProjectile = null;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner && proj.ai[1] == 1)
                    {
                        foundMiddleProjectile = proj;
                        break;
                    }
                }
                if (foundMiddleProjectile != null)
                {
                    Projectile.position.Y = foundMiddleProjectile.position.Y + 5f;
                }
                else
                {
                    Projectile.position.Y += 10f;
                }
            }
            Projectile.frame = (int)Projectile.ai[1];
            if (Projectile.position.Y > Main.player[Projectile.owner].Center.Y)
            {
                if (CollidesWithCavernTile(Projectile.position))
                {
                    DestroyAllPlatformProjectiles();
                }
            }
        }

        private static bool CollidesWithCavernTile(Vector2 position)
        {
            Tile tile = Main.tile[(int)position.X / 16, (int)position.Y / 16];
            return tile.HasTile && (tile.TileType == ModContent.TileType<CavernCrystalTile>() ||
                                    tile.TileType == ModContent.TileType<SmoothCavernStoneTile>() ||
                                    tile.TileType == ModContent.TileType<CitadelBrickTile>() ||
                                    tile.TileType == ModContent.TileType<ChargedStoneTile>());
        }

        private void DestroyAllPlatformProjectiles()
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    proj.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 120f)
            {
                lightColor = Color.Blue * 0.5f;
            }

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/CrystalTumbler/PlatformProjectile").Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}