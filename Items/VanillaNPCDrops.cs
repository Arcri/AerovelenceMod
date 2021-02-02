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
			if (npc.type == NPCID.Probe)
			{
				if (Main.rand.Next(1001) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
				}
			}
			if (npc.type == NPCID.TheDestroyer)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
				}
			}
			if (npc.type == NPCID.SkeletronPrime)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
				}
			}
			if (npc.type == NPCID.PrimeLaser)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
				}
			}
			if (npc.type == NPCID.Retinazer)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("ClockworkLazinator"));
				}
			}
			if (npc.type == NPCID.Plantera)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("CrystalGlade"));
				}
			}
			if (npc.type == NPCID.EyeofCthulhu)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("CthulhusWrath"));
				}
			}
			if (npc.type == NPCID.KingSlime)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("SlimyKnife"));
				}
			}
			if (npc.type == NPCID.QueenBee)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("BookOfBees"));
				}
			}
			if (npc.type == NPCID.SkeletronHead)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("SkullBow"));
				}
			}
			if (npc.type == NPCID.Golem)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("NapalmLauncher"));
				}
			}
			if (npc.type == NPCID.WallofFleshEye)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("VampiricRapier"));
				}
			}
			if (npc.type == NPCID.Bunny)
			{
				if (Main.rand.Next(1001) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("BunnyCannon"));
				}
			}
			if (npc.type == NPCID.PossessedArmor)
			{
				if (Main.rand.Next(251) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("FAMASTER"));
				}
			}
			if (npc.type == mod.NPCType("LuminousDefender"))
			{
				if (Main.rand.Next(6) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("LustrousCrystal"));
				}
			}


			if (npc.type == NPCID.IceSlime || npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.IceBat || npc.type == NPCID.UndeadViking || npc.type == NPCID.CyanBeetle)
			{
				if (Main.rand.Next(16) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("FrostShard"));
				}
			}


			if (npc.type == NPCID.WalkingAntlion)
			{
				if (Main.rand.Next(26) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("MandiBlaster"));
				}
			}
			if (npc.type == NPCID.BlueSlime)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("SlimyGreatsword"));
				}
			}
			if (npc.type == NPCID.Poltergeist)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("Miasmi"));
				}
			}
			if (npc.type == NPCID.Hellbat)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("LightOfTheAncients"));
				}
			}
			if (npc.type == NPCID.FireImp)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("FlameShot"));
				}
			}
			if (npc.type == NPCID.TacticalSkeleton)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("ConfrenceCall"));
				}
			}
			if (npc.type == NPCID.ChaosElemental)
			{
				if (Main.rand.Next(51) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("Exodious"));
				}
			}
			if (npc.type == NPCID.Hornet)
			{
				if (Main.rand.Next(26) == 0)
				{
					Item.NewItem(npc.getRect(), mod.ItemType("Stinger"));
				}
			}
		}
	}
}