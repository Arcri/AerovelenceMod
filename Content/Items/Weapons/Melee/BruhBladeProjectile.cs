using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class BruhBladeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 64;
            projectile.height = 64;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 10000f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 300f;
        }
        int t;
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public static void /*credits to eldrazi*/ SpawnDustFromTexture(Vector2 position, int dustType, /*credits to eldrazi*/ float size, string imagePath, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(-rot, rot);
                Texture2D texture = ModContent.GetTexture(imagePath);
                int[] pixelData = new /*credits to eldrazi*/int[texture.Width * texture.Height];

                texture.GetData(pixelData);

                for (int i = 0; i < texture.Width; i += 2)
                {
                    for (int j = 0; j < texture.Height; j += 2)
                    {
                        if /*credits to eldrazi*/(pixelData[j * texture.Width + i] != 0)
                        {
                            Vector2 dustPosition = /*credits to eldrazi*/new Vector2(i - texture.Width / 2, j - texture.Height / 2) * size;
                            Dust.NewDustPerfect(position, dustType, /*credits to eldrazi*/ dustPosition.RotatedBy(rotation)).noGravity = noGravity;
                        }
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            t++;
            target.life = 0;
            if (t % 50 == 0)
            {
                SpawnDustFromTexture(projectile.position, DustID.Fire, 0.5f, "AerovelenceMod/Content/Items/Weapons/Ranged/CockSprite");
            }
            Rectangle r = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            CombatText.NewText(r, new Color(89, 32, 255), $"Ech");
        }

    }
}