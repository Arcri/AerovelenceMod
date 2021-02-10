using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class Bonecutter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonecutter");
            Tooltip.SetDefault("Does a lot more damage to skeletons");
        }
        public override void SetDefaults()
        {
            item.crit = 5;
            item.damage = 15;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 19;
            item.useAnimation = 19;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.useTurn = true;
            item.autoReuse = true;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if(target.type == NPCID.Skeleton)
            {
                damage *= 2;
            }
        }
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AerovelenceMod:GoldBars", 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}