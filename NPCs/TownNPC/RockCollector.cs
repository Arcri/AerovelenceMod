using System;
using System.Linq;
using AerovelenceMod;
using AerovelenceMod.Dusts;
using AerovelenceMod.Items.Accessories;
using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Others.Misc;
using AerovelenceMod.Items.Others.Quest;
using AerovelenceMod.Items.Others.UIButton;
using AerovelenceMod.Items.Weapons.CavernsHMPlaceholder;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Thrown;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.TownNPC
{
	// [AutoloadHead] and npc.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class RockCollector : ModNPC
	{
		public override string Texture => "AerovelenceMod/NPCs/TownNPC/RockCollector";

		public override bool Autoload(ref string name)
		{
			name = "RockCollector";
			return mod.Properties.Autoload;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from .lang files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[npc.type] = 25;
			NPCID.Sets.ExtraFramesCount[npc.type] = 9;
			NPCID.Sets.AttackFrameCount[npc.type] = 4;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = 0;
			NPCID.Sets.AttackTime[npc.type] = 90;
			NPCID.Sets.AttackAverageChance[npc.type] = 30;
			NPCID.Sets.HatOffsetY[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;
			npc.defense = 15;
			npc.lifeMax = 250;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
			animationType = NPCID.Guide;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			int num = npc.life > 0 ? 1 : 5;
			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Sparkle>());
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			for (int k = 0; k < 255; k++)
			{
				Player player = Main.player[k];
				if (!player.active)
				{
					continue;
				}
				if(AeroWorld.downedCrystalTumbler)
                {
					return true;
                }
			}
			return false;
		}

		public override string TownNPCName()
		{
			switch (WorldGen.genRand.Next(4))
			{
				case 0:
					return "Roxxane";
				case 1:
					return "Myne Err";
				case 2:
					return "S'tony";
				case 3:
					return "Dwayne";
				case 4:
					return "Rockgomery";
				case 5:
					return "Geolbert";
				case 6:
					return "Stephing Stone";
				case 7:
					return "Steve Ege";
				default:
					return "John Dygg";
			}
		}

		public override void FindFrame(int frameHeight)
		{
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat()
		{
			int Angler = NPC.FindFirstNPC(NPCID.Angler);
			if (Angler >= 0 && Main.rand.NextBool(4))
			{
				return "Hate that " + Main.npc[Angler].GivenName + " guy. Imagine thinking fish rock. Pff.";
			}
			switch (Main.rand.Next(4))
			{
				case 0:
					return "Y�know, that Phantic stuff is great if you wanna get stoned.";
				case 1:
					return "Is that a new outfit? I dig it.";
				case 2:
					{
						Main.npcChatCornerItem = ModContent.ItemType<OreQuest>();
						return $"If you find a [i:{ModContent.ItemType<CopperCluster>()}], [i:{ModContent.ItemType<TinCluster>()}], [i:{ModContent.ItemType<IronCluster>()}], [i:{ModContent.ItemType<LeadCluster>()}], [i:{ModContent.ItemType<SilverCluster>()}], [i:{ModContent.ItemType<TungstenCluster>()}], [i:{ModContent.ItemType<PlatinumCluster>()}] or [i:{ModContent.ItemType<GoldCluster>()}] I'll reward you with something!";
					}
				
				case 3:
                    {
						return "Don�t waste time mining diamonds. Platinum�s where it�s at.";

					}
				default:
					return "Hooks? Pickaxes? Cool mining accessories? You want it? It's yours my friend, as long as you have enough ores.";
			}
		}
		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Become Bitchass";
			if (Main.LocalPlayer.HasItem(ModContent.ItemType<CopperCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<TinCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<IronCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<LeadCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<SilverCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<TungstenCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<PlatinumCluster>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<GoldCluster>()))
				button = "Upgrade " + Lang.GetItemNameValue(ItemID.HiveBackpack);
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			int[] itemsToReceive = new int[] { ModContent.ItemType<ReinforcedPlatinumGrapple>(), ModContent.ItemType<MiningSack>(), ModContent.ItemType<AmuletOfGlory>() };

			int[] searchForItems = {
		ModContent.ItemType<CopperCluster>(), ModContent.ItemType<TinCluster>(),
		ModContent.ItemType<IronCluster>(), ModContent.ItemType<LeadCluster>(),
		ModContent.ItemType<SilverCluster>(), ModContent.ItemType<TungstenCluster>(),
		ModContent.ItemType<PlatinumCluster>(), ModContent.ItemType<GoldCluster>() };
			if (firstButton)
			{

				int selectedIndex = -1;
				for (int i = 0; i < Main.LocalPlayer.inventory.Length; ++i)
				{
					if (Main.LocalPlayer.inventory[i].IsAir)
					{
						continue;
					}

					if (searchForItems.Contains(Main.LocalPlayer.inventory[i].type))
					{
						selectedIndex = i;
						break;
					}
				}

				if (selectedIndex != -1)
				{
					Main.LocalPlayer.inventory[selectedIndex].TurnToAir();
					int itemToReceive = itemsToReceive[Main.rand.Next(itemsToReceive.Length)];
					Main.PlaySound(SoundID.Item37); // Reforge/Anvil sound
					Main.npcChatText = $"I took you for granite. I'm so sorry... Here. Have a {Lang.GetItemNameValue(itemToReceive)}";

					Main.LocalPlayer.QuickSpawnItem(itemToReceive);
				}

				shop = true;
			}
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM1>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM2>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM3>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM4>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM5>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM6>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM7>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<HM8>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<PhanticClaws>());
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<PhanticSword>());
			nextSlot++;
		}

		public override void NPCLoot()
		{
			Item.NewItem(npc.getRect(), ModContent.ItemType<PhanticSword>());
		}

		public override bool CanGoToStatue(bool toKingStatue)
		{
			return true;
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ModContent.ProjectileType<RockPouchProj>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 12f;
			randomOffset = 2f;
		}
	}
}