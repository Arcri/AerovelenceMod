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
			Item.useTurn = true;
			Item.UseSound = SoundID.Item1;
			Item.crit = 7;
            Item.damage = 56;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 80; 
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 3, 25, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = false;
        }
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 200);
            Projectile.NewProjectile(target.GetSource_OnHit(target), target.position, target.velocity, ProjectileID.DD2ExplosiveTrapT3Explosion, Item.damage / 2, player.whoAmI);
		}
    }
}