using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class FlameShot : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flameshot");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item5;
			item.crit = 9;
            item.damage = 26;
            item.ranged = true;
            item.width = 30;
            item.height = 54;
            item.useTime = 22;
			item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = AmmoID.Arrow;
			item.useAmmo = AmmoID.Arrow;
            item.shootSpeed = 7f;
            fireArrow = new Projectile();
        }
        private Projectile fireArrow;
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
            fireArrow = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), ProjectileID.FireArrow, damage, knockBack, player.whoAmI);




			return false;
		}
        public override void UpdateInventory(Player player)
        {
            if (fireArrow.active && fireArrow.type == ProjectileID.FireArrow)
            {

                int projectiles = 3;
                Vector2 position = fireArrow.Center;
                float numberProjectiles = 5f;
                float rotation = MathHelper.ToRadians(180f);
                int i = 0; 
                if (Main.rand.NextFloat() <= 0.05f)
                {
                    while (i < numberProjectiles)
                    {
                        Vector2 perturbedSpeed = Utils.RotatedBy(new Vector2(fireArrow.velocity.X, fireArrow.velocity.Y), MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1f)), default) * 0.2f;
                        var m = Projectile.NewProjectileDirect(position, new Vector2(perturbedSpeed.X, perturbedSpeed.Y), ProjectileID.MolotovFire, (int)(player.HeldItem.damage * 1.5f), 0, player.whoAmI, 0f, 0f);
                        m.friendly = true;
                        m.hostile = false;
                        i++;
                    }
                }
            }
        }
    }
}