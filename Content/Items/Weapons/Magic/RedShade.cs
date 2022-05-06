using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class RedShade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Shade");
            Tooltip.SetDefault("Fires Skulls");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.mana = 5;
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.width = 36;
            Item.height = 48;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RedShadeSoul>();
            Item.shootSpeed = 12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 12)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class RedShadeSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            DisplayName.SetDefault("Shade Soul");
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            //AI (Movement)
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = Main.rand.NextFloat(0.00000000000001f, 9.86960440108877f);
                Projectile.ai[1] = (((Projectile.velocity.Y > 0) ? -1 : 1) == ((Projectile.velocity.X < 0) ? -1 : 1)) ? 1 : -1; //Don't question the logic please.
            }
            if (Projectile.velocity.LengthSquared() < 324f)
            {
                Projectile.velocity *= 1.014f;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy((Math.Sin((Projectile.timeLeft + Projectile.ai[0]) / MathHelper.Pi) / 30) + (Projectile.ai[1] * .016415f));
            Projectile.rotation = Projectile.velocity.ToRotation();

            //AI (Visuals, not Animation)
            if (Main.rand.NextFloat() <= .5f)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, 60);
                d.noGravity = true;
                d.velocity = 1.2f * Projectile.velocity;
            }

            //Animation
            //projectile.spriteDirection = projectile.velocity.X < 0 ? -1 : 1;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
        }
        //Levitating/floating would be a bool/buff handled in GlobalNPC, could be as simple as subtracting from y-velocity.
    }
}