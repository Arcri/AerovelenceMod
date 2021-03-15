using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class MothLeg : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moth Leg");
            Tooltip.SetDefault("Hitting an enemy can chain electricity to hit multiple enemies");
		}
        public override void SetDefaults()
        {	
			item.crit = 8;
            item.damage = 20;
            item.melee = true;
            item.width = 20;
            item.height = 30;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 3;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("MothLegProjectile");
            item.UseSound = SoundID.Item1;
            item.shootSpeed = 20f;
        }


        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class MothLegProjectile : ModProjectile
    {
        int i;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth Leg");
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 82;
            projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.magic = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            int num622 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X, projectile.position.Y - projectile.velocity.Y), projectile.width, projectile.height, 191, 0f, 0f, 100, default, 2f);
            Main.dust[num622].noGravity = true;
            Main.dust[num622].scale = 0.5f;
            projectile.rotation += 0.05f;
        }
    }
}