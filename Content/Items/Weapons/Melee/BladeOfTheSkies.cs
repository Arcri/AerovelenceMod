using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class BladeOfTheSkies : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of the Skies");
            Tooltip.SetDefault("Hitting enemies releases shards of crystal around the enemy that will implode");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 24;
            item.melee = true;
            item.width = 46;
            item.height = 46;
            item.useTime = 25;
            item.useAnimation = 25;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
        }
    }
}