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
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using System.Reflection.PortableExecutable;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using System.Threading;
using static AerovelenceMod.Common.Utilities.ProjectileExtensions;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Systems;

namespace AerovelenceMod.Content.Items.Weapons.Ocean
{
    public class OceanMist : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.knockBack = KnockbackTiers.Weak;
            Item.mana = 9;
            Item.shootSpeed = 8f;

            Item.width = 40;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 30;

            Item.shoot = ModContent.ProjectileType<OceanMistHeldProj>();
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(0, 0, 15, 0);

            Item.autoReuse = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes at full mana <1.3x multiplier> [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int a = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<OceanMistHeldProj>(), damage, knockback, player.whoAmI);
            
            if (player.statMana + player.GetManaCost(player.inventory[player.selectedItem]) == player.statManaMax2)
                (Main.projectile[a].ModProjectile as OceanMistHeldProj).shouldSkillStrike = true;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PalmWood, 25).
                AddIngredient(ItemID.Coral, 5).
                AddIngredient(ItemID.FallenStar, 3).
                AddTile(TileID.Anvils).
                Register();
        }

    }

    public class OceanMistHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int timer = 0;
        public float OFFSET = -15; //30
        public float alphaPercent = 0;

        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 100;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        
        public bool shouldSkillStrike = false;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            #region held proj stuff

            Projectile.velocity = Vector2.Zero;
            Player.itemTime = 2; 
            Player.itemAnimation = 2;

            KillHeldProjIfPlayerDeadOrStunned(Projectile);

            //Get angle to mouse
            if (Projectile.owner == Main.myPlayer)
                Angle = (Main.MouseWorld - Player.Center).ToRotation();

            //Have player turn whichever direction they point
            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            float goalRotation = direction.ToRotation() + (MathHelper.PiOver4 * Player.direction);
            float startRotation = goalRotation + (MathHelper.TwoPi * -1.25f) * Player.direction;

            float spinInProgress = Math.Clamp((float)timer / 20f, 0f, 1f);
            Projectile.rotation = MathHelper.Lerp(startRotation, goalRotation, Easings.easeInOutSine(spinInProgress));

            #endregion


            if (timer >= 42)
            {
                if (timer == 42)
                {
                    //Start fadeout
                    Projectile.timeLeft = 15;
                }
                else
                {
                    //Fade out projectile
                    OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, -15f, 0.03f), -20, 14);
                    alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, -0.25f, 0.15f), 0, 1);
                }
            }
            else
            {
                //Fade in
                OFFSET = Math.Clamp(MathHelper.Lerp(OFFSET, 14, 0.2f), -100, 14);
                alphaPercent = Math.Clamp(MathHelper.Lerp(alphaPercent, 1, 0.08f), 0, 1);
            }

            if (timer == 20)
            {
                //FX
                glowAlpha = 1f;
                glowScale = 1f; 

                //Dust
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                
                Vector2 vel = new Vector2(12.5f, 0).RotatedBy(direction.ToRotation());
                
                for (int i = 0; i < 4; i++)
                {
                    if (i < 4)
                    {
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + vel, ModContent.DustType<GlowCircleDust>(), Main.rand.NextVector2Circular(5, 5), new Color(30, 105, 255), 0.6f, 0.7f, 0f, dustShader2);
                        gd.fadeIn = 2;
                        gd.scale *= Main.rand.NextFloat(0.9f, 1.3f);
                    }

                    int a = Dust.NewDust(Projectile.Center + vel * 2, 1, 1, DustID.BlueTorch, Scale: 2f);
                    Main.dust[a].noGravity = true;
                }

                //Sounds
                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/CommonWaterFallLight00") with { Volume = .23f, Pitch = .54f, PitchVariance = .4f, MaxInstances = -1, };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ENV_water_splash_01") with { Pitch = 0.1f, PitchVariance = 0.1f, Volume = 0.75f, MaxInstances = -1 }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                //Spawn Proj
                int shot = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Angle.ToRotationVector2() * 8, ModContent.ProjectileType<OceanMistShot>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

                if (shouldSkillStrike)
                    SkillStrikeUtil.setSkillStrike(Main.projectile[shot], 1.3f, 100, 0.35f, 0f); //1
            }

            //Swoosh Sound 
            if (timer % 7 == 0 && timer <= 20)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Item_7") with { Pitch = .45f, PitchVariance = 0.2f }; SoundEngine.PlaySound(style, Projectile.Center);
            }

            //Vfx values
            glowAlpha = Math.Clamp(MathHelper.Lerp(glowAlpha, -0.5f, 0.05f), 0f, 1f);
            glowScale = Math.Clamp(MathHelper.Lerp(glowScale, -0.15f, 0.02f), 0f, 1.2f);

            // For having the spin always rotate away from the player
            Projectile.ai[0] = Player.direction;

            timer++;
        }

        float justShotPower = 0f;
        float glowAlpha = 0f;
        float glowScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D Weapon = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMist");
            Texture2D Twirl = CommonTextures.PixelSwirl.Value;
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ocean/OceanMistGlowy");


            SpriteEffects mySE = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0f, Player.gfxOffY);

            Color col = shouldSkillStrike ? Color.Gold with { A = 0 } : Color.LightSkyBlue;

            if (timer <= 20)
            {
                Main.spriteBatch.Draw(Twirl, pos, null, col * 0.35f * alphaPercent, Projectile.rotation, Twirl.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(Glow, pos, null, Color.Black * glowAlpha * 0.3f, Projectile.rotation, Glow.Size() / 2, Projectile.scale * glowScale, mySE, 0f);

            Main.spriteBatch.Draw(Glow, pos, null, Color.SkyBlue with { A = 0 } * glowAlpha, Projectile.rotation, Glow.Size() / 2, Projectile.scale * glowScale, mySE, 0f);
            Main.spriteBatch.Draw(Weapon, pos, null, lightColor * alphaPercent, Projectile.rotation, Weapon.Size() / 2, Projectile.scale, mySE, 0f);


            return false;
        }

    }

    public class OceanMistShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
 
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 100;
            Projectile.penetrate = 10;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

        }
        int maximumPierce = 10;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (maximumPierce <= 0)
                return false;

            int i = 0;
            foreach (Vector2 vec in previousPostions)
            {
                i++;
                if (i % 4 == 0 && targetHitbox.Distance(vec) < 10)
                    return true;
            }
            return false;
        }

        int timer = 0;
        public override void AI()
        {
            int trailCount = 20;

            if (timer % 2 == 0)
            {
                previousRotations.Add(Projectile.velocity.ToRotation());
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }


            Projectile.velocity.Y += 0.09f;

            timer++;
        }



        float overallAlpha = 1f;
        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPostions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            PixellationSystem.QueuePixelationAction(() =>
            {
                DrawTrail();
            }, PixellationSystem.RenderType.AlphaBlend);

                return false;
        }

        public void DrawTrail()
        {
            Texture2D line = CommonTextures.Flare.Value;

            //After-Image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.25f) * 0.1f;

                    Vector2 AfterImagePos = previousPostions[i] - Main.screenPosition + Main.rand.NextVector2Circular(5f, 5f); //3f

                    float startScale = Projectile.scale + sineScale;

                    Color between = Color.Lerp(Color.DeepSkyBlue, Color.DodgerBlue, 0.75f);
                    Color col = Color.Lerp(between, Color.DodgerBlue, 1f - progress);

                    float easedFadeValue = Easings.easeInSine(progress);


                    Vector2 lineScale = new Vector2(1.25f, 0.5f + 0.4f * progress); //
                    Vector2 lineScale2 = new Vector2(1.25f, 0.08f + 0.05f * progress); //0.1f 0.2f

                    //Main
                    Main.EntitySpriteDraw(line, AfterImagePos / 2, null, col with { A = 0 } * 1f * easedFadeValue,
                        previousRotations[i], line.Size() / 2f, lineScale * startScale * 0.5f, SpriteEffects.None);

                    //White
                    Main.EntitySpriteDraw(line, AfterImagePos / 2, null, Color.White with { A = 0 } * 1f * easedFadeValue,
                        previousRotations[i], line.Size() / 2f, lineScale2 * startScale * 0.5f, SpriteEffects.None);

                }

            }



        }

        public override void OnKill(int timeLeft)
        {

            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ENV_water_splash_01") with { Pitch = .51f, Volume = 0.5f, MaxInstances = -1 }; SoundEngine.PlaySound(style, Projectile.Center);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (maximumPierce % 2 == 0)
                Projectile.damage = (int)(Projectile.damage * 0.95f);
            maximumPierce--;
        }
    }

}
