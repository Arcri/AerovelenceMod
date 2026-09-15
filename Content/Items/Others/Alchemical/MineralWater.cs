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
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Others.Alchemical
{
    public class MineralWater : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Alchemical/MineralWater";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Mineral Water", "Restores 60 life with 30 seconds of potion sickness\nGrants 40 defense for 30 seconds, but inflicts mineral poisoning for 12 seconds\nPoisoning drains 10 life per second\nContains enough sediment to cause stomach or throat stones. Worse than a kidney stone.")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Agua Mineral")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Restaura 60 de vida con 30 segundos de enfermedad de poción\nOtorga 40 de defensa durante 30 segundos, pero causa envenenamiento mineral durante 12 segundos\nEl veneno drena 10 de vida por segundo\nContiene suficientes sedimentos para formar piedras en el estómago o la garganta.");
            
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Restores 60 life with 30 seconds of potion sickness\nGrants 40 defense for 30 seconds, but inflicts mineral poisoning for 12 seconds\nPoisoning drains 10 life per second\nContains enough sediment to cause stomach or throat stones. Worse than a kidney stone."));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;

            Item.value = Item.sellPrice(silver: 1);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
            Item.healLife = 60;
            Item.potion = true;
            Item.potionDelay = 1800;
            
        }
        public override bool? UseItem(Player player)
        {
            MineralWaterVFX.Burst(player.Center, 12, 2.5f);
            player.AddBuff(ModContent.BuffType<MineralPoisoning>(), 720);
            player.AddBuff(ModContent.BuffType<MineralBoost>(), 1800);
            return true;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient<CavernCrystalItem>().AddTile(TileID.Bottles).Register();
        

    }
    public class MineralPoisoning : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Alchemical/MineralPoisoning";
        public override LocalizedText DisplayName => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MineralPoisoning.DisplayName", () => "Mineral Poisoning");
        public override LocalizedText Description => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MineralPoisoning.Description", () => "Too many to list. Assume you are being poisoned by every possible mineral.");
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MineralWaterPlayer>().Poisoned = true;
    }
    public class MineralBoost : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Alchemical/MineralWaterBoost";
        public override LocalizedText DisplayName => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MineralBoost.DisplayName", () => "Mineral Boost");
        public override LocalizedText Description => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MineralBoost.Description", () => "40 increased defense");
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 40;
            if (!Main.dedServ && Main.GameUpdateCount % 18 == 0)
            {
                Dust dust = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(14, 22), ModContent.DustType<GlowPixelCross>(), new Vector2(0, -.7f), newColor: new Color(110, 215, 235), Scale: .2f);
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: .1f, timeBeforeSlow: 8, preSlowPower: .94f, postSlowPower: .87f, velToBeginShrink: 1, fadePower: .87f, shouldFadeColor: false);
            }
        }
    }
    public class MineralWaterPlayer : ModPlayer
    {
        public bool Poisoned;
        public override void ResetEffects() => Poisoned = false;
        public override void UpdateBadLifeRegen()
        {
            if (!Poisoned) return;
            if (Player.lifeRegen > 0) Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= 20;
        }
    }

    internal static class MineralWaterVFX
    {
        internal static readonly Color Aqua = new(85, 218, 255);

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
