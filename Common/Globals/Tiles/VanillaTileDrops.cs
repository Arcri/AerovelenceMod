using AerovelenceMod.Content.Items.Others.Quest;
using AerovelenceMod.Content.Items.Sets.Phantic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Tiles
{
    public class VanillaTileDrops : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly || noItem || Main.netMode == NetmodeID.MultiplayerClient || WorldGen.gen)
                return;
            int item = type switch
            {
                TileID.Copper => ModContent.ItemType<CopperCluster>(),
                TileID.Tin => ModContent.ItemType<TinCluster>(),
                TileID.Iron => ModContent.ItemType<IronCluster>(),
                TileID.Lead => ModContent.ItemType<LeadCluster>(),
                TileID.Silver => ModContent.ItemType<SilverCluster>(),
                TileID.Tungsten => ModContent.ItemType<TungstenCluster>(),
                TileID.Gold => ModContent.ItemType<GoldCluster>(),
                TileID.Platinum => ModContent.ItemType<PlatinumCluster>(),
                TileID.Cobalt => ModContent.ItemType<CobaltCluster>(),
                TileID.Palladium => ModContent.ItemType<PalladiumCluster>(),
                TileID.Mythril => ModContent.ItemType<MythrilCluster>(),
                TileID.Orichalcum => ModContent.ItemType<OrichalcumCluster>(),
                TileID.Adamantite => ModContent.ItemType<AdamantiteCluster>(),
                TileID.Titanium => ModContent.ItemType<TitaniumCluster>(),
                _ => type == ModContent.TileType<PhanticOreTile>() ? ModContent.ItemType<PhanticCluster>() : 0
            };
            if (item == 0) return;
            if ((type == TileID.Adamantite || type == TileID.Titanium) && Main.rand.NextBool(351))
                item = type == TileID.Adamantite ? ModContent.ItemType<AdamantiteSuperCluster>() : ModContent.ItemType<TitaniumSuperCluster>();
            else if (!Main.rand.NextBool(151))
                return;
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, item);
        }
    }
}
