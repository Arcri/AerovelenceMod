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
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class Saphead : TranslatableModItem
    {
        internal const string HeadTexture = "AerovelenceMod/Content/NPCs/CrystalCaverns/Sapper";
        private const string EnglishTooltip = "Guide a living sapper head toward the cursor on a flexible stem\nReaches up to 10 tiles; bring it closer to spout crystal fog faster\nThe stem glows and the flower swells before each puff\nConsumes 5 mana per puff; the initial cast pays for the first\nRelease to coil the stem back into your hand\n'A severed sapper stem, improbably still alive'";
        public override string Texture => HeadTexture;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Saphead", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Enemies caught in its crystal fog")
                .AddName(Language.Spanish, "Cabeza de Savia")
                .AddTooltip(Language.Spanish, "Guía una cabeza viva de Sapper hacia el cursor con un tallo flexible\nAlcanza hasta 10 bloques; acércala para expulsar niebla de cristal más rápido\nEl tallo brilla y la flor se hincha antes de cada bocanada\nConsume 5 de maná por bocanada; el lanzamiento inicial paga la primera\nSuelta para recoger el tallo en tu mano\n'Un tallo de Sapper cortado que, increíblemente, sigue vivo'")
                .AddSkillStrike(Language.Spanish, "Enemigos atrapados en su niebla de cristal");
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
            Item.width = Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SapheadFlail>();
            Item.shootSpeed = 1f;
            Item.knockBack = 2f;
            Item.mana = 5;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 offset = Main.MouseWorld - player.MountedCenter;
            Vector2 aim = offset.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, player.MountedCenter + aim * 14f, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI,
                aim.ToRotation(), MathHelper.Clamp(offset.Length(), 44f, 160f));
            return false;
        }
    }

    public class SapheadDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<global::AerovelenceMod.Content.NPCs.CrystalCaverns.Sapper>())
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Saphead>(), 20));
        }
    }

    public class SapheadFlail : ModProjectile
    {
        private readonly SapheadOrbit orbit = new();
        private int seenBlooms;
        private float bloomFlash;
        private float age;
        private float targetAngle;
        private bool Returning => Projectile.ai[2] > 0f;
        private float Opacity => Math.Clamp(orbit.Radius / 22f, 0f, 1f);
        public override string Texture => Saphead.HeadTexture;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ownerHitCheck = true;
            Projectile.netImportant = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Returning || orbit.Radius < 24f ? false : null;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetAngle);
            writer.Write(orbit.Radius);
            writer.Write(orbit.Charge);
            writer.Write(orbit.Blooms);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetAngle = reader.ReadSingle();
            orbit.Radius = reader.ReadSingle();
            orbit.Charge = reader.ReadSingle();
            orbit.Blooms = reader.ReadInt32();
        }

        private void Return()
        {
            Projectile.ai[2] = 1f;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<Saphead>())
            {
                Projectile.Kill();
                return;
            }
            if (age == 0f) targetAngle = Projectile.ai[0];
            age++;
            if (Projectile.owner == Main.myPlayer && !Returning)
            {
                if (!player.channel)
                    Return();
                Vector2 cursor = Main.MouseWorld - player.MountedCenter;
                float desired = MathHelper.Clamp(cursor.Length(), 24f, 160f);
                float nextAngle = cursor.SafeNormalize(Projectile.ai[0].ToRotationVector2()).ToRotation();
                if (Math.Abs(desired - Projectile.ai[1]) > 2f || Math.Abs(MathHelper.WrapAngle(nextAngle - targetAngle)) > 0.025f)
                {
                    Projectile.ai[1] = desired;
                    targetAngle = nextAngle;
                    Projectile.netUpdate = true;
                }
            }
            orbit.Advance(Projectile.ai[1], Returning);
            if (Returning && orbit.Radius < 6f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 60;
            float spin = Projectile.velocity.X < 0f ? -1f : 1f;
            Projectile.ai[0] = MathHelper.WrapAngle(Projectile.ai[0] + Math.Clamp(MathHelper.WrapAngle(targetAngle - Projectile.ai[0]) * 0.22f, -0.18f, 0.18f));
            Vector2 axis = Projectile.ai[0].ToRotationVector2();
            if (Math.Abs(axis.X) > 0.05f)
                player.ChangeDir(axis.X > 0f ? 1 : -1);
            Projectile.Center = player.MountedCenter + axis * orbit.Radius;
            Projectile.rotation = Projectile.ai[0] - MathHelper.PiOver2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.ai[0] - MathHelper.PiOver2);
            Vector2 mouth = Projectile.Center + axis * 13f;
            if (!Returning && orbit.Ready && Projectile.owner == Main.myPlayer &&
                Collision.CanHitLine(player.MountedCenter, 1, 1, mouth, 1, 1) && !Collision.SolidCollision(mouth - new Vector2(10f), 20, 20))
            {
                if (!orbit.Prepaid && !player.CheckMana(player.HeldItem, -1, true))
                    Return();
                else
                {
                    orbit.Bloom();
                    player.manaRegenDelay = Math.Max(player.manaRegenDelay, 60);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), mouth, axis * 1.25f + axis.RotatedBy(spin * MathHelper.PiOver2) * 0.35f,
                        ModContent.ProjectileType<SapheadFog>(), Projectile.damage, 0f, Projectile.owner);
                    Projectile.netUpdate = true;
                }
            }
            bloomFlash *= 0.82f;
            if (seenBlooms != orbit.Blooms)
            {
                seenBlooms = orbit.Blooms;
                bloomFlash = 1f;
            }
            if (!Returning && orbit.Charge > 0.65f && age % 5f == 0f)
                SapheadVFX.Spark(mouth + Main.rand.NextVector2Circular(9f, 9f), -axis * 0.5f, 0.12f + orbit.Charge * 0.07f);
            Lighting.AddLight(Projectile.Center, SapheadVFX.Aqua.ToVector3() * (0.12f + orbit.Charge * 0.25f) * Opacity);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SapheadVFX.Burst(Projectile.Center, 5, 2f);
            SoundEngine.PlaySound(SoundID.NPCHit1 with { Volume = 0.3f, Pitch = 0.15f }, Projectile.Center);
        }

        public override void OnKill(int timeLeft) => SapheadVFX.Burst(Projectile.Center, Returning ? 4 : 10, Returning ? 1f : 2.5f);

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 axis = Projectile.ai[0].ToRotationVector2();
            Vector2 tangent = axis.RotatedBy(MathHelper.PiOver2) * (Projectile.velocity.X < 0f ? 1f : -1f);
            Texture2D vine = ModContent.Request<Texture2D>(Saphead.HeadTexture + "_Vines").Value;
            Texture2D vineGlow = ModContent.Request<Texture2D>(Saphead.HeadTexture + "_Vines_Glow").Value;
            Vector2 root = player.MountedCenter + axis * 5f;
            Vector2 end = Projectile.Center - axis * 9f;
            int segments = Math.Max(2, (int)(orbit.Radius / 15f));
            Vector2 previous = root;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 point = Vector2.Lerp(root, end, t) + tangent * MathF.Sin(t * MathHelper.Pi) * orbit.Radius * 0.14f;
                Vector2 delta = point - previous;
                Rectangle frame = new(0, ((Projectile.identity + i) % 3) * 34, 22, 32);
                float pulse = MathF.Pow(Math.Max(0f, MathF.Cos(t * 9f - age * 0.16f)), 6f) * orbit.Charge;
                Vector2 scale = new(0.45f + pulse * 0.08f, (delta.Length() + 1f) / 32f);
                float rotation = delta.ToRotation() - MathHelper.PiOver2;
                Color light = Lighting.GetColor(previous.ToTileCoordinates());
                Main.EntitySpriteDraw(vine, previous - Main.screenPosition, frame, light * Opacity, rotation, new Vector2(11f, 0f), scale, SpriteEffects.None);
                Main.EntitySpriteDraw(vineGlow, previous - Main.screenPosition, frame, SapheadVFX.Additive(SapheadVFX.Aqua, (0.15f + pulse * 0.8f + bloomFlash * 0.3f) * Opacity),
                    rotation, new Vector2(11f, 0f), scale, SpriteEffects.None);
                previous = point;
            }
            Texture2D head = TextureAssets.Projectile[Type].Value;
            float swell = orbit.Charge * orbit.Charge;
            Vector2 headScale = new Vector2(1f + swell * 0.16f + bloomFlash * 0.12f, 1f - swell * 0.06f - bloomFlash * 0.12f) * (32f / head.Width);
            Vector2 center = Projectile.Center - Main.screenPosition;
            SapheadVFX.Glow(Projectile.Center + axis * 8f, new Vector2(48f + swell * 16f), SapheadVFX.Violet, (0.16f + swell * 0.24f) * Opacity);
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(head, center + (i * MathHelper.PiOver2 + age * 0.02f).ToRotationVector2() * (1f + swell), null,
                    SapheadVFX.Additive(SapheadVFX.Aqua, swell * 0.17f * Opacity), Projectile.rotation, head.Size() * 0.5f, headScale, SpriteEffects.None);
            Main.EntitySpriteDraw(head, center, null, Color.Lerp(lightColor, Color.White, 0.2f) * Opacity, Projectile.rotation, head.Size() * 0.5f, headScale, SpriteEffects.None);
            Main.EntitySpriteDraw(head, center, null, SapheadVFX.Additive(SapheadVFX.Aqua, (swell * 0.3f + bloomFlash * 0.6f) * Opacity),
                Projectile.rotation, head.Size() * 0.5f, headScale, SpriteEffects.None);
            return false;
        }
    }

    public class SapheadFog : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Smoke/Smoke1Enhanced";
        private float Age => Projectile.ai[0];
        private float Radius => SapheadOrbit.FogRadius(Age);
        private float Opacity => Math.Clamp(Age / 6f, 0f, 1f) * Math.Clamp((56f - Age) / 18f, 0f, 1f);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 56;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
        }

        public override bool? CanDamage() => SapheadOrbit.FogCanDamage(Age) ? null : false;
        public override bool? CanHitNPC(NPC target) => Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(Projectile.Center, nearest) <= Radius * Radius;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
            => SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 1, 0.2f, 0.45f);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Age == 1f)
            {
                SoundEngine.PlaySound(SoundID.Item34 with { Volume = 0.35f, Pitch = -0.3f, PitchVariance = 0.1f }, Projectile.Center);
                SapheadVFX.Burst(Projectile.Center, 7, 2.5f);
                for (int i = 0; i < 3; i++)
                    SapheadVFX.Smoke(Projectile.Center, Projectile.velocity.RotatedBy((i - 1) * 0.5f), 32f, SapheadVFX.Aqua * 0.7f);
            }
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.018f;
            if (Age % 5f == 0f && Age < 48f)
                SapheadVFX.Spark(Projectile.Center + Main.rand.NextVector2Circular(Radius * 0.8f, Radius * 0.8f), Main.rand.NextVector2Circular(0.7f, 0.7f), 0.15f * Opacity);
            Lighting.AddLight(Projectile.Center, SapheadVFX.Aqua.ToVector3() * 0.3f * Opacity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float seed = Projectile.identity * 2.39996f;
            SapheadVFX.Glow(Projectile.Center, new Vector2(Radius * 2.9f), SapheadVFX.Violet, Opacity * 0.25f);
            for (int i = 0; i < 4; i++)
            {
                float rotation = seed + i * 1.7f + Projectile.rotation * (i % 2 == 0 ? 1f : -1f);
                Vector2 offset = rotation.ToRotationVector2() * Radius * 0.24f;
                Color tint = Color.Lerp(SapheadVFX.Violet, SapheadVFX.Aqua, i / 3f);
                SapheadVFX.Sprite(i % 2 == 0 ? Texture : "AerovelenceMod/Assets/Smoke/smoke_02", Projectile.Center + offset,
                    new Vector2(Radius * (2f + i * 0.13f)), SapheadVFX.Additive(tint, Opacity * (0.26f + i * 0.035f)), rotation);
            }
            for (int i = 0; i < 4; i++)
            {
                float angle = seed + i * MathHelper.PiOver2 - Age * 0.025f;
                Vector2 center = Projectile.Center + angle.ToRotationVector2() * Radius * (0.35f + 0.12f * MathF.Sin(Age * 0.08f + i));
                SapheadVFX.Crystal(center, angle + Age * 0.03f, new Vector2(3f, 8f), Opacity * 0.7f, 0.3f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
                SapheadVFX.Burst(Projectile.Center, 6, 1.5f);
        }
    }

    internal sealed class SapheadOrbit
    {
        internal float Radius = 14f;
        internal float Charge;
        internal int Blooms;
        internal bool Ready => Charge >= 0.99999f;
        internal bool Prepaid => Blooms == 0;
        internal static float Interval(float radius) => 30f + 65f * Math.Clamp((radius - 44f) / 116f, 0f, 1f);

        internal void Advance(float target, bool returning)
        {
            if (returning)
            {
                Radius *= 0.84f;
                Charge *= 0.88f;
                return;
            }
            Radius += Math.Clamp((Math.Clamp(target, 24f, 160f) - Radius) * 0.065f, -4f, 4f);
            Charge = Math.Min(1f, Charge + 1f / Interval(Radius));
        }

        internal void Bloom()
        {
            Charge = 0f;
            Blooms++;
        }

        internal static float FogRadius(float age)
        {
            float growth = Math.Clamp(age / 12f, 0f, 1f);
            return 8f + 28f * growth * (2f - growth);
        }

        internal static bool FogCanDamage(float age) => age >= 5f && age < 38f;
    }

    internal static class SapheadVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);

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
    }
}
