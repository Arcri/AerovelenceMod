using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Projectiles;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class AdobeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.staff[item.type] = true;
            DisplayName.SetDefault("Adobe Staff");
        }
        public override void SetDefaults()
        {
            item.crit = 5;
            item.damage = 13;
            item.magic = true;
            item.mana = 15;
            item.width = 50;
            item.height = 50;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 10, 0);
            item.rare = ItemRarityID.LightRed;
            item.autoReuse = true;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 11f;
		}
        public override void AddRecipes()
        {
            ModRecipe modRecipe = new ModRecipe(mod);
            modRecipe.AddIngredient(ModContent.ItemType<FrostRay>(), 1);
            modRecipe.AddIngredient(ItemID.WandofSparking, 1);
            modRecipe.AddIngredient(ItemID.Vilethorn, 1);
            modRecipe.AddIngredient(ItemID.NaturesGift, 1);
            modRecipe.AddIngredient(ItemID.HellstoneBar, 15);
            modRecipe.AddTile(TileID.Anvils);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();
        }
    }
}

