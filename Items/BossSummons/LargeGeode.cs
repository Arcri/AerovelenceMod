using System;
using System.Collections.Generic;
using System.IO;
using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using AerovelenceMod.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.Items.BossSummons
{
    public class LargeGeode : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Large Geode");
			Tooltip.SetDefault("Summons the Crystal Tumbler");
		}
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 30;
            item.useTime = 30;
			item.maxStack = 999;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(mod.NPCType("CrystalTumbler"));
        }
        public override bool UseItem(Player player)
        {
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 250, ModContent.NPCType<CrystalTumbler>());
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 1);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 15);
            recipe.AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
