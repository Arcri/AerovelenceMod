using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using AerovelenceMod.Core;
using AerovelenceMod.Backgrounds.Skies;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Common.IL;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using ReLogic.Graphics;
using AerovelenceMod.Common;
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Items.Weapons.Misc.Melee;
using AerovelenceMod.Content.Items.Weapons.Starglass;
using AerovelenceMod.Content.Biomes;
using AerovelenceMod.Common.Systems.Language;
using System;
using AerovelenceMod.Common.Interfaces;
using System.Linq;
using AerovelenceMod.Common.Globals.Worlds;
using AerovelenceMod.Content.Items.BossSummons;

namespace AerovelenceMod
{
    public class AerovelenceMod : Mod
    {
		public Asset<Effect> TrailShader;

		public static IDictionary<string, Effect> ShaderDict = new Dictionary<string, Effect>();

		public const bool DEBUG = true;

		internal static string PLACEHOLDER_TEXTURE = "AerovelenceMod/Blank";
		public const string ProjectileAssets = "AerovelenceMod/Assets/Projectiles/";
		public const string CrystalCavernsAssets = "AerovelenceMod/Assets/CrystalCaverns/";

		public const string Abbreviation = "AM";
		public const string AbbreviationPrefix = Abbreviation + ":";

		// UI
		internal UserInterface MarauderUserInterface;
		internal UserInterface RockCollectorUserInterface;

		//Mod Support
		public bool FargosModMutant;

        public const string AssetPath = $"{nameof(AerovelenceMod)}/Assets/";

        internal static AerovelenceMod Instance { get; set; }

        public AerovelenceMod()
        {
            Instance = this;
            LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender){ ForceRefreshAllTranslations(); }


        public override void PostSetupContent()
		{
			DoBossChecklistIntegration();
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

		private void DoBossChecklistIntegration()
		{
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
				string cyvercryInternalName = "Cyvercry";
				float cyvercryWeight = 12.3f;
				Func<bool> cyvercryDowned = () => DownedWorld.DownedCyvercry;
				int cyvercryBossType = ModContent.NPCType<Content.NPCs.Bosses.Cyvercry.Cyvercry2>();
				int cyvercrySpawnItem = ModContent.ItemType<Content.Items.BossSummons.ObsidianEye>();
				List<int> cyvercryCollectibles = new List<int>()
				{
					ModContent.ItemType<Content.Items.Weapons.Aurora.Eos.Eos>()
				};
                LocalizedText cyvercrySpawnInfo = Terraria.Localization.Language.GetText("Mods.AerovelenceMod.NPCs.Cyvercry2.SpawnInfo").WithFormatArgs("[i:" + ModContent.ItemType<ObsidianEye>() + "]");
                bossChecklistMod.Call(
					"LogBoss",
					Instance,
					cyvercryInternalName,
					cyvercryWeight,
					cyvercryDowned,
					cyvercryBossType,
					new Dictionary<string, object>()
					{
						["spawnItems"] = cyvercrySpawnItem,
						["collectibles"] = cyvercryCollectibles,
						["spawnInfo"] = cyvercrySpawnInfo

					}
				);

				string tumblerInternalName = "CrystalTumbler";
				float tumblerWeight = 1.8f;
				Func<bool> tumblerDowned = () => DownedWorld.DownedCrystalTumbler;
				int tumblerBossType = ModContent.NPCType<Content.NPCs.Bosses.CrystalTumbler.CrystalTumbler2>();
				int tumblerSpawnItem = ModContent.ItemType<Content.Items.BossSummons.LargeGeode>();
				List<int> tumblerCollectibles = new List<int>()
				{

				};
				LocalizedText tumblerDisplayName = Terraria.Localization.Language.GetText("Mods.AerovelenceMod.NPCs.CrystalTumbler.DisplayName");
                LocalizedText tumblerSpawnInfo = Terraria.Localization.Language.GetText("Mods.AerovelenceMod.NPCs.CrystalTumbler.SpawnInfo").WithFormatArgs("[i:" + ModContent.ItemType<LargeGeode>() + "]");
				Action<SpriteBatch, Rectangle, Color> tumblerPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
				{
					Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler2").Value;
					Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
					spriteBatch.Draw(texture, centered, color);
                    Texture2D eyeTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalTumbler2Eye").Value;
                    Vector2 eyeCentered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                    spriteBatch.Draw(eyeTexture, eyeCentered, color);

                };
				bossChecklistMod.Call(
					"LogBoss",
					Instance,
					tumblerInternalName,
					tumblerWeight,
					tumblerDowned,
					tumblerBossType,
					new Dictionary<string, object>()
					{
						["spawnItems"] = tumblerSpawnItem,
						["collectibles"] = tumblerCollectibles,
						["spawnInfo"] = tumblerSpawnInfo,
						["displayName"] = tumblerDisplayName,
						["customPortrait"] = tumblerPortrait
					}
				);
            }
        }

