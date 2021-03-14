using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class BeholdersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Beholder's Staff");
            Tooltip.SetDefault("Does Something");
        }
        public override void SetDefaults()
        {
            item.crit = 11;
            item.damage = 82;
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
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("HomingWisp");
            item.shootSpeed = 40f;
        }
        public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        {
            var posArray = new Vector2[num];
            float spread = (float)(angle * 0.075);
            float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
            double baseAngle = System.Math.Atan2(speedX, speedY);
            double randomAngle;
            for (int i = 0; i < num; ++i)
            {
                randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
            }
            return (Vector2[])posArray;
        }

        public static void SpawnDustFromTexture(Vector2 position, int dustType, float size, string imagePath, bool noGravity = true, float rot = 0.34f)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float rotation = Main.rand.NextFloat(-rot, rot);
                Texture2D texture = ModContent.GetTexture(imagePath);
                int[] pixelData = new int[texture.Width * texture.Height];

                texture.GetData(pixelData);

                for (int i = 0; i < texture.Width; i += 2)
                {
                    for (int j = 0; j < texture.Height; j += 2)
                    {
                        if (pixelData[j * texture.Width + i] != 0)
                        {
                            Vector2 dustPosition = new Vector2(i - texture.Width / 2, j - texture.Height / 2) * size;
                            Dust.NewDustPerfect(position, dustType, dustPosition.RotatedBy(rotation)).noGravity = noGravity;
                        }
                    }
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speed = 0f;
            Vector2 velocity = new Vector2(speed, speed).RotatedByRandom(MathHelper.ToRadians(360));
            Projectile.NewProjectile(Main.MouseWorld, velocity, ModContent.ProjectileType<EyeOfTheBeholder>(), 50, 5f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostRay>(), 1);
            modRecipe.AddIngredient(ItemID.StaffofEarth, 1);
            modRecipe.AddIngredient(ItemID.SpectreStaff, 1);
            modRecipe.AddIngredient(ItemID.ShadowbeamStaff, 1);
            modRecipe.AddIngredient(ItemID.Ectoplasm, 40);
            modRecipe.AddIngredient(ItemID.ShroomiteBar, 15);
            modRecipe.AddTile(TileID.MythrilAnvil);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}