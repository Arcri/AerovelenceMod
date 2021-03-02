using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.CrystalTorrent
{
	public class CrystalTorrentWorld : ModWorld
	{
		public static bool CrystalTorrentUp = false;
		public static bool downedCrystalTorrent = false;

		public override void Initialize()
		{
			Main.invasionSize = 0;
			CrystalTorrentUp = false;
			downedCrystalTorrent = false;
		}

		public override TagCompound Save()
		{
			var downed = new List<string>();
			if (downedCrystalTorrent)
			{
				downed.Add("CrystalTorrent");
			}
			return new TagCompound
			{
				["downed"] = downed,
			};
		}
		public override void Load(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");
			downedCrystalTorrent = downed.Contains("CrystalTorrent");
		}
		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
			}
			else
			{
				mod.Logger.WarnFormat("AerovelenceMod: Unknown loadVersion: {0}", loadVersion);
			}
		}
		public override void NetSend(BinaryWriter writer)
		{
			var flags = new BitsByte();
			flags[3] = downedCrystalTorrent;
			writer.Write(flags);
		}
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedCrystalTorrent = flags[3];
		}


		public override void PostUpdate()
		{
			if (CrystalTorrentUp)
			{
				if (Main.invasionX == Main.spawnTileX)
				{
					CrystalTorrentInvasion.CheckCrystalTorrentProgress();
				}
				CrystalTorrentInvasion.UpdateCrystalTorrent();
			}
		}
	}
}