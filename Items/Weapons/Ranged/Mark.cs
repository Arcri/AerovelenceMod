using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class Mark : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Mark");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item11;
			item.crit = 8;
            item.damage = 132;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 31;
			item.useAnimation = 31;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("MarkLaser");
            item.shootSpeed = 25f;
        }
    }
}
	
namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class MarkLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
            projectile.alpha = 60;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }
        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 vector = new Vector2((float)Main.projectileTexture[projectile.type].Width * 0.5f, (float)projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}