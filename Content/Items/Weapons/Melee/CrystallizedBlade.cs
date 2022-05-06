using AerovelenceMod.Content.Items.Others.Crafting;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class CrystallizedBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallized Blade");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 21;
            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.type == Mod.Find<ModNPC>("CrystalSlime").Type)
            {
                damage *= 3;
            }
            {
                if (target.type == Mod.Find<ModNPC>("LuminousDefender").Type)
                {
                    damage *= 3;
                }
                {
                    if (target.type == Mod.Find<ModNPC>("Tumblerock1").Type)
                    {
                        damage *= 3;
                    }
                    {
                        if (target.type == Mod.Find<ModNPC>("Tumblerock2").Type)
                        {
                            damage *= 3;
                        }
                        {
                            if (target.type == Mod.Find<ModNPC>("Tumblerock3").Type)
                            {
                                damage *= 3;
                            }
                            {
                                if (target.type == Mod.Find<ModNPC>("Tumblerock4").Type)
                                {
                                    damage *= 3;
                                }
                                {
                                    if (target.type == Mod.Find<ModNPC>("LuminoJelly").Type)
                                    {
                                        damage *= 3;
                                    }
                                    {
                                        if (target.type == Mod.Find<ModNPC>("MinorCrystalSerpentHead").Type)
                                        {
                                            damage *= 3;
                                        }
                                        {
                                            if (target.type == Mod.Find<ModNPC>("CrystalWormHead").Type)
                                            {
                                                damage *= 3;
                                            }
                                            {
                                                if (target.type == Mod.Find<ModNPC>("DarkseaAngler").Type)
                                                {
                                                    damage *= 3;
                                                }
                                                {
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<CavernCrystal>(), 50)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}