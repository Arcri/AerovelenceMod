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
    public class PinCushionStone : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/PinCushionStone/PinCushionStone";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Pin-Cushion Stone", "First throw launches a floating pin-cushion stone\nFurther throws fire darts that pierce one enemy\nDarts lodge in the stone and nudge it, empowering its next impacts\nEvery third empowered collision rebounds much harder\nAfter 10 seconds without a dart, the stone dissolves and releases its pins")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Piedra Alfiletero")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "El primer lanzamiento crea una piedra flotante\nLos siguientes lanzan dardos que atraviesan a un enemigo\nLos dardos se incrustan en la piedra y la empujan, potenciando sus impactos\nCada tercer impacto potenciado rebota con más fuerza\nTras 10 segundos sin recibir dardos, la piedra libera sus púas");
            this.AddSkillStrike(Language.Default, "Strike enemies with the stone shortly after pushing it with a dart");
            this.AddSkillStrike(Language.Spanish, "Golpea a los enemigos con la piedra poco después de empujarla con un dardo");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "First throw launches a floating pin-cushion stone\nFurther throws fire darts that pierce one enemy\nDarts lodge in the stone and nudge it, empowering its next impacts\nEvery third empowered collision rebounds much harder\nAfter 10 seconds without a dart, the stone dissolves and releases its pins"));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PinCushionBall>();
            Item.shootSpeed = 8;
            Item.knockBack = 3;
            Item.noUseGraphic = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool ball = PinCushionBall.Find(player.whoAmI) != null;
            Projectile.NewProjectile(source, position, velocity * (ball ? 1.7f : PinCushionBall.MovementScale), ball ? ModContent.ProjectileType<PinCushionDart>() : type,
                ball ? (int)(damage * .65f) : damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class PinCushionDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<global::AerovelenceMod.Content.NPCs.CrystalCaverns.Condurtle>())
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PinCushionStone>(), 15));
        }
    }
    public class PinCushionBall : ModProjectile
    {
        internal const float MovementScale = 1f / 3f;
        public override string Texture => PinCushionStoneRelicArt.Pincushion;
        private readonly List<float> pins = new();
        private int consecutive;
        public static PinCushionBall Find(int owner)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == owner && p.ModProjectile is PinCushionBall ball && p.ai[0] < 600) return ball;
            }
            return null;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
            Projectile.netImportant = true;
        }
        public override bool? CanDamage() => Projectile.ai[0] < 600 && Projectile.velocity.LengthSquared() > MovementScale * MovementScale ? null : false;
        public void Embed(Projectile dart)
        {
            if (Projectile.ai[0] >= 600f) return;
            Projectile.ai[2] = 1f;
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 90;
            Vector2 push = dart.velocity.SafeNormalize(Vector2.UnitX) * (1.5f * MovementScale);
            Projectile.velocity += push;
            if (Projectile.velocity.Length() > 10f * MovementScale) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (10f * MovementScale);
            if (pins.Count < 24) pins.Add((dart.Center - Projectile.Center).ToRotation() - Projectile.rotation);
            Projectile.netUpdate = true;
            PinCushionStoneVFX.Burst(dart.Center, 7, 2);
            SoundEngine.PlaySound(SoundID.Tink with { Volume = .35f, Pitch = .5f }, Projectile.Center);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)pins.Count);
            foreach (float pin in pins) writer.Write(pin);
            writer.Write((byte)consecutive);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            pins.Clear();
            int count = reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                float angle = reader.ReadSingle();
                if (i < 24) pins.Add(angle);
            }
            consecutive = reader.ReadByte();
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || Vector2.DistanceSquared(owner.Center, Projectile.Center) > 1600 * 1600) Projectile.ai[0] = Math.Max(600, Projectile.ai[0]);
            if (++Projectile.ai[0] < 600) Projectile.timeLeft = 30;
            Projectile.ai[1] = Math.Max(0, Projectile.ai[1] - 1);
            Projectile.ai[2] *= 0.88f;
            Projectile.velocity *= .994f;
            Projectile.rotation += Projectile.velocity.X * .025f;
            if (Projectile.ai[0] >= 600) { Projectile.velocity *= .9f; Projectile.tileCollide = false; }
            if (Projectile.ai[1] <= 0) consecutive = 0;
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (Projectile.ai[1] > 0) SkillStrikeUtil.setSkillStrike(Projectile, 1.65f, 100, .4f, .7f);
            if (Projectile.ai[1] > 0 && Main.GameUpdateCount % 5 == 0) PinCushionStoneVFX.Spark(Projectile.Center, -Projectile.velocity * .12f);
            Lighting.AddLight(Projectile.Center, new Vector3(.07f, .15f, .25f));
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X * .85f;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y * .85f;
            PinCushionStoneVFX.Burst(Projectile.Center, 4, 1.5f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer) return;
            consecutive = Projectile.ai[1] > 0 ? consecutive + 1 : 0;
            bool rebound = consecutive >= 3;
            if (rebound) consecutive = 0;
            Projectile.ai[2] = rebound ? 2f : 1f;
            Vector2 normal = (Projectile.Center - target.Center).SafeNormalize(-Projectile.velocity.SafeNormalize(Vector2.UnitX));
            float speed = Math.Max(2.5f * MovementScale, Projectile.velocity.Length());
            Projectile.velocity = normal * (rebound ? Math.Min(14f * MovementScale, speed * 1.8f + 3f * MovementScale) : speed * .9f);
            Projectile.netUpdate = true;
            PinCushionStoneVFX.Burst(Projectile.Center, rebound ? 22 : 10, rebound ? 5 : 3);
            SoundEngine.PlaySound(SoundID.Item37 with { Volume = .4f, Pitch = rebound ? -.3f : .2f }, Projectile.Center);
        }
        public override void OnKill(int timeLeft)
        {
            PinCushionStoneVFX.Burst(Projectile.Center, 20, 4);
            if (Projectile.owner != Main.myPlayer || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead) return;
            foreach (float pin in pins)
            {
                Vector2 direction = (pin + Projectile.rotation).ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + direction * 18, direction * 2.5f - Vector2.UnitY,
                    ModContent.ProjectileType<PinCushionDart>(), (int)(Projectile.damage * .5f), 1, Projectile.owner, 1);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Projectile.ai[0] >= 600f ? Projectile.timeLeft / 30f : 1f;
            Texture2D rock = ModContent.Request<Texture2D>(Texture).Value;
            float charge = Projectile.ai[1] / 90f;
            PinCushionStoneVFX.Glow(Projectile.Center, new Vector2(54f), PinCushionStoneVFX.Violet, (0.15f + charge * 0.3f) * fade);
            foreach (float pin in pins)
            {
                float angle = pin + Projectile.rotation;
                Vector2 axis = angle.ToRotationVector2();
                PinCushionStoneVFX.Sprite(PinCushionStoneRelicArt.Needle, Projectile.Center + axis * 18f, new Vector2(8f, 15f), Color.White * fade, angle + MathHelper.PiOver2);
            }
            Main.EntitySpriteDraw(rock, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.25f) * fade,
                Projectile.rotation, rock.Size() * 0.5f, 30f / rock.Width, SpriteEffects.None);
            Main.EntitySpriteDraw(rock, Projectile.Center - Main.screenPosition, null, PinCushionStoneVFX.Additive(PinCushionStoneVFX.Aqua, (charge * 0.45f + Projectile.ai[2] * 0.4f) * fade),
                Projectile.rotation, rock.Size() * 0.5f, 30f / rock.Width, SpriteEffects.None);
            for (int i = 0; i < consecutive; i++)
                PinCushionStoneVFX.Crystal(Projectile.Center + new Vector2((i * 2f - consecutive + 1f) * 7f, -27f), 0f, new Vector2(5f, 10f), fade, 0.5f);
            if (Projectile.ai[2] > 0.05f)
                PinCushionStoneVFX.Ring(Projectile.Center, new Vector2(40f + (1f - Math.Min(1f, Projectile.ai[2])) * 30f), Color.White, Math.Min(0.7f, Projectile.ai[2]) * fade);
            return false;
        }
    }
    public class PinCushionDart : ModProjectile
    {
        public override string Texture => PinCushionStoneRelicArt.Needle;
        public override void SetStaticDefaults() { ProjectileID.Sets.TrailCacheLength[Type] = 5; ProjectileID.Sets.TrailingMode[Type] = 2; }
        public override bool? CanDamage() => Projectile.timeLeft <= 15 ? false : null;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 1) Projectile.velocity.Y = Math.Min(12, Projectile.velocity.Y + .22f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0 && Projectile.owner == Main.myPlayer)
            {
                PinCushionBall ball = PinCushionBall.Find(Projectile.owner);
                if (ball != null)
                {
                    float collision = 0;
                    if (Collision.CanHitLine(Projectile.Center, 1, 1, ball.Projectile.Center, 1, 1) && Collision.CheckAABBvLineCollision(ball.Projectile.position, ball.Projectile.Size, Projectile.Center,
                        Projectile.Center + Projectile.velocity, 7, ref collision))
                    {
                        ball.Embed(Projectile);
                        Projectile.Kill();
                    }
                }
            }
        }
        public override void OnKill(int timeLeft) => PinCushionStoneVFX.Burst(Projectile.Center, 4, 1.5f);
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 15f);
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
                if (Projectile.oldPos[i] != Vector2.Zero)
                    PinCushionStoneVFX.Sprite(Texture, Projectile.oldPos[i] + Projectile.Size * 0.5f, new Vector2(7f, 13f),
                        PinCushionStoneVFX.Additive(PinCushionStoneVFX.Aqua, (1f - i / 5f) * fade * 0.3f), Projectile.oldRot[i] + MathHelper.PiOver2);
            PinCushionStoneVFX.Sprite(Texture, Projectile.Center, new Vector2(10f, 17f), Color.White * fade, Projectile.rotation + MathHelper.PiOver2);
            return false;
        }
    }

    internal static class PinCushionStoneRelicArt
    {
        internal const string Needle = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/PinCushionStone/PinCushionNeedle";
        internal const string Pincushion = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/PinCushionStone/PinCushionStoneStone";
    }

    internal static class PinCushionStoneVFX
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
    }
}
