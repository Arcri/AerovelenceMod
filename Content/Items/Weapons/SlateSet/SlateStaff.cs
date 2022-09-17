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
using AerovelenceMod.Common.Utilities;



namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{
    public class SlateStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slate Staff");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = SoundID.Item5;
            Item.crit = 0;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 40; //5
            Item.UseSound = SoundID.Item88.WithPitchOffset(0.2f);
            Item.useAnimation = 40; //5
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.mana = 16;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlateStaffHeldProj>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;

        }
    }
}
