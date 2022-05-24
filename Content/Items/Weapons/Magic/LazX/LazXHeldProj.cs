using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;


namespace AerovelenceMod.Content.Items.Weapons.Magic.LazX
{
	public class LazXHeldProj : ModProjectile
	{

		public ref float Angle => ref projectile.ai[1];


		public int timer;
		public float lerpage = 0.32f;
		private Vector2 mousePos;

		bool targetFound = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laz-X");
		}

		public override void SetDefaults()
		{

			projectile.width = 42;
			projectile.height = 42;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.magic = true;
			projectile.damage = 0;

		}

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
		{
			Player player = Main.player[projectile.owner];


			if (player.dead || !player.active || !player.channel)
				projectile.Kill();

			Vector2 center = player.MountedCenter + (projectile.direction < 0 ? new Vector2(-10, 5) : new Vector2(-18, 5));   

			projectile.Center = center;
			projectile.rotation = projectile.velocity.ToRotation();
			float extrarotate = ((projectile.direction * player.gravDir) < 0) ? MathHelper.Pi : 0;
			float itemrotate = projectile.direction < 0 ? MathHelper.Pi : 0;
			player.itemRotation = projectile.velocity.ToRotation() + itemrotate;
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
			player.ChangeDir(projectile.direction);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 10;
			player.itemAnimation = 10;
			Vector2 HoldOffset = new Vector2(15, 0).RotatedBy(MathHelper.WrapAngle(projectile.velocity.ToRotation()));

			projectile.Center += HoldOffset;
			projectile.spriteDirection = projectile.direction * (int)player.gravDir;
			projectile.rotation -= extrarotate;

			projectile.velocity = Vector2.Lerp(Vector2.Normalize(projectile.velocity),Vector2.Normalize(mousePos - player.MountedCenter), lerpage); //slowly move towards direction of cursor
			projectile.velocity.Normalize();


			if (projectile.owner == Main.myPlayer)
            {
				mousePos = Main.MouseWorld;
			}
            else
            {
				projectile.Center += projectile.velocity * 20;
				return;
			}

			if (player.channel)
            {
				projectile.timeLeft++;
				projectile.Center += projectile.velocity * 20;

			}


			timer++;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Player player = Main.player[projectile.owner];

			Texture2D texture = mod.GetTexture("Content/Items/Weapons/Magic/LazX/LazXL");
			Rectangle yeah = texture.Frame(1, 1, 0, 0);
			Vector2 origin2 = yeah.Size() / 2f;

			Vector2 topLeft = Vector2.Zero;
			Vector2 topRight = Vector2.Zero;
			Vector2 BottomLeft = Vector2.Zero;
			Vector2 BottomRight = Vector2.Zero;
			Vector2 storedPos = Vector2.Zero; //Yes I need to do all this



			for (int npcWho = 0; npcWho < 200; npcWho++)
			{
				NPC npc = Main.npc[npcWho];

				if (npc.CanBeChasedBy() && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                {
					float sqrDistanceToTarget = Vector2.DistanceSquared(npc.Center, Main.MouseWorld);

					if (sqrDistanceToTarget < (150 * 150))
                    {
						targetFound = true;
						int width = Main.npc[npcWho].width;
						int height = Main.npc[npcWho].height;
						Vector2 center = Main.npc[npcWho].Center;
						storedPos = center;

						topLeft = center + new Vector2(width / -2, height / -2);
						topRight = center + new Vector2(width / 2, height / -2);
						BottomLeft = center + new Vector2(width / -2, height / 2);
						BottomRight = center + new Vector2(width / 2, height / 2);
						//Sets the position where we will draw the Ls

					}

				}
			}
			if (timer % 50 == 0 && Main.player[projectile.owner].statMana >= 12 && topLeft.X != 0) //The topleft check is to make sure that if nothing is targeted, it won't fire at 0,0
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
					Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithVolume(2), storedPos);

					Projectile.NewProjectile(storedPos, Vector2.Zero, ModContent.ProjectileType<LazXExplosion>(), player.inventory[player.selectedItem].damage, 2, player.whoAmI);
					Main.player[projectile.owner].statMana -= 12;
				}


				for (int i = 0; i < 6; i++) //A sad and sorry attempt to make dust come out of the gun when shot :/
				{
					//Dust dust = Dust.NewDustDirect(projectile.Center + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 30) + new Vector2(0,-10), 0, 0, DustID.RubyBolt, 0, 0, 0, Color.Red);
					//dust.noGravity = true;
					//dust.scale *= 1.3f;
					//dust.velocity *= 0.5f;
					//dust.velocity += (projectile.velocity * 20) * 0.1f;
				}

			}


			spriteBatch.End(); //Additive Blending my beloved
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
			if (targetFound)
            {
				Main.spriteBatch.Draw(texture, topLeft - Main.screenPosition + new Vector2(0.5f, -0.5f), new Microsoft.Xna.Framework.Rectangle?(yeah), Color.White * 0.7f, MathHelper.ToRadians(90), origin2, 0.6f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture, topRight - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(yeah), Color.White * 0.7f, MathHelper.ToRadians(180), origin2, 0.6f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture, BottomLeft - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(yeah), Color.White * 0.7f, MathHelper.ToRadians(0), origin2, 0.6f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture, BottomRight - Main.screenPosition + new Vector2(-0.5f, .5f), new Microsoft.Xna.Framework.Rectangle?(yeah), Color.White * 0.7f, MathHelper.ToRadians(270), origin2, 0.6f, SpriteEffects.None, 0f);

			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


			return base.PreDraw(spriteBatch, lightColor);
        }

    }
}

