using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.BossSummons
{
	public class ObsidianEye : AerovelenceItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Obsidian Eye");
			Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'\nNot consumable");
		}

		public override void SetDefaults()
		{
			Item.consumable = false;

			Item.maxStack = 20;
			Item.useAnimation = 45;
			Item.useTime = 45;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item44;
			Item.rare = ItemRarityID.Cyan;
		}

		public override bool CanUseItem(Player player) => !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Cyvercry>());

		public override bool? UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Cyvercry2>());
			//SoundEngine.PlaySound(SoundID.Roar, player.position);

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.HallowedBar, 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}