using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Projectiles;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class Delvara : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[Item.type] = true;
            DisplayName.SetDefault("Delvara");
            Tooltip.SetDefault("Fires a speedy fireball");
        }

        public override void SetDefaults()
        {
            Item.crit = 5;
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 10, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("DelvaraFireball").Type;
            Item.shootSpeed = 10f;
		}

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostRay>(), 1)
                .AddIngredient(ItemID.WandofSparking, 1)
                .AddIngredient(ItemID.Vilethorn, 1)
                .AddIngredient(ItemID.NaturesGift, 1)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    internal sealed class DelvaraFireball : ModProjectile
    {
        // Thanks to Eldrazi for the code :D
        // Determines how fast the helix rotates.
        // Currently set to half a second for a full rotation.
        private readonly int oneHelixRevolutionInUpdateTicks = 30;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;

            Projectile.alpha = 3;

            Projectile.friendly = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreAI()
        {
            // Dust spawning.
            ++Projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;

            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;

            Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), DustID.RedTorch);
            newDust.noGravity = true;

            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), DustID.RedTorch);
            newDust.noGravity = true;

            // Rotate the projectile towards the direction it's going.
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;

            return (false);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            SoundEngine.PlaySound(SoundID.Item10);
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            return true;
        }

		public override bool PreDraw(ref Color lightColor) 
            => this.DrawAroundOrigin(Main.spriteBatch, lightColor * Projectile.Opacity);
    }
}
