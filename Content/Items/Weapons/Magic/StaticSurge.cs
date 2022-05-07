using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class StaticSurge : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Static Surge");
            Tooltip.SetDefault("Casts lightning volleys that explode on contact");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 43;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.DiamondBolt;
            Item.shootSpeed = 15f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }
    }
}