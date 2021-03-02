using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class SlicerOfSouls : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slicer of Souls");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.crit = 20;
            item.damage = 14;
            item.melee = true;
            item.width = 28;
            item.height = 36;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 65, 20);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 10);
            recipe.AddIngredient(ItemID.RottenChunk, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.type == NPCID.EaterofSouls)
            {
                damage *= 3;
            }
            {
                if (target.type == -11)
                {
                    damage *= 3;
                }
                {
                    if (target.type == -12)
                    {
                        damage *= 3;
                    }
                }
            }
        }
    }
}