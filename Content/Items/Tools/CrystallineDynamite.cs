using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Content.Items.Tools
{
    public class CrystallineDynamite : TranslatableModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Dynamite}";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Crystalline Dynamite", "A reusable stick of dynamite\nThe blast scatters crystal fragments that remagnetize toward you\nReforms 30 seconds after being thrown")
                .AddName(Language.Default, "Crystalline Dynamite")
                .AddTooltip(Language.Default, "A reusable stick of dynamite\nThe blast scatters crystal fragments that remagnetize toward you\nReforms 30 seconds after being thrown")
                .AddName(Language.Spanish, "Dinamita Cristalina")
                .AddTooltip(Language.Spanish, "Una dinamita reutilizable\nLa explosión dispersa fragmentos que vuelven a magnetizarse hacia ti\nSe reforma 30 segundos después de lanzarse");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Dynamite);
            Item.maxStack = 1;
            Item.consumable = false;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ProjectileID.Dynamite;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<CrystallineDynamitePlayer>().Cooldown <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CrystallineDynamitePlayer state = player.GetModPlayer<CrystallineDynamitePlayer>();
            state.Cooldown = CrystallineDynamitePlayer.CooldownDuration;

            int index = Projectile.NewProjectile(source, position, velocity, ProjectileID.Dynamite, damage, knockback, player.whoAmI);
            if (index >= 0 && index < Main.maxProjectiles)
                Main.projectile[index].GetGlobalProjectile<CrystallineDynamiteGlobalProjectile>().Crystalline = true;

            if (!Main.dedServ)
            {
                CrystallineDynamiteVFX.Burst(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, velocity.ToRotation() - MathHelper.PiOver2), 6, 1.8f);
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.12f, Pitch = 0.45f, PitchVariance = 0.08f }, player.Center);
            }

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.gameMenu)
            {
                int cooldown = Main.LocalPlayer.GetModPlayer<CrystallineDynamitePlayer>().Cooldown;
                if (cooldown > 0)
                {
                    float seconds = cooldown / 60f;
                    tooltips.Add(new TooltipLine(Mod, "CrystallineCooldown", $"Reforming: {seconds:0.0}s") { OverrideColor = CrystallineDynamiteVFX.CrystalBlue });
                }
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.gameMenu || Main.LocalPlayer.GetModPlayer<CrystallineDynamitePlayer>().Cooldown <= 0)
                return true;

            Texture2D texture = TextureAssets.Item[ItemID.Dynamite].Value;
            float time = Main.GlobalTimeWrappedHourly;
            float pulse = 0.7f + MathF.Sin(time * 5f) * 0.12f;
            Vector2 drift = new Vector2(MathF.Cos(time * 3.2f), MathF.Sin(time * 4.1f)) * 1.2f;
            Color ghostOuter = new Color(85, 205, 255, 0) * (0.18f * pulse);
            Color ghostInner = new Color(215, 245, 255, 120) * (0.48f + pulse * 0.12f);
            spriteBatch.Draw(texture, position + drift, frame, ghostOuter, 0f, origin, scale * 1.08f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - drift * 0.45f, frame, ghostOuter, 0f, origin, scale * 0.98f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, frame, ghostInner, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.gameMenu)
                return;

            int cooldown = Main.LocalPlayer.GetModPlayer<CrystallineDynamitePlayer>().Cooldown;
            if (cooldown <= 0)
                return;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            float pulse = 0.75f + MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.15f;
            Color color = CrystallineDynamiteVFX.Additive(CrystallineDynamiteVFX.CrystalBlue, 0.08f * pulse);
            spriteBatch.Draw(glow, position, null, color, 0f, glow.Size() * 0.5f, 0.18f * scale, SpriteEffects.None, 0f);
        }
    }

    public class CrystallineDynamitePlayer : ModPlayer
    {
        internal const int CooldownDuration = 1800;
        internal int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown <= 0)
                return;

            Cooldown--;
            if (Cooldown == 0 && Player.whoAmI == Main.myPlayer && !Main.dedServ)
            {
                CrystallineDynamiteVFX.Burst(Player.Center, 14, 3.2f);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.22f, Pitch = 0.55f, PitchVariance = 0.05f }, Player.Center);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (Cooldown > 0)
                tag["CrystallineDynamiteCooldown"] = Cooldown;
        }

        public override void LoadData(TagCompound tag)
        {
            Cooldown = Math.Max(0, tag.GetInt("CrystallineDynamiteCooldown"));
        }
    }

    public class CrystallineDynamiteGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        internal bool Crystalline;

        public override void AI(Projectile projectile)
        {
            if (!Crystalline || projectile.owner != Main.myPlayer || Main.dedServ)
                return;

            Lighting.AddLight(projectile.Center, CrystallineDynamiteVFX.CrystalBlue.ToVector3() * 0.22f);
            if (Main.rand.NextBool(5))
            {
                Vector2 velocity = -projectile.velocity * Main.rand.NextFloat(0.02f, 0.08f) + Main.rand.NextVector2Circular(0.25f, 0.25f);
                CrystallineDynamiteVFX.Spark(projectile.Center + Main.rand.NextVector2Circular(5f, 5f), velocity, Main.rand.NextFloat(0.08f, 0.14f));
            }
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (!Crystalline || projectile.owner != Main.myPlayer)
                return;

            if (!Main.dedServ)
            {
                CrystallineDynamiteVFX.Burst(projectile.Center, 24, 5.5f);
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.28f, Pitch = 0.3f, PitchVariance = 0.12f }, projectile.Center);
            }

            for (int i = 0; i < 18; i++)
            {
                float angle = MathHelper.TwoPi * i / 18f + Main.rand.NextFloat(-0.14f, 0.14f);
                Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(3.8f, 8.5f);
                velocity.Y -= Main.rand.NextFloat(0.5f, 2.4f);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity,
                    ModContent.ProjectileType<CrystallineDynamiteShard>(), 0, 0f, projectile.owner,
                    Main.rand.NextFloat(20f, 34f), Main.rand.NextFloat(0f, MathHelper.TwoPi), Main.rand.NextFloat(0.34f, 0.68f));
            }
        }
    }

    public class CrystallineDynamiteShard : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.localAI[0]++;
            float age = Projectile.localAI[0];
            float returnDelay = Projectile.ai[0];
            float phase = Projectile.ai[1];
            Projectile.scale = Projectile.ai[2] <= 0f ? 0.5f : Projectile.ai[2];
            Projectile.rotation += 0.08f + Projectile.velocity.X * 0.025f;
            if (age < returnDelay)
            {
                Projectile.velocity *= 0.965f;
                Projectile.velocity.Y += 0.045f;
            }
            else
            {
                float returnProgress = MathHelper.Clamp((age - returnDelay) / 45f, 0f, 1f);
                Vector2 orbit = new Vector2(MathF.Cos(age * 0.11f + phase), MathF.Sin(age * 0.13f + phase)) * MathHelper.Lerp(20f, 2f, returnProgress);
                Vector2 target = owner.MountedCenter + new Vector2(0f, owner.gfxOffY) + orbit;
                Vector2 desired = (target - Projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(5f, 17f, returnProgress);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.07f + returnProgress * 0.1f);
                if (Vector2.DistanceSquared(Projectile.Center, owner.MountedCenter + new Vector2(0f, owner.gfxOffY)) < 18f * 18f && returnProgress > 0.3f)
                {
                    if (!Main.dedServ)
                        CrystallineDynamiteVFX.Burst(Projectile.Center, 2, 0.8f);
                    Projectile.Kill();
                    return;
                }
            }
            Lighting.AddLight(Projectile.Center, CrystallineDynamiteVFX.CrystalBlue.ToVector3() * 0.13f);
            if (!Main.dedServ && Main.rand.NextBool(2))
            {
                Color color = Main.rand.NextBool() ? CrystallineDynamiteVFX.CrystalBlue : CrystallineDynamiteVFX.CrystalViolet;
                CrystallineDynamiteVFX.Spark(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(0.03f, 0.09f), Main.rand.NextFloat(0.06f, 0.12f), color);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D crystal = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            float pulse = 0.82f + MathF.Sin(Main.GlobalTimeWrappedHourly * 7f + Projectile.identity) * 0.12f;
            Color glowColor = CrystallineDynamiteVFX.Additive(CrystallineDynamiteVFX.CrystalBlue, 0.16f * pulse);
            Main.EntitySpriteDraw(glow, position, null, glowColor, 0f, glow.Size() * 0.5f, 0.2f * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(crystal, position, null, Color.Lerp(lightColor, Color.White, 0.6f), Projectile.rotation, crystal.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(crystal, position, null, CrystallineDynamiteVFX.Additive(Color.White, 0.18f * pulse), Projectile.rotation, crystal.Size() * 0.5f, Projectile.scale * 1.06f, SpriteEffects.None);
            return false;
        }
    }

    internal static class CrystallineDynamiteVFX
    {
        internal static readonly Color CrystalBlue = new(105, 225, 255);
        internal static readonly Color CrystalViolet = new(185, 125, 255);

        internal static Color Additive(Color color, float opacity)
        {
            color *= MathHelper.Clamp(opacity, 0f, 1f);
            color.A = 0;
            return color;
        }

        internal static void Spark(Vector2 position, Vector2 velocity, float scale, Color? color = null)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(), velocity, 0, color ?? CrystalBlue, scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.13f, timeBeforeSlow: 6, preSlowPower: 0.95f,
                postSlowPower: 0.87f, velToBeginShrink: 0.8f, fadePower: 0.88f, shouldFadeColor: false);
        }

        internal static void Burst(Vector2 position, int count, float speed)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
            {
                Color color = Main.rand.NextBool(3) ? CrystalViolet : CrystalBlue;
                Spark(position + Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextVector2CircularEdge(speed, speed) * Main.rand.NextFloat(0.35f, 1f), Main.rand.NextFloat(0.08f, 0.18f), color);
            }
        }
    }
}