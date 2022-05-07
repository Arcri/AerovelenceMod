using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class ScepterOfTheSkies : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Scepter of the skies");
            Tooltip.SetDefault("Unleashes the power from the sky");
        }

        public override void SetDefaults()
        {
            Item.crit = 5;
            Item.damage = 58;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("HomingWisp").Type;
            Item.shootSpeed = 12f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(45);
            
            for (int i = 0; i < 2; ++i)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HomingWisp>(), ModContent.ProjectileType<HomingWispPurple>() });
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<Delvara>(), 1)
                .AddIngredient(ItemID.CrystalVileShard, 1)
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}