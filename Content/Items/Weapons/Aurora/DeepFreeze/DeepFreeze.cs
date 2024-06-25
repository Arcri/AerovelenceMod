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
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using Terraria.Audio;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Aurora.DeepFreeze
{
    public class DeepFreeze : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deep Freeze");
            // Tooltip.SetDefault("'Is it cold in here, or is it just me?'");
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.knockBack = KnockbackTiers.VeryWeak;
            Item.mana = 4;
            Item.shootSpeed = 9f; //8f

            Item.width = 98;
            Item.height = 40;
            Item.useAnimation = 15; //8 1
            Item.useTime = 15;

            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarities.LatePHM;
            Item.shoot = ModContent.ProjectileType<DeepFreezeHeldProjectile>();
            Item.value = Item.sellPrice(0, 4, 0, 0);

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes under 25% mana [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_wind_attack_0") with { Volume = .36f, Pitch = -.84f, PitchVariance = .15f, MaxInstances = 0, };
            SoundEngine.PlaySound(style2, position);
            
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MeteoriteBar, 7).
                AddIngredient(ItemID.HellstoneBar, 7).
                AddRecipeGroup("AerovelenceMod:EvilBars", 7).
                AddTile(TileID.Anvils).
                Register();
        }

    }

    public class DeepFreezeHeldProjectile : ModProjectile
    {
        int maxTime = 50;

        int timer = 0;
        int soundTimer = 0;
        private int OFFSET = 10; //30
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float rotDirection = 0f;
        public float lerpToStuff = 0;

        Vector2 storedMousePos = Vector2.Zero;
        Color eyeCol = Color.White;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            maxTime = 90;
            Projectile.timeLeft = maxTime;


            Projectile.width = Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.scale = 1f;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
        

        Color[] bandColors = new Color[3];
        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Player Player = Main.player[Projectile.owner];
            eyeCol = FetchRainbow(timer);
            bandColors[0] = FetchRainbow((int)(timer * 1.5f));
            bandColors[1] = FetchRainbow((int)(timer * 2f));
            bandColors[2] = FetchRainbow((int)(timer * 2.5f));

            storedMousePos = Vector2.Lerp(storedMousePos, Main.MouseWorld, 0.04f);

            if (timer % 10 == 0 && Main.rand.NextBool() && timer > 0) 
            {
                
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                Vector2 offsetVel = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextBool() ? Main.rand.NextFloat(-0.6f, -0.3f) : Main.rand.NextFloat(0.3f, 0.6f));
                Dust p = GlowDustHelper.DrawGlowDustPerfect((Projectile.rotation.ToRotationVector2() * 45) + Projectile.Center, 
                    ModContent.DustType<FuzzySpark>(), offsetVel * (4f + Main.rand.NextFloat(-1f, 1f)),
                    eyeCol, Main.rand.NextFloat(0.1f, 0.1f), 0.4f, 0f, dustShader);
                p.noLight = true;
                
            }

            if (timer == 0)
            {
                storedMousePos = Main.MouseWorld;
                rotDirection = (Main.MouseWorld - Player.Center).ToRotation();
            }

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (storedMousePos - (Player.Center)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                maxTime = 30;
            }
            else
            {
                maxTime--;
            }

            if (maxTime <= 0 || !Player.CheckMana(Player.inventory[Player.selectedItem], pay: false))
                Projectile.active = false;

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);


            float targetRot = (Main.MouseWorld - Player.Center).ToRotation();
            float diff = CompareAngle(Projectile.rotation, targetRot);
            float maxRot = 0.03f;
            rotDirection -= MathHelper.Clamp(diff, -maxRot, maxRot);

            Player.ChangeDir(rotDirection.ToRotationVector2().X > 0 ? 1 : -1);

            //direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.Center + (rotDirection.ToRotationVector2() * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = rotDirection;//.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = rotDirection;// direction.ToRotation();

            //The > 5 check is for weird instances where the shot would go off in random dir
            if (timer % 6 == 0 && maxTime > 5 && Player.CheckMana(Player.inventory[Player.selectedItem], pay: true))
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_55") with { Volume = .06f, Pitch = .85f, PitchVariance = .1f, MaxInstances = 0, };
                SoundEngine.PlaySound(style, Projectile.Center);

                if (Player.channel)
                {
                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_wind_attack_0") with { Volume = .30f, Pitch = -.84f, PitchVariance = .25f, MaxInstances = 0, };
                    SoundEngine.PlaySound(style2, Projectile.Center);
                }
                

                Vector2 spawnPos = Projectile.Center + (Projectile.rotation.ToRotationVector2() * 35);
                int a = Projectile.NewProjectile(null, spawnPos, Projectile.rotation.ToRotationVector2() * 5, ModContent.ProjectileType<DeepFreezeProj>(), 0, 0);
                Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                if (Main.projectile[a].ModProjectile is DeepFreezeProj explo)
                {
                    explo.size = 1f;
                    explo.multiplier = 1.5f;
                    explo.sticky = true;
                }
                int aa = Projectile.NewProjectile(null, spawnPos, Projectile.rotation.ToRotationVector2() * 4.5f, ModContent.ProjectileType<AuroraBlast>(), Projectile.damage, 0, Main.myPlayer);

                if (Player.statMana < (Player.statManaMax2 / 4))
                    SkillStrikeUtil.setSkillStrike(Main.projectile[aa], 1.3f, 100, 0.15f, 0f);
            }

            timer++;
        }

        //From SLR
        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/DeepFreeze_Glow").Value;
            Texture2D orb = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/spiky_10").Value;
            Texture2D orb2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_1").Value;


            Texture2D eyeOrb = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_3").Value;


            Texture2D eye = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/eyeGlow").Value;
            Texture2D band1 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/band1").Value;
            Texture2D band2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/band2").Value;
            Texture2D band3 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/band3").Value;


            Vector2 origin = texture.Size() / 2f;
            Vector2 position = (Projectile.Center - (0.5f * (rotDirection.ToRotationVector2() * OFFSET * -1f)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition).Floor();
            Vector2 orbPosition = (Projectile.Center - (0.5f * (rotDirection.ToRotationVector2() * OFFSET * -10f)) + new Vector2(0f, Player.gfxOffY) - Main.screenPosition).Floor();

            SpriteEffects effects = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float extraRot = Player.direction == 1 ? 0 : MathF.PI;

            Main.EntitySpriteDraw(texture, new Vector2((int)position.X, (int)position.Y), null, lightColor, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(eye, position, null, eyeCol, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);

            Vector2 eyeStarScale = new Vector2(0.15f, 1f);
            Vector2 eyePos = new Vector2(23f * Player.direction, -9f).RotatedBy(rotDirection + extraRot) + position;

            float sinScale = (float)Math.Sin((float)timer * 0.06f) * 0.02f;

            //MouthOrb
            Main.spriteBatch.Draw(orb, orbPosition, null, eyeCol * 0.3f, rotDirection - (timer * 0.1f * Player.direction), orb.Size() / 2, 0.17f + sinScale, effects, 0.0f);
            Main.spriteBatch.Draw(orb, orbPosition, null, eyeCol * 0.5f, rotDirection + (timer * 0.05f * Player.direction), orb.Size() / 2, 0.15f + sinScale, effects, 0.0f);
            Main.spriteBatch.Draw(orb2, orbPosition, null, Color.White, rotDirection + (timer * 0.01f * Player.direction), orb2.Size() / 2, 0.15f + sinScale, effects, 0.0f);


            //EyeOrb
            Main.EntitySpriteDraw(eyeOrb, eyePos, null, eyeCol * 0.5f, rotDirection + (timer * 0.1f * Player.direction) + extraRot, eyeOrb.Size() / 2, 0.4f * eyeStarScale, effects, 0.0f);
            Main.EntitySpriteDraw(eyeOrb, eyePos, null, eyeCol * 0.5f, rotDirection + (timer * 0.05f * Player.direction) + extraRot, eyeOrb.Size() / 2, 0.3f * eyeStarScale, effects, 0.0f);
            Main.EntitySpriteDraw(eyeOrb, eyePos, null, Color.White * 0.5f, rotDirection + (timer * 0.01f * Player.direction) + extraRot, eyeOrb.Size() / 2, 0.2f * eyeStarScale, effects, 0.0f);

            //ExtraEye
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(eye, position + Main.rand.NextVector2Circular(4, 4), null, bandColors[i] * 0.6f, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);

                Main.EntitySpriteDraw(band1, position + Main.rand.NextVector2Circular(2, 2), null, eyeCol * 0.6f, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);
                Main.EntitySpriteDraw(band2, position + Main.rand.NextVector2Circular(2, 2), null, eyeCol * 0.6f, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);
                Main.EntitySpriteDraw(band3, position + Main.rand.NextVector2Circular(2, 2), null, eyeCol * 0.6f, rotDirection + extraRot, origin, Projectile.scale, effects, 0.0f);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //I have no fucking idea why I have to do this twice but if I don't the player's arm will draw with additive blending somehow someway somewhy help
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public Color FetchRainbow(int timer)
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(timer * 2));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(timer * 2 + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(timer * 2 + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }


    }


    //Instead of making every wind a projectile, we are making 1 proj have 5 winds
    public class IcyWind
    {
        public Vector2 Velocity;
        public Vector2 Center;

        public float rotation;
        public Color color;
        public float scale;

        public int timer;

        public Vector2 distanceFromPlayer = Vector2.Zero;
        public int playerIndex = 0;

        public IcyWind(Vector2 pos, Vector2 vel, Vector2 distanceFromPlayer, int playerIndex)
        {
            Center = pos;
            Velocity = vel;
            scale = 0f;
            rotation = (float)Main.rand.NextDouble() * 6.28f;
            this.distanceFromPlayer = distanceFromPlayer;
            this.playerIndex = playerIndex;
        }

        public void Update()
        {
            float size = 0.35f; //.45 for Rime
            scale = MathHelper.Clamp(MathHelper.Lerp(scale, size, 0.04f), 0f, size / 2);

            distanceFromPlayer += Velocity;
            Center = distanceFromPlayer + Main.player[playerIndex].Center;
            
            //Center += Velocity;
            timer++;
        }


        public void DrawIce(SpriteBatch sb, Texture2D tex)
        {
            sb.Draw(tex, Center - Main.screenPosition, null, color, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }

    public class AuroraBlast : ModProjectile
    {

        public List<IcyWind> Wind = new List<IcyWind>();
        public int timer = 0;
        public bool spawnedWind = false;
        float colorIntensity = 1.75f;
        float colorTimeOffset = Main.rand.NextFloat(0, 1000);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.scale = 3f;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }


        bool enemyHit = false;
        public override bool? CanDamage()
        {
            if (colorIntensity < 1f || enemyHit)
                return false;
            return true;
        }

        Vector2 startingVel = Vector2.Zero;
        Vector2 distFromPlayer = Vector2.Zero;

        public override void AI()
        {
            if (!spawnedWind)
            {
                if (timer == 0)
                    distFromPlayer = Projectile.Center - Main.player[Projectile.owner].Center;

                for (int i = 0; i < 5; i++)
                {
                    IcyWind newWind = new IcyWind(Projectile.Center, 
                        Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f,0.05f)) * Main.rand.NextFloat(0.85f, 1.15f), 
                        distFromPlayer, Projectile.owner);

                    Wind.Add(newWind);
                }
                spawnedWind = true;

                startingVel = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;

            }

            foreach (IcyWind wind in Wind)
            {
                wind.Update();
            }

            if (colorIntensity < 1f)
                colorIntensity -= 0.04f;
            else
                colorIntensity -= 0.02f;

            if (colorIntensity <= 0)
                Projectile.active = false;

            distFromPlayer += startingVel;
            Projectile.Center = Main.player[Projectile.owner].Center + distFromPlayer;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Aurora/DeepFreeze/DeepFreezeProj").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(FetchRainbow().ToVector3() * (colorIntensity));
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.5f); //0.6
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();
            foreach (IcyWind wind in Wind)
            {
                wind.DrawIce(Main.spriteBatch, Tex);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public Color FetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(timer + colorTimeOffset));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(timer + colorTimeOffset + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(timer + colorTimeOffset + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff<AuroraFire>())
            {
                target.AddBuff(ModContent.BuffType<AuroraFire>(), 120);

                //This check is to make sure that enemies immune to the debuff (like Cultist) dont repeatedly play the inflict debuff sound when hit being hit by the weapon
                if (target.HasBuff<AuroraFire>())
                {
                    SoundStyle stylea = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_flame_breath") with { Volume = .4f, Pitch = .64f, PitchVariance = .22f, };
                    SoundEngine.PlaySound(stylea, target.Center);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = .42f, PitchVariance = .42f, Volume = 0.4f, MaxInstances = 2 };
                    SoundEngine.PlaySound(style, target.Center);
                }


                for (int i = 0; i < 9; i++)
                {
                    //have to make new dustShader everytime so color is different
                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    Color c = new Color(
                        (byte)Main.rand.Next(0, 255),
                        (byte)Main.rand.Next(0, 255),
                        (byte)Main.rand.Next(0, 255));

                    int p = GlowDustHelper.DrawGlowDust(target.position, target.width, target.height, ModContent.DustType<GlowCircleQuadStar>(), c, 0.7f, 0.6f, 0f, dustShader);
                    Main.dust[p].noLight = true;
                    Main.dust[p].velocity *= 0.3f;

                    Main.dust[p].velocity = Vector2.Normalize(target.Center - Main.dust[p].position) * Main.rand.NextFloat(5,10);

                    //int p = GlowDustHelper.DrawGlowDust(target.position, target.width, target.height, ModContent.DustType<GlowCircleDust>(), c * 2f, 0.75f, 0.8f, 0f, dustShader);
                    Main.dust[p].fadeIn = 35 + Main.rand.NextFloat(5, 15f);
                    //Main.dust[p].velocity *= 3f;
                }
                
            }
            
            target.AddBuff(ModContent.BuffType<AuroraFire>(), 120);
            target.immune[Projectile.owner] = 10; //20 
        }
    }
}
