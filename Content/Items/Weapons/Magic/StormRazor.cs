using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class StormRazor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            Tooltip.SetDefault("Casts razorwind that speeds up when it hits a block");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 70;
            item.magic = true;
            item.mana = 16;
            item.width = 36;
            item.height = 42;
            item.useTime = 25;
            item.useAnimation = 25;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<StormRazorProjectile>();
            item.shootSpeed = 18f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = new Vector2(position.X, position.Y - 12);
            return true;
        }
    }
}