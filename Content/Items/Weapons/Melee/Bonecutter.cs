using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
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
            Item.crit = 5;
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.useTurn = true;
            Item.autoReuse = true;
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
            CreateRecipe(1)
                .AddRecipeGroup("AerovelenceMod:GoldBars", 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}