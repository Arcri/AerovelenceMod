using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;
using System.Linq;

namespace AerovelenceMod.Content.Items.Tools
{
    public class ElectricBlueSolution : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemID.Sets.SortingPriorityTerraforming[Type] = 101;
            this.ModifyLocalization("ElectricBlueSolution", "Spreads Crystal Caverns")
            .AddName(Language.Default, "Electric Blue Solution").AddTooltip(Language.Default, "Spreads Crystal Caverns")
            .AddName(Language.Spanish, "Solución Azul Eléctrica").AddTooltip(Language.Spanish, "Expande las Cavernas de Cristal")
            .AddName(Language.French, "Solution Bleue Électrique").AddTooltip(Language.French, "Étend les Cavernes de Cristal")
            .AddName(Language.German, "Elektrisch Blaue Lösung").AddTooltip(Language.German, "Verbreitet Kristallhöhlen")
            .AddName(Language.Italian, "Soluzione Blu Elettrica").AddTooltip(Language.Italian, "Diffonde le Caverne di Cristallo")
            //.AddName(Language.Polish, "Elektryczny Niebieski Roztwór").AddTooltip(Language.Polish, "Rozprzestrzenia Kryształowe Jaskinie")
            //.AddName(Language.PortugueseBrazil, "Solução Azul Elétrica").AddTooltip(Language.PortugueseBrazil, "Espalha as Cavernas de Cristal")
            .AddName(Language.Russian, "Электрический Синий Раствор").AddTooltip(Language.Russian, "Распространяет Кристальные Пещеры");
            //.AddName(Language.ChineseTraditional, "電藍溶液").AddTooltip(Language.ChineseTraditional, "擴展水晶洞穴")
            //.AddName(Language.ChineseSimplified, "电蓝溶液").AddTooltip(Language.ChineseSimplified, "扩展水晶洞穴");
        }

        public override void SetDefaults()
        {
            Item.DefaultToSolution(ModContent.ProjectileType<ElectricBlueSolutionProjectile>());
            Item.value = Item.buyPrice(silver: 25);
            Item.rare = ItemRarityID.Orange;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Solutions;
        }
    }

    public class ElectricBlueSolutionProjectile : ModProjectile
    {

        public static int ConversionType;

        public ref float Progress => ref Projectile.ai[0];
        // Solutions shot by the terraformer get an increase in conversion area size, indicated by the second AI parameter being set to 1
        public bool ShotFromTerraformer => Projectile.ai[1] == 1f;

        public override void SetStaticDefaults()
        {
            // Cache the conversion type here instead of repeately fetching it every frame
            ConversionType = ModContent.GetInstance<ElectricBlueSolutionConversion>().Type;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the projectile properties to match other sprays.
            Projectile.DefaultToSpray();
            Projectile.aiStyle = 0; // Here we set aiStyle back to 0 because we have custom AI code
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {

            if (Projectile.timeLeft > 133)
                Projectile.timeLeft = 133;

            if (Projectile.owner == Main.myPlayer)
            {
                int size = ShotFromTerraformer ? 3 : 2;
                Point tileCenter = Projectile.Center.ToTileCoordinates();
                WorldGen.Convert(tileCenter.X, tileCenter.Y, ConversionType, size, true, true);
            }

            int spawnDustThreshold = 7;
            if (ShotFromTerraformer)
                spawnDustThreshold = 3;

            if (Progress > (float)spawnDustThreshold)
            {
                float dustScale = 1f;
                int dustType = DustID.BlueCrystalShard;

                if (Progress == spawnDustThreshold + 1)
                    dustScale = 0.2f;
                else if (Progress == spawnDustThreshold + 2)
                    dustScale = 0.4f;
                else if (Progress == spawnDustThreshold + 3)
                    dustScale = 0.6f;
                else if (Progress == spawnDustThreshold + 4)
                    dustScale = 0.8f;

                int dustArea = 0;
                if (ShotFromTerraformer)
                {
                    dustScale *= 1.2f;
                    dustArea = (int)(12f * dustScale);
                }

                Dust sprayDust = Dust.NewDustDirect(new Vector2(Projectile.position.X - dustArea, Projectile.position.Y - dustArea), Projectile.width + dustArea * 2, Projectile.height + dustArea * 2, dustType, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100);
                sprayDust.noGravity = true;
                sprayDust.scale *= 1.75f * dustScale;
            }

            Progress++;
            Projectile.rotation += 0.3f * Projectile.direction;
        }
    }

    public class ElectricBlueSolutionConversion : ModBiomeConversion
    {
        public static int DirtWallType;
        public static int UnsafeDirtWallType;
        public static int GrassWallType;
        public static int UnsafeGrassWallType;
        public static int StoneWallType;
        public static int UnsafeStoneWallType;
        public static int SandWallType;
        public static int UnsafeSandWallType;

        public static int DirtType;
        public static int GrassType;
        public static int StoneType;
        public static int MossType;
        public static int SandType;
        public static int ClayType;
        public static int VinesType;

        public static int Rubble1x1CeilingType;
        public static int Rubble1x1FloorType;
        public static int Rubble1x2CeilingType;
        public static int Rubble1x2FloorType;
        public static int Rubble3x2FloorType;

        public static int[] TargetRubble;

        public override void PostSetupContent()
        {

            // Cache the conversion types.
            DirtWallType = ModContent.WallType<CavernDirtWall>();
            UnsafeDirtWallType = ModContent.WallType<CavernDirtWallUnsafe>();
            GrassWallType = ModContent.WallType<CrystalGrassWall>();
            UnsafeGrassWallType = ModContent.WallType<CrystalGrassWallUnsafe>();
            StoneWallType = ModContent.WallType<CavernStoneWall>();
            UnsafeStoneWallType = ModContent.WallType<CavernStoneWallUnsafe>();
            SandWallType = ModContent.WallType<CavernSandWall>();
            UnsafeSandWallType = ModContent.WallType<CavernStoneWallUnsafe>();

            DirtType = ModContent.TileType<CrystalDirtTile>();
            GrassType = ModContent.TileType<CrystalGrassTile>();
            StoneType = ModContent.TileType<CavernStoneTile>();
            MossType = ModContent.TileType<LushGrowthTile>();
            SandType = ModContent.TileType<CavernSandTile>();
            ClayType = ModContent.TileType<ChargedStoneTile>();
            VinesType = ModContent.TileType<CrystalVines>();

            Rubble1x1CeilingType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();
            Rubble1x1FloorType = ModContent.TileType<CavernStone1x1FloorRubbleNatural>();
            Rubble1x2CeilingType = ModContent.TileType<CavernStone1x2CeilingRubbleNatural>();
            Rubble1x2FloorType = ModContent.TileType<CavernStone1x2FloorRubbleNatural>();
            Rubble3x2FloorType = ModContent.TileType<CavernStone3x2FloorRubbleNatural>();
            TargetRubble = [TileID.Stalactite, TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2];

            for (int i = 0; i < WallLoader.WallCount; i++)
            {
                int targetWallType = -1;
                if (WallID.Sets.Conversion.Dirt[i] ||
                        i == WallID.Cave6Unsafe ||
                        i == WallID.CaveWall ||
                        i == WallID.CaveWall2 ||
                        i == WallID.DirtUnsafe1 ||
                        i == WallID.DirtUnsafe2 ||
                        i == WallID.DirtUnsafe3 ||
                        i == WallID.DirtUnsafe4)
                    targetWallType = UnsafeDirtWallType;
                else if (WallID.Sets.Conversion.Grass[i])
                    targetWallType = UnsafeGrassWallType;
                else if (WallID.Sets.Conversion.Stone[i] ||
                        WallID.Sets.Conversion.NewWall1[i] || // NewWalls are the underground wall variants
                        WallID.Sets.Conversion.NewWall2[i] ||
                        WallID.Sets.Conversion.NewWall3[i] ||
                        WallID.Sets.Conversion.NewWall4[i] ||
                        i == WallID.RocksUnsafe1 ||
                        i == WallID.RocksUnsafe2 ||
                        i == WallID.RocksUnsafe3 ||
                        i == WallID.RocksUnsafe4 ||
                        i == WallID.CaveUnsafe ||
                        i == WallID.Cave2Unsafe ||
                        i == WallID.Cave3Unsafe ||
                        i == WallID.Cave4Unsafe ||
                        i == WallID.Cave5Unsafe ||
                        i == WallID.Cave7Unsafe ||
                        i == WallID.Cave8Unsafe)
                    targetWallType = UnsafeStoneWallType;
                else if (WallID.Sets.Conversion.Sandstone[i] ||
                        WallID.Sets.Conversion.HardenedSand[i] ||
                        WallID.Sets.Conversion.PureSand[i])
                    targetWallType = UnsafeSandWallType;
                if (targetWallType != -1)
                    WallLoader.RegisterSimpleConversion(i, Type, targetWallType);
            }

            // We register a conversion method and fallback separately rather than using RegisterSimpleConversion, because ConvertGrass has custom logic for converting trees on the tile above
            TileLoader.RegisterConversion(TileID.Grass, Type, ConvertGrass);
            TileLoader.RegisterConversionFallback(GrassType, TileID.Grass, Type);

            bool Purify(int i, int j, int type, int conversionType)
            {
                WorldGen.ConvertTile(i, j, TileID.Grass);
                return false;
            }
            TileLoader.RegisterConversion(GrassType, BiomeConversionID.Purity, Purify);
            TileLoader.RegisterConversion(GrassType, BiomeConversionID.PurificationPowder, Purify);
            TileLoader.RegisterConversion(GrassType, BiomeConversionID.Chlorophyte, Purify);

            // This registers a conversion from the base tile to the modded tile, as well as a fallback from the modded tile to the base tile, so other solutions can convert the modded tile (eg to Ebonstone)
            TileLoader.RegisterSimpleConversion(TileID.Dirt, Type, DirtType);
            TileLoader.RegisterConversion(TileID.Stone, Type, ConvertStone);
            TileLoader.RegisterConversion(TileID.GreenMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.BrownMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.RedMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.BlueMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.PurpleMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.LavaMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.KryptonMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.XenonMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.ArgonMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.VioletMoss, Type, ConvertMoss);
            TileLoader.RegisterConversion(TileID.RainbowMoss, Type, ConvertMoss);
            TileLoader.RegisterSimpleConversion(TileID.Sandstone, Type, SandType);
            TileLoader.RegisterSimpleConversion(TileID.HardenedSand, Type, SandType);
            TileLoader.RegisterSimpleConversion(TileID.Sand, Type, SandType);
            TileLoader.RegisterSimpleConversion(TileID.ClayBlock, Type, ClayType);
        }

        public bool ConvertGrass(int i, int j, int type, int conversionType)
        {

            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            FindAndConvertTree(i, j, tileTypeAbove);

            WorldGen.ConvertTile(i, j, GrassType);

            return false;
        }

        public bool ConvertStone(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            int tileTypeBelow = -1;
            if (j > 1 && Main.tile[i, j + 1].HasTile)
                tileTypeBelow = Main.tile[i, j + 1].TileType;

            ConvertRubble(i, j - 1, tileTypeAbove, conversionType);
            ConvertRubble(i, j + 1, tileTypeBelow, conversionType);

            WorldGen.ConvertTile(i, j, StoneType);

            return false;
        }

        public bool ConvertMoss(int i, int j, int type, int conversionType)
        {
            int tileTypeAbove = -1;
            if (j > 1 && Main.tile[i, j - 1].HasTile)
                tileTypeAbove = Main.tile[i, j - 1].TileType;

            int tileTypeBelow = -1;
            if (j > 1 && Main.tile[i, j + 1].HasTile)
                tileTypeBelow = Main.tile[i, j + 1].TileType;

            ConvertRubble(i, j - 1, tileTypeAbove, conversionType);
            ConvertRubble(i, j + 1, tileTypeBelow, conversionType);

            WorldGen.ConvertTile(i, j, MossType);

            return false;
        }

        public void FindAndConvertTree(int i, int j, int tileTypeAbove)
        {

            if (tileTypeAbove == -1)
                return;

            if (!TileID.Sets.IsATreeTrunk[tileTypeAbove])
                return;

            int treeBottom = j;
            int treeTop = treeBottom - 1;
            int treeCenterX = i;

            // Check for if the tile is the tree's "trunk" or just the root tiles on the side
            // We do this by checking for the specific tile frame of the tree tile.
            // Necessary because the "IsATreeTrunk" ID set doesn't care about the tile's frame and returns true even if the tile isnt the tree's "trunk"
            int treeFrameX = Main.tile[treeCenterX, treeTop].TileFrameX / 22;
            int treeFrameY = Main.tile[treeCenterX, treeTop].TileFrameY / 22;
            bool isTreeTrunk = (treeFrameX != 1 && treeFrameX != 2) || treeFrameY < 6;

            // Niche edgecase check: If a grass block was placed under a tree's branch, it shouldnt be converted at all, as it is not actually attached to the grass tile below
            bool isTreeBranch = (treeFrameX == 3 && treeFrameY < 3) || (treeFrameX == 4 && treeFrameY >= 3 && treeFrameY < 6);
            if (isTreeBranch)
                return;

            // If the tile above wasn't a tree trunk but instead a root tile on the side, check the adjacent two tiles to find it
            if (!isTreeTrunk)
            {
                for (int x = treeCenterX - 1; x < treeCenterX + 2; x += 2)
                {

                    Tile topTile = Main.tile[x, treeTop];
                    if (!topTile.HasTile || !TileID.Sets.IsATreeTrunk[topTile.TileType])
                        continue;

                    // Check for tree trunk framing
                    treeFrameX = topTile.TileFrameX / 22;
                    treeFrameY = topTile.TileFrameY / 22;
                    isTreeTrunk = (treeFrameX != 1 && treeFrameX != 2) || treeFrameY < 6;

                    // We found our tree trunk center
                    if (isTreeTrunk)
                    {
                        treeCenterX = x;
                        break;
                    }
                }
            }

            // Find the top of the tree by repeatedly going up until we don't find any more tree tiles
            while (treeTop >= 0 && Main.tile[treeCenterX, treeTop].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[treeCenterX, treeTop].TileType])
                treeTop--;

            // Turn all the tiles around it into hell trees
            for (int x = treeCenterX - 1; x < treeCenterX + 2; x++)
            {
                for (int y = treeTop; y < treeBottom; y++)
                {
                    Tile t = Main.tile[x, y];
                    if (t.HasTile && TileID.Sets.IsATreeTrunk[t.TileType])
                        t.TileType = TileID.Trees;
                }
            }

            // Turn the floor into grass (We have to convert the adjacent tiles, otherwise the side root tiles may get broken)
            // The framing will happen naturally when the floor tile below gets converted and frames the other adjacent tiles, so we don't need to use WorldGen.Convert here
            for (int x = treeCenterX - 1; x < treeCenterX + 2; x++)
            {
                Tile t = Main.tile[x, treeBottom];
                if (t.HasTile && TileID.Sets.Conversion.Grass[t.TileType] || TileID.Sets.Conversion.GolfGrass[t.TileType])
                    t.TileType = (ushort)GrassType;
            }
        }

        public bool ConvertRubble(int i, int j, int type, int conversionType)
        {
            int desiredRubbleType = -1;
            bool shortRubble = false;

            short resultingVariants;
            short frameVariant;

            if (!TargetRubble.Contains(type))
                return false;

            Tile tileTarget = Main.tile[i, j];

            Tile tileAbove = Main.tile[i, j - 1];
            int tileTypeAbove = -1;
            if (j > 1 && tileAbove.HasTile)
                tileTypeAbove = tileAbove.TileType;

            Tile tileBelow = Main.tile[i, j + 1];
            int tileTypeBelow = -1;
            if (j > -1 && tileBelow.HasTile)
                tileTypeBelow = tileBelow.TileType;

            Tile tileRight = Main.tile[i + 1, j];
            int tileTypeRight = -1;
            if (i > -1 && tileRight.HasTile)
                tileTypeRight = tileRight.TileType;

            Tile tileLeft = Main.tile[i - 1, j];
            int tileTypeLeft = -1;
            if (i > 1 && tileLeft.HasTile)
                tileTypeLeft = tileLeft.TileType;

            Tile tileTwiceLeft = Main.tile[i - 2, j];
            int tileTypeTwiceLeft = -1;
            if (i > 2 && tileTwiceLeft.HasTile)
                tileTypeTwiceLeft = tileTwiceLeft.TileType;

            Tile tileAboveLeft = Main.tile[i - 1, j - 1];
            int tileTypeAboveLeft = -1;
            if (i > 1 && j > 1 && tileAboveLeft.HasTile)
                tileTypeAboveLeft = tileAboveLeft.TileType;

            Tile tileAboveTwiceLeft = Main.tile[i - 2, j - 1];
            int tileTypeAboveTwiceLeft = -1;
            if (i > 2 && j > 1 && tileAboveTwiceLeft.HasTile)
                tileTypeAboveTwiceLeft = tileAboveTwiceLeft.TileType;

            // Ensure operation is done on bottom and right-most tile of the rubble
            if (tileBelow.HasTile && tileBelow.TileType == type)
            {
                return ConvertRubble(i, j + 1, type, conversionType);
            }
            // Small pile handling of this is after tile replacement to avoid infinite loops back and forth
            if (tileRight.HasTile && tileRight.TileType == type && !(type == TileID.Stalactite || type == TileID.SmallPiles))
            {
                return ConvertRubble(i + 1, j, type, conversionType);
            }

            if ((!tileBelow.HasTile && tileAbove.HasTile && tileAbove.TileType != type) ||
                (!tileAbove.HasTile && tileBelow.HasTile && tileBelow.TileType != type))
            {
                shortRubble = true;
            }

            switch (type)
            {
                case TileID.Stalactite:
                    if (tileBelow.HasTile && Main.tileSolid[tileBelow.TileType] && !tileBelow.TopSlope && !tileBelow.IsHalfBlock)
                    {
                        if (shortRubble)
                        {
                            desiredRubbleType = Rubble1x1FloorType;
                            resultingVariants = 12;
                        }
                        else
                        {
                            desiredRubbleType = Rubble1x2FloorType;
                            resultingVariants = 6;
                        }
                    }
                    else
                    {
                        if (shortRubble)
                        {
                            desiredRubbleType = Rubble1x1CeilingType;
                            resultingVariants = 6;
                        }
                        else
                        {
                            desiredRubbleType = Rubble1x2CeilingType;
                            resultingVariants = 5;
                        }
                    }

                    if (shortRubble)
                    {
                        tileTarget.TileType = (ushort)desiredRubbleType;

                        tileTarget.TileFrameX = (short)(WorldGen.genRand.Next(resultingVariants) * 18);

                        tileTarget.TileFrameY = 0;

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendTileSquare(-1, i, j);
                        }
                    }
                    else
                    {
                        tileAbove.TileType = (ushort)desiredRubbleType;
                        tileTarget.TileType = (ushort)desiredRubbleType;

                        frameVariant = (short)(WorldGen.genRand.Next(resultingVariants) * 18);
                        tileAbove.TileFrameX = frameVariant;
                        tileTarget.TileFrameX = frameVariant;

                        tileAbove.TileFrameY = 0;
                        tileTarget.TileFrameY = 18;

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendTileSquare(-1, i, j - 1, 1, 2);
                        }   
                    }

                    // WorldGen.ConvertTile() Causes rubble tiles to break. Manually handling tile framing and typing solves the issue. 
                    //WorldGen.ConvertTile(i, j, desiredRubbleType);
                    break;

                case TileID.SmallPiles:
                    // Would have included logic for 2x1 rubble but we don't have assets for it so all small piles will become 1x1 rubble
                    desiredRubbleType = Rubble1x1FloorType;
                    resultingVariants = 12;

                    tileTarget.TileType = (ushort)desiredRubbleType;

                    frameVariant = (short)(WorldGen.genRand.Next(resultingVariants));
                    tileTarget.TileFrameX = (short)(frameVariant * 18);

                    tileTarget.TileFrameY = 0;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i, j);
                    }

                    // Necessary or else adjacent strings of small piles would be replaced with a single one
                    if (tileLeft.HasTile && tileLeft.TileType == TileID.SmallPiles)
                    {
                        ConvertRubble(i - 1, j, type, conversionType);
                    }
                    else if (tileRight.HasTile && tileRight.TileType == TileID.SmallPiles)
                    {
                        ConvertRubble(i + 1, j, type, conversionType);
                    }

                    // WorldGen.ConvertTile() Causes rubble tiles to break. Manually handling tile framing and typing solves the issue. 
                    //WorldGen.ConvertTile(i, j, desiredRubbleType);
                    break;

                case TileID.LargePiles:
                case TileID.LargePiles2:
                    // Thankfully for once all rubble of these tile ids are the same size
                    desiredRubbleType = Rubble3x2FloorType;
                    resultingVariants = 6;

                    tileAboveTwiceLeft.TileType = (ushort)desiredRubbleType;
                    tileAboveLeft.TileType = (ushort)desiredRubbleType;
                    tileAbove.TileType = (ushort)desiredRubbleType;
                    tileTwiceLeft.TileType = (ushort)desiredRubbleType;
                    tileLeft.TileType = (ushort)desiredRubbleType;
                    tileTarget.TileType = (ushort)desiredRubbleType;

                    frameVariant = (short)WorldGen.genRand.Next(resultingVariants);
                    tileAboveTwiceLeft.TileFrameX = (short)(frameVariant * 3 * 18);
                    tileAboveLeft.TileFrameX = (short)((frameVariant * 3 + 1) * 18);
                    tileAbove.TileFrameX = (short)((frameVariant * 3 + 2) * 18);
                    tileTwiceLeft.TileFrameX = (short)(frameVariant * 3 * 18);
                    tileLeft.TileFrameX = (short)((frameVariant * 3 + 1) * 18);
                    tileTarget.TileFrameX = (short)((frameVariant * 3 + 2) * 18);

                    tileAboveTwiceLeft.TileFrameY = 0;
                    tileAboveLeft.TileFrameY = 0;
                    tileAbove.TileFrameY = 0;
                    tileTwiceLeft.TileFrameY = 18;
                    tileLeft.TileFrameY = 18;
                    tileTarget.TileFrameY = 18;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i - 2, j - 1, 3, 2);
                    }

                    // WorldGen.ConvertTile() Causes rubble tiles to break. Manually handling tile framing and typing solves the issue. 
                    //WorldGen.ConvertTile(i, j, desiredRubbleType);
                    break;
            }

            return false;
        }

        //public bool ConvertChairs(int i, int j, int type, int conversionType)
        //{
        //    // Find the bottom of the chair
        //    if (Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType == TileID.Chairs)
        //        j++;

        //    Tile tileTop = Main.tile[i, j - 1];
        //    Tile tileBottom = Main.tile[i, j];

        //    // Manually convert the top part of the chair, and then the bottom half through WorldGen.Convert so it automatically handles the framing and syncing
        //    tileTop.TileType = (ushort)ChairType;


        //    // Reset the Y frame to be within the bounds of examplechair's tilesheet
        //    tileTop.TileFrameY = 0;
        //    tileBottom.TileFrameY = 18;

        //    WorldGen.ConvertTile(i, j, ChairType);
        //    return false;
        //}

        //public bool ConvertWorkbenches(int i, int j, int type, int conversionType)
        //{
        //    // Find the right of the workbench
        //    if (Main.tile[i + 1, j].HasTile && Main.tile[i + 1, j].TileType == TileID.WorkBenches)
        //        i++;

        //    Tile tileLeft = Main.tile[i - 1, j];
        //    Tile tileRight = Main.tile[i, j];

        //    // Manually convert the right part of the workbench, and then the right half through WorldGen.Convert so it automatically handles the framing and syncing
        //    tileLeft.TileType = (ushort)WorkbenchType;

        //    // Reset the X frame to be within the bounds of exampleworkbench's tilesheet
        //    tileLeft.TileFrameX = 0;
        //    tileRight.TileFrameX = 18;

        //    WorldGen.ConvertTile(i, j, WorkbenchType);
        //    return false;
        //}

        //public enum ConvertType
        //{
        //    Pure,
        //    Corrupt,
        //    Crimson,
        //    Hallow
        //}
    }
}