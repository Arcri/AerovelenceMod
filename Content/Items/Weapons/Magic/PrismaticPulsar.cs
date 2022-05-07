using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class PrismaticPulsar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Prismatic Pulsar");
            Tooltip.SetDefault("Burns with the fury of the lost souls");
        }

        public override void SetDefaults()
        {
            Item.crit = 5;
            Item.damage = 82;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("HomingWisp").Type;
            Item.shootSpeed = 14f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(45);
            for (int i = 0; i < 3; ++i)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HomingWisp>(), ModContent.ProjectileType<HomingWispPurple>() });
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, 1f, player.whoAmI, 2);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<ScepterOfTheSkies>(), 1)
                .AddIngredient(ItemID.FrostStaff, 1)
                .AddIngredient(ItemID.StaffofEarth, 1)
                .AddIngredient(ItemID.SpectreStaff, 1)
                .AddIngredient(ItemID.ShadowbeamStaff, 1)
                .AddIngredient(ItemID.Ectoplasm, 40)
                .AddIngredient(ItemID.ShroomiteBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}