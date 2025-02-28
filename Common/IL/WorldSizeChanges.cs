using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.ILEditing
{
    public partial class WorldSizeChanges : ModSystem
    {
        public override void Load()
        {
            IL_UIWorldCreation.AddWorldSizeOptions += SwapSmallDescriptionKey;
        }

        /// <summary>
        /// Changes the small world string to warn players against using small worlds.
        /// </summary>
        private static void SwapSmallDescriptionKey(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(MoveType.After, x => x.MatchLdstr("UI.WorldDescriptionSizeSmall")))
            {
                ModContent.GetInstance<AerovelenceMod>().Logger.Error("Change Small World Description: Could not match string \"UI.WorldDescriptionSizeSmall\".");
                return;
            }
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldstr, "Mods.AerovelenceMod.UI.SmallWorldWarning");
        }
    }
}