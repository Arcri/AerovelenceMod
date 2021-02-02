using AerovelenceMod.World.WormCaveGen.WormCaveGen;
using System;
using System.Collections.Generic;
using Terraria;


namespace AerovelenceMod.World.WormCaveGen.CrystalCaves
{
    /// <summary>
    /// Represents a crystal cave "puddle" cave path (and its associated forks). Uses `Create` as its primary
    /// factory method.
    /// </summary>
    public partial class CrystalCavePuddleGen : CrystalCaveGen {
		public static CrystalCavePuddleGen Create(
					WormNode sourceNode,
					int tileX,
					int tileY,
					int length,
					int forkCount = 0 ) {
			var randForks = new List<WormGen>( forkCount );

			for( int i = 0; i < forkCount; i++ ) {
				int randLen = WorldGen.genRand.Next(
					Math.Max(length / 2, 3),
					Math.Max(length, 4)
				);

				var fork = CrystalCaveGen.Create( 0, 0, randLen, 0 );

				randForks.Add( fork );
			}

			return new CrystalCavePuddleGen( sourceNode, tileX, tileY, length, randForks );
		}



		////////////////

		protected WormNode SourceNode;



		////////////////

		protected CrystalCavePuddleGen( WormNode sourceNode, int tileX, int tileY, int length, IList<WormGen> forks )
					: base(
						tileX: tileX,
						tileY: tileY,
						length: length,
						forks: forks,
						minRadius: sourceNode.TileRadius * 2,
						maxRadius: (sourceNode.TileRadius * 2) + 1,
						starterNodeCount: 0,
						finisherNodeCount: length
					) {
			this.SourceNode = sourceNode;
		}


		////////////////

		protected override float GaugeCrystalCaveNode(
					WormSystemGen wormSys,
					WormNode testNode,
					WormNode prevNode,
					float tilePadding ) {
			float gauged = base.GaugeCrystalCaveNode( wormSys, testNode, prevNode, tilePadding );
			
			// downward is best
			float vertGauge = (testNode.TileY - prevNode.TileY) > 0f
				? 0 : 100000f;
			// closest to center is best
			float horizGauge = Math.Abs( prevNode.TileX - testNode.TileX );
			horizGauge *= 10;

			return gauged + vertGauge + horizGauge;
		}


		////////////////

		public override void PostPaintTile( WormNode node, int i, int j ) {
			//if( this.KeyNodes[0] == node ) {
			//	return;
			//}

			Main.tile[i, j].liquid = 255;
			Main.tile[i, j].liquidType( 0 );
		}
	}
}