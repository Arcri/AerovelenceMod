using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.NPCs.Bosses.LightningMoth;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.consumable = true;

            Item.maxStack = 999;
            Item.useAnimation = 30;
            Item.useTime = 30;

            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = 100;
        }

        public override bool CanUseItem(Player player) => player.GetModPlayer<ZonePlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(ModContent.NPCType<LightningMoth>());

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, Mod.Find<ModNPC>("LightningMoth").Type);
            SoundEngine.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<ChargedStoneItem>(), 15)
                .AddIngredient(ModContent.ItemType<LustrousCrystal>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
