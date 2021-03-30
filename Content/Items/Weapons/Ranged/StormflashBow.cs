using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class StormflashBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormflash Bow");
            Tooltip.SetDefault("Converts Wooden Arrows into Shocking ones");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 56;
            item.damage = 42;
            item.knockBack = 2;
            item.crit = 20;
            item.useTime = 18;
            item.useAnimation = 18;
            item.shootSpeed = 18f;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.UseSound = SoundID.Item5;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.LightRed;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<ShockingArrow>();
            }
            return true;
        }
    }
}