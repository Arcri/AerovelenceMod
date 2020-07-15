using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class Electrobolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Electro-bolt");
            Tooltip.SetDefault("Shoots a bolt of electrically charged water\nGets faster every time it hits a block");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 35;
            item.magic = true;
            item.mana = 12;
            item.width = 28;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 0, 95, 50);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElectricitySpark>();
            item.shootSpeed = 5f;
        }
    }
}