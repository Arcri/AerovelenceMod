using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Common.Globals.Players
{
    public class DebuffWorkOnEnemies : GlobalBuff
    {
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type == BuffID.Electrified)
            {
                if (npc.velocity != new Vector2(0, 0))
                    npc.lifeRegen -= 60; //Original = 32
                else
                    npc.lifeRegen -= 15; //Original = 8
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.3f, 0.8f, 1.1f);
                Dust Dust1 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 226, 0f, 0f, 100, default, 0.5f);
                Dust1.velocity *= 1.6f;
                Dust Dust2 = Dust1;
                Dust2.velocity.Y -= 1f;
                Dust1.position = Vector2.Lerp(Dust1.position, npc.Center, 0.5f);
            }
        }
    }
}