using AerovelenceMod.Buffs;
using AerovelenceMod.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Summoning
{
    public class AetherVision : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether Vision");
            Tooltip.SetDefault("Summons a mini Cyvercry to protect you");
        }
        public override void SetDefaults()
        {
            item.mana = 8;
            item.damage = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 62;
            item.height = 62;
            item.useTime = 16;
            item.useAnimation = 16;
            item.noMelee = true;
            item.knockBack = 1f;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 2, 0, 0);
            item.UseSound = SoundID.Item8;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<Minicry>();
            item.shootSpeed = 0f;
            item.summon = true;
            item.buffType = ModContent.BuffType<AetherVisionBuff>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }
    }
}