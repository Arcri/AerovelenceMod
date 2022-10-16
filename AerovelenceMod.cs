using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using AerovelenceMod.Core;
using AerovelenceMod.Backgrounds.Skies;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Common.IL;
using AerovelenceMod.Core.Prim;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;
using AerovelenceMod.Content.Skies;

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


        }
		public override void PostSetupContent()
		{
			/*var bossChecklist = ModLoader.GetMod("BossChecklist");
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
					);


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
				);
		}*/
		}

		public static Effect LegElectricity;
		public static Effect RailgunShader;
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				string shaderName = "AerovelenceMod:CavernCrystalShine";
				string shaderPath = "Effects/CavernCrystalShine";

				var shaderRef = new Ref<Effect>(Instance.Assets.Request<Effect>(shaderPath).Value);
				(Filters.Scene[shaderName] = new Filter(new ScreenShaderData(shaderRef, shaderName + "Pass"), EffectPriority.High)).Load();

			} 
			GemGrapplingRange.Load();

            

			Filters.Scene["AerovelenceMod:FoggyFields"] = 
                new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.168f, 0.168f, 0.188f).UseOpacity(0.1f), EffectPriority.High);
			
            //yManager.Instance["AerovelenceMod:FoggyFields"] = new CrystalTorrentSky();

			Filters.Scene["AerovelenceMod:CrystalTorrents"] = 
                new Filter(new CrystalTorrentScreenShaderData("FilterBloodMoon").UseColor(0.0f, 0.5f, 0.0f), EffectPriority.Medium);

			Filters.Scene["AerovelenceMod:DarkNights"] =
                new Filter(new DarkNightScreenShaderData("FilterBloodMoon").UseColor(0.0f, 0.2f, 0.2f), EffectPriority.Medium);

			SkyManager.Instance["AerovelenceMod:Cyvercry2"] = new CyverSky();

			Overlays.Scene.Load();
			Filters.Scene.Load();

            if (Main.netMode != NetmodeID.Server)
			{

				Ref<Effect> MiscGlow = new Ref<Effect>(Assets.Request<Effect>("Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["GlowMisc"] = new MiscShaderData(MiscGlow, "Glow");

				LegElectricity = Instance.Assets.Request<Effect>("Effects/LegElectricity").Value;
				RailgunShader = Instance.Assets.Request<Effect>("Effects/RailgunShader").Value;


				Ref<Effect> LaserShaderRef = new Ref<Effect>(Assets.Request<Effect>("Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["LaserShader"] = new MiscShaderData(LaserShaderRef, "Aura");//.UseImage0("Images/Misc/Perlin");

				//Ref<Effect> DarkBeamRef = new Ref<Effect>(Assets.Request<Effect>("Effects/DarkBeam", AssetRequestMode.ImmediateLoad).Value);
				//GameShaders.Misc["DarkBeam"] = new MiscShaderData(DarkBeamRef, "Aura");//.UseImage0("Images/Misc/Perlin");

				//Ref<Effect> RimeLaserRef = new Ref<Effect>(Assets.Request<Effect>("Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value);
				//GameShaders.Misc["RimeLaser"] = new MiscShaderData(RimeLaserRef,  "Aura");//.UseImage0("Images/Misc/Perlin");

			}

			if (!Main.dedServ)
            {
                MarauderUserInterface = new UserInterface();
				RockCollectorUserInterface = new UserInterface();
				//DiscordRichPresence.Initialize();
				//Main.OnTickForThirdPartySoftwareOnly += DiscordRichPresence.Update;
			}

			primitives = new PrimTrailManager();

			LoadDetours();
		}

		public override void Unload()
		{
			if (!Main.dedServ)
			{
				//DiscordRichPresence.Deinitialize();
				//Main.OnTickForThirdPartySoftwareOnly -= DiscordRichPresence.Update;
			}
			UnloadDetours();
			FargosModMutant = false;
			Instance = null;
			LegElectricity = null;
			RailgunShader = null;
		}

		public override void Close()
		{

			base.Close();
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
				RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Adamantite Bars", new int[]
				{
					ItemID.AdamantiteBar,
					ItemID.TitaniumBar
				});
				RecipeGroup.RegisterGroup("AerovelenceMod:TitaniumBars", group);

			}
			{
				RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Cobalt Bars", new int[]
				{
					ItemID.CobaltBar,
					ItemID.PalladiumBar
				});
				RecipeGroup.RegisterGroup("AerovelenceMod:CobaltBars", group);

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

		

		private void LoadDetours()
		{
			On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
			AeroPlayer aeroPlayer = new AeroPlayer();
			//On.Terraria.Player.ItemCheck += aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw += DrawMoonlordLayer;
		}

		private void UnloadDetours()
		{
			On.Terraria.Main.DrawProjectiles -= Main_DrawProjectiles;
			AeroPlayer aeroPlayer = new AeroPlayer();
			//On.Terraria.Player.ItemCheck -= aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw -= DrawMoonlordLayer;
		}
		private void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
		{
			primitives.DrawTrails(Main.spriteBatch);
			orig(self);
		}
    }
}
