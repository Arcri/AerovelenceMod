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

namespace AerovelenceMod.Content.Items.Tools
{
    public class ElectricBlueSolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
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
        public ref float Progress => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.DefaultToSpray();
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            int dustType = DustID.BlueCrystalShard;

            if (Projectile.owner == Main.myPlayer)
            {
                Convert((int)(Projectile.position.X + (Projectile.width * 0.5f)) / 16, (int)(Projectile.position.Y + (Projectile.height * 0.5f)) / 16, 2);
            }

            if (Projectile.timeLeft > 133)
            {
                Projectile.timeLeft = 133;
            }

            if (Progress > 7f)
            {
                float dustScale = 1f;

                if (Progress == 8f)
                {
                    dustScale = 0.2f;
                }
                else if (Progress == 9f)
                {
                    dustScale = 0.4f;
                }
                else if (Progress == 10f)
                {
                    dustScale = 0.6f;
                }
                else if (Progress == 11f)
                {
                    dustScale = 0.8f;
                }

                Progress += 1f;

                var dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

                dust.noGravity = true;
                dust.scale *= 1.75f;
                dust.velocity.X *= 2f;
                dust.velocity.Y *= 2f;
                dust.scale *= dustScale;
            }
            else
            {
                Progress += 1f;
            }

