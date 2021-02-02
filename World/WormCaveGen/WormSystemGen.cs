using AerovelenceMod.World.WormCaveGen.WormCaveGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.World.Generation;


namespace AerovelenceMod.World.WormCaveGen {
	/// <summary>
	/// Represents all of the data of a worm cave system, painting it finally into the world when `PaintNodes` is
	/// called.
	/// </summary>
	public abstract partial class WormSystemGen : IEnumerable<WormNode> {
		public int NodeCount => this.Nodes.Count;


		////////////////

		protected IList<WormNode> Nodes = new List<WormNode>();
		protected int OriginTileX;
		protected int OriginTileY;



		////////////////

		public WormSystemGen(
					GenerationProgress progress,
					float thisProgress,
					float postProcessProgress,
					IList<WormGen> worms ) {
			ISet<WormGen> wormGenSet = new HashSet<WormGen>( worms );

			this.GenerateNodes( progress, thisProgress, wormGenSet );

			if( this.PostProcessNodes(progress, postProcessProgress, out wormGenSet) ) {
				this.GenerateNodes( progress, postProcessProgress, wormGenSet );
			}
		}

		////

		private void GenerateNodes( GenerationProgress progress, float thisProgress, ISet<WormGen> worms ) {
			int maxNodes = worms.Max( wg => wg.CalculateFurthestKeyNode() );
			float progStep = thisProgress / (float)maxNodes;

			do {
				foreach( WormGen worm in worms.ToArray() ) {
					if( !worm.GenerateNextKeyNode(this, out WormGen fork) ) {
						worms.Remove( worm );
						continue;
					}

					if( fork != null ) {
						worms.Add( fork );
					}

					IList<WormNode> interpNodes = worm.CreateInterpolatedNodesFromRecentNodes();
					if( interpNodes.Count > 0 ) {
						this.Nodes = this.Nodes.Union( interpNodes ).ToList();
					}
				}

				progress.Value += progStep;
			} while( worms.Count > 0 );
		}


		////////////////

		IEnumerator IEnumerable.GetEnumerator() => this.Nodes.GetEnumerator();

		public IEnumerator<WormNode> GetEnumerator() => this.Nodes.GetEnumerator();


		////////////////
		
		protected virtual bool PostProcessNodes(
					GenerationProgress progress,
					float postProcessProgress,
					out ISet<WormGen> newWorms ) {
			newWorms = null;
			return false;
		}
	}
}