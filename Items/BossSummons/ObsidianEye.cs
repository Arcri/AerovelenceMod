using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.NPCs.Bosses.Snowrium;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.BossSummons
{
    public class ObsidianEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obsidian Eye");
            Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'");
        }
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = ItemRarityID.Cyan;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item44;
			item.consumable = true;
		}

		// We use the CanUseItem hook to prevent a player from using this item while the boss is present in the world.
		public override bool CanUseItem(Player player)
		{
			// "player.ZoneUnderworldHeight" could also be written as "player.position.Y / 16f > Main.maxTilesY - 200"
			return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Cyvercry.Cyvercry>());
		}

		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.Bosses.Cyvercry.Cyvercry>());
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofNight, 5);
			recipe.AddIngredient(ItemID.HallowedBar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}