using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class OceanMist : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Ocean Mist");
            Tooltip.SetDefault("Rains down harmful ocean mist");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 15;
            item.magic = true;
            item.mana = 7;
            item.width = 28;
            item.height = 30;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ProjectileID.WaterStream;
            item.shootSpeed = 13f;
        }
    }
}