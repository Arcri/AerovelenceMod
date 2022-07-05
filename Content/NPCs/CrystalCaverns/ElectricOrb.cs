using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.NPCs.CrystalCaverns
{
    public class ElectricOrb : ModNPC
    {
        public int i;
        public int counter = 0;
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Orb");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.aiStyle = 5;
            NPC.damage = 40;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 10;
            NPC.height = 10;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item10;
            NPC.DeathSound = SoundID.Item10;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 1;
            NPC.damage = 60;
        }


        public override void AI()
        {
            i++;
            Player player = Main.player[NPC.target];
            Vector2 moveTo = player.Center;
            float speed = 5f;
            Vector2 move = moveTo - NPC.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            move *= speed / magnitude;
            if (i % 1 == 0)
            {
                NPC.velocity = move;
            }
            ++NPC.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float piFractionVelocity = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float ReversepiFraction = MathHelper.Pi + oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((NPC.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * NPC.height;
            Dust newDust = Dust.NewDustPerfect(NPC.Center + newDustPosition.RotatedBy(NPC.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(NPC.Center + newDustPosition.RotatedBy(NPC.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            Vector2 newDustPosition2 = new Vector2(0, (float)Math.Sin((NPC.localAI[0] % oneHelixRevolutionInUpdateTicks) * ReversepiFraction)) * NPC.height;
            Dust newDust2 = Dust.NewDustPerfect(NPC.Center + newDustPosition2.RotatedBy(NPC.velocity.ToRotation()), 68);
            newDust2.noGravity = true;
            newDustPosition2.Y *= -1;
            newDust2 = Dust.NewDustPerfect(NPC.Center + newDustPosition2.RotatedBy(NPC.velocity.ToRotation()), 160);
            newDust2.noGravity = true;
            newDust2.velocity *= 0f;
            NPC.rotation += NPC.velocity.Length() * 0.1f * NPC.direction;
            Vector2 Velocity2 = new Vector2(0, (float)Math.Sin(NPC.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * NPC.height;

            Vector2 distanceNorm = player.position - NPC.position;
            distanceNorm.Normalize();
            NPC.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - NPC.position.Y, player.position.X - NPC.position.X) - 3.14159265f;
            NPC.rotation = optimalRotation;
            NPC.TargetClosest(false);

            if (NPC.ai[0] % 5 == 0)
            {
                NPC.ai[1]++;
                if (NPC.ai[1] > 3)
                    NPC.ai[1] = 0;
            }
            NPC.rotation = (NPC.Center - player.Center).ToRotation();
            if (!NPC.noTileCollide)
            {
                if (NPC.collideX)
                {
                    SoundEngine.PlaySound(SoundID.Item10, NPC.position);
                    NPC.active = false;
                }
                if (NPC.collideY)
                {
                    SoundEngine.PlaySound(SoundID.Item10, NPC.position);
                    NPC.active = false;
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
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

