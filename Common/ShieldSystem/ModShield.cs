using Microsoft.Xna.Framework;
using System;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.ShieldSystem
{
    public abstract class ModShield : ModItem
    {
        /// <summary>The type of the shield.</summary>
        public abstract ShieldTypes ShieldType { get; }

        /// <summary>How much HP of damage this shield can take before failing.</summary>
        public abstract int MaxCapacity { get; }

        /// <summary>How long it takes for this shield to start regenerating outside of combat in seconds.</summary>
        public abstract float Delay { get; }

        /// <summary>How long it takes this shield to fully regenerate in seconds.</summary>
        public abstract int RechargeRate { get; }

        /// <summary>How much HP this shield takes off of the player.</summary>
        public abstract int HPPenalty { get; }

        /// <summary>Radius of the shield in pixels. Allows for different sizes of shields if necessary.</summary>
        public abstract int Radius { get; }

        public int capacity = 0;

        internal int delayTimer = 0;
        internal float rechargeTimer = 0;

        internal string InformationTooltip() => $"[c/63B4B8:{ShieldType} Shield]\n{Delay} second delay\n{RechargeRate} second recharge rate\n{HPPenalty}hp burden";

        public override void AutoStaticDefaults()
        {
            base.AutoStaticDefaults(); //Base defaults for texture loading & name

            Tooltip.SetDefault(InformationTooltip()); //Base tooltip
        }

        public sealed override void SetDefaults()
        {
            item.accessory = true;

            SafeSetDefaults();
        }

        internal bool PlayerShieldEquipped()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                if (!otherAccessory.IsAir && !item.IsTheSameAs(otherAccessory) && otherAccessory.modItem is ModShield)
                    return false;
            }
            return true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => PlayerShieldEquipped();

        /// <summary>Used to set values apart from accessory and defense.</summary>
        public virtual void SafeSetDefaults() { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            delayTimer++;

            if (delayTimer > Delay * 60)
            {
                rechargeTimer++;
                if (rechargeTimer > RechargeRate * 60)
                    rechargeTimer = RechargeRate * 60;

                capacity = (int)MathHelper.Lerp(0, MaxCapacity, rechargeTimer / (RechargeRate * 60));
            }

            player.GetModPlayer<ShieldPlayer>().wornShield = this; //Set the player's shield to this shield

            SafeUpdateAccessory();
        }

        /// <summary>Used for equip effects outside of the base recharge.</summary>
        public virtual void SafeUpdateAccessory()
        {
            
        }

        internal void BasicReflect(Player player, Projectile projectile) => projectile.velocity = projectile.DirectionFrom(player.Center) * projectile.velocity.Length();

        internal float GetBaseBlockChance()
        {
            switch (ShieldType)
            {
                case ShieldTypes.Bubble:
                    return 1f; //100% chance
                case ShieldTypes.Impact:
                    return 0.5f; //50% chance
                case ShieldTypes.Nova:
                    return 1f; //100% chance; does not block by default
                default:
                    throw new Exception("Why do you have a shield of type Count?");
            }
        }

        /// <summary>If the given player, projectile and current shield can affect the given projectile.</summary>
        /// <param name="player">The player that is wearing the shield.</param>
        /// <param name="projectile">The projectile in question. The projectile MUST be hostile and within Radius pixels of the player.</param>
        public virtual bool CanEffectProjectile(Player player, Projectile projectile) => GetBaseBlockChance() > Main.rand.NextFloat();

        /// <summary>Called when a projectile can be affected.</summary>
        /// <param name="player">Player wearing the shield.</param>
        /// <param name="projectile">Projectile to modify.</param>
        public virtual void OnEffectProjectile(Player player, Projectile projectile)
        {
            if (ShieldType == ShieldTypes.Bubble) //Deflect projectile
                BasicReflect(player, projectile);
            else if (ShieldType == ShieldTypes.Impact) //Kill projectile, do nothing else
                projectile.Kill();
            //No base nova behaviour because I don't know what that'd be
        }

        public virtual int GetCapacityDeplete(Player player, Projectile projectile) => projectile.damage;
    }
}
