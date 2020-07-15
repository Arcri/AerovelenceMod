using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class DarknessDischarge : ModItem
    {
		int i;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Darkness Discharge");
		}
        public override void SetDefaults()
        {
			item.mana = 6;
			item.crit = 8;
            item.damage = 32;
            item.magic = true;
            item.width = 124;
            item.height = 88;
            item.useTime = 12;
            item.useAnimation = 12;
			item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
			item.shoot = ProjectileID.ClothiersCurse;
			item.shootSpeed = 18f;
        }
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (i == 1)
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.ShadowFlameKnife, damage, knockBack, player.whoAmI);				
			}
			if (i >= 2)
			{
				i = 0;
				Projectile.NewProjectile(position.X, position.Y, speedX * 2, speedY * 2, ProjectileID.ShadowFlame, damage, knockBack, player.whoAmI);
			}
			i++;
            return true;
        }
    }
}