using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Common.Globals.Worlds
{
    public class DownedWorld : ModSystem
    {
        public static bool DownedCrystalTumbler;
        public static bool DownedRimegeist;
        public static bool DownedLightningMoth;
        public static bool DownedCyvercry;
        public static bool DownedTheFallen;

        public override void OnWorldLoad()
        {
            DownedCrystalTumbler = false;
            DownedRimegeist = false;
            DownedLightningMoth = false;
            DownedCyvercry = false;
            DownedTheFallen = false;
        }

        public override void OnWorldUnload()
        {
            OnWorldLoad();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = [];
            if (DownedCrystalTumbler)
                downed.Add("CrystalTumbler");
            if (DownedRimegeist)
                downed.Add("Rimegeist");
            if (DownedLightningMoth)
                downed.Add("LightningMoth");
            if (DownedCyvercry)
                downed.Add("Cyvercry");
            if (DownedTheFallen)
                downed.Add("TheFallen");
            tag["downed"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downed");
            DownedCrystalTumbler = downed.Contains("CrystalTumbler");
            DownedRimegeist = downed.Contains("Rimegeist");
            DownedLightningMoth = downed.Contains("LightningMoth");
            DownedCyvercry = downed.Contains("Cyvercry");
            DownedTheFallen = downed.Contains("TheFallen");
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new()
            {
                [0] = DownedCrystalTumbler,
                [1] = DownedRimegeist,
                [2] = DownedLightningMoth,
                [3] = DownedCyvercry,
                [4] = DownedTheFallen
            };
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            DownedCrystalTumbler = flags[0];
            DownedRimegeist = flags[1];
            DownedLightningMoth = flags[2];
            DownedCyvercry = flags[3];
            DownedTheFallen = flags[4];
        }
    }
}
