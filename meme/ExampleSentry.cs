using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Projectiles.Weapons.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.meme
{
    public class ExampleSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Sentry"); //The english name of the summoning weapon.
            Tooltip.SetDefault("Summons a mini Cyvercry to protect you");
        }
        public override void SetDefaults()
        {
            item.mana = 8; //How much mana this takes to use.
            item.damage = 26; //How much damage this weapon can do.
            item.useStyle = ItemUseStyleID.SwingThrow; //The type of animation that plays when used, SwingThrow is similar to a sword.
            item.width = 62; //The width of the item hitbox.
            item.height = 62; //The height of the item hitbox.
            item.useTime = 16; //How fast you can use this weapon
            item.useAnimation = 16; //The use.
            item.noMelee = true; //Make this weapon do no damage.
            item.knockBack = 1f; //The amount of knockback an enemy can take when struck with this weapon.
            item.rare = ItemRarityID.Pink; //The vanilla terraria rarity.
            item.value = Item.sellPrice(0, 2, 0, 0); //How much this item can sell for.
            item.UseSound = SoundID.Item8; //The sound that plays when the item is used.
            item.autoReuse = false; //If this is set to true, the player can hold down the mouse.
            item.shoot = ModContent.ProjectileType<ExampleSentryMinion>();
            item.shootSpeed = 0f; //How fast this weapon can shoot, this weapon spawns a projectile where the mouse cursor is, however.
            item.summon = true; //Sets the item to the summoner damage class.
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                position = Main.MouseWorld; //Spawn the projectile wherever the mouse cursor is
            }
            return true; //Return true, that way it shoots the projectile set in SetDefaults().
        }
    }
}