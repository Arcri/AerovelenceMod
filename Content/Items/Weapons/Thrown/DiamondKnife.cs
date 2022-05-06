using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class DiamondKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diamond Knife");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 32;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = Mod.Find<ModProjectile>("DiamondKnifeProjectile").Type;
            Item.shootSpeed = 16f;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.Diamond, 3)
                .AddRecipeGroup("IronBar", 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DiamondKnifeProjectile : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            SoundEngine.PlaySound(SoundID.Item10);
            return true;
        }
        public override void AI()
        {
            i++;
            if (i % 2 == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, 132);
            }
            Projectile.alpha += 2;
            Projectile.velocity *= 0.99f;
            if (!e)
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
                e = true;
            }
            Projectile.rotation += rot;
            rot *= 0.99f;
        }
    }
}