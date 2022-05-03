using Terraria.ID;
using Terraria;
using Terraria.GameInput;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.ShieldSystem;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;

namespace AerovelenceMod
{
	public class AeroPlayer : ModPlayer
	{

		public int Shake = 0;

		public bool zooming;

		public bool FishPartner;

		public bool SoulFire;
		public bool MiningAbilityCooldown;
		public bool Electrified;
		public bool badHeal;

		public bool QueensStinger;
		public bool UpgradedHooks;
		public bool EmeraldEmpoweredGem;
		public bool MidasCrown;
		public bool PoweredBattery;

		public bool AdobeHelmet;
		public bool AmbrosiaBonus;
		public bool PhanticMeleeBonus;
		public bool PhanticRangedBonus;
		public bool PhanticMagicBonus;
		public bool FrostMelee;
		public bool FrostProjectile;
		public bool FrostMinion;
		public bool BurnshockArmorBonus;
		public bool SpiritCultistBonus;
		public bool lumberjackSetBonus;

		public int burnshockSetBonusCooldown;
		public readonly int defaultBurnshockSetBonusCooldown = 1200;

		public bool ShiverMinion;
		public bool NeutronMinion;
		public bool StarDrone;
		public bool Minicry;
		public bool charmingBush;
		public bool huntressSummon;

		public bool IsETPBeingLinked;
		public bool TravellingByETP;
		public int ETPDustDelay;
		public int ETPSoundDelay;
		public Vector2 ETPBeingLinkedPosition = new Vector2();
		public Vector2 ETPDestination;

		public bool ShieldOn;
		public bool ShieldBroken;
		public float ShieldCapacity;
		public ShieldTypes ShieldType;
		public override void Initialize()
		{
			zooming = false;

			FishPartner = false;

			AdobeHelmet = false;
			AmbrosiaBonus = false;
			PhanticMeleeBonus = false;
			PhanticRangedBonus = false;
			PhanticMagicBonus = false;
			SpiritCultistBonus = false;
			FrostMelee = false;
			FrostProjectile = false;
			FrostMinion = false;
			BurnshockArmorBonus = false;

			MidasCrown = false;
			UpgradedHooks = false;
			EmeraldEmpoweredGem = false;
			QueensStinger = false;
			PoweredBattery = false;

			//ShieldOn = false;
			//ShieldBroken = false;
			//ShieldCapacity = 0;
			//ShieldType = ShieldTypes.Count;
		}


		public override void OnEnterWorld(Player player)
		{
			Main.NewText("NOTE: Aerovelence is still in development. Some items may work differently than intended!", Color.Orange);
			Main.NewText("We are looking for team members who can help out. If you are interested, join our Discord", Color.Orange);
			Main.NewText("by clicking our mod's home-page. Thank you, and have fun!", Color.Orange);
		}
		public override void UpdateDead()
		{
			zooming = false;

			FishPartner = false;

			SoulFire = false;
			MiningAbilityCooldown = false;
			badHeal = false;
			Electrified = false;
		}
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (AerovelenceMod.ArmorHotKey.JustPressed)
				if (MiningAbilityCooldown == false)

