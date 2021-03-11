using AerovelenceMod.CrystalTorrent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.BossSummons
{
    public class CrystalTorrentSpawner : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.scale = 1;
            item.maxStack = 99;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
        }
        public override bool UseItem(Player player)
        {
            if (!CrystalTorrentWorld.CrystalTorrentUp)
            {
                Main.NewText("A violent electrical storm floods the land...", 175, 75, 255, false);
                CrystalTorrentInvasion.StartCrystalTorrent();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}