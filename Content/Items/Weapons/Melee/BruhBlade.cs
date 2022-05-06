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
            Item.channel = true;		
			Item.crit = 2000;
            Item.damage = 7000000;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.mana = 10;
			Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = 10000;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = Mod.Find<ModProjectile>("BruhBladeProjectile").Type;
            Item.shootSpeed = 2f;
        }
    }
}