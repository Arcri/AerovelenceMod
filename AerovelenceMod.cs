using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using AerovelenceMod.Core;
using AerovelenceMod.Backgrounds.Skies;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Common.IL;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Core.Prim;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using AerovelenceMod.Content.Items.BossSummons;
using System;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using AerovelenceMod.Content.Items.Weapons.Melee;
using AerovelenceMod.Content.Items.Weapons.Magic;
using AerovelenceMod.Content.Items.TreasureBags;
using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Items.Placeables.Trophies;
using AerovelenceMod.Content.Items.Armor.Vanity;
using AerovelenceMod.Content.Items.Placeables.MusicBoxes;
using AerovelenceMod.Content.Items.Weapons.Summoning;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;

namespace AerovelenceMod
{
    public class AerovelenceMod : Mod
    {
		public const bool DEBUG = true;

		internal static string PLACEHOLDER_TEXTURE = "AerovelenceMod/Blank";
		public const string ProjectileAssets = "AerovelenceMod/Assets/Projectiles/";
		public const string CrystalCavernsAssets = "AerovelenceMod/Assets/CrystalCaverns/";

		public const string Abbreviation = "AM";
		public const string AbbreviationPrefix = Abbreviation + ":";

		// Hotkeys
		public static ModHotKey ArmorHotKey;

		// UI
		internal UserInterface MarauderUserInterface;
		internal UserInterface RockCollectorUserInterface;

		public static PrimTrailManager primitives;

		//Mod Support
		public bool FargosModMutant;



		internal static AerovelenceMod Instance { get; set; }

