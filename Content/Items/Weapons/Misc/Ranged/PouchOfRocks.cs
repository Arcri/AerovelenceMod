using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class PouchOfRocks : ModItem 
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.useTime = Item.useAnimation = 35;
            Item.shootSpeed = 16;
            Item.knockBack = 3;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<PouchOfRocksProj>();

            Item.width = 58;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 55, 40);
            Item.rare = ItemRarities.EarlyPHM;

            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Shoot;
        }
    }

    public class PouchOfRocksProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
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