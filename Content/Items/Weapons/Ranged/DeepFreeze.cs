using AerovelenceMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class DeepFreeze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Freeze");
        }
        public override void SetDefaults()
        {
            item.damage = 13;
            item.noMelee = true;
            item.ranged = true;
            item.channel = true;
            item.width = 70;
            item.height = 44;
            item.useTime = item.useAnimation = 20;
            item.UseSound = SoundID.Item13;
            item.useStyle = 5;
            item.shootSpeed = 14f;
            
            item.shoot = ModContent.ProjectileType<DeepFreezeProjectile>();
            item.value = Item.sellPrice(silver: 3);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
 
    }
}