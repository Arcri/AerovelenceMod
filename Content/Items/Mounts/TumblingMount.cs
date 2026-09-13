using System.Collections.Generic;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Mounts
{
    public class TumblingMount : ModMount
    {
        protected override string GetExtraTexture(MountTextureType textureType) => textureType == MountTextureType.Front
            ? "AerovelenceMod/Content/NPCs/CrystalCaverns/TumblerockMedium" : null;

        public override void SetStaticDefaults()
        {
            MountData.buff = ModContent.BuffType<TumblingMountBuff>();
            MountData.heightBoost = 32;
            MountData.playerYOffsets = [32, 32];
            MountData.playerHeadOffset = 32;
            MountData.bodyFrame = 0;
            MountData.totalFrames = 2;
            MountData.standingFrameCount = MountData.runningFrameCount = MountData.inAirFrameCount = MountData.flyingFrameCount = MountData.idleFrameCount = MountData.swimFrameCount = 1;
            MountData.standingFrameDelay = MountData.runningFrameDelay = MountData.inAirFrameDelay = MountData.flyingFrameDelay = MountData.idleFrameDelay = MountData.swimFrameDelay = 10;
            MountData.runSpeed = TumblingRampMotion.GroundSpeed;
            MountData.dashSpeed = TumblingRampMotion.GroundSpeed;
            MountData.acceleration = 0.16f;
            MountData.jumpHeight = 12;
            MountData.jumpSpeed = 6f;
            MountData.fallDamage = 0f;
            MountData.spawnDust = DustID.GemSapphire;
            MountData.spawnDustNoGravity = true;
            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.frontTexture.Width();
                MountData.textureHeight = MountData.frontTexture.Height();
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            skipDust = true;
            player.mount._frame = 0;
            player.mount._frameCounter = 0f;
            player.mount._frameExtra = 0;
            player.mount._frameExtraCounter = 0f;
            player.GetModPlayer<TumblingMountPlayer>().ResetRide();
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            skipDust = true;
            player.GetModPlayer<TumblingMountPlayer>().ResetRide();
        }

        public override void UpdateEffects(Player player)
        {
            player.noFallDmg = true;
            Lighting.AddLight(player.Bottom - new Vector2(0f, 16f), new Vector3(0.06f, 0.18f, 0.25f));
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            mountedPlayer.mount._frame = 0;
            mountedPlayer.mount._frameCounter = 0f;
            return false;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            TumblingMountPlayer rider = drawPlayer.GetModPlayer<TumblingMountPlayer>();
            drawPosition = rider.BallCenter - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
            frame = texture.Frame(1, 2, 0, drawPlayer.whoAmI % 2);
            frame.Height = 32;
            drawOrigin = frame.Size() * 0.5f;
            rotation = rider.Rotation;
            spriteEffects = SpriteEffects.None;
            drawScale = 1f;
            if (shadow == 0f && rider.Charge > 0.01f)
            {
                Color color = TumblerVFX.Glow(TumblerVFX.PhaseColor(0f), rider.Charge * 0.2f);
                for (int i = 0; i < 4; i++)
                    playerDrawData.Add(new DrawData(texture, drawPosition + (MathHelper.PiOver2 * i).ToRotationVector2() * 1.5f, frame, color, rotation, drawOrigin, 1.04f, SpriteEffects.None, 0f));
            }
            DrawData body = new(texture, drawPosition, frame, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            body.shader = drawPlayer.cMount;
            playerDrawData.Add(body);
            return false;
        }
    }
}
