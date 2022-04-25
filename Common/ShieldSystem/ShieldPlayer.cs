using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.ShieldSystem
{
    public class ShieldPlayer : ModPlayer
    {
        public ModShield wornShield = null;

        public override void ResetEffects()
        {
            wornShield = null;
        }

        public override void PostUpdateEquips()
        {
            if (wornShield != null && wornShield.capacity > 0)
            {
                player.statLifeMax2 -= wornShield.HPPenalty;

                ReflectProjectiles();
            }
        }

        /// <summary>Handles shield/</summary>
        private void ReflectProjectiles()
        {
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.hostile && proj.DistanceSQ(player.Center) < wornShield.Radius * wornShield.Radius)
                {
                    if (proj.GetGlobalProjectile<ShieldGlobalProjectile>().shieldImmune[player.whoAmI] <= 0 && wornShield.CanEffectProjectile(player, proj))
                    {
                        Main.NewText(wornShield.capacity);

                        wornShield.OnEffectProjectile(player, proj);

                        wornShield.capacity -= wornShield.GetCapacityDeplete(player, proj);
                        wornShield.rechargeTimer = wornShield.delayTimer = 0;

                        Main.NewText(wornShield.capacity);
                    }

                    if (proj.active)
                        proj.GetGlobalProjectile<ShieldGlobalProjectile>().shieldImmune[player.whoAmI] = GetProjectileShieldIFrames();
                }
            }
        }

        private int GetProjectileShieldIFrames()
        {
            if (wornShield.ShieldType == ShieldTypes.Bubble)
                return 30;
            else if (wornShield.ShieldType == ShieldTypes.Impact)
                return 5 * 60;
            else if (wornShield.ShieldType == ShieldTypes.Nova)
                return 60;
            return 0;
        }
    }
}
