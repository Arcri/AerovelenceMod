using Microsoft.Xna.Framework;
using AerovelenceMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MarshmallowCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marshmallow Cannon");
            Tooltip.SetDefault("Shoots a marshmallow");
        }
        public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 20;
            item.height = 10;
            item.useTime = 25;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.2f;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item11;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("MallowBullet");
            item.shootSpeed = 10f;
        }
    }
}