using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public abstract class TranslatableModNPC : ModNPC
    {
        private string cachedName = null;
        private string cachedFlavor = null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterWithLanguageSystem();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            UpdateLocalizedName();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            base.SetBestiary(database, bestiaryEntry);
            for (int i = bestiaryEntry.Info.Count - 1; i >= 0; i--)
            {
                if (bestiaryEntry.Info[i] is FlavorTextBestiaryInfoElement || bestiaryEntry.Info[i] is NamePlateInfoElement)
                    bestiaryEntry.Info.RemoveAt(i);
            }
            bestiaryEntry.Info.Add(new NamePlateInfoElement($"NPCName.{Type}", Type));
            string flavorText = GetLocalizedBestiaryFlavor();
            if (!string.IsNullOrEmpty(flavorText))
                bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement(flavorText));
        }

        public void RegisterWithLanguageSystem()
        {
            string localizedName = GetLocalizedName();
            string numericKey = $"NPCName.{Type}";
            LanguageManager.Instance.GetOrRegister(numericKey, () => localizedName);
            string nameKey = $"NPCName.{Name}";
            LanguageManager.Instance.GetOrRegister(nameKey, () => localizedName);
            string modKey = $"Mods.{Mod.Name}.NPCName.{Name}";
            LanguageManager.Instance.GetOrRegister(modKey, () => localizedName);
            string altKey = $"Mods.{Mod.Name}.{Name}.DisplayName";
            LanguageManager.Instance.GetOrRegister(altKey, () => localizedName);
        }

        public void UpdateLocalizedName()
        {
            cachedName = null;
            NPC.GivenName = GetLocalizedName();
        }

        public string GetLocalizedName()
        {
            if (cachedName != null)
                return cachedName;
            string key = $"{Mod.Name}.{GetType().Name}.DisplayName";
            string translation = LocalizationManager.GetTranslation(key);
            if (translation == key)
            {
                string numericKey = $"NPCName.{Type}";
                translation = Terraria.Localization.Language.GetTextValue(numericKey);

                if (translation == numericKey)
                    translation = GetType().Name;
            }

            cachedName = translation;
            return translation;
        }

        public string GetLocalizedBestiaryFlavor()
        {
            if (cachedFlavor != null)
                return cachedFlavor;
            string key = $"{Mod.Name}.{GetType().Name}.BestiaryFlavor";
            string translation = LocalizationManager.GetTranslation(key);
            if (translation == key)
                translation = "";
            cachedFlavor = translation;
            return translation;
        }

        public void ClearCache()
        {
            cachedName = null;
            cachedFlavor = null;
        }
    }
}