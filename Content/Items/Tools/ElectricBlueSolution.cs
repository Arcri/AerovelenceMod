using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using AerovelenceMod.Content.Walls.CrystalCaverns.Natural;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural.Flora;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture;
using System.Net.Mime;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble;

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
        //public ref float Progress => ref Projectile.ai[0];

        //public override void SetDefaults()
        //{
        //    Projectile.DefaultToSpray();
        //    Projectile.aiStyle = 0;
        //}

        //public override void AI()
        //{
        //    int dustType = DustID.BlueCrystalShard;

        //    if (Projectile.owner == Main.myPlayer)
        //    {
        //        Convert((int)(Projectile.position.X + (Projectile.width * 0.5f)) / 16, (int)(Projectile.position.Y + (Projectile.height * 0.5f)) / 16, 2);
        //    }

        //    if (Projectile.timeLeft > 133)
        //    {
        //        Projectile.timeLeft = 133;
        //    }

        //    if (Progress > 7f)
        //    {
        //        float dustScale = 1f;

        //        if (Progress == 8f)
        //        {
        //            dustScale = 0.2f;
        //        }
        //        else if (Progress == 9f)
        //        {
        //            dustScale = 0.4f;
        //        }
        //        else if (Progress == 10f)
        //        {
        //            dustScale = 0.6f;
        //        }
        //        else if (Progress == 11f)
        //        {
        //            dustScale = 0.8f;
        //        }

        //        Progress += 1f;

        //        var dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

        //        dust.noGravity = true;
        //        dust.scale *= 1.75f;
        //        dust.velocity.X *= 2f;
        //        dust.velocity.Y *= 2f;
        //        dust.scale *= dustScale;
        //    }
        //    else
        //    {
        //        Progress += 1f;
        //    }

        //    Projectile.rotation += 0.3f * Projectile.direction;
        //}

        //private static void Convert(int i, int j, int size = 4)
        //{
        //    for (int k = i - size; k <= i + size; k++)
        //    {
        //        for (int l = j - size; l <= j + size; l++)
        //        {
        //            if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
        //            {
        //                ConvertTile(k, l);
        //            }
        //        }
        //    }
        //}

        //private static void ConvertTile(int x, int y)
        //{
        //    Tile tile = Main.tile[x, y];

        //    if (tile == null)
        //        return;
        //    int type = tile.TileType;
        //    int wall = tile.WallType;
        //    if (TileID.Sets.Conversion.JungleGrass[type] || type == TileID.JungleGrass || type == TileID.Mud)
        //        return;
        //    if (wall != 0 && !IsProtectedWall(wall))
        //    {
        //        ConvertWall(x, y, wall);
        //    }
        //    if (!IsProtectedTile(type))
        //    {
        //        ConvertTileType(x, y, type);
        //        if (type == TileID.Dirt || type == TileID.SnowBlock ||
        //            type == ModContent.TileType<CrystalDirtTile>())
        //        {
        //            CheckAndConvertToCrystalGrass(x, y);
        //        }
        //    }
        //}

        //private static bool IsProtectedWall(int wallType)
        //{
        //    return wallType == ModContent.WallType<CavernDirtWall>() ||
        //           wallType == ModContent.WallType<CavernDirtWallUnsafe>() ||
        //           wallType == ModContent.WallType<CavernSandWallUnsafe>() ||
        //           wallType == ModContent.WallType<CavernStoneWall>() ||
        //           wallType == ModContent.WallType<CavernStoneWallUnsafe>() ||
        //           wallType == ModContent.WallType<CitadelBrickWall>() ||
        //           wallType == ModContent.WallType<ColumnWall>() ||
        //           wallType == ModContent.WallType<CrystalGrassWall>() ||
        //           wallType == ModContent.WallType<CrystalGrassWallUnsafe>() ||
        //           wallType == ModContent.WallType<GlimmerwoodPlankedWall>() ||
        //           wallType == ModContent.WallType<GlimmerwoodWall>() ||
        //           wallType == ModContent.WallType<LushGrowthWall>();

        //}

        //private static bool IsProtectedTile(int tileType)
        //{
        //    return tileType == ModContent.TileType<CrystalGrassTile>() ||
        //           tileType == ModContent.TileType<CavernStoneTile>() ||
        //           tileType == ModContent.TileType<CavernSandTile>() ||
        //           tileType == ModContent.TileType<CrystalVines>();
        //}

        //private static void ConvertWall(int x, int y, int wallType)
        //{
        //    if (WallID.Sets.Conversion.Dirt[wallType])
        //    {
        //        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWallUnsafe>();
        //        WorldGen.SquareWallFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (WallID.Sets.Conversion.Grass[wallType])
        //    {
        //        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CrystalGrassWallUnsafe>();
        //        WorldGen.SquareWallFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (WallID.Sets.Conversion.HardenedSand[wallType])
        //    {
        //        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernSandWallUnsafe>();
        //        WorldGen.SquareWallFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (WallID.Sets.Conversion.Sandstone[wallType])
        //    {
        //        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernSandWallUnsafe>();
        //        WorldGen.SquareWallFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (WallID.Sets.Conversion.Stone[wallType])
        //    {
        //        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWallUnsafe>();
        //        WorldGen.SquareWallFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else
        //    {
        //        switch (wallType)
        //        {
        //            case WallID.DirtUnsafe:
        //            case WallID.DirtUnsafe1:
        //            case WallID.DirtUnsafe2:
        //            case WallID.DirtUnsafe3:
        //            case WallID.DirtUnsafe4:
        //            case WallID.CaveUnsafe:
        //            case WallID.Cave2Unsafe:
        //            case WallID.Cave3Unsafe:
        //            case WallID.Cave4Unsafe:
        //            case WallID.Cave5Unsafe:
        //            case WallID.Cave6Unsafe:
        //            case WallID.Cave7Unsafe:
        //            case WallID.Cave8Unsafe:
        //            case WallID.Dirt:
        //                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWallUnsafe>();
        //                WorldGen.SquareWallFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            /*case WallID.SnowWallUnsafe:
        //                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWall>();
        //                WorldGen.SquareWallFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;*/
        //            /*case WallID.IceUnsafe:
        //                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWall>();
        //                WorldGen.SquareWallFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;*/
        //            /*case WallID.LivingWoodUnsafe:
        //                Main.tile[x, y].WallType = (ushort)ModContent.WallType<GlimmerwoodWall>();
        //                WorldGen.SquareWallFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;*/
        //        }
        //    }
        //}

        //private static void ConvertTileType(int x, int y, int tileType)
        //{
        //    if (IsProtectedTile(tileType))
        //        return;

        //    if (TileID.Sets.Conversion.Grass[tileType] && !TileID.Sets.GrassSpecial[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalGrassTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (TileID.Sets.Conversion.Stone[tileType] || Main.tileMoss[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (TileID.Sets.Conversion.Sand[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernSandTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (TileID.Sets.Conversion.HardenedSand[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (TileID.Sets.Conversion.Sandstone[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }
        //    else if (TileID.Sets.Conversion.Ice[tileType])
        //    {
        //        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
        //        WorldGen.SquareTileFrame(x, y, true);
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //    }

        //    else
        //    {
        //        switch (tileType)
        //        {
        //            case TileID.Dirt:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.SnowBlock:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.Silt:
        //            case TileID.Slush:
        //            case TileID.DesertFossil:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.ClayBlock:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.Vines:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalVines>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.LivingWood:
        //                Main.tile[x, y].TileType = (ushort)ModContent.TileType<FreshGlimmerwoodTile>();
        //                WorldGen.SquareTileFrame(x, y, true);
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;
        //            case TileID.LeafBlock:
        //            case TileID.Sunflower:
        //            case TileID.Plants:
        //            case TileID.Plants2:
        //            case TileID.JunglePlants:
        //            case TileID.JunglePlants2:
        //            case TileID.CorruptPlants:
        //            case TileID.CrimsonPlants:
        //            case TileID.HallowedPlants:
        //            case TileID.HallowedPlants2:
        //                WorldGen.KillTile(x, y);
        //                if (Main.netMode == NetmodeID.MultiplayerClient)
        //                {
        //                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
        //                }
        //                break;
        //            case TileID.Stalactite:
        //            case TileID.SmallPiles:
        //            case TileID.LargePiles:
        //            case TileID.LargePiles2:
        //                //Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStone3x2FloorRubbleNatural>();
        //                //WorldGen.SquareTileFrame(x, y, true);
        //                //NetMessage.SendTileSquare(-1, x, y, 1);
        //                break;

        //        }
        //    }
        //}

        //private static void CheckAndConvertToCrystalGrass(int i, int j)
        //{
        //    if (Main.tile[i, j].TileType != ModContent.TileType<CrystalDirtTile>())
        //        return;
        //    bool exposedToAir = false;
        //    if (IsExposedToAir(i, j - 1))
        //        exposedToAir = true;
        //    else if (IsExposedToAir(i, j + 1))
        //        exposedToAir = true;
        //    else if (IsExposedToAir(i - 1, j))
        //        exposedToAir = true;
        //    else if (IsExposedToAir(i + 1, j))
        //        exposedToAir = true;
        //    if (exposedToAir)
        //    {
        //        Main.tile[i, j].TileType = (ushort)ModContent.TileType<CrystalGrassTile>();
        //        WorldGen.SquareTileFrame(i, j, true);
        //        NetMessage.SendTileSquare(-1, i, j, 1);
        //    }
        //}

        //private static bool IsExposedToAir(int i, int j)
        //{
        //    if (!WorldGen.InWorld(i, j))
        //        return false;
        //    Tile tile = Main.tile[i, j];
        //    if (tile == null)
        //        return true;
        //    if (!tile.HasTile || !Main.tileSolid[tile.TileType])
        //        return true;
        //    return false;
        //}

        //public static void ConvertFromCrystalCavern(int startX, int endX, int startY, int endY, ConvertType convert)
        //{

        //    for (int x = startX; x <= endX; x++)
        //    {
        //        for (int y = startY; y <= endY; y++)
        //        {
        //            ConvertFromCrystalCavern(x, y, convert);
        //        }
        //    }
        //}

        //public static void ConvertFromCrystalCavern(int x, int y, ConvertType convert, bool tileframe = true)
        //{
        //    Tile tile = Main.tile[x, y];
        //    if (tile == null)
        //        return;

        //    int type = tile.TileType;
        //    int wallType = tile.WallType;

        //    if (WorldGen.InWorld(x, y, 1))
        //    {
        //        if (wallType == ModContent.WallType<CavernDirtWall>() || wallType == ModContent.WallType<CavernDirtWallUnsafe>())
        //        {
        //            Main.tile[x, y].WallType = WallID.DirtUnsafe;
        //        }
        //        else if (wallType == ModContent.WallType<CavernStoneWall>() || wallType == ModContent.WallType<CavernStoneWallUnsafe>())
        //        {
        //            switch (convert)
        //            {
        //                case ConvertType.Corrupt:
        //                    Main.tile[x, y].WallType = WallID.EbonstoneUnsafe;
        //                    break;
        //                case ConvertType.Crimson:
        //                    Main.tile[x, y].WallType = WallID.CrimstoneUnsafe;
        //                    break;
        //                case ConvertType.Hallow:
        //                    Main.tile[x, y].WallType = WallID.PearlstoneBrickUnsafe;
        //                    break;
        //                case ConvertType.Pure:
        //                    Main.tile[x, y].WallType = WallID.Stone;
        //                    break;
        //            }
        //        }
        //        else if (wallType == ModContent.WallType<CavernSandWallUnsafe>())
        //            Main.tile[x, y].WallType = WallID.Sandstone;
        //        else if (wallType == ModContent.WallType<GlimmerwoodWall>())
        //            Main.tile[x, y].WallType = WallID.LivingWood;

        //        if (type == ModContent.TileType<CrystalDirtTile>())
        //            tile.TileType = TileID.Dirt;
        //        else if (type == ModContent.TileType<CrystalGrassTile>())
        //            SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass, TileID.Grass);
        //        else if (type == ModContent.TileType<CavernStoneTile>())
        //            SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
        //        else if (type == ModContent.TileType<CavernSandTile>())
        //            SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
        //        else if (type == ModContent.TileType<FreshGlimmerwoodTile>())
        //            tile.TileType = TileID.LivingWood;
        //        else if (type == ModContent.TileType<CrystalVines>())
        //            SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
        //        if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
        //            WorldGen.SquareTileFrame(x, y);

        //        if (tileframe)
        //        {
        //            if (Main.netMode == NetmodeID.SinglePlayer)
        //                WorldGen.SquareTileFrame(x, y, true);
        //            else if (Main.netMode == NetmodeID.Server)
        //                NetMessage.SendTileSquare(-1, x, y, 1);
        //        }
        //    }
        //}

        //private static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corruptType, ushort crimsonType, ushort hallowType, ushort pureType)
        //{
        //    switch (convert)
        //    {
        //        case ConvertType.Corrupt:
        //            if (corruptType != ushort.MaxValue)
        //                Main.tile[x, y].TileType = corruptType;
        //            else
        //                Main.tile[x, y].TileType = pureType;
        //            break;
        //        case ConvertType.Crimson:
        //            if (crimsonType != ushort.MaxValue)
        //                Main.tile[x, y].TileType = crimsonType;
        //            else
        //                Main.tile[x, y].TileType = pureType;
        //            break;
        //        case ConvertType.Hallow:
        //            if (hallowType != ushort.MaxValue)
        //                Main.tile[x, y].TileType = hallowType;
        //            else
        //                Main.tile[x, y].TileType = pureType;
        //            break;
        //        case ConvertType.Pure:
        //            Main.tile[x, y].TileType = pureType;
        //            break;
        //    }
        //}

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
        public static int SandType;
        public static int ClayType;
        public static int VinesType;

        //public static int ChairType;
        //public static int WorkbenchType;
        public static int Rubble1x1CeilingType;
        public static int Rubble1x1FloorType;
        public static int Rubble1x2CeilingType;
        public static int Rubble1x2FloorType;
        public static int Rubble3x2FloorType;

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
            SandType = ModContent.TileType<CavernSandTile>();
            ClayType = ModContent.TileType<ChargedStoneTile>();
            VinesType = ModContent.TileType<CrystalVines>();

            //ChairType = ModContent.TileType<ExampleChair>();
            //WorkbenchType = ModContent.TileType<ExampleWorkbench>();
            Rubble1x1CeilingType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();
            Rubble1x1FloorType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();
            Rubble1x2CeilingType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();
            Rubble1x2FloorType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();
            Rubble3x2FloorType = ModContent.TileType<CavernStone1x1CeilingRubbleNatural>();

            // Normally we'd just use WallLoader.RegisterSimpleConversion on the basic wall types and rely on the fallback system
            // but we want to convert safe walls to safe example walls and unsafe to unsafe, where vanilla convers safe walls to unsafe walls on all conversions
            for (int i = 0; i < WallLoader.WallCount; i++)
            {
                if (WallID.Sets.Conversion.Dirt[i] ||
                    WallID.Sets.Conversion.Grass[i] ||
                    WallID.Sets.Conversion.Stone[i] ||
                    WallID.Sets.Conversion.Sandstone[i] ||
                    WallID.Sets.Conversion.HardenedSand[i] ||
                    WallID.Sets.Conversion.PureSand[i] ||
                    //WallID.Sets.Conversion.Ice[i] ||
                    WallID.Sets.Conversion.NewWall1[i] || // NewWalls are the underground wall variants
                    WallID.Sets.Conversion.NewWall2[i] ||
                    WallID.Sets.Conversion.NewWall3[i] ||
                    WallID.Sets.Conversion.NewWall4[i])
                    WallLoader.RegisterConversion(i, Type, ConvertWalls);
            }
            WallLoader.RegisterConversionFallback(DirtWallType, WallID.Dirt, Type);
            WallLoader.RegisterConversionFallback(UnsafeDirtWallType, WallID.DirtUnsafe, Type);
            WallLoader.RegisterConversionFallback(GrassWallType, WallID.Grass, Type);
            WallLoader.RegisterConversionFallback(UnsafeGrassWallType, WallID.GrassUnsafe, Type);
            WallLoader.RegisterConversionFallback(StoneWallType, WallID.Cave8Echo, Type);
            WallLoader.RegisterConversionFallback(UnsafeStoneWallType, WallID.Cave8Unsafe, Type);
            WallLoader.RegisterConversionFallback(SandWallType, WallID.SandstoneEcho, Type);
            WallLoader.RegisterConversionFallback(UnsafeSandWallType, WallID.Sandstone, Type);



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
            TileLoader.RegisterSimpleConversion(TileID.Stone, Type, StoneType);
            TileLoader.RegisterSimpleConversion(TileID.Sand, Type, SandType);
            TileLoader.RegisterSimpleConversion(TileID.ClayBlock, Type, ClayType);

            // Chairs and Workbenches aren't normally converted by solutions, so there's no sensible fallback to register.
            // We could register a purifying conversion for these too if we wanted
            //TileLoader.RegisterConversion(TileID.Chairs, Type, ConvertChairs);
            //TileLoader.RegisterConversion(TileID.WorkBenches, Type, ConvertWorkbenches);
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

        public void FindAndConvertTree(int i, int j, int tileTypeAbove)
        {

            if (tileTypeAbove == -1)
                return;

            if (!(tileTypeAbove == TileID.VanityTreeSakura) &&
                !(tileTypeAbove == TileID.VanityTreeYellowWillow) &&
                !(tileTypeAbove == TileID.Trees))
                return;

            int treeBottom = j;
            int treeTop = treeBottom - 1;
            int treeCenterX = i;

            // Check for if the tile is the tree's "trunk" or just the root tiles on the side
            // We do this by checking for the specific tile frame of the tree tile.
            // Necessary because the Trees ID doesn't care about the tile's frame and returns true even if the tile isnt the tree's "trunk"
            int treeFrameX = Main.tile[treeCenterX, treeTop].TileFrameX / 22;
            int treeFrameY = Main.tile[treeCenterX, treeTop].TileFrameY / 22;
            bool isTreeTrunk = (treeFrameX != 1 && treeFrameX != 2) || treeFrameY < 6;

            // Niche edgecase check: If a block was placed under a tree's branch, it shouldn't be converted at all, as it is not actually attached to the tile below
            bool isTreeBranch = (treeFrameX == 3 && treeFrameY < 3) || (treeFrameX == 4 && treeFrameY >= 3 && treeFrameY < 6);
            if (isTreeBranch)
                return;

            // If the tile above wasn't a tree trunk but instead a root tile on the side, check the adjacent two tiles to find it
            if (!isTreeTrunk)
            {
                for (int x = treeCenterX - 1; x < treeCenterX + 2; x += 2)
                {

                    Tile topTile = Main.tile[x, treeTop];
                    if (!topTile.HasTile || (!(topTile.TileType == TileID.VanityTreeSakura) &&
                            !(topTile.TileType == TileID.VanityTreeYellowWillow) &&
                            !(topTile.TileType == TileID.Trees)))
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
            while (treeTop >= 0 && Main.tile[treeCenterX, treeTop].HasTile && (Main.tile[treeCenterX, treeTop].TileType == TileID.VanityTreeSakura ||
                    Main.tile[treeCenterX, treeTop].TileType == TileID.VanityTreeYellowWillow) ||
                    Main.tile[treeCenterX, treeTop].TileType == TileID.Trees)
                treeTop--;

            // Turn all the tiles around it into trees
            for (int x = treeCenterX - 1; x < treeCenterX + 2; x++)
            {
                for (int y = treeTop; y < treeBottom; y++)
                {
                    Tile t = Main.tile[x, y];
                    if (t.HasTile && (t.TileType == TileID.VanityTreeSakura ||
                            t.TileType == TileID.VanityTreeYellowWillow ||
                            t.TileType == TileID.Trees))
                        t.TileType = TileID.Trees;
                }
            }
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

        public bool ConvertWalls(int i, int j, int type, int conversionType)
        {
            int wallType;
            if (WallID.Sets.Conversion.Dirt[type])
            {
                wallType = Main.wallHouse[type] ? DirtWallType : UnsafeDirtWallType;
            }
            else if (WallID.Sets.Conversion.Grass[type])
            {
                wallType = Main.wallHouse[type] ? GrassWallType : UnsafeGrassWallType;
            }
            else if (WallID.Sets.Conversion.Stone[type] ||
                WallID.Sets.Conversion.NewWall1[type] ||
                WallID.Sets.Conversion.NewWall2[type] ||
                WallID.Sets.Conversion.NewWall3[type] ||
                WallID.Sets.Conversion.NewWall4[type])
            {
                wallType = Main.wallHouse[type] ? StoneWallType : UnsafeStoneWallType;
            }
            else if (WallID.Sets.Conversion.Sandstone[type] || WallID.Sets.Conversion.HardenedSand[type] || WallID.Sets.Conversion.PureSand[type])
            {
                wallType = Main.wallHouse[type] ? SandWallType : UnsafeSandWallType;
            } else wallType = Main.wallHouse[type] ? DirtWallType : UnsafeDirtWallType;

            WorldGen.ConvertWall(i, j, wallType);
            return false;
        }
    }

    //public enum ConvertType
    //{
    //    Pure,
    //    Corrupt,
    //    Crimson,
    //    Hallow
    //}
}