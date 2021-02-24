using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Oblivion : ModItem
    {
        public bool NPCHit;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oblivion");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 75;
            item.melee = true;
            item.width = 60;
            item.height = 68;
            item.useTime = 25;
            item.useAnimation = 25;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7;
            item.shoot = ProjectileID.FlamingArrow;
            item.shootSpeed = 60f;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }
        public override bool UseItem(Player player)
        {
            player.direction = (Main.MouseWorld.X - player.Center.X > 0) ? 1 : -1;
            return true;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<OblivionsWrath>();
            {
                for (int i = -4; i < 4; i++)
                {
                    position = Main.MouseWorld + new Vector2(i * 20, -850);
                    Vector2 velocity = (Main.MouseWorld - position).SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * item.shootSpeed;
                    Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
                }
                return false;
            }
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class OblivionsWrath : ModProjectile
    {
        int i;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oblivion's Wrath");
        }
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            drawOffsetX = -45;
            projectile.alpha = 255;
            drawOriginOffsetY = 0;
            projectile.damage = 65;
            drawOriginOffsetX = 23;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            i++;
            if (i % 1 == 0)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height / 2, 164);
            }
        }
    }
}