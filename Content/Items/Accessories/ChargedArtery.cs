using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class ChargedArtery : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Charged Artery");
			Tooltip.SetDefault("Life is increased by 50\nLife regen increased\n You run faster while this is equipped");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 24;
            Item.height = 26;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.statLifeMax2 += 50;
			player.lifeRegen += 2;
			player.maxRunSpeed += 0.3f;
        }
    }
}