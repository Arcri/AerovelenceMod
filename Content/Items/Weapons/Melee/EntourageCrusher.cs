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
			Item.value = Item.sellPrice(0, 25, 0, 0);
			Item.useTurn = true;
			Item.UseSound = SoundID.Item1;
			Item.crit = 34;
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 15;
			Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 12;
            Item.rare = ItemRarityID.Cyan;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 121; i <= 126; i++)
            {
                int left = Projectile.NewProjectile(target.Center.X, target.Center.Y + target.height / 2, -15, 0, i, Item.damage, 0f, Main.myPlayer, 0f, 0f);
                int right = Projectile.NewProjectile(target.Center.X, target.Center.Y + target.height / 2, 15, 0, i, Item.damage, 0f, Main.myPlayer, 0f, 0f);
                // Main.projectile[left].magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                // Main.projectile[right].magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                Main.projectile[left].DamageType = DamageClass.Melee;
                Main.projectile[right].DamageType = DamageClass.Melee;
            }
        }
    }
}