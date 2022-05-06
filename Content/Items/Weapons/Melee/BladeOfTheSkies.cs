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
            Item.crit = 4;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
        }
    }
}