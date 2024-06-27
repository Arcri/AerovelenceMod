using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;

namespace AerovelenceMod.Content.NPCs
{    public abstract class CollideableNPC : ModNPC
    {
        public bool Grappled = false;
    }
}