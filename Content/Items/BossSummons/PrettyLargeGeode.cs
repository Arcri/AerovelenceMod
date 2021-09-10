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
    public class PrettyLargeGeode : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pretty Large Geode");
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
            player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns;

        public override bool UseItem(Player player)
        {
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 250, ModContent.NPCType<CrystalTumbler>());
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }
    }
}
