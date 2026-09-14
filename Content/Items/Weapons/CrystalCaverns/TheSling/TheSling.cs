using AerovelenceMod.Content.Items.Ammo;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class TheSling : TranslatableModItem
    {
        private const string EnglishTooltip = "Hold to wind up a slug in a moth-silk sling\nRelease to throw in the direction the pouch is traveling\nAim beyond the orbit; the white flash and chime mark a powerful, well-aligned throw\nStone slugs hit hard, wood flies farther, and crystal leaves splinters\nUses one slug per wind-up";
        public override string Texture => SlingArt.PouchTexture;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("The Sling", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Release during the white flash for a faster throw and your slug's Skill Strike bonus")
                .AddName(Language.Spanish, "La Honda")
                .AddTooltip(Language.Spanish, "Mantén pulsado para hacer girar un proyectil en una honda de seda de polilla\nSuelta para lanzarlo en la dirección en que se mueve la bolsa\nApunta más allá de la órbita; el destello blanco y el tintineo indican un lanzamiento potente y bien alineado\nLa piedra golpea fuerte, la madera llega más lejos y el cristal deja astillas\nUsa un proyectil cada vez que comienzas a girar")
                .AddSkillStrike(Language.Spanish, "Suelta durante el destello blanco para lanzar más rápido y obtener el Golpe de Habilidad de tu munición");
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
            Item.width = 24;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = Item.channel = true;
            Item.shoot = ModContent.ProjectileType<StoneSlugShot>();
            Item.shootSpeed = 10f;
            Item.knockBack = 3f;
            Item.useAmmo = ModContent.ItemType<StoneSlug>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<SlingHeld>()] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX * player.direction);
            float angle = aim.ToRotation() - MathHelper.PiOver2;
            Projectile.NewProjectile(source, player.MountedCenter + angle.ToRotationVector2() * SlingMotion.Radius(0f), new Vector2(velocity.Length() / 11f, 0f),
                ModContent.ProjectileType<SlingHeld>(), damage, knockback, player.whoAmI, angle, 0f, type);
            return false;
        }
    }

    internal static class SlingArt
    {
        internal const string PouchTexture = "AerovelenceMod/Content/Items/Weapons/Misc/Ranged/PouchOfRocks";
        private const string CordTexture = "AerovelenceMod/Content/Items/Weapons/Misc/Melee/FlailChain";

        internal static void Cord(Vector2 start, Vector2 end, Vector2 bow, Color light, float opacity, float charge)
        {
            Texture2D cord = ModContent.Request<Texture2D>(CordTexture).Value;
            int count = Math.Max(2, (int)(Vector2.Distance(start, end) / 6f));
            Vector2 previous = start;
            for (int i = 1; i <= count; i++)
            {
                float t = i / (float)count;
                Vector2 point = Vector2.Lerp(start, end, t) + bow * MathF.Sin(t * MathHelper.Pi);
                Vector2 delta = point - previous;
                Vector2 scale = new(0.45f, (delta.Length() + 0.6f) / cord.Height);
                Main.EntitySpriteDraw(cord, previous - Main.screenPosition, null, Color.Lerp(light, Color.LightSteelBlue, 0.25f) * opacity,
                    delta.ToRotation() - MathHelper.PiOver2, new Vector2(cord.Width * 0.5f, 0f), scale, SpriteEffects.None);
                Main.EntitySpriteDraw(cord, previous - Main.screenPosition, null, TheSlingVFX.Additive(TheSlingVFX.Aqua, charge * opacity * 0.35f),
                    delta.ToRotation() - MathHelper.PiOver2, new Vector2(cord.Width * 0.5f, 0f), scale, SpriteEffects.None);
                previous = point;
            }
        }
    }

    public class SlingHeld : ModProjectile
    {
        private float aimAngle;
        private float sentAim = float.NaN;
        private bool releaseReady;
        private bool released;
        private int releaseTicks;
        private float glow;
        private int chimeCooldown;
        public override string Texture => SlingArt.PouchTexture;
        private TheSlingSlugMaterial Material => (int)Projectile.ai[2] == ModContent.ProjectileType<WoodSlugShot>() ? TheSlingSlugMaterial.Wood :
            (int)Projectile.ai[2] == ModContent.ProjectileType<CrystalStoneSlugShot>() ? TheSlingSlugMaterial.Crystal : TheSlingSlugMaterial.Stone;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void OnSpawn(IEntitySource source) => aimAngle = Projectile.ai[0] + MathHelper.PiOver2;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(aimAngle);
            writer.Write(released);
            writer.Write(releaseTicks);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            aimAngle = reader.ReadSingle();
            released = reader.ReadBoolean();
            releaseTicks = reader.ReadInt32();
        }

        private void Release(Player player)
        {
            released = true;
            Projectile.netUpdate = true;
            Vector2 tangent = (Projectile.ai[0] + MathHelper.PiOver2).ToRotationVector2();
            Vector2 muzzle = Projectile.Center;
            if (!Collision.CanHitLine(player.Center, 1, 1, muzzle, 1, 1))
                muzzle = player.Center;
            float speed = SlingMotion.ReleaseSpeed(Projectile.ai[1], Projectile.ai[0], releaseReady) * Math.Max(0.1f, Projectile.velocity.X);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), muzzle, tangent * speed + player.velocity * 0.15f,
                (int)Projectile.ai[2], Projectile.damage, Projectile.knockBack, Projectile.owner, releaseReady ? 1f : 0f);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<TheSling>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 60;
            chimeCooldown = Math.Max(0, chimeCooldown - 1);
            Vector2 radial = Projectile.ai[0].ToRotationVector2();
            float radius = SlingMotion.Radius(Projectile.ai[1]);
            Projectile.Center = player.MountedCenter + radial * radius;
            if (!released && Projectile.owner == Main.myPlayer && !player.channel)
                Release(player);
            if (released)
            {
                releaseTicks++;
                if (releaseTicks >= 14)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.ai[0] += SlingMotion.AngularSpeed(Projectile.ai[1], Projectile.ai[0]) * (1f - releaseTicks / 14f);
                radius *= 1f - releaseTicks / 16f;
                releaseReady = false;
            }
            else
            {
                Projectile.ai[1]++;
                float oldAngle = Projectile.ai[0];
                Projectile.ai[0] = MathHelper.WrapAngle(oldAngle + SlingMotion.AngularSpeed(Projectile.ai[1], oldAngle));
                radius = SlingMotion.Radius(Projectile.ai[1]);
            }
            radial = Projectile.ai[0].ToRotationVector2();
            Projectile.Center = player.MountedCenter + radial * radius;
            if (!released)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    float desiredAim = (Main.MouseWorld - Projectile.Center).SafeNormalize(radial.RotatedBy(MathHelper.PiOver2)).ToRotation();
                    if (Projectile.ai[1] % 6f == 0f && (float.IsNaN(sentAim) || Math.Abs(MathHelper.WrapAngle(desiredAim - sentAim)) > 0.06f))
                    {
                        sentAim = desiredAim;
                        Projectile.netUpdate = true;
                    }
                    aimAngle = desiredAim;
                }
                bool ready = SlingMotion.Ready(Projectile.ai[1], Projectile.ai[0], aimAngle);
                if (ready && !releaseReady && chimeCooldown == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.25f, Pitch = 0.55f }, Projectile.Center);
                    TheSlingVFX.Burst(Projectile.Center, 5, 1.8f);
                    chimeCooldown = 18;
                }
                releaseReady = ready;
                if (Projectile.ai[1] % 32f == 0f)
                    SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.2f, Pitch = 0.3f + SlingMotion.Charge(Projectile.ai[1]) * 0.3f }, Projectile.Center);
            }
            glow = MathHelper.Lerp(glow, releaseReady ? 1f : 0f, releaseReady ? 0.65f : 0.3f);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.ai[0] - MathHelper.PiOver2);
            if (releaseReady && Projectile.ai[1] % 3f == 0f)
                TheSlingVFX.Spark(Projectile.Center, radial.RotatedBy(MathHelper.PiOver2) * 1.5f, 0.2f, Color.White);
        }

        public override void OnKill(int timeLeft)
        {
            if (!released)
                TheSlingVFX.Burst(Projectile.Center, 4, 1.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 radial = Projectile.ai[0].ToRotationVector2();
            Vector2 tangent = radial.RotatedBy(MathHelper.PiOver2);
            float opacity = released ? 1f - releaseTicks / 14f : 1f;
            float charge = SlingMotion.Charge(Projectile.ai[1]);
            float radius = Vector2.Distance(Projectile.Center, player.MountedCenter);
            if (!released)
            {
                for (int i = 5; i >= 1; i--)
                {
                    float angle = Projectile.ai[0] - i * SlingMotion.AngularSpeed(Projectile.ai[1], Projectile.ai[0]);
                    TheSlingSlugArt.Draw(Main.spriteBatch, player.MountedCenter + angle.ToRotationVector2() * radius - Main.screenPosition, Material,
                        angle, 9f, TheSlingVFX.Additive(TheSlingSlugArt.Tint(Material), (1f - i / 6f) * charge * 0.3f), 0f);
                }
            }
            for (int sign = -1; sign <= 1; sign += 2)
            {
                Vector2 start = player.MountedCenter + tangent * sign * 3f;
                Vector2 end = Projectile.Center + tangent * sign * 5f;
                Vector2 bow = -tangent * (3f + (released ? releaseTicks : 0f));
                SlingArt.Cord(start, end, bow, lightColor, opacity, glow);
            }
            Texture2D pouch = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(pouch, Projectile.Center - Main.screenPosition, null, lightColor * opacity, Projectile.ai[0] - MathHelper.PiOver2,
                pouch.Size() * 0.5f, new Vector2(18f, 20f) / pouch.Size(), SpriteEffects.None);
            if (!released)
                TheSlingSlugArt.Draw(Main.spriteBatch, Projectile.Center - radial * 2f - Main.screenPosition, Material, Projectile.ai[0], 11f, Color.White, glow * 0.55f);
            if (glow > 0.02f)
            {
                TheSlingVFX.Glow(Projectile.Center, new Vector2(46f), TheSlingVFX.Aqua, glow * 0.5f * opacity);
                if (releaseReady)
                    TheSlingVFX.Flare(Projectile.Center, 40f, opacity, Projectile.ai[0]);
            }
            if (releaseReady)
                TheSlingVFX.Sprite("AerovelenceMod/Assets/MuzzleFlashes/WhitePixelMuzzleFlash", Projectile.Center + tangent * 18f,
                    new Vector2(23f, 12f), TheSlingVFX.Additive(Color.White, 0.75f), tangent.ToRotation());
            return false;
        }
    }

    internal static class SlingMotion
    {
        internal static float Charge(float age) => Math.Clamp(age / 40f, 0f, 1f);
        internal static float Radius(float age) => 24f + 22f * Charge(age);
        internal static float AngularSpeed(float age, float angle) => 0.062f + 0.05f * Charge(age) + 0.022f * MathF.Sin(angle);
        internal static bool Ready(float age, float angle, float aim)
            => age >= 28f && AngularSpeed(age, angle) >= 0.075f && MathF.Cos(angle + MathF.PI * 0.5f - aim) >= 0.9f;
        internal static float ReleaseSpeed(float age, float angle, bool ready)
            => (5f + AngularSpeed(age, angle) * 75f) * (0.6f + Charge(age) * 0.4f) * (ready ? 1.22f : 1f);
    }

    public enum TheSlingSlugMaterial { Stone, Wood, Crystal }

    internal static class TheSlingSlugArt
    {
        internal static Color Tint(TheSlingSlugMaterial material) => material switch
        {
            TheSlingSlugMaterial.Wood => new Color(220, 177, 115),
            TheSlingSlugMaterial.Crystal => TheSlingVFX.Aqua,
            _ => new Color(155, 180, 218)
        };
        internal static void Draw(SpriteBatch spriteBatch, Vector2 center, TheSlingSlugMaterial material, float angle, float size, Color light, float charge, float opacity = 1f)
        {
            string asset = material switch
            {
                TheSlingSlugMaterial.Wood => "AerovelenceMod/Content/Items/Ammo/WoodSlug",
                TheSlingSlugMaterial.Crystal => "AerovelenceMod/Content/Items/Ammo/CrystalStoneSlug",
                _ => "AerovelenceMod/Content/Items/Ammo/StoneSlug"
            };
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            float scale = size / Math.Max(texture.Width, texture.Height);
            spriteBatch.Draw(texture, center, null, light * opacity, angle, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            if (charge > 0f)
                spriteBatch.Draw(texture, center, null, TheSlingVFX.Additive(Color.White, charge * opacity * 0.6f), angle, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    internal static class TheSlingVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal const string RockTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ChargedStoneProjectile";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Rectangle RockFrame = new(4, 6, 54, 48);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

        internal static void Sprite(string asset, Vector2 center, Vector2 size, Color color, float rotation = 0f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, color, rotation, texture.Size() * 0.5f, size / texture.Size(), SpriteEffects.None);
        }

        internal static void Glow(Vector2 center, Vector2 size, Color color, float opacity)
            => Sprite("AerovelenceMod/Assets/Orbs/SoftGlow", center, size, Additive(color, opacity));

        internal static void Flare(Vector2 center, float size, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/ImpactTextures/Spike", center, new Vector2(size * 0.65f, size), Additive(Color.White, opacity), rotation);

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
    }
}