        public AerovelenceMod()
        {
            Instance = this;

            Properties = new ModProperties
            {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }
		public override void PostSetupContent()
		{
			var bossChecklist = ModLoader.GetMod("BossChecklist");
			var terrariaAmbience = ModLoader.GetMod("TerrariaAmbience");
			if (terrariaAmbience != null)
            {
				terrariaAmbience.Call("AddTilesToList", this, "Stone", new string[] { "CavernStone", "CavernCrystal", "ChargedStone", "HardenedIce", "SmoothCavernStone", "MilitaryMetal", "SlateOreBlock", "PhanticBarPlaced" }, null);
				terrariaAmbience.Call("AddTilesToList", this, "Grass", new string[] { "CrystalDirt", "CrystalGrass", "ValleyGrass", "ValleyDirt" }, null);
				terrariaAmbience.Call("AddTilesToList", this, "Sand", new string[] { "CrystalSand" }, null);
			}
			if (bossChecklist != null)
			{
				bossChecklist.Call(
					"AddBoss",
					1.5f,
					ModContent.NPCType<CrystalTumbler>(),
					this,
					"Crystal Tumbler",
					(Func<bool>)(() => DownedWorld.DownedCrystalTumbler),
					ModContent.ItemType<LargeGeode>(),
					new List<int>
					{
						ModContent.ItemType<CrystalTumblerMask>(),
						ModContent.ItemType<CrystalTumblerTrophy>(),
						ModContent.ItemType<CrystalTumblerBoxItem>()
					},
					new List<int>
					{
						ModContent.ItemType<CrystalTumblerBag>(),
						ModContent.ItemType<DiamondDuster>(),
						ModContent.ItemType<DarkCrystalStaff>(),
						ModContent.ItemType<PrismThrasher>(),
						ModContent.ItemType<CavernousImpaler>(),
						ModContent.ItemType<PrismPiercer>(),
						ModContent.ItemType<CrystallineQuadshot>(),
						ModContent.ItemType<PrismaticSoul>() },
					$"Use a [i:" + ModContent.ItemType<LargeGeode>() + "] in the Crystal Caverns"
				);


				/*bossChecklist.Call(
					"AddBoss",
					5.5f,
					ModContent.NPCType<Rimegeist>(),
					this,
					"Rimegeist",
					(Func<bool>)(() => DownedWorld.DownedRimegeist),
					ModContent.ItemType<GlowingSnow>(),
					new List<int>
					{
						ModContent.ItemType<RimegeistMask>(),
						ModContent.ItemType<RimegeistTrophy>(),
						ModContent.ItemType<RimegeistBoxItem>()
					},
					new List<int> { ModContent.ItemType<RimegeistBag>(),
						ModContent.ItemType<FragileIceCrystal>(),
						ModContent.ItemType<IcySaber>(),
						ModContent.ItemType<CrystalArch>(),
						ModContent.ItemType<Snowball>(),
						ModContent.ItemType<DeepFreeze>() },
					$"Use a [i:" + ModContent.ItemType<GlowingSnow>() + "] at night"
					);

				bossChecklist.Call(
					"AddBoss",
					6.5f,
					ModContent.NPCType<LightningMoth>(),
					this,
					"Lightning Moth",
					(Func<bool>)(() => DownedWorld.DownedLightningMoth),
					ModContent.ItemType<GlowingSnow>(),
					new List<int>
					{
						ModContent.ItemType<LightningMothMask>(),
						ModContent.ItemType<LightningMothTrophy>(),
						ModContent.ItemType<LightningMothBoxItem>()
					},
					new List<int>
					{
						ModContent.ItemType<RimegeistBag>(),
						ModContent.ItemType<BladeOfTheSkies>(),
						ModContent.ItemType<EyeOfTheGreatMoth>(),
						ModContent.ItemType<MothLeg>(),
						ModContent.ItemType<Electromagnetism>(),
						ModContent.ItemType<Florentine>(),
						ModContent.ItemType<ElectrapulseCanister>(),
						ModContent.ItemType<SongOfTheStorm>(),
						ModContent.ItemType<StaticSurge>()},
					$"Use a [i:" + ModContent.ItemType<TorrentialTotem>() + "] at night in the Crystal Caverns"
					);*/


				bossChecklist.Call(
					"AddBoss",
					9.5f,
					ModContent.NPCType<Cyvercry>(),
					this,
					"Cyvercry",
					(Func<bool>)(() => DownedWorld.DownedCyvercry),
					ModContent.ItemType<GlowingSnow>(),
					new List<int>
					{
						ModContent.ItemType<CyvercryMask>(),
						ModContent.ItemType<CyvercryTrophy>(),
						ModContent.ItemType<CyvercryBoxItem>()
					},
					new List<int>
					{ 
						ModContent.ItemType<CyvercryBag>(),
						ModContent.ItemType<Oblivion>(),
						ModContent.ItemType<Cyverthrow>(),
						ModContent.ItemType<CyverCannon>(),
						ModContent.ItemType<DarknessDischarge>(),
						ModContent.ItemType<AetherVision>(),
						ModContent.ItemType<EnergyShield>()},
					$"Use a [i:" + ModContent.ItemType<ObsidianEye>() + "] at night anywhere"
					);

				/*bossChecklist.Call(
					"AddBoss",
					12.5f,
					ModContent.NPCType<TheFallenSpirit>(),
					this,
					"The Fallen",
					(Func<bool>)(() => DownedWorld.DownedTheFallen),
					ModContent.ItemType<GlowingSnow>(),
					new List<int>
					{
						ModContent.ItemType<TheFallenMask>(),
						ModContent.ItemType<TheFallenTrophy>(),
						ModContent.ItemType<TheFallenBoxItem>()
					},

					new List<int>
					{
						ModContent.ItemType<TheFallenBag>(),
						ModContent.ItemType<OzoneShredder>(),
						ModContent.ItemType<WindboundWave>(),
						ModContent.ItemType<StormRazor>()
					},
					$"Use a [i:" + ModContent.ItemType<AncientAmulet>() + "] during the day in the sky"
					);*/
			}
		}

		public static Effect LegElectricity;
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				string shaderName = "AerovelenceMod:CavernCrystalShine";
				string shaderPath = "Effects/CavernCrystalShine";

				var shaderRef = new Ref<Effect>(Instance.GetEffect(shaderPath));
				(Filters.Scene[shaderName] = new Filter(new ScreenShaderData(shaderRef, shaderName + "Pass"), EffectPriority.High)).Load();

			}
			GemGrapplingRange.Load();

            ArmorHotKey = RegisterHotKey("Armor Set Bonus", "F");

