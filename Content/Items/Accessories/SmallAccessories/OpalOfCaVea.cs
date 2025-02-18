using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using AerovelenceMod.Common.Systems;
using System;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class OpalOfCaVea : ModItem
    {
        public override void SetStaticDefaults()
        {
            //Opal of Ca Vea;
            //Grows crystals on your back and emits a blue glow that intensifies with crystal growth
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.EarlyPHM;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OpalOfCaVeaPlayer>().hasOpal = true;

            if (!hideVisual)
            {
                player.GetModPlayer<OpalOfCaVeaPlayer>().hasOpalVisibility = true;
            }
        }
    }

    public class OpalOfCaVeaPlayer : ModPlayer
    {
        public bool hasOpal;
        public bool hasOpalVisibility;
        public int crystalCount;
        private int crystalTimer;

        public override void ResetEffects()
        {
            if (!hasOpal)
            {
                crystalCount = crystalTimer = 0;
            }
            hasOpal = false;
        }

        public override void PostUpdate()
        {
            if (hasOpal)
            {
                crystalTimer++;
                if (crystalCount < 3 && crystalTimer >= 300)
                {
                    crystalCount++;
                    crystalTimer = 0;
                }
                float intensity = crystalCount / 3f;
                Lighting.AddLight(Player.Center, new Vector3(0.0f, 0.3f, 0.7f) * intensity);
            }
        }

        public override void OnRespawn()
        {
            crystalCount = crystalTimer = 0;
        }


        public override void OnHitByNPC(NPC npc, Player.HurtInfo info)
        {
            if (hasOpal && crystalCount >= 3)
            {
                int reflectDamage = 20;
                npc.StrikeNPC(new NPC.HitInfo
                {
                    Damage = reflectDamage,
                    Knockback = 0f,
                    HitDirection = 0,
                    Crit = false
                }, fromNet: false, noPlayerInteraction: false);

                if (Main.myPlayer == Player.whoAmI)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<ElectricPopEffect>(), 0, 0f, Player.whoAmI);
                    crystalCount = 0;
                    crystalTimer = 0;
                    for (int t = 0; t < 8; t++)
                    {
                        Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);

                        Dust gd = Dust.NewDustPerfect(npc.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.SkyBlue, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                        gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                            preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
                    }
                }
            }
        }
    }

    public class OpalOfCaVeaCrystalLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<OpalOfCaVeaPlayer>().hasOpal;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (player.dead) return;

            var modPlayer = player.GetModPlayer<OpalOfCaVeaPlayer>();
            if (!modPlayer.hasOpal) return;

            if (modPlayer.hasOpalVisibility)
            {
                Texture2D crystalTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Natural/CavernCrystalItem").Value;
                Vector2 basePosition = player.MountedCenter;
                Vector2 screenPos = Main.screenPosition;
                float heightFactor = player.mount.Active ? 0.8f : 1f;
                Vector2 offset1 = new(-14 * player.direction, -player.height * 0.5f * heightFactor);
                Vector2 offset2 = new(-13 * player.direction, -player.height * 0.2f * heightFactor);
                Vector2 offset3 = new(-8 * player.direction, player.height * 0.1f * heightFactor);
                Vector2[] offsets = [offset1, offset2, offset3];

                int crystalsToDraw = modPlayer.crystalCount;
                for (int i = 0; i < crystalsToDraw; i++)
                {
                    Vector2 offset = offsets[i];
                    offset.X -= i * 4 * player.direction;
                    offset.Y += 10f;
                    Vector2 drawPos = basePosition + offset - screenPos;
                    float rotation = (player.direction == 1) ? MathHelper.ToRadians(45) : MathHelper.ToRadians(-45);
                    float dynamicSway = player.velocity.X * 0.05f;
                    rotation -= dynamicSway;
                    SpriteEffects effects = (player.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    DrawData data = new(crystalTexture, drawPos, null, Color.White, rotation, crystalTexture.Size() * 0.5f, 1f, effects, 0);
                    drawInfo.DrawDataCache.Add(data);
                }
            }
        }
    }


    public class OpalGlowLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<OpalOfCaVeaPlayer>().crystalCount >= 3;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
        }
    }


    public class ElectricPopEffect : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/ElectricPopD";
        public float drawScale = 0.005f;
        private float colorLerpProgress = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.1f;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Projectile.alpha = Math.Min(Projectile.alpha + 8, 255);
            colorLerpProgress = (120f - Projectile.timeLeft) / 60f;
            drawScale += 0.0055f;
            Projectile.rotation += 0.3f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixellationSystem.QueuePixelationAction(() =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/ElectricPopE").Value;
                Texture2D texture2 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/ElectricPopC").Value;
                Rectangle frame = texture.Frame();
                Vector2 origin = frame.Size() / 2f;
                Color color1Start = new(0, 255, 255);
                Color color1End = new(0, 0, 255);
                Color color2Start = new(255, 255, 255);
                Color color2End = new(255, 255, 0);
                Color drawColor = Color.Lerp(color1Start, color1End, colorLerpProgress) * ((255 - Projectile.alpha) / 255f);
                Color drawColor2 = Color.Lerp(color2Start, color2End, colorLerpProgress) * ((255 - Projectile.alpha) / 255f);
                Player player = Main.player[Projectile.owner];
                Vector2 drawPos = (player.Center - Main.screenPosition) / 2f;
                float finalDrawScale = drawScale / 2;
                spriteBatch.Draw(texture, drawPos, frame, drawColor, Projectile.rotation, origin, finalDrawScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture2, drawPos, frame, drawColor2, -Projectile.rotation, origin, finalDrawScale, SpriteEffects.None, 0f);
            }, PixellationSystem.RenderType.Additive);

            return false;
        }
    }
}