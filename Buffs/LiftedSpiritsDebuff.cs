using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Buffs
{
    public class LiftedSpiritsDebuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lifted Spirits");
            Description.SetDefault("'Oh god, a rat'");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;

        }


        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.velocity.Y > -1)
            {
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
            }
            npc.velocity.X *= 0.9f;
            npc.velocity.Y *= 0.9f;
        }

    }
}