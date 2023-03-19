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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee
{
    public class EntourageCrusher : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Entourage Crusher");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 38;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Purple;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<EntourageCrusherHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
           tick = !tick;
           Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
           return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 30).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class EntourageCrusherHeldProj : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Entourage Crusher");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

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
            Projectile.extraUpdates = 8;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f);
            return shouldDamage;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;

        float Angle = 0;

        bool playedSound = false;
        bool shotShards = false;

        float storedDirection = 1;

        int timer = 0;

        int timerAfterEnd = 8;
        float mytrailWidth = 60;
        public override void AI()
        { 
            Player player = Main.player[Projectile.owner];
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Projectile.Center - (player.Center)).ToRotation();
            }

            if (firstFrame)
                storedDirection = player.direction;

            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = Angle + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            //if (easingProgress > 0.5)
                //player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            //player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (frontHandPos - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword 
            if (firstFrame)
            {
                if (Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits >= Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().hitsForCrit)
                {
                    Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
                    Projectile.GetGlobalProjectile<SkillStrikeGProj>().travelDust = (int)SkillStrikeGProj.TravelDustType.None;
                    Projectile.GetGlobalProjectile<SkillStrikeGProj>().critImpact = (int)SkillStrikeGProj.CritImpactType.glowTargetCenter;
                }

                easingProgress = 0.15f;
                timer = 0;
                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);

                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                //we set Projectile.ai[0] in the slate set. This is so the sword alternates direction
                if (Projectile.ai[0] == 1)
                {
                    startingAng = startingAng - MathHelper.ToRadians(-160);
                }
                else
                {
                    startingAng = startingAng + MathHelper.ToRadians(-160);
                }

                currentAng = startingAng;
                firstFrame = false;
            }


            if (timer >= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((320 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((320 * getProgress(easingProgress)));

                easingProgress = Math.Clamp(easingProgress + 0.002f * Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee), 0.05f, 0.95f); //0.002
            }


            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * 50) + player.RotatedRelativePoint(player.MountedCenter);
            player.itemTime = 10;
            player.itemAnimation = 10;

            timer++;

            if (getProgress(easingProgress) >= 0.35f && getProgress(easingProgress) <= 0.78f)
            {
                Projectile.ai[1] = 1;
            }
            else
                Projectile.ai[1] = 0;

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                //int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OzoneShredderDistort>(), 0, 0, Main.myPlayer);
                //distortProj = Main.projectile[proj];

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.2f, PitchVariance = 0.15f, Volume = 0.67f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.8f, Pitch = 0.8f }, Projectile.Center);
                //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Volume = .22f, Pitch = .59f, PitchVariance = 0.3f };
                //SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Slash_Heavy_S_a") with { Volume = .4f, Pitch = -.33f, PitchVariance = .35f, };
                SoundEngine.PlaySound(style);

                //String extra = Main.rand.NextBool() ? "M_a" : "L_a";
                String soundLocation = Main.rand.NextBool() ? "AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_L_a" : "AerovelenceMod/Sounds/Effects/GGS/Sword_Sharp_L_b";
                SoundStyle slash = new SoundStyle(soundLocation) with { Pitch = -0.25f, PitchVariance = .3f, Volume = 0.1f };
                SoundEngine.PlaySound(slash, Projectile.Center);
                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f)
            {
                if (timerAfterEnd == 0)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    if (!hasHit)
                        Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits = 1;
                    Projectile.active = false;
                }
                timerAfterEnd--;
                
            }

            if (getProgress(easingProgress) >= 0.8)
            {
                mytrailWidth = MathHelper.Lerp(mytrailWidth, 0, 0.015f);

            }

            //Trail
            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trailColor = Color.Purple * 1f;
            trailPointLimit = 800;
            trailMaxLength = 300;
            trailTime = timer * 0.007f;
            timesToDraw = 2;

            trailRot = Projectile.rotation + MathHelper.PiOver4;
            trailPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.75f) * 37;
            TrailLogic();

            //Dust
            if (timer % 6 == 0 && getProgress(easingProgress) > 0.2f && getProgress(easingProgress) < 0.7f)
            {
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                int a = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowCircleDust>(),
                    Color.Purple, 0.55f, 0.4f, 0f, dustShader2);
                //int a = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard);
                Main.dust[a].noGravity = true;
                Main.dust[a].color = Color.Purple;
                Main.dust[a].velocity *= 0.4f;
                Main.dust[a].velocity += Projectile.rotation.ToRotationVector2() * 4;
                Main.dust[a].fadeIn = 2;
            }

            if (timer % 9 == 0 && getProgress(easingProgress) > 0.3f && getProgress(easingProgress) < 0.7f)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                int a = GlowDustHelper.DrawGlowDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FuzzySpark>(),
                    Color.Purple, 0.1f, 0.35f, 0, dustShader);
                //int a = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard);
                Main.dust[a].noGravity = true;
                Main.dust[a].color = Color.Purple;
                Main.dust[a].velocity *= 0.5f;
                Main.dust[a].velocity += Projectile.rotation.ToRotationVector2() * 4;
                Main.dust[a].fadeIn = 45;
            }

            /*
            if (getProgress(easingProgress) >= 0.5f && !shotShards)
            {
                for (int j = 0; j < 5; j++)
                {
                    int b = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(0.4f) * 5, ProjectileID.CrystalStorm, 2, 0, Main.myPlayer);
                    Main.projectile[b].timeLeft = 20;
                }
                shotShards = true;
            }
            */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusherBlade");


            Vector2 aeOffset = new Vector2(Blade.Width / 2, Blade.Height / 2);

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 1f) * 0.3f + 1.3f;
            Color effectColor = Color.Purple * 0.5f;
            if (Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike)
                effectColor = Color.Purple * 1f;

            effectColor = effectColor * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                Main.spriteBatch.Draw(Blade, Projectile.Center + (TwoPi * num5).ToRotationVector2() * (6f + offset * 1.5f) - Main.screenPosition, null, effectColor,
                    Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2,
                    Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusher");
            Main.spriteBatch.Draw(Sword, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Sword.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            //Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/EntourageCrusherBlade");
            //Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, Color.MediumPurple, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            
            return false;
        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            #region easeExpo
            /*
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;
            */

            #endregion;

            #region easeCircle
            /*
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            }
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            */
            #endregion

            #region easeOutBack
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            #endregion
        }

        public override float WidthFunction(float progress)
        {
            return mytrailWidth;
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * (mytrailWidth / 28); // 0.3f
            
        }


        bool hasHit = false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!hasHit)
            {
                Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits = Main.player[Projectile.owner].GetModPlayer<EntourageCounter>().successiveHits + 1;
                hasHit = true;
            }
            
            hasHit = true;

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_hurt_2") with { Pitch = .25f, PitchVariance = .35f, Volume = 0.85f};
            SoundEngine.PlaySound(style, target.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_0") with { Pitch = .1f, PitchVariance = .25f, Volume = 0.7f };
            SoundEngine.PlaySound(style, target.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = (target.Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));

                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowSpark>(), vel.SafeNormalize(Vector2.UnitX) * (14f + Main.rand.NextFloat(-2f, 2f)),
                    Color.Purple, Main.rand.NextFloat(0.1f, 0.3f), 0.9f, 0f, dustShader);
                p.fadeIn = 50 + Main.rand.NextFloat(-10, 15);
                p.velocity *= 0.3f;
            }

            for (int j = 0; j < 0; j++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.rotation.ToRotationVector2() * 4, ProjectileID.CrystalStorm, 2, 0, Main.myPlayer);
            }

            //GlowDustHelper.Draw

        }
    }

    public class EntourageCounter : ModPlayer
    {
        public int successiveHits = 1;
        public int hitsForCrit = 4;
        public override void ResetEffects()
        {
            ResetVariables();
        }
        public override void UpdateDead()
        {
            ResetVariables();
        }
        private void ResetVariables()
        {
            //successiveHits = 1;
        }

    }
}
