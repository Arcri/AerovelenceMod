using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Weapons.Underworld;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using System.Collections.Generic;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.UI.Elements;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns
{
    public class RedundantFibber : TranslatableModItem
    {
        public static int CurrentColorIndex = 0;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("RedundantFibber", "+50% love\n'Would I lie to you?'\nBullets shot will ricochet")
            .AddName(Language.Default, "Redundant Fibber").AddTooltip(Language.Default, "+50% love\n'Would I lie to you?'\nBullets shot will ricochet")
            .AddSkillStrike(Language.Default, "Ricochet bullets may or may not Skill Strike")

            .AddName(Language.Spanish, "Redundant Fibber").AddTooltip(Language.Spanish, "+50% amor\n'¿Te mentiría yo?'\nLas balas disparadas rebotarán").AddSkillStrike(Language.Spanish, "Las balas rebotadas pueden o no realizar Golpes de Habilidad")
            .AddName(Language.French, "Redundant Fibber").AddTooltip(Language.French, "50% d'amour\n'Te mentirais-je?'\nLes balles tirées ricocheront").AddSkillStrike(Language.French, "Les balles ricochées peuvent ou non déclencher des Coups de Compétence")
            .AddName(Language.German, "Redundant Fibber").AddTooltip(Language.German, "50% Liebe\n'Würde ich dich anlügen?'\nAbgeschossene Kugeln prallen ab").AddSkillStrike(Language.German, "Abprallende Kugeln können Fähigkeitsschläge auslösen… oder auch nicht")
            .AddName(Language.Italian, "Redundant Fibber").AddTooltip(Language.Italian, "50% amore\n'Ti mentirei mai?'\nI proiettili sparati rimbalzeranno").AddSkillStrike(Language.Italian, "I proiettili rimbalzati possono o meno eseguire Colpi dell'Abilità")
            //.AddName(Language.Polish, "Zbędny Kłamca").AddTooltip(Language.Polish, "50% miłości\n'Czy bym cię okłamał?'\nStrzelone pociski będą rykoszetować").AddSkillStrike(Language.Polish, "Odbite pociski mogą, ale nie muszą, wykonać Ciosy Umiejętności")
            //.AddName(Language.PortugueseBrazil, "Mentiroso Redundante").AddTooltip(Language.PortugueseBrazil, "50% amor\n'Eu mentiria para você?'\nAs balas disparadas ricochetearão").AddSkillStrike(Language.PortugueseBrazil, "As balas ricocheteadas podem ou não realizar Golpes de Habilidade")
            .AddName(Language.Russian, "Редундант Фибер").AddTooltip(Language.Russian, "50% любви\n'Разве я бы тебе солгал?'\nПули будут рикошетить").AddSkillStrike(Language.Russian, "Рикошетирующие пули могут или не могут активировать Навык Удара");
            //.AddName(Language.ChineseTraditional, "多餘的騙子").AddTooltip(Language.ChineseTraditional, "50%的愛\n'我會對你撒謊嗎？'\n子彈會反彈").AddSkillStrike(Language.ChineseTraditional, "反彈的子彈可能會，也可能不會觸發技能打擊")
            //.AddName(Language.ChineseSimplified, "多余的骗子").AddTooltip(Language.ChineseSimplified, "50% 的爱\n'我会对你撒谎吗？'\n子弹会反弹").AddSkillStrike(Language.ChineseSimplified, "反弹的子弹可能会，也可能不会触发技能打击");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 55;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarities.RarePrePlant;
            Item.shoot = ModContent.ProjectileType<RedundantFibberHeldProj>();
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipHelper.ApplyTranslations(this, tooltips);
            foreach (TooltipLine line in tooltips)
            {
                if (line.Name == "Damage")
                {
                    line.Text = line.Text.Replace(Item.damage.ToString(), "523,031");
                }
            }
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool foundExistingProj = false;
            int existingProjIndex = -1;

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_0") with { Pitch = 1f, MaxInstances = 4, PitchVariance = .1f, };
            SoundEngine.PlaySound(style2, position);
            int ammoType = ItemID.SilverBullet;
            for (int i = 54; i < 58; i++)
            {
                Item item = player.inventory[i];
                if (item.ammo == AmmoID.Bullet && item.stack > 0)
                {
                    ammoType = item.type;
                    break;
                }
            }
            FibberBullet.CurrentAmmoType = ammoType;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<RedundantFibberHeldProj>())
                {
                    foundExistingProj = true;
                    existingProjIndex = i;
                    break;
                }
            }
            if (foundExistingProj && existingProjIndex != -1)
            {
                if (Main.projectile[existingProjIndex].ModProjectile is RedundantFibberHeldProj gunProj)
                    gunProj.TriggerShoot();
            }
            else
            {
                int heldProj = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<RedundantFibberHeldProj>(), 0, 0, player.whoAmI);

                if (Main.projectile[heldProj].ModProjectile is RedundantFibberHeldProj gunProj)
                    gunProj.TriggerShoot();
            }
            return false;
        }
    }

    public class RedundantFibberHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private bool needToShoot = false;

        private Player Owner => Main.player[Projectile.owner];
        private float _offset = 0f;
        private Vector2 CurrentDirection => Projectile.rotation.ToRotationVector2();

        private int inactiveCounter = 0;
        private int MAX_INACTIVE_TIME = 60;

        private bool canSkillStrike = false;

        private int totalFakeDamageDealt = 0;
        private int fakeDpsUpdateTimer = 0;
        private int fakeDpsInterval = 2;

        private int timeSinceLastShot = 0;
        private int inactiveTimeout = 9;

        public void TriggerShoot()
        {
            needToShoot = true;
            timeSinceLastShot = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
        }

        public int OFFSET = 22;
        public int VERTICAL_OFFSET = +4;
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpVal = 0;

        public override bool? CanDamage() => false;

        private Vector2 recoilOffset = Vector2.Zero;
        private float recoilStrength = 8f;
        private float recoilRecoverySpeed = 0.5f;
        private bool hasRecoil = false;
        private float recoilRotation = 0f;
        private float maxRecoilRotation = -0.25f;
        private float rotationalRecoilRecoverySpeed = 0.5f;

        private void ApplyRecoil()
        {
            float backwardRecoil = -recoilStrength;
            float upwardRecoil = recoilStrength * 0.8f;
            float heatMultiplier = 1f;
            if (Owner.direction == -1)
                recoilOffset = new Vector2(backwardRecoil, upwardRecoil) * heatMultiplier;
            else
                recoilOffset = new Vector2(backwardRecoil, -upwardRecoil) * heatMultiplier;
            recoilRotation = maxRecoilRotation * heatMultiplier;
            hasRecoil = true;
        }

        private int lastSelectedItem = -1;

        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
                Angle = (Main.MouseWorld - (Owner.MountedCenter)).ToRotation();
            direction = Angle.ToRotationVector2();
            Owner.ChangeDir(direction.X > 0 ? 1 : -1);
            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);
            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * Owner.direction * -1f);
            float armRotation = Projectile.rotation - MathHelper.PiOver2;
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float offsetForward = 10f;
            float offsetSide = -2f;
            if (Owner.direction < 0)
                offsetSide *= -1;
            Vector2 offsetVector = new(
                offsetForward * (float)Math.Cos(direction.ToRotation()) - offsetSide * (float)Math.Sin(direction.ToRotation()),
                offsetForward * (float)Math.Sin(direction.ToRotation()) + offsetSide * (float)Math.Cos(direction.ToRotation())
            );
            if (hasRecoil)
            {
                recoilOffset = Vector2.Lerp(recoilOffset, Vector2.Zero, recoilRecoverySpeed);
                recoilRotation = MathHelper.Lerp(recoilRotation, 0f, rotationalRecoilRecoverySpeed);
                if (recoilOffset.Length() < 0.1f && Math.Abs(recoilRotation) < 0.01f)
                {
                    hasRecoil = false;
                    recoilOffset = Vector2.Zero;
                    recoilRotation = 0f;
                }
            }
            Vector2 recoilVector = Vector2.Zero;
            if (hasRecoil)
            {
                recoilVector = new Vector2(
                    recoilOffset.X * (float)Math.Cos(direction.ToRotation()) - recoilOffset.Y * (float)Math.Sin(direction.ToRotation()),
                    recoilOffset.X * (float)Math.Sin(direction.ToRotation()) + recoilOffset.Y * (float)Math.Cos(direction.ToRotation())
                );
            }
            Projectile.Center = armPosition + offsetVector + recoilVector;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float finalRotation = direction.ToRotation();
            if (hasRecoil)
                finalRotation += recoilRotation * Owner.direction;
            Projectile.rotation = finalRotation;
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead)
                player.heldProj = Projectile.whoAmI;
            if (!player.channel)
            {
                inactiveCounter++;
                if (inactiveCounter >= MAX_INACTIVE_TIME)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                inactiveCounter = 0;
            }
            bool holdingCorrectItem = player.HeldItem.type == ModContent.ItemType<RedundantFibber>();
            bool selectedItemChanged = lastSelectedItem != player.selectedItem;
            if (selectedItemChanged)
                lastSelectedItem = player.selectedItem;

            if (!holdingCorrectItem || player.dead || !player.active)
            {
                Projectile.Kill();
                return;
            }
            if (player.selectedItem != lastSelectedItem)
            {
                Projectile.Kill();
                return;
            }
            timeSinceLastShot++;

            if (timeSinceLastShot >= inactiveTimeout)
            {
                Projectile.Kill();
                return;
            }
            fakeDpsUpdateTimer++;
            if (fakeDpsUpdateTimer >= fakeDpsInterval)
            {
                fakeDpsUpdateTimer = 0;
                if (Main.LocalPlayer.whoAmI == Projectile.owner)
                {
                    int fakeDamage = Main.rand.Next(500000, 1000001);
                    totalFakeDamageDealt += fakeDamage;
                    Main.LocalPlayer.dpsDamage += fakeDamage;
                }
            }

            player.heldProj = Projectile.whoAmI;
            Vector2 aimDirection = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
            bool playerWantsToShoot = needToShoot;

            if (playerWantsToShoot)
            {
                needToShoot = false;
                int bulletDamage = (int)(player.HeldItem.damage * player.GetDamage(DamageClass.Ranged).Multiplicative);
                Vector2 velocity = aimDirection * player.HeldItem.shootSpeed;
                int bulletType = ModContent.ProjectileType<FibberBullet>();
                bool skillStrikeShot = canSkillStrike;
                int bulletProj = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + aimDirection * 36f, velocity, bulletType, bulletDamage, player.HeldItem.knockBack, player.whoAmI);
                if (Main.projectile[bulletProj].ModProjectile is FibberBullet fibberBullet)
                {
                    fibberBullet.fakeDisplayDamage = Main.rand.Next(500000, 1000001);
                    fibberBullet.colorIndex = RedundantFibber.CurrentColorIndex;
                    RedundantFibber.CurrentColorIndex = (RedundantFibber.CurrentColorIndex + 1) % FibberBullet.colorOptions.Length;
                }

                ApplyRecoil();

                if (skillStrikeShot)
                {
                    SkillStrikeUtil.setSkillStrikeWithImpactType(Main.projectile[bulletProj], 1.5f, 1, SkillStrikeImpactType.Basic, 0.6f, 1.2f);
                    for (int i = 0; i < 12; i++)
                    {
                        Dust d = Dust.NewDustDirect(
                            Projectile.Center,
                            10, 10,
                            DustID.GoldFlame,
                            aimDirection.X * 2f, aimDirection.Y * 2f,
                            0, Color.Orange, 1.2f);
                        d.noGravity = true;
                    }
                    SoundStyle skillStrikeSound = new SoundStyle("Terraria/Sounds/Item_14") with { Pitch = 0.15f, Volume = 0.7f };
                    SoundEngine.PlaySound(skillStrikeSound, Projectile.position);
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/Guns/RedundantFibber").Value;
            float rotation = Projectile.rotation;
            if (Owner.direction == -1)
                rotation += MathHelper.Pi;
            SpriteEffects spriteEffects = (Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            float scale = 1f;
            Vector2 actualPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, actualPos, null, lightColor, rotation, origin, scale, spriteEffects, 0f);
        }
    }

    public class FibberBullet : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public static int CurrentAmmoType = AmmoID.Bullet;
        public int fakeDisplayDamage = 0;
        public int colorIndex = 0;
        public static readonly Color[] colorOptions =
        [
        new Color(204, 168, 0),   //fibber gold
        new Color(033, 234, 241), //fibber blue
        new Color(192, 040, 039)  //fibber red
        ];
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        int timer = 0;
        float alpha = 1;
        bool justHit = false;
        float justHitTimer = 4;

        public override bool? CanDamage()
        {
            return timer < 50 && !justHit;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            originalDamage = (int)modifiers.FinalDamage.Base;
            modifiers.HideCombatText();
        }

        private int originalDamage = 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            justHit = true;
            hit.HideCombatText = true;

            if (Main.myPlayer == Projectile.owner)
            {
                int displayDamage = fakeDisplayDamage > 0 ? fakeDisplayDamage : Main.rand.Next(500000, 1000001);
                CombatText.NewText(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height), new Color(201, 125, 062), displayDamage, dramatic: true, dot: false);
            }

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    colorOptions[colorIndex], Main.rand.NextFloat(0.35f, 0.55f), 0.4f, 0f, dustShader);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            CreateRicochetBullets(oldVelocity);
            return true;
        }

        private void CreateRicochetBullets(Vector2 oldVelocity)
        {
            if (Main.myPlayer != Projectile.owner) return;
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 10, 10, DustID.Smoke, 0, 0, 100, colorOptions[colorIndex], 1f);
                d.noGravity = true;
                d.velocity *= 2f;
            }
            SoundStyle ricochetSound = new SoundStyle("Terraria/Sounds/Item_28") with
            {
                Pitch = 0.2f,
                PitchVariance = 0.3f,
                Volume = 0.2f
            };
            SoundEngine.PlaySound(ricochetSound, Projectile.Center);
            int ricochetController = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FibberRicochetController>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, oldVelocity.X, oldVelocity.Y);
            if (ricochetController >= 0 && Main.projectile[ricochetController].ModProjectile is FibberRicochetController controller)
            {
                controller.colorIndex = colorIndex;
                controller.ammoType = CurrentAmmoType;
            }
        }

        public override void AI()
        {
            Projectile.rotation += 5;
            if (!justHit)
            {
                trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/ThinGlowLine").Value;
                trailColor = colorOptions[colorIndex];
                trailTime = timer * 0.02f;
                trailPointLimit = 22;
                trailWidth = 2;
                trailMaxLength = 200;
                trailRot = Projectile.velocity.ToRotation();
                trailPos = Projectile.Center + Projectile.velocity;
                TrailLogic();
                Vector3 lightColor = colorOptions[colorIndex].ToVector3();
                Lighting.AddLight(Projectile.position, lightColor * 0.45f);

                if (timer > 10)
                {
                    if (timer > 40)
                        alpha = MathHelper.Lerp(alpha, 0, 0.08f);
                }
                if (alpha < 0.05f)
                    Projectile.active = false;

                timer++;
            }
            else
            {
                justHitTimer--;
                trailColor = Color.Lerp(Color.White, colorOptions[colorIndex], 0.8f);

                if (justHitTimer <= 0)
                {
                    Projectile.Kill();
                    Projectile.active = false;
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Color trailColorWithAlpha = trailColor * alpha;
            trailColor = trailColorWithAlpha;
            TrailDrawing();
            Texture2D tex = Mod.Assets.Request<Texture2D>("Assets/Pixel/VanillaStarBlackBG").Value;
            Color col = colorOptions[colorIndex];

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 1, 0, 0), col * 2f * alpha, Projectile.rotation, tex.Size() / 2, 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 1, 0, 0), col * alpha, Projectile.rotation, tex.Size() / 2, 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 26f, num) * 0.5f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = -0.4f, PitchVariance = .28f, MaxInstances = 4, Volume = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            Collision.HitTiles(Projectile.position + (Projectile.velocity * 0.5f), Projectile.velocity * 0.5f, Projectile.width, Projectile.height);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    colorOptions[colorIndex], Main.rand.NextFloat(0.35f, 0.55f), 0.4f, 0f, dustShader);
            }
        }
    }

    public class FibberRicochetController : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int colorIndex = 0;
        public int ammoType = AmmoID.Bullet;
        private bool hasSpawnedBullets = false;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.alpha += 1;
            if (!hasSpawnedBullets && Projectile.owner == Main.myPlayer)
            {
                hasSpawnedBullets = true;
                SpawnRicochetBullets();
            }
        }

        private void SpawnRicochetBullets()
        {
            Vector2 oldVelocity = new(Projectile.ai[0], Projectile.ai[1]);
            int projType;
            if (ammoType > ItemID.None)
                projType = ContentSamples.ItemsByType[ammoType].shoot;
            else
                projType = ProjectileID.Bullet;
            Vector2 reflectVelocity = oldVelocity * -0.8f;
            List<int> ricochetBullets = [];

            for (int i = 0; i < 5; i++)
            {
                float spreadAngle = MathHelper.ToRadians(Main.rand.Next(-5, 5));
                Vector2 spreadVelocity = reflectVelocity.RotatedBy(spreadAngle);
                float speed = oldVelocity.Length() * Main.rand.NextFloat(0.8f, 1.2f);
                spreadVelocity = Vector2.Normalize(spreadVelocity) * speed;
                Vector2 offsetPosition = Projectile.Center + Vector2.Normalize(spreadVelocity) * 8f;

                int bullet = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    offsetPosition,
                    spreadVelocity,
                    projType,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner);

                if (bullet >= 0)
                {
                    ricochetBullets.Add(bullet);
                    Main.projectile[bullet].timeLeft = Math.Min(Main.projectile[bullet].timeLeft, 60);
                    Main.projectile[bullet].GetGlobalProjectile<FibberLieDamageGlobal>().isFibberRicochet = true;
                    Main.projectile[bullet].GetGlobalProjectile<FibberLieDamageGlobal>().fakeDamage = Main.rand.Next(100000, 900001);
                    Color dustColor = FibberBullet.colorOptions[colorIndex];
                    for (int d = 0; d < 3; d++)
                    {
                        Dust.NewDustDirect(offsetPosition, 4, 4, DustID.GoldFlame, spreadVelocity.X * 0.1f, spreadVelocity.Y * 0.1f, 0, dustColor, 1f).noGravity = true;
                    }
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class FibberLieDamageGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool isFibberRicochet = false;
        public int fakeDamage = 0;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (isFibberRicochet && Main.myPlayer == projectile.owner)
            {
                hit.HideCombatText = true;
                CombatText.NewText(
                    new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height), new Color(201, 125, 62), fakeDamage, dramatic: true, dot: false);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 velocity = new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f));
                    Dust.NewDust(target.position, target.width, target.height, DustID.GoldCoin, velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(1f, 1.5f));
                }
            }
        }
    }
}