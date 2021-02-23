using AerovelenceMod.Items.BossBags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    [AutoloadBossHead]
    public class Snowrium : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 13;    //boss frame/animation 
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;  //5 is the flying AI
            npc.lifeMax = 9500;   //boss life
            npc.damage = 32;  //boss damage
            npc.defense = 9;    //boss defense
            npc.alpha = 0;
            npc.knockBackResist = 0f;
            npc.width = 188;
            npc.height = 182;
            npc.value = Item.buyPrice(0, 5, 75, 45);
            npc.npcSlots = 1f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCHit5;
            npc.buffImmune[24] = true;
            bossBag = ModContent.ItemType<SnowriumBag>();
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Snowrium");
        }
        private enum RimegeistState
        {
            IdleMovement = 0,
            SpiritSouls = 1,
            IcicleDrop = 2,
            AuroraBeams = 3,
            VoidstoneDrop = 4
        }
        private RimegeistState State
        {
            get => (RimegeistState)npc.ai[0];
            set => npc.ai[0] = (float)value;
        }
        private float AttackTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public bool phaseTwo
        {
            get { return npc.life < npc.lifeMax / 2; }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 11500;  //boss life scale in expertmode
            npc.damage = 40;  //boss damage increase in expermode
        }
        public override void AI()
        {

            npc.TargetClosest(true);

            Player player = Main.player[npc.target];
            npc.velocity = (player.Center - npc.Center) / 50;
            if (!player.active || player.dead)
            {
                npc.noTileCollide = true;
                npc.TargetClosest(false);
                npc.velocity.Y = 20f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (State == RimegeistState.IdleMovement)
            {
                if (++AttackTimer >= 250)
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        State = (RimegeistState)Main.rand.Next(1, 5);
                    }

                    npc.netUpdate = true;
                }
            }
            else if (State == RimegeistState.SpiritSouls)
            {
                if (AttackTimer++ == 0)
                {
                    float Speed = 7f;
                    int damage = Main.expertMode ? 15 : 10;
                    int type = mod.ProjectileType("IceBlast");
                    float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                }
                else if (AttackTimer++ == 5)
                {
                    float Speed = 7f;
                    int damage = Main.expertMode ? 15 : 10;
                    int type = mod.ProjectileType("IceBlast");
                    float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);
                }
                else if (AttackTimer++ == 10)
                {
                    float Speed = 7f;
                    int damage = Main.expertMode ? 15 : 10;
                    int type = mod.ProjectileType("IceBlast");
                    float rotation = (float)Math.Atan2(npc.Center.Y - (player.position.Y + (player.height * 0.5f)), npc.Center.X - (player.position.X + (player.width * 0.5f)));
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1), type, damage, 0f, 0);

                    AttackTimer = 0;
                    State = RimegeistState.IdleMovement;
                }
            }
            else if (State == RimegeistState.IcicleDrop)
            {
                if (AttackTimer++ == 0)
                {
                    Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<IceBlast>(), 12, 1f, Main.myPlayer, 0);
                }
                if (AttackTimer++ == 5)
                {
                    Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<IceBlast>(), 12, 1f, Main.myPlayer, 0);
                }
                if (AttackTimer++ == 10)
                {
                    Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<IceBlast>(), 12, 1f, Main.myPlayer, 0);

                    AttackTimer = 0;
                    State = RimegeistState.IdleMovement;
                }
            }
            else if (State == RimegeistState.AuroraBeams)
            {
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
                if (AttackTimer++ == 10)
                {
                    Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<IceBlast>(), 12, 1f, Main.myPlayer, 0);

                    AttackTimer = 0;
                    State = RimegeistState.IdleMovement;
                }
            }
            else if (State == RimegeistState.VoidstoneDrop)
            {
                npc.velocity.Y += 5f;
                if (AttackTimer++ == 10)
                {
                    Projectile.NewProjectile(npc.Center, default, ModContent.ProjectileType<IceBlast>(), 12, 1f, Main.myPlayer, 0);

                    AttackTimer = 0;
                    State = RimegeistState.IdleMovement;
                }
            }
        }
    }
}

    namespace AerovelenceMod.NPCs.Bosses.Snowrium
{

    public class ShiverSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.IceElemental);
            npc.width = 46;
            npc.height = 31;
            npc.damage = 7;
            npc.defense = 2;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.5f;

        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.width = 46;
            npc.height = 31;
            npc.damage = 10;
            npc.defense = 3;
            npc.lifeMax = 175;
            npc.knockBackResist = 0.5f;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustType = DustID.Ice;
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
                dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
    }
}
namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IcySpike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity.Y = projectile.velocity.Y + 0.15f;
            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Terraria.Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
            dust.noGravity = true;


        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/IcySpike_Glowmask");
            Vector2 drawPos = projectile.Center + new Vector2(0, projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size() * 0.5f,
                projectile.scale, SpriteEffects.None, //adjust this according to the sprite
                0f
                );
        }
    }
}


namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceCube : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {

            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
            dust.noGravity = true;


        }
        public override void Kill(int timeLeft)
        {
            int type = ProjectileID.FrostShard;
            float speed = 6f;
            int damage = 10;
            Vector2 position = projectile.Center;
            Vector2 perturbedSpeed = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(0));
            Vector2 perturbedSpeed1 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(45));
            Vector2 perturbedSpeed2 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(90));
            Vector2 perturbedSpeed3 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(135));
            Vector2 perturbedSpeed4 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(180));
            Vector2 perturbedSpeed5 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(225));
            Vector2 perturbedSpeed6 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(270));
            Vector2 perturbedSpeed7 = new Vector2(speed, speed).RotatedBy(MathHelper.ToRadians(315));
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed3.X, perturbedSpeed3.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed4.X, perturbedSpeed4.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed5.X, perturbedSpeed5.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed6.X, perturbedSpeed6.Y, type, damage, 2f);
            Projectile.NewProjectile(position.X, position.Y, perturbedSpeed7.X, perturbedSpeed7.Y, type, damage, 2f);
        }
    }
}
namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 12;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            Dust dust;
            Vector2 position = projectile.Center;
            dust = Main.dust[Terraria.Dust.NewDust(position, projectile.width, projectile.height, DustID.AncientLight, 0f, 0f, 255)];
            dust.noGravity = true;


        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/Snowrium/IceBolt_Glowmask");
            Vector2 drawPos = projectile.Center + new Vector2(0, projectile.gfxOffY) - Main.screenPosition;
            //keep an eye on the width and height when doing this. It matters
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size() * 0.5f,
                projectile.scale,
                SpriteEffects.None, //adjust this according to the sprite
                0f
                );
        }
    }
}



namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class IceBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FrostBlastHostile;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FrostBlastHostile);
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.damage = 25;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            aiType = ProjectileID.FrostBlastHostile;

        }

    }
}

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
    public class SnowriumFlailProjectile : ModProjectile
    {
        NPC npc1;


        private const string ChainTexturePath = "AerovelenceMod/NPCs/Bosses/Snowrium/Projectiles/SnowriumFlailProjectileChain";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowrium Flail");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }
        public override void AI()
        {

            var dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 1.5f);
            dust.noGravity = true;
            dust.velocity /= 2f;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].FullName == "Snowrium")
                {
                    npc1 = Main.npc[i];
                }
            }
            var npc = npc1;
            npc1 = npc;
            if (!npc.active)
            {
                projectile.Kill();
                return;
            }
            int newDirection = projectile.Center.X > npc.Center.X ? 1 : -1;
            projectile.direction = newDirection;

            var vectorToPlayer = npc.Center - projectile.Center;
            float currentChainLength = vectorToPlayer.Length();
            if (projectile.ai[0] == 0f)
            {
                float maxChainLength = 600f;
                projectile.tileCollide = false;

                if (currentChainLength > maxChainLength)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                float elasticFactorA = 10f;
                float elasticFactorB = 10f;
                float maxStretchLength = 600f;

                if (projectile.ai[1] == 1f)
                    projectile.tileCollide = false;

                if (currentChainLength > maxStretchLength || !projectile.tileCollide)
                {
                    projectile.ai[1] = 1f;

                    if (projectile.tileCollide)
                        projectile.netUpdate = true;

                    projectile.tileCollide = false;

                    if (currentChainLength < 20f)
                        projectile.Kill();
                }

                if (!projectile.tileCollide)
                    elasticFactorB *= 2f;

                int restingChainLength = 600;

                if (currentChainLength > restingChainLength || !projectile.tileCollide)
                {
                    var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    projectile.velocity *= 0.98f;
                    projectile.velocity += elasticAcceleration;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
                    {
                        projectile.velocity.X *= 0.96f;
                        projectile.velocity.Y += 0.2f;
                    }
                    if (npc.velocity.X == 0f)
                        projectile.velocity.X *= 0.96f;
                }
            }
            projectile.rotation = vectorToPlayer.ToRotation() - projectile.velocity.X * 0.1f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 1f);
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool shouldMakeSound = false;

            if (oldVelocity.X != projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.X += projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.2f;
            }

            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    shouldMakeSound = true;
                }

                projectile.position.Y += projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }
            projectile.ai[0] = 1f;

            if (shouldMakeSound)
            {
                projectile.netUpdate = true;
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var npc = npc1;

            Vector2 mountedCenter = npc.Center;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToNpc = mountedCenter - drawPosition;

            float rotation = remainingVectorToNpc.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {




                //player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }
            while (true)
            {
                float length = remainingVectorToNpc.Length();
                if (length < 25f || float.IsNaN(length))
                    break;
                drawPosition += remainingVectorToNpc * 12 / length;
                remainingVectorToNpc = mountedCenter - drawPosition;
                Color color = new Color(1f, 1f, 1f, 1f);
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
