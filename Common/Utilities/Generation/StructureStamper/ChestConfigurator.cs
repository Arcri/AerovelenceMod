using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using Terraria.Utilities;

namespace AerovelenceMod.Common.Utilities.Generation.StructureStamper
{
    public static class ChestConfigurator
    {

        private static readonly object chestLock = new object();
        private static bool isConfiguringChest = false;
        private static UnifiedRandom rand = WorldGen.genRand;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="chestConfig"></param>
        /// <remarks>ENSURE YOU HAVE TOP LEFT TILE OF CHEST</remarks>
        public static void ApplyConfiguration(int x, int y, ChestConfiguration chestConfig)
        {
            lock (chestLock) //Not really sure lock is needed here, but I'm not inclined to believe it's harming anyone, so will not remove
            {
                try
                {
                    isConfiguringChest = true;

                    Tile tile = Main.tile[x, y];
                    if (!TileID.Sets.BasicChest[tile.TileType])
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn($"Tile at ({x}, {y}) is not a valid chest.");
                        return;
                    }

                    int chestIndex = Chest.FindChest(x, y);
                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.CreateChest(x, y);
                        if (chestIndex == -1 || chestIndex >= Main.chest.Length)
                        {
                            ModContent.GetInstance<AerovelenceMod>()?.Logger.Error($"Failed to create chest at ({x}, {y}).");
                            return;
                        }
                    }

                    Chest chest = Main.chest[chestIndex];
                    if (chest == null)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Error($"Chest at ({x}, {y}) is null.");
                        return;
                    }

                    if (chest.item == null || chest.item.Length != 40)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Error($"Chest at ({x}, {y}) has an invalid item array.");
                        return;
                    }

                    for (int i = 0; i < chest.item.Length; i++)
                    {
                        if (chest.item[i] == null)
                            chest.item[i] = new Item();
                        chest.item[i].TurnToAir();
                    }

                    int slotIndex = 0;
                    int maxSlots = chest.item.Length;
                    if (chestConfig.PrimaryItems != null)
                    {
                        float totalWeight = 0;
                        foreach (var primaryConfig in chestConfig.PrimaryItems)
                        {
                            if (primaryConfig != null)
                            {
                                totalWeight += primaryConfig.Weight;
                            }
                        }
                        float currentWeightThresh = 0;
                        float choice = rand.NextFloat() * totalWeight;
                        foreach (var primaryConfig in chestConfig.PrimaryItems)
                        {
                            if (primaryConfig == null || slotIndex >= maxSlots) break;
                            currentWeightThresh += primaryConfig.Weight;
                            if (currentWeightThresh > choice)
                            {
                                slotIndex = PlaceItemInNextAvailableSlot(chest.item, primaryConfig, slotIndex, x, y);
                            }
                        }
                    }
                    if (chestConfig.Items != null)
                    {
                        foreach (var itemConfig in chestConfig.Items)
                        {
                            if (slotIndex >= maxSlots) break;
                            if (itemConfig != null && rand.NextFloat() < itemConfig.SpawnChance)
                            {
                                slotIndex = PlaceItemInNextAvailableSlot(chest.item, itemConfig, slotIndex, x, y);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<AerovelenceMod>()?.Logger.Error($"Error applying chest configuration at ({x}, {y}): {ex.Message}");
                }
                finally
                {
                    isConfiguringChest = false;
                }
            }
        }

        private static int PlaceItemInNextAvailableSlot(Item[] items, ItemConfiguration itemConfig, int startSlot, int chestX, int chestY)
        {
            for (int i = startSlot; i < items.Length; i++)
            {
                if (items[i].IsAir)
                {
                    if (itemConfig.ItemTypeChoices == null || itemConfig.ItemTypeChoices.Count == 0)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn($"ItemTypeChoices is null or empty for chest at ({chestX}, {chestY}).");
                        continue;
                    }

                    int itemType = itemConfig.ItemTypeChoices[rand.Next(itemConfig.ItemTypeChoices.Count)];
                    if (itemType <= 0 || itemType >= ItemLoader.ItemCount)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn($"Invalid itemType {itemType} for chest at ({chestX}, {chestY}).");
                        continue;
                    }

                    try
                    {
                        int stackSize = Math.Clamp(
                            rand.Next(itemConfig.MinStack, itemConfig.MaxStack + 1),
                            1,
                            ContentSamples.ItemsByType[itemType].maxStack
                        );
                        items[i] = new Item(itemType, stackSize);
                        return i + 1;
                    }
                    catch (Exception ex)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Error($"Error setting item in chest at ({chestX}, {chestY}): {ex.Message}");
                    }
                }
            }
            return items.Length;
        }
    }

    [Serializable]
    public class ChestConfiguration
    {
        public List<PrimaryItemConfiguration> PrimaryItems { get; set; } = new List<PrimaryItemConfiguration>();
        public List<ItemConfiguration> Items { get; set; } = new List<ItemConfiguration>();

        public void AddPrimaryItemConfiguration(PrimaryItemConfiguration itemConfig)
        {
            PrimaryItems.Add(itemConfig);
        }

        public void AddItemConfiguration(ItemConfiguration itemConfig)
        {
            Items.Add(itemConfig);
        }
    }

    [Serializable]
    public class PrimaryItemConfiguration : ItemConfiguration
    {
        public float Weight { get; set; }

        public PrimaryItemConfiguration(int itemType, int minStack, int maxStack, float weight)
            : base(itemType, minStack, maxStack, 1)
        {
            Weight = weight;
        }

        public PrimaryItemConfiguration(List<int> itemTypeChoices, int minStack, int maxStack, float weight)
            : base(itemTypeChoices, minStack, maxStack, 1)
        {
            Weight = weight;
        }
    }

    [Serializable]
    public class ItemConfiguration
    {
        public List<int> ItemTypeChoices { get; set; }
        public int MinStack { get; set; }
        public int MaxStack { get; set; }
        public float SpawnChance { get; set; }

        public ItemConfiguration(List<int> itemTypeChoices, int minStack, int maxStack, float spawnChance)
        {
            ItemTypeChoices = itemTypeChoices;
            MinStack = minStack;
            MaxStack = maxStack;
            SpawnChance = spawnChance;
        }

        public ItemConfiguration(int itemType, int minStack, int maxStack, float spawnChance)
        {
            ItemTypeChoices = new List<int> { itemType };
            MinStack = minStack;
            MaxStack = maxStack;
            SpawnChance = spawnChance;
        }
    }
}