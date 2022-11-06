using AerovelenceMod.Common.Globals.SkillStrikes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
	public class WarBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("War Bow");
			Tooltip.SetDefault("Hold to charge the bow, increasing damage and velocity\nSkill Strike by releasing with perfect timing");
		}

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.rare = ItemRarityID.Yellow;
			Item.width = 58;
			Item.height = 20;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shootSpeed = 15f;
			Item.knockBack = 6f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.UseSound = SoundID.DD2_PhantomPhoenixShot;
			Item.useAmmo = AmmoID.Arrow;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(8, 0);
		}

        //public static Vector2[] randomSpread(float speedX, float speedY, int angle, int num)
        //{
        /*
        var posArray = new Vector2[num];
        float spread = (float)(angle * 0.0555);
        float baseSpeed = (float)System.Math.Sqrt(speedX * speedX + speedY * speedY);
        double baseAngle = System.Math.Atan2(speedX, speedY);
        double randomAngle;
        for (int i = 0; i < num; ++i)
        {
            randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
            posArray[i] = new Vector2(baseSpeed * (float)System.Math.Sin(randomAngle), baseSpeed * (float)System.Math.Cos(randomAngle));
        }
        return posArray;
        */
        //}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            return false;
        }

    }
}