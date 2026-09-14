using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class CavernousRampart : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/CavernousRampart/CavernousRampart";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Cavernous Rampart", "Hold to guard toward the cursor; cannot be used while mounted\nHits against the shield receive 12 additional defense\nA guarded hit exceeding 20% of maximum life shatters it for 6 seconds\nFast movement bashes enemies and grants a brief moment of invulnerability")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Baluarte Cavernoso")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Mantén pulsado para protegerte hacia el cursor; no se puede usar sobre una montura\nLos golpes contra el escudo reciben 12 de defensa adicional\nUn golpe bloqueado superior al 20% de tu vida máxima lo rompe durante 6 segundos\nEl movimiento rápido embiste a los enemigos y concede una breve invulnerabilidad");

            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Hold to guard toward the cursor; cannot be used while mounted\nHits against the shield receive 12 additional defense\nA guarded hit exceeding 20% of maximum life shatters it for 6 seconds\nFast movement bashes enemies and grants a brief moment of invulnerability"));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RampartHeld>();
            Item.shootSpeed = 1;
            Item.knockBack = 4;
            Item.channel = true;
        }
        public override bool CanUseItem(Player player) => !player.mount.Active && player.GetModPlayer<RampartPlayer>().Cooldown == 0 && player.ownedProjectileCounts[Item.shoot] == 0;
    }
    public class RampartPlayer : ModPlayer
    {
        public int Cooldown, ImpactCooldown;
        private bool guardedHit;
        internal float GuardFlash;
        public override void PostUpdate()
        {
            if (Cooldown > 0 && --Cooldown == 0 && Player.HeldItem.type == ModContent.ItemType<CavernousRampart>())
            {
                CavernousRampartVFX.Burst(Player.Center, 14, 3);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = .35f }, Player.Center);
            }
            if (ImpactCooldown > 0) ImpactCooldown--;
            GuardFlash *= 0.85f;
        }
        public override void UpdateDead()
        {
            if (Cooldown > 0) Cooldown--;
            ImpactCooldown = 0;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            guardedHit = false;
            if (Cooldown > 0 || Player.mount.Active || Player.HeldItem.type != ModContent.ItemType<CavernousRampart>() || !Player.channel
                || Player.CCed || Player.noItems || Player.whoAmI != Main.myPlayer) return;
            if (!modifiers.DamageSource.TryGetCausingEntity(out Entity source)) return;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile shield = Main.projectile[i];
                if (!shield.active || shield.owner != Player.whoAmI || shield.type != ModContent.ProjectileType<RampartHeld>()) continue;
                Vector2 direction = shield.rotation.ToRotationVector2();
                Vector2 incoming = (source.Center - Player.Center).SafeNormalize(direction);
                if (Vector2.Dot(direction, incoming) < .35f) return;
                guardedHit = true;
                modifiers.FinalDamage.Base -= 12 * Player.DefenseEffectiveness.Value;
                modifiers.Knockback *= .7f;
                break;
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (!guardedHit) return;
            guardedHit = false;
            bool broken = info.Damage > Player.statLifeMax2 * 0.2d;
            GuardFlash = 1f;
            if (broken) CavernousRampartVFX.Rubble(Player.Center, 12, 4.5f);
            CavernousRampartVFX.Burst(Player.Center, broken ? 28 : 12, broken ? 5 : 3);
            SoundEngine.PlaySound(broken ? SoundID.Shatter with { Volume = .6f } : SoundID.Tink with { Volume = .5f }, Player.Center);
            if (broken) Cooldown = 360;
        }
    }
    public class RampartCooldownLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int cooldown = player.GetModPlayer<RampartPlayer>().Cooldown;
            if (player.whoAmI != Main.myPlayer || drawInfo.shadow != 0f || player.dead || cooldown <= 0 || player.HeldItem.type != ModContent.ItemType<CavernousRampart>()) return;
            Texture2D frame = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightMeter").Value;
            Vector2 center = player.Top - Main.screenPosition - Vector2.UnitY * 18f;
            drawInfo.DrawDataCache.Add(new DrawData(frame, center, new Rectangle(0, 24, 98, 30), Color.White, 0f, new Vector2(49f, 15f), 0.6f, SpriteEffects.None));
            float ready = 1f - cooldown / 360f;
            drawInfo.DrawDataCache.Add(new DrawData(TextureAssets.MagicPixel.Value, center + new Vector2(-23f, 0f), new Rectangle(0, 0, 1, 1), CavernousRampartVFX.Aqua,
                0f, Vector2.Zero, new Vector2(46f * ready, 3f), SpriteEffects.None));
        }
    }
    public class RampartHeld : ModProjectile
    {
        public override string Texture => CavernousRampartRelicArt.Shield;
        private float fade = 1;
        private bool retiring;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];
            return !retiring && player.GetModPlayer<RampartPlayer>().ImpactCooldown == 0 && Vector2.Dot(player.velocity, Projectile.rotation.ToRotationVector2()) > 5 ? null : false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool stop = !player.active || player.dead || player.CCed || player.noItems || player.mount.Active || player.HeldItem.type != ModContent.ItemType<CavernousRampart>() || player.GetModPlayer<RampartPlayer>().Cooldown > 0;
            if (Projectile.owner == Main.myPlayer)
            {
                if (stop || !player.channel) { Projectile.ai[1] = 1; Projectile.netUpdate = true; }
                else
                {
                    float angle = (Main.MouseWorld - player.MountedCenter).ToRotation();
                    if (Math.Abs(MathHelper.WrapAngle(angle - Projectile.ai[0])) > .03f) { Projectile.ai[0] = angle; Projectile.netUpdate = true; }
                }
            }
            retiring = stop || Projectile.ai[1] == 1;
            if (retiring)
            {
                fade -= .12f;
                if (fade <= 0) Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 60;
                player.itemTime = player.itemAnimation = 2;
                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Math.Cos(Projectile.ai[0]) >= 0 ? 1 : -1);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.ai[0] - MathHelper.PiOver2);
            }
            Projectile.rotation = Projectile.ai[0];
            Projectile.Center = player.MountedCenter + Projectile.rotation.ToRotationVector2() * 23;
            if (!retiring && player.velocity.Length() > 5 && Main.GameUpdateCount % 4 == 0) CavernousRampartVFX.Spark(Projectile.Center, -player.velocity * .1f);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float speed = Main.player[Projectile.owner].velocity.Length();
            modifiers.SourceDamage *= MathHelper.Clamp(1 + (speed - 5) / 5, 1, 3);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.whoAmI == Main.myPlayer)
            {
                player.GetModPlayer<RampartPlayer>().ImpactCooldown = 30;
                player.immune = true;
                player.immuneTime = Math.Max(player.immuneTime, 8);
                for (int i = 0; i < player.hurtCooldowns.Length; i++) player.hurtCooldowns[i] = Math.Max(player.hurtCooldowns[i], 8);
                player.velocity *= .85f;
            }
            player.GetModPlayer<RampartPlayer>().GuardFlash = 1f;
            CavernousRampartVFX.Rubble(Projectile.Center, 6, 3f);
            CavernousRampartVFX.Burst(Projectile.Center, 18, 4);
            SoundEngine.PlaySound(SoundID.Item37 with { Volume = .45f }, Projectile.Center);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 side = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2);
            float collision = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - side * 18, Projectile.Center + side * 18, 14, ref collision);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D shield = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            float speed = Math.Max(0f, Vector2.Dot(player.velocity, Projectile.rotation.ToRotationVector2()));
            float power = Math.Clamp((speed - 3f) / 5f, 0f, 1f);
            float flash = player.GetModPlayer<RampartPlayer>().GuardFlash;
            Vector2 center = Projectile.Center - Main.screenPosition;
            CavernousRampartVFX.Glow(Projectile.Center, new Vector2(55f), CavernousRampartVFX.Violet, (0.2f + power * 0.25f + flash * 0.3f) * fade);
            Main.EntitySpriteDraw(shield, center, null, lightColor * fade, Projectile.rotation, shield.Size() * 0.5f, 1.7f, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, center, null, CavernousRampartVFX.Additive(CavernousRampartVFX.Aqua, (0.4f + power * 0.5f) * fade),
                Projectile.rotation, glow.Size() * 0.5f, 1.7f, SpriteEffects.None);
            Main.EntitySpriteDraw(shield, center, null, CavernousRampartVFX.Additive(Color.White, flash * fade), Projectile.rotation, shield.Size() * 0.5f, 1.7f, SpriteEffects.None);
            if (flash > 0.04f)
                CavernousRampartVFX.Ring(Projectile.Center, new Vector2(45f + (1f - flash) * 30f, 60f), CavernousRampartVFX.Aqua, flash * fade * 0.6f, Projectile.rotation);
            return false;
        }
        public override void OnKill(int timeLeft) => CavernousRampartVFX.Burst(Projectile.Center, 6, 2);
    }

    internal static class CavernousRampartRelicArt
    {
        internal const string Shield = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/CavernousRampart/CavernousRampartHeld";
    }

    public class CavernousRampartDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, CavernousRampartVFX.Additive(CavernousRampartVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class CavernousRampartVFX
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

        internal static void Ring(Vector2 center, Vector2 size, Color color, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/Ring/GlowRing", center, size, Additive(color, opacity), rotation);

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

        internal static void Rubble(Vector2 center, int count, float speed = 4f)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Dust.NewDustPerfect(center, ModContent.DustType<CavernousRampartDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
