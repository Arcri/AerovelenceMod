using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class HailWarning : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Hail Warning");
            Tooltip.SetDefault("'Ancient memories of frost and lost souls'");
        }
        public override void SetDefaults()
        {
            item.crit = 6;
            item.damage = 11;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.UseSound = SoundID.Item101;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 30, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("HailProjectile");
            item.shootSpeed = 40f;
        }
    }
}
namespace AerovelenceMod.Items.Weapons.Magic
{
    public class HailProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.aiStyle = -1;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        private int timer;

        public override void AI()
        {
            timer++;
            if (timer % 5 == 0)
            {
                Vector2 offset = new Vector2(0, -500);
                Projectile.NewProjectile(projectile.Center + offset, new Vector2(0 + ((float)Main.rand.Next(20) / 10) - 1, -3 + ((float)Main.rand.Next(20) / 10) - 1), ModContent.ProjectileType<HailIcicle>(), 6, 1f, Main.myPlayer);
            }
        }
    }
}
namespace AerovelenceMod.Items.Weapons.Magic
{
    public class HailIcicle : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.aiStyle = -1;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
        }
        public override void AI()
        {
            projectile.velocity.Y += 3;
            projectile.rotation += projectile.velocity.X * 0.01f;
        }
    }
}