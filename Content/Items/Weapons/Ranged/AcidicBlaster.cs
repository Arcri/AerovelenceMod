using System;
using AerovelenceMod.Content.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AcidicBlaster : ModItem
    {
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Acidic Blaster");
		}
        public override void SetDefaults()
        {
			Item.shootSpeed = 24f;
			Item.crit = 8;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
			Item.UseSound = SoundID.Item5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 35, 20);
            Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<DiseasedBlob>();
            Item.autoReuse = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }
    }
}