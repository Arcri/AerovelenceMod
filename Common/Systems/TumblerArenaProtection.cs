using System;
using System.IO;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Common.Systems
{
    public class TumblerArenaSystem : ModSystem
    {
        public override void Load()
        {
            On_WorldGen.PlaceTile += PlaceArenaTile;
            On_WorldGen.PlaceWall += PlaceArenaWall;
            On_Liquid.AddWater += KeepArenaDry;
            On_Liquid.Update += UpdateArenaLiquid;
        }

        public override void Unload()
        {
            On_WorldGen.PlaceTile -= PlaceArenaTile;
            On_WorldGen.PlaceWall -= PlaceArenaWall;
            On_Liquid.AddWater -= KeepArenaDry;
            On_Liquid.Update -= UpdateArenaLiquid;
        }

        private static bool PlaceArenaTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int type, bool mute, bool forced, int player, int style)
        {
            return (WorldGen.gen || !ArenaData.Valid || !ArenaData.TileBounds.Contains(i, j) || AllowsDecoration(type) && !Framing.GetTileSafely(i, j).HasTile) && orig(i, j, type, mute, forced, player, style);
        }

        public static bool AllowsDecoration(int type) => type >= 0 && type < Main.tileSolid.Length
            && type != TileID.Tombstones && type != ModContent.TileType<CavernGatewayTile>() && type != ModContent.TileType<ArenaCavernCrystalTile>()
            && !Main.tileSolid[type] && !Main.tileSolidTop[type] && !TileID.Sets.Platforms[type]
            && !TileID.Sets.BasicChest[type] && !TileID.Sets.BasicDresser[type];

        public static bool RemovableDecoration(int type) => type == TileID.Tombstones || AllowsDecoration(type);

        private static bool DryArea(int x, int y)
        {
            if (!ArenaData.Valid || WorldGen.gen)
                return false;
            Rectangle bounds = ArenaData.TileBounds;
            bounds.Inflate(8, 8);
            return bounds.Contains(x, y);
        }

        private static void KeepArenaDry(On_Liquid.orig_AddWater orig, int x, int y)
        {
            if (DryArea(x, y))
            {
                if (ArenaData.TileBounds.Contains(x, y))
                    Framing.GetTileSafely(x, y).LiquidAmount = 0;
            }
            else
                orig(x, y);
        }

        private static void UpdateArenaLiquid(On_Liquid.orig_Update orig, Liquid liquid)
        {
            if (DryArea(liquid.x, liquid.y))
            {
                if (!ArenaData.TileBounds.Contains(liquid.x, liquid.y))
                {
                    liquid.kill = 999;
                    return;
                }
                Framing.GetTileSafely(liquid.x, liquid.y).LiquidAmount = 0;
            }
            orig(liquid);
        }

        public override void PostUpdateWorld()
        {
            if (!ArenaData.Valid || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            Rectangle bounds = ArenaData.TileBounds;
            for (int x = bounds.Left; x < bounds.Right; x++)
            {
                for (int y = bounds.Top; y < bounds.Bottom; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.LiquidAmount == 0 && !tile.RedWire && !tile.BlueWire && !tile.GreenWire && !tile.YellowWire && !tile.HasActuator)
                        continue;
                    tile.LiquidAmount = 0;
                    tile.RedWire = tile.BlueWire = tile.GreenWire = tile.YellowWire = false;
                    tile.HasActuator = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y);
                }
            }
        }

        private static void PlaceArenaWall(On_WorldGen.orig_PlaceWall orig, int i, int j, int type, bool mute)
        {
            if (WorldGen.gen || !ArenaData.Valid || !ArenaData.TileBounds.Contains(i, j))
                orig(i, j, type, mute);
        }

        public override void OnWorldLoad()
        {
            ArenaData.Reset();
        }

        public override void OnWorldUnload()
        {
            ArenaData.Reset();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (!ArenaData.Valid)
                return;
            tag["TumblerArenaX"] = ArenaData.TileBounds.X;
            tag["TumblerArenaY"] = ArenaData.TileBounds.Y;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("TumblerArenaX") && tag.ContainsKey("TumblerArenaY"))
            {
                ArenaData.Initialize(new Point(tag.GetInt("TumblerArenaX"), tag.GetInt("TumblerArenaY")));
                ArenaData.ClearTemporaryBarriers();
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ArenaData.Valid);
            if (ArenaData.Valid)
            {
                writer.Write(ArenaData.TileBounds.X);
                writer.Write(ArenaData.TileBounds.Y);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
                ArenaData.Initialize(new Point(reader.ReadInt32(), reader.ReadInt32()));
            else
                ArenaData.Reset();
        }
    }

    public class TumblerArenaTileProtection : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            return !InsideArena(i, j) || TumblerArenaSystem.RemovableDecoration(type);
        }

        public override bool CanPlace(int i, int j, int type)
        {
            return !InsideArena(i, j) || TumblerArenaSystem.AllowsDecoration(type) && !Framing.GetTileSafely(i, j).HasTile;
        }

        public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
        {
            return !InsideArena(i, j);
        }

        public override bool Slope(int i, int j, int type)
        {
            return !InsideArena(i, j);
        }

        public override bool PreHitWire(int i, int j, int type)
        {
            return !InsideArena(i, j);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (InsideArena(i, j) && !TumblerArenaSystem.RemovableDecoration(type))
            {
                fail = true;
                effectOnly = false;
                noItem = true;
            }
        }

        public override bool CanExplode(int i, int j, int type)
        {
            return !InsideArena(i, j);
        }

        private static bool InsideArena(int i, int j)
        {
            return ArenaData.Valid && ArenaData.TileBounds.Contains(i, j);
        }
    }

    public class TumblerArenaWallProtection : GlobalWall
    {
        public override bool CanPlace(int i, int j, int type)
        {
            return !InsideArena(i, j);
        }

        public override void KillWall(int i, int j, int type, ref bool fail)
        {
            if (InsideArena(i, j))
                fail = true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            return !InsideArena(i, j);
        }

        private static bool InsideArena(int i, int j)
        {
            return ArenaData.Valid && ArenaData.TileBounds.Contains(i, j);
        }
    }

    public class TumblerArenaItemProtection : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (!ArenaData.Valid)
                return true;
            bool playerInside = ArenaData.WorldBounds.Contains(player.Center.ToPoint());
            bool targetingArena = player.whoAmI == Main.myPlayer && ArenaData.TileBounds.Contains(Player.tileTargetX, Player.tileTargetY);
            if (!playerInside && !targetingArena)
                return true;
            bool placing = item.createTile >= 0 && !TumblerArenaSystem.AllowsDecoration(item.createTile) || item.createWall >= 0;
            bool explosive = item.shoot > ProjectileID.None && item.shoot < ProjectileID.Sets.Explosive.Length && ProjectileID.Sets.Explosive[item.shoot];
            return !placing && !explosive;
        }
    }

    public class TumblerArenaProjectileProtection : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (ArenaData.Valid && ProjectileID.Sets.IsAGravestone[projectile.type] && ArenaData.WorldBounds.Intersects(projectile.Hitbox))
            {
                projectile.Kill();
                return false;
            }
            if (IsExplosiveNearArena(projectile))
            {
                projectile.Kill();
                return false;
            }
            return true;
        }

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (ArenaData.Valid && ProjectileID.Sets.IsAGravestone[projectile.type] && ArenaData.WorldBounds.Intersects(projectile.Hitbox))
                return false;
            if (!IsExplosiveNearArena(projectile))
                return true;
            if (Main.netMode != NetmodeID.SinglePlayer && projectile.owner == Main.myPlayer)
                NetMessage.SendData(MessageID.KillProjectile, number: projectile.identity, number2: projectile.owner);
            return false;
        }

        public override bool? GrappleCanLatchOnTo(Projectile projectile, Player player, int x, int y)
        {
            if (ArenaData.Valid && ArenaData.TileBounds.Contains(x, y))
            {
                Tile tile = Framing.GetTileSafely(x, y);
                bool baseFloor = y >= (int)(ArenaData.FloorY / 16f) && Main.tileSolid[tile.TileType];
                if (!tile.HasTile || tile.IsActuated || !TileID.Sets.Platforms[tile.TileType] && !baseFloor)
                    return false;
            }
            return null;
        }

        private static bool IsExplosiveNearArena(Projectile projectile)
        {
            if (!ArenaData.Valid)
                return false;
            bool explosive = projectile.aiStyle == ProjAIStyleID.Explosive || projectile.type > ProjectileID.None && projectile.type < ProjectileID.Sets.Explosive.Length && ProjectileID.Sets.Explosive[projectile.type];
            if (!explosive)
                return false;
            Rectangle blastBounds = projectile.Hitbox;
            blastBounds.Inflate(160, 160);
            return ArenaData.WorldBounds.Intersects(blastBounds);
        }
    }

    public class TumblerSharedProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool FromEncounter { get; private set; }

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ProjectileType<ElectricBolt>() || entity.type == ModContent.ProjectileType<CrystalShard>();
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
                FromEncounter = parent.Entity is NPC { ModNPC: CrystalTumbler } || parent.Entity is Projectile { ModProjectile: TumblerStar or TumblerAimLine or TumblerKnifeBall or TumblerGuidedShard };
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(FromEncounter);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            FromEncounter = bitReader.ReadBit();
        }
    }

    public class TumblerArenaPlayer : ModPlayer
    {
        private int electricCooldown;
        private int arenaSearchTimer;

        public override void PreUpdate()
        {
            if (!Player.active || Player.dead)
                return;
            if (electricCooldown > 0)
                electricCooldown--;
            if (!ArenaData.Valid && arenaSearchTimer <= 0)
            {
                arenaSearchTimer = 90;
                if (ArenaData.TryInitializeFromWorld(Player.Center, 80) && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            else if (!ArenaData.Valid)
                arenaSearchTimer--;
            else if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer && ArenaData.WorldBounds.Intersects(Player.Hitbox))
            {
                if (--arenaSearchTimer <= 0)
                {
                    arenaSearchTimer = 90;
                    ArenaData.RefreshGeometry();
                }
            }
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || Player.dead || Player.ghost || !ArenaData.Valid || electricCooldown > 0 || Player.velocity.Y < 0f)
                return;
            int tileType = ModContent.TileType<ArenaCavernCrystalTile>();
            Point left = new Vector2(Player.Left.X + 5f, Player.Bottom.Y + 2f).ToTileCoordinates();
            Point right = new Vector2(Player.Right.X - 5f, Player.Bottom.Y + 2f).ToTileCoordinates();
            bool touching = IsElectricCrystal(left, tileType) || IsElectricCrystal(right, tileType);
            if (!touching)
                return;
            electricCooldown = 45;
            Player.AddBuff(BuffID.Electrified, 90);
            Player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(Player.name + " was grounded by the crystal current.")), 65, Player.direction == 0 ? 1 : -Player.direction);
        }

        private static bool IsElectricCrystal(Point point, int tileType)
        {
            if (!ArenaData.TileBounds.Contains(point))
                return false;
            Tile tile = Framing.GetTileSafely(point.X, point.Y);
            return tile.HasTile && !tile.IsActuated && tile.TileType == tileType;
        }
    }
}
