using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class MutilatingEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutilating Edge");
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.crit = 20;
            Item.damage = 17;
            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
            Item.height = 36;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 65, 20);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddRecipeGroup("IronBar", 10)
                .AddIngredient(ItemID.Vertebrae, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.type == NPCID.Crimera)
            {
                damage *= 3;
            }
            if (target.type == -22)
            {
                damage *= 3;
            }
            if (target.type == -23)
            {
                damage *= 3;
            }
        }
    }
}