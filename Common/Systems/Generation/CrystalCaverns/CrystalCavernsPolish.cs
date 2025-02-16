using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Common.Utilities.StructureStamper;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Systems.Generation.CrystalCaverns
{
    public class CrystalCavernsPolish : GenPass
    {
        public CrystalCavernsPolish(string name, double loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = WorldGenSystem.CrystalCavernsTerrainPassMessage.Value;
            CrystalCavernsTerrainPass mainPass = CrystalCavernsTerrainPass.Instance();
            #region CC Gen Cleanup
            Point surfaceRectOrigin = new Point(mainPass.Origin.X - mainPass.BiomeWidth / 2, mainPass.Origin.Y - (int)(mainPass.SurfaceHeight * 1.75));
            ShapeData surfaceRectShapeData = new ShapeData();
            WorldUtils.Gen(surfaceRectOrigin, new Shapes.Rectangle(mainPass.BiomeWidth, (int)(mainPass.SurfaceHeight * 1.75)), new Actions.Blank().Output(surfaceRectShapeData));

            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall)
            }));
            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.Expand(3, 0),
                new Modifiers.Dither(0.75),
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall)
            }));
            WorldUtils.Gen(surfaceRectOrigin, new ModShapes.All(surfaceRectShapeData), Actions.Chain(new GenAction[]
            {
                new Modifiers.Expand(5, 0),
                new Modifiers.Dither(0.75),
                new Modifiers.OnlyWalls(WallID.DirtUnsafe, WallID.FlowerUnsafe, WallID.GrassUnsafe, 59, WallID.SnowWallUnsafe, WallID.Sandstone, WallID.HardenedSand),
                new Actions.PlaceWall(mainPass.DirtWall),
            }));
            #endregion

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

            AeroStructure tumblerArena = StructureStamper.LoadStructure(new Vector2(mainPass.TumblerTunnelEnd.X - 60 + 60 * mainPass.TumblerArenaPolarity, mainPass.TumblerTunnelEnd.Y - 46), "tumblerarena")
                .ProtectStructure();

            AeroStructure crystalShrine = PlaceStructureSafely("crystalshrine", 20, 20)
                .ProtectStructure()
                .ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);

            for (int i = 0; i < 100; i++)
            {
                PlaceStructureSafely("crystalshrine", 20, 20)
                .ProtectStructure()
                .ApplyItemConfigurationsToAll(rand, crystalShrinePrimary, crystalShrineSecondary);
            }

        }

        private AeroStructure PlaceStructureSafely(string name, int xMarginPercentage = 10, int yMarginPercentage = 10, int attempts = 5000)
        {
            AeroStructure structure = AeroStructure.Empty;
            for (int i = 0; i < attempts; i++)
            {
                Vector2 randVector = GetRandomUndergroundVector();
                structure = StructureStamper.LoadStructure(randVector, name, placeStructure: false, checkIfProtected: true);
                if (structure != AeroStructure.Empty)
                {
                    randVector = randVector.MoveTowards(new Vector2(randVector.X - structure.Width / 2, randVector.Y - structure.Height / 2), float.MaxValue);
                    structure = StructureStamper.LoadStructure(randVector, name, placeStructure: false, checkIfProtected: true);
                }
                if (structure != AeroStructure.Empty) 
                {
                    structure = StructureStamper.LoadStructure(randVector, name, checkIfProtected: true);
                    return structure;
                }
            }
            //Console.WriteLine("Failed to load structure " + name);
            return structure;
        }

        private Vector2 GetRandomUndergroundVector(int xMarginPercentage = 10, int yMarginPercentage = 10)
        {
            CrystalCavernsTerrainPass mainPass = CrystalCavernsTerrainPass.Instance();
            UnifiedRandom rand = WorldGen.genRand;
            int x = mainPass.Origin.X - mainPass.BiomeWidth / 2 + (int)(rand.NextFloat(xMarginPercentage * 0.01f, 1 - xMarginPercentage * 0.01f) * mainPass.BiomeWidth);
            int y = mainPass.Origin.Y + (int)(rand.NextFloat(yMarginPercentage * 0.01f, 1 - yMarginPercentage * 0.01f) * mainPass.BiomeHeight);
            Console.WriteLine(x.ToString() + y.ToString());
            return new Vector2(x, y);
        }
    }
}
