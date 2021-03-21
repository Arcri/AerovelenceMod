using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class PrismThrasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Thrasher");
            Tooltip.SetDefault("Right click for true melee");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 8;
            item.damage = 26;
            item.melee = true;
            item.width = 36;
            item.height = 66;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 1, 20, 25);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTime = 21;
                item.useAnimation = 21;
                item.damage = 32;
                item.shoot = ProjectileID.None;
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTime = 30;
                item.useAnimation = 30;
                item.damage = 26;
                item.shoot = ModContent.ProjectileType<CavernousCrystal>();
                item.shootSpeed = 6f;
            }
            return base.CanUseItem(player);
        }
    }
}