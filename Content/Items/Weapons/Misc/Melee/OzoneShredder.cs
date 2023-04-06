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


namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class OzoneShredder : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ozone Shredder");
            /* Tooltip.SetDefault("Hit enemies while dashing to bash off of them" +
                "\n'Sever the Skyline!'"); */
        }
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.crit = 2;
            Item.damage = 309;
            Item.useAnimation = 100;
            Item.useTime = 100;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Purple;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = - 0.2f };
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<OzoneShredderHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
           return false;
        }

    }
}
