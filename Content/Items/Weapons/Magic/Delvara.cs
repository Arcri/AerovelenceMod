using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class Delvara : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Delvara");
            Tooltip.SetDefault("Fires a speedy fireball");
        }

        public override void SetDefaults()
        {
            item.crit = 5;
            item.damage = 35;
            item.magic = true;
            item.mana = 15;
            item.width = 50;
            item.height = 50;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 10, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("DelvaraFireball");
            item.shootSpeed = 10f;
		}

        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostRay>(), 1);
            modRecipe.AddIngredient(ItemID.WandofSparking, 1);
            modRecipe.AddIngredient(ItemID.Vilethorn, 1);
            modRecipe.AddIngredient(ItemID.NaturesGift, 1);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
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
            projectile.width = projectile.height = 12;

            projectile.alpha = 3;

            projectile.friendly = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreAI()
        {
            // Dust spawning.
            ++projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;

            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * projectile.height;

            Dust newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), DustID.Fire);
            newDust.noGravity = true;

            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(projectile.Center + newDustPosition.RotatedBy(projectile.velocity.ToRotation()), DustID.Fire);
            newDust.noGravity = true;

            // Rotate the projectile towards the direction it's going.
            projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;

            return (false);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            Main.PlaySound(SoundID.Item10);
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Fire, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
            => this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
    }
}
