using AerovelenceMod.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class IcySaber : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Icy Saber");

        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.damage = 24;
            item.melee = true;
            item.width = 64;
            item.height = 72;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 7, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.channel = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<IcySaberProj>();
        }

    }
}