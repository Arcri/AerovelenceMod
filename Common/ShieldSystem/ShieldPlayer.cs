using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.ShieldSystem
{
    public class ShieldPlayer : ModPlayer
    {
        public ModShield WornShield => wornShieldItem.modItem as ModShield;

        public Item wornShieldItem = null;

        public override void ResetEffects()
        {
            wornShieldItem = null;
        }

        public override void PostUpdateEquips()
        {
            if (wornShieldItem != null && WornShield.capacity > 0)
            {
                player.statLifeMax2 -= WornShield.RealData.HPPenalty;

                ReflectProjectiles();
            }
        }

        /// <summary>Handles shield/</summary>
        private void ReflectProjectiles()
        {
            for (int i = 0; i < Main.maxProjectiles; ++i)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.hostile && proj.DistanceSQ(player.Center) < WornShield.RealData.Radius * WornShield.RealData.Radius)
                {
                    if (proj.GetGlobalProjectile<ShieldGlobalProjectile>().shieldImmune[player.whoAmI] <= 0 && WornShield.CanEffectProjectile(player, proj))
                    {
                        WornShield.OnEffectProjectile(player, proj);

                        WornShield.capacity -= WornShield.GetCapacityDeplete(player, proj);
                        WornShield.rechargeTimer = WornShield.delayTimer = 0;

                        if (WornShield.capacity <= 0)
                            WornShield.OnBreak(player, proj);
                    }

                    if (proj.active)
                        proj.GetGlobalProjectile<ShieldGlobalProjectile>().shieldImmune[player.whoAmI] = GetProjectileShieldIFrames();
                }
            }
        }

        private int GetProjectileShieldIFrames()
        {
            if (WornShield.ShieldType == ShieldTypes.Bubble)
                return 30;
            else if (WornShield.ShieldType == ShieldTypes.Impact)
                return 5 * 60;
            else if (WornShield.ShieldType == ShieldTypes.Nova)
                return 60;
            return 0;
        }
    }
}
