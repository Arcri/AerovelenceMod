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
            Item.width = 48;
            Item.height = 48;
            Item.damage = 50;
            Item.knockBack = 6;
            Item.crit = 9;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.mana = 16;            
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.shoot = ModContent.ProjectileType<MoonpiercerProj>();
            Item.useStyle = ItemUseStyleID.Shoot;     
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            return true;
        }
    }
}