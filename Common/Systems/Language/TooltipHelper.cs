using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems.Language
{
    public static class TooltipHelper
    {
        public static void ApplyTranslations(ModItem item, List<TooltipLine> tooltips)
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
    }
}