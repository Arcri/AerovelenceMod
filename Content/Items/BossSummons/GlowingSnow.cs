/*using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.BossSummons
{
    public class GlowingSnow : AerovelenceItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glowing Snow");
            Tooltip.SetDefault("Summons Rimegeist\nOnly works at night\nMust be in the snow biome");
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

        //public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<Rimegeist>());

        public override bool UseItem(Player player)
        {
            if (player.ZoneSnow)
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Rimegeist"));
            
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar);
            recipe.AddIngredient(ItemID.SnowBlock, 25);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
*/