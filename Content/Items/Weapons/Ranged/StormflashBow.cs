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
            Item.width = 32;
            Item.height = 56;
            Item.damage = 40;
            Item.knockBack = 2;
            Item.crit = 22;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.shootSpeed = 18f;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
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