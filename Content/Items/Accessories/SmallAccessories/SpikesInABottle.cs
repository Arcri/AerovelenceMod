using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class SpikesInABottle : TranslatableModItem
    {
        public override string Texture => "Terraria/Images/Item_53";
        private const string EnglishTooltip = "Allows a Cloud-strength double jump that scatters crystal caltrops\nTaking at least 15% of maximum life in one hit scatters more after half a second\nThe retaliation can occur once every 6 seconds\nCannot be made into a balloon: it would pop";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Spikes in a Bottle", EnglishTooltip)
                .AddName(Language.Spanish, "Púas en una Botella")
                .AddTooltip(Language.Spanish, "Permite un doble salto como el de Nube en una Botella que esparce abrojos de cristal\nRecibir al menos el 15% de tu vida máxima en un golpe esparce más tras medio segundo\nLa represalia puede ocurrir una vez cada 6 segundos\nNo se puede combinar con un globo: lo pincharía");
            base.SetStaticDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpikesInABottlePlayer>().Equipped = true;
            player.GetJumpState<SpikedBottleJump>().Enable();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D crystal = ModContent.Request<Texture2D>(SpikesInABottleVFX.CrystalTexture).Value;
            for (int i = -1; i <= 1; i++)
                spriteBatch.Draw(crystal, position + new Vector2(i * 4f, 4f) * scale, null, Color.White, i * 0.45f, crystal.Size() * 0.5f, scale * 0.45f, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
            => SpikesInABottleVFX.Crystal(Item.Center + new Vector2(0f, 3f), rotation, new Vector2(5f, 10f) * scale);
    }

    public class SpikedBottleJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(CloudInABottle);
        public override float GetDurationMultiplier(Player player) => CloudInABottle.GetDurationMultiplier(player);
        public override void OnStarted(Player player, ref bool playSound)
        {
            SpikesInABottleVFX.Burst(player.Bottom, 10, 3f);
            if (!Main.dedServ)
                for (int i = 0; i < 7; i++)
                    SpikesInABottleVFX.Smoke(player.Bottom + new Vector2(i * 5f - 15f, 0f), new Vector2((i - 3) * 0.55f, 1.2f), 64f, Color.LightBlue);
            player.GetModPlayer<SpikesInABottlePlayer>().Scatter(5, false);
        }
    }

    public class BottleCaltrop : ModProjectile
    {
        public override string Texture => SpikesInABottleVFX.CrystalTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(10f, Projectile.velocity.Y + 0.3f);
            Projectile.rotation += Projectile.velocity.X * 0.08f;
            Lighting.AddLight(Projectile.Center, SpikesInABottleVFX.Aqua.ToVector3() * 0.14f * Math.Min(1f, Projectile.timeLeft / 30f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                Projectile.velocity.X *= 0.65f;
                if (oldVelocity.Y > 2f)
                {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
                    SpikesInABottleVFX.Burst(Projectile.Bottom, 3, 1.2f);
                }
            }
            if (oldVelocity.X != Projectile.velocity.X && Projectile.velocity.X == 0f)
                Projectile.velocity.X = -oldVelocity.X * 0.3f;
            return false;
        }

        public override bool? CanDamage() => Projectile.timeLeft > 30 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpikesInABottleVFX.Burst(Projectile.Center, 5, 2f);
            SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.2f, PitchVariance = 0.2f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = Math.Min(1f, Projectile.timeLeft / 30f);
            SpikesInABottleVFX.Glow(Projectile.Center, new Vector2(26f), SpikesInABottleVFX.Aqua, opacity * 0.22f);
            for (int i = 0; i < 3; i++)
                SpikesInABottleVFX.Crystal(Projectile.Center, Projectile.rotation + i * MathHelper.TwoPi / 3f, new Vector2(5f, 14f), opacity, 0.25f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 1)
                return;
            SpikesInABottleVFX.Burst(Projectile.Center, 4, 1.5f);
            SpikesInABottleVFX.Rubble(Projectile.Center, 2, 2f);
        }
    }

    public class SpikesInABottlePlayer : ModPlayer
    {
        public bool Equipped;
        private int delay;
        private int cooldown;
        public override void ResetEffects() => Equipped = false;
        public override void UpdateDead()
        {
            delay = 0;
            if (cooldown > 0)
                cooldown--;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.whoAmI == Main.myPlayer && Equipped && cooldown == 0 && info.Damage >= Math.Max(1, (int)Math.Ceiling(Player.statLifeMax2 * 0.15d)))
            {
                delay = 30;
                cooldown = 360;
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.25f, Pitch = -0.2f }, Player.Center);
            }
        }

        public override void PostUpdate()
        {
            if (cooldown > 0)
                cooldown--;
            if (!Equipped)
                delay = 0;
            if (delay <= 0)
                return;
            if (delay % 3 == 0)
            {
                float angle = delay * 0.4f;
                Vector2 offset = angle.ToRotationVector2() * (12f + delay * 0.65f);
                SpikesInABottleVFX.Spark(Player.Center + offset, -offset * 0.08f, 0.16f);
            }
            if (--delay == 0)
                Scatter(9, true);
        }

        public void Scatter(int count, bool retaliation)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            int active = Player.ownedProjectileCounts[ModContent.ProjectileType<BottleCaltrop>()];
            count = Math.Min(count, Math.Max(0, 24 - active));
            Vector2 origin = retaliation ? Player.Center : Player.Bottom - Vector2.UnitY * 4f;
            for (int i = 0; i < count; i++)
            {
                float x = MathHelper.Lerp(-4.5f, 4.5f, count <= 1 ? 0.5f : i / (float)(count - 1));
                Vector2 velocity = new(x + Player.velocity.X * 0.2f, retaliation ? -4.5f - Main.rand.NextFloat(2f) : 1f + Main.rand.NextFloat(2f));
                Projectile.NewProjectile(Player.GetSource_FromThis(), origin, velocity, ModContent.ProjectileType<BottleCaltrop>(), 12, 2f, Player.whoAmI);
            }
            if (retaliation && count > 0)
            {
                SpikesInABottleVFX.Burst(origin, 12, 4f);
                SpikesInABottleVFX.Smoke(origin, -Vector2.UnitY, 100f, SpikesInABottleVFX.Aqua);
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.35f, Pitch = 0.35f }, origin);
            }
        }
    }

    public class SpikesInABottleDebris : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Magnetic_Platform_Debris";
        private static readonly Rectangle[] Frames = [new(0, 0, 12, 22), new(16, 0, 18, 22), new(38, 0, 26, 22)];

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Frames[Main.rand.Next(Frames.Length)];
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.fadeIn = Main.rand.NextFloat(-0.12f, 0.12f);
            dust.noGravity = dust.noLight = true;
            dust.customData = 0;
        }

        public override bool Update(Dust dust)
        {
            int age = (int)dust.customData + 1;
            dust.customData = age;
            dust.velocity.Y = Math.Min(12f, dust.velocity.Y + 0.22f);
            Vector2 movement = Collision.TileCollision(dust.position - Vector2.One * 2f, dust.velocity, 4, 4);
            dust.position += movement;
            if (movement.Y != dust.velocity.Y)
            {
                dust.velocity.Y *= -0.3f;
                dust.velocity.X *= 0.7f;
                dust.fadeIn *= 0.6f;
            }
            if (movement.X != dust.velocity.X)
                dust.velocity.X *= -0.3f;
            dust.rotation += dust.fadeIn;
            if (age > 26)
                dust.alpha += 9;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            float opacity = 1f - dust.alpha / 255f;
            Vector2 position = dust.position - Main.screenPosition;
            Color light = Lighting.GetColor(dust.position.ToTileCoordinates());
            Main.EntitySpriteDraw(texture, position, dust.frame, Color.Lerp(light, Color.White, 0.25f) * opacity, dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, position, dust.frame, SpikesInABottleVFX.Additive(SpikesInABottleVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class SpikesInABottleVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal static readonly Color Aqua = new(85, 218, 255);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

        internal static void Sprite(string asset, Vector2 center, Vector2 size, Color color, float rotation = 0f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, color, rotation, texture.Size() * 0.5f, size / texture.Size(), SpriteEffects.None);
        }

        internal static void Glow(Vector2 center, Vector2 size, Color color, float opacity)
            => Sprite("AerovelenceMod/Assets/Orbs/SoftGlow", center, size, Additive(color, opacity));

        internal static void Crystal(Vector2 center, float rotation, Vector2 size, float opacity = 1f, float charge = 0.3f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(CrystalTexture).Value;
            Vector2 screen = center - Main.screenPosition;
            Vector2 scale = size / texture.Size();
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = (i * MathHelper.PiOver2 + rotation).ToRotationVector2() * (1f + charge);
                Main.EntitySpriteDraw(texture, screen + offset, null, Additive(Aqua, opacity * charge * 0.45f), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, screen, null, Color.White * opacity, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, screen, null, Additive(Color.White, opacity * charge), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
        }

        internal static void Spark(Vector2 center, Vector2 velocity, float scale = 0.2f, Color? color = null)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: color ?? Aqua, Scale: scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.95f,
                timeBeforeSlow: 6, postSlowPower: 0.86f, velToBeginShrink: 1f, fadePower: 0.86f, shouldFadeColor: false);
        }

        internal static void Burst(Vector2 center, int count, float speed)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Spark(center, Main.rand.NextVector2CircularEdge(speed, speed) * Main.rand.NextFloat(0.4f, 1f), Main.rand.NextFloat(0.13f, 0.28f));
        }

        internal static void Smoke(Vector2 center, Vector2 velocity, float size, Color color)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<HighResSmoke>(), velocity, newColor: color, Scale: size / 128f);
            dust.customData = new HighResSmokeBehavior
            {
                randomSmokeNumber = 1,
                frameToStartFade = 8,
                fadeDuration = 28,
                velSlowAmount = 0.96f,
                drawSoftGlowUnder = false,
                overallAlpha = 0.8f
            };
        }

        internal static void Rubble(Vector2 center, int count, float speed = 4f)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Dust.NewDustPerfect(center, ModContent.DustType<SpikesInABottleDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
