using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.BossSummons
{
	public class ObsidianEye : TranslatableModItem
	{
		public override void SetStaticDefaults()
		{
            this.ModifyLocalization("ObsidianEye", "Not consumable\nSummons Cyvercry\nOnly usable at night")
            .AddName(Language.Default, "Obsidian Eye").AddTooltip(Language.Default, "Not consumable\nSummons Cyvercry\nOnly usable at night")
            .AddName(Language.Spanish, "Ojo de Obsidiana").AddTooltip(Language.Spanish, "No consumible\nInvoca a Cyvercry\nSolo usable de noche")
            .AddName(Language.French, "Œil d'Obsidienne").AddTooltip(Language.French, "Non consommable\nInvoque Cyvercry\nUtilisable uniquement la nuit")
            .AddName(Language.German, "Obsidiansauge").AddTooltip(Language.German, "Nicht verbrauchbar\nBeschwört Cyvercry\nNur nachts verwendbar")
            .AddName(Language.Italian, "Occhio d'Ossidiana").AddTooltip(Language.Italian, "Non consumabile\nEvoca Cyvercry\nUtilizzabile solo di notte")
            .AddName(Language.Polish, "Oko Obsydianu").AddTooltip(Language.Polish, "Nie zużywa się\nPrzywołuje Cyvercry\nMożna używać tylko w nocy")
            .AddName(Language.PortugueseBrazil, "Olho de Obsidiana").AddTooltip(Language.PortugueseBrazil, "Não consumível\nInvoca Cyvercry\nSomente utilizável à noite")
            .AddName(Language.Russian, "Обсидиановый Глаз").AddTooltip(Language.Russian, "Не расходуется\nПризывает Cyvercry\nМожно использовать только ночью")
            .AddName(Language.ChineseTraditional, "黑曜石之眼").AddTooltip(Language.ChineseTraditional, "不可消耗\n召喚 Cyvercry\n僅限夜間使用")
            .AddName(Language.ChineseSimplified, "黑曜石之眼").AddTooltip(Language.ChineseSimplified, "不可消耗\n召唤 Cyvercry\n仅限夜间使用");
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