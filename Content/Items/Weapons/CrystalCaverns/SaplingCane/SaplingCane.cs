using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class SaplingCane : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SaplingCane/SaplingCane";
        private const string EnglishTooltip = "Plants three baby sappers at once\nHold right click to guide them to your cursor\nThe sap-lings target the nearest enemy to your cursor when guided";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Sap-ling Cane", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Skill Strikes when hitting your targetted enemy")
                .AddName(Language.Spanish, "Bastón de Brotes")
                .AddTooltip(Language.Spanish, "Planta tres pequeños Sappers usando un espacio de centinela\nMantén pulsado el botón derecho para guiarlos con pequeños saltos y designar enemigos\nMuerden a los enemigos cercanos y regresan a su sitio al dejarlos solos\nLos mordiscos contra el objetivo designado para tus invocaciones causan Golpes de Habilidad")
                .AddSkillStrike(Language.Spanish, "Muerde al enemigo designado como objetivo de tus invocaciones");
            Item.staff[Type] = true;
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
            Item.width = Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 12;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SaplingCluster>();
            Item.shootSpeed = 1;
            Item.knockBack = 2;
            Item.mana = 10;
            Item.sentry = true;
            Item.noUseGraphic = false;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2) return true;
            player.MinionNPCTargetAim(true);
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.MouseWorld;
            if (Vector2.DistanceSquared(target, player.Center) > 600 * 600) target = player.Center + (target - player.Center).SafeNormalize(Vector2.UnitX) * 600;
            if (!Collision.CanHitLine(player.Center, 1, 1, target, 1, 1)) target = player.Center;
            target.X = MathHelper.Clamp(target.X, 40f, Main.maxTilesX * 16f - 40f);
            target.Y = MathHelper.Clamp(target.Y, 40f, Main.maxTilesY * 16f - 80f);
            for (int step = 0; step < 12 && Collision.SolidCollision(target - new Vector2(12f, 15f), 24, 30); step++)
                target.Y -= 8f;
            int index = Projectile.NewProjectile(source, target, Vector2.Zero, type, damage, knockback, player.whoAmI);
            Main.projectile[index].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }

    }
    public class SaplingCluster : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SaplingCane/SaplingCluster";
        private int timer;
        private bool spawned;
        public override void SetStaticDefaults() => ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead) { Projectile.Kill(); return; }
            Projectile.timeLeft = Math.Max(2, Projectile.timeLeft);
            timer++;
            if (!spawned && Projectile.owner == Main.myPlayer)
            {
                spawned = true;
                Projectile.netUpdate = true;
                for (int i = -1; i <= 1; i++)
                {
                    Vector2 spawn = Projectile.Center + new Vector2(i * 28f, -10f);
                    if (Collision.SolidCollision(spawn - new Vector2(12f, 15f), 24, 30))
                        spawn = Projectile.Center;
                    int child = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, Vector2.Zero,
                        ModContent.ProjectileType<BabySapper>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.identity, i);
                    Main.projectile[child].originalDamage = Projectile.originalDamage;
                }
            }
            if (timer == 1)
            {
                SaplingCaneVFX.Burst(Projectile.Center, 12, 3f);
                SoundEngine.PlaySound(SoundID.Item44 with { Volume = 0.45f, Pitch = 0.2f }, Projectile.Center);
            }
            Vector2 center = Vector2.Zero;
            int count = 0;
            foreach (Projectile child in Main.ActiveProjectiles)
                if (child.owner == Projectile.owner && child.type == ModContent.ProjectileType<BabySapper>() && child.ai[0] == Projectile.identity)
                {
                    center += child.Center;
                    count++;
                }
            if (count > 0)
                Projectile.Center = center / count;
            if (Projectile.owner != Main.myPlayer) return;
            bool command = player.HeldItem.type == ModContent.ItemType<SaplingCane>() && !player.CCed && !player.noItems && Main.mouseRight && !player.mouseInterface && !Main.blockMouse;
            float mode = command ? 1 : 0;
            if (mode != Projectile.ai[0] || command && timer % 6 == 0)
            {
                Vector2 target = Main.MouseWorld;
                if (Vector2.DistanceSquared(target, player.Center) > 700 * 700) target = player.Center + (target - player.Center).SafeNormalize(Vector2.UnitX) * 700;
                target.X = MathHelper.Clamp(target.X, 32f, Main.maxTilesX * 16f - 32f);
                target.Y = MathHelper.Clamp(target.Y, 32f, Main.maxTilesY * 16f - 32f);
                Projectile.ai[0] = mode;
                Projectile.ai[1] = target.X;
                Projectile.ai[2] = target.Y;
                Projectile.netUpdate = true;
            }
            if (command && timer % 10 == 0) player.MinionNPCTargetAim(true);
        }
        public override bool PreDraw(ref Color lightColor) => false;
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(spawned);
        public override void ReceiveExtraAI(BinaryReader reader) => spawned = reader.ReadBoolean();
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile child = Main.projectile[i];
                if (child.active && child.owner == Projectile.owner && child.ModProjectile is BabySapper baby && child.ai[0] == Projectile.identity)
                    baby.Retire();
            }
        }
    }

    public class BabySapper : ModProjectile
    {
        private bool onGround;
        private bool anchored;
        private bool retiring;
        private int retireAge;
        private int orphanTicks;
        private int age;
        private int hopAge = 60;
        private int landingAge = 60;
        private int windup;
        private int targetIndex = -1;
        private Vector2 idleAnchor;
        private Vector2 jumpVelocity;
        private float headRotation;
        private float hitFlash;
        private bool wasCommanded;
        private bool anchorOnLanding = true;
        private bool initializedFacing;
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SaplingCane/SaplingCluster";
        private const string StemTexture = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/SaplingCane/SaplingStem";
        private float Opacity => Math.Min(1f, age / 12f) * (retiring ? Math.Max(0f, 1f - retireAge / 20f) : 1f);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = false;
            ProjectileID.Sets.SentryShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => retiring || orphanTicks > 0 || age < 12 ? false : null;
        public override bool? CanHitNPC(NPC target) => Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) ? null : false;

        private Projectile Parent()
        {
            foreach (Projectile parent in Main.ActiveProjectiles)
                if (parent.owner == Projectile.owner && parent.type == ModContent.ProjectileType<SaplingCluster>() && parent.identity == (int)Projectile.ai[0])
                    return parent;
            return null;
        }

        public void Retire()
        {
            if (retiring)
                return;
            retiring = true;
            Projectile.friendly = false;
            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(anchored);
            writer.Write(anchorOnLanding);
            writer.Write(idleAnchor.X);
            writer.Write(idleAnchor.Y);
            writer.Write(windup);
            writer.Write(jumpVelocity.X);
            writer.Write(jumpVelocity.Y);
            writer.Write(hopAge);
            writer.Write(retiring);
            writer.Write(targetIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            anchored = reader.ReadBoolean();
            anchorOnLanding = reader.ReadBoolean();
            idleAnchor = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            windup = reader.ReadInt32();
            jumpVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            hopAge = reader.ReadInt32();
            retiring = reader.ReadBoolean();
            targetIndex = reader.ReadInt32();
        }

        private bool CanReach(NPC target)
            => target.CanBeChasedBy(Projectile) && Vector2.DistanceSquared(Projectile.Center, target.Center) < 150f * 150f &&
                Vector2.DistanceSquared(idleAnchor, target.Center) < 180f * 180f && Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1);

        private NPC FindTarget(Player player)
        {
            int designated = player.MinionAttackTargetNPC;
            if (designated >= 0 && designated < Main.maxNPCs && CanReach(Main.npc[designated]))
                return Main.npc[designated];
            NPC closest = null;
            float nearest = 150f * 150f;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float distance = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                if (distance < nearest && CanReach(npc))
                {
                    closest = npc;
                    nearest = distance;
                }
            }
            return closest;
        }

        public override void AI()
        {
            age++;
            hopAge++;
            landingAge++;
            hitFlash *= 0.8f;
            Projectile.timeLeft = 60;
            Player player = Main.player[Projectile.owner];
            Projectile parent = Parent();
            if (!player.active || player.dead)
                Retire();
            if (parent == null)
            {
                if (++orphanTicks > 15)
                    Retire();
            }
            else
                orphanTicks = 0;
            if (retiring)
            {
                Projectile.velocity.X *= 0.8f;
                Projectile.velocity.Y = Math.Min(12f, Projectile.velocity.Y + 0.3f);
                if (++retireAge >= 20)
                    Projectile.Kill();
                return;
            }
            if (parent == null)
            {
                Projectile.velocity.Y = Math.Min(12f, Projectile.velocity.Y + 0.3f);
                return;
            }
            bool command = parent.ai[0] == 1f;
            if (command)
                anchorOnLanding = true;
            if (!anchored || command || wasCommanded)
            {
                idleAnchor = Projectile.Center;
                anchored = true;
            }
            if (wasCommanded && !command && Projectile.owner == Main.myPlayer)
                Projectile.netUpdate = true;
            wasCommanded = command;
            NPC target = null;
            if (Projectile.owner == Main.myPlayer)
            {
                target = FindTarget(player);
                int nextTarget = target?.whoAmI ?? -1;
                if (nextTarget != targetIndex)
                {
                    targetIndex = nextTarget;
                    Projectile.netUpdate = true;
                }
            }
            else if (targetIndex >= 0 && targetIndex < Main.maxNPCs && Main.npc[targetIndex].active)
                target = Main.npc[targetIndex];
            Vector2 destination = command ? new Vector2(parent.ai[1] + Projectile.ai[1] * 28f, parent.ai[2]) : target?.Center ?? idleAnchor;
            float distance = destination.X - Projectile.Center.X;
            float height = destination.Y - Projectile.Center.Y;
            bool grounded = onGround;
            Projectile.ai[2]++;
            int cooldown = (command ? 22 : target != null ? 32 : 36) + ((int)Projectile.ai[1] + 1) * 3;
            if (Projectile.owner == Main.myPlayer && windup == 0 && grounded && Projectile.ai[2] >= cooldown && SaplingMotion.NeedsHop(distance, height, target != null))
            {
                jumpVelocity = new Vector2(SaplingMotion.HopX(distance, height), SaplingMotion.HopY(height));
                windup = 6;
                Projectile.netUpdate = true;
            }
            if (windup > 0)
            {
                Projectile.velocity.X *= 0.5f;
                if (--windup == 0 && grounded)
                {
                    Projectile.velocity = jumpVelocity;
                    Projectile.ai[2] = 0f;
                    hopAge = 0;
                    onGround = false;
                    SaplingCaneVFX.Burst(Projectile.Bottom, 4, 1.7f);
                    SaplingCaneVFX.Smoke(Projectile.Bottom, new Vector2(-jumpVelocity.X * 0.15f, -0.3f), 22f, SaplingCaneVFX.Violet * 0.65f);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                }
            }
            if (onGround)
                Projectile.velocity.X *= 0.75f;
            onGround = false;
            Projectile.velocity.Y = Math.Min(12f, Projectile.velocity.Y + 0.3f);
            Vector2 look = target != null ? target.Center - Projectile.Center : new Vector2(command ? distance : Projectile.velocity.X * 10f, -25f);
            if (!initializedFacing)
            {
                Projectile.spriteDirection = Projectile.ai[1] > 0f ? 1 : -1;
                initializedFacing = true;
            }
            if (Math.Abs(look.X) > 8f)
                Projectile.spriteDirection = look.X > 0f ? 1 : -1;
            float baseAngle = Projectile.spriteDirection > 0 ? -MathHelper.PiOver4 : -3f * MathHelper.PiOver4;
            float tilt = MathHelper.Clamp(MathHelper.WrapAngle(look.ToRotation() - baseAngle), -0.55f, 0.55f);
            headRotation = MathHelper.Lerp(headRotation, tilt + Projectile.velocity.X * 0.02f, 0.12f);
            Projectile.frame = SaplingMotion.HeadFrame(age, (int)Projectile.ai[1], target != null);
            Lighting.AddLight(Projectile.Center, SaplingCaneVFX.Aqua.ToVector3() * 0.12f * Opacity);
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => false;
        public override void PostAI()
        {
            Projectile parent = Parent();
            bool fall = parent != null && parent.ai[0] == 1f && parent.ai[2] > Projectile.Bottom.Y + 24f;
            if (Projectile.velocity.Y >= 0f)
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            Vector2 oldVelocity = Projectile.velocity;
            Vector4 downhill = Collision.WalkDownSlope(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, 0.3f);
            Projectile.velocity = new Vector2(downhill.Z, downhill.W);
            Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, fall, fall);
            Projectile.position += Projectile.velocity;
            Vector4 slope = Collision.SlopeCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, 0f, fall);
            Projectile.position = new Vector2(slope.X, slope.Y);
            Projectile.velocity = new Vector2(slope.Z, slope.W);
            if (Projectile.velocity != oldVelocity)
                OnTileCollide(oldVelocity);
            if (!fall && oldVelocity.Y >= 0f && Math.Abs(Projectile.velocity.Y) < 0.01f)
                onGround = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0f)
            {
                onGround = true;
                if (anchorOnLanding)
                {
                    idleAnchor = Projectile.Center;
                    anchorOnLanding = false;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                }
                if (oldVelocity.Y > 2f)
                {
                    landingAge = 0;
                    if (!anchored || wasCommanded)
                        idleAnchor = Projectile.Center;
                    if (!retiring)
                    {
                        SaplingCaneVFX.Burst(Projectile.Bottom, 3, 1.3f);
                        SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.16f, Pitch = 0.5f }, Projectile.Bottom);
                    }
                }
                Projectile.velocity.X *= 0.65f;
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (Main.player[Projectile.owner].MinionAttackTargetNPC == target.whoAmI)
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 1, 0.3f, 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitFlash = 1f;
            SaplingCaneVFX.Burst(Projectile.Center, 7, 2.5f);
            SoundEngine.PlaySound(SoundID.Item2 with { Volume = 0.2f, Pitch = 0.4f }, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            if (!retiring)
                SaplingCaneVFX.Burst(Projectile.Center, 10, 2.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D head = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D stem = ModContent.Request<Texture2D>(StemTexture).Value;
            bool jumping = !onGround;
            int stemFrame = jumping ? SaplingMotion.StemFrame(hopAge) : 0;
            Rectangle stemSource = new(0, stemFrame == 0 ? 0 : 20, 22, 18);
            Rectangle headSource = new(0, Projectile.frame * 22, 22, 22);
            float squash = Math.Max(windup / 6f * 0.25f, SaplingMotion.LandingSquash(landingAge) * 0.2f);
            float bob = MathF.Sin((age + Projectile.ai[1] * 13f) * 0.09f) * 0.7f;
            Vector2 foot = Projectile.Bottom - Main.screenPosition;
            Vector2 stemScale = new(0.78f + squash * 0.3f, (jumping ? 0.9f : 0.68f) * (1f - squash));
            Main.EntitySpriteDraw(stem, foot, stemSource, lightColor * Opacity, Projectile.velocity.X * 0.02f,
                new Vector2(11f, 18f), stemScale, SpriteEffects.None);
            Vector2 headPoint = foot - new Vector2(0f, 19f - squash * 7f + (jumping ? 3f : bob));
            Vector2 headScale = new(1f + squash, 1f - squash);
            SpriteEffects flip = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            bool designated = targetIndex >= 0 && Main.player[Projectile.owner].MinionAttackTargetNPC == targetIndex;
            float glow = (designated ? 0.35f : 0.08f) + hitFlash * 0.5f;
            SaplingCaneVFX.Glow(headPoint + Main.screenPosition, new Vector2(30f), SaplingCaneVFX.Aqua, glow * Opacity * 0.5f);
            Main.EntitySpriteDraw(head, headPoint, headSource, Color.Lerp(lightColor, Color.White, 0.15f) * Opacity,
                headRotation, new Vector2(11f), headScale, flip);
            Main.EntitySpriteDraw(head, headPoint, headSource, SaplingCaneVFX.Additive(SaplingCaneVFX.Aqua, glow * Opacity),
                headRotation, new Vector2(11f), headScale, flip);
            Main.EntitySpriteDraw(stem, foot, stemSource, SaplingCaneVFX.Additive(SaplingCaneVFX.Aqua, (jumping ? 0.35f : 0.1f) * Opacity),
                Projectile.velocity.X * 0.02f, new Vector2(11f, 18f), stemScale, SpriteEffects.None);
            return false;
        }
    }

    internal static class SaplingMotion
    {
        internal static int HeadFrame(int age, int sibling, bool attacking) => ((age + (sibling + 1) * 5) / (attacking ? 6 : 11)) % 2;
        internal static int StemFrame(int hopAge) => hopAge < 7 ? 0 : 1;
        internal static float HopY(float height) => height < -45f ? -7f : -4.8f;
        internal static float HopX(float distance, float height) => Math.Clamp(distance / (Math.Abs(HopY(height)) * 2f / 0.3f), -5f, 5f);
        internal static bool NeedsHop(float distance, float height, bool attacking) => Math.Abs(distance) > (attacking ? 8f : 16f) || height < -20f;
        internal static float LandingSquash(int age) => Math.Max(0f, 1f - age / 9f);
    }

    internal static class SaplingCaneVFX
    {
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
    }
}
