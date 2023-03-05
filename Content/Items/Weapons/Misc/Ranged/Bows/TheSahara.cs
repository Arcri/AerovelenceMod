using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Bows
{
	public class TheSahara : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sahara");
			Tooltip.SetDefault("TODO");
		}

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.rare = ItemRarityID.Orange;

			Item.width = 58;
			Item.height = 20;

			Item.useAnimation = 20;
			Item.useTime = 20;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 15f;
			Item.knockBack = 2f;

			Item.DamageType = DamageClass.Ranged;

			Item.autoReuse = true;
			Item.noMelee = true;

			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.WoodenArrowFriendly;

			Item.useAmmo = AmmoID.Arrow;
			Item.channel = true;
			Item.noUseGraphic = true;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(8, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile proj2 = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ModContent.ProjectileType<TheSaharaHeldProj>(), damage, 0, player.whoAmI);

			if (proj2.ModProjectile is TheSaharaHeldProj wb)
            {
				wb.projToShootID = type;
            }

			return false;
        }

    }

    public class TheSaharaHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sahara");
        }

        public int OFFSET = 10; //15
        public ref float Angle => ref Projectile.ai[1];

        public Vector2 direction = Vector2.Zero;

        public float lerpage = 0.82f;

        public float lerpToStuff = 0;

        int timer = 0;

        int timeBeforeKill = 0; //once the player lets go, this will start ticking up and the proj will die after a certain time

        float percentDrawnBack = 0;

        public bool runOnceCharge = true;

        public override void SetDefaults()
        {
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

        public override bool? CanCutTiles()
        {
            return false;
        }

        public int projToShootID = ProjectileID.WoodenArrowFriendly;

        public override void AI()
        {
            Player Player = Main.player[Projectile.owner];

            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Player.itemTime = 2; // Set Item time to 2 frames while we are used
            Player.itemAnimation = 2; // Set Item animation time to 2 frames while we are used

            if (Player.channel)
            {
                //Main.NewText(Player.direction);
                if (Projectile.owner == Main.myPlayer)
                {
                    Angle = (Main.MouseWorld - (Player.MountedCenter)).ToRotation();
                }

                direction = Angle.ToRotationVector2();
                Player.ChangeDir(direction.X > 0 ? 1 : -1);

                percentDrawnBack = MathHelper.Clamp(percentDrawnBack + 0.03f, 0f, 1f);
                if (timer < 10)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                else if (timer < 20)
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
                else
                    Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (percentDrawnBack >= 1f && runOnceCharge)
                {
                    runOnceCharge = false;
                }
            }
            else
            {
                Vector2 projPosition = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY).Floor();


                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, Projectile.rotation - MathHelper.PiOver2);

                if (timeBeforeKill == 0)
                {
                    float vel = MathHelper.Clamp(20 * percentDrawnBack, 6, 20);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, direction.SafeNormalize(Vector2.UnitX) * vel, projToShootID, Projectile.damage / 2 + (int)(Projectile.damage * percentDrawnBack), 0, Player.whoAmI);



                    for (int j = 0; j < 20; j++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * vel, ModContent.DustType<StillDust>(),
                            Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(0.5f, 1.2f), newColor: Color.OrangeRed);

                        //Dust dust1 = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + direction.SafeNormalize(Vector2.UnitX) * vel, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 3),
                        //Color.Orange, Main.rand.NextFloat(0.3f, 0.5f), 0.6f, 0f, dustShader2);
                        //dust1.velocity *= 1f;

                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 0.75f;
                        Dust dust1 = Dust.NewDustPerfect(Projectile.Center + randomStart + direction.SafeNormalize(Vector2.UnitX) * vel, DustID.Torch, randomStart * Main.rand.NextFloat(0.65f, 1.35f));
                    }

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_5") with { Pitch = -.53f, };
                    SoundEngine.PlaySound(style2, Player.Center);

                    SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .24f, Volume = 0.5f };
                    SoundEngine.PlaySound(style3, Player.Center);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_javelin_throwers_attack_1") with { Pitch = .45f, PitchVariance = 0.1f, Volume = 0.5f + (0.5f * percentDrawnBack) };
                    SoundEngine.PlaySound(style, Player.Center);
                }

                if (timeBeforeKill >= 40)
                    Projectile.active = false;
                timeBeforeKill++;
            }

            //lerpToStuff = Math.Clamp(MathHelper.Lerp(lerpToStuff, -0.2f, 0.06f), 0, 0.4f);


            direction = Angle.ToRotationVector2().RotatedBy(lerpToStuff * Player.direction * -1f);
            Projectile.Center = Player.MountedCenter + (direction * OFFSET);
            Projectile.velocity = Vector2.Zero;
            Player.itemRotation = direction.ToRotation();

            if (Player.direction != 1)
                Player.itemRotation -= 3.14f;

            Player.itemRotation = MathHelper.WrapAngle(Player.itemRotation);

            Player.heldProj = Projectile.whoAmI;

            Projectile.rotation = direction.ToRotation();

            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            timer++;
        }

        float starScaleMultiplier = 0;
        float outerStarScale = 0;
        float innerStarScale = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Bows/TheSahara").Value;
            int height1 = texture.Height;
            Vector2 origin = new Vector2((float)texture.Width / 2f, (float)height1 / 2f);
            Vector2 position = (Projectile.position - (0.5f * (direction * OFFSET * -1)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Player.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation(), origin, Projectile.scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, direction.ToRotation() - 3.14f, origin, Projectile.scale, effects1, 0.0f);
            }


            //Arrow Drawing
            Texture2D arrowTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Ranged/Bows/Wooden_Arrow").Value;
            Vector2 origin2 = new Vector2((float)arrowTexture.Width / 2f, (float)arrowTexture.Height / 2f);
            Vector2 position2 = (Projectile.position - (0.5f * (direction * OFFSET * -1.5f)) + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Vector2 chargeOffset = new Vector2(-5 * percentDrawnBack, 0).RotatedBy(direction.ToRotation());

            Vector2 lineOffsetRot1 = new Vector2(0f, -15f).RotatedBy(Projectile.rotation);
            Vector2 lineOffsetRot2 = new Vector2(0f, 15f).RotatedBy(Projectile.rotation);

            Vector2 arrowButtPos = position2 + chargeOffset + new Vector2(0, -13).RotatedBy(direction.ToRotation() - MathHelper.PiOver2);
            if (Player.channel)
            {
                //Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
                //Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, arrowButtPos + Main.screenPosition, lightColor, lightColor, 1.5f);
            }
            else
            {
                //Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot1, Projectile.Center + lineOffsetRot2, lightColor, lightColor, 1.5f);

                ////Utils.DrawLine(Main.spriteBatch, Projectile.Center + lineOffsetRot2, Projectile.Center + lineOffsetRot1, lightColor, lightColor, 1.5f);
            }


            if (Player.channel)
                Main.spriteBatch.Draw(arrowTexture, position2 + chargeOffset, null, lightColor, direction.ToRotation() - MathHelper.PiOver2, origin2, 1f, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            return false;
        }

    }
}