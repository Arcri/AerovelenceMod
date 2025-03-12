using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public class LocalizationPatcher : ModSystem
    {
        private static Type itemTypesType;
        private static FieldInfo namesField;

        public override void Load()
        {
            itemTypesType = typeof(ItemID).Assembly.GetType("Terraria.ID.ItemID");
            namesField = itemTypesType?.GetField("names", BindingFlags.Static | BindingFlags.Public);
            LanguageManager.Instance.OnLanguageChanged += (sender) => { ApplyNamePatches(); };
        }

        public static void ApplyNamePatches()
        {
            if (namesField == null || itemTypesType == null)
                return;
            try
            {
                string[] names = (string[])namesField.GetValue(null);
                foreach (var item in ModContent.GetContent<TranslatableModItem>())
                {
                    int type = item.Type;
                    if (type >= 0 && type < names.Length)
                    {
                        string key = $"{item.Mod.Name}.{item.GetType().Name}.DisplayName";
                        string localizedName = LocalizationManager.GetTranslation(key);
                        names[type] = localizedName;
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<AerovelenceMod>().Logger.Warn($"Error patching item names: {ex.Message}");
            }
        }
    }


    public static class ContentInstanceExtensions
    {
        public static void ForceUpdateDisplayName(this Item item)
        {
            if (item != null && item.ModItem is TranslatableModItem translatable)
            {
                string localizedName = translatable.GetLocalizedName();
                item.SetNameOverride(localizedName);
            }
        }

        public static void ForceUpdateDisplayName(this NPC npc)
        {
            if (npc != null && npc.ModNPC is TranslatableModNPC translatable)
            {
                string localizedName = translatable.GetLocalizedName();
                npc.GivenName = localizedName;
                if (!string.IsNullOrEmpty(npc.TypeName))
                    LanguageManager.Instance.GetOrRegister(npc.TypeName, () => localizedName);
            }
        }
    }
}