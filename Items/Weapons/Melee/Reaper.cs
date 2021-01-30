using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Reaper : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Reaper");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 23;
            item.damage = 36;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 24;
            item.useAnimation = 24;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("ReaperProj");
            item.shootSpeed = 2f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class ReaperProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 6;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 216f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 13f;
        }
    }
}