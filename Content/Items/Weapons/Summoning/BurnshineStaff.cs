using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
    public class BurnshineStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burnshine Staff");
            Tooltip.SetDefault("Summons a neutron star that fires stars at enemies");
        }

        public override void SetDefaults()
        {
            Item.mana = 8;
            Item.damage = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<BurningNeutronStar>();
            Item.shootSpeed = 0f;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<BurnshineStaffBuff>();
		}
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }
    }
}