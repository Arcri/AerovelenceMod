using Terraria.ID;
using Terraria;
using Terraria.GameInput;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using AerovelenceMod.Common.Systems;
using ReLogic.Content;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic;
using static Humanizer.In;
using System;

namespace AerovelenceMod
{
	public class AeroPlayer : ModPlayer
	{

		public int PlatformTimer = 0;

		public float ScreenShakePower;

		public override void ModifyScreenPosition()
		{
			if (ScreenShakePower > 0.1f) //Kackbrise#5454 <3 <3 Thank
			{
				Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakePower), Main.rand.NextFloat(ScreenShakePower));
				ScreenShakePower *= 0.9f;
			}
		}

		public override void PreUpdate()
		{
			PlatformTimer--;
		}

		#region Usestyle Code
		public object useStyleData;
		public int useStyleInt;

		public int fireballFrame;
		public float glowPercent;
		public float fireBallAlpha;

		public float KWRot;

		public float[] orbGlowAmount = new float[5];

		//Draw Layer

		//TODO move this with the weapon
		private class CthulhusWrathDrawLayer : PlayerDrawLayer
		{
			private Asset<Texture2D> fireBallTex;
            private Asset<Texture2D> fireBallGlowTex;

            private Asset<Texture2D> KW;
            private Asset<Texture2D> KWWhite;
            private Asset<Texture2D> KWGlow;

            private Asset<Texture2D> Line;
            private Asset<Texture2D> Glow;
            private Asset<Texture2D> Glow2;


            public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
			{
				return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<CthulhusWrath>() &&
					drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().useStyleData is (Vector2[]) &&
					drawInfo.drawPlayer.controlUseItem &&
					drawInfo.drawPlayer.CheckMana(drawInfo.drawPlayer.HeldItem);
			}
			public override Position GetDefaultPosition()
			{
				return new BeforeParent(PlayerDrawLayers.HeldItem);
			}
			protected override void Draw(ref PlayerDrawSet drawInfo)
			{

				//textures
				if (fireBallTex == null)
                    fireBallTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWFireball");

                if (fireBallGlowTex == null)
                    fireBallGlowTex = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWFireballGlow");


                if (KW == null)
                    KW = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CthulhusWrath");

                if (KWWhite == null)
                    KWWhite = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWWhite");

                if (KWGlow == null)
                    KWGlow = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/KWWhiteGlow");

                if (Line == null)
                    Line = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray");

                if (Glow == null)
                    Glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/impact_2fade2");

                if (Glow2 == null)
                    Glow2 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");

				if (drawInfo.shadow != 0f)
					return;

                Vector2 itemOrigin = new Vector2(0, KW.Height());
				float rotation = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().KWRot + MathHelper.PiOver4;
				//Vector2 armPosition = drawInfo.drawPlayer.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation); // get position of hand

				//armPosition.Y += drawInfo.drawPlayer.gfxOffY;

				Vector2 drawPos = drawInfo.drawPlayer.MountedCenter - Main.screenPosition;
                drawPos.Y += drawInfo.drawPlayer.gfxOffY;


                drawInfo.DrawDataCache.Add(new DrawData(KW.Value, drawPos, null, Color.White * 1f, rotation, itemOrigin, 1f, SpriteEffects.None, 0));

                float intensity = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().glowPercent;


                int numberOfFrames = 6;
                int frameHeight = fireBallTex.Height() / numberOfFrames;
                int startY = frameHeight * drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().fireballFrame;

                // Get this frame on texture
                Rectangle sourceRectangle = new Rectangle(0, startY, fireBallTex.Width(), frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                Vector2[] orbs = (Vector2[])drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().useStyleData;
				for (int i = 0; i < orbs.Length; i++)
				{

                    Vector2 pos = drawInfo.drawPlayer.MountedCenter + (orbs[i] * 4) - Main.screenPosition;

					float orbAlpha = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().fireBallAlpha;


                    float glowOrbAmount = drawInfo.drawPlayer.GetModPlayer<AeroPlayer>().orbGlowAmount[i];

                    //drawInfo.DrawDataCache.Add(new DrawData(Glow2.Value, pos, null, Color.Orange with { A = 0 } * 1f, (float)Main.timeForVisualEffects * 0.02f + orbs[i].ToRotation(), Glow2.Size() / 2, glowOrbAmount * orbAlpha, SpriteEffects.None, 0));
                    //drawInfo.DrawDataCache.Add(new DrawData(Glow2.Value, pos, null, Color.White with { A = 0 } * 1f, (float)Main.timeForVisualEffects * 0.02f + orbs[i].ToRotation(), Glow2.Size() / 2, glowOrbAmount * 0.75f * orbAlpha, SpriteEffects.None, 0));

                    drawInfo.DrawDataCache.Add(new DrawData(fireBallTex.Value, pos, sourceRectangle, Color.Yellow * 0.2f * orbAlpha, orbs[i].ToRotation(), origin, 1.4f, SpriteEffects.None, 0));
					drawInfo.DrawDataCache.Add(new DrawData(fireBallTex.Value, pos + new Vector2(Main.rand.NextFloat(0,2), Main.rand.NextFloat(0, 2)), sourceRectangle, Color.White * 1f * orbAlpha, orbs[i].ToRotation(), origin, 1f, SpriteEffects.None, 0));

                    //drawInfo.DrawDataCache.Add(new DrawData(fireBallGlowTex.Value, pos, sourceRectangle, Color.White * 1f * orbAlpha * glowOrbAmount, orbs[i].ToRotation(), origin, 1f, SpriteEffects.None, 0));

                    drawInfo.DrawDataCache.Add(new DrawData(KWWhite.Value, drawPos, null, Color.White * intensity, rotation, itemOrigin, 1f, SpriteEffects.None, 0));
                    drawInfo.DrawDataCache.Add(new DrawData(KWGlow.Value, drawPos + Main.rand.NextVector2Circular(2.5f, 2.5f), null, Color.Orange with { A = 0 } * intensity * 0.8f, rotation, itemOrigin, 1f, SpriteEffects.None, 0));


				}
			}
			public override void Unload()
			{
                fireBallTex = null;
				fireBallGlowTex = null;

                KW = null;
                KWGlow = null;
                KWWhite = null;

				Line = null;
                Glow = null;
                Glow2 = null;

            }
        }
		#endregion


		/*

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
		public bool crabSummon;

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
			
			if (KeybindSystem.ArmorHotKey.JustPressed)
				if (MiningAbilityCooldown == false)

					if (AmbrosiaBonus)
					{
						Terraria.Projectile.NewProjectile(Player.GetSource_Misc("SetBonus_AmbrosiaSetBonus"), Player.Center, (Terraria.Main.MouseWorld - Player.Center) / 10, ModContent.ProjectileType<MiningEnergyBlast>(), 1, 0);
						Player.AddBuff(ModContent.BuffType<MiningAbilityCooldown>(), 100);
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
					Terraria.Projectile.NewProjectile(player.GetSource_OnHurt(player), player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
			}
			if (BurnshockArmorBonus)
			{
				if (damage > 25)
				{
					Vector2 offset = new Vector2(0, -100);

					Terraria.Projectile.NewProjectile(player.GetSource_OnHurt(player), player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.GetSource_OnHurt(player), player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.GetSource_OnHurt(player), player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
				}
			}
			
		}
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			
			if (ShieldOn)
			{
				foreach (Item item in Player.armor)
				{
					if (item.ModItem != null && item.ModItem.GetType().IsSubclassOf(typeof(ShieldItem)) && ShieldCapacity > 200)
					{
						ShieldCapacity -= 200;
						if (ShieldType == ShieldTypes.Bubble || (ShieldType == ShieldTypes.Impact && Main.rand.Next(1, 3) % 3 == 0) || ShieldType == ShieldTypes.Nova && (damageSource.SourceNPCIndex != -1 || damageSource.SourceProjectileIndex != -1))
						{
							Player.ShadowDodge();
							return false;
						}
					}
				}
			}
			
			return true;
		}
		public override void OnHitByProjectile(Terraria.Projectile proj, int damage, bool crit)
		{
			/*
			if (PhanticMeleeBonus)
			{
				if (damage > 10)
				{
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(Projectile.GetSource_None(), Player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
			}
			
		}
		public override void OnHitNPCWithProj(Terraria.Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
		{
			/*
			if (QueensStinger)
				if (proj.type != 181)
					if (Terraria.Main.rand.NextBool(10))
						Terraria.Projectile.NewProjectile(target.GetSource_OnHurt(target), target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.Bee, 3, 2, Player.whoAmI);

			if (EmeraldEmpoweredGem)
				target.AddBuff(39, 40);

			if (MidasCrown)
				target.AddBuff(BuffID.Midas, 900);

			if (FrostProjectile)
				if (Terraria.Main.rand.NextBool(2))
					target.AddBuff(BuffID.Frostburn, 120);


			if (SpiritCultistBonus && proj.DamageType == DamageClass.Magic && !target.boss)
			{
				if (target.FindBuffIndex(ModContent.BuffType<LiftedSpiritsDebuff>()) < 1)
					target.velocity.Y -= 20;

				target.AddBuff(ModContent.BuffType<LiftedSpiritsDebuff>(), 210);
			}
			if (PhanticRangedBonus && proj.DamageType == DamageClass.Ranged && Terraria.Main.rand.NextFloat() < 0.15f && proj.type != ModContent.ProjectileType<PhanticSoul>())
			{
				float rot = Terraria.Main.rand.NextFloat(MathHelper.TwoPi);
				Vector2 position = target.Center + Vector2.One.RotatedBy(rot) * 180;
				Vector2 velocity = Vector2.One.RotatedBy(rot) * -1 * 12f;
				Terraria.Projectile.NewProjectile(Player.GetSource_Misc("SetBonus_PhanticSetBonus"), position, velocity, ModContent.ProjectileType<PhanticSoul>(), 30, Player.HeldItem.knockBack, Player.whoAmI, 0, 0);
			}
			
		}
		public override void PreUpdate()
		{
			if (TravellingByETP)
			{
				if (Player.Hitbox.Intersects(new Rectangle((int)ETPDestination.X + 24 - 16, (int)ETPDestination.Y - 16, 24, 48)))
				{
					TravellingByETP = false;
					ETPDestination = new Vector2(0, 0);
					Player.velocity *= 0.05f;
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
						Terraria.Dust Dust1 = Terraria.Dust.NewDustPerfect(Player.Center, 229, default, 0, Color.DarkBlue, 2f);
						Dust1.noGravity = true;
						ETPDustDelay = 3;
					}
					ETPSoundDelay--;
					if (ETPSoundDelay <= 0)
					{
						SoundEngine.PlaySound(SoundID.Item93, Player.Center);
						ETPSoundDelay = 20;
					}
					Terraria.Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 0.3f, 0.8f, 1.1f);
					Player.AddBuff(BuffID.Cursed, 2);
					Player.AddBuff(BuffID.Invisibility, 2);
					Player.gravity = 0f;
					Player.direction = Player.velocity.X > 0 ? 1 : -1;
					Player.velocity = Player.DirectionTo(ETPDestination) * 6f;
					Player.position += Player.velocity;
				}
			}
		}
		public override void SetControls()
		{
			if (TravellingByETP)
				Player.controlDown = true;
		}
		internal void DetouredItemCheck(On.Terraria.Player.orig_ItemCheck orig, Terraria.Player self, int i)
		{
			
			if (self.GetModPlayer<AeroPlayer>().PhanticMagicBonus && self.HeldItem.DamageType == DamageClass.Magic && Terraria.Main.rand.NextFloat() < 0.125f)
				if (!self.releaseUseItem && self.itemAnimation == self.HeldItem.useAnimation - 1 && self.itemAnimation != 0)
					//Terraria.Projectile.NewProjectile((IEntitySource)self, self.Center, self.DirectionTo(Terraria.Main.MouseWorld) * self.HeldItem.shootSpeed / 2,
						//ModContent.ProjectileType<PhanticSoul>(), 40, self.HeldItem.knockBack, self.whoAmI);

			orig(self, i);
			
		}
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			if (SoulFire)
			{
				if (Terraria.Main.rand.NextBool(4) && drawInfo.shadow == 0f)
				{
					//int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), Player.width + 4,
						//Player.height + 4, ModContent.DustType<WispDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f,
						//100, default, 3f);

					//Terraria.Main.dust[dust].noGravity = true;
					//Terraria.Main.dust[dust].velocity *= 1.8f;
					//Terraria.Main.dust[dust].velocity.Y -= 0.5f;

					//Terraria.Main.playerDrawDust.Add(dust);
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
					//int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), Player.width + 4,
					//	Player.height + 4, DustID.AncientLight, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100,
					//	default, 3f);
					//Terraria.Main.dust[dust].noGravity = true;
					//Terraria.Main.dust[dust].velocity *= 1.8f;
					//Terraria.Main.dust[dust].velocity.Y -= 0.5f;
					//Terraria.Main.playerDrawDust.Add(dust);
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
			crabSummon = false;

		ShieldOn = false;
		}
		//public static readonly PlayerDrawLayers MiscEffects = new PlayerDrawLayer("AerovelenceMod", "MiscEffects", PlayerDrawLayers.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
		//{
			/*if (drawInfo.shadow != 0f)
				return;

			Mod mod = ModLoader.GetMod("AerovelenceMod");

			Terraria.Player drawPlayer = drawInfo.drawPlayer;
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();

			if (modPlayer.badHeal)
			{
				Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Buffs/SoulFire");

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
				Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Buffs/MiningAbilityCooldown");

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
				Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Buffs/Electrified");

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
				Vector2 position = modPlayer.Player.Center - Main.screenPosition;
				if (modPlayer.ShieldType == ShieldTypes.Bubble)
                {
					texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/BubbleShield");
					color = new Color(160, 160, 160, 160);
				}
				else 
				{ 
					texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/NovaShield");
					color = Color.White;
					position = modPlayer.Player.Center - Main.screenPosition + new Vector2(0,5);
				}
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, position, null, color, 0f,
					new Vector2(texture.Width / 2f, texture.Height / 2f), MathHelper.Lerp(0.9f + ShieldProjectile.sizeBoost, 1f + ShieldProjectile.sizeBoost, ShieldProjectile.scale), SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);
			}*/
		//});
		/*public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("AerovelenceMod", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();
			if (modPlayer.ShieldType == ShieldTypes.Nova && !modPlayer.ShieldBroken && modPlayer.ShieldOn)
			{
				Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/NovaShield_Back");
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, modPlayer.Player.Center - Main.screenPosition + new Vector2(0, 5), null, Color.White, 0f,
					new Vector2(texture.Width / 2f, texture.Height / 2f), MathHelper.Lerp(0.9f + ShieldProjectile.sizeBoost, 1f + ShieldProjectile.sizeBoost, ShieldProjectile.scale), SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);
			}
		});
		public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
		{
			//MiscEffectsBack.visible = true;
			//MiscEffects.visible = true;
			//layers.Insert(0, MiscEffectsBack);
			//layers.Add(MiscEffects);
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

	*/
	}
}
