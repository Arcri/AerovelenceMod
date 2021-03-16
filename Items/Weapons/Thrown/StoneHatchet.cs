using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class StoneHatchet : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stone Hatchet");
		}
        public override void SetDefaults()
        {	
			item.crit = 4;
            item.damage = 23;
            item.ranged = true;
            item.width = 38;
            item.height = 38;
            item.useTime = 30;
            item.useAnimation = 30;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 2, 45, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<StoneHatchetProjectile>();
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 10f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
	public class StoneHatchetProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stone Hatchet");
		}

		int i;

		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 30;
			projectile.aiStyle = 3;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.magic = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
		}
    }
}