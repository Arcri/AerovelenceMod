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
            Item.crit = 20;
            Item.maxStack = 5;
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("SoulChakramProjectile").Type;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override bool CanUseItem(Player player)
        {
            
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active == true && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot && Main.projectile[i] != previousProjectile)
                {
                    amount++;
                    if (amount >= Item.stack)
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
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            // projectile.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            i++;
            if (i == 45)
            {
                Projectile.damage *= 2;
            }
            int num622 = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y), Projectile.width, Projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            Projectile.rotation += 0.1f;
        }
    }
}