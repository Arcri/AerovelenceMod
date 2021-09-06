using System.IO;
using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Globals.Players
{
    /// <summary>
    /// ModPlayer responsible for handling biome logic.
    /// </summary>
    public class ZonePlayer : ModPlayer
    {
        public const int DEFAULT_TILE_REQUIREMENT = 100;

        public bool ZoneCrystalCaverns { get; private set; }
        public bool ZoneCrystalCitadel { get; private set; }

        public override void UpdateBiomes()
        {
            ZoneCrystalCaverns = ZoneWorld.CavernTiles > DEFAULT_TILE_REQUIREMENT;
            ZoneCrystalCitadel = ZoneWorld.CitadelTiles > DEFAULT_TILE_REQUIREMENT;
        }

        public override bool CustomBiomesMatch(Player other)
        {
            var modOther = other.GetModPlayer<ZonePlayer>();

            return ZoneCrystalCaverns == modOther.ZoneCrystalCaverns && ZoneCrystalCitadel == modOther.ZoneCrystalCitadel;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            var modOther = other.GetModPlayer<ZonePlayer>();

            modOther.ZoneCrystalCaverns = ZoneCrystalCaverns;
            modOther.ZoneCrystalCitadel = ZoneCrystalCitadel;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte
            {
                [0] = ZoneCrystalCaverns,
                [1] = ZoneCrystalCitadel
            };

            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneCrystalCaverns = flags[0];
            ZoneCrystalCitadel = flags[1];
        }

        /*public override void UpdateBiomeVisuals()
        {
            if (ZoneCrystalCaverns)
                player.ManageSpecialBiomeVisuals("AerovelenceMod:FoggyFields", FoggyFieldsWorld.FoggyFields, player.Center);

            player.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalTorrents", CrystalTorrentWorld.CrystalTorrents, player.Center);
            player.ManageSpecialBiomeVisuals("AerovelenceMod:DarkNights", DarkNightWorld.DarkNight, player.Center);
        }*/

        public override Texture2D GetMapBackgroundImage()
        {
            if (ZoneCrystalCaverns)
                return mod.GetTexture("Backgrounds/CrystalCaverns/CrystalCavernsMapBackground");
            
            return null;
        }
    }
}
