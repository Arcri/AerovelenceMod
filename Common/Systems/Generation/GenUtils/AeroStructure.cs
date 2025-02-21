using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using AerovelenceMod.Common.Utilities.StructureStamper;

namespace AerovelenceMod.Common.Systems.Generation.GenUtils
{
    public record AeroStructure(Vector2 StartPosition, int Width, int Height, string Name)
    {
        public static AeroStructure Empty { get; private set; } = new AeroStructure(Vector2.Zero, 0, 0, "");

        public static List<Rectangle> ProtectedStructures { get; private set; } = new List<Rectangle>();
        public bool Protected { get; private set; } = false;

        public AeroStructure ApplyItemConfigurationsToAll(UnifiedRandom random, List<PrimaryItemConfiguration> primaryItems, List<ItemConfiguration> secondaryItems)
        {
            if (this == Empty) { return this; }

            HashSet<int> configuredChests = new HashSet<int>();
            for (int x = (int)StartPosition.X; x < (int)StartPosition.X + Width; x++)
            {
                for (int y = (int)StartPosition.Y; y < (int)StartPosition.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    //TileFrameX = 0 for left side of chests/interactable tiles
                    if (tile != null && TileID.Sets.BasicChest[tile.TileType] && tile.TileFrameX == 0)
                    {
                        int chestIndex = Chest.FindChest(x, y);
                        if (chestIndex == -1)
                            continue;
                        if (configuredChests.Contains(chestIndex))
                            continue;

                        var chestConfig = new ChestConfiguration();

                        if (primaryItems != null && primaryItems.Count > 0)
                        {
                            var weightTable = new WeightedRandom<PrimaryItemConfiguration>(random);
                            foreach (var item in primaryItems)
                            {
                                weightTable.Add(item, (int)(item.Weight * 100));
                            }
                            var selectedPrimaryItem = weightTable.Get();
                            if (selectedPrimaryItem != null)
                            {
                                chestConfig.AddPrimaryItemConfiguration(selectedPrimaryItem);
                                if (selectedPrimaryItem.ItemTypeChoices.Contains(ItemID.FlareGun))
                                {
                                    chestConfig.AddPrimaryItemConfiguration(
                                        new PrimaryItemConfiguration(ItemID.Flare, 25, 50, 1f)
                                    );
                                }
                            }
                        }

                        if (secondaryItems != null)
                        {
                            foreach (var item in secondaryItems)
                            {
                                chestConfig.AddItemConfiguration(item);
                            }
                        }

                        ChestConfigurator.ApplyConfiguration(x, y, chestConfig);
                        configuredChests.Add(chestIndex);
                        y++; // Move down one additional step to ensure chest isn't counted twice
                    }
                }
            }

            return this;
        }

        public AeroStructure ApplyChestConfigurationsToAll(ChestConfiguration chestConfig)
        {
            if (this == Empty) { return this; }

            for (int x = (int)StartPosition.X; x < (int)StartPosition.X + Width; x++)
            {
                for (int y = (int)StartPosition.Y; y < (int)StartPosition.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (TileID.Sets.BasicChest[tile.TileType] && tile.TileFrameX == 0)
                    {
                        ChestConfigurator.ApplyConfiguration(x, y, chestConfig);
                        y++; // Move down one additional step to ensure chest isn't counted twice
                    }
                }
            }

            return this;
        }

        public AeroStructure ProtectStructure()
        {
            if (this == Empty)
                return this;

            if (!Protected)
            {
                GenVars.structures.AddProtectedStructure(ToRectangle(), 0);
                ProtectedStructures.Add(ToRectangle());
                Protected = true;
            }
            return this;
        }


        public bool CanPlace()
        {
            if (this == Empty) { return false; }

            return !ProtectedStructures.Any(x => x.Intersects(ToRectangle()));
        }

        public Rectangle ToRectangle()
        {
            if (this == Empty) { return Rectangle.Empty; }

            return new Rectangle((int)StartPosition.X, (int)StartPosition.Y, Width, Height);
        }
    }
}
