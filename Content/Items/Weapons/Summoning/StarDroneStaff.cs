using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
    public class StarDroneStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star drone Staff");
            Tooltip.SetDefault("Summons a neutron star that fires stars at enemies");
        }
        public override void SetDefaults()
        {
            Item.mana = 8;
            Item.damage = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 34;
            Item.height = 62;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<ShockburnDrone>();
            Item.shootSpeed = 0f;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<BurnshockDroneBuff>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(Item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }
    }
}