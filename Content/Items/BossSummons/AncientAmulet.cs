using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.NPCs.Bosses.TheFallen;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class AncientAmulet : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Amulet");
            Tooltip.SetDefault("Summons The Fallen\nOnly works during the day\n'Legend has it that this was used in ancient rituals'");
        }

		public override void SetDefaults()
		{
            Item.consumable = true;

            Item.maxStack = 20;
			Item.useAnimation = 45;
            Item.useTime = 45;

            Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Cyan;
		}

		public override bool CanUseItem(Player player) => Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<TheFallen>());

		public override bool? UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TheFallen>());
			SoundEngine.PlaySound(SoundID.Roar, player.position);

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
