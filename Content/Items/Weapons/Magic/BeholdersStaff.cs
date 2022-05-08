using System;
using AerovelenceMod.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class BeholdersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            DisplayName.SetDefault("Beholder's Staff");
            Tooltip.SetDefault("Does Something");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 82;
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
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("BeholderOrb").Type;
            Item.shootSpeed = 40f;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostRay>(), 1)
                .AddIngredient(ItemID.StaffofEarth, 1)
                .AddIngredient(ItemID.SpectreStaff, 1)
                .AddIngredient(ItemID.ShadowbeamStaff, 1)
                .AddIngredient(ItemID.Ectoplasm, 40)
                .AddIngredient(ItemID.ShroomiteBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}