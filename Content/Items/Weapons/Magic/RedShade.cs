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
            item.crit = 4;
            item.mana = 5;
            item.damage = 18;
            item.magic = true;
            item.width = 36;
            item.height = 48;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<RedShadeSoul>();
            item.shootSpeed = 12f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 12);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class RedShadeSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
            DisplayName.SetDefault("Shade Soul");
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 600;
            projectile.penetrate = 1;
        }
        public override void AI()
        {
            //AI (Movement)
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = Main.rand.NextFloat(0.00000000000001f, 9.86960440108877f);
                projectile.ai[1] = (((projectile.velocity.Y > 0) ? -1 : 1) == ((projectile.velocity.X < 0) ? -1 : 1)) ? 1 : -1; //Don't question the logic please.
            }
            if (projectile.velocity.LengthSquared() < 324f)
            {
                projectile.velocity *= 1.014f;
            }
            projectile.velocity = projectile.velocity.RotatedBy((Math.Sin((projectile.timeLeft + projectile.ai[0]) / MathHelper.Pi) / 30) + (projectile.ai[1] * .016415f));
            projectile.rotation = projectile.velocity.ToRotation();

            //AI (Visuals, not Animation)
            if (Main.rand.NextFloat() <= .5f)
            {
                Dust d = Dust.NewDustDirect(projectile.Center, 1, 1, 60);
                d.noGravity = true;
                d.velocity = 1.2f * projectile.velocity;
            }

            //Animation
            //projectile.spriteDirection = projectile.velocity.X < 0 ? -1 : 1;
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= 3)
                    projectile.frame = 0;
            }
        }
        //Levitating/floating would be a bool/buff handled in GlobalNPC, could be as simple as subtracting from y-velocity.
    }
}