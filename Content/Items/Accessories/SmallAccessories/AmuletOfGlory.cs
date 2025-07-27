using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Common.Systems.Language;
using System;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class AmuletOfGlory : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("AmuletOfGlory", "Hitting enemies increases your movement and mining speed\nEnemies nearby chests will emit light")
            .AddName(Language.Spanish, "Amuleto de la Gloria").AddTooltip(Language.Spanish, "Golpear enemigos aumenta tu velocidad de movimiento y minería\nLos enemigos cercanos a cofres emitirán luz")
            .AddName(Language.French, "Amulette de Gloire").AddTooltip(Language.French, "Frapper des ennemis augmente votre vitesse de déplacement et d’extraction\nLes ennemis proches des coffres émettront de la lumière")
            .AddName(Language.German, "Amulett des Ruhms").AddTooltip(Language.German, "Das Treffen von Gegnern erhöht deine Bewegungs- und Bergbaugeschwindigkeit\nFeinde in der Nähe von Truhen leuchten")
            .AddName(Language.Italian, "Amuleto della Gloria").AddTooltip(Language.Italian, "Colpire i nemici aumenta la velocità di movimento e di estrazione\nI nemici vicino ai bauli emetteranno luce")
            //.AddName(Language.Polish, "Amulet Chwały").AddTooltip(Language.Polish, "Trafienie wroga zwiększa twoją prędkość ruchu i wydobycia\nWrogowie w pobliżu skrzyń emitują światło")
            //.AddName(Language.PortugueseBrazil, "Amuleto da Glória").AddTooltip(Language.PortugueseBrazil, "Acertar inimigos aumenta sua velocidade de movimento e mineração\nInimigos próximos a baús emitirão luz")
            .AddName(Language.Russian, "Амулет Славы").AddTooltip(Language.Russian, "Попадание по врагам увеличивает вашу скорость передвижения и добычи\nВраги рядом с сундуками будут излучать свет");
            //.AddName(Language.ChineseTraditional, "榮耀護身符").AddTooltip(Language.ChineseTraditional, "擊中敵人會提高你的移動速度和挖掘速度\n靠近寶箱的敵人會發出光芒")
            //.AddName(Language.ChineseSimplified, "荣耀护身符").AddTooltip(Language.ChineseSimplified, "击中敌人会提高你的移动速度和挖掘速度\n靠近宝箱的敌人会发光");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.EarlyPHM;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AmuletPlayer>().hasAmulet = true;
        }
    }

    public class AmuletPlayer : ModPlayer
    {
        public bool hasAmulet;
        public override void ResetEffects() => hasAmulet = false;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[proj.owner];
            if (hasAmulet)
            {
                player.AddBuff(ModContent.BuffType<Glory>(), 300);
            }
        }
    }

    public class AmuletOfGlorySystem : ModSystem
    {
        private static HashSet<Point> knownChestPositions = [];
        private static Dictionary<int, float> highlightedEnemies = [];
        private static int chestUpdateTimer;
        private const int chestUpdateInterval = 60;
        private const int revealRadius = 20 * 16;
        public static bool TryGetNPCHighlight(int npcWhoAmI, out float intensity) => highlightedEnemies.TryGetValue(npcWhoAmI, out intensity);

        public override void PostUpdateEverything()
        {
            if (Main.gameMenu) { highlightedEnemies.Clear(); return; }
            bool anyAmulet = Main.player.Any(p => p.active && p.GetModPlayer<AmuletPlayer>().hasAmulet);
            if (!anyAmulet) { highlightedEnemies.Clear(); return; }
            UpdateChestCache();
            UpdateEnemyHighlights();
            foreach (var kvp in highlightedEnemies)
            {
                int npcIndex = kvp.Key;
                float intensity = kvp.Value;
                bool exists = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<EnemyGlowEffect>() && (int)proj.ai[0] == npcIndex)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    Projectile.NewProjectile( Main.LocalPlayer.GetSource_FromThis(),
                        position: Main.npc[npcIndex].Center,
                        velocity: Vector2.Zero,
                        Type: ModContent.ProjectileType<EnemyGlowEffect>(),
                        Damage: 0,
                        KnockBack: 0f,
                        Owner: Main.LocalPlayer.whoAmI,
                        ai0: npcIndex,
                        ai1: intensity
                    );

                }
            }
        }

        private static void UpdateChestCache()
        {
            chestUpdateTimer++;
            if (chestUpdateTimer < chestUpdateInterval) return;
            chestUpdateTimer = 0;
            knownChestPositions.Clear();
            Rectangle screenRect = new(
                (int)((Main.screenPosition.X - revealRadius) / 16),
                (int)((Main.screenPosition.Y - revealRadius) / 16),
                (int)(Main.screenWidth / 16) + revealRadius * 2 / 16,
                (int)(Main.screenHeight / 16) + revealRadius * 2 / 16);
            for (int i = screenRect.X; i < screenRect.X + screenRect.Width; i++)
            {
                for (int j = screenRect.Y; j < screenRect.Y + screenRect.Height; j++)
                {
                    if (i < 0 || i >= Main.maxTilesX || j < 0 || j >= Main.maxTilesY)
                        continue;
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.HasTile && (tile.TileType == TileID.Containers || tile.TileType == TileID.Containers2))
                        knownChestPositions.Add(new Point(i, j));
                }
            }
        }

        private static void UpdateEnemyHighlights()
        {
            highlightedEnemies.Clear();
            if (knownChestPositions.Count == 0) return;
            Rectangle screenRect = new(
                (int)(Main.screenPosition.X - revealRadius),
                (int)(Main.screenPosition.Y - revealRadius),
                Main.screenWidth + revealRadius * 2,
                Main.screenHeight + revealRadius * 2);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.townNPC)
                    continue;
                if (!screenRect.Intersects(npc.getRect()))
                    continue;
                float closest = float.MaxValue;
                foreach (Point chest in knownChestPositions)
                {
                    Vector2 chestCenter = chest.ToVector2() * 16f + new Vector2(8f);
                    float d = Vector2.Distance(npc.Center, chestCenter);
                    if (d < closest) closest = d;
                }
                if (closest < revealRadius)
                    highlightedEnemies[npc.whoAmI] = 1f - (closest / revealRadius);
            }
        }
    }

    public class EnemyGlowEffect : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            int npcIndex = (int)Projectile.ai[0];
            if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }
            NPC npc = Main.npc[npcIndex];
            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = npc.Center;
            if (!AmuletOfGlorySystem.TryGetNPCHighlight(npcIndex, out float intensity))
                intensity = 0f;
            if (intensity <= 0f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[1] = intensity;
            Projectile.alpha = (int)(255 * (1f - intensity));
            float brightness = 0.5f * intensity;
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0.4f) * brightness);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Additive,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                Main.GameViewMatrix.TransformationMatrix
            );

            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2f;
            float scale = 0.2f;
            float intensity = Projectile.ai[1];
            Color color = Color.Yellow * intensity * 0.15f;
            spriteBatch.Draw(texture, drawPos, null, color, 0f, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                Main.GameViewMatrix.TransformationMatrix
            );
            return false;
        }
    }
}