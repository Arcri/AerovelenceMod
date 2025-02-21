using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.IL
{
	public class CthulhuShieldBonk : ModSystem
	{
		public override void Load()
		{
			IL_Player.DashMovement += IL_DashMovement;
		}
		public override void Unload()
		{
			IL_Player.DashMovement -= IL_DashMovement;
		}

		private void IL_DashMovement(ILContext context)
		{
			void Error(string message)
			{
				ModContent.GetInstance<AerovelenceMod>().Logger.Error(message);
			}

			ILCursor cursor = new ILCursor(context);

			ILLabel? IL_022D = null;
			if (!cursor.TryGotoNext(MoveType.Before,
			i => i.MatchLdloc(2), //Main.npc[i]
			i => i.MatchLdfld<Entity>("active"), //npc.active
			i => i.MatchBrfalse(out IL_022D), //if(!npc.active)...
			i => i.MatchLdloc(2), //Main.npc[i]
			i => i.MatchLdfld<NPC>("dontTakeDamage"), //npc.dontTakeDamage
			i => i.MatchBrtrue(out _))) //if(npc.dontTakeDamage)...
			{
				//Did not match. Don't try to apply edit
				Error($"Couldn't match IL Patch: {context.Method.Name} @ {cursor.Index}");
				return;
			}
			if (IL_022D == null)
			{
				//For some reason it matched, but the label it should jump to doesn't exist
				Error($"IL Label {nameof(IL_022D)} not found in IL Patch {context.Method.Name} @ {cursor.Index}");
				return;
			}

			cursor.EmitLdarg(0); //Player
			cursor.EmitLdloc(2); //Main.npc[i]
			cursor.EmitDelegate((Player player, NPC npc) => { //bool A(Player, NPC)
				if (npc.type == ModContent.NPCType<Content.NPCs.Bosses.CrystalTumbler.CrystalTumbler2>())
					return false;
				if (npc.type == Terraria.ID.NPCID.Deerclops)
				{
					//I took a guess at this one, I'm not gonna test it either.
					if (player.position.Y > npc.Center.Y)
						return false;
				}
				return true;
			});

			cursor.EmitBrfalse(IL_022D);
		}
	}
}
