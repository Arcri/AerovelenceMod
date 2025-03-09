using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Overworld
{
    public class MeteorCrossbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;

            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 32;
            Item.knockBack = 4f;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<MeteorArrow>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;

            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.UseSound = SoundID.DD2_BallistaTowerShot;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] The Arrow at full velocity Skill Strikes [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MeteoriteBar, 15);
            recipe.AddIngredient(ItemID.Obsidian, 10);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 originalVelocity = velocity.SafeNormalize(Vector2.Zero) * Item.shootSpeed;
            float muzzleOffsetDistance = 40f;
            Vector2 muzzleOffset = velocity.SafeNormalize(Vector2.Zero) * muzzleOffsetDistance;
            Vector2 spawnPosition = position + muzzleOffset;
            float arcDistance = 100f;
            Vector2 arcEndPosition = spawnPosition + velocity.SafeNormalize(Vector2.Zero) * arcDistance;
            Vector2 initialVelocity = velocity.SafeNormalize(Vector2.Zero) * 3f;
            Projectile proj = Projectile.NewProjectileDirect(source, spawnPosition, initialVelocity, ModContent.ProjectileType<MeteorArrow>(), damage, knockback, player.whoAmI);

            if (proj.ModProjectile is MeteorArrow meteorArrow)
            {
                meteorArrow.TargetVelocity = originalVelocity / 2;
                meteorArrow.StartPosition = spawnPosition;
                meteorArrow.ArcEndPosition = arcEndPosition;
                Vector2 midPoint = spawnPosition + (arcEndPosition - spawnPosition) * 0.5f;
                Vector2 direction = (arcEndPosition - spawnPosition).SafeNormalize(Vector2.Zero);
                Vector2 upVector = new(0, -1);
                Vector2 perp1 = new(-direction.Y, direction.X);
                Vector2 perp2 = new(direction.Y, -direction.X);
                Vector2 perpendicular = Vector2.Dot(perp1, upVector) > Vector2.Dot(perp2, upVector) ? perp1 : perp2;
                perpendicular = perpendicular.SafeNormalize(Vector2.Zero) * 30f;
                meteorArrow.ControlPoint = midPoint + perpendicular;
            }
            Projectile.NewProjectile(source, spawnPosition, Vector2.Zero, ModContent.ProjectileType<MeteorCrossbowHeld>(), 0, 0f, player.whoAmI);
            return false;
        }
    }

    public class MeteorCrossbowHeld : ModProjectile
    {
        private float _glowIntensity = 1f;
        private float _offset = 0f;

        public override string Texture => "AerovelenceMod/Content/Items/Weapons/Overworld/MeteorCrossbow";

        private Vector2 CurrentDirection => Projectile.rotation.ToRotationVector2();
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.itemTime <= 1)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Owner.Center;
            if (Projectile.owner == Main.myPlayer)
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
            Owner.ChangeDir(Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1);
            if (Projectile.ai[0] == 0)
            {
                _offset = -15f;
                _glowIntensity = 1f;
            }

            _offset = MathHelper.Lerp(_offset, 0f, 0.2f);
            _glowIntensity = MathHelper.Lerp(_glowIntensity, 0f, 0.1f);

            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Overworld/MeteorCrossbowGlow").Value;
            Vector2 position = (Owner.MountedCenter + (CurrentDirection * _offset)) - Main.screenPosition;
            position.Y += Owner.gfxOffY;
            Vector2 handOffset = new Vector2(20, 0).RotatedBy(Projectile.rotation);
            Vector2 verticalOffset = new Vector2(0, 1 * Owner.direction).RotatedBy(Projectile.rotation);
            position += handOffset + verticalOffset;
            float rotation = CurrentDirection.ToRotation() + (Owner.direction == 1 ? 0 : -MathHelper.Pi);
            SpriteEffects spriteEffects = (Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            Vector2 origin = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, position, null, lightColor, rotation, origin, 1f, spriteEffects, 0f);
            if (_glowIntensity > 0.01f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Color glowColor = new Color(255, 120, 0, 0) * _glowIntensity;

                Main.spriteBatch.Draw(glowTexture, position, null, Color.White * _glowIntensity, rotation, origin, 1f, spriteEffects, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                //hhhh needs this otherwise arm glow
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }
    }

    public class MeteorArrow : ModProjectile
    {
        public Vector2 TargetVelocity;
        private bool _rocketIgnited = false;
        private int _initialArcTime = 40;
        private int _timer = 0;

        public Vector2 StartPosition;
        public Vector2 ControlPoint;
        public Vector2 ArcEndPosition;
        private float _bezierProgress = 0f;

        private readonly int _afterimageLength = 10;
        private Vector2[] _oldPositions;
        private float[] _oldRotations;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;

            _oldPositions = new Vector2[_afterimageLength];
            _oldRotations = new float[_afterimageLength];
        }

        public override void AI()
        {
            _timer++;
            if (!_rocketIgnited)
            {
                if (_timer <= _initialArcTime)
                {
                    _bezierProgress = (float)_timer / _initialArcTime;

                    //go my math
                    float oneMinusT = 1f - _bezierProgress;
                    Vector2 newPosition =
                        oneMinusT * oneMinusT * StartPosition +
                        2f * oneMinusT * _bezierProgress * ControlPoint +
                        _bezierProgress * _bezierProgress * ArcEndPosition;
                    Projectile.Center = newPosition;
                    Vector2 tangent =
                        2f * oneMinusT * (ControlPoint - StartPosition) +
                        2f * _bezierProgress * (ArcEndPosition - ControlPoint);
                    Projectile.rotation = tangent.ToRotation();
                    float slowdownFactor = 1f - (_bezierProgress * 0.8f);
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * (3f * slowdownFactor);
                    if (Main.rand.NextBool(20))
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    if (_timer >= _initialArcTime)
                        IgniteRocket();
                }
            }
            else
            {
                if (Projectile.velocity.Length() > 9f)
                {
                    SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
                }
                Player player = Main.player[Projectile.owner];
                Vector2 mouseWorldPosition = Main.MouseWorld;
                Vector2 directionToMouse = (mouseWorldPosition - Projectile.Center).SafeNormalize(Vector2.Zero);
                float mouseInfluence = 0.015f;
                Vector2 currentDir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float distanceToMouse = Vector2.Distance(mouseWorldPosition, Projectile.Center);
                if (distanceToMouse < 100f)
                    mouseInfluence *= distanceToMouse / 100f;
                Vector2 newDir = Vector2.Lerp(currentDir, directionToMouse, mouseInfluence).SafeNormalize(Vector2.Zero);
                float speed = Projectile.velocity.Length();
                Projectile.velocity = newDir * speed;
                Projectile.velocity *= 1.01f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Main.rand.NextBool(2))
                {
                    Vector2 dustVel = Main.rand.NextVector2Circular(1f, 1.3f);
                    Dust gd = Dust.NewDustPerfect(
                        Projectile.Center,
                        ModContent.DustType<GlowPixelCross>(),
                        dustVel,
                        newColor: Color.Orange,
                        Scale: Main.rand.NextFloat(0.2f, 0.4f)
                    );

                    gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.2f,
                        timeBeforeSlow: 5,
                        preSlowPower: 0.95f,
                        postSlowPower: 0.89f,
                        velToBeginShrink: 1f,
                        fadePower: 0.9f,
                        shouldFadeColor: false
                    );
                }
            }

            for (int i = _afterimageLength - 1; i > 0; i--)
            {
                _oldPositions[i] = _oldPositions[i - 1];
                _oldRotations[i] = _oldRotations[i - 1];
            }
            _oldPositions[0] = Projectile.Center;
            _oldRotations[0] = Projectile.rotation;
        }

        private void IgniteRocket()
        {
            _rocketIgnited = true;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, velocity.X, velocity.Y, 100, Color.Orange, Main.rand.NextFloat(1.0f, 2.0f));

                dust.noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                float angle = i * MathHelper.TwoPi / 15;
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 2f;
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Meteorite, velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(1.0f, 1.5f));

                dust.noGravity = true;
            }

            Vector2 mouseWorldPosition = Main.MouseWorld;
            Vector2 directionToMouse = (mouseWorldPosition - Projectile.Center).SafeNormalize(Vector2.Zero);
            Vector2 targetDirection = TargetVelocity.SafeNormalize(Vector2.Zero);
            Vector2 finalDirection = Vector2.Lerp(targetDirection, directionToMouse, 0.002f).SafeNormalize(Vector2.Zero);
            Projectile.velocity = finalDirection * TargetVelocity.Length();
            Projectile.light = 0.8f;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D OuterGlow = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Overworld/MeteorArrowGlow").Value;
            Vector2 drawOrigin = new(texture.Width * 0.5f, texture.Height * 0.5f);

            if (_rocketIgnited)
            {
                for (int i = 0; i < _afterimageLength; i++)
                {
                    if (_oldPositions[i] == Vector2.Zero)
                        continue;

                    float fade = 1f - (float)i / _afterimageLength;
                    Color afterimageColor = new Color(255, 120, 0) * fade * 0.5f;
                    afterimageColor.A = 0;

                    Vector2 drawPos = _oldPositions[i] - Main.screenPosition;
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                    Main.spriteBatch.Draw(texture, drawPos, null, afterimageColor, _oldRotations[i], drawOrigin, 1f - (float)i / _afterimageLength * 0.2f, SpriteEffects.None, 0f);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                }
                Color glowColor = new Color(255, 100, 0, 255) * 0.6f;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw( texture, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(OuterGlow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 1f;
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Meteorite, velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(1.5f, 2.5f));
                dust.noGravity = true;
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, velocity.X, velocity.Y, 100, Color.Orange, Main.rand.NextFloat(1.2f, 2.2f));
                dust.noGravity = true;
                dust.fadeIn = 1.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}