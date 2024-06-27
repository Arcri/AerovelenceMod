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


namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class Oblivion : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Oblivion");
            //Tooltip.SetDefault("Fires a rock on swing ");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 82;
            Item.crit = 2;
            Item.damage = 89;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.5f, Pitch = 0.8f };
            Item.useStyle = ItemUseStyleID.Swing;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<Oblivion2ElectricBoogaloo>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }
}
*/