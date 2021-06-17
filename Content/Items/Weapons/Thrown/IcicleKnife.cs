using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class IcicleKnife : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Icicle Knife");
        public override void SetDefaults()
        {           
            item.damage = 16;
            
            item.width = 60;
            item.height = 32;
            item.useTime = item.useAnimation = 20;
                       
            item.noMelee = item.consumable = item.melee = item.autoReuse = item.noUseGraphic = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.maxStack = 999;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            
            item.shoot = ModContent.ProjectileType<IcicleKnifeProj>();
            item.shootSpeed = 18f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld - new Vector2(0f, 600f);

            for (int i = 0; i < 1; i++)
            {
                position.Y -= 500 * i;
                position.X += Main.rand.Next(-6, 6) * 16f;

                var velocity = Vector2.Normalize(Main.MouseWorld - position) * item.shootSpeed;

                Projectile.NewProjectileDirect(position, velocity, type, damage, knockBack, player.whoAmI);
            }                    
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 5);
            recipe.AddIngredient(ItemID.IronBar, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }
    }

    public class IcicleKnifeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;

            projectile.width = 24;
            projectile.height = 18;
            projectile.friendly = projectile.melee = projectile.tileCollide = projectile.ignoreWater = true;

            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.aiStyle = 1;
        }
        private bool rotChanged = false;   
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item27);
            Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/IcicleKnifeGore1"), 1f); 
            Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/IcicleKnifeGore2"), 1f);
            Gore.NewGore(projectile.position, Vector2.Zero, mod.GetGoreSlot("Gores/IcicleKnifeGore3"), 1f);
            
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60);

            projectile.velocity = target.position;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Blue, Color.Purple, 0.5f + (float)Math.Sin(MathHelper.ToRadians(projectile.frame)) / 2f) * 0.7f;
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.oldRot[i], rectangle.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            return true;
        }
       
        public override void AI()
        {
            if (!rotChanged)
            {
                projectile.rotation = projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
            projectile.velocity.X *= 0.982f;
            projectile.velocity.Y += 0.14f;     
        }
    }
}