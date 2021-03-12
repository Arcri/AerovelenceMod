using AerovelenceMod.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Tools
{
    public class SpeedstersPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Speedster's Pickaxe");
		}
        public override void SetDefaults()
        {
			item.crit = 4;
            item.damage = 5;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 6;
            item.useAnimation = 6;
			item.pick = 50;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 3, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
        }
    }
}