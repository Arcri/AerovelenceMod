using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using Terraria.Utilities;

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
                UnifiedRandom rand = WorldGen.genRand;
                List<PrimaryItemConfiguration> crystalShrinePrimary =
[
    new(ItemID.BandofRegeneration, 1, 1, 1f),
                new(ItemID.MagicMirror, 1, 1, 1f),
                new(ItemID.CloudinaBottle, 1, 1, 1f),
                new(ItemID.HermesBoots, 1, 1, 1f),
                new(ItemID.EnchantedBoomerang, 1, 1, 1f),
                new(ItemID.ShoeSpikes, 1, 1, 1f),
                new(ItemID.FlareGun, 1, 1, 1f),
                new(ItemID.Extractinator, 1, 1, 1f),
                new(ItemID.LavaCharm, 1, 1, 1f),
                new(ItemID.LuckyHorseshoe, 1, 1, 1f),
                new(ModContent.ItemType<Eos>(), 1, 1, 1f)
];

                List<ItemConfiguration> crystalShrineSecondary =
                [
                    new(ItemID.SuspiciousLookingEye, 1, 1),
                new(ItemID.Dynamite, 1, 1),
                new(ItemID.JestersArrow, 25, 50),
                new([ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar], 3, 10),
                new([ItemID.FlamingArrow, ItemID.ThrowingKnife], 25, 50),
                new(ItemID.HealingPotion, 3, 5),
                new(
                [
                    ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                    ItemID.ArcheryPotion, ItemID.GravitationPotion, ItemID.ThornsPotion, ItemID.InvisibilityPotion,
                    ItemID.HunterPotion, ItemID.BattlePotion, ItemID.TeleportationPotion
                ], 1, 2),
                new(ItemID.RecallPotion, 1, 2),
                new([ItemID.Torch, ItemID.Glowstick], 15, 29),
                new(ItemID.GoldCoin, 1, 2)
                ];

                AeroStructure crystalShrine = PlaceStructureSafely(player, "crystalshrine", 20, 20, 5000).ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);
            }
            else
            {
                StructureStamper.LoadStructure(player.position.ToTileCoordinates().ToVector2(), "crystalshrine");
            }

            return true;
        }

        private AeroStructure PlaceStructureSafely(Player player, string name, int xMarginPercentage = 10, int yMarginPercentage = 10, int attempts = 5000)
        {
            AeroStructure structure = AeroStructure.Empty;
            Vector2 playerPosition = player.Center.ToTileCoordinates().ToVector2();
            for (int i = 0; i < attempts; i++)
            {
                Vector2 centeredPosition = playerPosition - new Vector2(structure.Width / 2, structure.Height / 2);
                structure = StructureStamper.LoadStructure(centeredPosition, name, placeStructure: false, checkIfProtected: false);
                if (structure != AeroStructure.Empty)
                {
                    centeredPosition = centeredPosition.MoveTowards(new Vector2(centeredPosition.X - structure.Width / 2, centeredPosition.Y - structure.Height / 2), float.MaxValue);
                    structure = StructureStamper.LoadStructure(centeredPosition, name, placeStructure: false, checkIfProtected: false);
                }
                if (structure != AeroStructure.Empty)
                {
                    structure = StructureStamper.LoadStructure(centeredPosition, name, checkIfProtected: false);
                    return structure;
                }
            }
            //Console.WriteLine("Failed to load structure " + name);
            return structure;
        }
    }
}