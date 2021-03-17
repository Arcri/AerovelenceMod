using AerovelenceMod.Blocks.CrystalCaverns.Tiles;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Accessories
{
	[AutoloadEquip(EquipType.Shoes)]
	public class CrystalStompers : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Stompers");
			Tooltip.SetDefault("The wearer can run super fast\nDouble tap down ");
		}
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 20;
			item.value = Item.buyPrice(0, 0, 5, 0);
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.moveSpeed += 0.35f;
			CrystalStompersPlayer mp = player.GetModPlayer<CrystalStompersPlayer>();

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
						float num = 30f * player.meleeDamage;
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
						if (Main.rand.Next(100) < player.meleeCrit)
						{
							crit = true;
						}
						int num3 = player.direction;
						if (player.whoAmI == Main.myPlayer)
						{
							player.ApplyDamageToNPC(nPC, (int)num, num2, num3, crit);
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
			if (mp.DashTimer == CrystalStompersPlayer.MAX_DASH_TIMER) //activates at the start of the dash
			{
				Vector2 newVelocity = player.velocity;

				if ((mp.DashDir == CrystalStompersPlayer.DashDown && player.velocity.Y < mp.DashVelocity))
				{
					float dashDirection = mp.DashDir == CrystalStompersPlayer.DashDown ? 1f : -1.3f;
					newVelocity.Y = dashDirection * mp.DashVelocity;
				}
				player.velocity = newVelocity;
			}

			Dust dust = Dust.NewDustDirect(player.position + new Vector2(0, 32), player.width, player.height - 32, DustID.BlueCrystalShard, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default, 1.9f);
			dust.noGravity = true;
			dust.velocity *= 0.4f;
			dust.scale *= 1.2f;
			mp.DashTimer--;
			mp.DashDelay--;
			if (mp.DashDelay == 0)
			{
				mp.DashDelay = CrystalStompersPlayer.MAX_DASH_DELAY;
				mp.DashTimer = CrystalStompersPlayer.MAX_DASH_TIMER;
				mp.DashActive = false;
				player.eocHit = -1;
			}
		}


		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 15);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}

namespace AerovelenceMod.Items.Accessories
{
	public class CrystalStompersPlayer : ModPlayer
	{
		public static readonly int DashDown = 0;

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
			for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
			{
				Item item = player.armor[i];

				if (item.type == ModContent.ItemType<CrystalStompers>())
					dashAccessoryEquipped = true;
				else if (item.type == ItemID.HermesBoots || item.type == ItemID.FlurryBoots || item.type == ItemID.SailfishBoots || item.type == ItemID.FrostsparkBoots || item.type == ItemID.SpectreBoots)
					return;
			}

			if (!dashAccessoryEquipped || player.setSolar || player.mount.Active || DashActive)
				return;

			if (player.controlDown && player.releaseDown && player.doubleTapCardinalTimer[DashDown] < 15)
				DashDir = DashDown;
			else
				return;

			DashActive = true;

			//Here you'd be able to set an effect that happens when the dash first activates
			//Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
		}
	}
}