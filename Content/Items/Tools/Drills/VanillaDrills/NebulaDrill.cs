using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Tools.Drills
{
    public class NebulaDrill : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.NebulaDrill)
            {
                item.damage = 50;
                item.knockBack = 0.5f;
                item.width = item.height = 26;
                item.pick = 225;
                item.useAnimation = 15;
                item.useTime = 2;
                item.shootSpeed = 32f;

                item.rare = ItemRarities.PillarsAndML;
                item.value = Item.sellPrice(0, 7, 0, 0);
                item.useStyle = ItemUseStyleID.Shoot;
                item.DamageType = DamageClass.Melee;
                item.shoot = ModContent.ProjectileType<NebulaDrillProj>();

                item.channel = true;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.autoReuse = true;
            }
        }


        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.NebulaDrill)
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<NebulaDrillProj>()] == 0;
            }
            return base.CanUseItem(item, player);
        }


        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

    }

    public class NebulaDrillProj : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Tools/Drills/VanillaDrills/NebulaDrillProj";
        private Texture2D _colorGlowTexture;
        private Texture2D _pulseGlowTexture;
        private Texture2D _drillTexture;

        private float _colorCycleTimer;
        private int _pulseTimer;

        private bool _playedStartSound = false;
        private ActiveSound _loopSoundInstance;

        private bool _loopStarted;
        private SlotId _startSlot;
        private ActiveSound _startSoundInstance;
        private SlotId _loopSlot;

        private float _orangeIntensity = 0f;

        private Vector2 _drillJostleOffset;
        private int _jostleTimer;


        public int OFFSET = 15;
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpVal = 0;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        public override void SetDefaults()
        {
            _colorGlowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Tools/Drills/VanillaDrills/NebulaDrillProjGlow").Value;
            _pulseGlowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Tools/Drills/VanillaDrills/NebulaDrillDrillOrange").Value;
            _drillTexture = Mod.Assets.Request<Texture2D>("Content/Items/Tools/Drills/VanillaDrills/NebulaDrillDrill").Value;

            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region heldProjStuff
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            player.itemTime = 2;
            player.itemAnimation = 2;


            if (player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                    Angle = (Main.MouseWorld - (player.MountedCenter)).ToRotation();
                direction = Angle.ToRotationVector2();
                player.ChangeDir(direction.X > 0 ? 1 : -1);

            }
            else
            {
                _orangeIntensity *= 0.98f;
                Projectile.active = false;
                if (_loopSoundInstance != null)
                {
                    _loopSoundInstance.Stop();
                    _loopSoundInstance = null;
                }

                if (_startSoundInstance != null)
                {
                    _startSoundInstance.Stop();
                    _startSoundInstance = null;
                }

                SoundStyle endSound = new("AerovelenceMod/Sounds/Effects/DrillEnd") { Volume = 3f, Pitch = 0.0f, };
                SoundEngine.PlaySound(endSound, Projectile.Center);
            }
            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);
            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * player.direction * -1f);

            _jostleTimer++;
            if (_jostleTimer % 2 == 0)
                _drillJostleOffset = Main.rand.NextVector2Circular(0.5f, 0.5f);

            Projectile.Center = player.MountedCenter + direction * OFFSET + new Vector2(0f, player.gfxOffY) + _drillJostleOffset;
            Lighting.AddLight(Projectile.Center, 0, 0.7f * _orangeIntensity, 1.1f * _orangeIntensity);

            Projectile.velocity = Vector2.Zero;
            player.itemRotation = direction.ToRotation();
            if (player.direction != 1)
                player.itemRotation -= 3.14f;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
            #endregion

            #region Sound stuff
            if (!_playedStartSound)
            {
                _playedStartSound = true;
                SoundStyle startSound = new("AerovelenceMod/Sounds/Effects/DrillStart") { Volume = 3f, Pitch = 0.0f, };
                _startSlot = SoundEngine.PlaySound(startSound, Projectile.Center);
                if (_startSlot.IsValid && SoundEngine.TryGetActiveSound(_startSlot, out var startFoundSound))
                    _startSoundInstance = startFoundSound;
            }

            if (!_loopStarted && _startSoundInstance != null)
            {
                if (_startSoundInstance.Sound == null || _startSoundInstance.Sound.State != Microsoft.Xna.Framework.Audio.SoundState.Playing)
                {
                    SoundStyle loopSound = new("AerovelenceMod/Sounds/Effects/DrillLoop1") { Volume = 3f, Pitch = 0.0f, IsLooped = true };
                    _loopSlot = SoundEngine.PlaySound(loopSound, Projectile.Center);
                    if (_loopSlot.IsValid && SoundEngine.TryGetActiveSound(_loopSlot, out var loopFoundSound))
                        _loopSoundInstance = loopFoundSound;
                    _loopStarted = true;
                }
            }

            if (_loopSoundInstance != null)
                _loopSoundInstance.Position = Projectile.Center;
            #endregion

            _orangeIntensity = Math.Min(_orangeIntensity + 0.01f, 1f);

            _colorCycleTimer += 4.05f;
            if (_pulseTimer > 0)
                _pulseTimer--;

            if (Main.GameUpdateCount % 3 == 0)
            {
                Vector2 tipPosition = Projectile.Center + direction * 18f;

                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.2f, 1.2f);

                Dust gd = Dust.NewDustPerfect(tipPosition, ModContent.DustType<GlowPixelCross>(), dustVel, 0, Color.Pink, Main.rand.NextFloat(0.2f, 0.4f));
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

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects spriteEffects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height1 = texture.Height;
            Vector2 origin = new(texture.Width / 2f, height1 / 2f);
            Vector2 actualPos = Projectile.Center - Main.screenPosition;
            Color cycleColor = GetCycleColor(_colorCycleTimer);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, actualPos, null, lightColor, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(_drillTexture, actualPos, null, cycleColor * 0.75f, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.Draw(_colorGlowTexture, actualPos, null, cycleColor * 0.75f, direction.ToRotation() + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0);

            Color orangeColor = Color.Orange * _orangeIntensity;
            Main.spriteBatch.Draw(_pulseGlowTexture, actualPos, null, orangeColor, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //for some reason we have to duplicate this twice otherwise the player's arm glows?
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        private static Color GetCycleColor(float timer)
        {
            const float segment = 30f;
            int index = (int)(timer / segment) % 3;
            int nextIndex = (index + 1) % 3;
            float localT = (timer % segment) / segment;
            Color[] cycle = [Color.Orange, Color.White, Color.Blue];
            return Color.Lerp(cycle[index], cycle[nextIndex], localT);
        }
    }
}