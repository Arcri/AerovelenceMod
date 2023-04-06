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
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Weapons.NatureSet
{
    public class Woodpecker : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Woodpecker");
            // Tooltip.SetDefault("TODO");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WoodpeckerArrow>();
            //Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 12f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            return true;
        }

    }
}
