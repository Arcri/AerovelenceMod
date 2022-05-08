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
            Projectile.extraUpdates = 0;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 30;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 10000f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 300f;
        }
        int t;
        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public static void /*credits to eldrazi*/ SpawnDustFromTexture(Vector2 position, int DustType, /*credits to eldrazi*/ float size, string imagePath, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(-rot, rot);
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(imagePath);
                int[] pixelData = new /*credits to eldrazi*/int[texture.Width * texture.Height];

                texture.GetData(pixelData);

                for (int i = 0; i < texture.Width; i += 2)
                {
                    for (int j = 0; j < texture.Height; j += 2)
                    {
                        if /*credits to eldrazi*/(pixelData[j * texture.Width + i] != 0)
                        {
                            Vector2 dustPosition = /*credits to eldrazi*/new Vector2(i - texture.Width / 2, j - texture.Height / 2) * size;
                            Dust.NewDustPerfect(position, DustType, /*credits to eldrazi*/ dustPosition.RotatedBy(rotation)).noGravity = noGravity;
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
                SpawnDustFromTexture(Projectile.position, DustID.Torch, 0.5f, "AerovelenceMod/Content/Items/Weapons/Ranged/CockSprite");
            }
            Rectangle r = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
            CombatText.NewText(r, new Color(89, 32, 255), $"Ech");
        }

    }
}