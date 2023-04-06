using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;


namespace AerovelenceMod.Content.Items.Weapons.Desert
{
    public class StaffOfShiftingSands : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Staff of Shifting Sands");
            //Tooltip.SetDefault("Fires a rock on swing ");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 82;
            Item.crit = 2;
            Item.damage = 14;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.4f, Pitch = 0.3f };
            Item.useStyle = ItemUseStyleID.Swing;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<StaffOfShiftingSandsHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }
}
