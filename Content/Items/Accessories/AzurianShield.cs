using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
	public class AzurianShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Azurian Shield");
			Tooltip.SetDefault("Increases life by 15\nImmune to fire\n'An ancient shield used by the Citadel's top warriors'");
		}
		public override void SetDefaults()
		{
			Item.defense = 3;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 38;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 60);
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 15;
			player.buffImmune[156] = true;
			player.buffImmune[24] = true;
		}
	}
}




/*using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class EnergyShield : ModItem
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Energy Shield");
			Tooltip.SetDefault("Grants immunity to most flame debuffs\n+15 max life and increased life regen\nGrants a Shadowflame dash\nExpert");
		}
		public override void SetDefaults()
		{
			item.accessory = true;
			item.defense = 6;
			item.width = 32;
			item.height = 32;
			item.value = Item.sellPrice(0, 5, 0, 0);
			item.rare = -12;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
			target.AddBuff(BuffID.ShadowFlame, 300);
		}

		public override void OnHitPvp(Player player, Player target, int damage, bool crit)
		{
			player.AddBuff(BuffID.ShadowFlame, 300);

		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.dash = 2;
			player.statLifeMax2 += 15;
			player.buffImmune[44] = true;
			player.buffImmune[24] = true;
			player.lifeRegen += 2;
		}
	}
}*/
