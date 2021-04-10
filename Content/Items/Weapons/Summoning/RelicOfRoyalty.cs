using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Projectiles.Weapons.Summoning;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Summoning
{
    public class RelicOfRoyalty : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic Of Royalty");
            Tooltip.SetDefault("Summons an ancient royal protector to protect you");
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
            item.shoot = ModContent.ProjectileType<RoyalDestroyer>();
            item.shootSpeed = 0f;
            item.summon = true;
            item.buffType = ModContent.BuffType<AetherVisionBuff>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                return (false);
            }

            // Spawn new projectile with a random type.
            int summonType = Main.rand.Next(3);
            if (summonType == 0)
            {
                damage -= 9;
            }
            else if (summonType == 1)
            {
                damage += 7;
            }

            player.AddBuff(item.buffType, item.buffTime);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, summonType);

            return (false);
        }

        public override void UseStyle(Player player)
         => player.itemLocation -= new Vector2(96 * player.direction, 18);
    }
}
