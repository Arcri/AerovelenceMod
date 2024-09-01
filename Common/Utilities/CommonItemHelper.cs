using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Utilities
{
    public static class CommonItemHelper
    {
        public static void SetupPlaceableItem(ModItem modItem, int width, int height, int value, int createTileType, int placeStyle = 0, int maxStack = 99, int useAnimation = 15, int useTime = 10)
        {
            modItem.Item.width = width;
            modItem.Item.height = height;
            modItem.Item.maxStack = maxStack;
            modItem.Item.value = value;
            modItem.Item.useTurn = true;
            modItem.Item.autoReuse = true;
            modItem.Item.useAnimation = useAnimation;
            modItem.Item.useTime = useTime;
            modItem.Item.useStyle = ItemUseStyleID.Swing;
            modItem.Item.consumable = true;
            modItem.Item.createTile = createTileType;
            modItem.Item.placeStyle = placeStyle;
        }

        public static void SetupTorch(ModItem modItem, int tileType, int value, int shimmerTransformToItem = 0, float lightR = 1f, float lightG = 1f, float lightB = 1f, int researchUnlockCount = 100)
        {
            modItem.Item.width = 10;
            modItem.Item.height = 12;
            modItem.Item.maxStack = 999;
            modItem.Item.value = value;
            modItem.Item.useTurn = true;
            modItem.Item.autoReuse = true;
            modItem.Item.useAnimation = 15;
            modItem.Item.useTime = 10;
            modItem.Item.useStyle = ItemUseStyleID.Swing;
            modItem.Item.consumable = true;
            modItem.Item.createTile = tileType;
            modItem.Item.holdStyle = 1;
            modItem.Item.placeStyle = 0;
            modItem.Item.flame = true;
            modItem.Item.noWet = false;

            if (shimmerTransformToItem > 0)
            {
                ItemID.Sets.ShimmerTransformToItem[modItem.Type] = shimmerTransformToItem;
            }

            modItem.Item.DefaultToTorch(tileType, 0, false);

            ItemID.Sets.SingleUseInGamepad[modItem.Type] = true;
            ItemID.Sets.Torches[modItem.Type] = true;

            modItem.Item.GetGlobalItem<TorchGlobalItem>().LightR = lightR;
            modItem.Item.GetGlobalItem<TorchGlobalItem>().LightG = lightG;
            modItem.Item.GetGlobalItem<TorchGlobalItem>().LightB = lightB;

            modItem.Item.ResearchUnlockCount = researchUnlockCount;
        }

        public class TorchGlobalItem : GlobalItem
        {
            public float LightR;
            public float LightG;
            public float LightB;

            public override bool InstancePerEntity => true;

            public override void HoldItem(Item item, Player player)
            {
                if (item.createTile != -1 && ItemID.Sets.Torches[item.type])
                {
                    if (!player.wet)
                    {
                        Vector2 position = player.RotatedRelativePoint(player.itemLocation, true);
                        Lighting.AddLight(position, LightR, LightG, LightB);
                    }
                }
            }

            public override void PostUpdate(Item item)
            {
                if (item.createTile != -1 && ItemID.Sets.Torches[item.type])
                {
                    if (!item.wet)
                    {
                        Lighting.AddLight(item.Center, LightR, LightG, LightB);
                    }
                }
            }
        }
    }
}