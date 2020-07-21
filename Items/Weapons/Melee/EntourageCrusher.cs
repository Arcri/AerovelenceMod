using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class EntourageCrusher : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Entourage Crusher");
		}
        public override void SetDefaults()
        {
			item.value = Item.sellPrice(0, 25, 0, 0);
			item.useTurn = true;
			item.UseSound = SoundID.Item1;
			item.crit = 34;
            item.damage = 120;
            item.ranged = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 7;
			item.useAnimation = 7;
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