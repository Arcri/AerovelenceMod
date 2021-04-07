using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class HellBrigade : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Hell Brigade");
			Tooltip.SetDefault("Hitting enemies causes an explosion");
		}
        public override void SetDefaults()
        {
			item.useTurn = true;
			item.UseSound = SoundID.Item1;
			item.crit = 7;
            item.damage = 56;
            item.melee = true;
            item.width = 36;
            item.height = 80; 
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5;
            item.value = Item.sellPrice(0, 3, 25, 0);
            item.rare = ItemRarityID.LightPurple;
            item.autoReuse = false;
        }
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 200);
            Projectile.NewProjectile(target.position, target.velocity, ProjectileID.DD2ExplosiveTrapT3Explosion, item.damage / 2, player.whoAmI);
		}
    }
}