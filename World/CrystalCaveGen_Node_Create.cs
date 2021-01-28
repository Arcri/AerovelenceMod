using System;
using System.Collections.Generic;


namespace AerovelenceMod.World {
	public partial class CrystalCaveGen : WormGen {
		protected override WormNode CreateKeyNode( WormSystemGen wormSys ) {
			this.CalculateNextRadiusAndNodeSpacing( out int radius, out int nodeSpacing );

			WormNode newNode;
			if( this.KeyNodes.Count == 0 ) {
				newNode = new WormNode( this.OriginTileX, this.OriginTileY, radius, nodeSpacing, this );
			} else {
				newNode = this.CreateNextCrystalCaveKeyNode( wormSys, radius, nodeSpacing );
			}

			return newNode;
		}


		protected virtual WormNode CreateNextCrystalCaveKeyNode( WormSystemGen wormSys, int radius, int nodeSpacing ) {
			int tests = 14;
			int tilePadding = 8;

			WormNode currNode = this.KeyNodes[ this.KeyNodes.Count - 1 ];

			var testNodes = this.CreateTestNodes( tests, radius, nodeSpacing, currNode );

			WormNode bestNode = null;
			float prevGauged = -1f;
			foreach( WormNode testNode in testNodes ) {
				float gauged = this.GaugeCrystalCaveNode( wormSys, testNode, currNode, tilePadding );
				if( prevGauged != -1 && gauged > prevGauged ) { continue; }

				prevGauged = gauged;
				bestNode = testNode;
			}

			return bestNode;
		}


		////////////////

		protected virtual IList<WormNode> CreateTestNodes( int count, int radius, int nodeSpacing, WormNode currNode ) {
			var testNodes = new List<WormNode>( count );

			for( int i = 0; i < count; i++ ) {
				WormNode testNode = this.CreateTestNode( currNode, radius, nodeSpacing );
				testNodes.Add( testNode );
			}

			return testNodes;
		}
	}
}