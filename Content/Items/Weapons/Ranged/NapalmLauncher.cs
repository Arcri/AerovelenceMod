using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class NapalmLauncher : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Napalm Launcher");
			Tooltip.SetDefault("The sun god's flame, encapsuled into something usable");
		}
        public override void SetDefaults()
        {
			Item.crit = 11;
            Item.damage = 125;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 92;
            Item.height = 30;
            Item.useTime = 12;
            Item.useAnimation = 12;
			Item.UseSound = SoundID.Item11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 11, 40, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Gel; 
            Item.shoot = ProjectileID.Flames;
            Item.shootSpeed = 10f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; ++i)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            return false;
		}
			
    	public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Napalm Launcher", "Artifact Weapon")
			{
				OverrideColor = new Color(255, 241, 000)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.OverrideColor = new Color(255, 132, 000);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
	}
}