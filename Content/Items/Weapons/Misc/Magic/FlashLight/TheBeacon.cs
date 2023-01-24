using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight
{
    public class TheBeacon : ModItem
    {
        
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Locks onto targets clost to the cursor and well as shooting projeciles towards it");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            //Item.mana = 6;
            Item.autoReuse = true;
            Item.useStyle = 5;
            Item.knockBack = 5f;
            Item.width = 38;
            Item.height = 10;
            Item.useAnimation = 16;
            Item.useTime = 8;
            Item.damage = 80;
            Item.shootSpeed = 12.5f;
            Item.noMelee = true;
            Item.rare = 8;
            Item.value = 5400 * 5;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<TheBeaconProj>();
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<TheLaserPointer>()).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddIngredient(ItemID.LunarTabletFragment).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
