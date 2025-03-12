using AerovelenceMod.Common.Utilities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Potions
{
    public class OnTheRocks : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("OnTheRocks", "'Stone-Aged'")
            .AddName(Language.Default, "On The Rocks").AddTooltip(Language.Default, "'Stone-Aged'")
            .AddName(Language.Spanish, "En Las Rocas").AddTooltip(Language.Spanish, "'Añejado desde la Edad de Piedra'")
            .AddName(Language.French, "Sur Les Roches").AddTooltip(Language.French, "'Mûri depuis l'Âge de Pierre'")
            .AddName(Language.German, "Auf Den Felsen").AddTooltip(Language.German, "'Seit der Steinzeit gereift'")
            .AddName(Language.Italian, "Sulle Rocce").AddTooltip(Language.Italian, "'Invecchiato dall'Età della Pietra'")
            .AddName(Language.Polish, "Na Skałach").AddTooltip(Language.Polish, "'Dojrzewało od Epoki Kamienia'")
            .AddName(Language.PortugueseBrazil, "Nas Rochas").AddTooltip(Language.PortugueseBrazil, "'Envelhecido desde a Idade da Pedra'")
            .AddName(Language.Russian, "На Камнях").AddTooltip(Language.Russian, "'Настоящая древность'");
            //.AddName(Language.ChineseTraditional, "石上").AddTooltip(Language.ChineseTraditional, "'石器時代'")
            //.AddName(Language.ChineseSimplified, "石上").AddTooltip(Language.ChineseSimplified, "'石器时代'");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarities.EarlyPHM;
        }
    }
}