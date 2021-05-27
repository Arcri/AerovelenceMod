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
            item.crit = 4;
            item.damage = 13;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.Stabbing;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 3, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
        }

        public override void UseStyle(Player player)
        {
            float cosRot = (float)Math.Cos(player.itemRotation - 0.78f * player.direction * player.gravDir);
            float sinRot = (float)Math.Sin(player.itemRotation - 0.78f * player.direction * player.gravDir);
            for (int i = 0; i < 1; i++)
            {
                float length = (item.width * 1.2f - i * item.width / 9) * item.scale - 4; //length to base + arm displacemen
            }
        }
    }
}