using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class ObsidianEye : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obsidian Eye");
            Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'");
        }

		public override void SetDefaults()
		{
            item.consumable = true;

            item.maxStack = 20;
			item.useAnimation = 45;
            item.useTime = 45;

            item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item44;
            item.rare = ItemRarityID.Cyan;
		}

		public override bool CanUseItem(Player player) => !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Cyvercry>());

		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Cyvercry>());
			Main.PlaySound(SoundID.Roar, player.position, 0);

			return true;
		}

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.HallowedBar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}