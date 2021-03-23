using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.VoidReaver
{
    public class VoidReaver : ModNPC
    {
        public override void AI()
        {
			{
				if (npc.ai[3] > 0f)
				{
					npc.realLife = (int)npc.ai[3];
				}
				if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				{
					npc.TargetClosest();
				}
				if (npc.type >= 134 && npc.type <= 136)
				{
					npc.velocity.Length();
					if (npc.type == 134 || (npc.type != 134 && Main.npc[(int)npc.ai[1]].alpha < 128))
					{
						if (npc.alpha != 0)
						{
							for (int i = 0; i < 2; i++)
							{
								int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
								Main.dust[num].noGravity = true;
								Main.dust[num].noLight = true;
							}
						}
						npc.alpha -= 42;
						if (npc.alpha < 0)
						{
							npc.alpha = 0;
						}
					}
				}
				if (npc.type > 134)
				{
					bool flag = false;
					if (npc.ai[1] <= 0f)
					{
						flag = true;
					}
					else if (Main.npc[(int)npc.ai[1]].life <= 0)
					{
						flag = true;
					}
					if (flag)
					{
						npc.life = 0;
						npc.HitEffect();
						npc.checkDead();
					}
				}
				if (Main.netMode != 1)
				{
					if (npc.ai[0] == 0f && npc.type == 134)
					{
						npc.ai[3] = npc.whoAmI;
						npc.realLife = npc.whoAmI;
						int num2 = 0;
						int num3 = npc.whoAmI;
						int num4 = 80;
						for (int j = 0; j <= num4; j++)
						{
							int num5 = 135;
							if (j == num4)
							{
								num5 = 136;
							}
							num2 = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), num5, npc.whoAmI);
							Main.npc[num2].ai[3] = npc.whoAmI;
							Main.npc[num2].realLife = npc.whoAmI;
							Main.npc[num2].ai[1] = num3;
							Main.npc[num3].ai[0] = num2;
							NetMessage.SendData(23, -1, -1, null, num2);
							num3 = num2;
						}
					}
					if (npc.type == 135)
					{
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
				if (!flag2)
				{
					if (npc.type != 135 || npc.ai[2] != 1f)
					{
						Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.3f, 0.1f, 0.05f);
					}
					npc.localAI[1] = 1f;
					if (npc.type == 134)
					{
						Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
						int num16 = 1000;
						bool flag3 = true;
						if (npc.position.Y > Main.player[npc.target].position.Y)
						{
							for (int m = 0; m < 255; m++)
							{
								if (Main.player[m].active)
								{
									Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, num16 * 2);
									if (rectangle.Intersects(rectangle2))
									{
										flag3 = false;
										break;
									}
								}
							}
							if (flag3)
							{
								flag2 = true;
							}
						}
					}
				}
				else
				{
					npc.localAI[1] = 0f;
				}
				float num17 = 16f;
				if (Main.dayTime || Main.player[npc.target].dead)
				{
					flag2 = false;
					npc.velocity.Y += 1f;
					if (npc.position.Y > Main.worldSurface * 16.0)
					{
						npc.velocity.Y += 1f;
						num17 = 32f;
					}
					if (npc.position.Y > Main.rockLayer * 16.0)
					{
						for (int n = 0; n < 200; n++)
						{
							if (Main.npc[n].aiStyle == npc.aiStyle)
							{
								Main.npc[n].active = false;
							}
						}
					}
				}
				float num18 = 0.1f;
				float num19 = 0.15f;
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
				if (!flag2)
				{
					npc.TargetClosest();
					npc.velocity.Y += 0.15f;
					if (npc.velocity.Y > num17)
					{
						npc.velocity.Y = num17;
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num17 * 0.4)
					{
						if (npc.velocity.X < 0f)
						{
							npc.velocity.X -= num18 * 1.1f;
						}
						else
						{
							npc.velocity.X += num18 * 1.1f;
						}
					}
					else if (npc.velocity.Y == num17)
					{
						if (npc.velocity.X < num20)
						{
							npc.velocity.X += num18;
						}
						else if (npc.velocity.X > num20)
						{
							npc.velocity.X -= num18;
						}
					}
					else if (npc.velocity.Y > 4f)
					{
						if (npc.velocity.X < 0f)
						{
							npc.velocity.X += num18 * 0.9f;
						}
						else
						{
							npc.velocity.X -= num18 * 0.9f;
						}
					}
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
					num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
					float num25 = Math.Abs(num20);
					float num26 = Math.Abs(num21);
					float num27 = num17 / num22;
					num20 *= num27;
					num21 *= num27;
					if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
					{
						if (npc.velocity.X < num20)
						{
							npc.velocity.X += num19;
						}
						else if (npc.velocity.X > num20)
						{
							npc.velocity.X -= num19;
						}
						if (npc.velocity.Y < num21)
						{
							npc.velocity.Y += num19;
						}
						else if (npc.velocity.Y > num21)
						{
							npc.velocity.Y -= num19;
						}
					}
					if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
					{
						if (npc.velocity.X < num20)
						{
							npc.velocity.X += num18;
						}
						else if (npc.velocity.X > num20)
						{
							npc.velocity.X -= num18;
						}
						if (npc.velocity.Y < num21)
						{
							npc.velocity.Y += num18;
						}
						else if (npc.velocity.Y > num21)
						{
							npc.velocity.Y -= num18;
						}
						if (Math.Abs(num21) < num17 * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
						{
							if (npc.velocity.Y > 0f)
							{
								npc.velocity.Y += num18 * 2f;
							}
							else
							{
								npc.velocity.Y -= num18 * 2f;
							}
						}
						if (Math.Abs(num20) < num17 * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
						{
							if (npc.velocity.X > 0f)
							{
								npc.velocity.X += num18 * 2f;
							}
							else
							{
								npc.velocity.X -= num18 * 2f;
							}
						}
					}
					else if (num25 > num26)
					{
						if (npc.velocity.X < num20)
						{
							npc.velocity.X += num18 * 1.1f;
						}
						else if (npc.velocity.X > num20)
						{
							npc.velocity.X -= num18 * 1.1f;
						}
						if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num17 * 0.5)
						{
							if (npc.velocity.Y > 0f)
							{
								npc.velocity.Y += num18;
							}
							else
							{
								npc.velocity.Y -= num18;
							}
						}
					}
					else
					{
						if (npc.velocity.Y < num21)
						{
							npc.velocity.Y += num18 * 1.1f;
						}
						else if (npc.velocity.Y > num21)
						{
							npc.velocity.Y -= num18 * 1.1f;
						}
						if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num17 * 0.5)
						{
							if (npc.velocity.X > 0f)
							{
								npc.velocity.X += num18;
							}
							else
							{
								npc.velocity.X -= num18;
							}
						}
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