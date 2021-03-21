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
            item.UseSound = SoundID.Item40;
            item.crit = 20;
            item.damage = 16;
            item.ranged = true;
            item.width = 62;
            item.height = 44;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 10, 50, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = AmmoID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 24f;
        }
    }
}