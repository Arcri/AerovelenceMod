using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class Exodious : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exodious");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 20;
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
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("ExodiousProjectile");
            item.shootSpeed = 2f;
        }
    }
}