//using AerovelenceMod.Systems;
using AerovelenceMod.Items.Accessories;
using AerovelenceMod.Items.BossSummons;
using AerovelenceMod.Items.Weapons.Magic;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Ranged;
using AerovelenceMod.Items.Weapons.Thrown;
using AerovelenceMod.World;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AerovelenceMod
{
	public class AeroMod : Mod
	{
		// Todo: Convert mod.XType to the new ModContent system


		public override void PostSetupContent()
		{
			Mod bossChecklist = ModLoader.GetMod("BossChecklist");
			if (bossChecklist != null)
			{

				bossChecklist.Call("AddBossWithInfo", "Crystal Tumbler", 0.5f, (Func<bool>)(() => AeroWorld.downedCrystalTumbler), "Use a [i:" + ItemType("LargeGeode") + "] in the Crystal Caverns");
				bossChecklist.Call("AddBossWithInfo", "Snowrium", 5.5f, (Func<bool>)(() => AeroWorld.downedSnowrium), "Use a [i:" + ItemType("GlowingSnow") + "] at night in the snow biome");
				bossChecklist.Call("AddBossWithInfo", "Cyvercry", 9.5f, (Func<bool>)(() => AeroWorld.downedCyvercry), "Use a [i:" + ItemType("ObsidianEye") + "] at night");



				bossChecklist.Call("AddToBossLoot", "AerovelenceMod", "Crystal Tumbler", new List<int> { ModContent.ItemType<DiamondDuster>(), ModContent.ItemType<DarkCrystalStaff>(), ModContent.ItemType<CavernMauler>(), ModContent.ItemType<CavernousImpaler>(), ModContent.ItemType<PrismThrasher>(), ModContent.ItemType<CrystallineQuadshot>(), ModContent.ItemType<PrismPiercer>(), ModContent.ItemType<PrismaticSoul>() });
				bossChecklist.Call("AddToBossSpawnItems", "AerovelenceMod", "Crystal Tumbler", new List<int> { ModContent.ItemType<LargeGeode>() });
			}
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bars", new int[]
			{
		ItemID.SilverBar,
		ItemID.TungstenBar
			});
			RecipeGroup.RegisterGroup("AerovelenceMod:SilverBars", group);
		}

		public override void Load()
		{
			Instance = this;
			if (Main.netMode != NetmodeID.Server)
			{

				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns"), ItemType("CrystalCavernsBoxItem"), TileType("CrystalCavernsBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler"), ItemType("CrystalTumblerBoxItem"), TileType("CrystalTumblerBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium"), ItemType("SnowriumBoxItem"), TileType("SnowriumBox"));
			}
		}









		public override void Close()
		{
			var slots = new int[] {
				GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns")
			};
			foreach (var slot in slots)
			{
				if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
				{
					Main.music[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
				}
			}

			base.Close();
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
			{
				return;
			}
			Player player = Main.LocalPlayer;
			if (player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns)
			{
				music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns");
				priority = MusicPriority.BiomeMedium;
			}
		}
		public static AeroMod Instance { get; private set; }

		public override void Unload()
		{
			Instance = null;
		}
	}
}