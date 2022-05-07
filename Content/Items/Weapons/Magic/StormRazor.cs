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
            Item.crit = 11;
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StormRazorProjectile>();
            Item.shootSpeed = 18f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = new Vector2(position.X, position.Y - 12);
        }
    }
}