using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Terraria;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration.WormCaveGen {
	/// <summary>
	/// A specific node within a worm cave. Generates a node as a sphere when called to `Paint` itself into the
	/// world.
	/// </summary>
	public class WormNode {
		public int TileX;
		public int TileY;
		public int TileRadius;
		public int NodeSpacing;

		public IDictionary<int, ISet<int>> PaintedTiles = new ConcurrentDictionary<int, ISet<int>>();
		public IDictionary<int, ISet<int>> PaintedTileEdges = new ConcurrentDictionary<int, ISet<int>>();

		protected WormGen Parent;



		////////////////

		public WormNode( int tileX, int tileY, int tileRadius, int nodeSpacing, WormGen parent ) {
			this.TileX = tileX;
			this.TileY = tileY;
			this.TileRadius = tileRadius;
			this.NodeSpacing = nodeSpacing;
			this.Parent = parent;
		}

		public double GetDistance( WormNode node ) {
			int diffX = node.TileX - this.TileX;
			int diffY = node.TileY - this.TileY;
			return Math.Sqrt( ( diffX * diffX ) + ( diffY * diffY ) );
		}


		////////////////

		public virtual void Paint( Func<float, float> scale, Func<int, int, float, bool> painter ) {
			float rad = scale( this.TileRadius );
			int radSqr = (int)( rad * rad );
			int minX = this.TileX - (int)rad;
			int maxX = this.TileX + (int)rad;
			int minY = this.TileY - (int)rad;
			int maxY = this.TileY + (int)rad;

			for( int i = minX; i < maxX; i++ ) {
				for( int j = minY; j < maxY; j++ ) {
					int xDist = i - this.TileX;
					int yDist = j - this.TileY;
					int distSqr = ( xDist * xDist ) + ( yDist * yDist );

					if( distSqr > radSqr ) {
						if( j >= this.TileY ) {
							break;
						} else {
							continue;
						}
					}

					if( i >= 0 && i < Main.maxTilesX && j >= 0 && j < Main.maxTilesY ) {
						float perc = (float)distSqr / (float)radSqr;

						if( painter(i, j, perc) ) {
							if( !this.PaintedTiles.ContainsKey(i) ) {
								this.PaintedTiles[i] = new HashSet<int>();
							}
							this.PaintedTiles[i].Add( j );

							this.Parent.PostPaintTile( this, i, j );
						}
					}
				}
			}
		}
	}
}