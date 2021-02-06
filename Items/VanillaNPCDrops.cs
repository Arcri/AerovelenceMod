using AerovelenceMod.Items.Others.Crafting;
using AerovelenceMod.Items.Weapons.Ranged;
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
					if (Main.rand.Next(1001) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
					}
					break;
				case NPCID.TheDestroyer:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("PlaroveneBlaster"));
					}
					break;
				case NPCID.SkeletronPrime:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
					}
					break;
				case NPCID.PrimeLaser:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LazX"));
					}
					break;
				case NPCID.Retinazer:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("ClockworkLazinator"));
					}
					break;
				case NPCID.Plantera:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("CrystalGlade"));
					}
					break;
				case NPCID.EyeofCthulhu:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("CthulhusWrath"));
					}
					break;
				case NPCID.KingSlime:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyKnife"));
					}
					break;
				case NPCID.QueenBee:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyKnife"));
					}
					break;
				case NPCID.SkeletronHead:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SkullBow"));
					}
					break;
				case NPCID.Golem:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("NapalmLauncher"));
					}
					break;
				case NPCID.WallofFleshEye:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("VampiricRapier"));
					}
					break;
				case NPCID.Bunny:
					if (Main.rand.Next(1001) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("BunnyCannon"));
					}
					break;
				case NPCID.PossessedArmor:
					if (Main.rand.Next(251) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("FAMASTER"));
					}
					break;
				case NPCID.WalkingAntlion:
					if (Main.rand.Next(26) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("MandiBlaster"));
					}
					break;
				case NPCID.BlueSlime:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("SlimyGreatsword"));
					}
					break;
				case NPCID.Poltergeist:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("Miasmi"));
					}
					break;
				case NPCID.Hellbat:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("LightOfTheAncients"));
					}
					break;
				case NPCID.FireImp:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("FlameShot"));
					}
					break;
				case NPCID.TacticalSkeleton:
					if (Main.rand.Next(51) == 0)
					{
						Item.NewItem(npc.getRect(), mod.ItemType("ConfrenceCall"));
					}
					break;
				case NPCID.Hornet:
					if (Main.rand.Next(26) == 0)
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
			if (npc.type == ModContent.NPCType<LuminousDefender>())
			{
				if (Main.rand.NextBool(6))
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<LustrousCrystal>());
				}
			}
		}
	}
}