using System;

using Terraria;

using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace AerovelenceMod.ILHooks
{
	public static class GemGrapplingRange
	{
		public static void Load()
		{
			IL.Terraria.Projectile.VanillaAI += VanillaAI_GrapplingHookRange;
		}

		private static void VanillaAI_GrapplingHookRange(ILContext il)
		{
			var c = new ILCursor(il);

			// Start to look where int 230 is loaded onto the stack.
			// This is the type of one of the gem hook projectiles, which is our first point of reference.
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdcI4(230)))
			{
				return;
			}
			c.MoveAfterLabels();

			// Start to look where local variable number 127 (which is the grappling hook range) is set.
			// We want to move right after this and modify it depending on our projectile/player state.
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchStloc(127)))
			{
				return;
			}

			c.MoveAfterLabels();

			// Load argument 0 on the stack, this is the current Projectile object.
			c.Emit(OpCodes.Ldarg_0);
			// Load the original calculated grappling hook range onto the stack.
			c.Emit(OpCodes.Ldloc, 127);

			// Insert a custom delegate, which accepts a Projectile and int as parameters,
			// And returns an int as well.
			c.EmitDelegate<Func<Projectile, int, int>>((projectile, originalValue) =>
			{
				Player owner = Main.player[projectile.owner];

				if (owner.GetModPlayer<AeroPlayer>().UpgradedHooks)
				{
					return (originalValue + 200);
				}

				return (originalValue);
			});

			// Set the local variable number 127 (AKA the grappling hook range variable) to the value we just returned from the above Func/delegate ^
			c.Emit(OpCodes.Stloc, 127);
		}
	}
}
