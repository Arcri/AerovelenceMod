using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Bonecutter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonecutter");
        }
        public override void SetDefaults()
        {
            item.crit = 5;
            item.damage = 15;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 19;
            item.useAnimation = 19;
            item.UseSound = SoundID.Item1;
            item.useStyle = 1;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = 1;
            item.autoReuse = true;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if(target.type == NPCID.Skeleton)
            {
                damage = damage * 3;
            }
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(706, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}