using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class PlatinumHook : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("PlatinumHook", "Extends all gem hooks and upgrades them to glow")
            .AddName(Language.Default, "Platinum Hook").AddTooltip(Language.Default, "Extends all gem hooks and upgrades them to glow")
            .AddName(Language.Spanish, "Gancho de Platino").AddTooltip(Language.Spanish, "Extiende todos los ganchos de gema y los hace brillar")
            .AddName(Language.French, "Crochet en Platine").AddTooltip(Language.French, "Étend tous les crochets en gemmes et les fait briller")
            .AddName(Language.German, "Platin-Haken").AddTooltip(Language.German, "Erweitert alle Edelsteinhaken und lässt sie leuchten")
            .AddName(Language.Italian, "Gancio di Platino").AddTooltip(Language.Italian, "Estende tutti i ganci di gemme e li fa brillare")
            //.AddName(Language.Polish, "Platynowy Hak").AddTooltip(Language.Polish, "Wydłuża wszystkie haki z klejnotami i sprawia, że świecą")
            //.AddName(Language.PortugueseBrazil, "Gancho de Platina").AddTooltip(Language.PortugueseBrazil, "Estende todos os ganchos de gema e os faz brilhar")
            .AddName(Language.Russian, "Платиновый Крюк").AddTooltip(Language.Russian, "Удлиняет все крюки с драгоценными камнями и заставляет их светиться");
            //.AddName(Language.ChineseTraditional, "白金鉤爪").AddTooltip(Language.ChineseTraditional, "延長所有寶石鉤並讓它們發光")
            //.AddName(Language.ChineseSimplified, "白金钩爪").AddTooltip(Language.ChineseSimplified, "延长所有宝石钩并让它们发光");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.EarlyPHM;
            Item.accessory = true;
        }
    }
}