					if (AmbrosiaBonus)
					{
						Terraria.Projectile.NewProjectile(player.Center, (Terraria.Main.MouseWorld - player.Center) / 10, ModContent.ProjectileType<MiningEnergyBlast>(), 1, 0);
						player.AddBuff(ModContent.BuffType<MiningAbilityCooldown>(), 100);
					}
		}
		public override void OnHitNPC(Terraria.Item item, Terraria.NPC target, int damage, float knockback, bool crit)
		{
			if (FrostMelee)
				if (Terraria.Main.rand.NextBool(2))
					target.AddBuff(BuffID.Frostburn, 120);
		}
		public override void OnHitByNPC(Terraria.NPC npc, int damage, bool crit)
		{
			Terraria.Player player = Terraria.Main.player[npc.target];
			if (PhanticMeleeBonus)
			{
				if (damage > 10)
				{
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
			}
			if (BurnshockArmorBonus)
			{
				if (damage > 25)
				{
					Vector2 offset = new Vector2(0, -100);

					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
				}
			}
		}
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (ShieldOn)
			{
				foreach (Item item in player.armor)
				{
					if (item.modItem != null && item.modItem.GetType().IsSubclassOf(typeof(ShieldItem)) && ShieldCapacity > 200)
					{
						ShieldCapacity -= 200;
						if (ShieldType == ShieldTypes.Bubble || (ShieldType == ShieldTypes.Impact && Main.rand.Next(1, 3) % 3 == 0) || ShieldType == ShieldTypes.Nova && (damageSource.SourceNPCIndex != -1 || damageSource.SourceProjectileIndex != -1))
						{
							player.ShadowDodge();
							return false;
						}
					}
				}
			}
			return true;
		}
		public override void OnHitByProjectile(Terraria.Projectile proj, int damage, bool crit)
		{
			if (PhanticMeleeBonus)
			{
				if (damage > 10)
				{
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
			}
		}
		public override void OnHitNPCWithProj(Terraria.Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
		{
			if (QueensStinger)
				if (proj.type != 181)
					if (Terraria.Main.rand.NextBool(10))
						Terraria.Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.Bee, 3, 2, player.whoAmI);

			if (EmeraldEmpoweredGem)
				target.AddBuff(39, 40);

			if (MidasCrown)
				target.AddBuff(BuffID.Midas, 900);

			if (FrostProjectile)
				if (Terraria.Main.rand.NextBool(2))
					target.AddBuff(BuffID.Frostburn, 120);


			if (SpiritCultistBonus && proj.magic && !target.boss)
			{
				if (target.FindBuffIndex(ModContent.BuffType<LiftedSpiritsDebuff>()) < 1)
					target.velocity.Y -= 20;

				target.AddBuff(ModContent.BuffType<LiftedSpiritsDebuff>(), 210);
			}
			if (PhanticRangedBonus && proj.ranged && Terraria.Main.rand.NextFloat() < 0.15f && proj.type != ModContent.ProjectileType<PhanticSoul>())
			{
				float rot = Terraria.Main.rand.NextFloat(MathHelper.TwoPi);
				Vector2 position = target.Center + Vector2.One.RotatedBy(rot) * 180;
				Vector2 velocity = Vector2.One.RotatedBy(rot) * -1 * 12f;
				Terraria.Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<PhanticSoul>(), 30, player.HeldItem.knockBack, player.whoAmI, 0, 0);
			}
		}
		public override void PreUpdate()
		{
			if (TravellingByETP)
			{
				if (player.Hitbox.Intersects(new Rectangle((int)ETPDestination.X + 24 - 16, (int)ETPDestination.Y - 16, 24, 48)))
				{
					TravellingByETP = false;
					ETPDestination = new Vector2(0, 0);
					player.velocity *= 0.05f;
					for (int i2 = 0; i2 < 4; i2++)
					{
						Dust Dust1 = Dust.NewDustDirect(new Vector2(ETPDestination.X + 8f, ETPDestination.Y - 8f), 20, 20, DustID.Electric, 0f, 0f, 100, default, 1f);
						Dust1.velocity *= 1.6f;
						Dust Dust2 = Dust1;
						Dust2.velocity.Y -= 1f;
						Dust1.position = Vector2.Lerp(Dust1.position, new Vector2(ETPDestination.X + 8f, ETPDestination.Y - 8f), 0.5f);
					}
				}
				else
				{
					ETPDustDelay--;
					if (ETPDustDelay <= 0)
					{
						Terraria.Dust Dust1 = Terraria.Dust.NewDustPerfect(player.Center, 229, default, 0, Color.DarkBlue, 2f);
						Dust1.noGravity = true;
						ETPDustDelay = 3;
					}
					ETPSoundDelay--;
					if (ETPSoundDelay <= 0)
					{
						Main.PlaySound(SoundID.Item93, player.Center);
						ETPSoundDelay = 20;
					}
					Terraria.Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.3f, 0.8f, 1.1f);
					player.AddBuff(BuffID.Cursed, 2);
					player.AddBuff(BuffID.Invisibility, 2);
					player.gravity = 0f;
					player.direction = player.velocity.X > 0 ? 1 : -1;
					player.velocity = player.DirectionTo(ETPDestination) * 6f;
					player.position += player.velocity;
				}
			}
		}
		public override void SetControls()
		{
			if (TravellingByETP)
				player.controlDown = true;
		}
		internal void DetouredItemCheck(On.Terraria.Player.orig_ItemCheck orig, Terraria.Player self, int i)
		{
			if (self.GetModPlayer<AeroPlayer>().PhanticMagicBonus && self.HeldItem.magic && Terraria.Main.rand.NextFloat() < 0.125f)
				if (!self.releaseUseItem && self.itemAnimation == self.HeldItem.useAnimation - 1 && self.itemAnimation != 0)
					Terraria.Projectile.NewProjectile(self.Center, self.DirectionTo(Terraria.Main.MouseWorld) * self.HeldItem.shootSpeed / 2,
						ModContent.ProjectileType<PhanticSoul>(), 40, self.HeldItem.knockBack, self.whoAmI);

			orig(self, i);
		}
		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (SoulFire)
			{
				if (Terraria.Main.rand.NextBool(4) && drawInfo.shadow == 0f)
				{
					int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4,
						player.height + 4, ModContent.DustType<WispDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f,
						100, default, 3f);

					Terraria.Main.dust[dust].noGravity = true;
					Terraria.Main.dust[dust].velocity *= 1.8f;
					Terraria.Main.dust[dust].velocity.Y -= 0.5f;

					Terraria.Main.playerDrawDust.Add(dust);
				}

				r *= 0.1f;
				g *= 0.2f;
				b *= 0.7f;

				fullBright = true;
			}

			if (Electrified)
			{
				if (Terraria.Main.rand.NextBool(4) && drawInfo.shadow == 0f)
				{
					int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4,
						player.height + 4, DustID.AncientLight, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100,
						default, 3f);
					Terraria.Main.dust[dust].noGravity = true;
					Terraria.Main.dust[dust].velocity *= 1.8f;
					Terraria.Main.dust[dust].velocity.Y -= 0.5f;
					Terraria.Main.playerDrawDust.Add(dust);
				}

				r *= 0.0f;
				g *= 0.2f;
				b *= 0.7f;
				fullBright = true;
			}
		}
		public override void ResetEffects()
		{
			zooming = false;

			FishPartner = false;

			AdobeHelmet = false;
			ShiverMinion = false;
			FrostProjectile = false;
			FrostMelee = false;
			MiningAbilityCooldown = false;
			FrostMinion = false;
			PhanticMeleeBonus = false;
			PhanticMagicBonus = false;
			lumberjackSetBonus = false;
			UpgradedHooks = false;
			BurnshockArmorBonus = false;
			SpiritCultistBonus = false;
			badHeal = false;
			MiningAbilityCooldown = false;
			AmbrosiaBonus = false;
			QueensStinger = false;

			NeutronMinion = false;
			StarDrone = false;
			Minicry = false;
			charmingBush = false;
			huntressSummon = false;

			ShieldOn = false;
		}
		public static readonly PlayerLayer MiscEffects = new PlayerLayer("AerovelenceMod", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
				return;

			Mod mod = ModLoader.GetMod("AerovelenceMod");

			Terraria.Player drawPlayer = drawInfo.drawPlayer;
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();

			if (modPlayer.badHeal)
			{
				Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/Buffs/SoulFire");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X
						+ drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f,
						new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);

				Terraria.Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}

			if (modPlayer.MiningAbilityCooldown)
			{
				Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/Buffs/MiningAbilityCooldown");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X
					+ drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f,
					new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);

				Terraria.Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}

			if (modPlayer.Electrified)
			{
				Texture2D texture = ModContent.GetTexture("AerovelenceMod/Content/Buffs/Electrified");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X
					+ drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f,
					new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);

				Terraria.Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}

			if ((modPlayer.ShieldType == ShieldTypes.Bubble || modPlayer.ShieldType == ShieldTypes.Nova) && !modPlayer.ShieldBroken && modPlayer.ShieldOn)
            {
				Texture2D texture;
				Color color;
				Vector2 position = modPlayer.player.Center - Main.screenPosition;
				if (modPlayer.ShieldType == ShieldTypes.Bubble)
                {
					texture = ModContent.GetTexture("AerovelenceMod/Assets/Shield/BubbleShield");
					color = new Color(160, 160, 160, 160);
				}
				else 
				{ 
					texture = ModContent.GetTexture("AerovelenceMod/Assets/Shield/NovaShield");
					color = Color.White;
					position = modPlayer.player.Center - Main.screenPosition + new Vector2(0,5);
				}
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, position, null, color, 0f,
					new Vector2(texture.Width / 2f, texture.Height / 2f), MathHelper.Lerp(0.9f + ShieldProjectile.sizeBoost, 1f + ShieldProjectile.sizeBoost, ShieldProjectile.scale), SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);
			}
		});
		public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("AerovelenceMod", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();
			if (modPlayer.ShieldType == ShieldTypes.Nova && !modPlayer.ShieldBroken && modPlayer.ShieldOn)
			{
				Texture2D texture = ModContent.GetTexture("AerovelenceMod/Assets/Shield/NovaShield_Back");
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, modPlayer.player.Center - Main.screenPosition + new Vector2(0, 5), null, Color.White, 0f,
					new Vector2(texture.Width / 2f, texture.Height / 2f), MathHelper.Lerp(0.9f + ShieldProjectile.sizeBoost, 1f + ShieldProjectile.sizeBoost, ShieldProjectile.scale), SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);
			}
		});
		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			MiscEffectsBack.visible = true;
			MiscEffects.visible = true;
			layers.Insert(0, MiscEffectsBack);
			layers.Add(MiscEffects);
		}
		public override void ModifyZoom(ref float zoom)
		{
			if (zooming)
				zoom = 1f;
		}

		public override void ModifyScreenPosition()
		{
			float mult = Main.screenWidth / 2048f; //normalize for screen resolution

			Main.screenPosition.Y += Main.rand.Next(-Shake, Shake) * mult;
			Main.screenPosition.X += Main.rand.Next(-Shake, Shake) * mult;

			if (Shake > 0)
				Shake--;

			Main.screenPosition.X = (int)Main.screenPosition.X;
			Main.screenPosition.Y = (int)Main.screenPosition.Y;
		}
	}
}
