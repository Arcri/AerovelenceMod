﻿using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
	public class DarknessDischarge : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Magic;
			Item.damage = 30;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Pink;

			Item.shoot = ModContent.ProjectileType<CyverExplosionBall>();
			Item.shootSpeed = 10; //2

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 29; 
			Item.useAnimation = 29; 
			//Item.UseSound = SoundID.;
			Item.noMelee = true;
			
		}

        public override Vector2? HoldoutOffset()
        {
			return new Vector2(-8, 0);
        }

        public override void AddRecipes()
		{

		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			for (int i = 0; i < 6; i++)
            {
				int crossBombIndex = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<CyverExplosionBall>(), damage / 4, 2, Main.myPlayer);
				Projectile Bomb = Main.projectile[crossBombIndex];

				if (Bomb.ModProjectile is CyverExplosionBall BombadelaCross)
				{
					BombadelaCross.outVector = new Vector2(0, 450).RotatedBy(MathHelper.ToRadians(60 * i));
				}
			}


			return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/DarknessDischarge_Glow").Value;
			GlowmaskUtilities.DrawItemGlowmask(spriteBatch, texture, this.Item, rotation, scale);

		}
	}
}