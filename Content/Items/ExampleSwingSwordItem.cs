using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;

namespace AerovelenceMod.Content.Items
{
    //MOSTLY DONE
    public class ExampleSwingSwordItem : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Example Swing Sword");
            // Tooltip.SetDefault("Debug/Example Item");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Master;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;

            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<ExampleSwingSwordProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }

    public class ExampleSwingSwordProj : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 7;
        }

        bool playedSound = false;
        BaseTrailInfo relativeTrail = new BaseTrailInfo();

        public override void AI()
        {

            if (timer == 0)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
            }

            /* funny spin :)
            SwingHalfAngle = 400;
            easingAdditionAmount = 0.01f;
            offset = 60;
            frameToStartSwing = 2;
            timeAfterEnd = 5;
            startingProgress = 0.1f;
            */
            
            SwingHalfAngle = 190;
            easingAdditionAmount = 0.025f / Projectile.extraUpdates; //0.02f
            offset = 50;
            frameToStartSwing = 3;
            timeAfterEnd = 6;
            startingProgress = 0.02f;
            
            StandardSwingUpdate();
            StandardHeldProjCode();

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.62f, PitchVariance = .3f, Volume = 0.20f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);


            //Trail
            relativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/FireEdge").Value;
            relativeTrail.trailColor = Color.DodgerBlue;
            relativeTrail.trailPointLimit = 800;
            relativeTrail.trailWidth = 20;
            relativeTrail.trailMaxLength = 300;
            relativeTrail.timesToDraw = 3;
            relativeTrail.relativeToPlayer = true;
            relativeTrail.myPlayer = Main.player[Projectile.owner];
            //relativeTrail.trailTime = timer * 0.0035f;
            relativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            relativeTrail.trailPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-1f) * (30 + (intensity * 15)) - Main.player[Projectile.owner].Center;
            
            if (getProgress(easingProgress) >= 0.03f)
            relativeTrail.TrailLogic();

            /*
            if (getProgress(easingProgress) >= 0.98)
            {
                Main.player[Projectile.owner].itemTime = 0;
                Main.player[Projectile.owner].itemAnimation = 0;
                Projectile.active = false;
            }
            */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            relativeTrail.TrailDrawing(Main.spriteBatch);

            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/ExampleSwingSwordItem");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(0, Blade.Height);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Blade.Width, Blade.Height);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);

            //Sprite is 64x64 so -0 to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + (intensity * 0.5f), effects, 0f);

            /*
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            //Shit does not work for non-mirrored swords or at least ones that aren't close (design not dimensions)
            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = (Projectile.ai[0] != 1 ? 0 : MathHelper.ToRadians(45f));
                effects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }
            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2); // get position of hand
            
            if (Projectile.spriteDirection > 0) armPosition.Y += Main.player[Projectile.owner].gfxOffY;
            Main.spriteBatch.Draw(Blade, armPosition - Main.screenPosition + new Vector2(-8,0).RotatedBy(currentAngle), default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            return false;


            Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2, Projectile.scale /*+ ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f)*/ //, Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            //Vector2 frontHandPos = Main.player[Projectile.owner].Center;
            //float angle = (Projectile.Center - frontHandPos).ToRotation();
            //Utils.DrawLine(Main.spriteBatch, frontHandPos, frontHandPos + (angle.ToRotationVector2() * 200), Color.Red);

            return false;
        }
    }

}