using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class PhantomSong : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Song");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item5;
            item.crit = 4;
            item.damage = 17;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
            item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 8.5f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Main.rand.Next(new int[] { type, type, mod.ProjectileType("PhantomSongArrow") });
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 8);
            recipe.AddRecipeGroup("AerovelenceMod:EvilMaterials", 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class PhantomSongArrow : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.velocity *= 1.01f;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
        }
        public override void AI()
        {
            i++;
            if (i % 25 == 0)
            {
                Projectile.NewProjectile(projectile.Center, projectile.velocity * 0.99f, ModContent.ProjectileType<PhantomSongAura>(), projectile.damage, projectile.knockBack);
            }
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.frameCounter++;
            if (projectile.frameCounter % 3 == 0)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= 3)
                    projectile.frame = 0;
            }
            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 60);
            dust.noGravity = true;
            dust.velocity *= 0.1f;
        }
    }

    internal sealed class PhantomSongAura : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 10;
            projectile.penetrate = 5;
            projectile.alpha = 3;
            projectile.timeLeft = 100;
            projectile.damage = 60;
            projectile.scale = 1f;

            projectile.friendly = true;
        }
        public override bool PreAI()
        {
            projectile.scale *= 0.99f;
            projectile.velocity *= 0.000001f;
            Vector2 from = projectile.position;
            for (int i = 0; i < 360; i += 20)
            {
                Vector2 circular = new Vector2(24 * projectile.scale, 0).RotatedBy(MathHelper.ToRadians(i));
                circular.X *= 0.7f;
                circular = circular.RotatedBy(Math.Atan2(projectile.velocity.Y, projectile.velocity.X));
                Vector2 dustVelo = new Vector2(0, 0).RotatedBy(Math.Atan2(projectile.velocity.Y, projectile.velocity.X));
                Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, 60, 0, 0, projectile.alpha);
                dust.velocity *= 0.15f;
                dust.velocity += dustVelo;
                dust.noGravity = true;
            }
            return true;
        }
    }
}