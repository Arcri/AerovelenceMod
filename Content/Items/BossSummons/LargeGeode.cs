using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.BossSummons
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
            item.consumable = true;

            item.useAnimation = 30;
            item.useTime = 30;
            item.maxStack = 999;

            item.useStyle = ItemUseStyleID.HoldingUp;
            item.rare = ItemRarityID.Blue;
            item.value = 100; // TODO - Convert all item.value = int to item.value = Item.sellPrice()
        }

        public override bool CanUseItem(Player player) => 
            player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(ModContent.NPCType<CrystalTumbler>());

        public override bool UseItem(Player player)
        {
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 250, ModContent.NPCType<CrystalTumbler>());
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 1);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 15);
            recipe.AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
