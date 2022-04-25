using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.ShieldSystem
{
    public class ShieldGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int[] shieldImmune = new int[Main.maxPlayers];

        public override bool PreAI(Projectile projectile)
        {
            for (int i = 0; i < shieldImmune.Length; ++i)
                shieldImmune[i]--;
            return true;
        }
    }
}
