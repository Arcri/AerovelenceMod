using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class QueensStinger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Queens Stinger");
            Tooltip.SetDefault("Bees have a 10% chance of spawning with every projectile");
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 20;
            Item.height = 16;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AeroPlayer>().QueensStinger = true;
        }
    }
}