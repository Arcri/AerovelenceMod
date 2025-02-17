using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public static class ChestConfigurator
    {

        private static readonly object chestLock = new object();
        private static bool isConfiguringChest = false;
        public static void ApplyConfiguration(int x, int y, ChestConfiguration chestConfig)
        {
            lock (chestLock)
            {
                try
                {
                    isConfiguringChest = true;

                    Tile tile = Main.tile[x, y];
                    if (!TileID.Sets.BasicChest[tile.TileType])
                        return;

                    int chestIndex = Chest.FindChest(x, y);
                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.CreateChest(x, y);
                        if (chestIndex == -1 || chestIndex >= Main.chest.Length)
                            return;
                    }

                    Chest chest = Main.chest[chestIndex];
                    if (chest == null || chestConfig == null)
                        return;
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
                        foreach (var primaryConfig in chestConfig.PrimaryItems)
                        {
                            if (slotIndex >= maxSlots) break;
                            if (primaryConfig != null && Main.rand.NextFloat() < primaryConfig.Weight)
                            {
                                slotIndex = PlaceItemInNextAvailableSlot(chest.item, primaryConfig, slotIndex);
                            }
                        }
                    }
                    if (chestConfig.Items != null)
                    {
                        foreach (var itemConfig in chestConfig.Items)
                        {
                            if (slotIndex >= maxSlots) break;
                            if (itemConfig != null)
                            {
                                slotIndex = PlaceItemInNextAvailableSlot(chest.item, itemConfig, slotIndex);
                            }
                        }
                    }
                }
                finally
                {
                    isConfiguringChest = false;
                }
            }
        }

        private static int PlaceItemInNextAvailableSlot(Item[] items, ItemConfiguration itemConfig, int startSlot)
        {
            for (int i = startSlot; i < items.Length; i++)
            {
                if (items[i].IsAir)
                {
                    if (itemConfig.ItemTypeChoices == null || itemConfig.ItemTypeChoices.Count == 0)
                    {
                        continue;
                    }
                    int itemType = itemConfig.ItemTypeChoices[Main.rand.Next(itemConfig.ItemTypeChoices.Count)];
                    try
                    {
                        int stackSize = Main.rand.Next(itemConfig.MinStack, itemConfig.MaxStack + 1);

                        if (itemType <= 0 || itemType >= ItemLoader.ItemCount)
                        {
                            ModContent.GetInstance<AerovelenceMod>()?.Logger.Warn(
                                $"Skipping invalid itemType {itemType} at chest slot {i}."
                            );
                            return i + 1;
                        }

                        items[i].SetDefaults(itemType);
                        items[i].stack = stackSize;
                        items[i].Prefix(-1);
                        return i + 1;
                    }
                    catch (Exception ex)
                    {
                        ModContent.GetInstance<AerovelenceMod>()?.Logger.Error(
                            $"Error setting item defaults for itemType {itemType}: {ex}"
                        );
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
            : base(itemType, minStack, maxStack)
        {
            Weight = weight;
        }

        public PrimaryItemConfiguration(List<int> itemTypeChoices, int minStack, int maxStack, float weight)
            : base(itemTypeChoices, minStack, maxStack)
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

        public ItemConfiguration(List<int> itemTypeChoices, int minStack, int maxStack)
        {
            ItemTypeChoices = itemTypeChoices;
            MinStack = minStack;
            MaxStack = maxStack;
        }

        public ItemConfiguration(int itemType, int minStack, int maxStack)
        {
            ItemTypeChoices = new List<int> { itemType };
            MinStack = minStack;
            MaxStack = maxStack;
        }
    }
}