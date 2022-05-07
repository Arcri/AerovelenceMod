using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class PhantomSong : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Song");
            Tooltip.SetDefault("Has a chance to fire a phantom arrow");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.crit = 4;
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 8.5f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Main.rand.Next(new int[] { type, type, Mod.Find<ModProjectile>("PhantomSongArrow") .Type});
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<PhanticBar>(), 8)
                .AddRecipeGroup("AerovelenceMod:EvilMaterials", 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class PhantomSongArrow : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.velocity *= 1.01f;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
        }
        public override void AI()
        {
            i++;
            if (i % 25 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.99f, ModContent.ProjectileType<PhantomSongAura>(), Projectile.damage, Projectile.knockBack);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 60);
            dust.noGravity = true;
            dust.velocity *= 0.1f;
        }
    }

    internal sealed class PhantomSongAura : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 5;
            Projectile.alpha = 3;
            Projectile.timeLeft = 100;
            Projectile.damage = 60;
            Projectile.scale = 1f;

            Projectile.friendly = true;
        }
        public override bool PreAI()
        {
            Projectile.scale *= 0.99f;
            Projectile.velocity *= 0.000001f;
            Vector2 from = Projectile.position;
            for (int i = 0; i < 360; i += 20)
            {
                Vector2 circular = new Vector2(24 * Projectile.scale, 0).RotatedBy(MathHelper.ToRadians(i));
                circular.X *= 0.7f;
                circular = circular.RotatedBy(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
                Vector2 dustVelo = new Vector2(0, 0).RotatedBy(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
                Dust dust = Dust.NewDustDirect(from - new Vector2(5) + circular, 0, 0, 60, 0, 0, Projectile.alpha);
                dust.velocity *= 0.15f;
                dust.velocity += dustVelo;
                dust.noGravity = true;
            }
            return true;
        }
    }
}