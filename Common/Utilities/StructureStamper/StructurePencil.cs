using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public class StructurePencil : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                PlaceStructureWithChest(player);
            }
            else
            {
               // StructureStamper.LoadStructure(player.position.ToTileCoordinates().ToVector2(), "test");
            }

            return true;
        }

        private static void PlaceStructureWithChest(Player player)
        {

            //Example of a single chest in a structure (see the structure world gen for applying loot to every single chest otherwise it applies to the first chest
            Vector2 playerPosition = player.Center.ToTileCoordinates().ToVector2();
            Vector2 startPosition = playerPosition;

            var chestConfig = new ChestConfiguration();

            #region primary items
            List<PrimaryItemConfiguration> primaryItems =
            [
                new(ItemID.BandofRegeneration, 1, 1, 0.2f),
                new(ItemID.MagicMirror, 1, 1, 0.2f),
                new(ItemID.CloudinaBottle, 1, 1, 0.2f),
                new(ItemID.HermesBoots, 1, 1, 0.2f),
                new(ItemID.EnchantedBoomerang, 1, 1, 0.2f),
                new(ItemID.ShoeSpikes, 1, 1, 0.2f),
                new(ItemID.FlareGun, 1, 1, 0.2f),
                new(ItemID.Extractinator, 1, 1, 0.2f),
                new(ItemID.LavaCharm, 1, 1, 0.2f),
                new(ItemID.LuckyHorseshoe, 1, 1, 0.2f),
                new(ModContent.ItemType<Eos>(), 1, 1, 0.2f)
            ];

            PrimaryItemConfiguration selectedPrimaryItem = null;
            foreach (var item in primaryItems)
            {
                if (Main.rand.NextFloat() < item.Chance)
                {
                    selectedPrimaryItem = item;
                    break;
                }
            }

            if (selectedPrimaryItem != null)
            {
                chestConfig.AddPrimaryItemConfiguration(selectedPrimaryItem);
                if (selectedPrimaryItem.ItemTypeChoices.Contains(ItemID.FlareGun))
                {
                    chestConfig.AddPrimaryItemConfiguration(new PrimaryItemConfiguration(ItemID.Flare, 25, 50, 1f));
                }
            }
            #endregion

            #region common items
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.SuspiciousLookingEye, 1, 1));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.Dynamite, 1, 1));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.JestersArrow, 25, 50));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar], 3, 10));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.FlamingArrow, ItemID.ThrowingKnife], 25, 50));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.HealingPotion, 3, 5));
            chestConfig.AddItemConfiguration(new ItemConfiguration([
                ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
            ], 1, 2));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.RecallPotion, 1, 2));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.Torch, ItemID.Glowstick], 15, 29));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.GoldCoin, 1, 2));

            #endregion

            var (width, height) = StructureStamper.LoadStructure(playerPosition, "test", [chestConfig], placeStructure: false);
            Vector2 centeredPosition = playerPosition - new Vector2(width / 2, height / 2);
            StructureStamper.LoadStructure(centeredPosition, "test", [chestConfig]);
        }
    }
}