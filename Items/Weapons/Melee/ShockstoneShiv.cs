using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class ShockstoneShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrified Shiv");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.crit = 20;
            item.damage = 14;
            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 65, 20);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
        }
    }
}