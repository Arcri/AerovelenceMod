using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Content.Items.Weapons.Misc.Melee;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.WandOfExploding
{
    public class WandOfExploding : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = KnockbackTiers.Average;
            Item.mana = 14;
            Item.shootSpeed = 17f;

            Item.width = 38;
            Item.height = 38;
            Item.useAnimation = 40;
            Item.useTime = 40;

            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<WandOfExplodingHeldProj>();
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.sellPrice(0, 0, 75, 0);
 
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes under 50% mana [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AerovelenceMod:EvilBars", 10).
                AddIngredient(ItemID.Sapphire, 5).
                AddIngredient(ItemID.ManaCrystal, 3).
                 AddTile(TileID.Anvils).
                Register();
        }

    }

    public class WandOfExplodingHeldProj : ModProjectile
    {
        int timer = 0;
        public float OFFSET = 0; //20
        public float alphaPercent = 0;
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        float lerpToStuff = 0;
        bool fade = false;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Projectile.velocity = Vector2.Zero;

            if (Player.channel && !fade)
            {
                Projectile.timeLeft++;
                Player.itemTime = 2;
                Player.itemAnimation = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
                }
                direction = Angle.ToRotationVector2();

            }
            else
                fade = true;

            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            if (!fade)
            {
                OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 16, 0.1f), 0, 15);
                alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, 1.2f, 0.08f), 0, 1);
            } 
            else
            {
                OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, -2f, 0.1f), 0, 15);
                alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, -0.2f, 0.08f), 0, 1);
            }

            lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);

            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            if (timer % 40 == 0 && timer != 0 && !fade)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .86f, PitchVariance = 0.15f, Volume = 0.5f }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_book_staff_cast_0") with { Pitch = 0f, Volume = 0.3f }; 
                SoundEngine.PlaySound(style2, Projectile.Center);

                Vector2 vel = new Vector2(17, 0).RotatedBy(direction.ToRotation());
                int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel.SafeNormalize(Vector2.UnitX) * OFFSET, vel, ModContent.ProjectileType<WandOfExplodingProj>(),
                    Projectile.damage, 0, Main.myPlayer);

                for (int fg = 0; fg < 5 + Main.rand.Next(2); fg++)
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);

                    Vector2 dir = vel.SafeNormalize(Vector2.UnitX).RotatedByRandom(1f) * Main.rand.NextFloat(0.3f, 1.35f) * 2.5f;

                    Dust gd = Dust.NewDustPerfect(Projectile.Center + vel.SafeNormalize(Vector2.UnitX) * OFFSET, ModContent.DustType<GlowPixelAlts>(), dir, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.6f) * 0.4f);
                    gd.velocity += vel * 0.1f;
                }

                for (int i = 0; i < 3 + Main.rand.Next(2); i++)
                {
                    Vector2 v = Main.rand.NextVector2Unit();
                    Dust sa = Dust.NewDustPerfect(Projectile.Center + vel.SafeNormalize(Vector2.UnitX) * OFFSET, DustID.PortalBoltTrail, vel.SafeNormalize(Vector2.UnitX).RotatedByRandom(1.2) * Main.rand.NextFloat(2f, 4f), 0,
                        Color.LightSkyBlue, 1.2f);
                    sa.noGravity = true;
                }

                if (!Player.CheckMana(Player.inventory[Player.selectedItem], pay: true))
                    Projectile.active = false;

                OFFSET -= 20;
            }
            if (fade)
                fadeVal += 0.15f;

            Vector2 lightPos = Projectile.Center + (direction.SafeNormalize(Vector2.UnitX) * OFFSET);

            Lighting.AddLight(lightPos, Color.SkyBlue.ToVector3() * alphaPercent * 0.4f);

            timer++;
        }

        float fadeVal = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            if (fade)
                lightColor = Color.Lerp(lightColor, Color.Blue * 0.3f, fadeVal);

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/WandOfExploding").Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/WandOfExplodingGlowmask").Value;

            Vector2 origin = texture.Size() / 2f;
            Vector2 position = Projectile.Center - (0.5f * (direction * -17)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition;

            Vector2 newOffset = new Vector2(0, 2 * Player.direction).RotatedBy(Angle);

            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float bonusRot = Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver4 * -1;

            Main.spriteBatch.Draw(texture, position + newOffset, null, lightColor * alphaPercent, direction.ToRotation() + bonusRot, origin, Projectile.scale, myEffect, 0.0f);
            Main.spriteBatch.Draw(glowMask, position + newOffset, null, Color.White * alphaPercent * 0.3f, direction.ToRotation() + bonusRot, origin, Projectile.scale, myEffect, 0.0f);

            return false;
        }

    }

    public class WandOfExplodingProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        float alpha = 1;
        public override void AI()
        {    
            if (timer > 15)
                Projectile.velocity *= 0.8f;
            else if (timer > 10)
                Projectile.velocity *= 0.99f;
            
            if (timer > 15)
            {
                Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, -0.2f, 0.05f), 0f, 1f);
                alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.1f), 0f, 1f);


                if (alpha == 0)
                {
                    for (int fg = 0; fg < 20; fg++)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
                        Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: Color.DodgerBlue, Scale: Main.rand.NextFloat(1f, 1.6f) * 0.4f);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        var v = Main.rand.NextVector2Unit();
                        Dust a = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(1f, 6f), 0,
                            Color.DeepSkyBlue, Main.rand.NextFloat(0.4f, 0.9f));
                    }
                    
                    int afg = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DistortProj>(), 0, 0);
                    Main.projectile[afg].rotation = Main.rand.NextFloat(6.28f);

                    if (Main.projectile[afg].ModProjectile is DistortProj distort)
                    {
                        distort.tex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
                        distort.implode = true;
                        distort.scale = 0.6f;
                    }

                    int explo = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WandOfExplodingExplosion>(), (int)(Projectile.damage * 1.5f), 0, Projectile.owner);

                    if (Main.player[Projectile.owner].statMana <= Main.player[Projectile.owner].statManaMax2 / 2f)
                        SkillStrikeUtil.setSkillStrike(Main.projectile[explo], 1.3f, 100, 0.35f, 0f);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 0.16f, Pitch = 0.5f };
                    SoundEngine.PlaySound(style, Projectile.Center);

                    Projectile.active = false;
                }

            }


            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            timer++;

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/LintyTrail").Value;
            trailColor = Color.DodgerBlue * alpha;
            trailTime = timer * 0.05f;

            // other things you can adjust
            trailPointLimit = 10;
            trailWidth = 10;
            trailMaxLength = 120;

            //MUST call TrailLogic AFTER assigning trailRot and trailPos
            trailRot = Projectile.velocity.ToRotation();
            trailPos = Projectile.Center + Projectile.velocity;
            TrailLogic();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;
            
            TrailDrawing();

            Texture2D Proj = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/ExplodingBolt").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/WandOfExploding/ExplodingBoltGlowMask").Value;
            Texture2D Glorb = Mod.Assets.Request<Texture2D>("Assets/HitAnims/GlorbPMA3").Value;


            int frameHeight = Proj.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Proj.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;


            Main.spriteBatch.Draw(Proj, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor * alpha, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * alpha, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            Color col = Color.DeepSkyBlue;
            col.A = 0;
            Main.spriteBatch.Draw(Glorb, Projectile.Center - Main.screenPosition, null, col * alpha, Projectile.rotation, Glorb.Size() / 2, new Vector2(Projectile.scale, Projectile.scale * 0.5f), SpriteEffects.None, 0f);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, trailWidth, num) * 0.5f; // 0.3f
        }
    }

    public class WandOfExplodingExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() { return timer < 4; }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ManaLeech>(), 300);
        }

        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                if (Projectile.frame == 6)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Explo = Mod.Assets.Request<Texture2D>("Assets/HitAnims/BlueFlareDarkGlowPMA").Value;

            int frameHeight = Explo.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Color glowColor = Color.DeepSkyBlue;
            glowColor.A = 0;

            Color glowColor2 = Color.White;
            glowColor2.A = 0;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Explo.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            Vector2 scale12 = new Vector2(1f, 1f);

            Main.spriteBatch.Draw(Explo, Projectile.Center - Main.screenPosition, sourceRectangle, Color.Black * 0.4f, Projectile.rotation, origin, scale12, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Explo, Projectile.Center - Main.screenPosition, sourceRectangle, glowColor2, Projectile.rotation, origin, scale12, SpriteEffects.None, 0f);

            return false;
        }
    }
}