			Filters.Scene["AerovelenceMod:FoggyFields"] = 
                new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.168f, 0.168f, 0.188f).UseOpacity(0.1f), EffectPriority.High);
			
            SkyManager.Instance["AerovelenceMod:FoggyFields"] = new CrystalTorrentSky();

			Filters.Scene["AerovelenceMod:CrystalTorrents"] = 
                new Filter(new CrystalTorrentScreenShaderData("FilterBloodMoon").UseColor(0.0f, 0.5f, 0.0f), EffectPriority.Medium);

			Filters.Scene["AerovelenceMod:DarkNights"] =
                new Filter(new DarkNightScreenShaderData("FilterBloodMoon").UseColor(0.0f, 0.2f, 0.2f), EffectPriority.Medium);

			Overlays.Scene.Load();
			Filters.Scene.Load();

            if (Main.netMode != NetmodeID.Server)
			{
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns"), ItemType("CrystalCavernsBoxItem"), TileType("CrystalCavernsBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalTumbler"), ItemType("CrystalTumblerBoxItem"), TileType("CrystalTumblerBox"));
			//	AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Rimegeist"), ItemType("RimegeistBoxItem"), TileType("RimegeistBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium"), ItemType("SnowriumBoxItem"), TileType("SnowriumBox"));
				//AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/TheFallen"), ItemType("TheFallenBoxItem"), TileType("TheFallenBox"));
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Cyvercry"), ItemType("CyvercryBoxItem"), TileType("CyvercryBox"));
				//AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/CursedMachine"), ItemType("CursedMachineBoxItem"), TileType("CursedMachineBox"));

				LegElectricity = Instance.GetEffect("Effects/LegElectricity");
			}

            if (!Main.dedServ)
            {
                MarauderUserInterface = new UserInterface();
				RockCollectorUserInterface = new UserInterface();
				DiscordRichPresence.Initialize();
				Main.OnTick += DiscordRichPresence.Update;
			}

			primitives = new PrimTrailManager();

			LoadDetours();
		}

        public override void Unload()
        {
			if (!Main.dedServ)
			{
				DiscordRichPresence.Deinitialize();
				Main.OnTick -= DiscordRichPresence.Update;
			}
			UnloadDetours();
			FargosModMutant = false;
            ArmorHotKey = null;
            Instance = null;
			LegElectricity = null;
        }

		public override void UpdateUI(GameTime gameTime) => MarauderUserInterface?.Update(gameTime);

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"AerovelenceMod: Marauder UI",
					delegate
					{
                        MarauderUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"AerovelenceMod: RockCollector UI",
					delegate
					{
                        RockCollectorUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void Close()
		{
			var slots = new [] 
            {
				GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns"),
				GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel")
		    };

			foreach (var slot in slots)
                if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
                    Main.music[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);

            foreach (var slot in slots)
                if (Main.music.IndexInRange(slot) && Main.music[slot]?.IsPlaying == true)
                    Main.music[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);

            base.Close();
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
			if (Main.gameMenu)
                return;
			
			Player player = Main.LocalPlayer;
            var zonePlayer = player.GetModPlayer<ZonePlayer>();

            if (zonePlayer == null)
                return;

            if (zonePlayer.ZoneCrystalCaverns)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/CrystalCaverns");
                priority = MusicPriority.BiomeHigh;
            }

            if (zonePlayer.ZoneCrystalCitadel)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel");
                priority = MusicPriority.BiomeHigh;
			}

            /*if (!Main.dayTime && DarkNightWorld.DarkNight)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Citadel");
                priority = MusicPriority.Environment;
            }*/
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

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(this);

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.IceMirror, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(ItemID.MagicMirror, 1);
			recipe.AddRecipe();

			recipe = new ModRecipe(this);
			recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 2);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddTile(TileID.Bottles);
			recipe.SetResult(ItemID.WormholePotion, 1);
			recipe.AddRecipe();


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

		private void LoadDetours()
		{
			On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
			AeroPlayer aeroPlayer = new AeroPlayer();
			On.Terraria.Player.ItemCheck += aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw += DrawMoonlordLayer;
		}

		private void UnloadDetours()
		{
			On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
			AeroPlayer aeroPlayer = new AeroPlayer();
			On.Terraria.Player.ItemCheck -= aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw -= DrawMoonlordLayer;
		}
		public override void MidUpdateProjectileItem()
        {
			if (Main.netMode != NetmodeID.Server)
            {
                primitives.UpdateTrails();
            }
		}
		private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
		{
			primitives.DrawTrails(Main.spriteBatch);
			orig(self);
		}
    }
}
