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
    public class SpeleothemConjurer : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SpeleothemConjurer/SpeleothemConjurer";
        private const string EnglishTooltip = "Channel a shower of stalactites at the cursor\nStalactites leave crystal sediment on shattering\nRelease to pull the sediment together and erupt a towering stalagmite";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Speleothem Conjurer", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Sufficiently large stalagmites Skill Strike")
                .AddName(Language.Spanish, "Conjurador de Espeleotemas")
                .AddTooltip(Language.Spanish, "Canaliza una lluvia fija de estalactitas etÃ©reas en el cursor\nConsume 6 de manÃ¡ por pÃºa; los impactos dejan sedimento de cristal\nSuelta para reunir el sedimento y hacer brotar una gran estalagmita\nAcumula 12 impactos para un Golpe de Habilidad; el cristal brillarÃ¡ en blanco")
                .AddSkillStrike(Language.Spanish, "Haz brotar la estalagmita tras reunir al menos 12 impactos");
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
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SpeleothemHeld>();
            Item.shootSpeed = 1f;
            Item.knockBack = 3f;
            Item.mana = 6;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.MouseWorld;
            Vector2 offset = target - player.Center;
            if (offset.Length() > 600f)
                target = player.Center + offset.SafeNormalize(Vector2.UnitY) * 600f;
            target.X = MathHelper.Clamp(target.X, 32f, Main.maxTilesX * 16f - 32f);
            target.Y = MathHelper.Clamp(target.Y, 32f, Main.maxTilesY * 16f - 64f);
            if (!Collision.CanHitLine(player.Center, 1, 1, target, 1, 1))
                target = player.Center + new Vector2(player.direction * 32f, -16f);
            Projectile.NewProjectile(source, target, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<CavernStoneItem>(25).AddIngredient<CavernCrystalItem>(18).AddIngredient(ItemID.Amethyst, 6).AddTile(TileID.Anvils).Register();
    }

    public class SpeleothemHeld : ModProjectile
    {
        private enum CastPhase { Channel, Settling, Gathering, Fading }
        private CastPhase Phase => (CastPhase)Projectile.ai[2];
        private readonly List<Vector2> sediment = new();
        private readonly List<Vector2> sedimentVelocity = new();
        private Vector2 floor;
        private Vector2 source;
        private int visualCount;
        private bool initialized;
        private bool groundedOrigin;
        public override string Texture => SpeleothemConjurerVFX.CrystalTexture;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 300;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public static SpeleothemHeld Find(int owner, int identity)
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
                if (projectile.owner == owner && projectile.identity == identity && projectile.ModProjectile is SpeleothemHeld held)
                    return held;
            return null;
        }

        public void Accrue(Vector2 point)
        {
            if (Phase is CastPhase.Gathering or CastPhase.Fading || Projectile.ai[1] >= 24f)
                return;
            Projectile.ai[1]++;
            Projectile.netUpdate = true;
            AddSediment(point);
            visualCount++;
        }

        private void AddSediment(Vector2 point)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 3; i++)
            {
                sediment.Add(point);
                sedimentVelocity.Add(Main.rand.NextVector2Circular(3f, 2f) - Vector2.UnitY * 2.5f);
            }
        }

        private void SetPhase(CastPhase phase)
        {
            Projectile.ai[2] = (float)phase;
            Projectile.ai[0] = 0f;
            Projectile.netUpdate = true;
        }

        private bool FallingSpikesRemain()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
                if (projectile.owner == Projectile.owner && projectile.ModProjectile is EtherealStalactite && projectile.ai[0] == Projectile.identity)
                    return true;
            return false;
        }

        private void FindSurfaces()
        {
            initialized = true;
            floor = Projectile.Center;
            for (int i = 0; i < 100 && floor.Y < Main.maxTilesY * 16f - 32f && !Collision.SolidCollision(floor, 2, 2); i++)
                floor.Y += 8f;
            groundedOrigin = Collision.SolidCollision(floor, 2, 2);
            if (groundedOrigin)
                for (int i = 0; i < 10 && Collision.SolidCollision(floor, 2, 2); i++)
                    floor.Y--;
            source = Projectile.Center;
            for (int i = 0; i < 24 && source.Y > 32f && !Collision.SolidCollision(source - new Vector2(6f, 24f), 12, 24); i++)
                source.Y -= 8f;
        }

        public override void AI()
        {
            if (!initialized)
                FindSurfaces();
            Player player = Main.player[Projectile.owner];
            bool owner = Projectile.owner == Main.myPlayer;
            if (Phase != CastPhase.Fading && (!player.active || player.dead || Vector2.DistanceSquared(player.Center, Projectile.Center) > 1200f * 1200f))
                SetPhase(CastPhase.Fading);
            Projectile.timeLeft = 60;
            Projectile.ai[0]++;
            if (Phase == CastPhase.Channel)
            {
                if (owner && (!player.channel || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<SpeleothemConjurer>()))
                    SetPhase(CastPhase.Settling);
                else
                {
                    Vector2 aim = (Projectile.Center - player.MountedCenter).SafeNormalize(Vector2.UnitX);
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = player.itemAnimation = 2;
                    player.ChangeDir(aim.X >= 0f ? 1 : -1);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, aim.ToRotation() - MathHelper.PiOver2);
                    if (owner && (Projectile.ai[0] - 1f) % 10f == 0f)
                    {
                        if (Projectile.ai[0] > 1f && !player.CheckMana(player.HeldItem, -1, true))
                            SetPhase(CastPhase.Settling);
                        else
                        {
                            player.manaRegenDelay = 60;
                            Vector2 spawn = source + new Vector2(Main.rand.NextFloat(-42f, 42f), 0f);
                            if (Collision.SolidCollision(spawn - new Vector2(6f, 12f), 12, 24))
                                spawn = source;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, new Vector2(Main.rand.NextFloat(-0.35f, 0.35f), 5f),
                                ModContent.ProjectileType<EtherealStalactite>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.identity);
                        }
                    }
                }
            }
            if (owner && Phase == CastPhase.Settling && Projectile.ai[0] >= 12f && (!FallingSpikesRemain() || Projectile.ai[0] >= 60f))
                SetPhase(CastPhase.Gathering);
            while (visualCount < (int)Projectile.ai[1])
            {
                AddSediment(floor + new Vector2(Main.rand.NextFloat(-50f, 50f), -6f));
                visualCount++;
            }
            if (Projectile.ai[1] >= 12f && Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                SpeleothemConjurerVFX.Burst(floor, 16, 4f);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.4f, Pitch = 0.25f }, floor);
            }
            UpdateSediment();
            if (Phase == CastPhase.Gathering)
            {
                if (Projectile.ai[0] == 1f)
                    SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.5f, Pitch = -0.35f }, floor);
                if (owner && Projectile.ai[0] >= 36f)
                {
                    if (Projectile.ai[1] > 0f && groundedOrigin)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), floor, Vector2.Zero,
                            ModContent.ProjectileType<GatheredStalagmite>(), (int)(Projectile.damage * (1f + Projectile.ai[1] * 0.09f)), 7f, Projectile.owner, Projectile.ai[1]);
                    SetPhase(CastPhase.Fading);
                }
            }
            if (Phase == CastPhase.Fading && Projectile.ai[0] >= 20f)
                Projectile.Kill();
            float fade = Phase == CastPhase.Fading ? MathHelper.Clamp(1f - Projectile.ai[0] / 20f, 0f, 1f) : 1f;
            Lighting.AddLight(source, SpeleothemConjurerVFX.Aqua.ToVector3() * fade * 0.4f);
            Lighting.AddLight(floor, SpeleothemConjurerVFX.Violet.ToVector3() * fade * (0.2f + Projectile.ai[1] / 48f));
        }

        private void UpdateSediment()
        {
            for (int i = 0; i < sediment.Count; i++)
            {
                if (Phase == CastPhase.Channel)
                {
                    Vector2 velocity = sedimentVelocity[i] + Vector2.UnitY * 0.18f;
                    velocity.Y = Math.Min(12f, velocity.Y);
                    Vector2 permitted = Collision.TileCollision(sediment[i] - Vector2.One * 2f, velocity, 4, 4);
                    sediment[i] += permitted;
                    if (permitted.X != velocity.X)
                        velocity.X *= -0.35f;
                    if (permitted.Y != velocity.Y)
                        velocity = new Vector2(velocity.X * 0.72f, -velocity.Y * 0.3f);
                    sedimentVelocity[i] = velocity;
                }
                else
                {
                    float progress = Phase == CastPhase.Fading ? 1f : Phase == CastPhase.Settling ? 0.12f : MathHelper.Clamp(Projectile.ai[0] / 36f, 0f, 1f);
                    Vector2 destination = floor - new Vector2(0f, 7f);
                    Vector2 swirl = (sediment[i] - destination).RotatedBy(0.15f * (1f - progress));
                    sediment[i] = Vector2.Lerp(destination + swirl, destination, 0.04f + progress * progress * 0.4f);
                    if (Phase == CastPhase.Gathering && i % 3 == 0 && Projectile.ai[0] % 5f == 0f)
                        SpeleothemConjurerVFX.Spark(sediment[i], (destination - sediment[i]) * 0.03f, 0.12f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!initialized)
                return false;
            float fade = Phase == CastPhase.Fading ? MathHelper.Clamp(1f - Projectile.ai[0] / 20f, 0f, 1f) : 1f;
            float charge = Projectile.ai[1] / 24f;
            float gather = Phase == CastPhase.Gathering ? MathHelper.Clamp(Projectile.ai[0] / 36f, 0f, 1f) : 0f;
            Color color = Projectile.ai[1] >= 12f ? Color.Lerp(SpeleothemConjurerVFX.Aqua, Color.White, 0.55f) : SpeleothemConjurerVFX.Aqua;
            float time = Main.GlobalTimeWrappedHourly;
            if (Phase == CastPhase.Channel)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 aim = (Projectile.Center - player.MountedCenter).SafeNormalize(Vector2.UnitX);
                Vector2 hand = player.MountedCenter + aim * 18f;
                Texture2D staff = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SpeleothemConjurer/SpeleothemConjurer").Value;
                Main.EntitySpriteDraw(staff, hand - Main.screenPosition, null, lightColor, aim.ToRotation() + MathHelper.PiOver4, staff.Size() * 0.5f, 0.85f, SpriteEffects.None);
                SpeleothemConjurerVFX.Glow(hand + aim * 16f, new Vector2(50f), color, 0.35f);
            }
            float portalFade = Phase == CastPhase.Channel ? Math.Min(1f, Projectile.ai[0] / 12f) : Phase == CastPhase.Settling ? Math.Max(0f, 1f - Projectile.ai[0] / 20f) : 0f;
            SpeleothemConjurerVFX.Ring(source, new Vector2(130f, 35f), SpeleothemConjurerVFX.Violet, portalFade * 0.65f);
            SpeleothemConjurerVFX.Ring(source, new Vector2(95f, 20f), color, portalFade * 0.8f);
            for (int i = 0; i < 7; i++)
            {
                float angle = time * 1.5f + i * MathHelper.TwoPi / 7f;
                SpeleothemConjurerVFX.Crystal(source + new Vector2(MathF.Cos(angle) * 43f, MathF.Sin(angle) * 7f), MathF.Sin(angle) * 0.2f, new Vector2(5f, 15f), portalFade, 0.4f);
            }
            for (int i = 0; i < sediment.Count; i++)
                SpeleothemConjurerVFX.Crystal(sediment[i], i * 2.4f + (Phase == CastPhase.Gathering ? time * 4f : sedimentVelocity[i].X * 0.1f), new Vector2(3f, 6f), fade, 0.3f + gather * 0.7f);
            float pulse = 0.7f + MathF.Sin(time * 5f) * 0.15f;
            SpeleothemConjurerVFX.Glow(floor - Vector2.UnitY * 6f, new Vector2(100f + charge * 100f, 42f), color, (0.3f + gather * 0.5f) * fade);
            SpeleothemConjurerVFX.Ring(floor, new Vector2((80f + charge * 65f) * (1f - gather * 0.7f), 18f), color, pulse * fade);
            if (Projectile.ai[1] >= 12f || gather > 0f)
                SpeleothemConjurerVFX.Flare(floor - Vector2.UnitY * 6f, 40f + charge * 30f + gather * 90f, (0.35f + gather * 0.5f) * fade);
            return false;
        }
    }

    public class EtherealStalactite : ModProjectile
    {
        private bool impacted;
        public override string Texture => SpeleothemConjurerVFX.CrystalTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 100;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(16f, Projectile.velocity.Y + 0.35f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (++Projectile.ai[1] == 1f)
                SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.18f, Pitch = 0.4f, MaxInstances = 3 }, Projectile.Center);
            if (Projectile.ai[1] % 4f == 0f)
                SpeleothemConjurerVFX.Spark(Projectile.Center, -Projectile.velocity * 0.05f, 0.13f);
            Lighting.AddLight(Projectile.Center, SpeleothemConjurerVFX.Aqua.ToVector3() * 0.25f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            impacted = true;
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => impacted = true;
        public override void OnKill(int timeLeft)
        {
            SpeleothemConjurerVFX.Burst(Projectile.Center, 5, 2.5f);
            SpeleothemConjurerVFX.Smoke(Projectile.Center, -Vector2.UnitY, 38f, SpeleothemConjurerVFX.Aqua);
            if (impacted && Projectile.owner == Main.myPlayer)
                SpeleothemHeld.Find(Projectile.owner, (int)Projectile.ai[0])?.Accrue(Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.ai[1] / 4f);
            for (int i = Projectile.oldPos.Length - 1; i >= 1; i--)
                if (Projectile.oldPos[i] != Vector2.Zero)
                    SpeleothemConjurerVFX.Glow(Projectile.oldPos[i] + Projectile.Size * 0.5f, new Vector2(18f, 30f), SpeleothemConjurerVFX.Aqua, (1f - i / 5f) * 0.22f * fade);
            SpeleothemConjurerVFX.Crystal(Projectile.Center, Projectile.rotation, new Vector2(11f, 32f), fade, 0.6f);
            return false;
        }
    }

    public class GatheredStalagmite : ModProjectile
    {
        private float Height => 65f + Math.Min(24f, Projectile.ai[0]) * 6f;
        private float Width => 22f + Math.Min(24f, Projectile.ai[0]) * 1.7f;
        private float Growth => 1f - MathF.Pow(1f - MathHelper.Clamp(Projectile.ai[1] / 10f, 0f, 1f), 3f);
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SpeleothemConjurer/SpeleothemConjurerCrystal";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 240;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Projectile.timeLeft > 22 ? null : false;
        public override void AI()
        {
            if (++Projectile.ai[1] == 1f)
            {
                if (Projectile.ai[0] >= 12f)
                    SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 100, 0.45f, 0.7f);
                SpeleothemConjurerVFX.Burst(Projectile.Center, 20, 6f);
                SpeleothemConjurerVFX.Rubble(Projectile.Center, 8, 4.5f);
                for (int i = -2; i <= 2; i++)
                    SpeleothemConjurerVFX.Smoke(Projectile.Center, new Vector2(i * 1.7f, -1.5f), 85f, SpeleothemConjurerVFX.Violet);
                SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f, Pitch = -0.35f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.35f, Pitch = -0.1f }, Projectile.Center);
            }
            if (Projectile.ai[1] <= 10f || Projectile.timeLeft < 22 && Projectile.timeLeft % 3 == 0)
                SpeleothemConjurerVFX.Spark(Projectile.Center - Vector2.UnitY * Height * Growth * Main.rand.NextFloat(0.3f, 1f), Main.rand.NextVector2Circular(2f, 2f), 0.15f);
            Lighting.AddLight(Projectile.Center - Vector2.UnitY * Height * 0.5f, SpeleothemConjurerVFX.Aqua.ToVector3() * Math.Min(1f, Projectile.timeLeft / 22f) * 0.6f);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collision = 0f;
            Vector2 tip = Projectile.Center - Vector2.UnitY * Height * Growth;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, tip, Width * 0.65f, ref collision)
                && Collision.CanHitLine(Projectile.Center - Vector2.UnitY * 4f, 1, 1, targetHitbox.Center.ToVector2(), 1, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 22f);
            float h = Height * Growth;
            SpeleothemConjurerVFX.Glow(Projectile.Center - Vector2.UnitY * h * 0.45f, new Vector2(Width * 2.7f, h * 1.5f), SpeleothemConjurerVFX.Violet, fade * 0.5f);
            Texture2D formation = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(formation.Width * 0.5f, formation.Height);
            Vector2 scale = new(Width / formation.Width, h / formation.Height);
            Main.EntitySpriteDraw(formation, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.3f) * fade, 0f, origin, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(formation, Projectile.Center - Main.screenPosition, null, SpeleothemConjurerVFX.Additive(SpeleothemConjurerVFX.Aqua, fade * 0.3f), 0f, origin, scale, SpriteEffects.None);
            float shock = MathHelper.Clamp(1f - Projectile.ai[1] / 18f, 0f, 1f);
            SpeleothemConjurerVFX.Ring(Projectile.Center, new Vector2(80f + Projectile.ai[1] * 7f, 25f), SpeleothemConjurerVFX.Aqua, shock);
            SpeleothemConjurerVFX.Flare(Projectile.Center - Vector2.UnitY * h, 80f, shock);
            return false;
        }
        public override void OnKill(int timeLeft) => SpeleothemConjurerVFX.Burst(Projectile.Center - Vector2.UnitY * Height * 0.5f, 6, 2f);
    }

    public class SpeleothemConjurerDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, SpeleothemConjurerVFX.Additive(SpeleothemConjurerVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class SpeleothemConjurerVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);

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

        internal static void Crystal(Vector2 center, float rotation, Vector2 size, float opacity = 1f, float charge = 0.3f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(CrystalTexture).Value;
            Vector2 screen = center - Main.screenPosition;
            Vector2 scale = size / texture.Size();
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = (i * MathHelper.PiOver2 + rotation).ToRotationVector2() * (1f + charge);
                Main.EntitySpriteDraw(texture, screen + offset, null, Additive(Aqua, opacity * charge * 0.45f), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, screen, null, Color.White * opacity, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, screen, null, Additive(Color.White, opacity * charge), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
        }

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
                Dust.NewDustPerfect(center, ModContent.DustType<SpeleothemConjurerDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
