using AerovelenceMod.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalKunai : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Kunai");
            Tooltip.SetDefault("Inflicts frostburn");
		}
        public override void SetDefaults()
        {
			item.UseSound = SoundID.Item1;
			item.crit = 5;
            item.damage = 16;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
			item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("CrystalKunaiProj");
            item.shootSpeed = 8f;
		}
    }
}
	
namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalKunaiProj : ModProjectile
    {
		public bool e;
		public float rot = 0.5f;
		public int i;
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int index = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalKunaiProj2>(), projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
            Main.projectile[index].rotation = (float)(projectile.rotation + 3.14);
            projectile.Kill();
            return true;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Thrown
{
    public class CrystalKunaiProj2 : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.aiStyle = 2;
        }
        public override void AI()
        {
            projectile.velocity *= 0f;
        }
    }
}