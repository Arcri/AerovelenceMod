using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Sahara : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sahara");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 17;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8.5f;
        }
    }
}