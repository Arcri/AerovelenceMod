/*
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


namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateSword : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slate Sword");
            // Tooltip.SetDefault("Fires a rock on swing ");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 82;
            Item.crit = 2;
            Item.damage = 11;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.7f};
            Item.useStyle = ItemUseStyleID.Swing;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<SlateSwordHeldProj>();
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<SlateSwordThrown>();
                velocity *= 10;
            }

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));

            }
            else
            {
                tick = !tick;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            }

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            //if (player.ownedProjectileCounts[ModContent.ProjectileType<SlateSwordThrown>()] > 1)
                //return false;
            return base.CanUseItem(player);
        }
    }
}
*/