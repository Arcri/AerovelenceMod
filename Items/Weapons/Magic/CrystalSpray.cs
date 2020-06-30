using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using AerovelenceMod;
using AerovelenceMod.Items.Ores.PreHM.Frost;
using System;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CrystalSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Crystal Spray");
            Tooltip.SetDefault("Fires a stream of harmful crystalline water");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 82;
            item.magic = true;
            item.mana = 20;
            item.width = 28;
            item.height = 30;
            item.useTime = 65;
            item.useAnimation = 65;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ProjectileID.HolyArrow;
            item.shootSpeed = 40f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            throw new NotImplementedException();
        }
    }
}