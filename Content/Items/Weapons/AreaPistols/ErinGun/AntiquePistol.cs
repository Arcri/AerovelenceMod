using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.Players;

namespace AerovelenceMod.Content.Items.Weapons.AreaPistols.ErinGun
{
    public class AntiquePistol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antique Pistol");
            Tooltip.SetDefault("'Property of Erin'");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = SoundID.DD2_BallistaTowerShot with { Volume = 0.9f, Pitch = 0.4f, PitchVariance = 0.15f };
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = 27; //27
            Item.useAnimation = 27; //27
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BeeArrow;
            Item.shootSpeed = 8f;
            Item.noUseGraphic = true;

        }

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ErinCircle>()] < 1)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<ErinCircle>(), 0, 0, player.whoAmI);

            /*
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AmmoUI>()] < 1)
            {
                int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<AmmoUI>(), 0, 0, player.whoAmI);
                if (Main.projectile[a].ModProjectile is AmmoUI ui)
                {
                    ui.whatWeapon = AmmoUI.whatWeaponEnum.ErinGun;
                }
            }
            */
            //Main.NewText(player.GetModPlayer<AmmoPlayer>().ErinAmmoCount);




            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
            }
            else
            {
                Item.noUseGraphic = false;
            }
            if (player.GetModPlayer<AmmoPlayer>().ErinAmmoCount <= 0)
                Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            if (player.GetModPlayer<AmmoPlayer>().ErinAmmoCount <= 0)
            {
                player.itemAnimation = 0;
                player.itemTime = 0;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ReloadProj>()] < 1)
                    Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ReloadProj>(), damage, knockback, Main.myPlayer);
                return false;
            }

            if (player.altFunctionUse == 2)
            {
                damage = damage * 2;
                int altFire = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ErinAltFireHeldProjectile>(), damage, knockback, Main.myPlayer);
                return false;
            }

            player.GetModPlayer<AmmoPlayer>().ErinAmmoCount = player.GetModPlayer<AmmoPlayer>().ErinAmmoCount - 1;
            player.GetModPlayer<ErinGunPlayer>().justShotTime = 5;


            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_defense_tower_spawn") with { Pitch = .66f, MaxInstances = 1, PitchVariance = 0.3f, Volume = 0.5f };
            SoundEngine.PlaySound(style, position);
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_0") with { Pitch = .85f, PitchVariance = .25f, };
            SoundEngine.PlaySound(style2, position);

            //SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Volume = 0.7f, Pitch = 0.4f, PitchVariance = 0.15f });

            Gore.NewGore(source, position + velocity, new Vector2(velocity.X * -0.2f, -2), ModContent.GoreType<BulletCasing>());

            //int nm = Projectile.NewProjectile(source, position + new Vector2(-75, 0), Vector2.Zero, ModContent.ProjectileType<BigErinImpact>(), 0, 0, Main.myPlayer);
            //Main.projectile[nm].scale = 1.5f;
            //int fire = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ErinGunFireSuck>(), 0, 0, Main.myPlayer);
            //Main.projectile[fire].scale = 2f;


            //int flashIndex2 = Projectile.NewProjectile(source, position + new Vector2(56, velocity.X > 0 ? -10 : 10).RotatedBy(velocity.ToRotation()), Vector2.Zero, ModContent.ProjectileType<ErinGunFireSuck>(), 0, 0, Main.myPlayer);
            //Projectile mFlash2 = Main.projectile[flashIndex2];
            //mFlash2.rotation = velocity.ToRotation() + MathHelper.PiOver2;
            //SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_dark_mage_cast_heal_1") with { Pitch = .2f, };
            //SoundEngine.PlaySound(style);

            /*
            ArmorShaderData dustShader3 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 1; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(position + new Vector2(48, velocity.X > 0 ? -5 : 5).RotatedBy(velocity.ToRotation()), ModContent.DustType<GlowCircleQuadStar>(), velocity.SafeNormalize(Vector2.UnitX) * 5,
                    Color.Orange, 0.6f, 0.35f, 0f, dustShader3);
                p.fadeIn = 1;
                p.noGravity = true;
            }
            */
            int flashIndex = Projectile.NewProjectile(source, position + new Vector2(56, velocity.X > 0 ? -10 : 10).RotatedBy(velocity.ToRotation()), Vector2.Zero, ModContent.ProjectileType<ErinGunMuzzleFlash>(), 0, 0, Main.myPlayer);
            Projectile mFlash = Main.projectile[flashIndex];
            mFlash.scale = 0.85f;
            mFlash.rotation = velocity.ToRotation();


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(player.Center, Main.npc[i].Center) < 250f && !Main.npc[i].friendly)
                {
                    int Direction = 0;
                    if (player.position.X - Main.npc[i].position.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;


                    // NPC HURTBOX CHECK
                    float AngleToMouse = (Main.MouseWorld - player.Center).ToRotation() % 6.28f;

                    float AngleToNPCTL = (Main.npc[i].position - player.Center).ToRotation() % 6.28f;
                    float AngleToNPCTR = (Main.npc[i].TopRight - player.Center).ToRotation() % 6.28f;
                    float AngleToNPCBL = (Main.npc[i].BottomLeft - player.Center).ToRotation() % 6.28f;
                    float AngleToNPCBR = (Main.npc[i].BottomRight - player.Center).ToRotation() % 6.28f;


                    float AngleDiffTL = Math.Abs(CompareAngle(AngleToNPCTL, AngleToMouse));
                    float AngleDiffTR = Math.Abs(CompareAngle(AngleToNPCTR, AngleToMouse));
                    float AngleDiffBL = Math.Abs(CompareAngle(AngleToNPCBL, AngleToMouse));
                    float AngleDiffBR = Math.Abs(CompareAngle(AngleToNPCBR, AngleToMouse));


                    bool TopLeftCol = (AngleDiffTL > 0 && AngleDiffTL < 1f);
                    bool TopRightCol = (AngleDiffTR > 0 && AngleDiffTR < 1f);
                    bool BottomLeftCol = (AngleDiffBL > 0 && AngleDiffBL < 1f);
                    bool BottomRightCol = (AngleDiffBR > 0 && AngleDiffBR < 1f);

                    //Main.NewText("1 - " + AngleDiff);
                    //Main.NewText("2 - " + MathHelper.ToDegrees(AngleDiff));

                    
                    if (TopLeftCol || TopRightCol || BottomLeftCol || BottomRightCol)
                    {
                        Main.npc[i].StrikeNPC(damage, knockback, Direction);
                        damage = (int)(damage * 0.8f);
                        int a = Projectile.NewProjectile(null, Main.npc[i].Center, Vector2.Zero, ModContent.ProjectileType<ErinImpact>(), 0, 0);
                        Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center, ModContent.DustType<GlowCircleFlare>(), new Vector2(-0.25f, -0.25f), Color.Orange, 2f, 0.4f, 0f, dustShader2);
                        b.fadeIn = 2.5f;
                        b.noLight = true;
                        for (int m = 0; m < 8; m++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(8.5f, 8.5f);
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 54 + Main.rand.NextFloat(-3f, 4f);
                        }
                    }

                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4, 0);
        }

        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }
    }

    public class ErinCircle : ModProjectile
    { 
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 0.3f;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.alpha = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanCutTiles()
        {
            return false; //Makes it so the ring doesn't cut grass and shit
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            //behindProjectiles.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.timeLeft++;
            Projectile.scale = 0.5f;
            Player player = Main.player[Projectile.owner];

            if (player.inventory[player.selectedItem].type != ModContent.ItemType<AntiquePistol>())
            {
                Projectile.Kill();
            }

            Projectile.Center = player.Center;
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + MathHelper.Pi / 2;

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.ForestGreen * 0.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, 0f), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    
    public class ErinAltFireHeldProjectile : ModProjectile
    {
        int timer = 0;
        public int OFFSET = 15; //30
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpToStuff = 0;
        public bool hasReachedDestination = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antique Pistol");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 100;
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
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - (Player.Center)).ToRotation();
            }

            direction = Angle.ToRotationVector2();
            Player.ChangeDir(direction.X > 0 ? 1 : -1);

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

            //Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (timer == 20)
            {
                int flashIndex2 = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center + new Vector2(56, Player.direction == 1 ? -10 : 10).RotatedBy(direction.ToRotation()), Vector2.Zero, ModContent.ProjectileType<ErinGunFireSuck>(), 0, 0, Main.myPlayer);
                Projectile mFlash2 = Main.projectile[flashIndex2];
                mFlash2.rotation = direction.ToRotation() + MathHelper.PiOver2;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_dark_mage_cast_heal_1") with { Pitch = .2f, };
                SoundEngine.PlaySound(style, Projectile.Center);
            }
            else if (timer >= 70)
            {
                if (timer == 70)
                {
                    ShotLogic(Player);

                    Player.GetModPlayer<ErinGunPlayer>().justShotTime = 10;

                    Player.GetModPlayer<AmmoPlayer>().ErinAmmoCount = Player.GetModPlayer<AmmoPlayer>().ErinAmmoCount - 4;

                    //lmao
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + direction * 2, new Vector2(direction.X * -3f, -2), ModContent.GoreType<BulletCasing>());
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + direction * 2, new Vector2(direction.X * -3f, -2), ModContent.GoreType<BulletCasing>());
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + direction * 2, new Vector2(direction.X * -3f, -2), ModContent.GoreType<BulletCasing>());
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + direction * 2, new Vector2(direction.X * -3f, -2), ModContent.GoreType<BulletCasing>());


                    Vector2 muzzlePosition = Player.Center + new Vector2(56, Player.direction == 1 ? -10 : 10).RotatedBy(direction.ToRotation());

                    ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 offsetVel = direction.SafeNormalize(Vector2.UnitX);//.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                        Dust p = GlowDustHelper.DrawGlowDustPerfect(muzzlePosition, ModContent.DustType<GlowCircleQuadStar>(), offsetVel * 3,
                            Main.rand.NextBool() ? Color.Orange : Color.Gold, 1f, 0.5f, 0f, dustShader2);
                        p.fadeIn = 1;
                        p.noGravity = true;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 offsetVel = direction.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f));
                        Dust p = GlowDustHelper.DrawGlowDustPerfect(muzzlePosition, ModContent.DustType<LineGlow>(), offsetVel * 8,
                            Color.OrangeRed, 0.3f, 0.4f, 0f, dustShader2);
                        p.fadeIn = 47 + Main.rand.NextFloat(5, 10);
                    }

                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = 0.7f, PitchVariance = 0.2f }, Projectile.Center);

                    SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.5f, Volume = 1f, MaxInstances = -1, PitchVariance = 0.20f }, Projectile.Center);


                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = -.88f, Volume = 0.7f };
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    Player.GetModPlayer<AeroPlayer>().ScreenShakePower = 10;
                }

                if (hasReachedDestination == false)
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, 2f, 0.24f), 0, 0.7f);
                else
                    lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.03f), 0, 0.4f);

                if (lerpToStuff == 0.7f)
                {
                    hasReachedDestination = true;
                }
            }



            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * -17)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            
            SpriteEffects myEffect = Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, myEffect, 0.0f);


            return false;
        }

        public void ShotLogic(Player myPlayer)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(myPlayer.Center, Main.npc[i].Center) < 250f && !Main.npc[i].friendly)
                {
                    int Direction = 0;
                    if (myPlayer.position.X - Main.npc[i].position.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;


                    // NPC HURTBOX CHECK
                    float AngleToMouse = (Main.MouseWorld - myPlayer.Center).ToRotation() % 6.28f;

                    float AngleToNPCTL = (Main.npc[i].position - myPlayer.Center).ToRotation() % 6.28f;
                    float AngleToNPCTR = (Main.npc[i].TopRight - myPlayer.Center).ToRotation() % 6.28f;
                    float AngleToNPCBL = (Main.npc[i].BottomLeft - myPlayer.Center).ToRotation() % 6.28f;
                    float AngleToNPCBR = (Main.npc[i].BottomRight - myPlayer.Center).ToRotation() % 6.28f;


                    float AngleDiffTL = Math.Abs(CompareAngle(AngleToNPCTL, AngleToMouse));
                    float AngleDiffTR = Math.Abs(CompareAngle(AngleToNPCTR, AngleToMouse));
                    float AngleDiffBL = Math.Abs(CompareAngle(AngleToNPCBL, AngleToMouse));
                    float AngleDiffBR = Math.Abs(CompareAngle(AngleToNPCBR, AngleToMouse));


                    bool TopLeftCol = (AngleDiffTL > 0 && AngleDiffTL < 1f);
                    bool TopRightCol = (AngleDiffTR > 0 && AngleDiffTR < 1f);
                    bool BottomLeftCol = (AngleDiffBL > 0 && AngleDiffBL < 1f);
                    bool BottomRightCol = (AngleDiffBR > 0 && AngleDiffBR < 1f);

                    //Main.NewText("1 - " + AngleDiff);
                    //Main.NewText("2 - " + MathHelper.ToDegrees(AngleDiff));


                    if (TopLeftCol || TopRightCol || BottomLeftCol || BottomRightCol)
                    {


                        SoundStyle stylea = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_flame_breath") with { Volume = .6f, Pitch = .64f, PitchVariance = .22f, };
                        SoundEngine.PlaySound(stylea, Main.npc[i].Center);

                        for (int m = 0; m < 8; m++)
                        {
                            int p = Projectile.NewProjectile(null, Main.npc[i].Center, new Vector2(0.5f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.7f, 2f), ModContent.ProjectileType<FadeExplosion>(), 0, 0);
                            Main.projectile[p].rotation = Main.rand.NextFloat(6.28f);
                            if (Main.projectile[p].ModProjectile is FadeExplosion explo)
                            {
                                explo.color = Color.OrangeRed;
                                explo.size = 0.3f;
                                explo.colorIntensity = 0.6f; //0.5
                            }
                        }

                        Main.npc[i].StrikeNPC(Projectile.damage, 2, Direction);
                        int a = Projectile.NewProjectile(null, Main.npc[i].Center, Vector2.Zero, ModContent.ProjectileType<ErinImpact>(), 0, 0);
                        Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                        Main.projectile[a].scale = 1.25f;

                        ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                        Dust b = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center, ModContent.DustType<GlowCircleFlare>(), new Vector2(-0.25f, -0.25f), Color.Orange, 2f, 0.4f, 0f, dustShader2);
                        b.fadeIn = 2.5f;
                        b.noLight = true;
                        for (int m = 0; m < 8; m++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(16.5f, 16.5f);
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Main.npc[i].Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.OrangeRed, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 56 + Main.rand.NextFloat(-3f, 4f);
                        }
                    }

                }
            }
        }
        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }
    }

    public class ErinImpact : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erin Impact");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 26;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
            Projectile.scale = 1f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Projectile.timeLeft == 200)
            {
                //Hold the first frame for a little bit longer to add more oomf //OMG OOMFIE
                Projectile.frameCounter = -4;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 1)
            {
                if (Projectile.frame == 5)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/ErinGun/ErinImpact").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(5,5).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class ErinGunFireSuck : ModProjectile
    {
        public Vector2 distFromTarget = Vector2.Zero;
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erin Impact");
            Main.projFrames[Projectile.type] = 32;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
            Projectile.scale = 1f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {

            if (timer == 0)
            {
                distFromTarget = Projectile.Center - Main.player[Projectile.owner].Center;
            }

            float Angle = (Main.MouseWorld - (Main.player[Projectile.owner].Center)).ToRotation();

            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(75, Main.player[Projectile.owner].direction == 1 ? -15 : 5).RotatedBy(Angle);
            Projectile.rotation = Angle + MathHelper.PiOver2;

            


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                if (Projectile.frame == 30)
                    Projectile.active = false;

                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/ErinGun/ErinGunFireSuck").Value;

            int frameHeight = Tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, Tex.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + new Vector2(5, 5).RotatedBy(Projectile.rotation), sourceRectangle, Color.Yellow, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class ErinGunMuzzleFlash : MuzzleFlashBase
    {
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.NewText(Main.player[Main.myPlayer].GetModPlayer<ErinGunPlayer>().AmmoCount);

            //Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.1f);

            Texture2D flash = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/AreaPistols/ErinGun/ErinGunMuzzleFlash");
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/AreaPistols/ErinGun/ErinGunMuzzleFlashGlow");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, glow.Frame(1, 1, 0, 0), Color.Gold * fade, Projectile.rotation, glow.Size() / 2, Projectile.scale * fade, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(flash, Projectile.Center - Main.screenPosition, flash.Frame(1, 1, 0, 0), Color.Gold * fade, Projectile.rotation, flash.Size() / 2, Projectile.scale * fade, SpriteEffects.None, 0f);

            //Projectile.localAI[0] -= 0.01f;

            return false;
        }
    }

    public class ErinGunPlayer : ModPlayer
    {
        //Want to keep ammo in a player so it is synced between instances of a weapon
        //I don't want people to just have 20 of the weapon to overcome its downside

        public int MAX_AMMO = 12; //not const cuz testing currently
        public int AmmoCount = 12;
        public String ammoString = "";

        //public String totalAmmoString = "";
        public String preAmmoString = "";
        public String postAmmoString = "";

        public float justShotTime = 0f;

        public float reloadProgress = 0f;

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
            AmmoCount = MAX_AMMO;
        }

        public override void PostUpdateMiscEffects()
        {
            Update();
        }

        private void Update()
        {
            justShotTime = MathHelper.Clamp(justShotTime - 1, 0, 100);

            //preAmmoString = "" + AmmoCount + "";
            //postAmmoString = "18";
            //if the player is not holding the weapon, inc
            ammoString = AmmoCount + "/" + MAX_AMMO;
        }
    }

    public class ErinGunAmmoDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HeldItem);
        }
        public float timeJustHeld = 0f;
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;


            if (Player.HeldItem.type != ModContent.ItemType<AntiquePistol>())
            {
                timeJustHeld = 15f;
                return;
            }

            float currentAmmo = Player.GetModPlayer<AmmoPlayer>().ErinAmmoCount;
            float halfCurrentBullets = currentAmmo / 2f;

            timeJustHeld = Math.Clamp(timeJustHeld - 1, 0, 100);

            //Ensures that this isn't drawn multiple times if the player has an afterimage
            if (drawInfo.shadow == 0f)
            {
                Vector2 drawPos = Player.MountedCenter - Main.screenPosition;// - new Vector2(0, 10 - Player.gfxOffY);

                Texture2D backdropTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoUIBackdrop").Value;
                Texture2D borderTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/BulletUIBorder").Value;

                Texture2D TextTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoUIText").Value;

                /*
                Because we can't use DrawString in a draw layer, we have to get creative
                TextTex contains a spritesheet containing letters
                frames 1-9 are 1-9, frame 10 is 0, and frame 11 is /
                */
                //int maxAmmo = Player.GetModPlayer<ErinGunPlayer>().MAX_AMMO;
                //int currentAmmo = Player.GetModPlayer<ErinGunPlayer>().AmmoCount;

                //Draw tex to make bullets stand out more
                DrawData backdrop = new DrawData(backdropTex, new Vector2((int)drawPos.X, (int)drawPos.Y - 40), null,
                            Color.Black * 0.3f, 0f, backdropTex.Size() / 2, new Vector2(0.022f * currentAmmo, 0.15f), SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(backdrop);

                Texture2D BulletTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/AmmoUIBullet").Value;
                float shotReactTime = Player.GetModPlayer<ErinGunPlayer>().justShotTime;

                //Draw bullets
                for (float j = (-1 * halfCurrentBullets) + 0.5f; j < halfCurrentBullets; j++)
                {
                    DrawData preLetter = new DrawData(BulletTex, new Vector2((int)drawPos.X, (int)drawPos.Y) + new Vector2(8f * j, -40),
                        BulletTex.Frame(1,1,0,0), Color.Lerp(Color.Yellow, Color.Black, shotReactTime / 20), 0f, BulletTex.Size() / 2, new Vector2(0.76f - (timeJustHeld * 0.03f), 0.76f - (shotReactTime * 0.03f)), SpriteEffects.None, 0);
                    drawInfo.DrawDataCache.Add(preLetter);
                }

                /*
                if (currentAmmo > 0)
                {
                    DrawData leftBorder = new DrawData(borderTex, new Vector2((int)drawPos.X, (int)drawPos.Y) + new Vector2(8f * (-1 * halfCurrentBullets) - 1, -40),
                        borderTex.Frame(1, 1, 0, 0), Color.Black, 0f, borderTex.Size() / 2, new Vector2(0.36f - (timeJustHeld * 0.03f), 0.66f), SpriteEffects.None, 0);
                    drawInfo.DrawDataCache.Add(leftBorder);

                    DrawData rightBorder = new DrawData(borderTex, new Vector2((int)drawPos.X, (int)drawPos.Y) + new Vector2(8f * halfCurrentBullets + 1.5f, -40),
                        borderTex.Frame(1, 1, 0, 0), Color.Black, 0f, borderTex.Size() / 2, new Vector2(0.36f - (timeJustHeld * 0.03f), 0.66f), SpriteEffects.FlipHorizontally, 0);
                    drawInfo.DrawDataCache.Add(rightBorder);
                }
                */



                /*
                String preAmmo = Player.GetModPlayer<ErinGunPlayer>().preAmmoString;
                String postAmmo = Player.GetModPlayer<ErinGunPlayer>().postAmmoString;
                //Left ammoCount
                for (int i = 0; i < preAmmo.Length; i++)
                {
                    String character = preAmmo.Substring(i, preAmmo.Length - 1);
                    int frameToUse = StringToFrame(character);

                    DrawData preLetter = new DrawData(TextTex, new Vector2((int)drawPos.X + (20 * i) - 50, (int)drawPos.Y), 
                        new Rectangle(0, TextTex.Height / 11 * frameToUse, TextTex.Width, TextTex.Height / 11),
                        Color.White, 0f, TextTex.Size() / 2, 1f, SpriteEffects.None, 0);
                    drawInfo.DrawDataCache.Add(preLetter);
                }

                //Right ammoCount
                for (int i = 0; i < postAmmo.Length; i++)
                {
                    String character = postAmmo.Substring(i, postAmmo.Length - 1);
                    int frameToUse = StringToFrame(character);

                    DrawData postLetter = new DrawData(TextTex, new Vector2((int)drawPos.X + (20 * i) + 30, (int)drawPos.Y),
                        new Rectangle(0, TextTex.Height / 11 * frameToUse, TextTex.Width, TextTex.Height / 11),
                        Color.White, 0f, TextTex.Size() / 2, 1f, SpriteEffects.None, 0);
                    drawInfo.DrawDataCache.Add(postLetter);
                }
                */

                /*
                DrawData slash = new DrawData(TextTex, new Vector2((int)drawPos.X, (int)drawPos.Y),
                        new Rectangle(0, TextTex.Height / 11 * 11, TextTex.Width, TextTex.Height / 11),
                        Color.White, 0f, TextTex.Size() / 2, 1f, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(slash);
                */


            }

            /*
            DrawData backdrop = new DrawData();

            var value = new DrawData(
                        barTex,
                        new Vector2((int)drawPos.X, (int)drawPos.Y),
                        null,
                        Lighting.GetColor((int)(drawPos.X + Main.screenPosition.X) / 16, (int)(drawPos.Y + Main.screenPosition.Y) / 16),
                        0f,
                        barTex.Size() / 2,
                        1,
                        SpriteEffects.None,
                        0
                    );
            drawInfo.DrawDataCache.Add(value);

            var value2 = new DrawData(
                        glowTex,
                        new Vector2((int)drawPos.X, (int)drawPos.Y) - new Vector2(0, 1),
                        new Rectangle(0, 0, (int)(glowTex.Width * (Player.GetModPlayer<JetwelderPlayer>().scrap / 20f)), glowTex.Height),
                        Color.White,
                        0f,
                        glowTex.Size() / 2,
                        1,
                        SpriteEffects.None,
                        0
                    );
            drawInfo.DrawDataCache.Add(value2);
            */
        }

        public int StringToFrame(String input)
        {
            if (input.Equals(""))
                return 11;

            if (input.Equals("/"))
                return 11;

            int inputInt = int.Parse(input);


            if (inputInt >= 1 && inputInt <= 9)
                return inputInt - 1;
            else if (inputInt == 0)
                return 10;

            Main.NewText("StringToFrame bad input");
            return -1;


        }
    }

    public class BulletCasing : ModGore
    {
        public override string Texture => "AerovelenceMod/Content/Gores/BulletShell";
        public override bool Update(Gore gore)
        {
            gore.alpha += 8;

            if (gore.alpha >= 250)
                gore.active = false;
            return base.Update(gore);
        }
    }

    public class ReloadProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ErinReloadProj");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0f;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        int timer = 0;

        float lerp1progress = 0f;
        float lerp2progress = 0f;

        public override void AI()
        {

            //Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(0,-80);

            /*
            if (timer < 75)
                Projectile.scale = Math.Clamp(MathHelper.Lerp(0, 1.1f, lerp1progress), 0f, 1f);
            else
                Projectile.scale = Math.Clamp(MathHelper.Lerp(1, -0.5f, lerp2progress), 0, 1f);

            lerp1progress = Math.Clamp(lerp1progress + 0.06f, 0, 1);

            if (timer >= 75)
                lerp2progress = Math.Clamp(lerp2progress + 0.06f, 0, 1);
            */
            
            if (timer < 75)
            {
                Projectile.scale = Math.Clamp(MathHelper.Lerp(0, 1.1f, lerp1progress), 0f, 1f);

                if (timer < 60)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center + new Vector2(0, -55), lerp1progress);
                    lerp1progress = Math.Clamp(lerp1progress + 0.03f, 0, 1);

                }
                else
                {
                    float distFromPlayer = MathHelper.Lerp(-55, -75, lerp1progress - 1);
                    Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(0, distFromPlayer);//Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center + new Vector2(0, -70), lerp1progress * 0.25f);

                    lerp1progress = lerp1progress + 0.1f;

                }

                /*
                if (timer < 20)
                    lerp1progress = Math.Clamp(lerp1progress + 0.06f, 0, 1);
                else
                    lerp1progress = Math.Clamp(lerp1progress + 0.06f, 0, 1);
                */

                //lerp1progress = Math.Clamp(lerp1progress + 0.06f, 0, 1);
            }
            else if (timer >= 75)
            {
                Projectile.scale = Math.Clamp(MathHelper.Lerp(1, -0.5f, lerp2progress * 0.75f), 0, 1f);

                float distFromPlayer = MathHelper.Lerp(-75, 0, lerp2progress);
                Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(0, distFromPlayer);//Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, lerp2progress);

                lerp2progress = Math.Clamp(lerp2progress + 0.1f, 0, 1);

            }
            

            if (lerp2progress == 1)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Research_3") with { Pitch = 1f, Volume = 0.4f }; 
                SoundEngine.PlaySound(style, Projectile.Center);

                Player player = Main.player[Projectile.owner];
                player.GetModPlayer<AmmoPlayer>().ErinAmmoCount = player.GetModPlayer<AmmoPlayer>().ERIN_GUN_MAX_AMMO;

                player.GetModPlayer<ErinGunPlayer>().justShotTime = 20;


                for (int i = 0; i < 1; i++)
                {
                    int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HollowPulse>(), 0, 0, Main.myPlayer);
                    if (Main.projectile[a].ModProjectile is HollowPulse pulse)
                    {
                        pulse.color = Color.White * 0.85f;
                        pulse.oval = false;
                        pulse.size = 2;
                    }
                }

                Projectile.active = false;
            }

            
            Projectile.rotation += 0.2f;// * lerp1progress;
            timer++;
        }

        //start 0,0 scale
        //move to 



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/ErinGun/ReloadSymbol").Value;

            Vector2 scale = new Vector2(Projectile.scale, Projectile.scale);

            float OldPosDifference = (Projectile.Center - Projectile.oldPos[0]).Length();

            //if (timer < 30 || timer >= 90)
            //scale = new Vector2(Projectile.scale, Projectile.scale * 0.5f);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(Tex, new Vector2((int)Projectile.Center.X, (int)Projectile.Center.Y) - Main.screenPosition, Tex.Frame(1,1,0,0), Color.White, Projectile.rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}

