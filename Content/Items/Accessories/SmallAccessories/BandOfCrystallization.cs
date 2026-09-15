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
    public class BandOfCrystallization : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Accessories/SmallAccessories/BandOfCrystallization";
        private const string EnglishTooltip = "Grows 1 defense every 10 seconds in combat, up to 10\nOutside combat, grows 1 defense every 5 seconds, up to 20\nEntering combat sheds defense above 10; bosses keep you in combat\nGlowing crystals show your growing protection\nRemoving the band sheds its crystals";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Band of Crystallization", EnglishTooltip)
                .AddName(Language.Spanish, "Banda de Cristalización")
                .AddTooltip(Language.Spanish, "Gana 1 de defensa cada 10 segundos en combate, hasta 10\nFuera de combate, gana 1 cada 5 segundos, hasta 20\nEntrar en combate elimina la defensa que supere 10; los jefes mantienen el combate\nLos cristales luminosos muestran tu protección acumulada\nQuitarte la banda elimina sus cristales");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
            if (!Main.gameMenu && Main.LocalPlayer.GetModPlayer<BandOfCrystallizationPlayer>().Equipped)
            {
                var state = Main.LocalPlayer.GetModPlayer<BandOfCrystallizationPlayer>();
                bool spanish = Terraria.Localization.Language.ActiveCulture.Name == "es-ES";
                string status = spanish ? $"Protección acumulada: +{state.Defense} de defensa" : $"Grown protection: +{state.Defense} defense";
                tooltips.Add(new TooltipLine(Mod, "CrystalDefense", status) { OverrideColor = BandOfCrystallizationVFX.Aqua });
            }
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
            var state = player.GetModPlayer<BandOfCrystallizationPlayer>();
            state.Equipped = true;
            state.Visible |= !hideVisual;
        }
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => equippedItem.type != ModContent.ItemType<PetrifiedShackles>() && incomingItem.type != ModContent.ItemType<PetrifiedShackles>();


    }

    public class BandOfCrystallizationPlayer : ModPlayer
    {
        public bool Equipped;
        public bool Visible;
        private readonly BandOfCrystallizationCrystallizationGrowth growth = new();
        private int combatTicks;
        internal int Defense => growth.Defense;
        internal float DisplayDefense;
        internal float VisualOpacity;
        internal float GrowthFlash;
        internal float NextGrowth => growth.Ticks / (growth.InCombat ? 600f : 300f);
        public override void ResetEffects() => Equipped = Visible = false;
        public override void UpdateDead()
        {
            growth.Update(false, false);
            combatTicks = 0;
            DisplayDefense = VisualOpacity = GrowthFlash = 0f;
        }
        public override void OnHurt(Player.HurtInfo info) => combatTicks = 600;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && target.type != NPCID.TargetDummy && damageDone > 0)
                combatTicks = 600;
        }
        public override void PostUpdateEquips()
        {
            bool combat = combatTicks > 0;
            if (combatTicks > 0)
                combatTicks--;
            if (Equipped)
                foreach (NPC npc in Main.ActiveNPCs)
                    if (npc.boss)
                    {
                        combat = true;
                        break;
                    }
            int change = growth.Update(Equipped, combat);
            if (Equipped)
                Player.statDefense += Defense;
            if (change != 0 && !Main.dedServ && (Visible || VisualOpacity > 0.5f))
            {
                if (change > 0)
                {
                    GrowthFlash = 1f;
                    BandOfCrystallizationVFX.Burst(Player.Center, 5, 1.5f);
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.15f, Pitch = 0.15f + Defense * 0.02f, MaxInstances = 1 }, Player.Center);
                }
                else
                {
                    BandOfCrystallizationVFX.Burst(Player.Center, Math.Min(16, -change * 2), 3.5f);
                    SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.22f, Pitch = 0.4f }, Player.Center);
                }
            }
            DisplayDefense = MathHelper.Lerp(DisplayDefense, Defense, 0.12f);
            VisualOpacity = MathHelper.Lerp(VisualOpacity, Equipped && Visible ? 1f : 0f, 0.12f);
            GrowthFlash *= 0.92f;
            if (Visible && Equipped && Defense > 0)
                Lighting.AddLight(Player.Center, BandOfCrystallizationVFX.Aqua.ToVector3() * (0.04f + Defense * 0.004f));
        }
    }

    public class CrystallizationLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var state = player.GetModPlayer<BandOfCrystallizationPlayer>();
            if (drawInfo.shadow != 0f || player.dead || player.invis || state.VisualOpacity < 0.01f || state.DisplayDefense < 0.05f)
                return;
            Texture2D crystal = ModContent.Request<Texture2D>(BandOfCrystallizationVFX.CrystalTexture).Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            Vector2 center = drawInfo.Position + player.Size * 0.5f - Main.screenPosition;
            float time = Main.GlobalTimeWrappedHourly;
            for (int i = 0; i < 5; i++)
            {
                float growth = MathHelper.Clamp((state.DisplayDefense - i * 4f) / 4f, 0f, 1f);
                if (growth <= 0f)
                    continue;
                Vector2 offset = new(-player.direction * (12f + MathF.Sin(i * 0.8f) * 7f), (-18f + i * 8f + MathF.Sin(time * 2f + i) * 1.5f) * player.gravDir);
                float angle = -player.direction * (0.3f + i * 0.12f);
                float scale = (0.2f + growth * 0.35f);
                float opacity = growth * state.VisualOpacity;
                float breath = 0.6f + MathF.Sin(time * 3f + i) * 0.12f + state.GrowthFlash * 0.6f;
                drawInfo.DrawDataCache.Add(new DrawData(glow, center + offset, null, BandOfCrystallizationVFX.Additive(BandOfCrystallizationVFX.Aqua, opacity * breath * 0.3f), 0f, glow.Size() * 0.5f, 28f / glow.Width, SpriteEffects.None));
                drawInfo.DrawDataCache.Add(new DrawData(crystal, center + offset, null, Color.White * opacity, angle, crystal.Size() * 0.5f, scale, SpriteEffects.None));
                drawInfo.DrawDataCache.Add(new DrawData(crystal, center + offset, null, BandOfCrystallizationVFX.Additive(Color.White, opacity * state.GrowthFlash), angle, crystal.Size() * 0.5f, scale, SpriteEffects.None));
            }
        }
    }

    internal sealed class BandOfCrystallizationCrystallizationGrowth
    {
        internal int Defense { get; private set; }
        internal int Ticks { get; private set; }
        internal bool InCombat { get; private set; }

        internal int Update(bool equipped, bool combat, int combatCap = 10, int idleCap = 20, int combatInterval = 600, int idleInterval = 300)
        {
            int previous = Defense;
            if (!equipped)
                Defense = Ticks = 0;
            else
            {
                int cap = combat ? combatCap : idleCap;
                Defense = Math.Min(Defense, cap);
                if (combat != InCombat)
                    Ticks = 0;
                if (Defense < cap && ++Ticks >= (combat ? combatInterval : idleInterval))
                {
                    Defense++;
                    Ticks = 0;
                }
            }
            InCombat = combat;
            return Defense - previous;
        }
    }

    internal static class BandOfCrystallizationVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal static readonly Color Aqua = new(85, 218, 255);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

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
