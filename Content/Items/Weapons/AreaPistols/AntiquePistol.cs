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

namespace AerovelenceMod.Content.Items.Weapons.AreaPistols
{
    public class AntiquePistol : ModItem
    {
        Projectile currentCircle = Main.projectile[0];
        bool shouldSpawn = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antique Pistol");
            Tooltip.SetDefault("'Property of Erin'");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.DD2_BallistaTowerShot with { Volume = 0.9f, Pitch = 0.4f, PitchVariance = 0.15f };
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = 27; //14
            Item.useAnimation = 27; //14
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BeeArrow;
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {

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

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ErinCircle>()] < 1)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<ErinCircle>(), 0, 0, player.whoAmI);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
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

            Texture2D Tex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/AreaPistols/ErinImpact").Value;

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
}