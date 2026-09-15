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
    public class TumblerCommander : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/TumblerCommander/TumblerCommander";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Tumbler Commander", "Channel to guide a tiny tumbler towards your cursor\n The tumbler can scale up walls to reach your cursor")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Comandante Rodante")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Canaliza para guiar una pequeÃ±a roca rodante por suelos y paredes hacia el cursor\nLos objetivos lejanos la hacen rodar mÃ¡s rÃ¡pido\nConsume 4 de manÃ¡ cada medio segundo");
            this.AddSkillStrike(Language.Default, "Skill Strikes while rolling at sufficient speed");
            this.AddSkillStrike(Language.Spanish, "Golpea mientras ruedas a gran velocidad");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Channel to guide a tiny tumbler along floors and walls toward the cursor\nMore distant targets make it roll faster\nConsumes 4 mana each half-second"));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 17;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CommanderTumbler>();
            Item.shootSpeed = 1;
            Item.knockBack = 3;
            Item.channel = true;
            Item.mana = 4;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, Main.MouseWorld.X, Main.MouseWorld.Y);
            return false;
        }
    }
    public class CommanderTumbler : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/CrystalCaverns/TumblerockMedium";
        public override void SetStaticDefaults() { ProjectileID.Sets.TrailCacheLength[Type] = 7; ProjectileID.Sets.TrailingMode[Type] = 2; }
        private int age;
        private float charge;
        private float fade = 1;
        private float rollSpeed;
        private bool onGround;
        private bool retiring;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => !retiring && rollSpeed > 1 ? null : false;
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(retiring);
        public override void ReceiveExtraAI(BinaryReader reader) => retiring = reader.ReadBoolean();
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            age++;
            if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<TumblerCommander>()) retiring = true;
            if (Projectile.owner == Main.myPlayer)
            {
                if (!player.channel && !retiring) { retiring = true; Projectile.netUpdate = true; }
                if (!retiring && age % 30 == 0 && !player.CheckMana(player.HeldItem, -1, true)) { retiring = true; Projectile.netUpdate = true; }
                if (!retiring && age % 6 == 0)
                {
                    Vector2 target = Main.MouseWorld;
                    if (Vector2.DistanceSquared(target, player.Center) > 800 * 800) target = player.Center + (target - player.Center).SafeNormalize(Vector2.UnitX) * 800;
                    Projectile.ai[1] = target.X;
                    Projectile.ai[2] = target.Y;
                    Projectile.netUpdate = true;
                }
            }
            if (Vector2.DistanceSquared(player.Center, Projectile.Center) > 1200 * 1200) retiring = true;
            if (retiring)
            {
                Projectile.friendly = false;
                Projectile.velocity *= .9f;
                fade -= .05f;
                if (fade <= 0) Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 60;
            player.itemTime = player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;
            player.manaRegenDelay = 60;
            Vector2 aim = (new Vector2(Projectile.ai[1], Projectile.ai[2]) - player.MountedCenter).SafeNormalize(Vector2.UnitX);
            player.ChangeDir(aim.X >= 0 ? 1 : -1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -player.direction * MathHelper.PiOver2 - 0.2f * player.gravDir);
            Vector2 delta = new Vector2(Projectile.ai[1], Projectile.ai[2]) - Projectile.Center;
            float desiredSpeed = MathHelper.Clamp(delta.Length() / 45, 1.5f, 8);
            int drive = Math.Abs(delta.X) < 9 ? 0 : Math.Sign(delta.X);
            bool floor = onGround;
            int wall = (int)Projectile.ai[0];
            if (wall == 0 && drive != 0 && HasWall(drive))
            {
                wall = drive;
                Projectile.ai[0] = wall;
                Projectile.netUpdate = true;
            }
            if (wall != 0)
            {
                bool touching = HasWall(wall);
                if (!touching || drive == -wall)
                {
                    Projectile.ai[0] = 0;
                    Projectile.velocity = new Vector2((drive == -wall ? -wall : wall) * 2.5f, Math.Min(0f, Projectile.velocity.Y * 0.3f));
                    wall = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    float vertical = delta.Y > 25 ? desiredSpeed : -desiredSpeed;
                    if (Math.Abs(delta.Y) < 9 && Math.Abs(delta.X) < 30) vertical = 0;
                    if (vertical != 0f && Collision.SolidCollision(Projectile.position + new Vector2(0, Math.Sign(vertical) * 3), Projectile.width, Projectile.height)) vertical = 0f;
                    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, vertical, .16f);
                    Projectile.velocity.X = wall * .5f;
                    Projectile.rotation -= Projectile.velocity.Y * wall / 13;
                }
            }
            else
            {
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, drive * desiredSpeed, floor ? .12f : .035f);
                Projectile.velocity.Y = Math.Min(12, Projectile.velocity.Y + .35f);
                Projectile.rotation += Projectile.velocity.X / 13;
            }
            rollSpeed = wall != 0 ? Math.Abs(Projectile.velocity.Y) : floor ? Math.Abs(Projectile.velocity.X) : 0;
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (rollSpeed >= 6) SkillStrikeUtil.setSkillStrike(Projectile, 1.6f, 100, .35f, .6f);
            if (rollSpeed > 3 && age % 4 == 0) TumblerCommanderVFX.Spark(Projectile.Center, -Projectile.velocity * .15f, rollSpeed >= 6 ? .35f : .2f);
            charge = MathHelper.Lerp(charge, rollSpeed >= 6f ? 1f : 0f, 0.2f);
            if (rollSpeed > 2f && age % 5 == 0)
            {
                Vector2 contact = wall != 0 ? Projectile.Center + new Vector2(wall * 13f, 0f) : Projectile.Bottom;
                TumblerCommanderVFX.Smoke(contact, -Projectile.velocity * 0.12f, 24f, new Color(115, 130, 150));
            }
            Lighting.AddLight(Projectile.Center, TumblerCommanderVFX.Aqua.ToVector3() * (0.2f + charge * 0.3f));
        }
        private bool HasWall(int direction)
        {
            int x = (int)((Projectile.Center.X + direction * (Projectile.width * 0.5f + 4f)) / 16f);
            for (int y = (int)(Projectile.Top.Y + 3f) / 16; y <= (int)(Projectile.Center.Y + 3f) / 16; y++)
            {
                if (!WorldGen.InWorld(x, y, 2)) continue;
                Tile tile = Main.tile[x, y];
                if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock)
                    return true;
            }
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => false;
        public override void PostAI()
        {
            if (Projectile.ai[0] == 0f && Projectile.velocity.Y >= 0f)
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            Vector2 oldVelocity = Projectile.velocity;
            if (Projectile.ai[0] == 0f)
            {
                Vector4 downhill = Collision.WalkDownSlope(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, .35f);
                Projectile.velocity = new Vector2(downhill.Z, downhill.W);
            }
            Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, false, false);
            Projectile.position += Projectile.velocity;
            Vector4 slope = Collision.SlopeCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.position = new Vector2(slope.X, slope.Y);
            Projectile.velocity = new Vector2(slope.Z, slope.W);
            onGround = oldVelocity.Y >= 0f && Math.Abs(Projectile.velocity.Y) < .01f;
        }
        public override void OnKill(int timeLeft) => TumblerCommanderVFX.Burst(Projectile.Center, 18, 3);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => TumblerCommanderVFX.Burst(Projectile.Center, 10, 3);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new(0, (Projectile.identity % 2) * 32, 32, 32);
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
                if (Projectile.oldPos[i] != Vector2.Zero)
                    Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, frame,
                        TumblerCommanderVFX.Additive(TumblerCommanderVFX.Aqua, (1f - i / 7f) * charge * fade * 0.35f), Projectile.oldRot[i], frame.Size() * 0.5f, 0.875f, SpriteEffects.None);
            TumblerCommanderVFX.Glow(Projectile.Center, new Vector2(48f), TumblerCommanderVFX.Violet, (0.15f + charge * 0.4f) * fade);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor * fade, Projectile.rotation, frame.Size() * 0.5f, 0.875f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, TumblerCommanderVFX.Additive(Color.White, charge * 0.5f * fade),
                Projectile.rotation, frame.Size() * 0.5f, 0.875f, SpriteEffects.None);
            if (rollSpeed >= 6f && !retiring)
                TumblerCommanderVFX.Flare(Projectile.Center, 34f, fade * 0.5f, Projectile.rotation);
            Player player = Main.player[Projectile.owner];
            if (!retiring)
            {
                Texture2D rod = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/CrystalCaverns/TumblerCommander/TumblerCommander").Value;
                SpriteEffects flip = player.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 grip = new(player.direction > 0 ? 3f : rod.Width - 3f, rod.Height - 4f);
                Vector2 hand = player.MountedCenter + new Vector2(player.direction * 16f, -4f * player.gravDir);
                float rodAngle = player.direction * -0.12f * player.gravDir;
                Main.EntitySpriteDraw(rod, hand - Main.screenPosition, null, lightColor * fade, rodAngle, grip, 1f, flip);
                Vector2 tipOffset = new(player.direction * 22f, -18f);
                Vector2 rodTip = hand + tipOffset.RotatedBy(rodAngle);
                TumblerCommanderVFX.Glow(rodTip, new Vector2(20f), TumblerCommanderVFX.Aqua, (0.2f + charge * 0.15f) * fade);
            }
            return false;
        }
    }

    internal static class TumblerCommanderVFX
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
    }
}
