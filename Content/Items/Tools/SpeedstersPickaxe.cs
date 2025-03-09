using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using System;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Tools
{
    public class SpeedstersPickaxe : ModItem
    {
        private const int BaseUseTime = 10;
        private const int MinUseTime = 4;
        private const int MaxStacks = 40;
        private const int DecayInterval = 5;

        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = BaseUseTime;
            Item.useAnimation = BaseUseTime;

            Item.pick = 64;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.EarlyPHM;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override void HoldItem(Player player)
        {
            var modPlayer = player.GetModPlayer<SpeedsterPlayer>();
            if (!player.controlUseItem || player.itemAnimation <= 0)
            {
                modPlayer.MiningDecayTimer++;
                if (modPlayer.MiningDecayTimer >= DecayInterval)
                {
                    modPlayer.MiningDecayTimer = 0;
                    modPlayer.MiningStacks--;
                    if (modPlayer.MiningStacks < 0)
                        modPlayer.MiningStacks = 0;
                }
            }

            float fraction = modPlayer.MiningStacks / (float)MaxStacks;
            player.pickSpeed -= 0.01f * modPlayer.MiningStacks;
            int possibleReduction = BaseUseTime - MinUseTime;
            int reduction = (int)(possibleReduction * fraction);
            int newUse = BaseUseTime - reduction;
            if (newUse < MinUseTime) newUse = MinUseTime;
            Item.useTime = newUse;
            Item.useAnimation = newUse * 3;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<SpeedsterPlayer>();
            float fraction = modPlayer.MiningStacks / (float)MaxStacks;
            if (fraction <= 0f)
                return true;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Tools/SpeedstersPickaxe_Glow").Value;
            int sparkCount = (int)(5 + 15 * fraction);
            for (int i = 0; i < sparkCount; i++)
            {
                float colorIntensity = MathHelper.Lerp(0.3f, 0.9f, fraction);
                Color sparkColor = Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, Main.rand.NextFloat()) * colorIntensity;
                sparkColor.A = 0;
                float randomRange = MathHelper.Lerp(1f, 2f, fraction);
                Vector2 randomOffset = Main.rand.NextVector2Circular(randomRange, randomRange);
                float sparkScale = scale * (0.5f + 0.5f * fraction);
                spriteBatch.Draw(glowTexture, position + randomOffset, frame, sparkColor, 0f, origin, sparkScale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class WhyCantIDoThisInAModItemClassGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly) return;
            if (!WorldGen.SolidTile(i, j)) return;
            Player player = Main.LocalPlayer;
            if (player.HeldItem.type == ModContent.ItemType<SpeedstersPickaxe>())
            {
                const int MaxStacks = 40;
                var modPlayer = player.GetModPlayer<SpeedsterPlayer>();
                modPlayer.MiningStacks = Math.Min(modPlayer.MiningStacks + 1, MaxStacks);
                modPlayer.MiningDecayTimer = 0;
            }
        }


    }
    public class SpeedsterPlayer : ModPlayer
    {
        public int MiningStacks = 0;
        public int MiningDecayTimer = 0;
    }

    internal struct AfterimageData(Vector2 pos, float rot, SpriteEffects effects)
    {
        public Vector2 PositionOnScreen = pos;
        public float Rotation = rot;
        public SpriteEffects Effects = effects;
    }

    public class SpeedstersPickaxeDrawLayer : PlayerDrawLayer
    {
        private const int MaxAfterimages = 5;
        private static readonly List<AfterimageData> _trail = [];

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            bool visible =
                p.HeldItem.type == ModContent.ItemType<SpeedstersPickaxe>()
                && p.itemAnimation > 0;
            if (!visible)
                _trail.Clear();
            return visible;
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            if (p.HeldItem.type != ModContent.ItemType<SpeedstersPickaxe>())
                return;

            SpeedsterPlayer modPlayer = p.GetModPlayer<SpeedsterPlayer>();
            float fractionActual = modPlayer.MiningStacks / 20f;
            if (fractionActual <= 0f)
                return;

            float swingProgress = 1f - (p.itemAnimation / (float)p.itemAnimationMax);
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Tools/SpeedstersPickaxe_Glow").Value;
            float realRotation = p.itemRotation + p.fullRotation;
            SpriteEffects effects = drawInfo.playerEffect;
            Vector2 origin = new(0f, texture.Height);
            if (effects.HasFlag(SpriteEffects.FlipHorizontally))
                origin.X = texture.Width;
            if (effects.HasFlag(SpriteEffects.FlipVertically))
                origin.Y = 0f;
            Vector2 screenPos = drawInfo.ItemLocation - Main.screenPosition;
            _trail.Insert(0, new AfterimageData(screenPos, realRotation, effects));
            if (_trail.Count > MaxAfterimages)
                _trail.RemoveAt(_trail.Count - 1);
            int maxSparksAtFullCharge = 10;
            float sparkFactor = swingProgress;
            float chargeFactor = fractionActual;
            int sparkCount = (int)(5 + maxSparksAtFullCharge * chargeFactor * sparkFactor);

            for (int i = _trail.Count - 1; i >= 0; i--)
            {
                float progress = i / (float)_trail.Count;
                AfterimageData data = _trail[i];
                float alpha = (1f - progress) * 0.8f;
                Color trailColor = Color.DeepSkyBlue * alpha;
                trailColor.A = 0;
                Main.EntitySpriteDraw(texture, data.PositionOnScreen, null, trailColor, data.Rotation, origin, 1f, data.Effects, 0);
                if (sparkCount <= 0)
                    continue;
                int perFrameSpark = sparkCount / _trail.Count;
                for (int s = 0; s < perFrameSpark; s++)
                {
                    float fraction = modPlayer.MiningStacks / (float)40;
                    float randomRange = MathHelper.Lerp(1f, 2f, fraction);
                    Vector2 randomOffset = Main.rand.NextVector2Circular(randomRange, randomRange);
                    float sparkIntensity = MathHelper.Lerp(0.4f, 1f, chargeFactor);
                    sparkIntensity *= sparkFactor;
                    Color sparkColor = Color.Lerp(Color.DeepSkyBlue, Color.DarkTurquoise, Main.rand.NextFloat()) * sparkIntensity;
                    sparkColor.A = 0;
                    Main.EntitySpriteDraw(texture, data.PositionOnScreen + randomOffset, null, sparkColor, data.Rotation, origin, 1.1f, data.Effects, 0);
                }
            }
        }
    }
}