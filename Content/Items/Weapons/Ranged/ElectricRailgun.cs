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
            Item.channel = true;
            Item.damage = 300;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.useTurn = false;
            Item.value = Terraria.Item.sellPrice(0, 1, 42, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElectricRailgunProj1>();
            Item.shootSpeed = 25f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }
    }
}