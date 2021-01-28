using System;
using System.Collections.Generic;


namespace AerovelenceMod.World
{
	public abstract partial class WormGen : IEnumerable<WormNode> {
		public static double Lerp( double y1, double y2, double mu ) {
			double mu2;

			mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
			return y1 * (1 - mu2) + y2 * mu2;
		}



		////////////////

		public IList<WormNode> CreateInterpolatedNodesFromRecentNodes() {
			var nodes = new List<WormNode>();
			if( this.KeyNodes.Count < 2 ) {
				return nodes;
			}

			WormNode prevNode = this.KeyNodes[ this.KeyNodes.Count - 2 ];
			WormNode currNode = this.KeyNodes[ this.KeyNodes.Count - 1 ];

			double xDist = currNode.TileX - prevNode.TileX;
			double yDist = currNode.TileY - prevNode.TileY;
			double dist = Math.Sqrt( (xDist*xDist) + (yDist*yDist) );

			double incIntervals = 2d;
			if( dist < incIntervals ) {
				nodes.Add( currNode );
				return nodes;
			}

			for( double i=incIntervals; i<dist; i+=incIntervals ) {
				double perc = i / dist;
				int x = prevNode.TileX + (int)(xDist * perc);
				int y = prevNode.TileY + (int)(yDist * perc);
				int rad = (int)WormGen.Lerp( (double)prevNode.TileRadius, (double)currNode.TileRadius, (double)perc );
				int space = (int)WormGen.Lerp( (double)prevNode.NodeSpacing, (double)currNode.NodeSpacing, (double)perc );

				nodes.Add( new WormNode(x, y, rad, space, this) );
			}

			nodes.Add( currNode );
			return nodes;
		}
	}
}