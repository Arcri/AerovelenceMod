using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using AerovelenceMod.Items.Placeable.Ores;
using AerovelenceMod.Items.Weapons.Melee;
using AerovelenceMod.Items.Weapons.Ranged;
using AerovelenceMod.NPCs.Cave;
using AerovelenceMod.NPCs.CrystalCaverns;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items
{
	public class VanillaNPCDrops : GlobalNPC
	{
		public override void NPCLoot(NPC npc)
		{
			//TODO use a switch
			switch (npc.type)
			{
				case NPCID.Probe:
					if (Main.rand.NextBool(1001))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
					}
					break;
				case NPCID.TheDestroyer:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
					}
					break;
				case NPCID.SkeletronPrime:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
					}
					break;
				case NPCID.PrimeLaser:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
					}
					break;
				case NPCID.Retinazer:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("ClockworkLazinator"));
					}
					break;
				case NPCID.Plantera:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("CrystalGlade"));
					}
					break;
				case NPCID.EyeofCthulhu:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("CthulhusWrath"));
					}
					break;
				case NPCID.KingSlime:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyKnife"));
					}
					break;
				case NPCID.QueenBee:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyKnife"));
					}
					break;
				case NPCID.SkeletronHead:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SkullBow"));
					}
					break;
				case NPCID.Golem:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("NapalmLauncher"));
					}
					break;
				case NPCID.WallofFleshEye:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("VampiricRapier"));
					}
					break;
				case NPCID.Bunny:
					if (Main.rand.NextBool(1001))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("BunnyCannon"));
					}
					break;
				case NPCID.PossessedArmor:
					if (Main.rand.NextBool(251))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("FAMASTER"));
					}
					break;
				case NPCID.WalkingAntlion:
					if (Main.rand.NextBool(26))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("MandiBlaster"));
					}
					break;
				case NPCID.BlueSlime:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyGreatsword"));
					}
					break;
				case NPCID.Poltergeist:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("Miasmi"));
					}
					break;
				case NPCID.Hellbat:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LightOfTheAncients"));
					}
					break;
				case NPCID.FireImp:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("FlameShot"));
					}
					break;
				case NPCID.TacticalSkeleton:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("ConfrenceCall"));
					}
					break;
				case NPCID.Hornet:
					if (Main.rand.NextBool(26))
					{
						Item.NewItem(npc.getRect(), ModContent.ItemType<Stinger>());
					}
					break;
				case NPCID.ChaosElemental:
					if (Main.rand.NextBool(51))
					{
						Item.NewItem(npc.getRect(), mod.ItemType("Exodious"));
					}
					break;
				case NPCID.IceSlime:
				case NPCID.SpikedIceSlime:
				case NPCID.IceBat:
				case NPCID.UndeadViking:
				case NPCID.CyanBeetle:
					if (Main.rand.NextBool(16))
					{
						Item.NewItem(npc.getRect(), ModContent.ItemType<FrostShard>());
					}
					break;

			}
			if (AeroWorld.downedSnowrium)
			{
				if (npc.type == NPCID.IceSlime || npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.IceBat || npc.type == NPCID.UndeadViking || npc.type == NPCID.CyanBeetle)
				{
					if (Main.rand.Next(21) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("KelvinCore"));
					}
				}
			}
			if (npc.type == ModContent.NPCType<LuminousDefender>())
			{
				if (Main.rand.NextBool(6))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<LustrousCrystal>());
				}
			}
			if (npc.type == ModContent.NPCType<LuminousDefender>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(3, 10));
				}
			}
			if (npc.type == ModContent.NPCType<LivingBoulder>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<SlateOreItem>(), Main.rand.Next(1, 6));
				}
			}
			if (npc.type == ModContent.NPCType<CaveSnail>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<SlateOreItem>(), Main.rand.Next(1, 6));
				}
			}


			if (npc.type == ModContent.NPCType<Tumblerock1>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(1, 5));
				}
			}
			if (npc.type == ModContent.NPCType<Tumblerock2>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(1, 5));
				}
			}
			if (npc.type == ModContent.NPCType<Tumblerock3>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(1, 5));
				}
			}
			if (npc.type == ModContent.NPCType<Tumblerock4>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(1, 5));
				}
			}

			if (npc.type == ModContent.NPCType<CrystalSlime>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(2, 6));
				}
			}
			if (npc.type == ModContent.NPCType<CrystalWormHead>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(2, 8));
				}
			}
			if (npc.type == ModContent.NPCType<OvergrownTumblerock>())
			{
				if (Main.rand.NextBool(5))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<Crystallizer>());
				}
			}
			if (npc.type == ModContent.NPCType<OvergrownTumblerock>())
			{
				if (Main.rand.NextBool(1))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<CavernCrystalItem>(), Main.rand.Next(5, 15));
				}
			}

		}
	}
}