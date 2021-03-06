using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class EntourageCrusher : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Entourage Crusher");
            Tooltip.SetDefault("Hitting an enemy causes an energy sweep");
            Tooltip.SetDefault("'Can clear a room in seconds'");
        }
        public override void SetDefaults()
        {
			item.value = Item.sellPrice(0, 25, 0, 0);
			item.useTurn = true;
			item.UseSound = SoundID.Item1;
			item.crit = 34;
            item.damage = 120;
            item.ranged = true;
            item.width = 56;
            item.height = 56;
            item.useTime = 15;
			item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 12;
            item.rare = ItemRarityID.Cyan;
            item.autoReuse = true;
            item.shootSpeed = 8f;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 121; i <= 126; i++)
            {
                int left = Projectile.NewProjectile(target.Center.X, target.Center.Y + target.height / 2, -15, 0, i, item.damage, 0f, Main.myPlayer, 0f, 0f);
                int right = Projectile.NewProjectile(target.Center.X, target.Center.Y + target.height / 2, 15, 0, i, item.damage, 0f, Main.myPlayer, 0f, 0f);
                Main.projectile[left].magic = false;
                Main.projectile[right].magic = false;
                Main.projectile[left].melee = true;
                Main.projectile[right].melee = true;
            }
        }
    }
}