		public static Effect LegElectricity;
		public static Effect RailgunShader;

		public static Effect DistortShader;
		public static Effect CrystalShine;

		public static Effect Shockwave;

		public static Effect Test2;
		public static Effect BasicTrailShader;
		public static Effect TrailShaderPixelate;
		public static Effect TrailShaderGradient;

        public static Effect fadeShader;

        private List<IOrderedLoadable> loadCache;
        public override void Load()
		{
            // Literally ripped from SLR
            #region IOrderedLoadable Loading
            loadCache = new List<IOrderedLoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    object instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as IOrderedLoadable);
                }
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
            }
            #endregion

            //StarglassParticleDetour.Load();
            ModDetours.Load();

            ModContent.GetInstance<CrystalCavernsSurfaceBiome>();

            if (!Main.dedServ)
			{

				string shaderName = "AerovelenceMod:DistortScreen";
				//string shaderPath = "Effects/DistortScreen";

				var shaderRef = new Ref<Effect>(Assets.Request<Effect>("Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value);
				Filters.Scene[shaderName] = new Filter(new ScreenShaderData(shaderRef, "DistortPass"), EffectPriority.Low);
				Filters.Scene[shaderName].Load();
				//(Filters.Scene[shaderName] = new Filter(new ScreenShaderData(shaderRef, "DistortPass"), EffectPriority.Low)).Load(); //EF.High?


				//Filters.Scene[shaderName] = new Filter(new ScreenShaderData())


				DistortShader = ModContent.Request<Effect>("AerovelenceMod/Effects/DistortScreen", (AssetRequestMode)1).Value;
				CrystalShine = ModContent.Request<Effect>("AerovelenceMod/Effects/CrystalShine", (AssetRequestMode)1).Value;
				Test2 = ModContent.Request<Effect>("AerovelenceMod/Effects/Test2", (AssetRequestMode)1).Value;

				//Shockwave = ModContent.Request<Effect>("AerovelenceMod/Effects/Shockwave", (AssetRequestMode)1).Value;


				Filters.Scene["DistortScreen"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("AerovelenceMod/Effects/DistortScreen", AssetRequestMode.ImmediateLoad).Value), "DistortPass"), EffectPriority.VeryHigh);
				Filters.Scene["DistortScreen"].Load();


				Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("AerovelenceMod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value), "Shockwave"), EffectPriority.VeryHigh);
				Filters.Scene["Shockwave"].Load();
				
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
			SkyManager.Instance["AerovelenceMod:CrystalCavernsSurface"] = new CrystalCavernsSky();
            SkyManager.Instance["AerovelenceMod:CrystalCaverns"] = new CrystalCavernsSky();

            Overlays.Scene.Load();
			Filters.Scene.Load();

            if (Main.netMode != NetmodeID.Server)
			{

				Ref<Effect> MiscGlow = new Ref<Effect>(Assets.Request<Effect>("Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["GlowMisc"] = new MiscShaderData(MiscGlow, "Glow");

				LegElectricity = Instance.Assets.Request<Effect>("Effects/LegElectricity", AssetRequestMode.ImmediateLoad).Value;
				RailgunShader = Instance.Assets.Request<Effect>("Effects/RailgunShader").Value;


				Ref<Effect> LaserShaderRef = new Ref<Effect>(Assets.Request<Effect>("Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["LaserShader"] = new MiscShaderData(LaserShaderRef, "Aura");

				Ref<Effect> ShittyBallRef = new Ref<Effect>(Assets.Request<Effect>("Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["FireBallShader"] = new MiscShaderData(ShittyBallRef, "Aura");

				Ref<Effect> CyverAuraRef = new Ref<Effect>(Assets.Request<Effect>("Effects/CyverAura", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["CyverAura"] = new MiscShaderData(CyverAuraRef, "Aura");

				Ref<Effect> DistortMiscRef = new Ref<Effect>(Assets.Request<Effect>("Effects/DistortMisc", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["DistortMisc"] = new MiscShaderData(DistortMiscRef, "DistortPass");

				BasicTrailShader = Instance.Assets.Request<Effect>("Effects/TrailShaders/BasicTrailShader", AssetRequestMode.ImmediateLoad).Value;
				TrailShaderGradient = Instance.Assets.Request<Effect>("Effects/TrailShaders/TrailShaderGradient", AssetRequestMode.ImmediateLoad).Value;

                fadeShader = Instance.Assets.Request<Effect>("Effects/FadeShader", AssetRequestMode.ImmediateLoad).Value;


                //Ref<Effect> DarkBeamRef = new Ref<Effect>(Assets.Request<Effect>("Effects/DarkBeam", AssetRequestMode.ImmediateLoad).Value);
                //GameShaders.Misc["DarkBeam"] = new MiscShaderData(DarkBeamRef, "Aura");//.UseImage0("Images/Misc/Perlin");

                //Ref<Effect> RimeLaserRef = new Ref<Effect>(Assets.Request<Effect>("Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value);
                //GameShaders.Misc["RimeLaser"] = new MiscShaderData(RimeLaserRef,  "Aura");//.UseImage0("Images/Misc/Perlin");

                //putting this here just in case
                //Filters.Scene.Load();

                //TrailShader = Assets.Request<Effect>("Effects/Trail");

                Terraria.Graphics.Effects.On_FilterManager.EndCapture += FilterManager_EndCapture;
				CreateRender();

			}

			if (!Main.dedServ)
            {
                MarauderUserInterface = new UserInterface();
				RockCollectorUserInterface = new UserInterface();
				//DiscordRichPresence.Initialize();
				//Main.OnTickForThirdPartySoftwareOnly += DiscordRichPresence.Update;
			}

			LoadDetours();

			
        }

        public static bool shouldHide = false;
		
		
        public override void Unload()
		{
			Terraria.Graphics.Effects.On_FilterManager.EndCapture -= FilterManager_EndCapture;
			//StarglassParticleDetour.Unload();

			ModDetours.Unload();

			if (!Main.dedServ)
			{
				//DiscordRichPresence.Deinitialize();
				//Main.OnTickForThirdPartySoftwareOnly -= DiscordRichPresence.Update;
			}
			TrailShader = null;
			BasicTrailShader = null;
			TrailShaderPixelate = null;
			TrailShaderGradient = null;

			UnloadDetours();
			FargosModMutant = false;
			Instance = null;
			LegElectricity = null;
			RailgunShader = null;

            if (LanguageManager.Instance != null)
                LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

		public void ForceRefreshAllTranslations()
		{
			try
			{
				LocalizationPatcher.ApplyNamePatches();
                if (Main.netMode != NetmodeID.Server)
				{
					for (int i = 0; i < Main.player.Length; i++)
					{
						Player player = Main.player[i];
						if (player == null || !player.active)
							continue;
						for (int j = 0; j < player.inventory.Length; j++)
							player.inventory[j].ForceUpdateDisplayName();
						for (int j = 0; j < player.armor.Length; j++)
							player.armor[j].ForceUpdateDisplayName();
						for (int j = 0; j < player.bank.item.Length; j++)
							player.bank.item[j].ForceUpdateDisplayName();
						for (int j = 0; j < player.bank2.item.Length; j++)
							player.bank2.item[j].ForceUpdateDisplayName();
						for (int j = 0; j < player.bank3.item.Length; j++)
							player.bank3.item[j].ForceUpdateDisplayName();
						for (int j = 0; j < player.bank4.item.Length; j++)
							player.bank4.item[j].ForceUpdateDisplayName();
					}
					for (int i = 0; i < Main.item.Length; i++)
					{
						if (Main.item[i] != null && Main.item[i].active)
							Main.item[i].ForceUpdateDisplayName();
					}
					for (int i = 0; i < Main.npc.Length; i++)
					{
						if (Main.npc[i] != null && Main.npc[i].active)
						{
							Main.npc[i].ForceUpdateDisplayName();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warn($"Error refreshing translations: {ex.Message}");
			}
		}
    

    public override void Close()
		{
			base.Close();
		}

        [Obsolete]
        public override void AddRecipeGroups()/* tModPorter Note: Removed. Use ModSystem.AddRecipeGroups */
        {
            {
                RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Iron Bars", new int[]
                {
                    ItemID.IronBar,
                    ItemID.LeadBar
                });
                RecipeGroup.RegisterGroup("AerovelenceMod:IronBars", group);

            }

            {
                RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Silver Bars", new int[]
                {
                    ItemID.SilverBar,
                    ItemID.TungstenBar
                });
                RecipeGroup.RegisterGroup("AerovelenceMod:SilverBars", group);

            }
			{
				RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Adamantite Bars", new int[]
				{
					ItemID.AdamantiteBar,
					ItemID.TitaniumBar
				});
				RecipeGroup.RegisterGroup("AerovelenceMod:TitaniumBars", group);

			}
			{
				RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Cobalt Bars", new int[]
				{
					ItemID.CobaltBar,
					ItemID.PalladiumBar
				});
				RecipeGroup.RegisterGroup("AerovelenceMod:CobaltBars", group);

			}
			{
                RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Evil Materials", new int[]
                {
                    ItemID.ShadowScale,
                    ItemID.TissueSample
                });

                RecipeGroup.RegisterGroup("AerovelenceMod:EvilMaterials", group);
            }
            {
                RecipeGroup group = new RecipeGroup(() => Terraria.Localization.Language.GetTextValue("LegacyMisc.37") + " Gold Bars", new int[]
                {
                    ItemID.PlatinumBar,
                    ItemID.GoldBar
                });

                RecipeGroup.RegisterGroup("AerovelenceMod:GoldBars", group);
            }
        }

		private void LoadDetours()
		{
			AeroPlayer aeroPlayer = new AeroPlayer();
			//On.Terraria.Player.ItemCheck += aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw += DrawMoonlordLayer;
		}

		private void UnloadDetours()
		{
			AeroPlayer aeroPlayer = new AeroPlayer();
			//On.Terraria.Player.ItemCheck -= aeroPlayer.DetouredItemCheck;
			// IL.Terraria.Main.DoDraw -= DrawMoonlordLayer;
		}

		//Ripped from Regressus which was ripped from a chinese example mod i think
		public RenderTarget2D render3;
		private void FilterManager_EndCapture(Terraria.Graphics.Effects.On_FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, 
			RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
			GraphicsDevice gd = Main.instance.GraphicsDevice;
			SpriteBatch sb = Main.spriteBatch;

			#region ozoneShredder
			gd.SetRenderTarget(render3);
			gd.Clear(Color.Transparent);
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
			sb.End();
			gd.SetRenderTarget(Main.screenTargetSwap);
			gd.Clear(Color.Transparent);
			sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			foreach (Projectile projectile in Main.projectile)
			{
				//Want to do this first and separate because it will weed out more projectiles first, despite checking again later
				if (projectile.type == ModContent.ProjectileType<DistortProj>())
                {
					if (projectile.active && projectile.type == ModContent.ProjectileType<DistortProj>())
                    {
						Texture2D tex = null;
						float overallScale = 1;

						if (projectile.ModProjectile is DistortProj distort)
                        {
							tex = distort.tex;
							overallScale = distort.scale;
                        }

						Vector2 toProj = (projectile.Center - Main.player[Main.myPlayer].Center);
						Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + (toProj * (1 - Main.GameZoomTarget) * -1), null, Color.White, projectile.rotation, tex.Size() / 2, overallScale * projectile.scale * 0.5f * Main.GameZoomTarget, SpriteEffects.None, 0f);
					}
				}
			}
			sb.End();
			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Transparent);
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Test2.CurrentTechnique.Passes[0].Apply();
			Test2.Parameters["tex0"].SetValue(Main.screenTargetSwap);
			Test2.Parameters["i"].SetValue(0.02f);
			sb.Draw(render3, Vector2.Zero, Color.White);
			sb.End();
			#endregion


			orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

		}

		public void CreateRender()
		{
			Main.QueueMainThreadAction(() =>
			{
				render3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
			});
		}
		private void Main_OnResolutionChanged(Vector2 obj)
		{
			CreateRender();
		}
	}
}
