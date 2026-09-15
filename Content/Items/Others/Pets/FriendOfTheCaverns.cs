using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Pets
{
    public class FriendOfTheCaverns : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item44;
            Item.noMelee = true;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<FriendOfTheCavernsBuff>();
            Item.shoot = ModContent.ProjectileType<FriendOfTheCavernsPet>();
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }

    public class FriendOfTheCavernsBuff : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Pets/FriendOfTheCaverns";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<FriendOfTheCavernsPet>()] <= 0)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<FriendOfTheCavernsPet>(), 0, 0f, player.whoAmI);
        }
    }

    public class FriendOfTheCavernsPet : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Pets/FriendOfTheCaverns";

        private Vector2 eyeOffset;
        private Vector2 desiredEyeOffset;
        private int eyeTimer;
        private int blinkTimer = 90;
        private int blinkTime;
        private int curiousTimer;
        private bool ownerWasMoving;
        private Vector2 smoothedGoal;
        private bool smoothedGoalReady;
        private float smoothedRotation;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 1, 6)
                .WithOffset(-2f, -10f)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 2;
            Projectile.manualDirectionChange = true;
        }

        public override bool? CanDamage() => false;

        public override void DrawBehind(int index, System.Collections.Generic.List<int> behindNPCsAndTiles, System.Collections.Generic.List<int> behindNPCs, System.Collections.Generic.List<int> behindProjectiles, System.Collections.Generic.List<int> overPlayers, System.Collections.Generic.List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.Kill();
                return;
            }

            if (player.dead)
                player.ClearBuff(ModContent.BuffType<FriendOfTheCavernsBuff>());
            if (player.HasBuff(ModContent.BuffType<FriendOfTheCavernsBuff>()))
                Projectile.timeLeft = 2;

            Projectile.tileCollide = false;

            bool ownerMoving = player.velocity.LengthSquared() > 0.35f;
            if (!ownerMoving && ownerWasMoving)
                curiousTimer = 34;
            ownerWasMoving = ownerMoving;
            if (curiousTimer > 0)
                curiousTimer--;

            float lifeTime = Main.GameUpdateCount + Projectile.identity * 13f;
            float sway = (float)System.Math.Sin(lifeTime * 0.045f);
            float bob = (float)System.Math.Sin(lifeTime * 0.07f + 0.8f);
            float curiousProgress = curiousTimer / 34f;
            float curiousPulse = curiousTimer > 0 ? (float)System.Math.Sin((1f - curiousProgress) * MathHelper.Pi) : 0f;
            Vector2 anchor = player.Center + new Vector2(-player.direction * 30f, -42f + player.gfxOffY);
            Vector2 drift = new Vector2(sway * 10f, bob * 5.5f);
            drift += new Vector2(player.direction * curiousPulse * 10f, -curiousPulse * 3f);
            if (ownerMoving)
                drift += player.velocity.SafeNormalize(Vector2.Zero) * 8f;
            Vector2 goal = anchor + drift;

            if (!smoothedGoalReady)
            {
                smoothedGoal = goal;
                smoothedGoalReady = true;
            }

            float goalBlend = ownerMoving ? 0.16f : 0.105f;
            smoothedGoal = Vector2.Lerp(smoothedGoal, goal, goalBlend);

            Vector2 toGoal = smoothedGoal - Projectile.Center;
            float distance = toGoal.Length();

            if (distance > 900f)
            {
                Projectile.Center = smoothedGoal;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            else
            {
                float maxSpeed = MathHelper.Lerp(7f, 19f, MathHelper.Clamp(distance / 180f, 0f, 1f));
                Vector2 desiredVelocity = toGoal * 0.12f;
                if (desiredVelocity.LengthSquared() > maxSpeed * maxSpeed)
                    desiredVelocity = desiredVelocity.SafeNormalize(Vector2.Zero) * maxSpeed;

                float velocityBlend = ownerMoving ? 0.13f : 0.09f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, velocityBlend);

                if (distance < 3f)
                    Projectile.velocity *= 0.965f;
            }

            if (Projectile.velocity.X > 0.06f)
                Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < -0.06f)
                Projectile.spriteDirection = -1;
            else
                Projectile.spriteDirection = player.direction;

            float hangingSway = (float)System.Math.Sin(lifeTime * 0.052f) * 0.055f;
            float movementTilt = MathHelper.Clamp(Projectile.velocity.X * 0.022f, -0.14f, 0.14f);
            float targetRotation = movementTilt + hangingSway - player.direction * curiousPulse * 0.045f;
            smoothedRotation = Utils.AngleLerp(smoothedRotation, targetRotation, 0.11f);
            Projectile.rotation = smoothedRotation;

            UpdateEyes();
            UpdateBlink();

            float pulse = 0.9f + (float)System.Math.Sin(Main.GameUpdateCount * 0.08f) * 0.1f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.25f, 0.95f, 1f) * 0.75f * pulse);
            if (!Main.dedServ && Main.rand.NextBool(18))
            {
                Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.9f, -0.25f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-5f, 5f), 8f), DustID.GemDiamond, dustVelocity, 120, new Color(170, 210, 255), Main.rand.NextFloat(0.45f, 0.8f));
                dust.noGravity = true;
                dust.fadeIn = 0.8f;
            }
        }

        private void UpdateEyes()
        {
            Vector2 motionLook = Projectile.velocity.LengthSquared() > 0.08f ? Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f : Vector2.Zero;
            if (--eyeTimer <= 0)
            {
                Vector2 randomLook = Main.rand.NextVector2Circular(2f, 2f);
                desiredEyeOffset = Vector2.Clamp((motionLook * 0.65f) + (randomLook * 0.7f), new Vector2(-2f, -2f), new Vector2(2f, 2f));
                eyeTimer = Main.rand.Next(16, 46);
            }

            Vector2 preferred = Vector2.Clamp((desiredEyeOffset * 0.55f) + (motionLook * 0.75f), new Vector2(-2f, -2f), new Vector2(2f, 2f));
            eyeOffset = Vector2.Lerp(eyeOffset, preferred, 0.18f);
            eyeOffset.X = MathHelper.Clamp(eyeOffset.X, -2f, 2f);
            eyeOffset.Y = MathHelper.Clamp(eyeOffset.Y, -2f, 2f);
        }


        private void UpdateBlink()
        {
            if (blinkTime > 0)
            {
                blinkTime--;
                return;
            }

            if (--blinkTimer <= 0)
            {
                blinkTime = Main.rand.NextBool(5) ? 10 : 6;
                blinkTimer = Main.rand.Next(85, 190);
            }
        }

        private float EyeScaleY()
        {
            if (blinkTime <= 0)
                return 1f;

            int total = blinkTime > 6 ? 10 : 6;
            float progress = 1f - blinkTime / (float)total;
            float close = (float)System.Math.Sin(progress * MathHelper.Pi);
            return MathHelper.Lerp(1f, 0.08f, MathHelper.Clamp(close, 0f, 1f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D body = TextureAssets.Projectile[Type].Value;
            Texture2D eyes = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Others/Pets/FriendOfTheCavernsEyes").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = body.Size() * 0.5f;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float eyeScaleY = EyeScaleY();
            Vector2 eyeScale = new Vector2(Projectile.scale, Projectile.scale * eyeScaleY);
            Main.EntitySpriteDraw(body, drawPos, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects);
            Main.EntitySpriteDraw(eyes, drawPos + eyeOffset, null, new Color(185, 225, 255, 0) * 0.35f, Projectile.rotation, eyes.Size() * 0.5f, eyeScale * 1.16f, effects);
            Main.EntitySpriteDraw(eyes, drawPos + eyeOffset, null, Color.White, Projectile.rotation, eyes.Size() * 0.5f, eyeScale, effects);
            return false;
        }
    }
}