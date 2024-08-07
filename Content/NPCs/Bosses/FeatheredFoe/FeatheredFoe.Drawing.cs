using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Aurora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe
{
    public partial class FeatheredFoe : ModNPC
    {
        Texture2D NPCTexture => (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "FeatheredFoe");
        Texture2D BorderTexture => (Texture2D)ModContent.Request<Texture2D>(AssetDirectory + "FeatheredFoeBorder");


        public float overallAlpha = 1f;
        public float overallScale = 1f;

        float stretchIntensity = 0f;
        float squashAmount = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 origin = NPCTexture.Size() / 2;

            Main.EntitySpriteDraw(NPCTexture, drawPos, null, drawColor * overallAlpha, NPC.rotation, origin, NPC.scale * overallScale, SpriteEffects.None);

            for (int i = 0; i < 5; i++)
            {
                Color col = Color.DeepSkyBlue;
                Main.EntitySpriteDraw(BorderTexture, drawPos + Main.rand.NextVector2Circular(3f, 3f), null, col with { A = 0 } * overallAlpha, NPC.rotation, origin, NPC.scale * overallScale, SpriteEffects.None);
            }


            return false;
        }

    }
}
