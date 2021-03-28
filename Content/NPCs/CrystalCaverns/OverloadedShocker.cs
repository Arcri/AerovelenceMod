using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class OverloadedShocker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overloaded Shocker");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.BlueSlime];
            NPCID.Sets.TrailCacheLength[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 0;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            npc.lifeMax = 2000;
            npc.damage = 30;
            npc.defense = 32;
            npc.knockBackResist = 0f;
            animationType = NPCID.BlueSlime;
            npc.width = 70;
            npc.height = 46;
            npc.value = Item.buyPrice(0, 15, 7, 0);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath44;    
        }

        private int AI_State = 0;
        private Projectile aura;
        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.player.GetModPlayer<ZonePlayer>().zoneCrystalCaverns && Main.hardMode ? .1f : 0f;

        public override bool PreAI()
        {
            Player target = Main.player[Main.myPlayer];
            if (target.position.X > npc.position.X)
            {
                npc.spriteDirection = 1;
                npc.direction = 1;
            }
            else if (target.position.X < npc.position.X)
            {
                npc.spriteDirection = -1;
                npc.direction = -1;
            }
            if (AI_State == 0)
            {
                if (npc.ai[1] > Main.rand.Next(600, 3000) && Vector2.Distance(target.position, npc.position) < 350)
                {
                    AI_State--;
                    npc.velocity.Y += -15;
                }
            }
            else if (AI_State == 1)
            {
                aura.Center = npc.Center;
                if (npc.ai[1] < 80)
                {
                    npc.ai[1] += 4;
                    for (int i = 0; i < 90; i++)
                    {
                        Vector2 position = npc.position + new Vector2(0, 50) + new Vector2(0f, -(2 * npc.ai[1])).RotatedBy(MathHelper.ToRadians(-90) + MathHelper.ToRadians(360f / 180 * i));
                        Dust.NewDustDirect(position, 1, 1, DustID.Electric, 0, 0, 0, default, 0.5f);
                    }
                }
                if (npc.ai[1] == 80)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 vector2 = new Vector2(i - 2, -4f);
                        vector2.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector2.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector2.Normalize();
                        vector2 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                        Projectile.NewProjectile(npc.position.X, npc.position.Y, vector2.X, vector2.Y, ModContent.ProjectileType<LuminoShard>(), npc.damage, 0f);
                    }
                }
                if (npc.ai[1] > 600)
                {
                    AI_State--;
                    npc.ai[1] = 0;
                    aura.active = false;
                }
            }
            npc.ai[0]+=2;
            npc.ai[1]++;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (AI_State == -1)
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, (npc.height / Main.npcFrameCount[npc.type]) * 0.5f);
                for (int i = 0; i < npc.oldPos.Length; i++)
                {
                    Vector2 drawPos = npc.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, npc.gfxOffY);
                    Color color = npc.GetAlpha(Color.White) * ((float)(npc.oldPos.Length - i) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, new Rectangle?(npc.frame), color, npc.rotation, drawOrigin, npc.scale, SpriteEffects.None, 0f);
                }
                if (npc.collideY)
                {   
                    aura = Projectile.NewProjectileDirect(npc.position, new Vector2(2, 2), ModContent.ProjectileType<EnergyAura>(), npc.damage, 0f);
                    AI_State += 2;
                    npc.ai[1] = 0;
                }
            }      
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 || npc.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(npc.position, npc.width, npc.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                }
            }
        }
    }  
}

namespace AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns
{
    public class EnergyAura : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 180;
            projectile.height = 180;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
            projectile.timeLeft = 10;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            for (int i = 0; i < 90; i++)
            {
                Vector2 position = projectile.Center + new Vector2(0f, -100).RotatedBy(MathHelper.ToRadians(90 - 360f / 90 * i));
                if (!Collision.SolidCollision(position, 1, 1))
                {
                    Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.Electric, 0, 0, 128, default, 0.5f);
                    dust.velocity.X *= 0.5f;
                    dust.scale *= 0.99f;
                    dust.velocity.Y *= 0.1f;
                    dust.noGravity = true;
                }
            }
            projectile.timeLeft += 2;
            projectile.velocity.Y += 0.2f;
        }

        public override string Texture => "Terraria/Projectile_"+ ProjectileID.None;
    }
}