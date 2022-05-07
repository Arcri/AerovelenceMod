using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
			Item.UseSound = SoundID.Item1;
			Item.crit = 20;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 72;
            Item.useTime = 24;
			Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.FrostBoltSword;
            Item.shootSpeed = 8f;
            Item.staff[Item.type] = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        int rand1 = Main.rand.Next(-50, 50);
            Vector2 adjPos = player.Center + new Vector2(rand1, -700);
            Vector2 toMouse = Main.MouseWorld - adjPos;
            toMouse.Normalize();

            for (int i = 0; i < Main.rand.Next(1, 10); i++)
            {
                Projectile.NewProjectileDirect(source, adjPos, (toMouse * 15) + new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f)), ProjectileID.FrostBoltSword, damage, 2f, player.whoAmI);
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<FrostShard>(), 15)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
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
            if (projectile.type == ProjectileID.FrostBoltSword && Main.player[Main.myPlayer].GetModPlayer<AeroPlayer>().Player.HeldItem.type == ModContent.ItemType<Avalanche>())
                target.immune[projectile.owner] = 0;
        }
    }
}