using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles;
using ReLogic.Content;
using Terraria.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;
using System.Linq;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class FlailTest : ModItem
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flail Test");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 1;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<FlailTestProj>();
            Item.shootSpeed = 0f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.channel = true;

            Item.value = Item.sellPrice(silver: 45);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<FlailTestProj>());
        }

    }
    public class FlailTestProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flail");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
        }

        public float offset = 0;
        float rotationStrength = 0f;
        int timer = 0;
        int storedDirection = 1;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (timer == 0)
                storedDirection = owner.direction;
            owner.direction = storedDirection;

            if (!owner.channel)
                Projectile.active = false;

            Projectile.scale += 0.01f;
            Projectile.scale = MathHelper.Min(Projectile.scale, 1);
            Projectile.timeLeft = 2;

            rotationStrength = MathHelper.Lerp(rotationStrength, 0.05f, 0.007f); //0.05
            Projectile.rotation += rotationStrength * storedDirection; //0.05
            owner.itemAnimation = owner.itemTime = 2;
            Projectile.velocity = Vector2.Zero;

            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.Center = Projectile.rotation.ToRotationVector2() * offset + owner.Center;

            offset = MathHelper.SmoothStep(offset, 175f, 0.1f);

            if (timer % 120 == 0) //120
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Flail2") with { PitchVariance = .16f, MaxInstances = 1, Pitch = -0.2f, Volume = 0.3f }; 
                SoundEngine.PlaySound(style, Projectile.Center);
                //Dust.NewDustPerfect(Projectile.rotation.ToRotationVector2() * (offset - 10) + owner.Center, ModContent.DustType<DashTrailDust>(), Velocity: (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 2);
            }
                
            timer++;

            //ThinGlowLine
            //2 draws | timer * 0.005 | White | 120 30 800 |

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value;
            trailColor = Color.White;
            trailTime = timer * 0.005f;
            timesToDraw = 2;

            pixelate = true;
            pixelationAmount = 1f;

            trailPointLimit = 120;
            trailWidth = 30;
            trailMaxLength = 800;

            trailRot = Projectile.rotation + MathHelper.PiOver2;
            trailPos = Projectile.Center + Projectile.velocity;

            TrailLogic();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();


            Texture2D Ball = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/FlailHead");
            Texture2D Chain = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/FlailChain");

            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (Chain.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : Chain.Height) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values


                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(Chain, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Ball.Size() / 2, 1f, SpriteEffects.None, 0f);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, trailWidth, num) * 0.5f; //* 0.5f

        }
    }
}
