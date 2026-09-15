using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class RepurposedWinch : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Repurposed Winch", "Whips can no longer autoswing\nHold use to wind up, then release to attack\nQuick attacks have slightly reduced range\nCharging increases whip range and damage, up to 35% range and 50% damage");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 90);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RepurposedWinchPlayer state = player.GetModPlayer<RepurposedWinchPlayer>();
            state.Equipped = true;
            state.Visible |= !hideVisual;
        }
    }

    internal static class RepurposedWinchWhipUtil
    {
        internal static bool IsWhip(Item item)
        {
            return item != null && item.shoot > 0 && item.shoot < ProjectileID.Sets.IsAWhip.Length && ProjectileID.Sets.IsAWhip[item.shoot];
        }

        internal static bool IsWhip(Projectile projectile)
        {
            return projectile.type > 0 && projectile.type < ProjectileID.Sets.IsAWhip.Length && ProjectileID.Sets.IsAWhip[projectile.type];
        }
    }

    public class RepurposedWinchPlayer : ModPlayer
    {
        internal const int ChargeDuration = 40;
        internal const float MinimumRangeMultiplier = 0.9f;
        internal const float MaximumRangeMultiplier = 1.35f;
        internal const float MaximumDamageMultiplier = 1.5f;

        public bool Equipped;
        public bool Visible;
        internal int ChargeTicks;
        internal int GlowTime;
        internal float GlowFlash;
        internal float WhipRotation;
        internal float AttackCharge;
        internal bool ReleaseFireThisTick;
        private bool charging;
        private bool fullChargePlayed;
        private int chargingItemType = -1;
        private int releaseEffectCooldown;

        internal float ChargeProgress => MathHelper.Clamp(ChargeTicks / (float)ChargeDuration, 0f, 1f);
        internal bool FullyCharged => ChargeTicks >= ChargeDuration;
        internal bool Charging => charging;

        public override void ResetEffects()
        {
            Equipped = false;
            Visible = false;
        }

        public override void UpdateDead()
        {
            CancelCharge();
            ReleaseFireThisTick = false;
            AttackCharge = 0f;
            GlowTime = 0;
            GlowFlash = 0f;
            releaseEffectCooldown = 0;
        }

        public override bool PreItemCheck()
        {
            if (Player.whoAmI != Main.myPlayer)
                return true;

            Item item = Player.HeldItem;
            bool validWhip = Equipped && RepurposedWinchWhipUtil.IsWhip(item) && !Player.dead && !Player.CCed && !Player.noItems;

            if (!validWhip)
            {
                CancelCharge();
                return true;
            }

            if (chargingItemType != -1 && chargingItemType != item.type)
                CancelCharge();

            bool useHeld = Player.controlUseItem;

            if (useHeld)
            {
                if (!charging && Player.itemAnimation <= 0 && Player.itemTime <= 0)
                    BeginCharge(item);

                if (charging)
                {
                    UpdateAim();
                    if (ChargeTicks < ChargeDuration)
                        ChargeTicks++;
                    CheckFullCharge();
                    Player.controlUseItem = false;
                }

                return true;
            }

            if (charging)
            {
                UpdateAim();
                ArmRelease();
                Player.controlUseItem = true;
                Player.releaseUseItem = true;
            }

            return true;
        }

        public override void PostItemCheck()
        {
            if (Player.whoAmI == Main.myPlayer && ReleaseFireThisTick)
            {
                Player.controlUseItem = false;
                ReleaseFireThisTick = false;
                ChargeTicks = 0;
                fullChargePlayed = false;
                chargingItemType = -1;
            }
        }

        public override void PostUpdate()
        {
            if (releaseEffectCooldown > 0)
                releaseEffectCooldown--;
            if (GlowTime > 0)
                GlowTime--;
            GlowFlash *= 0.86f;
            UpdateVisuals();
        }

        private void BeginCharge(Item item)
        {
            charging = true;
            chargingItemType = item.type;
            ChargeTicks = 0;
            AttackCharge = 0f;
            fullChargePlayed = false;
            UpdateAim();
        }

        private void ArmRelease()
        {
            AttackCharge = ChargeProgress;
            ReleaseFireThisTick = true;
            charging = false;
            GlowTime = 10;
            GlowFlash = Math.Max(GlowFlash, 0.18f + AttackCharge * 0.22f);
        }

        private void CheckFullCharge()
        {
            if (!FullyCharged || fullChargePlayed)
                return;
            fullChargePlayed = true;
            GlowFlash = 0.45f;
            GlowTime = 12;
            Vector2 hand = GetHandPosition();
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.MaxMana with { Volume = 0.045f, Pitch = 0.55f, PitchVariance = 0.04f, MaxInstances = 1 }, hand);
                if (Visible)
                    RepurposedWinchVFX.Burst(hand, 4, 1.2f);
            }
        }

        private void UpdateAim()
        {
            Vector2 direction = (Main.MouseWorld - Player.MountedCenter).SafeNormalize(Vector2.UnitX * Player.direction);
            WhipRotation = direction.ToRotation();
            Player.ChangeDir(direction.X >= 0f ? 1 : -1);
        }

        private void UpdateVisuals()
        {
            float residual = MathHelper.Clamp(GlowTime / 12f, 0f, 1f);
            float intensity = Math.Max(charging ? ChargeProgress : 0f, residual);
            if (intensity <= 0.01f || !Visible || Main.dedServ)
                return;
            Vector2 hand = GetHandPosition();
            Lighting.AddLight(hand, Vector3.One * (0.02f + intensity * 0.08f));
            int dustChance = FullyCharged && charging ? 5 : ChargeTicks > ChargeDuration / 2 && charging ? 8 : 12;
            if (charging && Main.rand.NextBool(dustChance))
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(8f, 8f);
                Vector2 position = hand + offset;
                Vector2 velocity = (-offset).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.25f, 0.6f) + Main.rand.NextVector2Circular(0.1f, 0.1f);
                RepurposedWinchVFX.Spark(position, velocity, Main.rand.NextFloat(0.06f, 0.11f));
            }
        }

        internal bool HasReleaseAttack()
        {
            return Equipped && ReleaseFireThisTick;
        }

        internal float RangeMultiplier()
        {
            return MathHelper.Lerp(MinimumRangeMultiplier, MaximumRangeMultiplier, AttackCharge);
        }

        internal float DamageMultiplier()
        {
            return MathHelper.Lerp(1f, MaximumDamageMultiplier, AttackCharge);
        }

        internal void TriggerReleaseEffect(Vector2 direction)
        {
            WhipRotation = direction.SafeNormalize(Vector2.UnitX * Player.direction).ToRotation();
            GlowTime = Math.Max(GlowTime, 8);
            GlowFlash = Math.Max(GlowFlash, 0.16f + AttackCharge * 0.18f);

            if (releaseEffectCooldown > 0 || Main.dedServ)
                return;

            releaseEffectCooldown = 5;
            if (Visible)
                RepurposedWinchVFX.Burst(GetHandPosition(), 3, 1.35f);
        }

        internal Vector2 GetHandPosition()
        {
            return Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, WhipRotation - MathHelper.PiOver2) + new Vector2(0f, Player.gfxOffY);
        }

        private void CancelCharge()
        {
            charging = false;
            ChargeTicks = 0;
            fullChargePlayed = false;
            chargingItemType = -1;
            AttackCharge = 0f;
            ReleaseFireThisTick = false;
        }
    }

    public class RepurposedWinchGlobalItem : GlobalItem
    {
        public override bool? CanAutoReuseItem(Item item, Player player)
        {
            if (player.GetModPlayer<RepurposedWinchPlayer>().Equipped && RepurposedWinchWhipUtil.IsWhip(item))
                return false;

            return null;
        }
    }

    public class RepurposedWinchGlobalProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!RepurposedWinchWhipUtil.IsWhip(projectile) || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            RepurposedWinchPlayer state = player.GetModPlayer<RepurposedWinchPlayer>();
            if (!state.HasReleaseAttack())
                return;

            projectile.WhipSettings.RangeMultiplier *= state.RangeMultiplier();
            float damageMultiplier = state.DamageMultiplier();
            projectile.damage = Math.Max(1, (int)MathF.Round(projectile.damage * damageMultiplier));
            if (projectile.originalDamage > 0)
                projectile.originalDamage = Math.Max(1, (int)MathF.Round(projectile.originalDamage * damageMultiplier));

            state.TriggerReleaseEffect(projectile.velocity);
        }
    }

    public class RepurposedWinchHandGlowLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            RepurposedWinchPlayer state = player.GetModPlayer<RepurposedWinchPlayer>();
            if (drawInfo.shadow != 0f || player.dead || player.invis || !state.Visible)
                return;
            float residual = MathHelper.Clamp(state.GlowTime / 12f, 0f, 1f);
            float intensity = Math.Max(state.Charging ? state.ChargeProgress : 0f, residual);
            if (intensity <= 0.01f)
                return;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            Vector2 position = state.GetHandPosition() - Main.screenPosition;
            float pulse = 0.96f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.04f;
            float scale = MathHelper.Lerp(0.055f, 0.13f, intensity) * pulse;
            Color outer = RepurposedWinchVFX.Additive(Color.White, 0.02f + intensity * 0.045f + state.GlowFlash * 0.035f);
            Color inner = RepurposedWinchVFX.Additive(new Color(245, 250, 255), 0.035f + intensity * 0.075f + state.GlowFlash * 0.05f);
            drawInfo.DrawDataCache.Add(new DrawData(glow, position, null, outer, 0f, glow.Size() * 0.5f, scale * 1.8f, SpriteEffects.None));
            drawInfo.DrawDataCache.Add(new DrawData(glow, position, null, inner, 0f, glow.Size() * 0.5f, scale, SpriteEffects.None));
        }
    }

    internal static class RepurposedWinchVFX
    {
        internal static Color Additive(Color color, float opacity)
        {
            color *= MathHelper.Clamp(opacity, 0f, 1f);
            color.A = 0;
            return color;
        }

        internal static void Spark(Vector2 position, Vector2 velocity, float scale)
        {
            if (Main.dedServ)
                return;

            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(), velocity, 0, Color.White, scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, timeBeforeSlow: 5, preSlowPower: 0.93f,
                postSlowPower: 0.85f, velToBeginShrink: 0.65f, fadePower: 0.86f, shouldFadeColor: false);
        }

        internal static void Burst(Vector2 position, int count, float speed)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < count; i++)
                Spark(position + Main.rand.NextVector2Circular(2f, 2f), Main.rand.NextVector2CircularEdge(speed, speed) * Main.rand.NextFloat(0.3f, 0.85f), Main.rand.NextFloat(0.07f, 0.12f));
        }
    }
}