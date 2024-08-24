using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public static class ChestConfigurator
    {
        public static void ApplyConfiguration(int x, int y, ChestConfiguration chestConfig)
        {
            int chestIndex = Chest.FindChest(x, y);
            if (chestIndex == -1)
            {
                chestIndex = Chest.CreateChest(x, y);
            }

            if (chestIndex >= 0 && chestConfig != null && chestIndex < Main.chest.Length)
            {
                Chest chest = Main.chest[chestIndex];

                for (int i = 0; i < chest.item.Length; i++)
                {
                    chest.item[i].TurnToAir();
                }

                int slotIndex = 0;

                foreach (var primaryConfig in chestConfig.PrimaryItems)
                {
                    if (Main.rand.NextFloat() < primaryConfig.Chance)
                    {
                        slotIndex = PlaceItemInNextAvailableSlot(chest.item, primaryConfig, slotIndex);
                    }
                }

                foreach (var itemConfig in chestConfig.Items)
                {
                    slotIndex = PlaceItemInNextAvailableSlot(chest.item, itemConfig, slotIndex);
                }
            }
        }

        private static int PlaceItemInNextAvailableSlot(Item[] items, ItemConfiguration itemConfig, int startSlot)
        {
            for (int i = startSlot; i < items.Length; i++)
            {
                if (items[i].IsAir)
                {
                    int itemType = itemConfig.ItemTypeChoices[Main.rand.Next(itemConfig.ItemTypeChoices.Count)];
                    int stackSize = Main.rand.Next(itemConfig.MinStack, itemConfig.MaxStack + 1);

                    items[i].SetDefaults(itemType);
                    items[i].stack = stackSize;
                    items[i].Prefix(-1);
                    return i + 1;
                }
            }
            return items.Length;
        }
    }

    [Serializable]
    public class ChestConfiguration
    {
        public List<PrimaryItemConfiguration> PrimaryItems { get; set; } = [];
        public List<ItemConfiguration> Items { get; set; } = [];

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
        public float Chance { get; set; }

        public PrimaryItemConfiguration(int itemType, int minStack, int maxStack, float chance)
            : base(itemType, minStack, maxStack)
        {
            Chance = chance;
        }

        public PrimaryItemConfiguration(List<int> itemTypeChoices, int minStack, int maxStack, float chance)
            : base(itemTypeChoices, minStack, maxStack)
        {
            Chance = chance;
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
            ItemTypeChoices = [itemType];
            MinStack = minStack;
            MaxStack = maxStack;
        }
    }
}