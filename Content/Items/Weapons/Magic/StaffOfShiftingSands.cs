using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class StaffOfShiftingSands : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Staff Of Shifting Sands");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 17;
            item.magic = true;
            item.mana = 5;
            item.width = 50;
            item.height = 50;
            item.useTime = 16;
            item.useAnimation = 16;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 0, 40, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 12f;
		}
    }
}

