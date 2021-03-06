using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class SlimyGreatsword : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimy Greatsword");
			Tooltip.SetDefault("The great sword of the slimes.");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 8;
            item.damage = 14;
            item.melee = true;
            item.width = 50;
            item.height = 52; 
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
			item.value = Item.sellPrice(0, 0, 40, 20);
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
    }
}