using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.IL
{
	//Change from static class to ILoadable just so load/unload is handled here
	public class GemGrapplingRange : ILoadable
	{
		public void Load(Mod mod)
		{
			IL_Projectile.AI_007_GrapplingHooks += IL_HookRange;
		}
		public void Unload()
		{
			IL_Projectile.AI_007_GrapplingHooks -= IL_HookRange;
		}

		//==========
		// REPLACE THIS WITH MODPLAYER THAT HAS ACCESSORY BOOL OR SOMETHING
		private const bool ExtendGemHookRange = true;
		//==========


		private void IL_HookRange(ILContext context)
		{
			void Error(string message)
			{
				ModContent.GetInstance<AerovelenceMod>().Logger.Error(message);
			}

			try
			{
				ILCursor cursor = new ILCursor(context);

				if (!cursor.TryGotoNext(MoveType.Before,
					i => i.MatchLdloc(4), //num3 (Distance(owner, projectile))
					i => i.MatchLdloc(21), //num8 (max Distance)
					i => i.MatchConvR4(), //(float)num8
					i => i.MatchBleUn(out ILLabel _))) //if(num3 < num8)..
				{
					Error($"Couldn't match IL Patch: {context.Method.Name} @ {cursor.Index}");
					MonoModHooks.DumpIL(ModContent.GetInstance<AerovelenceMod>(), context);
					return;
				}

				cursor.Emit(OpCodes.Ldarg_0); //Projectile self
				cursor.EmitDelegate((Projectile projectile) => {
					//Replace with 'Main.player[projectile.owner].GetModPlayer<T>().upgradedHooks'
					return ExtendGemHookRange;
				});
				ILLabel conditionalJump = cursor.MarkLabel();
				cursor.Emit(OpCodes.Ldc_R4, 1.33f); //Push 1.33f to stack
				cursor.Emit(OpCodes.Ldloc, 21); //num8
				cursor.Emit(OpCodes.Conv_R4); //(float)num8
				cursor.Emit(OpCodes.Mul); //num8 * 1.33f
				cursor.Emit(OpCodes.Conv_I4); //(int)num8 * 1.33
				cursor.Emit(OpCodes.Stloc, 21); //push result to stack
				ILLabel orig_DistanceCheck = cursor.MarkLabel();
				cursor.GotoLabel(conditionalJump);
				cursor.EmitBrfalse(orig_DistanceCheck); //if(!ExtendHookRange)
			}
			catch (Exception x)
			{
				Error(x.Message);
				return;
			}
		}
	}
}
