using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;

namespace AerovelenceMod.Common
{
    public static class ModDetours
    {
        public static void Load()
        {
            Terraria.On_Player.Update_NPCCollision += CustomCollision.Player_UpdateNPCCollision;
            Terraria.On_Player.SlopingCollision += CustomCollision.Player_PlatformCollision;
        }

        public static void Unload()
        {
            Terraria.On_Player.Update_NPCCollision -= CustomCollision.Player_UpdateNPCCollision;
            Terraria.On_Player.SlopingCollision -= CustomCollision.Player_PlatformCollision;
        }
    }
}