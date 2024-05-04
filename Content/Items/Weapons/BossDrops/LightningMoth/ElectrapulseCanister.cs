using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AerovelenceMod.Core.Abstracts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Graphics;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.LightningMoth
{
    public class ElectrapulseCanister : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electrapulse Canister");
            // Tooltip.SetDefault("Hitting a tile or enemy releases a large electric explosion that releases lightning");
        }
        public override void SetDefaults()
        {
            //item.UseSound = SoundID.Item1;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ElectrapulseCanisterProj>();
            Item.shootSpeed = 16f;
        }
    }

    public class ElectrapulseCanisterProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            //Main.PlaySound(SoundID.Shatter, projectile.Center);
            //Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore1"), 1f);
            //Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore2"), 1f);
            //Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/CanisterGore3"), 1f);
            for (double i = 0; i < 6.28; i += Main.rand.NextFloat(1f, 2f))
            {
                Projectile lightningproj = Projectile.NewProjectileDirect(null, Projectile.Center, new Vector2((float)Math.Sin(i), (float)Math.Cos(i)) * 2.5f, ModContent.ProjectileType<ElectrapulseCanisterProj2>(), Projectile.damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
            }
            for (double i = 0; i < 6.28; i += 0.1)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }
        }
        public override void AI()
        {
            i++;
            if (i % 4 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
            }

            timer += 0.02f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.Kill();

        float timer = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            float colorFloat = 1f;
            Color ColorValue = Color.White;

            //float a = new (float)VertexStrip.StripHalfWidthFunction();
            /*
            VertexStrip.StripColorFunction colo;
            colo = colorStrip;

            VertexStrip.StripHalfWidthFunction shwf;
            shwf = widthStrip;

            //Prims
            BasicEffect effect = new BasicEffect(Main.graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            effect.View = Main.GameViewMatrix.TransformationMatrix;
            var viewport = Main.instance.GraphicsDevice.Viewport;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
            Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            effect.CurrentTechnique.Passes[0].Apply();

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, colo, widthStrip, includeBacksides: true);
            vertexStrip.DrawTrail();
            */

            //Prims


            var texture = TextureAssets.Projectile[Type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var drawColor = Projectile.GetAlpha(lightColor);
            //prim.Draw(Projectile.oldPos);
            var frame = texture.Frame(1,1,0,0);
            var origin = new Vector2(frame.Width, frame.Height / 2f - 1f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(texture, Projectile.position + offset, new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height), Color.White, Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public Color colorStrip(float progress)
        {
            return Color.SeaShell;
        }

        public float widthStrip(float progress)
        {
            return progress * 1f;
        }

    }

    public class ElectrapulseCanisterProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Canister Lightning");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage = 0;
            Projectile.timeLeft = 120;
            Projectile.alpha = 0;
            Projectile.extraUpdates = 5;
        }

        Vector2 initialVelocity = Vector2.Zero;

        private float lerp;
        public Vector2 DrawPos;
        public int boost;
        public override void AI()
        {
            if (initialVelocity == Vector2.Zero)
            {
                initialVelocity = Projectile.velocity;
            }
            if (Projectile.timeLeft % 10 == 0)
            {
                Projectile.velocity = initialVelocity.RotatedBy(Main.rand.NextFloat(-1, 1));
            }
            /* if (projectile.timeLeft % 2 == 0)
             {
                 Dust dust = Dust.NewDustPerfect(projectile.Center, 226);
                 dust.noGravity = true;
                 dust.scale = (float)Math.Sqrt(projectile.timeLeft) / 4;
                 dust.velocity = Vector2.Zero;
             }*/
            DrawPos = Projectile.position;
        }
    }
}