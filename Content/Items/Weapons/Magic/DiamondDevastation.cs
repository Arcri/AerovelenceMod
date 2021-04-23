using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DiamondDevastation : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Diamond Devastation");
            Tooltip.SetDefault("Casts fast diamond bolts");
        }
        public override void SetDefaults()
        {
            item.damage = 33;
            item.magic = true;
            item.mana = 20;
            item.width  = item.height = 64;
            
            item.useTime = item.useAnimation = 55;
           
            item.UseSound = SoundID.Item110;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DiamondBlastProjectile>(); ;
            item.shootSpeed = 15f;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numberProjectiles = 4;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(18));
                                                                                                                
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale; 
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
            }

            return false;
        }


        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.CrystalShard, 5);
            modRecipe.AddIngredient(ItemID.SoulofLight, 15);
            modRecipe.AddIngredient(ItemID.IronBar, 5);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this);
            modRecipe.AddRecipe();
        }
    }
}