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
using System;
using Terraria.Audio;
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class GraniteCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Cannon");
            Tooltip.SetDefault("TODO");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shootSpeed = 5f;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<GraniteChunk>(); //Need this for shoot() to activate
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_70") with { Pitch = .48f, };
            SoundEngine.PlaySound(style, player.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_62") with { Pitch = .68f, Volume = 0.5f };
            SoundEngine.PlaySound(style2, player.Center);


            for (int i = 0; i < 16; ++i)
            {
                Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 16)) * new Vector2(1f, 4f);
                spinningpoint5 = spinningpoint5.RotatedBy(velocity.ToRotation());

                Dust dust = Dust.NewDustPerfect(position + spinningpoint5 + velocity * 10, 136, spinningpoint5, 0, Color.Blue, 1.3f);
                dust.noGravity = true;
            }

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Projectile.NewProjectile(null, position, Vector2.Zero, ModContent.ProjectileType<GraniteCannonHeldProj>(), 0, 0, player.whoAmI);
            Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<GraniteChunk>(), damage, knockback, player.whoAmI);



            return false;
        }


        
    }

    public class GraniteCannonHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public float OFFSET = 20; //30

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Cannon");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 30;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer && timer == 0)
            {
                Angle = (Main.MouseWorld - Player.Center).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);


            if (timer == 2)
            {
                OFFSET = 5f;
            }
            OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 20f, 0.07f), 0, 17);

            direction = Angle.ToRotationVector2();
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;
            Projectile.rotation = direction.ToRotation();

            
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteCannon");
            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(Weapon, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);

            return false;
        }
    }

    public class GraniteChunk : TrailProjBase
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            DisplayName.SetDefault("Granite Chunk");
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();
            Texture2D Chunk = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Ranged/GraniteChunk");

            for (int j = 0; j < 4; j++)
            {
                //Main.spriteBatch.Draw(Chunk, Projectile.oldPos[j] - Main.screenPosition, null, Color.Black * (1 - j * 0.15f), Projectile.oldRot[j], Chunk.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            /*
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }*/

            Main.spriteBatch.Draw(Chunk, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Chunk.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        int timer = 0;
        public override void AI()
        {
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value;
            trailColor = new Color(25,25,255);
            trailTime = timer * 0.02f;
            timesToDraw = 4;

            trailPointLimit = 120;
            trailWidth = 10;
            trailMaxLength = 100;

            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center + Projectile.velocity;

            TrailLogic();
            timer++;

        }
        public override void Kill(int timeLeft)
        {

            for (double i = 0; i < 6.28; i += 0.3)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2((float)Math.Sin(i) * 1.3f, (float)Math.Cos(i)) * 2.4f);
                dust.noGravity = true;
            }

            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            //SoundEngine.PlaySound(SoundID.Item93 with { Pitch = -0.25f, Volume = 0.4f}, Projectile.position);
            SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { PitchVariance = 0.3f, Volume = 0.3f, Pitch = 0.2f }, Projectile.Center);

        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * 0.25f; //* 0.5f

        }
    }
}