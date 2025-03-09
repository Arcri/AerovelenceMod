using Steamworks;
using System;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Msc
{
    [AutoloadEquip(EquipType.Head)]
    class JellyfishHat : ModItem
    {
        public static bool isWearingJellyfishHat = false;


        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 1, silver: 10, copper: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BlueJellyfish)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PinkJellyfish)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.GreenJellyfish)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    class JellyfishAuraProjectile : ModProjectile
    {
        int frameCount = 3;
        int animationSpeed = 5;
        public override void SetDefaults()
        {
            AIType = -1;
            Projectile.damage = 10;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 40;
            Projectile.height = 58;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void AI()
        {
            //Main.NewText("working");
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            AnimateProjectile();
        }

        private void AnimateProjectile()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= animationSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % frameCount;
            }

        }
    }
}
