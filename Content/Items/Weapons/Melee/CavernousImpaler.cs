using System;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.damage = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 55;
            item.useTime = 24;
            item.shootSpeed = 1.8f;
            item.knockBack = 5f;
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.melee = true;
            item.noMelee = true; 
            item.noUseGraphic = true;
            item.autoReuse = true;

            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<CavernousImpalerProjectile>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
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
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;

            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }

        // Properties / Methods: PascalCase
        // Private fields: _camelCase
        // Public / internal fields: camelCase
        public float MoveFactor
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player projOwner = Main.player[projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projectile.direction = projOwner.direction;
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            projectile.position.X = ownerMountedCenter.X - (float)(projectile.width / 2);
            projectile.position.Y = ownerMountedCenter.Y - projectile.height / 2;
            if (!projOwner.frozen)
            {
                if (MoveFactor == 0f)
                {
                    MoveFactor = 3f;
                    projectile.netUpdate = true;
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
            projectile.position += projectile.velocity * MoveFactor;
            if (projOwner.itemAnimation == 0)
            {
                projectile.Kill();
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(90f);
            }

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.height, projectile.width, ModContent.DustType<Sparkle>(),
                    projectile.velocity.X * .2f, projectile.velocity.Y * .2f * projectile.alpha, 200, Scale: 1.2f);
                dust.velocity += projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.height, projectile.width, ModContent.DustType<Sparkle>(),
                    0, 0, 254, Scale: 0.3f);
                dust.velocity += projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }
            if (projOwner.itemAnimation == projOwner.itemAnimationMax - 1)
                Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 2f, projectile.velocity.Y * 2, ModContent.ProjectileType<CavernousImpalerProjectile2>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        }
    }

    public class CavernousImpalerProjectile2 : ModProjectile
    {
        public bool e;
        public float rot = 0f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.melee = true;
            projectile.timeLeft = 120;
            projectile.light = 0.5f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 132, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10);
            return true;
        }
        public override void AI()
        {
            i++;
            projectile.rotation += rot;
            projectile.scale *= 1.005f;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 132);
            }
            projectile.alpha += 2;
            rot *= 0.99f;
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = projectile.velocity.X;
                projectile.ai[1] = projectile.velocity.Y;
            }
            if (Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y) > 2.0)
            {
                projectile.velocity *= 0.99f;
            }
            for (int num437 = 0; num437 < 1000; num437++)
            {
                if (num437 != projectile.whoAmI && Main.projectile[num437].active && Main.projectile[num437].owner == projectile.owner && Main.projectile[num437].type == projectile.type && projectile.timeLeft > Main.projectile[num437].timeLeft && Main.projectile[num437].timeLeft > 30)
                {
                    projectile.alpha += 10;
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
                float num445 = Math.Abs(projectile.position.X + projectile.width / 2 - num443) + Math.Abs(projectile.position.Y + projectile.height / 2 - num444);
                if (num445 < num439 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num442].Center, 1, 1))
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
            if (projectile.timeLeft < 30)
            {
                flag14 = false;
            }
            if (flag14)
            {
                int num446 = Main.rand.Next(num438);
                num446 = array[num446];
                num440 = Main.npc[num446].position.X + Main.npc[num446].width / 2;
                num441 = Main.npc[num446].position.Y + Main.npc[num446].height / 2;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 8f)
                {
                    projectile.localAI[0] = 0f;
                    float num447 = 6f;
                    Vector2 vector31 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    vector31 += projectile.velocity * 4f;
                    float num448 = num440 - vector31.X;
                    float num449 = num441 - vector31.Y;
                    float num450 = (float)Math.Sqrt(num448 * num448 + num449 * num449);
                    float num451 = num450;
                    num450 = num447 / num450;
                    num448 *= num450;
                    num449 *= num450;
                    Projectile.NewProjectile(vector31.X, vector31.Y, num448, num449, ModContent.ProjectileType<CavernousImpalerProjectile3>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
        }
    }

    public class CavernousImpalerProjectile3 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.damage = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 100;
        }
        public override void AI()
        {
            for (int num452 = 0; num452 < 4; num452++)
            {
                Vector2 position = projectile.position;
                position -= projectile.velocity * (num452 * 0.25f);
                projectile.alpha = 255;
                int num453 = Dust.NewDust(position, 1, 1, 160);
                Main.dust[num453].position = position;
                Main.dust[num453].position.X += projectile.width / 2;
                Main.dust[num453].position.Y += projectile.height / 2;
                Main.dust[num453].scale = Main.rand.Next(70, 110) * 0.013f;
                Dust dust77 = Main.dust[num453];
                Dust dust2 = dust77;
                dust2.velocity *= 0.2f;
            }
            return;
        }
    }
}