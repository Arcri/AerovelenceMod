using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.BossSummons
{
	public class ObsidianEye : AerovelenceItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Obsidian Eye");
			// Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'\nNot consumable");
		}

		public override void SetDefaults()
		{
			Item.consumable = false;

			Item.maxStack = 1;
			Item.useAnimation = 45;
			Item.useTime = 45;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.Item44;
			Item.rare = ItemRarityID.Cyan;
		}

		public override bool CanUseItem(Player player) => !Main.dayTime;// && !NPC.AnyNPCs(ModContent.NPCType<Cyvercry2>());

		public override bool? UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Cyvercry2>());

			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
                .AddIngredient(ItemID.Obsidian, 10)
                .AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.ChlorophyteBar, 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}


    public class CyverSummonSkipIntro : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obsidian Eye");
            // Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'\nNot consumable");
        }

        public override void SetDefaults()
        {
            Item.consumable = false;

            Item.maxStack = 1;
            Item.useAnimation = 45;
            Item.useTime = 45;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool CanUseItem(Player player) => !Main.dayTime;// && !NPC.AnyNPCs(ModContent.NPCType<Cyvercry2>());

        public override bool? UseItem(Player player)
        {
			NPC a = NPC.NewNPCDirect(null, player.Center + new Vector2(-300f, -800), ModContent.NPCType<Cyvercry2>());

			(a.ModNPC as Cyvercry2).whatAttack = 1;

            return true;
        }
    }

    public class CyvercryThumbnailHelper : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obsidian Eye");
            // Tooltip.SetDefault("Summons Cyvercry\nOnly works at night\n'An ancient artifact, it has a subtle glow'\nNot consumable");
        }

        public override void SetDefaults()
        {
            Item.consumable = false;

            Item.maxStack = 1;
            Item.useAnimation = 45;
            Item.useTime = 45;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool CanUseItem(Player player) => !Main.dayTime;// && !NPC.AnyNPCs(ModContent.NPCType<Cyvercry2>());

        public override bool? UseItem(Player player)
        {
            NPC a = NPC.NewNPCDirect(null, player.Center, ModContent.NPCType<Cyvercry2>());

            (a.ModNPC as Cyvercry2).whatAttack = -4;

            return true;
        }
    }


}