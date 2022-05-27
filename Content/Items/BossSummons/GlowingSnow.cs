using AerovelenceMod.Content.Items.Others.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 30;
            Item.useTime = 30;

            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = 100;
        }

        //public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<Rimegeist>());

        public override bool? UseItem(Player player)
        {
            if (player.ZoneSnow)
                NPC.SpawnOnPlayer(player.whoAmI, Mod.Find<ModNPC>("Rimegeist").Type);
            
            SoundEngine.PlaySound(SoundID.Roar, player.position);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HellstoneBar)
                .AddIngredient(ItemID.SnowBlock, 25)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
