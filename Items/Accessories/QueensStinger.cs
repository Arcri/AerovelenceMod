using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
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
            item.accessory = true;
            item.width = 20;
            item.height = 16;
            item.value = 60000;
            item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AeroPlayer>().QueensStinger = true;
        }
    }
}