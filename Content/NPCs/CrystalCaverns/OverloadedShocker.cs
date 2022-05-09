using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Globals.Players;
using AerovelenceMod.Content.Projectiles.NPCs.CrystalCaverns;
using Terraria.GameContent;
using AerovelenceMod.Content.Biomes;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class OverloadedShocker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overloaded Shocker");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            NPC.lifeMax = 2000;
            NPC.damage = 30;
            NPC.defense = 32;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.BlueSlime;
            NPC.width = 70;
            NPC.height = 46;
            NPC.value = Item.buyPrice(0, 15, 7, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath44;    
        }

        private int AI_State = 0;
        private Projectile aura;
        public override float SpawnChance(NPCSpawnInfo spawnInfo) =>
            spawnInfo.Player.InModBiome(ModContent.GetInstance<CrystalCavernsBiome>()) && Main.hardMode ? .1f : 0f;

        public override bool PreAI()
        {
            Player target = Main.player[Main.myPlayer];
            if (target.position.X > NPC.position.X)
            {
                NPC.spriteDirection = 1;
                NPC.direction = 1;
            }
            else if (target.position.X < NPC.position.X)
            {
                NPC.spriteDirection = -1;
                NPC.direction = -1;
            }
            if (AI_State == 0)
            {
                if (NPC.ai[1] > Main.rand.Next(600, 3000) && Vector2.Distance(target.position, NPC.position) < 350)
                {
                    AI_State--;
                    NPC.velocity.Y += -15;
                }
            }
            else if (AI_State == 1)
            {
                aura.Center = NPC.Center;
                if (NPC.ai[1] < 80)
                {
                    NPC.ai[1] += 4;
                    for (int i = 0; i < 90; i++)
                    {
                        Vector2 position = NPC.position + new Vector2(0, 50) + new Vector2(0f, -(2 * NPC.ai[1])).RotatedBy(MathHelper.ToRadians(-90) + MathHelper.ToRadians(360f / 180 * i));
                        Dust.NewDustDirect(position, 1, 1, DustID.Electric, 0, 0, 0, default, 0.5f);
                    }
                }
                if (NPC.ai[1] == 80)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 vector2 = new Vector2(i - 2, -4f);
                        vector2.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector2.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector2.Normalize();
                        vector2 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position.X, NPC.position.Y, vector2.X, vector2.Y, ModContent.ProjectileType<LuminoShard>(), NPC.damage, 0f);
                    }
                }
                if (NPC.ai[1] > 600)
                {
                    AI_State--;
                    NPC.ai[1] = 0;
                    aura.active = false;
                }
            }
            NPC.ai[0]+=2;
            NPC.ai[1]++;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AI_State == -1)
            {
                Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, (NPC.height / Main.npcFrameCount[NPC.type]) * 0.5f);
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Vector2 drawPos = NPC.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                    Color color = NPC.GetAlpha(Color.White) * ((float)(NPC.oldPos.Length - i) / (float)NPC.oldPos.Length);
                    Main.EntitySpriteDraw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, new Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (NPC.collideY)
                {   
                    aura = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.position, new Vector2(2, 2), ModContent.ProjectileType<EnergyAura>(), NPC.damage, 0f);
                    AI_State += 2;
                    NPC.ai[1] = 0;
                }
            }      
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0 || NPC.life >= 0)
            {
                int d = 193;
                for (int k = 0; k < 12; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hitDirection, -2.5f, 0, Color.LightBlue, 0.7f);
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
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            for (int i = 0; i < 90; i++)
            {
                Vector2 position = Projectile.Center + new Vector2(0f, -100).RotatedBy(MathHelper.ToRadians(90 - 360f / 90 * i));
                if (!Collision.SolidCollision(position, 1, 1))
                {
                    Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.Electric, 0, 0, 128, default, 0.5f);
                    dust.velocity.X *= 0.5f;
                    dust.scale *= 0.99f;
                    dust.velocity.Y *= 0.1f;
                    dust.noGravity = true;
                }
            }
            Projectile.timeLeft += 2;
            Projectile.velocity.Y += 0.2f;
        }

        public override string Texture => "Terraria/Projectile_"+ ProjectileID.None;
    }
}