using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class ThornBallGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Thorn Glove");
            Tooltip.SetDefault("Throws a spiky ball that bounces around");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 57;
            item.ranged = true;
            item.width = 28;
            item.height = 30;
            item.useTime = 30;
            item.useAnimation = 30;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ThornBall>();
            item.shootSpeed = 15f;
        }
    }
}