using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class SoulChakram : ModItem
    {
        int amount = 0;
        Projectile previousProjectile = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Chakram");
            Tooltip.SetDefault("Stacks up to 5\nDeals double damage when going backwards");
        }
        public override void SetDefaults()
        {	
            item.crit = 20;
            item.maxStack = 5;
            item.damage = 16;
            item.melee = true;
            item.width = 20;
            item.height = 30;
            item.useTime = 14;
            item.useAnimation = 14;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("SoulChakramProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool CanUseItem(Player player)
        {
            
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active == true && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot && Main.projectile[i] != previousProjectile)
                {
                    amount++;
                    if (amount >= item.stack)
                    {
                        return false;
                    }
                }
                else if (Main.projectile[i] == previousProjectile)
                {
                    i = 0;
                }
                previousProjectile = Main.projectile[i];
            }
            amount = 0;
            return true;
        }
    }

    public class SoulChakramProjectile : ModProjectile
    {
        int i = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Chakram");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 30;
            projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.magic = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            i++;
            if (i == 45)
            {
                projectile.damage *= 2;
            }
            int num622 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X, projectile.position.Y - projectile.velocity.Y), projectile.width, projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            projectile.rotation += 0.1f;
        }
    }
}