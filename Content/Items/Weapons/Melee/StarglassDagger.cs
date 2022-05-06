using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class StarglassDagger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starglass Dagger");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }

        public override void UseStyle(Player player)
        {
            float cosRot = (float)Math.Cos(player.itemRotation - 0.78f * player.direction * player.gravDir);
            float sinRot = (float)Math.Sin(player.itemRotation - 0.78f * player.direction * player.gravDir);
            for (int i = 0; i < 1; i++)
            {
                float length = (Item.width * 1.2f - i * Item.width / 9) * Item.scale - 4; //length to base + arm displacemen
            }
        }
    }
}