using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration
{
	class WorldGenTutorialWorld : ModWorld
	{
		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdate()
		{
			// if (JustPressed(Keys.D1))
			// TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
		}

		private void TestMethod(int x, int y)
		{
			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

			// Code to test placed here:
			WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), TileID.CobaltBrick);

		}

		/*
		float width = Main.rand.NextFloat(2f, 8f);
		float angle = Main.rand.NextFloat(MathHelper.TwoPi);

		WorldUtils.Gen(point, new Spike(width, angle, 3 * width), Actions.Chain(
		new Actions.SetTile(Terraria.ID.TileID.RubyGemspark)
			));
			WorldUtils.Gen(point, new Spike(width, angle, 3 * width), Actions.Chain(
		new Actions.SetFrames(),
		new SpikeFrame()
		)); */


		public class Spike : GenShape
		{
			private readonly float _width;

			private Vector2 _endOffset;

			public Spike(float width, float angle = 0, float speed = 10f)
			{
				_width = width;

				_endOffset = angle.ToRotationVector2() * speed * 16f;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				Vector2 tileOrigin = new Vector2(origin.X << 4, origin.Y << 4);
				return PlotTileTale(tileOrigin, tileOrigin + _endOffset, _width,
					(int x, int y) => UnitApply(action, origin, x, y) || !_quitOnFail);
			}

			private bool PlotTileTale(Vector2 start, Vector2 end, float width, Utils.PerLinePoint plot)
			{
				float halfWidth = width / 2f;

				Point pEnd = end.ToTileCoordinates();
				Point pStart = start.ToTileCoordinates();

				int totalLength = -1,
					currentLength = 0;

				Utils.PlotLine(pStart, pEnd, delegate
				{
					totalLength++;
					return (true);
				});

				if (totalLength <= 0)
				{
					return (false);
				}

				return Utils.PlotLine(pStart, pEnd, (x, y) =>
				{
					float scaleFactor = 1f - (float)currentLength / (float)totalLength;

					PlaceBlotch(x, y, halfWidth * scaleFactor, plot);

					return (++currentLength <= totalLength);
				});
			}

			private void PlaceBlotch(int genX, int genY, float width, Utils.PerLinePoint plot)
			{
				Vector2 origin = new Vector2(genX, genY);

				for (int x = genX - (int)width - 1; x < genX + (int)width + 1; ++x)
				{
					for (int y = genY - (int)width - 1; y < genY + (int)width + 1; ++y)
					{
						if (Vector2.Distance(origin, new Vector2(x, y)) >= width)
						{
							continue;
						}

						if (Main.tile[x, y] == null)
							Main.tile[x, y] = new Tile();
						plot(x, y);
					}
				}
			}
		}

		public class SpikeFrame : GenAction
		{
			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (!WorldGen.InWorld(x, y, 5))
				{
					return (false);
				}

				Tile top = Framing.GetTileSafely(x, y - 1);
				Tile bottom = Framing.GetTileSafely(x, y + 1);

				Tile left = Framing.GetTileSafely(x - 1, y);
				Tile right = Framing.GetTileSafely(x + 1, y);

				if (!top.active() && !bottom.active() && !left.active() && !right.active())
				{
					Main.tile[x, y].active(false);
					return UnitApply(origin, x, y, args);
				}

				Tile.SmoothSlope(x, y);

				return UnitApply(origin, x, y, args);
			}
		}
	}
}