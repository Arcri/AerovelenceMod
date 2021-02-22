using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DiseasedBlobStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Diseased Blob Staff");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 49;
            item.ranged = true;
            item.mana = 12;
            item.width = 44;
            item.height = 46;
            item.useTime = 14;
			item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("DiseasedBlob");
            item.shootSpeed = 6;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 7; i++)
            {
                int rand = new Random().Next(0, 100001);
                Projectile.NewProjectile(position.X, position.Y, speedX * (float)new Random(i + rand).NextDouble(), speedY * (float)new Random(i - 1 + rand).NextDouble(), item.shoot, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DiseasedBlob : ModProjectile
    {
        public override void SetDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.hostile = false;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
        }
        public override void AI()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] % 7 == 0)
            {
                projectile.ai[1]++;
                if (projectile.ai[1] > 2)
                    projectile.ai[1] = 0;
                projectile.frame = (int)projectile.ai[1];
            }
        }
    }
}