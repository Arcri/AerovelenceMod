using System;
using System.Collections.Generic;
using Terraria;


namespace AerovelenceMod.World
{
	public partial class CrystalCaveGen : WormGen {
		public override int CalculateFurthestKeyNode() {
			int largest = this.TotalNodes;
			foreach( KeyValuePair<int, WormGen> kv in this._Forks ) {
				if( (kv.Key + kv.Value.TotalNodes) > largest ) {
					largest = kv.Key + kv.Value.TotalNodes;
				}
			}

			return largest;
		}


		protected override void CalculateNextRadiusAndNodeSpacing( out int radius, out int nodeSpacing ) {
			int startRange = this.StartNodeCount;   //2
			int endRange = this.EndNodeCount;   //5
			int remainingNodes = this.TotalNodes - this.KeyNodes.Count;

			nodeSpacing = WorldGen.genRand.Next( this.MinRadius, this.MaxRadius );
			
			// taper at end
			if( this.KeyNodes.Count >= (this.TotalNodes - endRange) ) {
				float radStep = (float)this.MinRadius / (float)remainingNodes;
				int steps = (endRange + 1) - remainingNodes;

				radius = this.MinRadius - (int)(radStep * (float)steps);  // taper
				return;
			}

			// start fat
			if( startRange > 0 && this.KeyNodes.Count == 0 ) {
				radius = WorldGen.genRand.Next( // start fat
					(int)( (float)this.MaxRadius * 1.25f ),
					(int)( (float)this.MaxRadius * 2f )
				);
				radius /= this.KeyNodes.Count + 1;
				return;
			}

			if( startRange > 0 && this.KeyNodes.Count <= startRange ) {
				int max = this.KeyNodes[ this.KeyNodes.Count - 1 ].TileRadius;
				int min = this.MaxRadius;
				float step = (float)(max - min) / (float)startRange;

				radius = max - (int)(step * this.KeyNodes.Count);
				nodeSpacing = radius;
				return;
			}

			radius = nodeSpacing;
		}


		////////////////

		protected virtual float GaugeCrystalCaveNode(
					WormSystemGen wormSys,
					WormNode testNode,
					WormNode prevNode,
					float tilePadding ) {
			float gauged = 0f;

			foreach( WormNode existingNode in wormSys ) {
				float value = (float)existingNode.GetDistance( testNode );

				value -= existingNode.TileRadius + testNode.TileRadius + tilePadding;
				if( value < 0f ) {
					value = 100000 - value;	// too close penalty
				}

				gauged += value;
			}

			return gauged / (float)wormSys.NodeCount;
		}
	}
}
