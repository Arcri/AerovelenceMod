using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class IcicleKnife : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Icicle Knife");
        public override void SetDefaults()
        {
            Item.damage = 16;

            Item.width = 60;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 20;

            Item.noMelee = true;
            Item.consumable = false;
            Item.DamageType = DamageClass.Melee;// item.autoReuse = item.noUseGraphic = true /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 999;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<IcicleKnifeProj>();
            Item.shootSpeed = 18f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld - new Vector2(0f, 600f);

            for (int i = 0; i < 1; i++)
            {
                position.Y -= 500 * i;
                position.X += Main.rand.Next(-6, 6) * 16f;

                var velocity1 = Vector2.Normalize(Main.MouseWorld - position) * Item.shootSpeed;

                Projectile.NewProjectileDirect(source, position, velocity1, type, damage, 2f, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 5)
                .AddIngredient(ItemID.IronBar, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IcicleKnifeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;

            Projectile.width = 24;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;// projectile.tileCollide = projectile.ignoreWater = true /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;

            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.aiStyle = 1;
        }
        private bool rotChanged = false;   
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            var entitySource = Projectile.GetSource_Death();
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            SoundEngine.PlaySound(SoundID.Item27);
            Gore.NewGore(entitySource, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/IcicleKnifeGore1").Type, 1f); 
            Gore.NewGore(entitySource, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/IcicleKnifeGore2").Type, 1f);
            Gore.NewGore(entitySource, Projectile.position, Vector2.Zero, Mod.Find<ModGore>("Gores/IcicleKnifeGore3").Type, 1f);

            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60);

            Projectile.velocity = target.position;
        }
		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Blue, Color.Purple, 0.5f + (float)Math.Sin(MathHelper.ToRadians(Projectile.frame)) / 2f) * 0.7f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.oldRot[i], rectangle.Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            return true;
        }
       
        public override void AI()
        {
            if (!rotChanged)
            {
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
            Projectile.velocity.X *= 0.982f;
            Projectile.velocity.Y += 0.14f;     
        }
    }
}