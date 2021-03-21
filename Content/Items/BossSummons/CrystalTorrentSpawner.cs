using Terraria;
using Terraria.ID;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class CrystalTorrentSpawner : AerovelenceItem
    {
        public override void SetDefaults()
        {
            item.maxStack = 99;
            item.useTime = 30;
            item.useAnimation = 30;

            item.consumable = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Orange;

            item.value = Item.buyPrice(gold: 1);
        }

        public override bool UseItem(Player player) => false;
    }
}