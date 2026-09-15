using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public abstract class RareOreCluster : TranslatableModItem
    {
        public abstract int RewardTier { get; }
        protected abstract int SpriteWidth { get; }
        protected abstract int SpriteHeight { get; }
        internal int RewardSilver => 20 + RewardTier * RewardTier * 5;
        internal int RewardCrystals => 2 + RewardTier;
        protected string EnglishTooltip => $"An unusually pure ore specimen\nTurn in to the Rock Collector for {RewardSilver} silver coins and {RewardCrystals} cavern crystals\nHold the specimen to choose it; favorite it to keep it";
        protected string SpanishTooltip => $"Una muestra de mineral extraordinariamente pura\nEntrégala al Coleccionista de Rocas por {RewardSilver} monedas de plata y {RewardCrystals} cristales de caverna\nSostén la muestra para elegirla; márcala como favorita para conservarla";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = SpriteWidth;
            Item.height = SpriteHeight;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = RewardTier >= 10 ? ItemRarityID.Pink : RewardTier >= 6 ? ItemRarityID.LightRed : ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: Math.Max(1, RewardSilver / 5));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }
    }
}
