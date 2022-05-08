using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    public class MirageCloak : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mirage Cloak");
			Tooltip.SetDefault("Has a chance to confuse enemies when hit\nCauses stars to fall out of the sky");
		}
        public override void SetDefaults()
        {
			Item.accessory = true;
            Item.width = 34;
            Item.height = 34;
            Item.value = 60000;
            Item.rare = ItemRarityID.Green;
        }
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetDamage(DamageClass.Ranged) += 0.1f;
            AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
            modPlayer.EmeraldEmpoweredGem = true;
        }
    }
}