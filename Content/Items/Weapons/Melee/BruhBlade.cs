using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class BruhBlade : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Supreme Ech Sword of Death and also death");
		}
        public override void SetDefaults()
        {
            item.channel = true;		
			item.crit = 2000;
            item.damage = 7000000;
            item.ranged = true;
            item.width = 64;
            item.height = 64;
            item.useTime = 24;
            item.useAnimation = 24;
            item.mana = 10;
			item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
			item.noUseGraphic = true;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("BruhBladeProjectile");
            item.shootSpeed = 2f;
        }
    }
}