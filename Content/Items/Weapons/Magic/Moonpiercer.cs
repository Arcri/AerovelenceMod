using System;
using System.Drawing;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class Moonpiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moonpiercer");
            Tooltip.SetDefault("Call upon the Moon's spears to strike down your foes");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 50;
            item.knockBack = 6;
            item.crit = 9;
            item.useTime = 26;
            item.useAnimation = 26;
            item.mana = 16;            
            item.magic = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.UseSound = SoundID.Item21;
            item.shoot = ModContent.ProjectileType<MoonpiercerProj>();
            item.useStyle = ItemUseStyleID.HoldingOut;     
            item.rare = ItemRarityID.Yellow;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            return true;
        }
    }
}