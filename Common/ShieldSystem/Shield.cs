using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Common.ShieldSystem
{
	// GlobalItem shield class to manage prefixes and shield values
	public class Shield : GlobalItem
	{
		public float capacity;
		public float rechargeRate;
		public float rechargeDelay;
		public float size = 1f;
		public ShieldTypes type = ShieldTypes.Count;
		public int projectileType = ModContent.ProjectileType<ShieldProjectile>();

		internal int capacityBoost;
		internal float rechargeRateBoost;
		internal float rechargeDelayBoost;
		internal float sizeBoost;

		public byte prefix;
		public int prefixSign;
		public override bool InstancePerEntity => true;
		public override GlobalItem Clone(Item item, Item itemClone)
		{
			Shield myClone = (Shield)base.Clone(item, itemClone);
			myClone.capacity = capacity;
			myClone.rechargeRate = rechargeRate;
			myClone.rechargeDelay = rechargeDelay;
			myClone.size = size;
			myClone.type = type;
			myClone.projectileType = projectileType;
			myClone.capacityBoost = capacityBoost;
			myClone.rechargeRateBoost = rechargeRateBoost;
			myClone.rechargeDelayBoost = rechargeDelayBoost;
			myClone.sizeBoost = sizeBoost;
			myClone.prefix = prefix;
			myClone.prefixSign = prefixSign;

			return myClone;
		}
		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (item.ModItem != null && item.ModItem.GetType().IsSubclassOf(typeof(ShieldItem)))
				return rand.Next(ShieldPrefixes.shieldPrefixes);
			return -1;
		} 
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.ModItem != null && item.ModItem.GetType().IsSubclassOf(typeof(ShieldItem)))
			{

				TooltipLine tooltip = tooltips.Find(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Tooltip0"));
				if (tooltip != null)
				{
					tooltip.Text = $"{capacity+capacityBoost} shield capacity";
				}
			    tooltip = tooltips.Find(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Tooltip1"));
				if (tooltip != null)
				{
					tooltip.Text = $"{rechargeRate + rechargeRateBoost} shield recharge rate";
				}
				tooltip = tooltips.Find(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Tooltip2"));
				if (tooltip != null)
				{
					tooltip.Text = $"{rechargeDelay - rechargeDelayBoost}s shield recharge delay";
				}

				TooltipLine line;
				if (capacityBoost!=0)
				{
					line = new TooltipLine(Mod, "ShieldCapacityPrefix", $"{(prefixSign > 0 ? '+' : '-')}" + prefix + "% capacity")
					{
						IsModifier = true
					};
					line.OverrideColor =  prefixSign > 0 ? Color.LightGreen : Color.Red;

					tooltips.Add(line);
				}
				else if (rechargeRateBoost != 0)
				{
					line = new TooltipLine(Mod, "ShieldRechargeRatePrefix", $"{(prefixSign > 0 ? '+' : '-')}" + prefix + " rechargeRate")
					{
						IsModifier = true
					};
					line.OverrideColor = prefixSign > 0 ? Color.LightGreen : Color.Red;
					tooltips.Add(line);
				}
				else if (rechargeDelayBoost != 0)
				{
					line = new TooltipLine(Mod, "ShieldRechargeDelayPrefix", $"{(prefixSign > 0 ? '-' : '+')}" + prefix + " delay")
					{
						IsModifier = true
					};
					line.OverrideColor = prefixSign > 0 ? Color.LightGreen : Color.Red;
					tooltips.Add(line);
				}
				else if (sizeBoost != 0)
				{
					line = new TooltipLine(Mod, "ShieldSizePrefix", $"{(prefixSign > 0 ? '+' : '-')}" + prefix + " size")
					{
						IsModifier = true
					};
					line.OverrideColor = prefixSign > 0 ? Color.LightGreen : Color.Red;
					tooltips.Add(line);
				}
			}
		}
	}
	
	// Actual shield class to be extended for each shield accessory
	public abstract class ShieldItem : ModItem
    {
		public Shield shield = new Shield();
		private Projectile projectile;
		private int counter = 0;
		public static bool rechargeCapacity;
		public override void SetDefaults()
        {          
			shield = Item.GetGlobalItem<Shield>();
		}
        public static bool isItemShield(Item item)
        {
			if (item.ModItem != null && item.ModItem.GetType().IsSubclassOf(typeof(ShieldItem)))
				return true;
			return false;
        }
		public static bool isItemShield(Item item, out Shield shield)
        {
			shield = null;
			if (isItemShield(item))
            {
				shield = item.GetGlobalItem<Shield>();
				return true;
            }
			return false;
        }
        //Method to be reworked when Req finalizes the shield UI
        /*
			public override bool CanEquipAccessory(Player player, int slot)
			{
				for (int i = 0; i < player.armor.Length; i++)
				{
					if (isItemShield(player.armor[i]))
					{
						if (i != slot)
							return false;
					}
				}
				return true;
			}
		*/

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetDefaults(); // The SetDefaults() is here as i love re-logic :heart: 
			AeroPlayer ModPlayer = player.GetModPlayer<AeroPlayer>();
			ModPlayer.ShieldType = shield.type;
			ModPlayer.ShieldOn = true;
			Main.NewTextMultiline("\n\n\nCapacity: " + ModPlayer.ShieldCapacity + "/" + (shield.capacity + shield.capacityBoost) + "\n\nRechargeRate: " + (shield.rechargeRate + shield.rechargeRateBoost) + "\n\nRechargeRate: " + (counter/60) + "/" + (shield.rechargeDelay + shield.rechargeDelayBoost) + "\n\nShieldBroken: "+ModPlayer.ShieldBroken);
			if (projectile == null || (projectile != null && !projectile.active))
			{
				//Avoiding spawning multiple shield projectiles 
				foreach (Projectile projectile in Main.projectile)
                {
					if (projectile.type == ModContent.ProjectileType<ShieldProjectile>())
						projectile.Kill();
                }																		
				projectile =  Projectile.NewProjectileDirect(null, player.Center, Vector2.Zero, shield.projectileType, 20, 0, player.whoAmI);
				ShieldProjectile.sizeBoost = shield.sizeBoost;
			}

			counter++;
			if (counter >= (shield.rechargeDelay+shield.rechargeDelayBoost)*60 || rechargeCapacity)
			{
				counter = 0; rechargeCapacity = false;
				ModPlayer.ShieldCapacity += shield.rechargeRate+shield.rechargeRateBoost;			
			}
			if (ModPlayer.ShieldCapacity >= shield.capacity+shield.capacityBoost)
			{
				ModPlayer.ShieldCapacity = shield.capacity + shield.capacityBoost;
				ModPlayer.ShieldBroken = false;
			}
		}
    }

	// Class for the shield's visual projectile
	public class ShieldProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 84;
			Projectile.height = 84;
			Projectile.friendly = true;
			Projectile.penetrate = 999;
			Projectile.timeLeft = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 0;
		}
		public static float sizeBoost;
		private float angle;             // The current angle of shield position around the player in grades 
		public static float scale = 0.1f;		 // value to increase/decrease periodically the proj scale
		private float increment = 0.01f; 
		public override void AI()
		{
			Player player = Main.player[Main.myPlayer];
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			switch ((int)modPlayer.ShieldType)
			{
				case 0: // General Bubble Shield AI	
					Projectile.Center = player.Center;
					for (int i = 0; i < Main.projectile.Length - 1; ++i)
					{
						if (Main.projectile[i].active && i != Projectile.whoAmI && !Main.projectile[i].friendly && Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox) && !modPlayer.ShieldBroken)
						{
							Main.projectile[i].Kill();
							modPlayer.ShieldCapacity -= 20;
							if (modPlayer.ShieldCapacity <= 0)
								modPlayer.ShieldBroken = true;
						}
					}
					// Implement code when sprites are done
					break;

				case 1: // Gen(shin)eral Impact Shield AI
					int npcDirection = 1;
					for (int k = 0; k < 200; k++)
					{
						if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
						{
							Vector2 delta = Main.npc[k].Center - player.Center;
							Vector2 delta2 = Projectile.Center - player.Center;

							float distanceTo = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
							if (distanceTo < 500f)
							{
								float npcAngle = (float)(Math.Atan2(-delta.Y, delta.X) * 180 / Math.PI);
								npcAngle = (npcAngle < 0) ? (360f + npcAngle) : npcAngle;

								float shieldAngle = (float)(Math.Atan2(-delta2.Y, delta2.X) * 180 / Math.PI);
								shieldAngle = (shieldAngle < 0) ? (360f + shieldAngle) : shieldAngle;

								if(npcAngle>shieldAngle)
                                {
									if (Math.Abs(npcAngle - shieldAngle) < 360 - Math.Abs(npcAngle - shieldAngle))
										npcDirection = -2;
									else
										npcDirection = 2;
								}
								else
                                {
									if (Math.Abs(shieldAngle - npcAngle) < 360 - Math.Abs(shieldAngle - npcAngle))
										npcDirection = 2;
									else
										npcDirection = -2;
								}
							}
						}
					}
					angle += 1f * npcDirection;
					Projectile.ai[1] = (float)Math.Abs((Math.PI * angle / 180.0));
					Projectile.Center = player.Center - new Vector2(-player.width, player.height / 2).RotatedBy(Projectile.ai[1]);
					Projectile.rotation = Projectile.ai[1];
					for (int i = 0; i < Main.projectile.Length - 1; ++i)
					{
						if (Main.projectile[i].active && i != Projectile.whoAmI && !Main.projectile[i].friendly && Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox) && !modPlayer.ShieldBroken && Main.rand.Next(1) == 0)
						{
							Main.projectile[i].Kill();
							modPlayer.ShieldCapacity -= 20;
							if (modPlayer.ShieldCapacity <= 0)
								modPlayer.ShieldBroken = true;							
						}
					}
					break;

				case 2: // General Nova Shield AI	
					Projectile.Center = player.Center + new Vector2(0,5);
					for (int i = 0; i < Main.projectile.Length - 1; ++i)
					{
						if (Main.projectile[i].active && i != Projectile.whoAmI && !Main.projectile[i].friendly && Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox) && !modPlayer.ShieldBroken)
						{
							Main.projectile[i].Kill();
							modPlayer.ShieldCapacity -= 20;
							if (modPlayer.ShieldCapacity <= 0)
								NovaExplosionEffect();
						}
					}
					break;

				default:
					break;
			}
			if (modPlayer.ShieldOn)
				Projectile.timeLeft++;
			if(modPlayer.ShieldBroken)
				Projectile.Size = Vector2.Zero;
		}

		//Override this for custom effects
		protected virtual void NovaExplosionEffect() 
		{
			AeroPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<AeroPlayer>();
			Projectile.Size = new Vector2(130, 130);
			Projectile.damage = 300;
			Projectile.position = Main.player[Main.myPlayer].Center;
			int counter = 0;
			while (counter <20)
			{
				for (int j = 0; j < 90; j++)
				{
					Vector2 position = Projectile.position + new Vector2(0f, -(5 * counter)).RotatedBy(MathHelper.ToRadians(360f / 90 * j));
					Dust.NewDustDirect(position, 1, 1, 197, 0, 0, 0, Color.LightCyan, 1f).noGravity = true;
				}
				counter += 5;
			}
			modPlayer.ShieldBroken = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Main.myPlayer];
			AeroPlayer modPlayer = player.GetModPlayer<AeroPlayer>();
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture); 
			bool textureExists = Texture != ("Terraria/Projectile_" + ProjectileID.None) ? true : false;
			player.heldProj = Projectile.whoAmI;
			increment *= scale <= 0f || scale >= 1f ? -1f : 1;
			scale -= increment;
			Projectile.scale = MathHelper.Lerp(1f + sizeBoost, 1.1f + sizeBoost, scale);
			if (!modPlayer.ShieldBroken)
			{
				switch ((int)modPlayer.ShieldType)
				{
					case 0: //Bubble Shield PreDraw [wip]							
						if (!textureExists)
							texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/BubbleShield");
						Projectile.Size = new Vector2(texture.Width, texture.Height) * Projectile.scale;
						Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(), Color.Transparent, Projectile.rotation, Projectile.Size / (2*Projectile.scale), Projectile.scale, SpriteEffects.None, 1);
						break;

					case 1: //Impact Shield PreDraw [wip]
						if (!textureExists)
							texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/ShieldTypeTest");
						if (Projectile.ai[0] > 8)
						{
							Projectile.Size = new Vector2(44, 44) * Projectile.scale;
							Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(1, 3, 0, 2), Color.White, Projectile.rotation, Projectile.Size / (2 * Projectile.scale), Projectile.scale, SpriteEffects.None, 1);
						}
						else if (Projectile.ai[0] > 4)
						{
							Projectile.Size = new Vector2(26, 26) * Projectile.scale;
							Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(1, 3, 0, 1), Color.White, Projectile.rotation, Projectile.Size / (2 * Projectile.scale), Projectile.scale, SpriteEffects.None, 1);
						}
						else
						{
							Projectile.Size = new Vector2(14, 14) * Projectile.scale;
							Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(1, 3, 0, 0), Color.White, Projectile.rotation, Projectile.Size / (2 * Projectile.scale), Projectile.scale, SpriteEffects.None, 1);
						}
						break;

					case 2: //Nova Shield PreDraw [wip]
						if (!textureExists)
							texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Shield/NovaShield");
						Projectile.Size = new Vector2(texture.Width, texture.Height) * Projectile.scale;
						Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Projectile.Size / 2, texture.Frame(), Color.Transparent, Projectile.rotation, Projectile.Size / (2 * Projectile.scale), Projectile.scale, SpriteEffects.None, 1);
						break;
					default:
						break;
				}
			}
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AeroPlayer modplayer = Main.player[Main.myPlayer].GetModPlayer<AeroPlayer>();
			if (!modplayer.ShieldBroken)
			{
				modplayer.ShieldCapacity -= 20;
				if (modplayer.ShieldCapacity <= 0)
				{
					if (modplayer.ShieldType == ShieldTypes.Nova)
						NovaExplosionEffect();
					else
					{
						modplayer.ShieldBroken = true;
						Projectile.Size = Vector2.Zero;
					}
				}
			}
			if (target.life <= 0 && modplayer.ShieldType == ShieldTypes.Impact)
			{
				Projectile.ai[0]++;
				ShieldItem.rechargeCapacity = true;
			}
		}

		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
	}
}
