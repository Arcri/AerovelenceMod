using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public abstract class TranslatableModItem : ModItem
    {
        private string _originalName = "";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            string displayNameKey = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string tooltipKey = $"{Mod.Name}.{GetType().Name}.Tooltip";
            string displayName = LocalizationManager.GetTranslation(displayNameKey);
            string tooltip = LocalizationManager.GetTranslation(tooltipKey);
            string officialNameKey = $"Mods.{Mod.Name}.ItemName.{Name}";
            string officialTooltipKey = $"Mods.{Mod.Name}.ItemTooltip.{Name}";
            LanguageManager.Instance.GetOrRegister(officialNameKey, () => displayName);
            LanguageManager.Instance.GetOrRegister(officialTooltipKey, () => tooltip);
            string altNameKey = $"Mods.{Mod.Name}.Items.{Name}.DisplayName";
            string altTooltipKey = $"Mods.{Mod.Name}.Items.{Name}.Tooltip";
            LanguageManager.Instance.GetOrRegister(altNameKey, () => displayName);
            LanguageManager.Instance.GetOrRegister(altTooltipKey, () => tooltip);
            LocalizationManager.RegisterLocalizationApplier(() => {
                string currentName = LocalizationManager.GetTranslation(displayNameKey);
                string currentTooltip = LocalizationManager.GetTranslation(tooltipKey);
                LanguageManager.Instance.GetOrRegister(officialNameKey, () => currentName);
                LanguageManager.Instance.GetOrRegister(officialTooltipKey, () => currentTooltip);
                LanguageManager.Instance.GetOrRegister(altNameKey, () => currentName);
                LanguageManager.Instance.GetOrRegister(altTooltipKey, () => currentTooltip);
                if (Item != null && Item.type > ItemID.None)
                {
                    Item.SetNameOverride(currentName);
                }
            });
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _originalName = Item.Name;
            string displayNameKey = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string displayName = LocalizationManager.GetTranslation(displayNameKey);
            Item.SetNameOverride(displayName);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            string displayNameKey = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string displayName = LocalizationManager.GetTranslation(displayNameKey);
            Item.SetNameOverride(displayName);
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            string displayNameKey = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string displayName = LocalizationManager.GetTranslation(displayNameKey);
            Item.SetNameOverride(displayName);
        }

        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
            string displayNameKey = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string displayName = LocalizationManager.GetTranslation(displayNameKey);
            Item.SetNameOverride(displayName);
        }

        public string GetLocalizedName()
        {
            string key = $"{Mod.Name}.{GetType().Name}.DisplayName";
            return LocalizationManager.GetTranslation(key);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) { ApplyTranslations(tooltips); }

        protected static void ApplyTranslations(ModItem item, List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Name == "ItemName")
                {
                    string key = $"{item.Mod.Name}.{item.GetType().Name}.DisplayName";
                    tooltips[i].Text = LocalizationManager.GetTranslation(key);
                }
                if (tooltips[i].Name == "Tooltip0")
                {
                    string key = $"{item.Mod.Name}.{item.GetType().Name}.Tooltip";
                    tooltips[i].Text = LocalizationManager.GetTranslation(key);
                }
            }
            string skillStrikeText = LocalizationExtensions.GetLocalizedSkillStrike(item);
            if (!string.IsNullOrEmpty(skillStrikeText))
            {
                TooltipLine skillStrikeLine = new(item.Mod, "SkillStrike", skillStrikeText)
                {
                    OverrideColor = new Color(255, 215, 0)
                };
                tooltips.Add(skillStrikeLine);
            }
        }

        private void ApplyTranslations(List<TooltipLine> tooltips) { ApplyTranslations(this, tooltips); }
    }
}