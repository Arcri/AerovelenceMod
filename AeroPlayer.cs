using AerovelenceMod.Buffs;
using AerovelenceMod.Dusts;
using AerovelenceMod.Events;
using AerovelenceMod.Projectiles.Other.ArmorSetBonus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod
{
    public class AeroPlayer : ModPlayer
	{
        //ARCRI I DONT UNDERSTAND THESE GROUPS
        public bool zooming;
		//PLEASE MOVE THIS - its used for zooming with weapons :wegud:

		#region PETS
		public bool FishPartner;
        #endregion

        public bool SoulFig;
		public bool KnowledgeFruit;
		public bool DevilsBounty;


		public bool ZoneCrystalCaverns;
		public bool ZoneCrystalCitadel;

		public bool SoulFire;
		public bool MiningAbilityCooldown;
		public bool Electrified;
		public bool badHeal;

		public bool QueensStinger;
		public bool UpgradedHooks;
		public bool EmeraldEmpoweredGem;
		public bool MidasCrown;

		public bool AdobeHelmet;
		public bool AmbrosiaBonus = false;
		public bool PhanticBonus;
		public bool PhanticMagicBonus;
		public bool FrostMelee;
		public bool FrostProjectile;
		public bool FrostMinion;
		public bool BurnshockArmorBonus;
		public bool SpiritCultistBonus = false;

		public bool ShiverMinion;
		public bool NeutronMinion = false;
		public bool StarDrone;
		public bool Minicry = false;


		public override void Initialize()
		{

			zooming = false;

			FishPartner = false;

			AdobeHelmet = false;
			AmbrosiaBonus = false;
			PhanticBonus = false;
			SpiritCultistBonus = false;
			FrostMelee = false;
			FrostProjectile = false;
			FrostMinion = false;
			BurnshockArmorBonus = false;

			MidasCrown = false;
			UpgradedHooks = false;
			EmeraldEmpoweredGem = false;
			QueensStinger = false;
		}



		public override void UpdateBiomeVisuals()
		{
			if (player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns)
			{
				player.ManageSpecialBiomeVisuals("AerovelenceMod:FoggyFields", FoggyFieldsWorld.FoggyFields, player.Center);
			}
			player.ManageSpecialBiomeVisuals("AerovelenceMod:CrystalTorrents", CrystalTorrentWorld.CrystalTorrents, player.Center);
			player.ManageSpecialBiomeVisuals("AerovelenceMod:DarkNights", DarkNightWorld.DarkNight, player.Center);
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
			if (AeroMod.ArmorHotKey.JustPressed)
			{
				if (MiningAbilityCooldown == false)
				{
					if (AmbrosiaBonus)
					{
						Terraria.Projectile.NewProjectile(player.Center, (Terraria.Main.MouseWorld - player.Center) / 10, ProjectileType<MiningEnergyBlast>(), 1, 0);
						player.AddBuff(BuffType<MiningAbilityCooldown>(), 600);
					}
				}
			}
		}

		public override void OnHitNPC(Terraria.Item item, Terraria.NPC target, int damage, float knockback, bool crit)
		{
			if (FrostMelee)
			{
				if (Terraria.Main.rand.NextBool(2)) //  50% chance
					target.AddBuff(BuffID.Frostburn, 120, false);
			}
		}

        public override void OnHitByNPC(Terraria.NPC npc, int damage, bool crit)
        {
			if (PhanticBonus)
            {
				if (damage > 10)
                {
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
            }
			if (BurnshockArmorBonus)
			{
				if (damage > 25)
				{
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ProjectileType<BurnshockCrystal>(), 40, 1f, Terraria.Main.myPlayer);
				}
			}
		}

        public override void OnHitByProjectile(Terraria.Projectile proj, int damage, bool crit)
		{
			if (PhanticBonus)
			{
				if (damage > 10)
				{
					Vector2 offset = new Vector2(0, -100);
					Terraria.Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Terraria.Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ProjectileType<PhanticSoul>(), 6, 1f, Terraria.Main.myPlayer);
				}
			}
		}

		public override void OnHitNPCWithProj(Terraria.Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
		{
			if (QueensStinger)
			{
				if (proj.type != 181)
					if (Terraria.Main.rand.NextBool(10)) //  1 in 10 chance
						Terraria.Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.Bee, 3, 2, player.whoAmI);
			}
			if (EmeraldEmpoweredGem)
			{
				target.AddBuff(39, 40, false);
			}
			if (MidasCrown)
			{
				target.AddBuff(BuffID.Midas, 900, false);
			}
			if (FrostProjectile)
			{
				if (Terraria.Main.rand.NextBool(2)) //  50% chance
				{
					target.AddBuff(BuffID.Frostburn, 120, false);
				}
			}
		
			if (SpiritCultistBonus && proj.magic && !target.boss)
			{
				if (target.FindBuffIndex(mod.BuffType("LiftedSpiritsDebuff")) < 1)
				{
					target.velocity.Y -= 20;
				}
				target.AddBuff(mod.BuffType("LiftedSpiritsDebuff"), 210, false);
			}
		}
		internal void DetouredItemCheck(On.Terraria.Player.orig_ItemCheck orig, Terraria.Player self, int i)
		{
			//MonoMod my beloved. ~Exitium
			if (self.GetModPlayer<AeroPlayer>().PhanticMagicBonus && self.HeldItem.magic && Main.rand.NextFloat() < 0.125f)
			{
				if (!self.releaseUseItem && self.itemAnimation == self.HeldItem.useAnimation - 1 && self.itemAnimation != 0)
				{
					Projectile.NewProjectile(self.Center, self.DirectionTo(Main.MouseWorld) * self.HeldItem.shootSpeed / 2, ProjectileType<PhanticSoul>(), self.HeldItem.damage, self.HeldItem.knockBack, self.whoAmI);
				}
			}
			orig(self, i);
		}
		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (SoulFire)
			{
				if (Terraria.Main.rand.NextBool(4) && drawInfo.shadow == 0f)
				{
					int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustType<WispDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
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
					int dust = Terraria.Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustID.AncientLight, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 3f);
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


		public override void UpdateBiomes()
		{
			ZoneCrystalCaverns = AeroWorld.cavernTiles > 50;
			ZoneCrystalCitadel = AeroWorld.citadelTiles > 50;
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
			PhanticBonus = false;
			PhanticMagicBonus = false;
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
		}

		public static readonly PlayerLayer MiscEffects = new PlayerLayer("AerovelenceMod", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}
			Terraria.Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("AerovelenceMod");
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();
			if (modPlayer.badHeal)
			{
				Texture2D texture = mod.GetTexture("Buffs/SoulFire");
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);
				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
				Terraria.Main.playerDrawData.Add(data);
				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}
			if (modPlayer.MiningAbilityCooldown)
			{
				Texture2D texture = mod.GetTexture("Buffs/MiningAbilityCooldown");
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);
				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
				Terraria.Main.playerDrawData.Add(data);
				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}
			if (modPlayer.Electrified)
			{
				Texture2D texture = mod.GetTexture("Buffs/Electrified");
				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Terraria.Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);
				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Terraria.Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
				Terraria.Main.playerDrawData.Add(data);
				for (int k = 0; k < 2; k++)
				{
					int dust = Terraria.Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Terraria.Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Terraria.Main.playerDrawDust.Add(dust);
				}
			}
		});
		public override void CopyCustomBiomesTo(Terraria.Player other)
		{
			AeroPlayer modOther = other.GetModPlayer<AeroPlayer>();
			modOther.ZoneCrystalCaverns = ZoneCrystalCaverns;
			modOther.ZoneCrystalCitadel = ZoneCrystalCitadel;
		}

		public override void SendCustomBiomes(BinaryWriter writer)
		{
			Terraria.BitsByte flags = new Terraria.BitsByte();
			flags[0] = ZoneCrystalCaverns;
			flags[1] = ZoneCrystalCitadel;
			writer.Write(flags);
		}



		public override void ReceiveCustomBiomes(BinaryReader reader)
		{
			Terraria.BitsByte flags = reader.ReadByte();
			ZoneCrystalCaverns = flags[0];
			ZoneCrystalCitadel = flags[1];
		}



		public override bool CustomBiomesMatch(Terraria.Player other)
		{
			AeroPlayer modOther = other.GetModPlayer<AeroPlayer>();
			return ZoneCrystalCaverns == modOther.ZoneCrystalCaverns && ZoneCrystalCitadel == modOther.ZoneCrystalCitadel;
		}

		public override Texture2D GetMapBackgroundImage()
		{
			if (ZoneCrystalCaverns)
			{
				return mod.GetTexture("CrystalCavernsMapBackground");
			}
			return null;
		}

        public override void ModifyZoom(ref float zoom)
        {
			if (zooming)
			{
				zoom = 1f;
			}
		}



        public static bool BossPresent => Terraria.Main.npc.ToList().Any(npc => npc.boss && npc.active);

		public Terraria.NPC GetFarthestBoss
		{
			get
			{
				//Make sure that when checking for bosses you also make sure they're active
				List<Terraria.NPC> npcList = Terraria.Main.npc.ToList();
				List<Terraria.NPC> bosses = npcList.Where(npc => npc.boss && npc.active).Where(me => Vector2.Distance(me.Center, player.Center) == npcList.Where(npc => npc.boss && npc.active).Max(boss => Vector2.Distance(boss.Center, player.Center)) && me.active).ToList();
				return bosses[0];
			}
		}
	}
}
