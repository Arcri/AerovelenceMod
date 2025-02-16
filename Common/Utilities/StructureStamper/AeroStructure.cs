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

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public record AeroStructure(Vector2 StartPosition, int Width, int Height, string Name)
    {
        public AeroStructure ApplyItemConfigurationsToAll(UnifiedRandom random, List<PrimaryItemConfiguration> primaryItems, List<ItemConfiguration> secondaryItems)
        {
            var chestConfig = new ChestConfiguration();

            PrimaryItemConfiguration selectedPrimaryItem = null;
            WeightedRandom<PrimaryItemConfiguration> weightTable = new WeightedRandom<PrimaryItemConfiguration>(random);
            foreach (var item in primaryItems)
            {
                weightTable.Add(item, (int)(item.Weight * 100));
            }

            Console.WriteLine(weightTable.elements.Count);

            selectedPrimaryItem = weightTable.Get();
            chestConfig.AddPrimaryItemConfiguration(selectedPrimaryItem);
            
            if (selectedPrimaryItem.ItemTypeChoices.Contains(ItemID.FlareGun))
            {
                chestConfig.AddPrimaryItemConfiguration(new PrimaryItemConfiguration(ItemID.Flare, 25, 50, 1f));
            } 

            foreach (var item in secondaryItems)
            {
                chestConfig.AddItemConfiguration(item);
            }

            return ApplyChestConfigurationsToAll(chestConfig);
        }

        public AeroStructure ApplyChestConfigurationsToAll(ChestConfiguration chestConfig)
        {
            for (int x = (int)StartPosition.X; x < (int)StartPosition.X + Width; x++)
            {
                for (int y = (int)StartPosition.Y; y < (int)StartPosition.Y + Height; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (TileID.Sets.BasicChest[tile.TileType])
                    {
                        ChestConfigurator.ApplyConfiguration(x, y, chestConfig);
                    }
                }
            }

            return this;
        }
    }
}
