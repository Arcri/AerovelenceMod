using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Crimson
{
    public class NerveFlagellator : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("NerveFlagellator", "Attacks with multiple nerve tendrils\nMarked enemies may spawn a friendly nerve creeper when hit")
            .AddName(Language.Default, "Nerve Flagellator").AddTooltip(Language.Default, "Attacks with multiple nerve tendrils\nMarked enemies may spawn a friendly nerve creeper when hit")
            .AddSkillStrike(Language.Default, "The small nerve tendrils Skill Strike")

            .AddName(Language.Spanish, "Flagelador Nervioso").AddTooltip(Language.Spanish, "Ataca con múltiples zarcillos nerviosos\nLos enemigos marcados pueden generar un rastreador nervioso amistoso al ser golpeados").AddSkillStrike(Language.Spanish, "Los pequeños zarcillos nerviosos realizan Golpes de Habilidad")
            .AddName(Language.French, "Flagellateur Nerveux").AddTooltip(Language.French, "Attaque avec plusieurs vrilles nerveuses\nLes ennemis marqués peuvent invoquer un rampant suiveur allié lorsqu'ils sont touchés").AddSkillStrike(Language.French, "Les petites vrilles nerveuses déclenchent des Coups de Compétence")
            .AddName(Language.German, "Nervenpeitscher").AddTooltip(Language.German, "Greift mit mehreren Nervenranken an\nMarkierte Feinde können einen freundlichen Nervenkreischer beschwören, wenn sie getroffen werden").AddSkillStrike(Language.German, "Die kleinen Nervenranken führen Fähigkeitsschläge aus")
            .AddName(Language.Italian, "Flagellatore Nervoso").AddTooltip(Language.Italian, "Attacca con più viticci nervosi\nI nemici marchiati possono evocare un rampicante nervoso amichevole quando colpiti").AddSkillStrike(Language.Italian, "I piccoli viticci nervosi eseguono Colpi dell'Abilità")
            //.AddName(Language.Polish, "Bicz Nerwowy").AddTooltip(Language.Polish, "Atakuje wieloma nerwowymi mackami\nOznaczeni wrogowie mogą przywołać przyjaznego nerwowego pełzacza po trafieniu").AddSkillStrike(Language.Polish, "Małe nerwowe macki wykonują Ciosy Umiejętności")
            //.AddName(Language.PortugueseBrazil, "Flagelador Nervoso").AddTooltip(Language.PortugueseBrazil, "Ataca com vários tentáculos nervosos\nInimigos marcados podem invocar um rastejador nervoso aliado ao serem atingidos").AddSkillStrike(Language.PortugueseBrazil, "Os pequenos tentáculos nervosos realizam Golpes de Habilidade")
            .AddName(Language.Russian, "Нервный Флагеллятор").AddTooltip(Language.Russian, "Атакует несколькими нервными щупальцами\nПомеченные враги могут призвать дружелюбного нервного ползучего при попадании").AddSkillStrike(Language.Russian, "Маленькие нервные щупальца активируют Навык Удара");
            //.AddName(Language.ChineseTraditional, "神經鞭笞者").AddTooltip(Language.ChineseTraditional, "使用多條神經觸鬚攻擊\n被標記的敵人被擊中時可能會生成友好的神經爬行者").AddSkillStrike(Language.ChineseTraditional, "小型神經觸鬚會發動技能打擊")
            //.AddName(Language.ChineseSimplified, "神经鞭笞者").AddTooltip(Language.ChineseSimplified, "使用多条神经触须攻击\n被标记的敌人被击中时可能会生成友好的神经爬行者").AddSkillStrike(Language.ChineseSimplified, "小型神经触须会发动技能打击");
        }

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<NerveFlagellatorProjectile>(), 50, 2f, 4f);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 75, 0);
            Item.damage = 24;
            Item.knockBack = 2.5f;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.channel = false;
        }

        public override bool MeleePrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int tendrilCount = Main.rand.Next(2, 4);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, 0f);
            for (int i = 0; i < tendrilCount - 1; i++)
            {
                float angleOffset = Main.rand.NextFloat(-0.25f, 0.25f);
                Vector2 newVelocity = velocity.RotatedBy(angleOffset);
                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI, 0f, 1f, i);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 12)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class NerveFlagellatorProjectile : ModProjectile
    {
        private bool _isMainTendril = true;
        private bool _isUpsideDown = false;
        private int _childIndex = -1;

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
            Projectile.WhipSettings.Segments = 12;
            Projectile.WhipSettings.RangeMultiplier = 1f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == 1f)
            {
                _isMainTendril = false;
                _childIndex = (int)Projectile.ai[2];
                _isUpsideDown = _childIndex % 2 == 1;
                if (_childIndex == 0)
                    Projectile.WhipSettings.RangeMultiplier *= Main.rand.NextFloat(0.4f, 0.5f);
                else
                    Projectile.WhipSettings.RangeMultiplier *= Main.rand.NextFloat(0.25f, 0.35f);
                Projectile.WhipSettings.Segments = (int)(Projectile.WhipSettings.Segments * 0.7f);
            }
            else
                _isMainTendril = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            if (_isUpsideDown)
                Projectile.spriteDirection = Projectile.velocity.X >= 0f ? -1 : 1;
            else
                Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Timer++;
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }
            if (_isMainTendril)
                owner.heldProj = Projectile.whoAmI;
            else
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
            if (_isMainTendril && Timer == swingTime / 2)
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }
            SpawnDust(swingTime);
        }

        private void SpawnDust(float swingTime)
        {
            float swingProgress = Timer / swingTime;

            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
            {
                List<Vector2> points = [];
                Projectile.FillWhipControlPoints(Projectile, points);
                if (points.Count < 10)
                    return;
                int maxIndex = points.Count - 1;
                int pointIndex = Math.Max(0, Math.Min(maxIndex, points.Count - 10 + Main.rand.Next(9)));
                if (pointIndex <= 0 || pointIndex >= points.Count)
                    return;
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = Main.rand.NextBool() ? DustID.Blood : DustID.RedTorch;
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);

                dust.position = points[pointIndex];
                dust.fadeIn = 0.3f;
                Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += spinningPoint.RotatedBy(Main.player[Projectile.owner].direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.MinionAttackTargetNPC = target.whoAmI;

            if (_isMainTendril && Main.rand.NextBool(2))
            {
                bool hasActiveNerveCreeper = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<NerveCreeper>())
                    {
                        hasActiveNerveCreeper = true;
                        break;
                    }
                }
                if (!hasActiveNerveCreeper)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NerveCreeper>(), Projectile.damage / 2, 0f, Projectile.owner);
                    //Main.NewText("NerveCreeper spawned", 175, 75, 255);
                }
            }

            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        private void DrawLine(List<Vector2> list)
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
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Red);
                Vector2 scale = new(1, (diff.Length() + 1) / frame.Height);
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);
                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = [];
            Projectile.FillWhipControlPoints(Projectile, list);

            Texture2D texture;
            if (_isMainTendril)
            {
                DrawLine(list);
                texture = TextureAssets.Projectile[Projectile.type].Value;
            }

            else
            {
                texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Crimson/NerveFlagellatorProjectileMini").Value;
            }

            SpriteEffects flip = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                flip = SpriteEffects.FlipHorizontally;
            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 14, 26);
                Vector2 origin = new(5, 8);
                float scale = 1;
                if (i == list.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;
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
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;
                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                if (_isUpsideDown)
                    rotation += MathHelper.Pi;
                Color color = Lighting.GetColor(element.ToTileCoordinates());
                color = color.MultiplyRGB(new Color(255, 200, 200));
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }

    public class NerveCreeper : ModProjectile
    {
        private float OrbitDistance = 50f;
        private float OrbitSpeed = 0.03f;
        private float AttackDistance = 400f;
        private float MaxChaseSpeed = 9f;
        private float MinChaseSpeed = 3f;
        private float IdleSpeed = 0.5f;
        private float AccelerationRate = 0.08f;
        private float RotationSmoothing = 3f;

        private bool _attacking;
        private int _targetNPC = -1;
        private float _orbitAngle;
        private int _attackCooldown;
        private int _idleTime;
        private Vector2 _currentVelocity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanCutTiles() => false;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.3f;

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            if (_attackCooldown > 0)
                _attackCooldown--;

            if(Projectile.timeLeft <= 70)
            {
                Projectile.scale *= 0.90f;
            }
            else
            {
                Projectile.scale = 1f + 0.05f * (float)Math.Sin(Main.GameUpdateCount * 0.1f);
            }

            if(Projectile.scale <= 0.11f)
            {
                Projectile.Kill();
            }

            FindTarget(owner);
            if (_attacking && _targetNPC >= 0 && _targetNPC < Main.maxNPCs && Main.npc[_targetNPC].active)
                AttackTarget(owner);
            else
                IdleOrbit(owner);
            Projectile.velocity = _currentVelocity;
            
            SpawnTrailingDust();
        }

        private void SpawnTrailingDust()
        {
            float speed = Projectile.velocity.Length();
            int dustChance = speed > 6f ? 2 : 3;

            if (Main.rand.NextBool(dustChance))
            {
                Vector2 dustOffset = Main.rand.NextVector2Circular(Projectile.width * 0.4f, Projectile.height * 0.4f);
                Dust dust = Dust.NewDustDirect(
                    Projectile.Center - new Vector2(5) + dustOffset,
                    10, 10,
                    DustID.Blood,
                    0f, 0f, 100, default, 0.7f + Main.rand.NextFloat() * 0.3f);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * -0.2f + Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust.fadeIn = 0.8f;
            }
        }

        private void FindTarget(Player owner)
        {
            int ownerMinionAttackTarget = owner.MinionAttackTargetNPC;
            if (ownerMinionAttackTarget >= 0 && Main.npc[ownerMinionAttackTarget].CanBeChasedBy(Projectile))
            {
                float distance = Vector2.Distance(Main.npc[ownerMinionAttackTarget].Center, Projectile.Center);
                if (distance < AttackDistance)
                {
                    _targetNPC = ownerMinionAttackTarget;
                    _attacking = true;
                    return;
                }
            }
            if (_targetNPC < 0 || !Main.npc[_targetNPC].active || !Main.npc[_targetNPC].CanBeChasedBy(Projectile) ||
                Vector2.Distance(Main.npc[_targetNPC].Center, owner.Center) > AttackDistance * 1.5f)
            {
                _targetNPC = -1;
                float closestDistance = AttackDistance;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.CanBeChasedBy(Projectile) && !npc.friendly)
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            _targetNPC = i;
                        }
                    }
                }

                _attacking = _targetNPC >= 0;
                if (_attacking)
                    _idleTime = 0;
            }
        }

        private void AttackTarget(Player owner)
        {
            NPC target = Main.npc[_targetNPC];

            if (target.active && !target.friendly)
            {
                Vector2 toTarget = target.Center - Projectile.Center;
                float distance = toTarget.Length();
                if (distance > AttackDistance * 1.5f)
                {
                    _attacking = false;
                    _targetNPC = -1;
                    return;
                }
                float speedMultiplier = Math.Min(1f, distance / 200f);
                float chaseSpeed = MathHelper.Lerp(MinChaseSpeed, MaxChaseSpeed, speedMultiplier);
                Vector2 normalizedDirection = toTarget;
                normalizedDirection.Normalize();
                Vector2 perpendicular = new Vector2(-normalizedDirection.Y, normalizedDirection.X);
                Vector2 targetVelocity = normalizedDirection * chaseSpeed + perpendicular;
                _currentVelocity = Vector2.Lerp(_currentVelocity, targetVelocity, AccelerationRate);
            }
            else
            {
                _attacking = false;
                _targetNPC = -1;
            }
        }

        private void IdleOrbit(Player owner)
        {
            _idleTime++;
            _orbitAngle += OrbitSpeed;
            if (_orbitAngle > MathHelper.TwoPi)
                _orbitAngle -= MathHelper.TwoPi;
            Vector2 orbitOffset = new(
                (float)Math.Cos(_orbitAngle) * OrbitDistance,
                (float)Math.Sin(_orbitAngle) * OrbitDistance
            );
            Vector2 targetPosition = owner.Center + orbitOffset;
            Vector2 toTarget = targetPosition - Projectile.Center;
            float distance = toTarget.Length();
            float speed = Math.Min(IdleSpeed + (distance * 0.05f), 12f);
            Vector2 targetVelocity = toTarget;
            if (distance > 0.1f)
            {
                targetVelocity.Normalize();
                targetVelocity *= speed;
            }
            else
                targetVelocity = Vector2.Zero;
            _currentVelocity = Vector2.Lerp(_currentVelocity, targetVelocity, AccelerationRate);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                Dust dust = Dust.NewDustDirect(
                    Projectile.Center,
                    10, 10,
                    DustID.Blood,
                    velocity.X, velocity.Y, 100, default, 1.2f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / Main.projFrames[Projectile.type] / 2);
            Rectangle frame = new(0, Projectile.frame * texture.Height / Main.projFrames[Projectile.type], texture.Width, texture.Height / Main.projFrames[Projectile.type]);
            float pulse = 0.7f + 0.3f * (float)Math.Sin(Main.GameUpdateCount * 0.1f);
            Color glowColor = new Color(255, 100, 100, 0) * pulse;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, glowColor, Projectile.rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}