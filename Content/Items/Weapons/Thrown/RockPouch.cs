using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class RockPouch : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock Pouch");
            Tooltip.SetDefault("Throws rocks");
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
            Item.shoot = Mod.Find<ModProjectile>("RockPouchProj").Type;
            Item.shootSpeed = 16f;
        }
    }

    public class RockPouchProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            Projectile.Kill();
            return true;
        }
    }
}