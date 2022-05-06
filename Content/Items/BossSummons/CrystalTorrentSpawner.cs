using Terraria;
using Terraria.ID;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class CrystalTorrentSpawner : AerovelenceItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.consumable = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Orange;

            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player) => false;
    }
}