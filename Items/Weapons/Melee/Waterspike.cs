using AerovelenceMod.Items.Placeable.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Waterspike : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waterspike");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 13;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.Stabbing;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }
    }
}