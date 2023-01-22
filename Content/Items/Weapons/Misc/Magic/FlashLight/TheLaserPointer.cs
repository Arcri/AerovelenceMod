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
    public class TheLaserPointer : ModItem
    {
        
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases in power the longer it is latched onto a target");
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
            Item.damage = 38;
            Item.shootSpeed = 12.5f;
            Item.noMelee = true;
            Item.rare = 8;
            Item.value = 5400 * 5;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<TheLaserPointerProj>();
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AquaScepter).
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
