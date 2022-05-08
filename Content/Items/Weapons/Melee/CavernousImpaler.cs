using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class CavernousImpaler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cavernous Impaler");
            Tooltip.SetDefault("Fires a crystal that explodes on impact");
        }
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 55;
            Item.useTime = 24;
            Item.shootSpeed = 1.8f;
            Item.knockBack = 5f;
            Item.width = 32;
            Item.height = 32;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true; 
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<CavernousImpalerProjectile>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }

    public class CavernousImpalerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cavernous Impaler");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 19;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;

            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        // Properties / Methods: PascalCase
        // Private fields: _camelCase
        // Public / internal fields: camelCase
        public float MoveFactor
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.direction = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - Projectile.height / 2;
            if (!projOwner.frozen)
            {
                if (MoveFactor == 0f)
                {
                    MoveFactor = 3f;
                    Projectile.netUpdate = true;
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                {
                    MoveFactor -= 2.4f;
                }
                else
                {
                    MoveFactor += 2.1f;
                }
            }
            Projectile.position += Projectile.velocity * MoveFactor;
            if (projOwner.itemAnimation == 0)
            {
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.ToRadians(90f);
            }

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(),
                    Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f * Projectile.alpha, 200, Scale: 1.2f);
                dust.velocity += Projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(),
                    0, 0, 254, Scale: 0.3f);
                dust.velocity += Projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }
            if (projOwner.itemAnimation == projOwner.itemAnimationMax - 1)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + Projectile.velocity.X, Projectile.Center.Y + Projectile.velocity.Y, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2, ModContent.ProjectileType<CavernousImpalerProjectile2>(), Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
        }
    }

    public class CavernousImpalerProjectile2 : ModProjectile
    {
        public bool e;
        public float rot = 0f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 120;
            Projectile.light = 0.5f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            SoundEngine.PlaySound(SoundID.Item10);
            return true;
        }
        public override void AI()
        {
            i++;
            Projectile.rotation += rot;
            Projectile.scale *= 1.005f;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
            }
            Projectile.alpha += 2;
            rot *= 0.99f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = Projectile.velocity.X;
                Projectile.ai[1] = Projectile.velocity.Y;
            }
            if (Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y) > 2.0)
            {
                Projectile.velocity *= 0.99f;
            }
            for (int num437 = 0; num437 < 1000; num437++)
            {
                if (num437 != Projectile.whoAmI && Main.projectile[num437].active && Main.projectile[num437].owner == Projectile.owner && Main.projectile[num437].type == Projectile.type && Projectile.timeLeft > Main.projectile[num437].timeLeft && Main.projectile[num437].timeLeft > 30)
                {
                    Projectile.alpha += 10;
                    Main.projectile[num437].timeLeft = 30;
                }
            }
            int[] array = new int[20];
            int num438 = 0;
            float num439 = 300f;
            bool flag14 = false;
            float num440 = 0f;
            float num441 = 0f;
            for (int num442 = 0; num442 < 200; num442++)
            {
                if (!Main.npc[num442].CanBeChasedBy(this))
                {
                    continue;
                }
                float num443 = Main.npc[num442].position.X + Main.npc[num442].width / 2;
                float num444 = Main.npc[num442].position.Y + Main.npc[num442].height / 2;
                float num445 = Math.Abs(Projectile.position.X + Projectile.width / 2 - num443) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - num444);
                if (num445 < num439 && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
                {
                    if (num438 < 20)
                    {
                        array[num438] = num442;
                        num438++;
                        num440 = num443;
                        num441 = num444;
                    }
                    flag14 = true;
                }
            }
            if (Projectile.timeLeft < 30)
            {
                flag14 = false;
            }
            if (flag14)
            {
                int num446 = Main.rand.Next(num438);
                num446 = array[num446];
                num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
                num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] > 8f)
                {
                    Projectile.localAI[0] = 0f;
                    float num447 = 6f;
                    Vector2 vector31 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    vector31 += Projectile.velocity * 4f;
                    float num448 = num440 - vector31.X;
                    float num449 = num441 - vector31.Y;
                    float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
                    float num451 = num450;
                    num450 = num447 / num450;
                    num448 *= num450;
                    num449 *= num450;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<CavernousImpalerProjectile3>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }

    public class CavernousImpalerProjectile3 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.damage = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 100;
        }
        public override void AI()
        {
            for (int num452 = 0; num452 < 4; num452++)
            {
                Vector2 position = Projectile.position;
                position -= Projectile.velocity * (num452 * 0.25f);
                Projectile.alpha = 255;
                int num453 = Dust.NewDust(position, 1, 1, 160);
                Main.dust[num453].position = position;
                Main.dust[num453].position.X += Projectile.width / 2;
                Main.dust[num453].position.Y += Projectile.height / 2;
                Main.dust[num453].scale = Main.rand.Next(70, 110) * 0.013f;
                Dust dust77 = Main.dust[num453];
                Dust dust2 = dust77;
                dust2.velocity *= 0.2f;
            }
            return;
        }
    }
}