using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class HailWarning : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Hail Warning");
            Tooltip.SetDefault("Casts an icicle that rains more icicles from the sky");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 11;
            item.magic = true;
            item.mana = 15;
            item.width = 34;
            item.height = 40;
            item.useTime = 15;
            item.useAnimation = 15;
            item.UseSound = SoundID.Item101;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 30, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("HailProjectile");
            item.shootSpeed = 40f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 10);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class HailProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.aiStyle = -1;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        private int timer;

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            timer++;
            if (timer % 5 == 0)
            {
                Vector2 offset = new Vector2(0, -500);
                Projectile.NewProjectile(projectile.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<HailIcicle>(), 6, 1f, Main.myPlayer);
            }
        }
    }

    public class HailIcicle : ModProjectile
    {
        int i;
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.aiStyle = -1;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            i++;
            if (i > 100)
            {
                projectile.tileCollide = true;
            }
            projectile.velocity.Y += 3;
            projectile.rotation = projectile.velocity.ToRotation();
        }
    }
}