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
			Item.staff[Item.type] = true;
            DisplayName.SetDefault("Staff Of Shifting Sands");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 17;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.RubyBolt;
            Item.shootSpeed = 12f;
		}
    }
}

