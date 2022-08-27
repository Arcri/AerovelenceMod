using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Gun");
            Tooltip.SetDefault("4% summon tag crit chance \n ");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item110;
            Item.crit = 4;
            Item.damage = 20;
            Item.DamageType = DamageClass.Summon;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FrostFlare>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 13f * 1.5f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}