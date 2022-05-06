using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.consumable = true;

            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.maxStack = 999;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;
            Item.value = 100; // TODO - Convert all item.value = int to item.value = Item.sellPrice()
        }

        public override bool CanUseItem(Player player) => 
            player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(ModContent.NPCType<CrystalTumbler>());

        public override bool? UseItem(Player player)
        {
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 250, ModContent.NPCType<CrystalTumbler>());
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup("IronBar", 1)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 15)
                .AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
