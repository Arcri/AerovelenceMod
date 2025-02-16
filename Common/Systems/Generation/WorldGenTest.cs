using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Terraria.Utilities;
using static Terraria.WorldBuilding.Shapes;
using ReLogic.Utilities;
using System;
using AerovelenceMod.Common.Systems.Generation.GenUtils;
using AerovelenceMod.Common.Utilities.StructureStamper;
using static Terraria.Collision;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using System.Collections.Generic;

namespace AerovelenceMod.Common.Systems.Generation
{
    public class WorldGenTest : ModSystem
    {
		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.RightAlt))
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
        }
        private void TestMethod(int x, int y)
        {
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            //WorldUtils.Gen(new Point(x, y), new Shapes.Tail(8, new Vector2D(WorldGen.genRand.Next(-25, 25), WorldGen.genRand.Next(-20, 20))), new Actions.SetTile((ushort)ModContent.TileType<ChargedStone>()));

            
            // Code to test placed here:
            Point origin = new Point(x, y);
            UnifiedRandom rand = new UnifiedRandom(); // Use WorldGen.genRand.Next() for actual world generation

            List<PrimaryItemConfiguration> primaryItems =
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

            List<ItemConfiguration> secondaryItems =
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

            StructureStamper.LoadStructure(new Vector2(x, y), "smallchestshrine").ApplyItemConfigurationsToAll(rand, primaryItems, secondaryItems);
            
            
            //StructureStamper.LoadStructure(new Vector2(x, y), "tumblerarena");

            //WorldUtils.Gen(origin, new AeroShapes.LightningBoltShape(350, 50, 3, 150, 30), new Actions.SetTile(TileID.Bubble));

            /// Creates an opening similar to the surface caves
            //WorldGen.CaveOpenater(origin.X, origin.Y);

            /// Creates a medium-sized, blobby underground cave
            //WorldGen.Caverer(origin.X, origin.Y);

            /// Creates a long, winding cave, presumably used in vanilla at the end of CaveOpenater's generation 
            //WorldGen.Cavinator(origin.X, origin.Y, 50);

            /// Creates a corruption chasm, extends existing corruption caves
            //WorldGen.ChasmRunner(origin.X, origin.Y, 50, true);

            /// Creates a single, configurable tunnel in a straight line extending from the origin. The result will not be the same every time.
            //WorldGen.digTunnel(origin.X, origin.Y, 1, 1, 50, 3);
        }
    }
}