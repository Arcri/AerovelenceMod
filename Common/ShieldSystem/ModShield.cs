using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.ShieldSystem
{
    public abstract class ModShield : ModItem
    {
        public override bool CloneNewInstances => true;

        /// <summary>The type of the shield.</summary>
        public abstract ShieldTypes ShieldType { get; }

        /// <summary>Data regarding the shield's size, capacity, etc.</summary>
        public abstract ShieldData BaseShieldData { get; }

        public ShieldData RealData => BaseShieldData + variableData;

        public ShieldData variableData = new ShieldData(0, 0, 0, 0, 0);
        public int capacity = 0;
        public int prefix = 0;

        internal int delayTimer = 0;
        internal float rechargeTimer = 0;

        private bool _fullRecharge = true;

        public override void AutoStaticDefaults()
        {
            base.AutoStaticDefaults(); //Base defaults for texture loading & name

            Tooltip.SetDefault($"[c/63B4B8:{ShieldType} Shield]");
        }

        public sealed override void SetDefaults()
        {
            Item.accessory = true;

            SafeSetDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ShieldInfo0", $"{RealData.Capacity} HP capacity " + GetBonus(variableData.Capacity, false)));
            tooltips.Add(new TooltipLine(Mod, "ShieldInfo1", $"{RealData.Delay} second delay " + GetBonus(variableData.Delay, true)));
            tooltips.Add(new TooltipLine(Mod, "ShieldInfo2", $"{RealData.RechargeRate} second recharge rate " + GetBonus(variableData.RechargeRate, true)));
            tooltips.Add(new TooltipLine(Mod, "ShieldInfo3", $"{RealData.HPPenalty} HP burden " + GetBonus(variableData.HPPenalty, true)));
            tooltips.Add(new TooltipLine(Mod, "ShieldInfo4", $"{RealData.Radius / 16d} tile radius " + GetBonus((float)(variableData.Radius / 16d), false)));
        }

        private string GetBonus(float variedVal, bool negativeDesired)
        {
            if (variedVal == 0) //No change
                return "";
            else
            {
                if (negativeDesired)
                    return Math.Sign(variedVal) == -1 ? $"[c/20942F:({variedVal})]" : $"[c/C5253F:(+{variedVal})]";
                else
                    return Math.Sign(variedVal) == 1 ? $"[c/20942F:(+{variedVal})]" : $"[c/C5253F:({variedVal})]";
            }
        }

        /// <summary>Allows you to override how a specific shield is affected by a prefix in case of edge cases.</summary>
        /// <param name="boost">Level of the prefix; 1-4.</param>
        /// <param name="type">Type of the prefix; refer to <see cref="ShieldPrefixType"/>.</param>
        /// <param name="sign">Whether this is a buff (1) or debuff (-1).</param>
        public virtual void ApplyShieldPrefix(int boost, byte type, int sign)
        {
            if (type < 8)
                variableData.Capacity = (int)(BaseShieldData.Capacity / 50f * boost);
            else if (type < 16)
                variableData.RechargeRate = -boost;
            else if (type < 24)
                variableData.Delay = -boost;
            else if (type < 32)
            {
                if (sign == 1)
                    variableData.Radius = (int)(BaseShieldData.Radius * boost * 0.2f);
                else
                    variableData.Radius = (int)(BaseShieldData.Radius * boost * 0.15f);
            }
        }

        internal bool PlayerShieldEquipped()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                if (!otherAccessory.IsAir && !Item.IsTheSameAs(otherAccessory) && otherAccessory.modItem is ModShield)
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

            if (delayTimer > RealData.Delay * 60)
            {
                rechargeTimer++;
                if (rechargeTimer > RealData.RechargeRate * 60)
                    rechargeTimer = RealData.RechargeRate * 60;

                capacity = (int)MathHelper.Lerp(0, RealData.Capacity, rechargeTimer / (RealData.RechargeRate * 60));

                if (!_fullRecharge && capacity >= RealData.Capacity)
                {
                    _fullRecharge = true;
                    OnFullRecharge(player);
                }
            }

            player.GetModPlayer<ShieldPlayer>().wornShieldItem = item; //Set the player's shield to this shield

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
            else if (ShieldType == ShieldTypes.Impact || ShieldType == ShieldTypes.Nova) //Kill projectile, do nothing else
                projectile.Kill();
        }

        /// <summary>Determines by how much the capacity depletes per projectile interaction.</summary>
        /// <param name="player">Player wearing the shield.</param>
        /// <param name="projectile">Projectile that was interacted with.</param>
        public virtual int GetCapacityDeplete(Player player, Projectile projectile) => projectile.damage;

        /// <summary>Runs when the shield loses all capacity.</summary>
        /// <param name="player">Player wearing the shield.</param>
        /// <param name="projectile">Projectile that was interacted with.</param>
        public virtual void OnBreak(Player player, Projectile projectile)
        {
        }

        /// <summary>Runs when the shield regains all capacity.</summary>
        /// <param name="player">Player wearing the shield.</param>
        public virtual void OnFullRecharge(Player player)
        {
        }
    }
}
