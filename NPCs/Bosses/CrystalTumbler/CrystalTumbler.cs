using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.CrystalTumbler
{
	[AutoloadBossHead]
    public class CrystalTumbler : ModNPC
    {
        public override void SetDefaults()
        {
            npc.aiStyle = 26;  //5 is the flying AI
            npc.lifeMax = 4800;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 24;    //boss defense
            npc.knockBackResist = 0f;
            npc.width = 116;
            npc.height = 120; 
            npc.value = Item.buyPrice(0, 40, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;  
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
	        npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
        }
		public override void AI()
		{
			npc.rotation += npc.velocity.X * 0.05f;
			{
				int attackcounter = npc.ai[0];
				attackcounter++;
				if (attackcounter == //insert time here)
				{
					var posArray = new Vector2[num];
					float spread = (float)(angle * 1);
					float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
					double baseAngle = System.Math.Atan2(speedX, speedY);
					double randomAngle;
						for (int i = 0; i < num; ++i)
						{
							randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
							posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
						}
					return (Vector2[])posArray;
				}
	
						public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
				{
					Vector2[] speeds = randomSpread(speedX, speedY, 5, 5);
					for (int i = 0; i < 5; ++i)
					{
						Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
					}
					return false;
				}
			}
		}
	}
}