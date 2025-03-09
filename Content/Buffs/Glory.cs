using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs
{
    public class Glory : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.2f;
            player.pickSpeed -= 0.1f;
        }
    }
}