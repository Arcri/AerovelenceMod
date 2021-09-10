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
            Tooltip.SetDefault("Deals double damage to most Crystal Caverns enemies");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 21;
            item.melee = true;
            item.width = 46;
            item.height = 46;
            item.useTime = 16;
            item.useAnimation = 16;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.type == mod.NPCType("CrystalSlime"))
            {
                damage *= 3;
            }
            {
                if (target.type == mod.NPCType("LuminousDefender"))
                {
                    damage *= 3;
                }
                {
                    if (target.type == mod.NPCType("Tumblerock1"))
                    {
                        damage *= 3;
                    }
                    {
                        if (target.type == mod.NPCType("Tumblerock2"))
                        {
                            damage *= 3;
                        }
                        {
                            if (target.type == mod.NPCType("Tumblerock3"))
                            {
                                damage *= 3;
                            }
                            {
                                if (target.type == mod.NPCType("Tumblerock4"))
                                {
                                    damage *= 3;
                                }
                                {
                                    if (target.type == mod.NPCType("LuminoJelly"))
                                    {
                                        damage *= 3;
                                    }
                                    {
                                        if (target.type == mod.NPCType("MinorCrystalSerpentHead"))
                                        {
                                            damage *= 3;
                                        }
                                        {
                                            if (target.type == mod.NPCType("CrystalWormHead"))
                                            {
                                                damage *= 3;
                                            }
                                            {
                                                if (target.type == mod.NPCType("DarkseaAngler"))
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CavernCrystal>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}