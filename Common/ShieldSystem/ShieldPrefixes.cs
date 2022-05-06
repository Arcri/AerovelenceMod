using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace AerovelenceMod.Common.ShieldSystem 
{
	public class ShieldPrefixes : ModPrefix
	{
		private readonly byte _value;
		private readonly int _sign;
		private readonly byte _type;

        public override PrefixCategory Category => PrefixCategory.Accessory;

        internal static List<byte> shieldPrefixes;

        public override bool CanRoll(Item item) => item.modItem is ModShield;
        public override float RollChance(Item item) => 5f;

		public ShieldPrefixes() { }

		public ShieldPrefixes(byte power, int sign, byte type)
		{
			_value = power;
			_sign = sign;
			_type = type;
		}

		public override bool Autoload(ref string name)
		{
			if (base.Autoload(ref name))
			{
				shieldPrefixes = new List<byte>();
				foreach (ShieldPrefixType prefix in Enum.GetValues(typeof(ShieldPrefixType)))
				{
					Mod.AddPrefix(prefix.ToString(), new ShieldPrefixes((byte)((int)prefix % 4 + 1), (int)prefix % 8 < 4 ? -1 : 1, (byte)prefix));
					shieldPrefixes.Add(Mod.GetPrefix(prefix.ToString()).Type);
				}
			}
			return false;
		}

		public override void ModifyValue(ref float valueMult)
		{
			float multiplier = 1f + 0.05f * _value * _sign;
			valueMult *= multiplier;
		}

		public override void Apply(Item item)
		{
			if (item.modItem is ModShield shield) 
			{
				int boost = _value * _sign;

				shield.prefix = _value;
				shield.variableData.Capacity = 0;
				shield.variableData.RechargeRate = 0;
				shield.variableData.Delay = 0;
				shield.variableData.Radius = 0;

                shield.ApplyShieldPrefix(boost, _type, _sign);
			}
		}
    }

	public enum ShieldPrefixType : byte
	{
		Timid,//CapacityTierMinus1, 
		Frail,//CapacityTierMinus2, 
		Weak,//CapacityTierMinus3, 
		Broken,//CapacityTierMinus4,

		Sturdy,//CapacityTierPlus1, 
		Hardened,//CapacityTierPlus2, 
		Strong,//CapacityTierPlus3, 
		Empowered,//CapacityTierPlus4,

		Tarnished,//RechargeRateTierMinus1, 
		Reduced,//RechargeRateTierMinus2, 
		Shattered,//RechargeRateTierMinus3, 
		Neutralized,//RechargeRateTierMinus4,

		Grounded,//RechargeRateTierPlus1, 
		Polarized,//RechargeRateTierPlus2, 
		Improved,//RechargeRateTierPlus3, 
		Maximal,//RechargeRateTierPlus4,

		Slowed,//RechargeDelayTierMinus1, 
		Delayed,//RechargeDelayTierMinus2, 
		Crawling,//RechargeDelayTierMinus3, 
		Miserable,//RechargeDelayTierMinus4,

		Boosted,//RechargeDelayTierPlus1,
		Speeding,//RechargeDelayTierPlus2,
		Charged,//RechargeDelayTierPlus3,
		Overdriven,//RechargeDelayTierPlus4,

        Small,//SizeTierMinus1
        Tiny,//SizeTierMinus2
        Puny,//SizeTierMinus3
        Miniscule,//SizeTierMinus4

        Bulky,//SizeTierPlus1, 
        Large,//SizeTierPlus2, 
        Giant,//SizeTierPlus3
        Massive,//SizeTierPlus3
	}
}