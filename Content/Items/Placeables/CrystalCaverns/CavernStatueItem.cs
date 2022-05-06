using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Placeables.CrystalCaverns
{
    public class CavernStatueItem : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cavern Statue");
		}
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.createTile = Mod.Find<ModTile>("CavernStatue").Type; //put your CustomBlock Tile name
        }
    }
}
