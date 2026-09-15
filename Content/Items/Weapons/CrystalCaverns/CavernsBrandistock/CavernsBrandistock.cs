using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class CavernsBrandistock : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/CavernsBrandistock/CavernsBrandistock";
        private const string EnglishTooltip = "Successful thrusts build blade stress\nAt 40% stress, three hidden crystal blades extend and Skill Strike\nAt maximum stress, the blades shatter and retract for 4 seconds\nYou can still thrust in staff mode while the mechanism recovers\nPause between attacks to let the stress fall";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Cavern's Brandistock", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Hit with the crystal blades fully extended")
                .AddName(Language.Spanish, "Brandistock de las Cavernas")
                .AddTooltip(Language.Spanish, "Los impactos acumulan tensión en las hojas\nAl 40% de tensión, tres hojas de cristal se extienden y asestan Golpes de Habilidad\nAl alcanzar el máximo, las hojas se rompen y retraen durante 4 segundos\nPuedes seguir atacando en modo bastón mientras se recupera el mecanismo\nHaz pausas entre ataques para reducir la tensión")
                .AddSkillStrike(Language.Spanish, "Golpea con las hojas de cristal completamente extendidas");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 58;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrandistockThrust>();
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mechanism = player.GetModPlayer<BrandistockPlayer>();
            Projectile.NewProjectile(source, player.MountedCenter, velocity.SafeNormalize(Vector2.UnitX * player.direction), type,
                damage, knockback, player.whoAmI, 0f, Math.Max(12, player.itemAnimationMax), mechanism.Mode);
            return false;
        }


        public override void AddRecipes() => CreateRecipe().AddIngredient<ChargedStoneItem>(20).AddIngredient<CavernCrystalItem>(12).AddRecipeGroup(RecipeGroupID.IronBar, 8).AddTile(TileID.Anvils).Register();
    }

    internal static class BrandistockArt
    {
        internal const string ShaftTexture = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/CavernsBrandistock/CavernsBrandistockBase";
        internal static float TipReach(float extension) => Math.Max(15f, 9.4f + 27.2f * MathHelper.SmoothStep(0f, 1f, extension));
        internal static void Draw(SpriteBatch spriteBatch, Vector2 socket, float angle, float scale, float extension, float flash, Color light, float opacity)
        {
            Texture2D shaft = ModContent.Request<Texture2D>(ShaftTexture).Value;
            Vector2 origin = new(60f, 22f);
            float rotation = angle + MathHelper.PiOver4;
            float size = scale * 1.2f;
            for (int i = 0; i < 3; i++)
            {
                Texture2D fork = ModContent.Request<Texture2D>(ShaftTexture.Replace("Base", "Fork" + (i + 1))).Value;
                Vector2 root = i == 0 ? new Vector2(56f, 18f) : i == 1 ? new Vector2(65f, 16f) : new Vector2(64f, 26f);
                Vector2 point = socket + ((root - origin) * size).RotatedBy(rotation);
                float growth = MathHelper.SmoothStep(0f, 1f, extension);
                float fade = MathHelper.Clamp(extension * 4f, 0f, 1f) * opacity;
                spriteBatch.Draw(fork, point, null, Color.Lerp(light, Color.White, 0.8f) * fade, rotation, root, size * growth, SpriteEffects.None, 0f);
                spriteBatch.Draw(fork, point, null, CavernsBrandistockVFX.Additive(CavernsBrandistockVFX.Aqua, fade * (0.25f + flash * 0.6f)), rotation, root, size * growth, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(shaft, socket, null, light * opacity, rotation, origin, size, SpriteEffects.None, 0f);
            Texture2D glow = ModContent.Request<Texture2D>(ShaftTexture.Replace("Base", "Held_Glowmask")).Value;
            float pulse = 0.6f + 0.15f * MathF.Sin(Main.GlobalTimeWrappedHourly * 4f);
            spriteBatch.Draw(glow, socket, null, Color.White * opacity * (pulse + extension * 0.25f), rotation, origin, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, socket, null, CavernsBrandistockVFX.Additive(Color.White, opacity * (extension * 0.2f + flash * 0.6f)), rotation, origin, size, SpriteEffects.None, 0f);
        }
    }

    public class BrandistockPlayer : ModPlayer
    {
        private readonly BrandistockMechanism mechanism = new();
        public float Stress => mechanism.Stress;
        public int Cooldown => mechanism.Cooldown;
        public bool Blades => mechanism.Blades;
        internal int Mode => Cooldown > 0 ? 2 : Blades ? 1 : 0;
        internal float MeterOpacity;
        internal float MeterStress;
        internal float MeterCrystalGrowth;
        internal float Flash;
        public override void UpdateDead()
        {
            mechanism.Reset();
            MeterOpacity = MeterStress = MeterCrystalGrowth = Flash = 0f;
        }
        public override void PostUpdate()
        {
            bool held = Player.HeldItem.type == ModContent.ItemType<CavernsBrandistock>();
            BrandistockChange change = mechanism.Tick();
            if (held && change != BrandistockChange.None)
                React(change, Player.MountedCenter);
            MeterOpacity = MathHelper.Lerp(MeterOpacity, held && (Stress > 0f || Flash > 0.05f) ? 1f : 0f, 0.16f);
            MeterStress = MathHelper.Lerp(MeterStress, Stress, 0.3f);
            MeterCrystalGrowth = Math.Clamp(MeterCrystalGrowth + (Blades ? 0.1f : -0.14f), 0f, 1f);
            Flash *= 0.88f;
        }
        internal void Strike(Vector2 point)
        {
            BrandistockChange change = mechanism.Strike();
            if (change != BrandistockChange.None)
                React(change, point);
        }
        private void React(BrandistockChange change, Vector2 point)
        {
            if (Main.dedServ)
                return;
            Flash = 1f;
            if (change == BrandistockChange.Overload)
            {
                CavernsBrandistockVFX.Burst(point, 18, 4.5f);
                CavernsBrandistockVFX.Rubble(point, 5, 3f);
                CavernsBrandistockVFX.Smoke(point, -Vector2.UnitY, 70f, CavernsBrandistockVFX.Violet);
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.45f, Pitch = 0.15f }, point);
            }
            else if (change == BrandistockChange.Extend)
            {
                CavernsBrandistockVFX.Burst(point, 9, 2.5f);
                SoundEngine.PlaySound(SoundID.Item37 with { Volume = 0.35f, Pitch = 0.45f }, point);
            }
            else if (change == BrandistockChange.Ready)
            {
                CavernsBrandistockVFX.Burst(point, 6, 1.5f);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.25f, Pitch = 0.3f }, point);
            }
            else
            {
                CavernsBrandistockVFX.Burst(point, 3, 1f);
                SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.15f, Pitch = 0.3f }, point);
            }
        }
    }

    public class BrandistockMeter : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var state = player.GetModPlayer<BrandistockPlayer>();
            if (player.whoAmI != Main.myPlayer || player.dead || drawInfo.shadow != 0f || state.MeterOpacity < 0.01f)
                return;
            Texture2D meter = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/CrystalCaverns/CavernsBrandistock/CavernsBrandistockBar").Value;
            const int frameWidth = 94;
            const int frameHeight = 30;
            const int stride = 32;
            Vector2 center = drawInfo.Position + new Vector2(player.width * 0.5f, -33f) - Main.screenPosition;
            Vector2 origin = new(frameWidth * 0.5f, frameHeight * 0.5f);
            int background = state.Cooldown > 0 ? 4 : 3;
            Rectangle frame = new(0, background * stride, frameWidth, frameHeight);
            drawInfo.DrawDataCache.Add(new DrawData(meter, center, frame, Color.White * state.MeterOpacity, 0f, origin, 1f, SpriteEffects.None));
            if (state.Cooldown == 0)
            {
                const int fillLeft = 14;
                const int fillWidth = 60;
                int width = Math.Clamp((int)(fillWidth * state.MeterStress / 100f), 0, fillWidth);
                if (width > 0)
                {
                    Rectangle fill = new(fillLeft, 2 * stride, width, frameHeight);
                    Vector2 fillOrigin = origin - new Vector2(fillLeft, 0f);
                    drawInfo.DrawDataCache.Add(new DrawData(meter, center, fill, Color.White * state.MeterOpacity, 0f, fillOrigin, 1f, SpriteEffects.None));
                    float warning = MathHelper.Clamp((state.Stress - 80f) / 20f, 0f, 1f) * (0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.5f);
                    drawInfo.DrawDataCache.Add(new DrawData(meter, center, fill, CavernsBrandistockVFX.Additive(Color.White, state.MeterOpacity * (state.Flash * 0.35f + warning * 0.2f)), 0f, fillOrigin, 1f, SpriteEffects.None));
                }
            }
            if (state.MeterCrystalGrowth > 0f && state.Cooldown == 0)
            {
                float growth = MathHelper.SmoothStep(0f, 1f, state.MeterCrystalGrowth);
                Rectangle crystals = new(72, 0, 22, frameHeight);
                Vector2 root = center + new Vector2(72f - origin.X, 0f);
                Vector2 crystalOrigin = new(0f, origin.Y);
                Vector2 scale = new(growth, growth);
                drawInfo.DrawDataCache.Add(new DrawData(meter, root, crystals, Color.White * state.MeterOpacity, 0f, crystalOrigin, scale, SpriteEffects.None));
                float pulse = 0.1f + 0.06f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
                drawInfo.DrawDataCache.Add(new DrawData(meter, root, crystals, CavernsBrandistockVFX.Additive(Color.Cyan, state.MeterOpacity * growth * (pulse + state.Flash * 0.3f)), 0f, crystalOrigin, scale, SpriteEffects.None));
            }
        }
    }

    public class BrandistockThrust : ModProjectile
    {
        private float extension;
        private float previousReach;
        private float glow;
        private bool initialized;
        private int previousMode;
        private Vector2 Axis => Projectile.velocity.SafeNormalize(Vector2.UnitX);
        private float Progress => Projectile.ai[0] / Math.Max(12f, Projectile.ai[1]);
        private Vector2 Hand => Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter);
        private float ItemScale => Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
        public override string Texture => BrandistockArt.ShaftTexture;
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 140;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => BrandistockMechanism.CanHit(Progress) ? null : false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<CavernsBrandistock>())
            {
                Projectile.Kill();
                return;
            }
            if (!initialized)
            {
                initialized = true;
                extension = Projectile.ai[2] == 1f ? 1f : 0f;
                previousMode = (int)Projectile.ai[2];
            }
            if (Projectile.owner == Main.myPlayer)
                UpdateMode(player.GetModPlayer<BrandistockPlayer>().Mode);
            float target = Projectile.ai[2] == 1f ? 1f : 0f;
            extension = MathHelper.Clamp(extension + Math.Sign(target - extension) * 0.2f, 0f, 1f);
            if (previousMode != (int)Projectile.ai[2])
            {
                glow = 1f;
                if (Projectile.owner != Main.myPlayer)
                    CavernsBrandistockVFX.Burst(Projectile.Center, Projectile.ai[2] == 2f ? 12 : 6, 3f);
                previousMode = (int)Projectile.ai[2];
            }
            glow *= 0.86f;
            player.ChangeDir(Axis.X < 0f ? -1 : 1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            Projectile.timeLeft = 120;
            previousReach = BrandistockMechanism.Reach(Progress) * ItemScale;
            float oldProgress = Progress;
            Projectile.ai[0]++;
            Projectile.Center = Hand + Axis * BrandistockMechanism.Reach(Progress) * ItemScale;
            Projectile.rotation = Axis.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
            if (oldProgress < 0.24f && Progress >= 0.24f)
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.45f, Pitch = target > 0f ? 0.35f : -0.05f }, Projectile.Center);
            if (target > 0f && Progress is > 0.25f and < 0.65f && Projectile.ai[0] % 3f == 0f)
                CavernsBrandistockVFX.Spark(Projectile.Center + Axis * (BrandistockArt.TipReach(extension) - 2f) * ItemScale, -Axis * 1.4f, 0.13f);
            if (Progress >= 1f)
            {
                player.itemTime = player.itemAnimation = 0;
                Projectile.Kill();
            }
        }
        private void UpdateMode(int mode)
        {
            if (Projectile.ai[2] == mode)
                return;
            Projectile.ai[2] = mode;
            Projectile.netUpdate = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            bool blades = Projectile.owner == Main.myPlayer ? Main.player[Projectile.owner].GetModPlayer<BrandistockPlayer>().Blades : Projectile.ai[2] == 1f;
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (BrandistockMechanism.CanSkillStrike(blades, extension))
                SkillStrikeUtil.setSkillStrike(Projectile, 1.65f, 1, 0.3f, 0.55f);
            modifiers.HitDirectionOverride = Axis.X < 0f ? -1 : 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collision = 0f;
            float scale = ItemScale;
            float reach = BrandistockMechanism.Reach(Progress) * scale;
            float bladeLength = BrandistockArt.TipReach(extension) * scale;
            Vector2 start = Hand + Axis * (Math.Min(previousReach, reach) - 22f * scale);
            Vector2 end = Hand + Axis * (Math.Max(previousReach, reach) + bladeLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, (26f + extension * 10f) * scale, ref collision);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool sharp = BrandistockMechanism.CanSkillStrike(Projectile.ai[2] == 1f, extension);
            Vector2 point = Vector2.Clamp(Projectile.Center + Axis * 15f, target.Hitbox.TopLeft(), target.Hitbox.BottomRight());
            glow = 1f;
            if (Projectile.owner == Main.myPlayer && !target.friendly && target.type != NPCID.TargetDummy && damageDone > 0)
            {
                var state = Main.player[Projectile.owner].GetModPlayer<BrandistockPlayer>();
                state.Strike(Projectile.Center);
                UpdateMode(state.Mode);
            }
            CavernsBrandistockVFX.Burst(point, sharp ? 8 : 3, sharp ? 3.5f : 1.5f);
            if (sharp)
            {
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.18f, Pitch = 0.5f, MaxInstances = 3 }, point);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, Vector2.Zero, ModContent.ProjectileType<CavernsBrandistockImpact>(), 0, 0f, Projectile.owner, 0.65f, Projectile.rotation);
            }
            else
                SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.2f, Pitch = -0.15f, MaxInstances = 3 }, point);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!initialized)
                return false;
            float fade = Math.Min(1f, Projectile.ai[0] / 2f) * MathHelper.Clamp((1f - Progress) / 0.1f, 0f, 1f);
            float scale = ItemScale;
            float thrust = MathHelper.Clamp((Progress - 0.22f) / 0.15f, 0f, 1f) * MathHelper.Clamp((0.75f - Progress) / 0.2f, 0f, 1f);
            float stress = Main.player[Projectile.owner].GetModPlayer<BrandistockPlayer>().Stress;
            float warning = Projectile.ai[2] == 1f ? MathHelper.Clamp((stress - 80f) / 20f, 0f, 1f) * (0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.5f) : 0f;
            for (int i = 4; i >= 1; i--)
            {
                Vector2 tail = Projectile.Center - Axis * i * 12f * scale;
                CavernsBrandistockVFX.Glow(tail, new Vector2(28f * scale), CavernsBrandistockVFX.Aqua, (1f - i / 5f) * extension * thrust * fade * 0.3f);
            }
            BrandistockArt.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.rotation, scale, extension, Math.Max(glow, warning), lightColor, fade);
            if (extension > 0f)
                CavernsBrandistockVFX.Flare(Projectile.Center + Axis * BrandistockArt.TipReach(extension) * scale, 48f * scale, (glow * 0.7f + thrust * 0.2f) * extension * fade, Projectile.rotation + MathHelper.PiOver2);
            return false;
        }
    }

    internal enum BrandistockChange { None, Extend, Retract, Overload, Ready }

    internal sealed class BrandistockMechanism
    {
        internal const float BladeThreshold = 40f;
        internal const int RecoveryTicks = 240;
        internal float Stress { get; private set; }
        internal int Cooldown { get; private set; }
        private int sinceHit;
        internal bool Blades => Cooldown == 0 && Stress >= BladeThreshold;

        internal void Reset()
        {
            Stress = 0f;
            Cooldown = sinceHit = 0;
        }

        internal BrandistockChange Tick()
        {
            if (Cooldown > 0)
            {
                Cooldown--;
                Stress = 100f * Cooldown / RecoveryTicks;
                sinceHit = 0;
                return Cooldown == 0 ? BrandistockChange.Ready : BrandistockChange.None;
            }
            bool extended = Blades;
            if (++sinceHit > 30)
                Stress = Math.Max(0f, Stress - 0.6f);
            return extended && !Blades ? BrandistockChange.Retract : BrandistockChange.None;
        }

        internal BrandistockChange Strike()
        {
            if (Cooldown > 0)
                return BrandistockChange.None;
            bool extended = Blades;
            Stress = Math.Min(100f, Stress + 14f);
            sinceHit = 0;
            if (Stress >= 100f)
            {
                Cooldown = RecoveryTicks;
                return BrandistockChange.Overload;
            }
            return !extended && Blades ? BrandistockChange.Extend : BrandistockChange.None;
        }

        internal static float Reach(float progress)
        {
            float p = Math.Clamp(progress, 0f, 1f);
            if (p < 0.2f)
                return 29f - Smooth(p / 0.2f) * 7f;
            if (p < 0.55f)
                return 22f + Smooth((p - 0.2f) / 0.35f) * 78f;
            return 100f - Smooth((p - 0.55f) / 0.45f) * 71f;
        }

        internal static bool CanHit(float progress) => progress >= 0.24f && progress <= 0.78f;
        internal static bool CanSkillStrike(bool blades, float extension) => blades && extension >= 0.85f;
        private static float Smooth(float value) => value * value * (3f - 2f * value);
    }

    public class CavernsBrandistockImpact : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/ImpactTextures/Burst_09";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float progress = 1f - Projectile.timeLeft / 20f;
            float power = Math.Clamp(Projectile.ai[0], 0.5f, 2f);
            float fade = (1f - progress) * (1f - progress);
            float size = (25f + MathF.Sqrt(progress) * 70f) * power;
            CavernsBrandistockVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), CavernsBrandistockVFX.Violet, fade * 0.5f);
            CavernsBrandistockVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), CavernsBrandistockVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            CavernsBrandistockVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), CavernsBrandistockVFX.Additive(CavernsBrandistockVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            CavernsBrandistockVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class CavernsBrandistockDebris : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Magnetic_Platform_Debris";
        private static readonly Rectangle[] Frames = [new(0, 0, 12, 22), new(16, 0, 18, 22), new(38, 0, 26, 22)];

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Frames[Main.rand.Next(Frames.Length)];
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.fadeIn = Main.rand.NextFloat(-0.12f, 0.12f);
            dust.noGravity = dust.noLight = true;
            dust.customData = 0;
        }

        public override bool Update(Dust dust)
        {
            int age = (int)dust.customData + 1;
            dust.customData = age;
            dust.velocity.Y = Math.Min(12f, dust.velocity.Y + 0.22f);
            Vector2 movement = Collision.TileCollision(dust.position - Vector2.One * 2f, dust.velocity, 4, 4);
            dust.position += movement;
            if (movement.Y != dust.velocity.Y)
            {
                dust.velocity.Y *= -0.3f;
                dust.velocity.X *= 0.7f;
                dust.fadeIn *= 0.6f;
            }
            if (movement.X != dust.velocity.X)
                dust.velocity.X *= -0.3f;
            dust.rotation += dust.fadeIn;
            if (age > 26)
                dust.alpha += 9;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            float opacity = 1f - dust.alpha / 255f;
            Vector2 position = dust.position - Main.screenPosition;
            Color light = Lighting.GetColor(dust.position.ToTileCoordinates());
            Main.EntitySpriteDraw(texture, position, dust.frame, Color.Lerp(light, Color.White, 0.25f) * opacity, dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, position, dust.frame, CavernsBrandistockVFX.Additive(CavernsBrandistockVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class CavernsBrandistockVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal const string RockTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ChargedStoneProjectile";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);
        internal static readonly Rectangle RockFrame = new(4, 6, 54, 48);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

        internal static void Sprite(string asset, Vector2 center, Vector2 size, Color color, float rotation = 0f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, color, rotation, texture.Size() * 0.5f, size / texture.Size(), SpriteEffects.None);
        }

        internal static void Glow(Vector2 center, Vector2 size, Color color, float opacity)
            => Sprite("AerovelenceMod/Assets/Orbs/SoftGlow", center, size, Additive(color, opacity));

        internal static void Ring(Vector2 center, Vector2 size, Color color, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/Ring/GlowRing", center, size, Additive(color, opacity), rotation);

        internal static void Flare(Vector2 center, float size, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/ImpactTextures/Spike", center, new Vector2(size * 0.65f, size), Additive(Color.White, opacity), rotation);

        internal static void Spark(Vector2 center, Vector2 velocity, float scale = 0.2f, Color? color = null)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: color ?? Aqua, Scale: scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.95f,
                timeBeforeSlow: 6, postSlowPower: 0.86f, velToBeginShrink: 1f, fadePower: 0.86f, shouldFadeColor: false);
        }

        internal static void Burst(Vector2 center, int count, float speed)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Spark(center, Main.rand.NextVector2CircularEdge(speed, speed) * Main.rand.NextFloat(0.4f, 1f), Main.rand.NextFloat(0.13f, 0.28f));
        }

        internal static void Smoke(Vector2 center, Vector2 velocity, float size, Color color)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<HighResSmoke>(), velocity, newColor: color, Scale: size / 128f);
            dust.customData = new HighResSmokeBehavior
            {
                randomSmokeNumber = 1,
                frameToStartFade = 8,
                fadeDuration = 28,
                velSlowAmount = 0.96f,
                drawSoftGlowUnder = false,
                overallAlpha = 0.8f
            };
        }

        internal static void Rubble(Vector2 center, int count, float speed = 4f)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Dust.NewDustPerfect(center, ModContent.DustType<CavernsBrandistockDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
