using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class PhanticWhip : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PhanticWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<PhanticWhipProjectile>(), 12, 2.5f, 4);
            Item.rare = ItemRarityID.Pink;
            Item.channel = true;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class PhanticWhipDebuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public static readonly int TagDamage = 12;

        public override LocalizedText Description => base.Description.WithFormatArgs(TagDamage);

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PhanticWhipDebuffNPC>().MarkedByPhanticWhip = true;
        }
    }

    public class PhanticWhipDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool MarkedByPhanticWhip;

        public override void ResetEffects(NPC npc)
        {
            MarkedByPhanticWhip = false;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (MarkedByPhanticWhip && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]))
            {
                modifiers.SourceDamage *= 1 + PhanticWhipDebuff.TagDamage / 100f;
                Player owner = Main.player[projectile.owner];
                if (owner.active && !owner.dead)
                    owner.GetModPlayer<PhanticSoulBankPlayer>().AddSouls(projectile.damage / 40f);
            }
        }
    }

    public class PhanticSoulBankPlayer : ModPlayer
    {
        public const int MaxSouls = 7;
        public float StoredSouls = 0;
        public bool ReleaseSoulsOnNextWhip = false;
        private int soulBankEffectTimer = 0;
        private Texture2D meterEmpty;
        private Texture2D meterFill;

        public override void Initialize()
        {
            StoredSouls = 0;
            ReleaseSoulsOnNextWhip = false;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                meterEmpty = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/PhanticWhip_Meter").Value;
                meterFill = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/PhanticWhip_MeterFull").Value;
            }
        }

        public override void UpdateDead()
        {
            StoredSouls = 0;
            ReleaseSoulsOnNextWhip = false;
        }

        public void AddSouls(float amount)
        {
            float previousSouls = StoredSouls;
            StoredSouls = Math.Min(StoredSouls + amount, MaxSouls);
            soulBankEffectTimer = 15;
            if (StoredSouls >= MaxSouls && previousSouls < MaxSouls)
            {
                if (Main.myPlayer == Player.whoAmI)
                    SoundEngine.PlaySound(SoundID.Item29, Player.Center);
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                        Dust gd = Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                        gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                            preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
                    }
                }
            }
        }

        public void ReleaseSouls()
        {
            if (StoredSouls <= 0) return;

            if (Main.myPlayer == Player.whoAmI)
            {
                int soulCount = (int)Math.Ceiling(StoredSouls);
                Vector2 aimDirection = (Main.MouseWorld - Player.MountedCenter).SafeNormalize(Vector2.UnitX);
                float baseDir = aimDirection.ToRotation();
                float spreadAngle = MathHelper.Lerp(MathHelper.Pi / 12, MathHelper.Pi / 4, soulCount / (float)MaxSouls);
                for (int i = 0; i < soulCount; i++)
                {
                    float angle = baseDir;
                    if (soulCount > 1)
                        angle += MathHelper.Lerp(-spreadAngle / 2, spreadAngle / 2, i / (float)(soulCount - 1));

                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (8f + Main.rand.NextFloat(2f));
                    Projectile.NewProjectile(
                        Player.GetSource_ItemUse(Player.HeldItem),
                        Player.MountedCenter,
                        velocity,
                        ModContent.ProjectileType<PhanticSoulProjectile>(),
                        30 + (int)(soulCount * 0.5f),
                        5f,
                        Player.whoAmI
                    );
                }
                SoundEngine.PlaySound(SoundID.NPCDeath52, Player.Center);
            }
            StoredSouls = 0;
            ReleaseSoulsOnNextWhip = false;
        }



        public override void PostUpdate()
        {
            if (soulBankEffectTimer > 0)
                soulBankEffectTimer--;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (StoredSouls > 0 && Main.rand.NextBool(Math.Max(1, 30 - (int)(StoredSouls * 0.5f))))
            {
                Vector2 dustPos = Player.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-40, -20));
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }

        public void DrawSoulBankMeter(SpriteBatch spriteBatch)
        {
            if (!Player.active || Player.dead || StoredSouls <= 0) return;
            if (meterEmpty == null || meterFill == null)
            {
                meterEmpty = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/PhanticWhip_Meter").Value;
                meterFill = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/PhanticWhip_MeterFull").Value;
                if (meterEmpty == null || meterFill == null)
                    return;
            }
            Vector2 worldPos = Player.Top + new Vector2(0, -30);
            Vector2 drawPos = worldPos - Main.screenPosition;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
            float fillAmount = StoredSouls / MaxSouls;
            spriteBatch.Draw(meterEmpty, drawPos, null, Color.White * (0.7f + (soulBankEffectTimer / 15f) * 0.3f), 0f, new Vector2(meterEmpty.Width / 2, meterEmpty.Height / 2), 1f, SpriteEffects.None, 0f);
            if (fillAmount > 0)
            {
                Rectangle fillRect = new Rectangle(0, 0, (int)(meterFill.Width * fillAmount), meterFill.Height);
                float pulseIntensity = fillAmount > 0.9f ? (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f : 0f;
                Color fillColor = Color.White * (0.8f + pulseIntensity + (soulBankEffectTimer / 15f) * 0.2f);
                spriteBatch.Draw(meterFill, drawPos - new Vector2(meterFill.Width / 2, meterFill.Height / 2), fillRect, fillColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            if (StoredSouls >= 1f)
            {
                string soulText = $"{Math.Floor(StoredSouls)}";
                Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(soulText);
                Vector2 textPos = drawPos - new Vector2(textSize.X / 2, 12);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, soulText, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 0.8f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix
            );
        }
    }

    public class PhanticWhipProjectile : ModProjectile
    {
        private List<WhipAfterimageData> afterimages = [];

        private Texture2D glowTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.WhipSettings.Segments = 24;
            Projectile.WhipSettings.RangeMultiplier = 3.8f;
            if (!Main.dedServ)
                glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/PhanticWhipProjectile_GlowBlack").Value;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 mousePos = Main.MouseWorld;
            Vector2 armPosition = Main.GetPlayerArmPosition(Projectile);
            Vector2 direction = mousePos - armPosition;
            direction.Normalize();
            Projectile.rotation = direction.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = armPosition + direction * Timer;
            Projectile.spriteDirection = mousePos.X > owner.MountedCenter.X ? 1 : -1;

            PhanticSoulBankPlayer soulBank = owner.GetModPlayer<PhanticSoulBankPlayer>();
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (soulBank.StoredSouls > 0 && Timer > 0)
            {
                if (Timer >= swingTime * 0.4f && Timer <= swingTime * 0.6f)
                {
                    soulBank.ReleaseSouls();
                }
            }

            if (!Charge(owner))
                return;

            Timer++;
            
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2)
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }

            float swingProgress = Timer / swingTime;
            if (Timer % 3 == 0 && swingProgress > 0.1f && swingProgress < 0.9f)
            {
                List<Vector2> points = new List<Vector2>();
                Projectile.FillWhipControlPoints(Projectile, points);
                afterimages.Add(new WhipAfterimageData
                {
                    Points = new List<Vector2>(points),
                    TimeLeft = 10,
                    Opacity = 0.7f,
                    Color = new Color(255, 50, 100)
                });
                if (afterimages.Count > 5)
                    afterimages.RemoveAt(0);
            }

            for (int i = afterimages.Count - 1; i >= 0; i--)
            {
                afterimages[i].TimeLeft--;
                if (afterimages[i].TimeLeft <= 0)
                {
                    afterimages.RemoveAt(i);
                    continue;
                }
                afterimages[i].Opacity *= 0.8f;
            }

            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(spawnArea.TopLeft(), ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }

        private bool Charge(Player owner)
        {
            if (!owner.channel || ChargeTime >= 80)
                return true;
            ChargeTime++;
            if (ChargeTime % 10 == 0) 
                Projectile.WhipSettings.Segments++;
            Projectile.WhipSettings.RangeMultiplier += 1.2f / 120f;
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;
            Vector2 mousePos = Main.MouseWorld;
            Vector2 armPosition = Main.GetPlayerArmPosition(Projectile);
            Vector2 direction = mousePos - armPosition;
            direction.Normalize();
            Projectile.velocity = direction;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<PhanticWhipDebuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.7f);

            for (int i = 0; i < 10; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }

            SoundEngine.PlaySound(SoundID.Item71.WithPitchOffset(0.2f), target.Center);
        }

        private static void DrawLine(List<Vector2> list, SpriteBatch spriteBatch, Color color)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);
            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color lineColor = Lighting.GetColor(element.ToTileCoordinates(), color);
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                spriteBatch.Draw(texture, pos - Main.screenPosition, frame, lineColor, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (afterimages.Count > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (var afterimage in afterimages)
                {
                    DrawLine(afterimage.Points, Main.spriteBatch, afterimage.Color * afterimage.Opacity * 0.5f);
                    DrawWhipSegments(afterimage.Points, Main.spriteBatch, afterimage.Color * afterimage.Opacity, flip, glowTexture);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            DrawLine(list, Main.spriteBatch, Color.White);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            DrawWhipSegments(list, Main.spriteBatch, lightColor, flip, texture);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawWhipSegments(list, Main.spriteBatch, new Color(255, 50, 100) * 0.8f, flip, glowTexture);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //neccessary because otherwise the player's arm glowssss
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void DrawWhipSegments(List<Vector2> points, SpriteBatch spriteBatch, Color color, SpriteEffects flip, Texture2D texture)
        {
            Vector2 pos = points[0];

            for (int i = 0; i < points.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 10, 26);
                Vector2 origin = new(5, 8);
                float scale = 1;
                if (i == points.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = points[i];
                Vector2 diff = points[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color segmentColor = Lighting.GetColor(element.ToTileCoordinates(), color);

                spriteBatch.Draw(texture, pos - Main.screenPosition, frame, segmentColor, rotation, origin, scale, flip, 0);

                pos += diff;
            }
        }

        private class WhipAfterimageData
        {
            public List<Vector2> Points;
            public float Opacity;
            public int TimeLeft;
            public Color Color;
        }
    }

    public class PhanticSoulProjectile : ModProjectile
    {
        private bool hasHitEnemy = false;
        private float alpha = 0f;
        private float scale = 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }

        public override string Texture => "AerovelenceMod/Content/Items/Sets/Phantic/PhanticSoul";

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 50;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft > 160 && alpha < 1f)
                alpha += 0.1f;
            if (Projectile.timeLeft < 30)
                alpha = Projectile.timeLeft / 30f;

            if (!hasHitEnemy)
                Projectile.velocity *= 0.98f;
            else
                Projectile.velocity = Vector2.Zero;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center - new Vector2(5, 5), ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            Lighting.AddLight(Projectile.Center, 1f * alpha, 0.2f * alpha, 0.4f * alpha);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hasHitEnemy = true;
            Projectile.timeLeft = 30;
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            SoundEngine.PlaySound(SoundID.Item104.WithPitchOffset(0.3f), target.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 drawOrigin = new(texture.Width / 2, frameHeight / 2);
            SpriteEffects spriteEffect = Projectile.velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float trailFactor = i / (float)Projectile.oldPos.Length;
                float opacity = (1f - trailFactor) * 0.6f * alpha;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Color trailColor = new Color(255, 50, 100) * opacity;
                Main.spriteBatch.Draw(texture, trailPos, frame, trailColor, Projectile.oldRot[i], drawOrigin, scale * (1f - trailFactor * 0.5f), spriteEffect, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Color color = new Color(255, 255, 255) * alpha;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation, drawOrigin, scale, spriteEffect, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 150) * alpha;
        }
    }


    public class SoulBankUISystem : ModSystem
    {
        private bool showDebug = false;
        private int debugTimer = 0;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int playerLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Player Health Bars"));
            if (playerLayerIndex == -1)
                playerLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));

            if (playerLayerIndex != -1)
            {
                layers.Insert(playerLayerIndex + 1, new LegacyGameInterfaceLayer(
                    "AerovelenceMod: Soul Bank Meters",
                    delegate {
                        if (!Main.gameMenu && Main.LocalPlayer.active)
                        {
                            var soulBank = Main.LocalPlayer.GetModPlayer<PhanticSoulBankPlayer>();
                            soulBank.DrawSoulBankMeter(Main.spriteBatch);
                            if (showDebug)
                            {
                                debugTimer++;
                                if (debugTimer < 180)
                                {
                                    string debugText = $"Soul Bank: {soulBank.StoredSouls}/{PhanticSoulBankPlayer.MaxSouls}";
                                    Vector2 textSize = FontAssets.MouseText.Value.MeasureString(debugText);
                                    Vector2 textPos = new(Main.screenWidth / 2 - textSize.X / 2, 100);

                                    Utils.DrawBorderStringFourWay(
                                        Main.spriteBatch,
                                        FontAssets.MouseText.Value,
                                        debugText,
                                        textPos.X,
                                        textPos.Y,
                                        Color.Yellow,
                                        Color.Black,
                                        Vector2.Zero,
                                        1f
                                    );
                                }
                                else
                                {
                                    showDebug = false;
                                    debugTimer = 0;
                                }
                            }
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        public void ToggleDebug()
        {
            showDebug = !showDebug;
            debugTimer = 0;
        }
    }
}
