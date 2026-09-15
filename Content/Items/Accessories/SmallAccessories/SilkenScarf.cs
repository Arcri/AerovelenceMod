using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class SilkenScarf : TranslatableModItem
    {
        internal const string RibbonTexture = "AerovelenceMod/Content/Items/Accessories/SmallAccessories/SilkenScarf";
        public override string Texture => "AerovelenceMod/Content/Items/Accessories/SmallAccessories/SilkenScarf";
        private const string EnglishTooltip = "Slightly reduces enemy aggression\nRestores 1 mana each second for each summoned minion\nAn ancient scarf of crystal moth silk, interlaced with crystal fibers\n'The moths refuse to eat it. Apart from the dust, it looks perfectly fine!'";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Silken Scarf", EnglishTooltip)
                .AddName(Language.Spanish, "Bufanda de Seda")
                .AddTooltip(Language.Spanish, "Reduce ligeramente la agresividad de los enemigos\nRestaura 1 de maná por segundo por cada esbirro invocado\nUna antigua bufanda de seda de polilla de cristal, entretejida con fibras de cristal\nLas polillas se niegan a comérsela. ¡Aparte del polvo, está como nueva!");
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
            Item.width = 22;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.aggro -= 100;
            var state = player.GetModPlayer<SilkenScarfPlayer>();
            state.Equipped = true;
            state.Visible |= !hideVisual;
        }


    }

    public class SilkenScarfPlayer : ModPlayer
    {
        public bool Equipped;
        public bool Visible;
        internal float Pulse;
        private readonly ScarfManaClock clock = new();
        public override void ResetEffects() => Equipped = Visible = false;
        public override void UpdateDead()
        {
            clock.Update(false, false);
            Pulse = 0f;
        }
        public override void PostUpdate()
        {
            Pulse *= 0.9f;
            if (!clock.Update(Equipped, !Player.dead) || Player.whoAmI != Main.myPlayer)
                return;
            int count = 0;
            foreach (Projectile minion in Main.ActiveProjectiles)
                if (IsMinion(minion))
                    count++;
            int restored = ScarfManaClock.Restoration(count, Player.statMana, Player.statManaMax2);
            if (restored == 0)
                return;
            Player.statMana += restored;
            Player.ManaEffect(restored);
            Pulse = 1f;
            if (!Visible)
                return;
            int motes = 0;
            foreach (Projectile minion in Main.ActiveProjectiles)
            {
                if (!IsMinion(minion) || Vector2.DistanceSquared(minion.Center, Player.Center) > 900f * 900f)
                    continue;
                Projectile.NewProjectile(Player.GetSource_FromThis(), minion.Center, Vector2.Zero, ModContent.ProjectileType<SilkenScarfMote>(), 0, 0f, Player.whoAmI);
                if (++motes >= Math.Min(4, restored))
                    break;
            }
        }
        private bool IsMinion(Projectile projectile)
            => projectile.owner == Player.whoAmI && projectile.minion && projectile.minionSlots > 0f;
    }

    public class SilkenScarfLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.NeckAcc);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var state = player.GetModPlayer<SilkenScarfPlayer>();
            if (!state.Equipped || !state.Visible || player.dead || player.invis || drawInfo.shadow != 0f)
                return;
            Texture2D scarf = ModContent.Request<Texture2D>(SilkenScarf.RibbonTexture).Value;
            Vector2 neck = drawInfo.Position + (player.MountedCenter - player.position) + player.bodyPosition - Main.screenPosition + new Vector2(0f, -4f * player.gravDir);
            Color cloth = Color.Lerp(drawInfo.colorArmorBody, Color.White, 0.3f);
            SpriteEffects flip = player.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (player.gravDir < 0f) flip |= SpriteEffects.FlipVertically;
            float rotation = player.bodyRotation;
            Vector2 tail = neck + new Vector2(-3f * player.direction, 4.5f * player.gravDir).RotatedBy(rotation);
            for (int i = 0; i < 4; i++)
            {
                float flutter = MathF.Sin(Main.GlobalTimeWrappedHourly * 5f - i * 0.7f) * (0.3f + Math.Abs(player.velocity.X) * 0.1f);
                Vector2 delta = new Vector2(-player.velocity.X * (0.1f + i * 0.025f) + flutter, 3.5f * player.gravDir).RotatedBy(rotation);
                Rectangle strip = new(2, 16 + i * 3, 10, 3);
                float angle = delta.ToRotation() - player.gravDir * MathHelper.PiOver2;
                Vector2 origin = new(5f, player.gravDir > 0f ? 0f : 3f);
                Vector2 scale = new(0.75f, (delta.Length() + 0.5f) / strip.Height);
                drawInfo.DrawDataCache.Add(new DrawData(scarf, tail, strip, cloth, angle, origin, scale, flip));
                drawInfo.DrawDataCache.Add(new DrawData(scarf, tail, strip, SilkenScarfVFX.Additive(Color.White, state.Pulse * 0.3f), angle, origin, scale, flip));
                tail += delta;
            }
            Rectangle collar = new(0, 0, scarf.Width, 16);
            Vector2 collarOrigin = new(scarf.Width * 0.5f, player.gravDir > 0f ? 10f : 6f);
            drawInfo.DrawDataCache.Add(new DrawData(scarf, neck, collar, cloth, rotation, collarOrigin, 0.75f, flip));
            drawInfo.DrawDataCache.Add(new DrawData(scarf, neck, collar, SilkenScarfVFX.Additive(Color.White, state.Pulse * 0.3f), rotation, collarOrigin, 0.75f, flip));
        }
    }

    public class SilkenScarfMote : ModProjectile
    {
        private Vector2 start;
        private bool initialized;
        public override string Texture => SilkenScarfVFX.CrystalTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            var state = player.GetModPlayer<SilkenScarfPlayer>();
            if (!player.active || player.dead || !state.Equipped || !state.Visible)
            {
                Projectile.Opacity *= 0.78f;
                if (Projectile.Opacity < 0.03f)
                    Projectile.Kill();
                return;
            }
            if (!initialized)
            {
                start = Projectile.Center;
                initialized = true;
            }
            Projectile.ai[0]++;
            float progress = MathHelper.Clamp(Projectile.ai[0] / 30f, 0f, 1f);
            Vector2 target = player.MountedCenter + player.bodyPosition + new Vector2(0f, -4f * player.gravDir);
            float ease = progress * progress * (3f - 2f * progress);
            Projectile.Center = Vector2.Lerp(start, target, ease) - Vector2.UnitY * MathF.Sin(progress * MathHelper.Pi) * 25f;
            if (Projectile.ai[0] % 5f == 0f)
                SilkenScarfVFX.Spark(Projectile.Center, Vector2.Zero, 0.08f);
            if (Projectile.ai[0] == 28f)
                state.Pulse = Math.Max(state.Pulse, 0.65f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = MathHelper.Clamp(Projectile.ai[0] / 5f, 0f, 1f) * Math.Min(1f, Projectile.timeLeft / 6f) * Projectile.Opacity;
            SilkenScarfVFX.Glow(Projectile.Center, new Vector2(24f), SilkenScarfVFX.Aqua, fade * 0.4f);
            SilkenScarfVFX.Crystal(Projectile.Center, Projectile.ai[0] * 0.06f, new Vector2(3f, 6f), fade, 0.4f);
            return false;
        }
    }

    internal sealed class ScarfManaClock
    {
        private int ticks;
        internal bool Update(bool equipped, bool alive)
        {
            if (!equipped || !alive)
            {
                ticks = 0;
                return false;
            }
            if (++ticks < 60)
                return false;
            ticks = 0;
            return true;
        }
        internal static int Restoration(int minions, int mana, int maximum)
            => Math.Max(0, Math.Min(minions, maximum - mana));
    }

    internal static class SilkenScarfVFX
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
    }
}
