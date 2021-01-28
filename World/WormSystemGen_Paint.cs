using System;
using System.Collections.Generic;
using Terraria;
using Terraria.World.Generation;


namespace AerovelenceMod.World
{
	public abstract partial class WormSystemGen : IEnumerable<WormNode> {
		public void PaintNodes( GenerationProgress progress, float thisProgress ) {
			float progressUnit = (float)thisProgress / (float)this.Nodes.Count;

			for( int i = 0; i < this.Nodes.Count; i++ ) {
				if( this.Nodes[i].TileRadius <= 1 ) {
					continue;
				}

				this.Nodes[i].Paint(
					r => Math.Max( r * 2, 16 ),
					this.PaintTileOuter
				);
				progress.Value += progressUnit * 0.5f;
			}

			for( int i = 0; i < this.Nodes.Count; i++ ) {
				if( this.Nodes[i].TileRadius <= 1 ) {
					continue;
				}

				this.Nodes[i].Paint(
					r => r,
					this.PaintTileInner
				);
				progress.Value += progressUnit * 0.5f;
			}

			this.PostProcessPaintedNodes();
		}


		////////////////

		protected abstract bool PaintTileInner( int i, int j, float percToEdge );

		protected abstract bool PaintTileOuter( int i, int j, float percToEdge );


		////////////////

		protected virtual void PostProcessPaintedNodes( ) { }
	}
}