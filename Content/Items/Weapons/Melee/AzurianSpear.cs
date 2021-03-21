using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class AzurianSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Azurian Spear");
            Tooltip.SetDefault("Fires a beam of light");
        }
        public override void SetDefaults()
        {
            item.damage = 22;
            item.crit = 2;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 30;
            item.useTime = 30;
            item.shootSpeed = 1.4f;
            item.knockBack = 6.5f;
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.melee = true;
            item.noMelee = true; 
            item.noUseGraphic = true;
            item.autoReuse = false;

            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<AzurianSpearProj>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }

    public class AzurianSpearProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Azurian Spear");
        }
        public int counter = 0;
        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;

            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }



        public float movementFactor
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
            projectile.position.X = ownerMountedCenter.X - projectile.width / 2;
            projectile.position.Y = ownerMountedCenter.Y - projectile.height / 2;
            if (!projOwner.frozen)
            {
                if (movementFactor == 0f)
                {
                    movementFactor = 3f;
                    projectile.netUpdate = true;
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                {
                    movementFactor -= 2.4f;
                }
                else
                {
                    movementFactor += 2.1f;
                }
            }
            projectile.position += projectile.velocity * movementFactor;
            if (projOwner.itemAnimation == 0)
            {
                projectile.Kill();
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(90f);
            }
            if (projOwner.itemAnimation == projOwner.itemAnimationMax - 2)
            {
                Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 2f, projectile.velocity.Y * 2, ModContent.ProjectileType<AzurianSpearBeam>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
            }
        }
    }

    public class AzurianSpearBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 120;
            projectile.penetrate = 5;
        }
        public override void AI()
        {

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projectilePosition = projectile.position;
                    projectilePosition -= projectile.velocity * (i * 0.25f);
                    projectile.alpha = 255;
                    int dust = Dust.NewDust(projectilePosition, 5, 18, 172, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position = projectilePosition;
                    Main.dust[dust].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dust].velocity *= 0.2f;
                }
            }
        }
    }
}