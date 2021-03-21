using System.IO;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Events.CrystalTorrent;
using AerovelenceMod.Content.Events.DarkNight;
using AerovelenceMod.Content.Events.FoggyFields;
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

        public bool zoneCrystalCaverns;
        public bool zoneCrystalCitadel;

        public override void UpdateBiomes()
        {
            zoneCrystalCaverns = ZoneWorld.CavernTiles > DEFAULT_TILE_REQUIREMENT;
            zoneCrystalCitadel = ZoneWorld.CitadelTiles > DEFAULT_TILE_REQUIREMENT;
        }

        public override bool CustomBiomesMatch(Player other)
        {
            var modOther = other.GetModPlayer<ZonePlayer>();

            return zoneCrystalCaverns == modOther.zoneCrystalCaverns && zoneCrystalCitadel == modOther.zoneCrystalCitadel;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            var modOther = other.GetModPlayer<ZonePlayer>();

            modOther.zoneCrystalCaverns = zoneCrystalCaverns;
            modOther.zoneCrystalCitadel = zoneCrystalCitadel;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte
            {
                [0] = zoneCrystalCaverns,
                [1] = zoneCrystalCitadel
            };

            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            zoneCrystalCaverns = flags[0];
            zoneCrystalCitadel = flags[1];
        }

        public override void UpdateBiomeVisuals()
        {
            if (zoneCrystalCaverns)
                player.ManageSpecialBiomeVisuals("AerovelenceMod:FoggyFields", FoggyFieldsWorld.FoggyFields, player.Center);

            player.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalTorrents", CrystalTorrentWorld.CrystalTorrents, player.Center);
            player.ManageSpecialBiomeVisuals("AerovelenceMod:DarkNights", DarkNightWorld.DarkNight, player.Center);
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (zoneCrystalCaverns)
                return mod.GetTexture("Backgrounds/CrystalCaverns/CrystalCavernsMapBackground");
            
            return null;
        }
    }
}
