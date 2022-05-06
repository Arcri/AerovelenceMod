using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class ImperialSeeker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Imperial Seeker");
            Tooltip.SetDefault("Right click for true melee");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 66;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 1, 20, 25);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTime = 21;
                Item.useAnimation = 21;
                Item.damage = 32;
                Item.shoot = ProjectileID.None;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.damage = 26;
                Item.shoot = ModContent.ProjectileType<CavernousCrystal>();
                Item.shootSpeed = 6f;
            }
            return base.CanUseItem(player);
        }
    }
}