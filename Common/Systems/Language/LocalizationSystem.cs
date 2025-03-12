using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public class LocalizationManager : ModSystem
    {
        private static Dictionary<string, string> translations = [];

        private string lastActiveLanguage = "";
        private static List<LocalizationApplier> pendingAppliers = [];

        public override void OnModLoad()
        {
            LanguageManager.Instance.OnLanguageChanged += (sender) =>
            {
                ApplyTranslations();
                BestiaryRefresher.ForceRebuildNPCBestiaryEntries();
            };
        }

        public override void PostSetupContent()
        {
            ApplyTranslations();
            BestiaryRefresher.ForceRebuildNPCBestiaryEntries();
        }

        public static void RegisterLocalizationApplier(Action applyMethod) { pendingAppliers.Add(new LocalizationApplier(applyMethod)); }

        public static void RegisterTranslation(string key, string text, string language = "en-US")
        {
            string fullKey = $"{language}:{key}";
            translations[fullKey] = text;
            string[] parts = key.Split('.');

            if (parts.Length >= 3)
            {
                string modName = parts[0];
                string entityName = parts[1];
                string property = string.Join(".", parts.Skip(2));
                string currentLanguage = LanguageManager.Instance.ActiveCulture.Name;
                bool isCurrentLanguage = (language == currentLanguage) || (language == "default" && currentLanguage == "en-US");

                if (isCurrentLanguage && property == "DisplayName")
                {
                    // Register with Terraria's language system
                    string nameKey = $"Mods.{modName}.ItemName.{entityName}";
                    LanguageManager.Instance.GetOrRegister(nameKey, () => text);
                }
            }
        }

        public static string GetTranslation(string key)
        {
            string currentLang = LanguageManager.Instance.ActiveCulture.Name;
            string exactKey = $"{currentLang}:{key}";
            if (translations.TryGetValue(exactKey, out string result))
                return result;
            string defKey = $"default:{key}";
            if (translations.TryGetValue(defKey, out result))
                return result;
            string enKey = $"en-US:{key}";
            if (translations.TryGetValue(enKey, out result))
                return result;
            return key;
        }

        private static void ApplyTranslations()
        {
            string currentLanguage = LanguageManager.Instance.ActiveCulture.Name;

            // Get translations for current language or fall back to default
            var forLang = translations.Where(kv => kv.Key.StartsWith($"{currentLanguage}:")).ToList();
            Dictionary<string, string> currentTranslations;
            if (forLang.Count > 0)
                currentTranslations = forLang.ToDictionary(kv => kv.Key[$"{currentLanguage}:".Length..], kv => kv.Value);
            else
            {
                var defLang = translations.Where(kv => kv.Key.StartsWith("default:")).ToList();
                currentTranslations = defLang.ToDictionary(kv => kv.Key["default:".Length..], kv => kv.Value);
            }

            foreach (var entry in currentTranslations)
            {
                string[] parts = entry.Key.Split('.');
                if (parts.Length >= 3)
                {
                    string modName = parts[0];
                    string entityName = parts[1];
                    string property = string.Join(".", parts.Skip(2));

                    // Determine if this is for an item or NPC based on existing patterns in your code
                    bool isNPC = IsNPCKey(entry.Key, modName, entityName);

                    if (isNPC)
                    {
                        ApplyNPCTranslation(modName, entityName, property, entry.Value);
                    }
                    else
                    {
                        ApplyItemTranslation(modName, entityName, property, entry.Value);
                    }
                }
                else
                {
                    ModContent.GetInstance<AerovelenceMod>().Logger.Info($"[ApplyTranslations] Key '{entry.Key}' does not have at least 3 parts");
                }
            }
            foreach (var applier in pendingAppliers) { applier.Apply(); }
        }

        private static bool IsNPCKey(string key, string modName, string entityName)
        {
            if (ModContent.TryFind(modName, entityName, out ModNPC _))
                return true;
            if (ModContent.TryFind(modName, entityName, out ModItem _))
                return false;
            return key.Contains(".BestiaryFlavor") || key.Contains(".NPCName");
        }

        private static void ApplyNPCTranslation(string modName, string npcName, string property, string value)
        {
            string fullKey = $"Mods.{modName}.{npcName}.{property}";
            var localizedText = LanguageManager.Instance.GetOrRegister(fullKey, () => value);
            OverrideLocalizedText(localizedText, value, fullKey);

            if (property == "DisplayName")
            {
                string npcKey = $"NPCName.{npcName}";
                var localizedNpcName = LanguageManager.Instance.GetOrRegister(npcKey, () => value);
                OverrideLocalizedText(localizedNpcName, value, npcKey);
                if (ModContent.TryFind(modName, npcName, out ModNPC foundNpc))
                {
                    int numericType = foundNpc.Type;
                    string numericKey = $"NPCName.{numericType}";
                    var localizedNpcName2 = LanguageManager.Instance.GetOrRegister(numericKey, () => value);
                    OverrideLocalizedText(localizedNpcName2, value, numericKey);
                }
            }
        }

        private static void ApplyItemTranslation(string modName, string itemName, string property, string value)
        {
            string fullKey = $"Mods.{modName}.{itemName}.{property}";
            var localizedText = LanguageManager.Instance.GetOrRegister(fullKey, () => value);
            OverrideLocalizedText(localizedText, value, fullKey);

            if (property == "DisplayName")
            {
                string itemNameKey = $"Mods.{modName}.ItemName.{itemName}";
                var localizedItemName = LanguageManager.Instance.GetOrRegister(itemNameKey, () => value);
                OverrideLocalizedText(localizedItemName, value, itemNameKey);
                if (ModContent.TryFind(modName, itemName, out ModItem foundItem))
                {
                    int numericType = foundItem.Type;
                    string numericKey = $"ItemName.{numericType}";
                    var localizedItemName2 = LanguageManager.Instance.GetOrRegister(numericKey, () => value);
                    OverrideLocalizedText(localizedItemName2, value, numericKey);
                }
            }
        }

        private static void OverrideLocalizedText(LocalizedText localizedText, string newValue, string key)
        {
            var field = localizedText.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.FieldType == typeof(string));
            if (field != null)
            {
                string oldValue = (string)field.GetValue(localizedText);
                field.SetValue(localizedText, newValue);
                string afterValue = (string)field.GetValue(localizedText);
            }
            //else
                //ModContent.GetInstance<AerovelenceMod>().Logger.Info($"[OverrideLocalizedText] Could NOT find any string field in {localizedText.GetType().Name} for key: {key}");

            //ModContent.GetInstance<AerovelenceMod>().Logger.Info($"[OverrideLocalizedText] For key: {key}, final = '{localizedText.Value}'");
        }

        private class LocalizationApplier
        {
            private Action applyMethod;
            public LocalizationApplier(Action applyMethod) => this.applyMethod = applyMethod;

            public override string ToString()
            {
                if (applyMethod == null) return "LocalizationApplier(null)";
                return $"LocalizationApplier({applyMethod.Method.Name})";
            }

            public void Apply() { applyMethod?.Invoke(); }
        }
    }

    public enum Language
    {
        /// <summary>
        /// Default/English language, used as fallback
        /// </summary>
        Default,

        /// <summary>
        /// Spanish (es-ES)
        /// </summary>
        Spanish,

        /// <summary>
        /// Russian (ru-RU)
        /// </summary>
        Russian,

        /// <summary>
        /// Chinese - Simplified (zh-Hans)
        /// </summary>
        ChineseSimplified,

        /// <summary>
        /// Chinese - Traditional (zh-Hant)
        /// </summary>
        ChineseTraditional,

        /// <summary>
        /// Portuguese - Brazil (pt-BR)
        /// </summary>
        PortugueseBrazil,

        /// <summary>
        /// German (de-DE)
        /// </summary>
        German,

        /// <summary>
        /// Italian (it-IT)
        /// </summary>
        Italian,

        /// <summary>
        /// French (fr-FR)
        /// </summary>
        French,

        /// <summary>
        /// Polish (pl-PL)
        /// </summary>
        Polish
    }

    public static class LanguageExtensions
    {
        public static string ToCultureCode(this Language language)
        {
            return language switch
            {
                Language.Default => "default",
                Language.Spanish => "es-ES",
                Language.Russian => "ru-RU",
                Language.ChineseSimplified => "zh-Hans",
                Language.ChineseTraditional => "zh-Hant",
                Language.PortugueseBrazil => "pt-BR",
                Language.German => "de-DE",
                Language.Italian => "it-IT",
                Language.French => "fr-FR",
                Language.Polish => "pl-PL",
                _ => "default"
            };
        }

        public static Language FromCultureCode(string cultureCode)
        {
            return cultureCode switch
            {
                "es-ES" => Language.Spanish,
                "ru-RU" => Language.Russian,
                "zh-Hans" => Language.ChineseSimplified,
                "zh-Hant" => Language.ChineseTraditional,
                "pt-BR" => Language.PortugueseBrazil,
                "de-DE" => Language.German,
                "it-IT" => Language.Italian,
                "fr-FR" => Language.French,
                "pl-PL" => Language.Polish,
                _ => Language.Default
            };
        }
    }

    public static class LocalizationExtensions
    {
        public static T ModifyLocalization<T>(this T item, string defaultName, string defaultTooltip) where T : ModItem
        {
            string nameKey = $"Mods.{item.Mod.Name}.ItemName.{item.Name}";
            string tooltipKey = $"Mods.{item.Mod.Name}.ItemTooltip.{item.Name}";
            RegisterItemText(item, "DisplayName", defaultName, Language.Default);
            RegisterItemText(item, "Tooltip", defaultTooltip, Language.Default);
            LanguageManager.Instance.GetOrRegister(nameKey, () => defaultName);
            LanguageManager.Instance.GetOrRegister(tooltipKey, () => defaultTooltip);

            return item;
        }

        public static T AddName<T>(this T item, Dictionary<Language, string> nameTranslations) where T : ModItem
        {
            foreach (var translation in nameTranslations)
            {
                RegisterItemText(item, "DisplayName", translation.Value, translation.Key);
                if (translation.Key != Language.Default)
                {
                    string cultureName = translation.Key.ToCultureCode();
                    if (cultureName != "default")
                    {
                        string nameKey = $"Mods.{item.Mod.Name}.ItemName.{item.Name}";
                        LanguageManager.Instance.GetOrRegister(nameKey, () => translation.Value);
                    }
                }
            }
            return item;
        }

        public static T AddTooltip<T>(this T item, Dictionary<Language, string> tooltipTranslations) where T : ModItem
        {
            foreach (var translation in tooltipTranslations)
            {
                RegisterItemText(item, "Tooltip", translation.Value, translation.Key);
                if (translation.Key != Language.Default)
                {
                    string cultureName = translation.Key.ToCultureCode();
                    if (cultureName != "default")
                    {
                        string tooltipKey = $"Mods.{item.Mod.Name}.ItemTooltip.{item.Name}";
                        LanguageManager.Instance.GetOrRegister(tooltipKey, () => translation.Value);
                    }
                }
            }
            return item;
        }

        public static T AddSkillStrike<T>(this T item, Dictionary<Language, string> skillStrikeTranslations) where T : ModItem
        {
            foreach (var translation in skillStrikeTranslations)
            {
                string formattedText = FormatSkillStrikeText(translation.Value);
                RegisterItemText(item, "SkillStrike", formattedText, translation.Key);
            }

            return item;
        }

        public static T AddSkillStrike<T>(this T item, Language language, string skillStrikeText) where T : ModItem
        {
            string formattedText = FormatSkillStrikeText(skillStrikeText);
            RegisterItemText(item, "SkillStrike", formattedText, language);
            return item;
        }

        public static T AddName<T>(this T item, Language language, string name) where T : ModItem
        {
            RegisterItemText(item, "DisplayName", name, language);
            if (language != Language.Default)
            {
                string cultureName = language.ToCultureCode();
                if (cultureName != "default")
                {
                    string nameKey = $"Mods.{item.Mod.Name}.ItemName.{item.Name}";
                    LanguageManager.Instance.GetOrRegister(nameKey, () => name);
                }
            }
            return item;
        }

        public static T AddTooltip<T>(this T item, Language language, string tooltip) where T : ModItem
        {
            RegisterItemText(item, "Tooltip", tooltip, language);
            if (language != Language.Default)
            {
                string cultureName = language.ToCultureCode();
                if (cultureName != "default")
                {
                    string tooltipKey = $"Mods.{item.Mod.Name}.ItemTooltip.{item.Name}";
                    LanguageManager.Instance.GetOrRegister(tooltipKey, () => tooltip);
                }
            }
            return item;
        }

        private static string FormatSkillStrikeText(string text)
        {
            if (text.Contains("[i:" + ItemID.FallenStar) || text.Contains("[i:16]"))
                return text;
            return $"[i:{ItemID.FallenStar}] {text} [i:{ItemID.FallenStar}]";
        }

        private static void RegisterItemText<T>(T item, string property, string text, Language language) where T : ModItem
        {
            string modName = item.Mod.Name;
            string itemName = item.GetType().Name;
            string cultureCode = language.ToCultureCode();
            LocalizationManager.RegisterTranslation($"{modName}.{itemName}.{property}", text, cultureCode);
        }

        public static string GetLocalizedSkillStrike(ModItem item)
        {
            string key = $"{item.Mod.Name}.{item.GetType().Name}.SkillStrike";
            string translation = LocalizationManager.GetTranslation(key);
            return translation == key ? "" : translation;
        }
    }

    public static class NPCLocalizationExtensions
    {
        public static T ModifyLocalization<T>(this T npc, string defaultName, string defaultFlavor) where T : ModNPC
        {
            string keyName = $"{npc.Mod.Name}.{npc.GetType().Name}.DisplayName";
            string keyFlavor = $"{npc.Mod.Name}.{npc.GetType().Name}.BestiaryFlavor";
            LocalizationManager.RegisterTranslation(keyName, defaultName, "default");
            LocalizationManager.RegisterTranslation(keyFlavor, defaultFlavor, "default");
            LanguageManager.Instance.GetOrRegister($"NPCName.{npc.Type}", () => defaultName);

            return npc;
        }

        public static T AddName<T>(this T npc, Language language, string translatedName) where T : ModNPC
        {
            string key = $"{npc.Mod.Name}.{npc.GetType().Name}.DisplayName";
            LocalizationManager.RegisterTranslation(key, translatedName, language.ToCultureCode());
            if (language != Language.Default)
            {
                string typeKey = $"NPCName.{npc.Type}";
                LanguageManager.Instance.GetOrRegister(typeKey, () => translatedName);
            }

            return npc;
        }

        public static T AddFlavor<T>(this T npc, Language language, string translatedFlavor) where T : ModNPC
        {
            string flavorKey = $"{npc.Mod.Name}.{npc.GetType().Name}.BestiaryFlavor";
            LocalizationManager.RegisterTranslation(flavorKey, translatedFlavor, language.ToCultureCode());

            return npc;
        }
    }
}