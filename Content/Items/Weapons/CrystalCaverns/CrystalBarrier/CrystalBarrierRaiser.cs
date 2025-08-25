using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Sets.Phantic;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CrystalBarrier;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CrystalBarrier
{
    internal class CrystalBarrierRaiser : TranslatableModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            this.ModifyLocalization("Crystal Thorn-Barrier Raiser", "(placeholder description, final item should not have a description as it is very simplistic and easy to craft) Summons a weak barrier to protect from enemies with large amounts of kb. Can also be used in a spam playstyle as an active move unlike standard summoner passive gameplay.")
            .AddName(Language.Default, "Crystal Thorn-Barrier Raiser")
            .AddTooltip(Language.Default, "(placeholder description, final item should not have a description as it is very simplistic and easy to craft) \nSummons a weak barrier to protect from enemies with large amounts of kb. \nCan also be used in a spam playstyle as an active move unlike standard summoner passive gameplay by ignoring the cooldown and resummoning.");
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.sentry = true;
            Item.mana = 8; //How much mana this weapon takes to use.
            Item.width = 28; //Item width hitbox.
            Item.height = 26; //Item height hitbox.
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true; //Restricts this weapon dealing melee damage.
            Item.knockBack = 9;
            Item.value = Item.buyPrice(0, 20, 0, 0); //How much this item is sold for.
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item83;
            Item.shoot = ModContent.ProjectileType<CrystalBarrier>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 5)
                .AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 16)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            FindSentryRestingSpot(player, type, out position);
            return;

        }
        public static void FindSentryRestingSpot(Player player, int checkProj, out Vector2 position)
        {
            player.FindSentryRestingSpot(checkProj, out int worldX, out int worldY, out int pushYUp);
            position = new Vector2(worldX, worldY);

            if (checkProj <= -1 || checkProj >= ProjectileLoader.ProjectileCount)
            {
                position.Y -= pushYUp;
                return;
            }

            Projectile p = ContentSamples.ProjectilesByType[checkProj];
            pushYUp = (int)Math.Ceiling(p.height / 2f); //Always round up in case height is not even

            position.Y -= pushYUp;
        }
    }

}







