using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class ElectricRailgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Annihilator");
            Tooltip.SetDefault("Hold down to blast your enemies with lightning");
        }
        public override void SetDefaults()
        {
            item.channel = true;
            item.damage = 300;
            item.ranged = true;
            item.width = 24;
            item.height = 24;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3;
            item.useTurn = false;
            item.value = Terraria.Item.sellPrice(0, 1, 42, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElectricRailgunProj1>();
            item.shootSpeed = 25f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }
    }
}