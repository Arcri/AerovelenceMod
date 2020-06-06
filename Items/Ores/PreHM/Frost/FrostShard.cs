using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Ores.PreHM.Frost
{
    public class FrostShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Shard");
        }		
        public override void SetDefaults()
        {
			item.maxStack = 999;
            item.width = 18;
            item.height = 18;
            item.value = 10;
            item.rare = ItemRarityID.Green;
        }
    }
}