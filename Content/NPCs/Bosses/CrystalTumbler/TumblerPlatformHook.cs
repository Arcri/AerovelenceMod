using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerPlatformHook : GlobalProjectile
    {
        private int platformIndex = -1;
        private int platformIdentity = -1;
        private int platformOwner = -1;
        private Vector2 offset;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.aiStyle == ProjAIStyleID.Hook;

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.aiStyle != ProjAIStyleID.Hook || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return true;
            Player player = Main.player[projectile.owner];
            if (platformIdentity >= 0 && (platformIndex < 0 || !Main.projectile[platformIndex].active || Main.projectile[platformIndex].identity != platformIdentity || Main.projectile[platformIndex].owner != platformOwner))
            {
                platformIndex = -1;
                foreach (Projectile candidate in Main.ActiveProjectiles)
                {
                    if (TumblerMagneticPlatform.IsArenaPlatform(candidate) && candidate.identity == platformIdentity && candidate.owner == platformOwner)
                    {
                        platformIndex = candidate.whoAmI;
                        break;
                    }
                }
                if (platformIndex < 0)
                {
                    platformIdentity = -1;
                    projectile.ai[0] = 1f;
                    projectile.timeLeft = 3600;
                }
            }
            if (platformIndex < 0 && projectile.ai[0] == 0f)
            {
                foreach (Projectile candidate in Main.ActiveProjectiles)
                {
                    if (!TumblerMagneticPlatform.IsArenaPlatform(candidate) || !((TumblerMagneticPlatform)candidate.ModProjectile).CanStand || !candidate.Hitbox.Intersects(projectile.Hitbox))
                        continue;
                    platformIndex = candidate.whoAmI;
                    platformIdentity = candidate.identity;
                    platformOwner = candidate.owner;
                    offset = projectile.Center - candidate.Center;
                    projectile.ai[0] = 2f;
                    projectile.netUpdate = true;
                    break;
                }
            }
            if (platformIndex < 0)
                return true;
            Projectile platform = Main.projectile[platformIndex];
            if (!TumblerMagneticPlatform.IsArenaPlatform(platform) || platform.identity != platformIdentity || !((TumblerMagneticPlatform)platform.ModProjectile).CanStand || player.dead || player.controlJump || projectile.ai[0] == 1f || Vector2.Distance(player.Center, platform.Center) > 650f)
            {
                platformIndex = -1;
                platformIdentity = -1;
                projectile.ai[0] = 1f;
                projectile.timeLeft = 3600;
                projectile.netUpdate = true;
                return true;
            }
            projectile.Center = platform.Center + offset;
            projectile.velocity = Vector2.Zero;
            projectile.timeLeft = 2;
            if (player.grapCount < player.grappling.Length)
                player.grappling[player.grapCount++] = projectile.whoAmI;
            return false;
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer)
        {
            writer.Write(platformIdentity);
            writer.Write(platformOwner);
            writer.Write(offset.X);
            writer.Write(offset.Y);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader)
        {
            platformIdentity = reader.ReadInt32();
            platformOwner = reader.ReadInt32();
            offset = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            platformIndex = -1;
        }
    }
}
