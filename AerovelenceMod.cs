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
using AerovelenceMod.Content.Skies;
using AerovelenceMod.Common.Globals.SkillStrikes;
using ReLogic.Graphics;
using AerovelenceMod.Common;
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Items.Weapons.Misc.Melee;

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

		public static Effect DistortShader;
		public static Effect CrystalShine;

		public static Effect Shockwave;

		public static Effect Test2;
		public static Effect BasicTrailShader;
		public static Effect TrailShaderPixelate;
		public static Effect TrailShaderGradient;


		public override void Load()
		{

			ModDetours.Load();


			if (Main.netMode != NetmodeID.Server)
			{

				string shaderName = "AerovelenceMod:DistortScreen";
				string shaderPath = "Effects/DistortScreen";

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

				Ref<Effect> BasicTrailRef = new Ref<Effect>(Assets.Request<Effect>("Effects/TrailShaders/IchorMissileExhaust", AssetRequestMode.ImmediateLoad).Value);
				GameShaders.Misc["IchorMissileExhaust"] = new MiscShaderData(BasicTrailRef, "ShaderPass");

				//Ref<Effect> BasicTrailRef2 = new Ref<Effect>(Assets.Request<Effect>("Effects/TrailShaders/BasicTrailShader", AssetRequestMode.ImmediateLoad).Value);
				//GameShaders.Misc["BasicTrailShader"] = new MiscShaderData(BasicTrailRef2, "DefaultPass");
				
				//Ref<Effect> DistortionRef = new Ref<Effect>(Assets.Request<Effect>("Effects/Distortion", AssetRequestMode.ImmediateLoad).Value);
				//Filters.Scene["AerovelenceMod:Distortion"] = new Filter(new ScreenShaderData("DistortionPulsePass"), EffectPriority.VeryHigh);

				BasicTrailShader = Instance.Assets.Request<Effect>("Effects/TrailShaders/BasicTrailShader", AssetRequestMode.ImmediateLoad).Value;
				TrailShaderPixelate = Instance.Assets.Request<Effect>("Effects/TrailShaders/TrailShaderPixelate", AssetRequestMode.ImmediateLoad).Value;
				TrailShaderGradient = Instance.Assets.Request<Effect>("Effects/TrailShaders/TrailShaderGradient", AssetRequestMode.ImmediateLoad).Value;


				//Ref<Effect> DarkBeamRef = new Ref<Effect>(Assets.Request<Effect>("Effects/DarkBeam", AssetRequestMode.ImmediateLoad).Value);
				//GameShaders.Misc["DarkBeam"] = new MiscShaderData(DarkBeamRef, "Aura");//.UseImage0("Images/Misc/Perlin");

				//Ref<Effect> RimeLaserRef = new Ref<Effect>(Assets.Request<Effect>("Effects/RimeLaser", AssetRequestMode.ImmediateLoad).Value);
				//GameShaders.Misc["RimeLaser"] = new MiscShaderData(RimeLaserRef,  "Aura");//.UseImage0("Images/Misc/Perlin");

				//putting this here just in case
				//Filters.Scene.Load();

				//TrailShader = Assets.Request<Effect>("Effects/Trail");

				On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture;
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
			On.Terraria.Graphics.Effects.FilterManager.EndCapture -= FilterManager_EndCapture;


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
		private void FilterManager_EndCapture(On.Terraria.Graphics.Effects.FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, 
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
				if (projectile.type == ModContent.ProjectileType<OzoneShredderHeldProj>() || 
					projectile.type == ModContent.ProjectileType<OzoneShredderImpact>() || 
					projectile.type == ModContent.ProjectileType<DistortProj>())
                {
					
					if (projectile.active && projectile.ai[1] == 1 && projectile.type == ModContent.ProjectileType<OzoneShredderHeldProj>())
                    {
						//Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/twirl_02");
						//Main.spriteBatch.Draw(a, Main.player[projectile.owner].Center - Main.screenPosition, null, Color.White, projectile.rotation, a.Size() / 2, new Vector2(0.75f, 0.75f) * projectile.scale, projectile.ai[0] != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically, 0f);

						Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/twirl_02");
						float extraRotation = projectile.ai[0] != 1 ? MathHelper.PiOver4 + 0.3f : MathHelper.PiOver2 - 1f;
						Main.spriteBatch.Draw(a, Main.player[projectile.owner].Center - Main.screenPosition, null, Color.White, projectile.rotation + extraRotation, a.Size() / 2, new Vector2(0.75f, 0.75f) * projectile.scale, projectile.ai[0] != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);


						//Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/spiky_16-export");
						//float extraRotation = projectile.ai[0] != 1 ? MathHelper.PiOver4 - 0.1f : MathHelper.PiOver2 * -1 - MathHelper.PiOver4 + 0.1f;
						//Main.spriteBatch.Draw(a, Main.player[projectile.owner].Center - Main.screenPosition, null, Color.White, projectile.rotation + extraRotation, a.Size() / 2, new Vector2(2.25f, 0.75f) * projectile.scale, projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
					}
                    else if (projectile.active && projectile.type == ModContent.ProjectileType<OzoneShredderImpact>())
                    {
						//Main.GameZoomTarget (from 1 to 2)
						
						Vector2 toProj = (projectile.Center - Main.player[Main.myPlayer].Center);

						Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
						//Texture2D a = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/OzoneShredderImpact");
						Main.spriteBatch.Draw(a, projectile.Center - Main.screenPosition + (toProj * (1 - Main.GameZoomTarget) * -1), null, Color.White, projectile.rotation, a.Size() / 2, projectile.scale * 0.5f * Main.GameZoomTarget, SpriteEffects.None, 0f);
					}
					else if (projectile.active && projectile.type == ModContent.ProjectileType<DistortProj>())
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
