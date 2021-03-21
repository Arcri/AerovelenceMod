using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other.ArmorSetBonus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod
{
    public class AeroPlayer : ModPlayer
	{
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

		public bool AdobeHelmet;
		public bool AmbrosiaBonus;
		public bool PhanticBonus;
		public bool PhanticMagicBonus;
		public bool FrostMelee;
		public bool FrostProjectile;
		public bool FrostMinion;
		public bool BurnshockArmorBonus;
		public bool SpiritCultistBonus;

		public bool ShiverMinion;
		public bool NeutronMinion;
		public bool StarDrone;
		public bool Minicry;


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
						Projectile.NewProjectile(player.Center, (Main.MouseWorld - player.Center) / 10, ModContent.ProjectileType<MiningEnergyBlast>(), 1, 0);
						player.AddBuff(ModContent.BuffType<MiningAbilityCooldown>(), 600);
					}
        }

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (FrostMelee)
                if (Main.rand.NextBool(2))
					target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
			if (PhanticBonus)
            {
				if (damage > 10)
                {
					Vector2 offset = new Vector2(0, -100);
					Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Terraria.Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Main.myPlayer);
				}
            }
			if (BurnshockArmorBonus)
			{
				if (damage > 25)
				{
					Vector2 offset = new Vector2(0, -100);

					Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Main.myPlayer);
					Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Main.myPlayer); 
					Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<BurnshockCrystal>(), 40, 1f, Main.myPlayer);
				}
			}
		}

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
		{
			if (PhanticBonus)
			{
				if (damage > 10)
				{
					Vector2 offset = new Vector2(0, -100);
					Projectile.NewProjectile(player.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<PhanticSoul>(), 6, 1f, Main.myPlayer);
				}
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (QueensStinger)
                if (proj.type != 181)
					if (Main.rand.NextBool(10))
						Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.Bee, 3, 2, player.whoAmI);
			
			if (EmeraldEmpoweredGem)
                target.AddBuff(39, 40);
			
			if (MidasCrown)
                target.AddBuff(BuffID.Midas, 900);
			
			if (FrostProjectile)
                if (Main.rand.NextBool(2))
                    target.AddBuff(BuffID.Frostburn, 120);
            
		
			if (SpiritCultistBonus && proj.magic && !target.boss)
			{
				if (target.FindBuffIndex(ModContent.BuffType<LiftedSpiritsDebuff>()) < 1)
                    target.velocity.Y -= 20;
				
				target.AddBuff(ModContent.BuffType<LiftedSpiritsDebuff>(), 210);
			}
		}
		internal void DetouredItemCheck(On.Terraria.Player.orig_ItemCheck orig, Player self, int i)
		{
            if (self.GetModPlayer<AeroPlayer>().PhanticMagicBonus && self.HeldItem.magic && Main.rand.NextFloat() < 0.125f)
                if (!self.releaseUseItem && self.itemAnimation == self.HeldItem.useAnimation - 1 && self.itemAnimation != 0)
                    Projectile.NewProjectile(self.Center, self.DirectionTo(Main.MouseWorld) * self.HeldItem.shootSpeed / 2, 
                        ModContent.ProjectileType<PhanticSoul>(), self.HeldItem.damage, self.HeldItem.knockBack, self.whoAmI);
            
            orig(self, i);
		}

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (SoulFire)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4,
                        player.height + 4, ModContent.DustType<WispDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f,
                        100, default, 3f);

                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;

                    Main.playerDrawDust.Add(dust);
                }

                r *= 0.1f;
                g *= 0.2f;
                b *= 0.7f;

                fullBright = true;
            }

            if (Electrified)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4,
                        player.height + 4, DustID.AncientLight, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100,
                        default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
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
                return;
            
			Mod mod = ModLoader.GetMod("AerovelenceMod");

            Player drawPlayer = drawInfo.drawPlayer;
			AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();

			if (modPlayer.badHeal)
			{
				Texture2D texture = mod.GetTexture("Buffs/SoulFire");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X 
                        + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, 
                        new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Main.playerDrawDust.Add(dust);
				}
			}

			if (modPlayer.MiningAbilityCooldown)
			{
				Texture2D texture = mod.GetTexture("Buffs/MiningAbilityCooldown");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Terraria.Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X 
                    + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, 
                    new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
				
                Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Main.playerDrawDust.Add(dust);
				}
			}

			if (modPlayer.Electrified)
			{
				Texture2D texture = mod.GetTexture("Buffs/Electrified");

				int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
				int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);

				DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X 
                    + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, 
                    new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);

				Main.playerDrawData.Add(data);

				for (int k = 0; k < 2; k++)
				{
					int dust = Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, ModContent.DustType<Smoke>(), 0f, 0f, 0, Color.Black);
					Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
					Main.playerDrawDust.Add(dust);
				}
			}
		});

        public override void ModifyZoom(ref float zoom)
        {
			if (zooming)
                zoom = 1f;
        }
    }
}
