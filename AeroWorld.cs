using AerovelenceMod;
using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using AerovelenceMod.World;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod
{
	public class AeroWorld : ModWorld
	{
		public static bool downedCrystalTumbler;
		public static bool downedRimegeist;
		public static bool downedLightningMoth;
		public static bool downedCyvercry;
		public static bool downedTheFallen;
		public static int cavernTiles;
		public static int citadelTiles;

		public override void Initialize()
		{
			downedCrystalTumbler = false;
			downedRimegeist = false;
			downedLightningMoth = false;
			downedCyvercry = false;
			downedTheFallen = false;
		}

		public override TagCompound Save()
		{
			var downed = new List<string>();
			if (downedCrystalTumbler)
			{
				downed.Add("CrystalTumbler");
			}
			if (downedRimegeist)
			{
				downed.Add("Rimegeist");
			}
			if (downedLightningMoth)
			{
				downed.Add("LightningMoth");
			}
			if (downedCyvercry)
			{
				downed.Add("Cyvercry");
			}
			if (downedTheFallen)
			{
				downed.Add("TheFallen");
			}
			return new TagCompound
			{
				["downed"] = downed,
			};
		}
		public override void Load(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");
			downedCrystalTumbler = downed.Contains("CrystalTumbler");
			downedRimegeist = downed.Contains("Rimegeist");
			downedLightningMoth = downed.Contains("LightningMoth");
			downedCyvercry = downed.Contains("Cyvercry");
			downedTheFallen = downed.Contains("");
		}
		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
				downedCrystalTumbler = flags[0];
				downedRimegeist = flags[1];
				downedLightningMoth = flags[2];
				downedCyvercry = flags[3];
				downedTheFallen = flags[4];
			}
			else
			{
				mod.Logger.WarnFormat("AerovelenceMod: Unknown loadVersion: {0}", loadVersion);
			}
		}
		public override void NetSend(BinaryWriter writer)
		{
			var flags = new BitsByte();
			flags[0] = downedCrystalTumbler;
			flags[1] = downedRimegeist;
			flags[2] = downedLightningMoth;
			flags[3] = downedCyvercry;
			flags[4] = downedTheFallen;
			writer.Write(flags);
		}
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedCrystalTumbler = flags[0];
			downedRimegeist = flags[1];
			downedLightningMoth = flags[2];
			downedCyvercry = flags[3];
			downedTheFallen = flags[4];
		}
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int idx = tasks.FindIndex(t => t.Name == "Underworld"); //Terrain
			if (idx == -1)
			{
				idx = 1;
			}

			var pass = new WormGenPass();

			tasks.Insert(idx, pass);    //+1

			totalWeight += pass.Weight;
		}
		public override void TileCountsAvailable(int[] tileCounts)
		{
			cavernTiles = tileCounts[TileType<CavernStone>()] + tileCounts[TileType<CrystalGrass>()] + tileCounts[TileType<CrystalDirt>()] + tileCounts[TileType<CavernCrystal>()];
			citadelTiles = tileCounts[TileType<CitadelStone>()];
		}
		public override void ResetNearbyTileEffects()
		{
			AeroPlayer modPlayer = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
			cavernTiles = 0;
			citadelTiles = 0;
		}
	}
}