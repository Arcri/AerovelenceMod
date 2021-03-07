//using AerovelenceMod.Systems;
using AerovelenceMod.Items.Accessories;
using AerovelenceMod.Items.BossSummons;
using AerovelenceMod.Items.Weapons.Magic;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Ranged;
using AerovelenceMod.Items.Weapons.Thrown;
using AerovelenceMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.World.Generation;
using AerovelenceMod.Prim;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Skies;
using AerovelenceMod.ILHooks;

namespace AerovelenceMod
{
	public class AeroMod : Mod
	{
		public static ModHotKey ArmorHotKey;





		// Todo: Convert mod.XType to the new ModContent system
		internal UserInterface MarauderUserInterface;
		private UserInterface _aeroUserInterface;
		public static PrimTrailManager primitives;
		public override void PostSetupContent()
		{
			Mod bossChecklist = ModLoader.GetMod("BossChecklist");
			if (bossChecklist != null)
			{

				bossChecklist.Call("AddBossWithInfo", "Crystal Tumbler", 1.5f, (Func<bool>)(() => AeroWorld.downedCrystalTumbler), "Use a [i:" + ItemType("LargeGeode") + "] in the Crystal Caverns");
				bossChecklist.Call("AddBossWithInfo", "Rimegeist", 5.5f, (Func<bool>)(() => AeroWorld.downedRimegeist), "Use a [i:" + ItemType("GlowingSnow") + "] at night in the snow biome");
				bossChecklist.Call("AddBossWithInfo", "LightningMoth", 6.5f, (Func<bool>)(() => AeroWorld.downedRimegeist), "Use a [i:" + ItemType("GlowingSnow") + "] at night in the Crystal Fields");
				bossChecklist.Call("AddBossWithInfo", "Cyvercry", 9.5f, (Func<bool>)(() => AeroWorld.downedCyvercry), "Use a [i:" + ItemType("ObsidianEye") + "] at night");
				bossChecklist.Call("AddBossWithInfo", "TheFallen", 12.5f, (Func<bool>)(() => AeroWorld.downedCyvercry), "Use a [i:" + ItemType("ObsidianEye") + "] in the sky");



				bossChecklist.Call("AddToBossLoot", "AerovelenceMod", "Crystal Tumbler", new List<int> { ModContent.ItemType<DiamondDuster>(), ModContent.ItemType<DarkCrystalStaff>(), ModContent.ItemType<CavernMauler>(), ModContent.ItemType<CavernousImpaler>(), ModContent.ItemType<PrismThrasher>(), ModContent.ItemType<CrystallineQuadshot>(), ModContent.ItemType<PrismPiercer>(), ModContent.ItemType<PrismaticSoul>() });
				bossChecklist.Call("AddToBossSpawnItems", "AerovelenceMod", "Crystal Tumbler", new List<int> { ModContent.ItemType<LargeGeode>() });
			}
		}

		public override void AddRecipeGroups()
		{
			{
				RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bars", new int[]
				{
					ItemID.SilverBar,
					ItemID.TungstenBar
				});
				RecipeGroup.RegisterGroup("AerovelenceMod:SilverBars", group);

			}
			{
				RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Evil Materials", new int[]
				{
				ItemID.ShadowScale,
				ItemID.TissueSample
				});

				RecipeGroup.RegisterGroup("AerovelenceMod:EvilMaterials", group);
			}
			{
				RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gold Bars", new int[]
				{
				ItemID.PlatinumBar,
				ItemID.GoldBar
				});

				RecipeGroup.RegisterGroup("AerovelenceMod:GoldBars", group);
			}
		}



		public override void Load()
		{
			GemGrapplingRange.Load();
			ArmorHotKey = RegisterHotKey("Armor Set Bonus", "F");
			Filters.Scene["AerovelenceMod:CrystalTorrents"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.168f, 0.168f, 0.188f).UseOpacity(0.1f), EffectPriority.High);
			SkyManager.Instance["AerovelenceMod:CrystalTorrents"] = new CrystalTorrentSky();
			Instance = this;
			if (Main.netMode != NetmodeID.Server)
			{

				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns"), ItemType("CrystalCavernsBoxItem"), TileType("CrystalCavernsBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler"), ItemType("CrystalTumblerBoxItem"), TileType("CrystalTumblerBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Rimegeist"), ItemType("RimegeistBoxItem"), TileType("RimegeistBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/TheFallen"), ItemType("TheFallenBoxItem"), TileType("TheFallenBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Cyvercry"), ItemType("CyvercryBoxItem"), TileType("CyvercryBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CursedMachine"), ItemType("CursedMachineBoxItem"), TileType("CursedMachineBox"));
			}
			if(!Main.dedServ)
            {
				_aeroUserInterface = new UserInterface();
				MarauderUserInterface = new UserInterface();
			}
			primitives = new PrimTrailManager();
			LoadDetours();
		}
		public override void UpdateUI(GameTime gameTime)
		{
			MarauderUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"AerovelenceMod: Marauder UI",
					delegate
					{
						// If the current UIState of the UserInterface is null, nothing will draw. We don't need to track a separate .visible value.
						MarauderUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
		public override void Close()
		{
			var slots = new int[] {
				GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns"),
				GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel")
		};
			foreach (var slot in slots)
			{
				if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
				{
					Main.music[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
				}
			}

			{
				GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel");
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
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
			{
				return;
			}
			if (player.GetModPlayer<AeroPlayer>().ZoneCrystalCitadel)
			{
				music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel");
				priority = MusicPriority.BiomeMedium;
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);

			recipe = new ModRecipe(this);
			recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 5);
			recipe.AddIngredient(ItemID.Cloud, 5);
			recipe.AddIngredient(ItemID.SunplateBlock, 3);
			recipe.AddTile(TileID.SkyMill);
			recipe.SetResult(ItemID.LuckyHorseshoe, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Cloud, 25);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddTile(TileID.SkyMill);
			recipe.SetResult(ItemID.CloudinaBottle, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SandBlock, 25);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddTile(TileID.SkyMill);
			recipe.SetResult(ItemID.SandstorminaBottle, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SnowBlock, 25);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddTile(TileID.SkyMill);
			recipe.SetResult(ItemID.BlizzardinaBottle, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Starfish, 12);
			recipe.AddIngredient(ItemID.Seashell, 12);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddTile(TileID.SkyMill);
			recipe.SetResult(ItemID.TsunamiInABottle, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddRecipeGroup("IronBar", 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(ItemID.Aglet, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddRecipeGroup("IronBar", 4);
			recipe.AddIngredient(ItemID.Stinger, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(ItemID.AnkletoftheWind, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.HellstoneBar, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(ItemID.LavaCharm, 1);
			recipe.AddRecipe();

		}





		public static AeroMod Instance { get; private set; }

		public override void Unload()
        {
			ArmorHotKey = null;
			UnloadDetours();
			Instance = null;
		}
		#region detours
		private void LoadDetours()
		{
			On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
			//IL.Terraria.Main.DoDraw += DrawMoonlordLayer;
		}

		private void UnloadDetours()
		{
			On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
			//IL.Terraria.Main.DoDraw -= DrawMoonlordLayer;
		}


		private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
		{
			primitives.DrawTrails(Main.spriteBatch);
			orig(self);
		}
		#endregion
	}
}