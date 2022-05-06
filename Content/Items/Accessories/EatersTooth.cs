using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class EatersTooth : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eater's Tooth");
			Tooltip.SetDefault("Melee damage increased slightly\nMelee speed increased slightly\nYou move slower when this item is equipped");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 14;
            Item.height = 20;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetDamage(DamageClass.Melee) += 0.10f;
			player.GetAttackSpeed(DamageClass.Melee) =+ 1.2f;
			player.maxRunSpeed -= 0.3f;
        }
    }
}