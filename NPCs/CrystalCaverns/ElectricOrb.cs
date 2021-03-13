using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.CrystalCaverns
{
    public class ElectricOrb : ModNPC
    {
        public int i;
        public int counter = 0;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Orb");
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 1;
            npc.aiStyle = 1;
            npc.damage = 40;
            npc.defense = 0;
            npc.knockBackResist = 0f;
            npc.width = 10;
            npc.height = 10;
            npc.value = Item.buyPrice(0, 0, 0, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.Item10;
            npc.DeathSound = SoundID.Item10;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 1;
            npc.damage = 60;
        }


        public override void AI()
        {
            i++;
            Player player = Main.player[npc.target];

            ++npc.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float piFractionVelocity = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            float ReversepiFraction = MathHelper.Pi + oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((npc.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * npc.height;
            Dust newDust = Dust.NewDustPerfect(npc.Center + newDustPosition.RotatedBy(npc.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(npc.Center + newDustPosition.RotatedBy(npc.velocity.ToRotation()), 67);
            newDust.noGravity = true;
            newDust.velocity *= 0f;
            Vector2 newDustPosition2 = new Vector2(0, (float)Math.Sin((npc.localAI[0] % oneHelixRevolutionInUpdateTicks) * ReversepiFraction)) * npc.height;
            Dust newDust2 = Dust.NewDustPerfect(npc.Center + newDustPosition2.RotatedBy(npc.velocity.ToRotation()), 68);
            newDust2.noGravity = true;
            newDustPosition2.Y *= -1;
            newDust2 = Dust.NewDustPerfect(npc.Center + newDustPosition2.RotatedBy(npc.velocity.ToRotation()), 160);
            newDust2.noGravity = true;
            newDust2.velocity *= 0f;
            npc.rotation += npc.velocity.Length() * 0.1f * npc.direction;
            Vector2 Velocity2 = new Vector2(0, (float)Math.Sin(npc.localAI[0] % oneHelixRevolutionInUpdateTicks * piFraction)) * npc.height;

            Vector2 distanceNorm = player.position - npc.position;
            distanceNorm.Normalize();
            npc.ai[0]++;
            float optimalRotation = (float)Math.Atan2(player.position.Y - npc.position.Y, player.position.X - npc.position.X) - 3.14159265f;
            npc.rotation = optimalRotation;
            npc.TargetClosest(false);

            if (npc.ai[0] % 5 == 0)
            {
                npc.ai[1]++;
                if (npc.ai[1] > 3)
                    npc.ai[1] = 0;
            }

            npc.rotation = (npc.Center - player.Center).ToRotation();
            if (!npc.noTileCollide)
            {
                if (npc.collideX)
                {
                    Main.PlaySound(SoundID.Item10, npc.position);
                    npc.active = false;
                }
                if (npc.collideY)
                {
                    Main.PlaySound(SoundID.Item10, npc.position);
                    npc.active = false;
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
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

