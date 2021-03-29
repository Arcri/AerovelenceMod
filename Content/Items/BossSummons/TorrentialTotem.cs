using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.NPCs.Bosses.LightningMoth;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class TorrentialTotem : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torrential Totem");
            Tooltip.SetDefault("Summons the Lightning Moth\nOnly works at night\nMust be in the Crystal Fields");
        }

        public override void SetDefaults()
        {
            item.consumable = true;

            item.maxStack = 999;
            item.useAnimation = 30;
            item.useTime = 30;

            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.value = 100;
        }

        public override bool CanUseItem(Player player) => player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(ModContent.NPCType<LightningMoth>());

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("LightningMoth"));
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ChargedStone>(), 15);
            recipe.AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
