using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerPlatformDebris : ModDust
    {
        private static readonly Rectangle[] Frames =
        [
            new(0, 0, 12, 22), new(16, 0, 18, 22), new(38, 0, 26, 22),
            new(68, 0, 20, 22), new(92, 0, 16, 22), new(112, 0, 20, 22),
            new(136, 0, 18, 22), new(158, 0, 12, 22), new(174, 0, 16, 22)
        ];
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Magnetic_Platform_Debris";
        public override void OnSpawn(Dust dust)
        {
            dust.frame = Frames[Main.rand.Next(Frames.Length)];
            dust.noGravity = true;
            dust.noLight = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.fadeIn = Main.rand.NextFloat(-0.1f, 0.1f);
            dust.customData = 0;
        }
        public override bool Update(Dust dust)
        {
            int age = dust.customData is int value ? value + 1 : 1;
            dust.customData = age;
            dust.velocity.Y = Math.Min(10f, dust.velocity.Y + 0.25f);
            dust.velocity.X *= 0.99f;
            dust.position += dust.velocity;
            dust.rotation += dust.fadeIn;
            if (ArenaData.Valid && dust.velocity.Y > 0f && dust.position.Y >= ArenaData.FloorY - 4f)
            {
                dust.position.Y = ArenaData.FloorY - 4f;
                dust.velocity *= new Vector2(0.65f, -0.3f);
                dust.fadeIn *= 0.65f;
            }
            if (age > 60)
                dust.alpha += 6;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = ModContent.Request<Texture2D>(Texture + "_Glowmask", AssetRequestMode.ImmediateLoad).Value;
            float opacity = 1f - dust.alpha / 255f;
            Color light = Lighting.GetColor((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f));
            Vector2 position = dust.position - Main.screenPosition;
            Vector2 origin = dust.frame.Size() * 0.5f;
            Main.spriteBatch.Draw(texture, position, dust.frame, Color.Lerp(light, Color.White, 0.2f) * opacity, dust.rotation, origin, dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mask, position, dust.frame, Color.White * opacity, dust.rotation, origin, dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mask, position, dust.frame, TumblerVFX.Glow(dust.color, opacity * 0.5f), dust.rotation, origin, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
