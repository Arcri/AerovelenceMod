using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.CrystalGlade
{
    public class CrystalGlade : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 73;
            Item.knockBack = KnockbackTiers.Average;
            Item.mana = 9;

            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.shootSpeed = 10f;

            Item.width = 34;
            Item.height = 38;

            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarities.PlanteraGolemTier;
            Item.shoot = ModContent.ProjectileType<CrystalGladeHeld>();
            Item.value = Item.sellPrice(0, 4, 90, 0);

            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Shots from the portal Skill Strike [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient(ItemID.ChlorophyteBar, 12).
                AddIngredient(ItemID.CrystalShard, 15).
                AddTile(TileID.Bookcases).
                Register();
        }

        public override bool AltFunctionUse(Player player) {  return true; }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 2f;
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 2.5f;
            else
                return 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.projectile)
                {
                    //Stacked for efficiency 
                    if (p.type == ModContent.ProjectileType<CrystalGladeFirst>())
                    {
                        if (p.active == true && p.owner == player.whoAmI)
                            p.Kill();
                    }
                }
                player.itemAnimation = player.itemTime;

                Projectile.NewProjectile(source, position, velocity * 1.5f, ModContent.ProjectileType<CrystalGladeFirst>(), damage, knockback, player.whoAmI);

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_hurt_2") with { Pitch = 0.3f, Volume = 1f, PitchVariance = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, player.Center);
            }
            else
            {
                bool found = false;
                //Check if the alt fire projectile exists
                foreach (Projectile p in Main.projectile)
                {
                    //Stacked for efficiency 
                    if (p.type == ModContent.ProjectileType<CrystalGladeFirst>())
                    {
                        if (p.active == true && p.owner == player.whoAmI)
                        {
                            if (p.ModProjectile is CrystalGladeFirst cgf)
                                cgf.Fire();
                            found = true;
                        }
                    }
                }

                //Shoot normally
                if (!found)
                {
                    Projectile.NewProjectile(source, position, velocity * 0.5f, ModContent.ProjectileType<CrystalGladeShot>(), damage, knockback, player.whoAmI);

                    for (int i = 0; i < 3; i++)
                    {
                        Dust d = Dust.NewDustPerfect(position, ModContent.DustType<MuraLineBasic>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.85f, 1.25f) * 0.4f, 0, Color.Green, 0.25f);
                        d.alpha = Main.rand.Next(3, 8);
                    }

                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Pitch = 0f, PitchVariance = .1f, Volume = 0.35f };
                    SoundEngine.PlaySound(style2, player.Center);

                }

            }


            return true;
        }

    }

    public class CrystalGladeHeld : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private bool firstFrame = false;

        private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

        Player owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() { return false; }
        public override void AI()
        {
            owner.heldProj = Projectile.whoAmI;

            if (owner.itemTime <= 1)
                Projectile.active = false;

            Projectile.Center = owner.Center;

            if (!firstFrame)
            {
                firstFrame = true;
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
            }

            glowIntensity = Math.Clamp(MathHelper.Lerp(glowIntensity, -0.5f, 0.05f), 0, 1);
        }

        private float glowIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGlade").Value;
            Texture2D GlowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeNewGlowmask").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeNewWhite2").Value;

            Vector2 position = (owner.Center + (currentDirection * 10)) - Main.screenPosition;
            position.Y += owner.gfxOffY;

            float rotation = currentDirection.ToRotation() + (owner.direction == 1 ? 0 : -MathF.PI);
            SpriteEffects SE = (owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            Main.spriteBatch.Draw(Texture, position, null, lightColor, rotation, Texture.Size() / 2, 1f, SE, 0.0f);
            Main.spriteBatch.Draw(GlowMask, position, null, Color.White, rotation, Texture.Size() / 2, 1f, SE, 0.0f);
            Main.spriteBatch.Draw(Glow, position, null, Color.MediumSeaGreen with { A = 0 } * glowIntensity * 0.8f, rotation, Glow.Size() / 2, 1f, SE, 0.0f);

            return false;
        }
    }

    public class CrystalGladeShot : ModProjectile
    {
        public int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.scale = 1f;

            Projectile.timeLeft = 125;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            if (timer < 25 && timer > 5) 
                Projectile.velocity *= 1.105f;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            int modVal = timer < 20 ? 3 : 1;
            if (timer % modVal == 0)
            {
                Vector2 vel = Projectile.velocity * 0.2f;

                //Spawn dust 75% of the time 
                if (timer % 2 != 0 || Main.rand.NextBool())
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowStrong>(), vel.X, vel.Y, 3, Color.Green, 0.35f);
            }

            Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, 2f, 0.04f), 0, 1);

            timer++;
        }

        float alpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Base = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeProj");
            Texture2D GlowMask = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeGlow");
            Texture2D Glow2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeGlow2");
            Texture2D Glow3 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Magic/CrystalGlade/CrystalGladeGlow3");

            Vector2 vscale = new Vector2(1f, 1f - (Projectile.velocity.Length() * 0.00f)) * Projectile.scale; //0.01
            SpriteEffects se = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - ((float)i / Projectile.oldPos.Length);

                Main.spriteBatch.Draw(Base, Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition, null, lightColor with { A = 0 } * progress * 0.85f, Projectile.rotation, Base.Size() / 2, vscale, se, 0f);
                Main.spriteBatch.Draw(Glow3, Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition, null, Color.DarkGreen with { A = 0 }, Projectile.oldRot[i], Glow3.Size() / 2f, vscale * 0.8f * progress, se, 0f);
            }

            Main.spriteBatch.Draw(Base, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Base.Size() / 2, vscale, se, 0f);
            Main.spriteBatch.Draw(GlowMask, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, GlowMask.Size() / 2, vscale, se, 0f);

            Main.spriteBatch.Draw(Glow2, Projectile.Center - Main.screenPosition, null, Color.Green with { A = 0 } * (Projectile.velocity.Length() * 0.08f), Projectile.rotation, Glow2.Size() / 2, vscale, se, 0f);
            Main.spriteBatch.Draw(Glow3, Projectile.Center - Main.screenPosition, null, Color.Green with { A = 0 } * (Projectile.velocity.Length() * 0.04f), Projectile.rotation, Glow3.Size() / 2, vscale, se, 0f);


            return false;
        }




        public override void OnKill(int timeLeft)
        {
            SoundStyle style4 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_crystal_impact_1") with { Pitch = 0f, PitchVariance = .25f, MaxInstances = 0, Volume = 0.5f }; 
            SoundEngine.PlaySound(style4, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = Projectile.rotation.ToRotationVector2() * 2f + (Projectile.velocity * 0.15f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineBasic>(), vel.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.85f, 1.25f), 0, Color.Green, 0.3f);
                d.alpha = Main.rand.Next(0, 8);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 1.75f;
                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), randomStart * Main.rand.NextFloat(0.9f, 1.5f));
                dust1.velocity += (Projectile.velocity * 0.15f);
                dust1.scale = 0.3f;
                dust1.alpha = 2;
                dust1.color = Color.ForestGreen;
                dust1.noGravity = true;
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike == true)
            {
                Color col = Color.Gold;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 1.75f;
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), randomStart * Main.rand.NextFloat(0.9f, 1.5f));
                    dust1.velocity += (Projectile.velocity * 0.25f);
                    dust1.scale = 0.3f;
                    dust1.alpha = 0; //2
                    dust1.color = col;
                }
            }

        }
    }

    //This is the little vfx pulse the star does
    public class CrystalGladePulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 1;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;
        
        int timer = 0;
        float scale = 0;
        float alpha = 1;

        public int rotDir = 1;

        public override void AI()
        {
            Projectile.rotation += rotDir * 0.01f;

            if (timer < 15)
            {
                scale = MathHelper.Lerp(scale, 0.65f, 0.04f);
            }
            else
                alpha -= 0.04f;

            if (timer >= 12)
            {
                scale = scale + 0.03f;
                alpha -= 0.01f;
            }

            Projectile.timeLeft = 2;

            if (alpha <= 0) 
                Projectile.active = false;

            //Intentional
            timer++;
            timer++;

        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //COOLNESS BELOW
            //A: Flare = DrawnStar | caus = Noise_1 | Grad = SofterBlueGrad | Noise = Swirl | 0.3, 1, 0.8, 0.06, 0, time * 0.01, alpha * 1.6 | drawn twice
            //^ Spawn twice in quick succession 

            //B: Flare = spotlight_8 | caus = Noise_1 | Grad = orangeGrad | Noise = Swirl | 0.3, 1, 0.8, 0.06, 0, time * 0.01, alpha * 2.6 | drawn twice

            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/TrailImages/GlowStar").Value;
            //Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/DrawnStar").Value;


            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/GreenGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 1.6f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale * 2.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, Flare.Size() / 2, scale * 2.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale * 2.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, Flare.Size() / 2, scale * 2.25f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class CrystalGladeFirst : ModProjectile
    {
        public int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.timeLeft = 500;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool? CanDamage() { return false; }

        public override void AI()
        {
            if (timer == 0)
                rotDir = Projectile.velocity.X > 0 ? 1 : -1;

            Projectile.velocity *= 0.95f;

            Projectile.rotation += (0.02f + (Projectile.velocity.Length() * 0.02f)) * rotDir;
            glowRot += 0.08f * rotDir;

            scale = Math.Clamp(MathHelper.Lerp(scale, 1.5f, 0.06f), 0, 1);

            Projectile.scale = scale;
            timer++;
        }

        float scale = 0f;
        int rotDir = 1;
        float glowRot = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Base = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/GlowStar");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");

            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(Base, pos, null, Color.Black * 0.25f, Projectile.rotation, Base.Size() / 2, scale * 0.8f, 0, 0f);
            Main.spriteBatch.Draw(Base, pos, null, Color.Black * 0.25f, Projectile.rotation + MathHelper.PiOver4, Base.Size() / 2, scale * 0.8f, 0, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(Glow, pos, null, Color.Green * 0.65f, glowRot, Glow.Size() / 2, scale * 0.28f, 0, 0f);
            Main.spriteBatch.Draw(Glow, pos, null, Color.Green * 0.65f, glowRot * -1.5f, Glow.Size() / 2, scale * 0.22f, 0, 0f);

            float rot = (float)Main.timeForVisualEffects * 0.12f;
            float offsetVal = MathF.Sin((float)Main.timeForVisualEffects * 0.14f) * 0.5f;

            Vector2 v = new Vector2(1.5f + offsetVal * (scale * 2), 0) + Main.rand.NextVector2Circular(1,1);
            Vector2 v2 = new Vector2(1.5f * (scale * 2), 0);


            float scale1 = (1f + (MathF.Sin((float)Main.timeForVisualEffects * 0.04f) * 0.1f)) * 0.8f * scale;
            float scale2 = (1f + (MathF.Cos((float)Main.timeForVisualEffects * 0.02f) * 0.1f)) * 1f * scale;//scale * 1f;

            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot), null, Color.Green * 0.5f, Projectile.rotation, Base.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot + MathHelper.PiOver2), null, Color.GreenYellow * 0.5f, Projectile.rotation, Base.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot + MathHelper.Pi), null, Color.ForestGreen * 0.5f, Projectile.rotation, Base.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot - MathHelper.PiOver2), null, Color.Aqua * 0.65f, Projectile.rotation, Base.Size() / 2, scale1, 0, 0f);


            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot), null, Color.Green * 0.5f, Projectile.rotation + MathHelper.PiOver4, Base.Size() / 2, scale2, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot + MathHelper.PiOver2), null, Color.GreenYellow * 0.5f, Projectile.rotation + MathHelper.PiOver4, Base.Size() / 2, scale2, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot + MathHelper.Pi), null, Color.ForestGreen * 0.5f, Projectile.rotation + MathHelper.PiOver4 , Base.Size() / 2, scale2, 0, 0f);
            Main.spriteBatch.Draw(Base, pos + v.RotatedBy(rot - MathHelper.PiOver2), null, Color.Aqua * 0.65f, Projectile.rotation + MathHelper.PiOver4, Base.Size() / 2, scale2, 0, 0f);


            Main.spriteBatch.Draw(Base, pos, null, Color.White * 0.85f, Projectile.rotation, Base.Size() / 2, scale1, 0, 0f);
            Main.spriteBatch.Draw(Base, pos, null, Color.White * 0.85f, Projectile.rotation + MathHelper.PiOver4, Base.Size() / 2, scale2, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void Fire()
        {
            Vector2 vel = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 5f;

            int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<CrystalGladeShot>(), Projectile.damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
            int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalGladePulse>(), 0, 0, Main.player[Projectile.owner].whoAmI);
            Main.projectile[a].rotation = Projectile.rotation;


            if (Main.projectile[a].ModProjectile is CrystalGladePulse cgp)
                cgp.rotDir = rotDir;

            SkillStrikeUtil.setSkillStrike(Main.projectile[shot], 1.3f, 1, 0.15f, 0f);

            for (int i = 0; i < 6; i++)
            {
                float rotAmount = Main.rand.NextBool() ? 2f : -2f;
                rotAmount += Main.rand.NextFloat(-0.75f, 0.75f);

                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), vel.SafeNormalize(Vector2.UnitX).RotatedBy(rotAmount) * Main.rand.NextFloat(1f, 4f));
                dust1.scale = Main.rand.NextFloat(0.25f, 0.4f); ;
                dust1.color = Color.Green;
                dust1.alpha = 2;
            }

            for (int i = 0; i < 8; i++)
            {
                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), vel.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 8f));
                dust1.scale = Main.rand.NextFloat(0.35f, 0.45f); ;
                dust1.color = Color.Green;
            }

            //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_101") with { Pitch = 0.15f, Volume = 0.3f, PitchVariance = 0.15f };
            //SoundEngine.PlaySound(style, Projectile.Center);

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Pitch = 0f, PitchVariance = .1f, Volume = 0.35f };
            SoundEngine.PlaySound(style2, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 start = Vector2.UnitX * 2.2f;
                Vector2 vel = start.RotatedBy((i * MathHelper.PiOver4) + Projectile.rotation);

                Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), vel, 2, Color.ForestGreen, 0.5f * Projectile.scale);
            }
            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/star_impact_01") with { Pitch = 0.2f, PitchVariance = .12f, Volume = 0.4f, MaxInstances = -1 };
            SoundEngine.PlaySound(style2, Projectile.Center);

            Projectile.active = false;
        }
    }

}