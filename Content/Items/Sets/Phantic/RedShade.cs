using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems.Language;
using Terraria.GameContent.Creative;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class RedShade : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            this.ModifyLocalization("RedShade", "Projectiles home in and possess enemies\nPossessed enemies release smaller copies of the spirit upon death")
            .AddName(Language.Default, "Red Shade").AddTooltip(Language.Default, "Projectiles home in and possess enemies\nPossessed enemies release smaller copies of the spirit upon death");

            //.AddName(Language.Spanish, "").AddSkillStrike(Language.Spanish, "")
            //.AddName(Language.French, "").AddSkillStrike(Language.French, "")
            //.AddName(Language.German, "").AddSkillStrike(Language.German, "")
            //.AddName(Language.Italian, "").AddSkillStrike(Language.Italian, "")
            //.AddName(Language.Polish, "").AddSkillStrike(Language.Polish, "")
            //.AddName(Language.PortugueseBrazil, "").AddSkillStrike(Language.PortugueseBrazil, "")
            //.AddName(Language.Russian, "").AddSkillStrike(Language.Russian, "");
            //.AddName(Language.ChineseTraditional, "").AddSkillStrike(Language.ChineseTraditional, "")
            //.AddName(Language.ChineseSimplified, "").AddSkillStrike(Language.ChineseSimplified, "")
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.sellPrice(silver: 50);

            Item.damage = 15;
            Item.knockBack = 2.5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<RedShadeSoul>();
            Item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddIngredient(ItemID.Book, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 offset = Vector2.Normalize(velocity) * 30f;
            position += offset;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(4));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVel = velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.2f, 0.5f);
                Dust gd = Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            SoundEngine.PlaySound(SoundID.Item104.WithPitchOffset(-0.3f), position);

            return true;
        }
    }

    public class RedShadeSoul : ModProjectile
    {
        private float homingStrength = 0.05f;
        private int homingDelay = 20;
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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 50;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 160 && alpha < 1f)
                alpha += 0.1f;
            if (Projectile.timeLeft < 20)
            {
                alpha = Projectile.timeLeft / 20f;
                scale -= 0.01f;
                if (scale < 0.1f) scale = 0.1f;
            }
            if (++Projectile.frameCounter >= 15)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ > homingDelay)
            {
                float maxHomingRange = 500f;
                NPC target = FindClosestNPC(maxHomingRange);

                if (target != null)
                {
                    Vector2 toTarget = target.Center - Projectile.Center;
                    float distanceToTarget = toTarget.Length();
                    Vector2 moveDirection = toTarget / distanceToTarget;
                    float distanceFactor = 1f - (distanceToTarget / maxHomingRange);
                    distanceFactor = Math.Max(0, distanceFactor);
                    float currentHomingStrength = homingStrength * distanceFactor;
                    if (distanceToTarget < 100f)
                    {
                        float closeRangeFactor = 1f - (distanceToTarget / 100f);
                        currentHomingStrength += homingStrength * closeRangeFactor * 0.5f;
                    }
                    if (distanceToTarget > 350f)
                        currentHomingStrength *= 0.5f;
                    if (currentHomingStrength > 0.01f)
                    {
                        float currentSpeed = Projectile.velocity.Length();
                        Vector2 targetVelocity = moveDirection * currentSpeed;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, currentHomingStrength);
                    }
                }
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            Lighting.AddLight(Projectile.Center, 0.8f * alpha, 0.1f * alpha, 0.3f * alpha);
        }

        private NPC FindClosestNPC(float maxDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDistance;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && !npc.immortal)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RedShadeCurse>(), 240);
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
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
                Color trailColor = new Color(200, 30, 70) * opacity;
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

    public class RedShadeCurse : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.CursedInferno;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(4))
            {
                Vector2 position = npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2);
                Dust dust = Dust.NewDustDirect(position, 4, 4, DustID.PinkTorch, 0f, -1f, 0, new Color(200, 30, 70), 0.5f);
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
            npc.GetGlobalNPC<RedShadeCurseNPC>().CursedByRedShade = true;
        }
    }

    public class RedShadeCurseNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool CursedByRedShade = false;

        public override void ResetEffects(NPC npc)
        {
            CursedByRedShade = false;
        }

        public override void OnKill(NPC npc)
        {
            if (CursedByRedShade)
            {
                int soulCount = Main.rand.Next(1, 4);
                for (int i = 0; i < soulCount; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                    Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, velocity, ModContent.ProjectileType<MiniRedShadeSoul>(), (int)(npc.damage * 0.5f), 2f, Main.myPlayer);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                    Dust gd = Dust.NewDustPerfect(npc.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                    gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                        preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
                }
                SoundEngine.PlaySound(SoundID.NPCDeath52.WithPitchOffset(0.3f), npc.Center);
            }
        }
    }

    public class MiniRedShadeSoul : ModProjectile
    {
        private float baseHomingStrength = 0.16f;
        private int homingDelay = 10;
        private float alpha = 0f;
        private float scale = 1f;
        private float maxHomingRange = 400f;
        private float closeHomingRange = 120f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }

        public override string Texture => "AerovelenceMod/Content/Items/Sets/Phantic/MiniPhanticSoul";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 50;
            Projectile.light = 0.3f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 100 && alpha < 1f)
                alpha += 0.2f;
            if (Projectile.timeLeft < 20)
            {
                alpha = Projectile.timeLeft / 20f;
                scale -= 0.01f;
                if (scale < 0.1f) scale = 0.1f;
            }
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ > homingDelay)
            {
                NPC target = FindClosestNPC(maxHomingRange);
                if (target != null)
                {
                    Vector2 toTarget = target.Center - Projectile.Center;
                    float distanceToTarget = toTarget.Length();
                    Vector2 moveDirection = toTarget / distanceToTarget;
                    float currentHomingStrength = 0f;
                    if (distanceToTarget < closeHomingRange)
                    {
                        float closeFactor = 1f - (distanceToTarget / closeHomingRange);
                        currentHomingStrength = baseHomingStrength * (1f + closeFactor * 0.8f);
                    }
                    else if (distanceToTarget < maxHomingRange * 0.6f)
                    {
                        currentHomingStrength = baseHomingStrength;
                    }
                    else
                    {
                        float distanceFactor = 1f - ((distanceToTarget - maxHomingRange * 0.6f) / (maxHomingRange * 0.4f));
                        currentHomingStrength = baseHomingStrength * distanceFactor * 0.7f;
                    }
                    if (currentHomingStrength > 0.01f)
                    {
                        float currentSpeed = Projectile.velocity.Length();
                        float targetSpeed = currentSpeed;
                        if (distanceToTarget < closeHomingRange)
                            targetSpeed *= 1.1f;
                        Vector2 targetVelocity = moveDirection * targetSpeed;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, currentHomingStrength);
                    }
                }
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.3f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            Lighting.AddLight(Projectile.Center, 0.6f * alpha, 0.1f * alpha, 0.3f * alpha);
        }

        private NPC FindClosestNPC(float maxDistance)
        {
            NPC closestNPC = null;
            float closestDistance = maxDistance;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && !npc.immortal)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            SoundEngine.PlaySound(SoundID.Item104.WithPitchOffset(0.5f), target.Center);
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
                float opacity = (1f - trailFactor) * 0.4f * alpha;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                Color trailColor = new Color(200, 30, 70) * opacity;
                Main.spriteBatch.Draw(texture, trailPos, frame, trailColor, Projectile.oldRot[i], drawOrigin, scale * (1f - trailFactor * 0.3f), spriteEffect, 0f);
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
}