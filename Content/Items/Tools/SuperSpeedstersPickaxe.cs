using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools
{
    public class SuperSpeedstersPickaxe : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Super Speedster's Pickaxe");
		}
        public override void SetDefaults()
        {
			item.crit = 30;
            item.damage = 5;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 4;
            item.useAnimation = 4;
			item.pick = 90;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
        }
    }
}