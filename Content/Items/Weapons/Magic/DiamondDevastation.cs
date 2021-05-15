using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DiamondDevastation : ModItem
    {
        public override void SetStaticDefaults()
        {
            
            DisplayName.SetDefault("Diamond Devastation");
            Tooltip.SetDefault("Casts fast diamond bolts");
        }
      
        
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Lighting.AddLight(item.position, 0.08f, .38f, .24f);
            Texture2D texture;
            texture = Main.itemTexture[item.type];
            spriteBatch.Draw
            (

                ModContent.GetTexture("AerovelenceMod/Content/Items/Weapons/Magic/DiamondDevastation_Glow"),
                new Vector2
                (
                    item.position.X - Main.screenPosition.X + item.width * 0.5f,
                    item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
        public override void SetDefaults()
        {
            Item.staff[item.type] = true;
            item.damage = 45;
            item.magic = true;
            item.mana = 7;
            item.width = item.height = 64;

            item.useTime = item.useAnimation = 32;

            item.UseSound = SoundID.Item110;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DiamondBlastProjectile>(); ;
            item.shootSpeed = 16f;
        }

        
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY - 1 )) * 61f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

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