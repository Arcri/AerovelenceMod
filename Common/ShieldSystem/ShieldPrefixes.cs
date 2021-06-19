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

		internal static List<byte> shieldPrefixes;
		public override bool CanRoll(Item item)
		{
			if (item.modItem != null && item.modItem.GetType().IsSubclassOf(typeof(ShieldItem)))
				return true;
			return false;
		}
		public override float RollChance(Item item) => 5f;
		public override PrefixCategory Category => PrefixCategory.Accessory;
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
					mod.AddPrefix(prefix.ToString(), new ShieldPrefixes((byte)((int)prefix % 4 + 1), (int)prefix % 8 < 4 ? -1 : +1, (byte)prefix));
					shieldPrefixes.Add(mod.GetPrefix(prefix.ToString()).Type);
				}
			}
			return false;
		}
		public override void ModifyValue(ref float valueMult)
		{
			float multiplier = 1f + 0.05f * _value*_sign;
			valueMult *= multiplier;
		}
		public override void Apply(Item item)
		{
			if (ShieldItem.isItemShield(item, out Shield shield)) 
			{
				int boost = _value * _sign;
				item.GetGlobalItem<Shield>().prefix = _value;
				item.GetGlobalItem<Shield>().prefixSign = _sign;
				shield.capacityBoost = 0;
				shield.rechargeRateBoost = 0;
				shield.rechargeDelayBoost = 0;
				shield.sizeBoost = 0;

				if (_type < 8)
					shield.capacityBoost = (int)(shield.capacity / 100) * boost;

				else if (_type < 16)
					shield.rechargeRateBoost = boost;

				else if (_type < 24)
					shield.rechargeDelayBoost = boost;

				else if (_type < 32)
					shield.sizeBoost = (shield.size / 20) * boost;
			}
		}
    }
	public enum ShieldPrefixType : byte
	{
		capacityTierMinus1, 
		capacityTierMinus2, 
		capacityTierMinus3, 
		capacityTierMinus4,

		capacityTierPlus1, 
		capacityTierPlus2, 
		capacityTierPlus3, 
		capacityTierPlus4,

		rechargeRateTierMinus1, 
		rechargeRateTierMinus2, 
		rechargeRateTierMinus3, 
		rechargeRateTierMinus4,

		rechargeRateTierPlus1, 
		rechargeRateTierPlus2, 
		rechargeRateTierPlus3, 
		rechargeRateTierPlus4,

		rechargeDelayTierMinus1, 
		rechargeDelayTierMinus2, 
		rechargeDelayTierMinus3, 
		rechargeDelayTierMinus4,

		rechargeDelayTierPlus1,
		rechargeDelayTierPlus2,
		rechargeDelayTierPlus3,
		rechargeDelayTierPlus4,

		sizeTierMinus1, 
		sizeTierMinus2, 
		sizeTierMinus3, 
		sizeTierMinus4,

		sizeTierPlus1, 
		sizeTierPlus2, 
		sizeTierPlus3, 
		sizeTierPlus4
	}
}