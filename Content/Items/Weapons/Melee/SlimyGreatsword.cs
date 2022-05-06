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
			Item.UseSound = SoundID.Item1;
			Item.crit = 8;
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 52; 
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
			Item.value = Item.sellPrice(0, 0, 40, 20);
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
        }
    }
}