using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class CrystalGlade : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Glade");
			Tooltip.SetDefault("Legends say that this was just a rewritten Plant Fiber Cordage");
		}
        public override void SetDefaults()
        {
			item.crit = 11;
            item.damage = 97;
            item.magic = true;
			item.mana = 5;
            item.width = 30;
            item.height = 34;
            item.useTime = 7;
            item.useAnimation = 7;
			item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
			item.value = Item.sellPrice(0, 20, 50, 0);
			item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<CrystalGladeProj>();
            item.shootSpeed = 14f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(mod, "Verbose:RemoveMe", "Why do I exist, dawg");
			tooltips.Add(line);

			line = new TooltipLine(mod, "Crystal Glade", "Legendary item")
			{
				overrideColor = new Color(255, 241, 000)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(255, 132, 000);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
	public class CrystalGladeProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.aiStyle = 227;
			projectile.width = 18;
			projectile.height = 38;
			projectile.alpha = 0;
			projectile.penetrate = 4;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}
        public override void AI()
        {
			projectile.rotation = projectile.velocity.ToRotation();
		}
    }
}