            Projectile.rotation += 0.3f * Projectile.direction;
        }

        private static void Convert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        ConvertTile(k, l);
                    }
                }
            }
        }

        private static void ConvertTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            if (tile == null)
                return;
            int type = tile.TileType;
            int wall = tile.WallType;
            if (TileID.Sets.Conversion.JungleGrass[type] || type == TileID.JungleGrass || type == TileID.Mud)
                return;
            if (wall != 0 && !IsProtectedWall(wall))
            {
                ConvertWall(x, y, wall);
            }
            if (!IsProtectedTile(type))
            {
                ConvertTileType(x, y, type);
                if (type == TileID.Dirt || type == TileID.SnowBlock ||
                    type == ModContent.TileType<CrystalDirtTile>())
                {
                    CheckAndConvertToCrystalGrass(x, y);
                }
            }
        }

        private static bool IsProtectedWall(int wallType)
        {
            return wallType == ModContent.WallType<CavernDirtWall>() ||
                   wallType == ModContent.WallType<CavernDirtWallUnsafe>() ||
                   wallType == ModContent.WallType<CavernStoneWall>() ||
                   wallType == ModContent.WallType<CavernStoneWallUnsafe>() ||
                   wallType == ModContent.WallType<CavernSandWallUnsafe>();
        }

        private static bool IsProtectedTile(int tileType)
        {
            return tileType == ModContent.TileType<CrystalGrassTile>() ||
                   tileType == ModContent.TileType<CavernStoneTile>() ||
                   tileType == ModContent.TileType<CavernSandTile>() ||
                   tileType == ModContent.TileType<CrystalVines>();
        }

        private static void ConvertWall(int x, int y, int wallType)
        {
            if (WallID.Sets.Conversion.Grass[wallType])
            {
                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWallUnsafe>();
                WorldGen.SquareWallFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (WallID.Sets.Conversion.HardenedSand[wallType])
            {
                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernSandWallUnsafe>();
                WorldGen.SquareWallFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (WallID.Sets.Conversion.Sandstone[wallType])
            {
                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWallUnsafe>();
                WorldGen.SquareWallFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (WallID.Sets.Conversion.Stone[wallType])
            {
                Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWall>();
                WorldGen.SquareWallFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else
            {
                switch (wallType)
                {
                    case WallID.DirtUnsafe:
                    case WallID.DirtUnsafe1:
                    case WallID.DirtUnsafe2:
                    case WallID.DirtUnsafe3:
                    case WallID.DirtUnsafe4:
                    case WallID.CaveUnsafe:
                    case WallID.Cave2Unsafe:
                    case WallID.Cave3Unsafe:
                    case WallID.Cave4Unsafe:
                    case WallID.Cave5Unsafe:
                    case WallID.Cave6Unsafe:
                    case WallID.Cave7Unsafe:
                    case WallID.Cave8Unsafe:
                    case WallID.Dirt:
                        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWall>();
                        WorldGen.SquareWallFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case WallID.SnowWallUnsafe:
                        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernDirtWall>();
                        WorldGen.SquareWallFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case WallID.DesertFossil:
                        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWall>();
                        WorldGen.SquareWallFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case WallID.IceUnsafe:
                        Main.tile[x, y].WallType = (ushort)ModContent.WallType<CavernStoneWall>();
                        WorldGen.SquareWallFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case WallID.LivingWoodUnsafe:
                        Main.tile[x, y].WallType = (ushort)ModContent.WallType<GlimmerwoodWall>();
                        WorldGen.SquareWallFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                }
            }
        }

        private static void ConvertTileType(int x, int y, int tileType)
        {
            if (IsProtectedTile(tileType))
                return;

            if (TileID.Sets.Conversion.Grass[tileType] && !TileID.Sets.GrassSpecial[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalGrassTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (TileID.Sets.Conversion.Stone[tileType] || Main.tileMoss[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (TileID.Sets.Conversion.Sand[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernSandTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (TileID.Sets.Conversion.HardenedSand[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (TileID.Sets.Conversion.Sandstone[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else if (TileID.Sets.Conversion.Ice[tileType])
            {
                Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
                WorldGen.SquareTileFrame(x, y, true);
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
            else
            {
                switch (tileType)
                {
                    case TileID.Dirt:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.SnowBlock:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.Silt:
                    case TileID.Slush:
                    case TileID.DesertFossil:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CavernStoneTile>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.ClayBlock:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalDirtTile>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.Vines:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<CrystalVines>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.LivingWood:
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<FreshGlimmerwoodTile>();
                        WorldGen.SquareTileFrame(x, y, true);
                        NetMessage.SendTileSquare(-1, x, y, 1);
                        break;
                    case TileID.LeafBlock:
                    case TileID.Sunflower:
                    case TileID.Plants:
                    case TileID.Plants2:
                    case TileID.JunglePlants:
                    case TileID.JunglePlants2:
                    case TileID.CorruptPlants:
                    case TileID.CrimsonPlants:
                    case TileID.HallowedPlants:
                    case TileID.HallowedPlants2:
                        WorldGen.KillTile(x, y);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                        }
                        break;
                }
            }
        }

        private static void CheckAndConvertToCrystalGrass(int i, int j)
        {
            if (Main.tile[i, j].TileType != ModContent.TileType<CrystalDirtTile>())
                return;
            bool exposedToAir = false;
            if (IsExposedToAir(i, j - 1))
                exposedToAir = true;
            else if (IsExposedToAir(i, j + 1))
                exposedToAir = true;
            else if (IsExposedToAir(i - 1, j))
                exposedToAir = true;
            else if (IsExposedToAir(i + 1, j))
                exposedToAir = true;
            if (exposedToAir)
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<CrystalGrassTile>();
                WorldGen.SquareTileFrame(i, j, true);
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }

        private static bool IsExposedToAir(int i, int j)
        {
            if (!WorldGen.InWorld(i, j))
                return false;
            Tile tile = Main.tile[i, j];
            if (tile == null)
                return true;
            if (!tile.HasTile || !Main.tileSolid[tile.TileType])
                return true;
            return false;
        }

        public static void ConvertFromCrystalCavern(int startX, int endX, int startY, int endY, ConvertType convert)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertFromCrystalCavern(x, y, convert);
                }
            }
        }

        public static void ConvertFromCrystalCavern(int x, int y, ConvertType convert, bool tileframe = true)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
                return;

            int type = tile.TileType;
            int wallType = tile.WallType;

            if (WorldGen.InWorld(x, y, 1))
            {
                if (wallType == ModContent.WallType<CavernDirtWall>() || wallType == ModContent.WallType<CavernDirtWallUnsafe>())
                {
                    Main.tile[x, y].WallType = WallID.DirtUnsafe;
                }
                else if (wallType == ModContent.WallType<CavernStoneWall>() || wallType == ModContent.WallType<CavernStoneWallUnsafe>())
                {
                    switch (convert)
                    {
                        case ConvertType.Corrupt:
                            Main.tile[x, y].WallType = WallID.EbonstoneUnsafe;
                            break;
                        case ConvertType.Crimson:
                            Main.tile[x, y].WallType = WallID.CrimstoneUnsafe;
                            break;
                        case ConvertType.Hallow:
                            Main.tile[x, y].WallType = WallID.PearlstoneBrickUnsafe;
                            break;
                        case ConvertType.Pure:
                            Main.tile[x, y].WallType = WallID.Stone;
                            break;
                    }
                }
                else if (wallType == ModContent.WallType<CavernSandWallUnsafe>())
                    Main.tile[x, y].WallType = WallID.Sandstone;
                else if (wallType == ModContent.WallType<GlimmerwoodWall>())
                    Main.tile[x, y].WallType = WallID.LivingWood;

                if (type == ModContent.TileType<CrystalDirtTile>())
                    tile.TileType = TileID.Dirt;
                else if (type == ModContent.TileType<CrystalGrassTile>())
                    SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass, TileID.Grass);
                else if (type == ModContent.TileType<CavernStoneTile>())
                    SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
                else if (type == ModContent.TileType<CavernSandTile>())
                    SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
                else if (type == ModContent.TileType<FreshGlimmerwoodTile>())
                    tile.TileType = TileID.LivingWood;
                else if (type == ModContent.TileType<CrystalVines>())
                    SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
                if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
                    WorldGen.SquareTileFrame(x, y);

                if (tileframe)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        WorldGen.SquareTileFrame(x, y, true);
                    else if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }
        }

        private static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corruptType, ushort crimsonType, ushort hallowType, ushort pureType)
        {
            switch (convert)
            {
                case ConvertType.Corrupt:
                    if (corruptType != ushort.MaxValue)
                        Main.tile[x, y].TileType = corruptType;
                    else
                        Main.tile[x, y].TileType = pureType;
                    break;
                case ConvertType.Crimson:
                    if (crimsonType != ushort.MaxValue)
                        Main.tile[x, y].TileType = crimsonType;
                    else
                        Main.tile[x, y].TileType = pureType;
                    break;
                case ConvertType.Hallow:
                    if (hallowType != ushort.MaxValue)
                        Main.tile[x, y].TileType = hallowType;
                    else
                        Main.tile[x, y].TileType = pureType;
                    break;
                case ConvertType.Pure:
                    Main.tile[x, y].TileType = pureType;
                    break;
            }
        }

    }
    public enum ConvertType
    {
        Pure,
        Corrupt,
        Crimson,
        Hallow
    }
}