using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Equipment
{
	internal class IronHook : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iron Hook");
		}

		public override void SetDefaults()
		{
			/*
				this.noUseGraphic = true;
				this.damage = 0;
				this.knockBack = 7f;
				this.useStyle = 5;
				this.name = "Amethyst Hook";
				this.shootSpeed = 10f;
				this.shoot = 230;
				this.width = 18;
				this.height = 28;
				this.useSound = 1;
				this.useAnimation = 20;
				this.useTime = 20;
				this.rare = 1;
				this.noMelee = true;
				this.value = 20000;
			*/
			// Instead of copying these values, we can clone and modify the ones we want to copy
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f;
			Item.shoot = ModContent.ProjectileType<IronHookProjectile>();
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ItemID.IronBar, 10)
				.AddIngredient(ItemID.Chain, 5)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	internal class IronHookProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("${ProjectileName.GemHookAmethyst}");
		}

		public override void SetDefaults()
		{
			/*	this.netImportant = true;
				this.name = "Gem Hook";
				this.width = 18;
				this.height = 18;
				this.aiStyle = 7;
				this.friendly = true;
				this.penetrate = -1;
				this.tileCollide = false;
				this.timeLeft *= 10;
			*/
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
		}
		public override bool? CanUseGrapple(Player player)
		{
			int hooksOut = 0;
			for (int l = 0; l < 1000; l++)
			{
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
				{
					hooksOut++;
				}
			}
			if (hooksOut > 1)
			{
				return false;
			}
			return true;
		}
		public override float GrappleRange()
		{
			return 200f;
		}
		public override void NumGrappleHooks(Player player, ref int numHooks)
		{
			numHooks = 1;
		}
		public override void GrappleRetreatSpeed(Player player, ref float speed)
		{
			speed = 11f;
		}
		public override void GrapplePullSpeed(Player player, ref float speed)
		{
			speed = 10;
		}
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
		{
			Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
			float hangDist = 50f;
			grappleX += dirToPlayer.X * hangDist;
			grappleY += dirToPlayer.Y * hangDist;
		}
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Equipment/IronHookChain");
			Vector2 vector = Projectile.Center;
			Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
			Rectangle? sourceRectangle = null;
			Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			float num = texture.Height;
			Vector2 vector2 = mountedCenter - vector;
			float rotation = (float)Math.Atan2(vector2.Y, vector2.X) - 1.57f;
			bool flag = true;
			if (float.IsNaN(vector.X) && float.IsNaN(vector.Y))
			{
				flag = false;
			}
			if (float.IsNaN(vector2.X) && float.IsNaN(vector2.Y))
			{
				flag = false;
			}
			while (flag)
			{
				if (vector2.Length() < num + 1.0)
				{
					flag = false;
				}
				else
				{
					Vector2 value = vector2;
					value.Normalize();
					vector += value * num;
					vector2 = mountedCenter - vector;
					Color color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16.0));
					color = Projectile.GetAlpha(color);
					Main.spriteBatch.Draw(texture, vector - Main.screenPosition, sourceRectangle, color, rotation, origin, 1f, SpriteEffects.None, 0f);
				}
			}
		}
	}
}