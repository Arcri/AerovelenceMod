using AerovelenceMod.Content.Items.Others.Quest;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Tiles
{
    /// <summary>
    /// GlobalTile responsible for giving modded drops to vanilla tiles
    /// </summary>
    public class VanillaTileDrops : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                int drop = 0;

                switch (type)
                {
                    case TileID.Copper:
                        drop = ModContent.ItemType<CopperCluster>();
                        break;

                    case TileID.Tin:
                        drop = ModContent.ItemType<TinCluster>();
                        break;

                    case TileID.Iron:
                        drop = ModContent.ItemType<IronCluster>();
                        break;

                    case TileID.Lead:
                        drop = ModContent.ItemType<LeadCluster>();
                        break;

                    case TileID.Silver:
                        drop = ModContent.ItemType<SilverCluster>();
                        break;

                    case TileID.Tungsten:
                        drop = ModContent.ItemType<TungstenCluster>();
                        break;

                    case TileID.Gold:
                        drop = ModContent.ItemType<GoldCluster>();
                        break;

                    case TileID.Platinum:
                        drop = ModContent.ItemType<PlatinumCluster>();
                        break;

                    case TileID.Cobalt:
                        break;

                    case TileID.Palladium:
                        break;

                    case TileID.Mythril:
                        drop = ModContent.ItemType<MythrilCluster>();
                        break;

                    case TileID.Orichalcum:
                        drop = ModContent.ItemType<OrichalcumCluster>();
                        break;

                    case TileID.Adamantite:
                        drop = ModContent.ItemType<AdamantiteCluster>();
                        break;

                    case TileID.Titanium:
                        drop = ModContent.ItemType<TitaniumCluster>();
                        break;
                }

                Item.NewItem(i * 16, j * 16, 16, 16, drop);
            }
        }
    }
}

            // Maybe implement this into the ModTile?
            /*
            if (type == ModContent.TileType<SlateOreBlock>() && fail == false)
            {
                if (Main.rand.NextBool(151))
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<SlateCluster>());
                }
            }
            if (type == ModContent.TileType<PhanticOreBlock>() && fail == false)
            {
                if (Main.rand.NextBool(151))
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<PhanticCluster>());
                }
            }
            */