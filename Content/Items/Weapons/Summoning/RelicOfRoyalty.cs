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
            Item.mana = 8;
            Item.damage = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RoyalDestroyer>();
            Item.shootSpeed = 0f;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<AetherVisionBuff>();
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

            player.AddBuff(Item.buffType, Item.buffTime);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, summonType);

            return (false);
        }

        public override void UseStyle(Player player)
         => player.itemLocation -= new Vector2(96 * player.direction, 18);
    }
}
