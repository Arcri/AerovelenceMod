using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using System.Linq;
using System;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public class StructureWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (genIndex != -1)
            {
                //tasks.Insert(genIndex + 1, new PassLegacy("Generate Test Structure", GenerateStructure));
            }
        }

        /*private void GenerateStructure(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Generating Test Structure";

            int structureCount = Main.maxTilesX / 100;

            for (int i = 0; i < structureCount; i++)
            {
                int centerX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int centerY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
                //var (width, height) = StructureStamper.LoadStructure(new Vector2(centerX, centerY), "test");

                int startX = centerX - width / 2;
                int startY = centerY - height / 2;

                //StructureStamper.LoadStructure(new Vector2(startX, startY), "test");

                ApplyLootToAllChestsInStructure(new Vector2(startX, startY), width, height);
            }
        }*/

        private static bool IsValidLocation(int x, int y)
        {
            for (int dx = -20; dx < 20; dx++)
            {
                for (int dy = -20; dy < 20; dy++)
                {
                    if (Main.tile[x + dx, y + dy].LiquidAmount > 0)
                        return false;
                }
            }
            return true;
        }

        private static void ApplyLootToAllChestsInStructure(Vector2 startPosition, int structureWidth, int structureHeight)
        {
            var chestConfig = new ChestConfiguration();

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

            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.SuspiciousLookingEye, 1, 1));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.Dynamite, 1, 1));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.JestersArrow, 25, 50));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar], 3, 10));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.FlamingArrow, ItemID.ThrowingKnife], 25, 50));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.HealingPotion, 3, 5));
            chestConfig.AddItemConfiguration(new ItemConfiguration(
            [
                ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
            ], 1, 2));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.RecallPotion, 1, 2));
            chestConfig.AddItemConfiguration(new ItemConfiguration([ItemID.Torch, ItemID.Glowstick], 15, 29));
            chestConfig.AddItemConfiguration(new ItemConfiguration(ItemID.GoldCoin, 1, 2));

            for (int x = (int)startPosition.X; x < (int)startPosition.X + structureWidth; x++)
            {
                for (int y = (int)startPosition.Y; y < (int)startPosition.Y + structureHeight; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (TileID.Sets.BasicChest[tile.TileType])
                    {
                        ChestConfigurator.ApplyConfiguration(x, y, chestConfig);
                    }
                }
            }
        }
    }
}