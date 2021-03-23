using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.VoidReaver
{
    public class VoidReaverTail : ModNPC
    {
        public override void AI()
        {
				if (npc.ai[3] > 0f)
				{
					npc.realLife = (int)npc.ai[3];
				}


						npc.localAI[0] += Main.rand.Next(4);
						if (npc.localAI[0] >= Main.rand.Next(1400, 26000))
						{
							npc.localAI[0] = 0f;
							npc.TargetClosest();
							if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
							{
								Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
								float num6 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector.X + Main.rand.Next(-20, 21);
								float num7 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector.Y + Main.rand.Next(-20, 21);
								float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
								num8 = 8f / num8;
								num6 *= num8;
								num7 *= num8;
								num6 += Main.rand.Next(-20, 21) * 0.05f;
								num7 += Main.rand.Next(-20, 21) * 0.05f;
								int num9 = 22;
								if (Main.expertMode)
								{
									num9 = 18;
								}
								int num10 = 100;
								vector.X += num6 * 5f;
								vector.Y += num7 * 5f;
								int num11 = Projectile.NewProjectile(vector.X, vector.Y, num6, num7, num10, num9, 0f, Main.myPlayer);
								Main.projectile[num11].timeLeft = 300;
								npc.netUpdate = true;
							}
						}
					
				
				int num12 = (int)(npc.position.X / 16f) - 1;
				int num13 = (int)((npc.position.X + npc.width) / 16f) + 2;
				int num14 = (int)(npc.position.Y / 16f) - 1;
				int num15 = (int)((npc.position.Y + npc.height) / 16f) + 2;
				if (num12 < 0)
				{
					num12 = 0;
				}
				if (num13 > Main.maxTilesX)
				{
					num13 = Main.maxTilesX;
				}
				if (num14 < 0)
				{
					num14 = 0;
				}
				if (num15 > Main.maxTilesY)
				{
					num15 = Main.maxTilesY;
				}
				bool flag2 = false;
				if (!flag2)
				{
					Vector2 vector2 = default(Vector2);
					for (int k = num12; k < num13; k++)
					{
						for (int l = num14; l < num15; l++)
						{
							if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[Main.tile[k, l].type] || (Main.tileSolidTop[Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
							{
								vector2.X = k * 16;
								vector2.Y = l * 16;
								if (npc.position.X + npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
				}
				else
				{
					npc.localAI[1] = 0f;
				}
				Vector2 vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num20 = Main.player[npc.target].position.X + Main.player[npc.target].width / 2;
				float num21 = Main.player[npc.target].position.Y + Main.player[npc.target].height / 2;
				num20 = (num20 / 16f) * 16;
				num21 = (num21 / 16f) * 16;
				vector3.X = (vector3.X / 16f) * 16;
				vector3.Y = (vector3.Y / 16f) * 16;
				num20 -= vector3.X;
				num21 -= vector3.Y;
				float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
				if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
				{
					try
					{
						vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
						num20 = Main.npc[(int)npc.ai[1]].position.X + Main.npc[(int)npc.ai[1]].width / 2 - vector3.X;
						num21 = Main.npc[(int)npc.ai[1]].position.Y + Main.npc[(int)npc.ai[1]].height / 2 - vector3.Y;
					}
					catch
					{
					}
					npc.rotation = (float)Math.Atan2(num21, num20) + 1.57f;
					num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
					int num23 = (int)(44f * npc.scale);
					num22 = (num22 - num23) / num22;
					num20 *= num22;
					num21 *= num22;
					npc.velocity = Vector2.Zero;
					npc.position.X += num20;
					npc.position.Y += num21;
					return;
				}
				else
				{
					if (npc.soundDelay == 0)
					{
						float num24 = num22 / 40f;
						if (num24 < 10f)
						{
							num24 = 10f;
						}
						if (num24 > 20f)
						{
							num24 = 20f;
						}
						npc.soundDelay = (int)num24;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y);
					}
				}
				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
				if (npc.type != 134)
				{
					return;
				}
				if (flag2)
				{
					if (npc.localAI[0] != 1f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 0f;
				}
				if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				{
					npc.netUpdate = true;
				}
			}
		}
    }
}