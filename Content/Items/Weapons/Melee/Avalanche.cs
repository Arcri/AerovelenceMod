using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Avalanche : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Avalanche");
            Tooltip.SetDefault("Rains down frost balls on your enemies");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 20;
            item.damage = 24;
            item.melee = true;
            item.width = 64;
            item.height = 72;
            item.useTime = 24;
			item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 7, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ProjectileID.FrostBoltSword;
            item.shootSpeed = 8f;
            Item.staff[item.type] = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int rand1 = Main.rand.Next(-50, 50);
            Vector2 adjPos = player.Center + new Vector2(rand1, -700);
            Vector2 toMouse = Main.MouseWorld - adjPos;
            toMouse.Normalize();

            for (int i = 0; i < Main.rand.Next(1, 10); i++)
            {
                Projectile.NewProjectileDirect(adjPos, (toMouse * 15) + new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f)), ProjectileID.FrostBoltSword, damage, knockBack, player.whoAmI);
            }

            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
    public class AvalancheBoltProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private float noCollideTime;
        private bool waterCollideCache;
        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == ProjectileID.FrostBoltSword)
            {
                waterCollideCache = projectile.ignoreWater;
                noCollideTime = 40;
            }
        }
        public override void PostAI(Projectile projectile)
        {
            if (projectile.type == ProjectileID.FrostBoltSword && projectile.active)
            {
                noCollideTime--;

                if (noCollideTime >= 0)
                {
                    projectile.tileCollide = false;
                    projectile.ignoreWater = true;
                }
                else
                {
                    projectile.tileCollide = true;
                    projectile.ignoreWater = waterCollideCache;
                }
            }
            else if (!projectile.active)
            {
                noCollideTime = 40;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.type == ProjectileID.FrostBoltSword && Main.player[Main.myPlayer].GetModPlayer<AeroPlayer>().player.HeldItem.type == ModContent.ItemType<Avalanche>())
                target.immune[projectile.owner] = 0;
        }
    }
}