using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
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
    public class PetrifiedShackles : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Accessories/SmallAccessories/PetrifiedShackles";
        private string EnglishName => "Petrified Shackles";
        private string EnglishTooltip => "Grants 1 defense and reduces attack speed by 10%\nGrows 1 defense every 7 seconds in combat, up to 15\nOutside combat, grows 1 defense every 4 seconds, up to 25\nEntering combat sheds growth above 15; bosses keep you in combat\nCannot be equipped with Band of Crystallization";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 34;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.accessory = true;
            Item.defense = 1;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) *= .9f;
            var state = player.GetModPlayer<PetrifiedShacklesPlayer>();
            state.Equipped = true;
            state.Visible = !hideVisual;
        }


        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Shackle).AddIngredient<BandOfCrystallization>().AddTile(TileID.TinkerersWorkbench).Register();

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) => equippedItem.type != ModContent.ItemType<BandOfCrystallization>() && incomingItem.type != ModContent.ItemType<BandOfCrystallization>();

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization(EnglishName, EnglishTooltip)
                .AddName(Language.Spanish, "Grilletes Petrificados")
                .AddTooltip(Language.Spanish, "Otorga 1 de defensa y reduce un 10% la velocidad de ataque\nGana 1 de defensa cada 7 segundos en combate, hasta 15\nFuera de combate, gana 1 cada 4 segundos, hasta 25\nEntrar en combate elimina la defensa que supere 15; los jefes mantienen el combate\nNo se puede combinar con la Banda de Cristalización");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
            if (!Main.gameMenu && Main.LocalPlayer.GetModPlayer<PetrifiedShacklesPlayer>().Equipped)
            {
                int defense = Main.LocalPlayer.GetModPlayer<PetrifiedShacklesPlayer>().Defense;
                string text = Terraria.Localization.Language.ActiveCulture.Name == "es-ES" ? $"Protección acumulada: +{defense} de defensa" : $"Grown protection: +{defense} defense";
                tooltips.Add(new TooltipLine(Mod, "CrystalDefense", text) { OverrideColor = PetrifiedShacklesVFX.Aqua });
            }
        }
    }

    public class PetrifiedShacklesPlayer : ModPlayer
    {
        public bool Equipped, Visible;
        private int combatTicks;
        private readonly PetrifiedShacklesCrystallizationGrowth growth = new();
        internal int Defense => growth.Defense;
        internal float DisplayDefense, VisualOpacity, GrowthFlash;

        public override void ResetEffects() { Equipped = Visible = false; }
        public override void UpdateDead() { combatTicks = 0; growth.Update(false, false); DisplayDefense = VisualOpacity = GrowthFlash = 0f; }
        public override void OnHurt(Player.HurtInfo info) => combatTicks = 600;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && target.type != NPCID.TargetDummy) combatTicks = 600;
        }
        public override void PostUpdateEquips()
        {
            bool combat = combatTicks > 0;
            if (combatTicks > 0) combatTicks--;
            if (Equipped)
                foreach (NPC npc in Main.ActiveNPCs)
                    if (npc.boss) { combat = true; break; }
            int change = growth.Update(Equipped, combat, 15, 25, 420, 240);
            if (Equipped) Player.statDefense += Defense;
            if (change != 0 && !Main.dedServ && (Visible || VisualOpacity > 0.5f))
            {
                GrowthFlash = change > 0 ? 1f : 0f;
                PetrifiedShacklesVFX.Burst(Player.Center, Math.Min(18, Math.Abs(change) * 3 + 3), change > 0 ? 1.5f : 3.5f);
                SoundEngine.PlaySound(change > 0 ? SoundID.Item4 with { Volume = 0.18f, Pitch = 0.3f } : SoundID.Shatter with { Volume = 0.25f }, Player.Center);
            }
            DisplayDefense = MathHelper.Lerp(DisplayDefense, Defense, 0.12f);
            VisualOpacity = MathHelper.Lerp(VisualOpacity, Equipped && Visible ? 1f : 0f, 0.12f);
            GrowthFlash *= 0.9f;
        }
        
    }

    internal sealed class PetrifiedShacklesCrystallizationGrowth
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

    internal static class PetrifiedShacklesVFX
    {
        internal static readonly Color Violet = new(115, 105, 235);
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
    public class PetrifiedShacklesLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var state = player.GetModPlayer<PetrifiedShacklesPlayer>();
            if (drawInfo.shadow != 0f || player.dead || player.invis || state.VisualOpacity < 0.01f)
                return;
            Texture2D crystal = ModContent.Request<Texture2D>(PetrifiedShacklesVFX.CrystalTexture).Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            Vector2 center = drawInfo.Position + player.Size * 0.5f - Main.screenPosition;
            for (int i = 0; i < 6; i++)
            {
                float growth = Math.Clamp((state.DisplayDefense - i * 4f) / 4f, 0f, 1f);
                float opacity = growth * state.VisualOpacity;
                Vector2 offset = new((i % 2 == 0 ? -1f : 1f) * (13f + i * 0.8f), (6f - i / 2 * 8f) * player.gravDir);
                float angle = (i % 2 == 0 ? -1f : 1f) * (0.3f + i * 0.09f);
                float pulse = 0.65f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + i) * 0.15f;
                drawInfo.DrawDataCache.Add(new DrawData(glow, center + offset, null, PetrifiedShacklesVFX.Additive(PetrifiedShacklesVFX.Violet, opacity * pulse * 0.3f), 0f,
                    glow.Size() * 0.5f, 30f / glow.Width, SpriteEffects.None));
                drawInfo.DrawDataCache.Add(new DrawData(crystal, center + offset, null, Color.White * opacity, angle, crystal.Size() * 0.5f, 0.3f + growth * 0.35f, SpriteEffects.None));
                drawInfo.DrawDataCache.Add(new DrawData(crystal, center + offset, null, PetrifiedShacklesVFX.Additive(Color.White, opacity * state.GrowthFlash), angle,
                    crystal.Size() * 0.5f, 0.3f + growth * 0.35f, SpriteEffects.None));
            }
        }
    }
}
