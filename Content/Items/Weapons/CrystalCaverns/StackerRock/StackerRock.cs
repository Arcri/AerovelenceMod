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
    public class StackerRock : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/StackerRock/StackerRock";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Stacker Rock", "Throws discs that linger on the floor; More can be piled up in one stack\nHit a tower from the side to topple it, sending rocks rolling")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Roca Apilable")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Rocas hechas para apilar\nLos discos se anclan en suelos sólidos y se apilan desde arriba\nCada roca añadida prolonga la duración de la torre\nGolpea una torre por el costado para derribarla antes\nLas rocas derribadas caen, rebotan y ruedan hacia delante");
            this.AddSkillStrike(Language.Default, "Topple a tower of at least five rocks to Skill Strike");
            this.AddSkillStrike(Language.Spanish, "Derriba una torre de al menos cinco rocas");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Throws discs that linger on the floor; More can be piled up in one stack\nHit a tower from the side to topple it, sending rocks rolling"));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StackerRockShot>();
            Item.shootSpeed = 9;
            Item.knockBack = 3;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 3);
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }
        public override void AddRecipes() => CreateRecipe(30).AddIngredient<CavernStoneItem>(3).AddTile(TileID.WorkBenches).Register();
    }
    public class StackerRockShot : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/StackerRock/StackerRockRock1";
        public override bool? CanDamage() => Projectile.timeLeft <= 15 ? false : null;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(14, Projectile.velocity.Y + .25f);
            Projectile.rotation = Projectile.velocity.X * .025f;
            if (Projectile.owner != Main.myPlayer) return;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.owner != Projectile.owner || p.ModProjectile is not StackerRockTower tower || p.ai[2] != 0) continue;
                float collision = 0;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, new Vector2(p.Center.X, tower.Top), 1, 1) || !Collision.CheckAABBvLineCollision(new Vector2(p.Center.X - 13, tower.Top), new Vector2(26, tower.Height),
                    Projectile.Center, Projectile.Center + Projectile.velocity, 8, ref collision)) continue;
                bool fromAbove = Projectile.velocity.Y > 0 && Projectile.Center.Y <= tower.Top + 5 && Math.Abs(Projectile.Center.X - p.Center.X) < 21;
                if (fromAbove) tower.Stack();
                else tower.Topple(Projectile.velocity.X >= 0 ? 1 : -1);
                Projectile.Kill();
                return;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.owner == Main.myPlayer && oldVelocity.Y > 0 && Projectile.velocity.Y != oldVelocity.Y)
            {
                Vector2 landing = Projectile.Bottom + Projectile.velocity;
                Point support = (landing + new Vector2(0, 2)).ToTileCoordinates();
                if (WorldGen.InWorld(support.X, support.Y, 2))
                {
                    Tile tile = Main.tile[support.X, support.Y];
                    if (tile.HasTile && !tile.IsActuated && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                    {
                        int standing = 0;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile p = Main.projectile[i];
                            if (p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<StackerRockTower>() && p.ai[2] == 0) standing++;
                        }
                        if (standing < 6) Projectile.NewProjectile(Projectile.GetSource_FromThis(), landing, Vector2.Zero,
                            ModContent.ProjectileType<StackerRockTower>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 240);
                    }
                }
            }
            return true;
        }
        public override void OnKill(int timeLeft) => StackerRockVFX.Burst(Projectile.Center, 5, 1.8f);
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 15f);
            StackerRockRelicArt.Disc(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.rotation, new Vector2(20f, 8f), lightColor, fade, 0.2f, Projectile.identity);
            return false;
        }
    }
    public class StackerRockTower : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/StackerRock/StackerRockRock1";
        public int Count => Math.Clamp((int)Projectile.ai[0], 1, 12);
        private const float RockHeight = 14f;
        public float Height => Count * RockHeight;
        public float Top => Projectile.Center.Y - Height;
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public void Stack()
        {
            if (Projectile.ai[2] != 0f) return;
            if (Collision.SolidCollision(new Vector2(Projectile.Center.X - 10f, Top - RockHeight), 20, (int)RockHeight))
            {
                Topple(Main.player[Projectile.owner].direction);
                return;
            }
            if (Count >= 12) { Topple(Main.player[Projectile.owner].direction); return; }
            Projectile.ai[0] = Count + 1;
            Projectile.ai[1] = Math.Min(900, Projectile.ai[1] + 120);
            Projectile.netUpdate = true;
            StackerRockVFX.Burst(new Vector2(Projectile.Center.X, Top), Count == 5 ? 18 : 6, 2.5f);
            SoundEngine.PlaySound(SoundID.Tink with { Volume = .4f, Pitch = Count * .04f }, Projectile.Center);
        }
        public void Topple(int direction)
        {
            if (Projectile.ai[2] != 0 || Projectile.owner != Main.myPlayer) return;
            Projectile.ai[2] = direction >= 0 ? 1 : -1;
            for (int i = 0; i < Count; i++)
            {
                float height = i / (float)Math.Max(1, Count - 1);
                Vector2 position = Projectile.Center - Vector2.UnitY * ((i + 0.5f) * RockHeight);
                Vector2 velocity = new(Projectile.ai[2] * (2.8f + height * 3.2f), -0.6f - height * 0.7f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity,
                    ModContent.ProjectileType<StackerRockSkipper>(), (int)(Projectile.damage * (0.6f + Count * 0.08f)),
                    Projectile.knockBack, Projectile.owner, Projectile.identity + i, Count, 30f - i % 3);
            }
            SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.45f, Pitch = -0.2f }, Projectile.Center);
            Projectile.Kill();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || Vector2.DistanceSquared(player.Center, Projectile.Center) > 1800 * 1800)
            {
                Projectile.ai[2] = 2f;
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);
                return;
            }
            if (Projectile.ai[2] != 0f) return;
            Projectile.timeLeft = 60;
            Projectile.ai[1]--;
            bool supported = Collision.SolidCollision(Projectile.Center + new Vector2(-6, 1), 12, 3);
            if (Projectile.owner == Main.myPlayer && (Projectile.ai[1] <= 0 || !supported)) Topple(player.direction);
            if (Count >= 5) Lighting.AddLight(Projectile.Center - Vector2.UnitY * Height * 0.5f, new Vector3(0.12f, 0.23f, 0.3f));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Projectile.ai[2] == 0f ? 1f : MathHelper.Clamp(Projectile.timeLeft / 20f, 0f, 1f);
            float instability = Projectile.ai[2] == 0f ? MathHelper.Clamp(1f - Projectile.ai[1] / 100f, 0f, 1f) : 0.15f;
            for (int i = 0; i < Count; i++)
            {
                float shake = MathF.Sin(Main.GlobalTimeWrappedHourly * 24f + i * 2.3f) * instability * (1f + i * 0.2f);
                Vector2 center = Projectile.Center + new Vector2(shake, -(i + 0.5f) * RockHeight).RotatedBy(Projectile.rotation);
                float charge = Count >= 5 ? 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f + i) * 0.15f : 0f;
                StackerRockRelicArt.Disc(Main.spriteBatch, center - Main.screenPosition, Projectile.rotation + shake * 0.02f,
                    new Vector2(30f - i % 3, RockHeight), lightColor, fade, charge, Projectile.identity + i);
            }
            return false;
        }
        public override void OnKill(int timeLeft) => StackerRockVFX.Burst(Projectile.Center, Count * 2, 2);
    }

    public class StackerRockSkipper : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/StackerRock/StackerRockRock1";
        private bool grounded;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 210;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => false;
        public override void AI()
        {
            Projectile.velocity.X *= grounded ? 0.982f : 0.998f;
            Projectile.velocity.Y = Math.Min(12f, Projectile.velocity.Y + 0.3f);
            if (grounded)
            {
                Vector4 downhill = Collision.WalkDownSlope(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, 0.3f);
                Projectile.velocity = new Vector2(downhill.Z, downhill.W);
            }
            Vector2 oldPosition = Projectile.position;
            Vector2 incoming = Projectile.velocity;
            Vector2 movement = Collision.TileCollision(Projectile.position, incoming, Projectile.width, Projectile.height, false, false);
            Projectile.position += movement;
            Vector4 slope = Collision.SlopeCollision(Projectile.position, movement, Projectile.width, Projectile.height);
            Projectile.position = new Vector2(slope.X, slope.Y);
            Projectile.velocity = new Vector2(slope.Z, slope.W);
            grounded = incoming.Y >= 0f && Projectile.velocity.Y < incoming.Y - 0.01f;
            if (movement.X != incoming.X)
                Projectile.velocity.X = -incoming.X * 0.45f;
            if (grounded)
            {
                Projectile.velocity.Y = incoming.Y > 1.4f ? -incoming.Y * 0.42f : 0f;
                Projectile.velocity.X *= incoming.Y > 1.4f ? 0.88f : 1f;
                if (incoming.Y > 2f)
                {
                    StackerRockVFX.Burst(Projectile.Bottom, 3, 1.2f);
                    SoundEngine.PlaySound(SoundID.Tink with { Volume = Math.Min(0.3f, incoming.Y * 0.025f), Pitch = (Projectile.ai[0] % 3) * 0.13f, MaxInstances = 3 }, Projectile.Center);
                }
                grounded = Projectile.velocity.Y == 0f;
            }
            else if (incoming.Y < 0f && movement.Y != incoming.Y)
                Projectile.velocity.Y = -incoming.Y * 0.25f;
            Projectile.rotation += (Projectile.position.X - oldPosition.X) / 11f;
            if (grounded && Math.Abs(Projectile.velocity.X) < 0.16f)
            {
                Projectile.velocity.X = 0f;
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 25);
            }
            if (Projectile.ai[1] >= 5f) SkillStrikeUtil.setSkillStrike(Projectile, 1.6f, 100, 0.4f, 0.7f);
        }
        public override bool? CanDamage() => Projectile.timeLeft > 15 && Projectile.velocity.LengthSquared() > 0.16f ? null : false;
        public override bool PreDraw(ref Color lightColor)
        {
            StackerRockRelicArt.Disc(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.rotation,
                new Vector2(Math.Clamp(Projectile.ai[2], 28f, 30f), 14f), lightColor, Math.Min(1f, Projectile.timeLeft / 25f),
                Projectile.ai[1] >= 5f ? 0.7f : 0f, (int)Projectile.ai[0]);
            return false;
        }
        public override void OnKill(int timeLeft) => StackerRockVFX.Burst(Projectile.Center, 3, 1.2f);
    }

    internal static class StackerRockRelicArt
    {
        internal static void Disc(SpriteBatch batch, Vector2 center, float rotation, Vector2 size, Color light, float opacity, float charge, int variant = 0)
        {
            Texture2D rock = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/CrystalCaverns/StackerRock/StackerRockRock" + (Math.Abs(variant % 3) + 1)).Value;
            Rectangle frame = rock.Bounds;
            Vector2 scale = size / frame.Size();
            if (charge > 0f)
                for (int i = 0; i < 4; i++)
                    batch.Draw(rock, center + (i * MathHelper.PiOver2).ToRotationVector2(), frame, StackerRockVFX.Additive(Color.White, opacity * charge * 0.65f),
                        rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            batch.Draw(rock, center, frame, Color.Lerp(light, Color.White, 0.15f) * opacity, rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            batch.Draw(rock, center, frame, StackerRockVFX.Additive(StackerRockVFX.Aqua, opacity * charge * 0.3f), rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    public class StackerRockDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, StackerRockVFX.Additive(StackerRockVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class StackerRockVFX
    {
        internal const string RockTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ChargedStoneProjectile";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);
        internal static readonly Rectangle RockFrame = new(4, 6, 54, 48);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

        internal static void Rock(Vector2 center, float rotation, float size, Color light, float opacity = 1f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(RockTexture).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, RockFrame, Color.Lerp(light, Color.White, 0.2f) * opacity,
                rotation, RockFrame.Size() * 0.5f, size / RockFrame.Width, SpriteEffects.None);
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
                Dust.NewDustPerfect(center, ModContent.DustType<StackerRockDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
