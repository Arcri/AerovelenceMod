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
		public static bool downedSnowrium;
		public static bool downedCyvercry;
		public static int cavernTiles;
		public static int citadelTiles;

		public override void Initialize()
		{
			downedCrystalTumbler = false;
			downedSnowrium = false;
			downedCyvercry = false;
		}
		public override TagCompound Save()
		{
			var downed = new List<string>();
			if (downedCrystalTumbler)
			{
				downed.Add("CrystalTumbler");
			}
			if (downedSnowrium)
			{
				downed.Add("Snowrium");
			}
			if (downedCyvercry)
			{
				downed.Add("Cyvercry");
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
			downedSnowrium = downed.Contains("Snowrium");
			downedCyvercry = downed.Contains("Cyvercry");
		}
		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
				downedCrystalTumbler = flags[0];
				downedSnowrium = flags[1];
				downedCyvercry = flags[2];
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
			flags[1] = downedSnowrium;
			flags[2] = downedCyvercry;
			writer.Write(flags);
		}
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedCrystalTumbler = flags[0];
			downedSnowrium = flags[1];
			downedCyvercry = flags[2];
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