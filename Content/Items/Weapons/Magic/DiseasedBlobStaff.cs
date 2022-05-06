using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class DiseasedBlobStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Diseased Blob Staff");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item11;
            Item.crit = 8;
            Item.damage = 49;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 44;
            Item.height = 46;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("DiseasedBlob").Type;
            Item.shootSpeed = 6;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 7; i++)
            {
                int rand = new Random().Next(0, 100001);
                Projectile.NewProjectile(position.X, position.Y, speedX * (float)new Random(i + rand).NextDouble(), speedY * (float)new Random(i - 1 + rand).NextDouble(), Item.shoot, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }

    public class DiseasedBlob : ModProjectile
    {
        public override void SetDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 420;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 7 == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 2)
                    Projectile.ai[1] = 0;
                Projectile.frame = (int)Projectile.ai[1];
            }
        }
    }
}