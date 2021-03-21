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
            item.crit = 11;
            item.damage = 43;
            item.magic = true;
            item.mana = 20;
            item.width = 64;
            item.height = 64;
            item.useTime = 65;
            item.useAnimation = 65;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.shoot = ProjectileID.DiamondBolt;
            item.shootSpeed = 15f;
        }
        public static Vector2[] RandomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.1f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return posArray;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2[] speeds = RandomSpread(speedX, speedY, 5, 5);
            for (int i = 0; i < 3; ++i)
            {
                type = Main.rand.Next(new int[] { type, ProjectileID.DiamondBolt, ProjectileID.DiamondBolt });
                Projectile.NewProjectile(position.X, position.Y, speeds[i].X, speeds[i].Y, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ItemID.CrystalShard, 5);
            modRecipe.AddIngredient(ItemID.SoulofLight, 15);
            modRecipe.AddRecipeGroup("IronBar", 5);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}