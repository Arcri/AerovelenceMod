using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace AerovelenceMod.Common.IL
{
	public static class GemGrapplingRange
	{
		public static void Load()
		{
			global::IL.Terraria.Projectile.VanillaAI += VanillaAI_GrapplingHookRange;
		}
        private static void VanillaAI_GrapplingHookRange(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchStloc(127)))
            {
                return;
            }

            // Load argument 0 on the stack, this is the current Projectile object.
            c.Emit(OpCodes.Ldarg_0);
            // Load the original calculated grappling hook range onto the stack.
            c.Emit(OpCodes.Ldloc, 127);

            // Insert a custom delegate, which accepts a Projectile and int as parameters,
            // And returns an int as well.
            c.EmitDelegate<Func<Projectile, int, int>>((projectile, originalValue) =>
            {
                AeroPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<AeroPlayer>();

                //if (modPlayer.UpgradedHooks)
                //{
                    //return (originalValue + 200);
                //}

                return (originalValue);
            });

            // Set the local variable number 127 (AKA the grappling hook range variable) to the value we just returned from the above Func/delegate ^
            c.Emit(OpCodes.Stloc, 127);
        }
    }
}
