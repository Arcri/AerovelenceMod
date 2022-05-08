using AerovelenceMod.Content.Items.Accessories.AerovelenceMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class EnergyShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("This is a modded accessory." +
				"\nDouble tap in any cardinal direction to do a dash!");
		}
		public override void SetDefaults()
		{
			Item.defense = 2;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 50;
			Item.width = 34;
			Item.height = 38;
			Item.knockBack = 9f;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 60);
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			EnergyShieldPlayer mp = player.GetModPlayer<EnergyShieldPlayer>();

			if (!mp.DashActive)
				return;
			Rectangle rectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5 - 4.0), (int)(player.position.Y + player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && player.eocHit != i)
				{
					NPC nPC = Main.npc[i];
					Rectangle rect = nPC.getRect();
					if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
					{
						float num = 30f * player.GetDamage(DamageClass.Melee).Multiplicative;
						float num2 = 9f;
						bool crit = false;
						if (player.kbGlove)
						{
							num2 *= 2f;
						}
						if (player.kbBuff)
						{
							num2 *= 1.5f;
						}
						if (Main.rand.Next(100) < player.GetCritChance(DamageClass.Melee))
						{
							crit = true;
						}
						int num3 = player.direction;
						if (player.whoAmI == Main.myPlayer)
						{
							player.ApplyDamageToNPC(nPC, (int)num, num2, num3, crit);
							nPC.AddBuff(BuffID.ShadowFlame, 360);
						}
						player.eocDash = 10;
						player.dashDelay = 30;
						player.velocity.X = -num3 * 9;
						player.velocity.Y = -4f;
						player.immune = true;
						player.immuneNoBlink = true;
						player.immuneTime = 4;
						player.eocHit = i;
					}
				}
			}
			player.eocDash = mp.DashTimer;
			player.armorEffectDrawShadowEOCShield = true;
			if (mp.DashTimer == EnergyShieldPlayer.MAX_DASH_TIMER) //activates at the start of the dash
			{
				Vector2 newVelocity = player.velocity;

				if ((mp.DashDir == EnergyShieldPlayer.DashUp && player.velocity.Y > -mp.DashVelocity) || (mp.DashDir == EnergyShieldPlayer.DashDown && player.velocity.Y < mp.DashVelocity))
				{
					float dashDirection = mp.DashDir == EnergyShieldPlayer.DashDown ? 1f : -1.3f;
					newVelocity.Y = dashDirection * mp.DashVelocity;
				}
				else if ((mp.DashDir == EnergyShieldPlayer.DashLeft && player.velocity.X > -mp.DashVelocity) || (mp.DashDir == EnergyShieldPlayer.DashRight && player.velocity.X < mp.DashVelocity))
				{
					float dashDirection = mp.DashDir == EnergyShieldPlayer.DashRight ? 1f : -1f;
					newVelocity.X = dashDirection * mp.DashVelocity;
				}
				player.velocity = newVelocity;
			}

			Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, 32), player.width, player.height - 32, DustID.Shadowflame, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default, 1.9f);
			dust.noGravity = true;
			dust.velocity *= 0.4f;
			dust.scale *= 1.2f;
			if (mp.DashDir == EnergyShieldPlayer.DashUp || mp.DashDir == EnergyShieldPlayer.DashDown)
				player.maxFallSpeed += 6;
			if (mp.DashDir == EnergyShieldPlayer.DashUp) //make player fall faster after upwards dash to prevent infinite upwards dashes
			{
				player.gravity += 0.36f;
			}
			mp.DashTimer--;
			mp.DashDelay--;
			if (mp.DashDelay == 0)
			{
				mp.DashDelay = EnergyShieldPlayer.MAX_DASH_DELAY;
				mp.DashTimer = EnergyShieldPlayer.MAX_DASH_TIMER;
				mp.DashActive = false;
				player.eocHit = -1;
			}
		}
	}
	namespace AerovelenceMod.Items.Accessories
	{
		public class EnergyShieldPlayer : ModPlayer
		{
			public static readonly int DashDown = 0;
			public static readonly int DashUp = 1;
			public static readonly int DashRight = 2;
			public static readonly int DashLeft = 3;

			public int DashDir = -1;

			public bool DashActive = false;
			public int DashDelay = MAX_DASH_DELAY;
			public int DashTimer = MAX_DASH_TIMER;

			public readonly float DashVelocity = 14f;
			public static readonly int MAX_DASH_DELAY = 50;
			public static readonly int MAX_DASH_TIMER = 35;
			public override void ResetEffects()
			{
				bool dashAccessoryEquipped = false;
				for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
				{
					Item item = Player.armor[i];

					if (item.type == ModContent.ItemType<EnergyShield>())
						dashAccessoryEquipped = true;
					else if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
						return;
				}

				if (!dashAccessoryEquipped || Player.setSolar || Player.mount.Active || DashActive)
					return;

				if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
					DashDir = DashDown;
				else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
					DashDir = DashUp;
				else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
					DashDir = DashRight;
				else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
					DashDir = DashLeft;
				else
					return;

				DashActive = true;

				//Here you'd be able to set an effect that happens when the dash first activates
				//Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
			}
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
