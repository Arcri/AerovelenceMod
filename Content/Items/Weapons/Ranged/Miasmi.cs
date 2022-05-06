using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class Miasmi : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miasmi");
            Tooltip.SetDefault("Runs on fear");
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 7);
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item40;
            Item.crit = 20;
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 44;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 24f;
        }
    }
}