using System;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration.WormCaveGen.CrystalCaves
{
    /// <summary>
    /// Represents a crystal cave system; a type of worm cave system world gen. Uses `Create` as its primary
    /// factory method.
    /// </summary>
    public partial class CrystalCaveSystemGen : WormSystemGen {
		public const int MinimumLength = 75 + 1;
		public const int MaximumLength = 125;
		public const int PuddleLength = 8;



		////////////////

		public static CrystalCaveSystemGen Create(
					GenerationProgress progress,
					float thisProgress,
					int tileX,
					int tileY ) {
			int totalLength = WorldGen.genRand.Next(
				CrystalCaveSystemGen.MinimumLength,
				CrystalCaveSystemGen.MaximumLength
			);
			int len1 = WorldGen.genRand.Next( 35, totalLength - 35 );
			int len2 = totalLength - len1;

			int totalForks = WorldGen.genRand.Next( 4, 8 );
			int forks1 = WorldGen.genRand.Next( 2, totalForks );
			int forks2 = totalForks - forks1;

			var wormDefs = new List<WormGen> {
				CrystalCaveGen.Create( tileX, tileY, len1, forks1 ),
				CrystalCaveGen.Create( tileX, tileY, len2, forks2 ),
			};

			return new CrystalCaveSystemGen( progress, thisProgress, wormDefs );
		}



		////////////////

		private CrystalCaveSystemGen( GenerationProgress progress, float thisProgress, IList<WormGen> wormDefs )
					: base( progress, thisProgress * 0.75f, thisProgress * 0.25f, wormDefs ) { }


		////////////////

		protected override bool PostProcessNodes(
					GenerationProgress progress,
					float postProcessProgress,
					out ISet<WormGen> newWorms ) {
			WormNode bottomNode = this.FindBestBottomNode();

			newWorms = new HashSet<WormGen> {
				CrystalCavePuddleGen.Create(
					tileX: bottomNode.TileX,
					tileY: bottomNode.TileY,
					length: CrystalCaveSystemGen.PuddleLength,
					forkCount: WorldGen.genRand.Next( 2, 4 ),
					sourceNode: bottomNode
				)
			};
			return true;
		}


		////////////////

		public WormNode FindBestBottomNode() {
			int leftMostX = Main.maxTilesX - 1;
			int rightMostX = 0;
			int topMostY = Main.maxTilesY - 1;
			int bottomMostY = 0;

			foreach( WormNode node in this.Nodes ) {
				if( node.TileX > rightMostX ) {
					rightMostX = node.TileX;
				}
				if( node.TileX < leftMostX ) {
					leftMostX = node.TileX;
				}
				if( node.TileY < topMostY ) {
					topMostY = node.TileY;
				}
				if( node.TileY > bottomMostY ) {
					bottomMostY = node.TileY;
				}
			}

			WormNode bestNode = null;
			float bestValue = 0f;

			float rangeX = rightMostX - leftMostX;
			float rangeY = bottomMostY - topMostY;

			foreach( WormNode node in this.Nodes ) {
				float percX = (float)(node.TileX - leftMostX) / rangeX;
				float percMidX = 0.5f - Math.Abs( 0.5f - percX );
				percMidX *= 2f;

				float percY = (float)(node.TileY - topMostY) / rangeY;

				float value = percMidX + (percY * 2f);
				value += Math.Min( (float)node.TileRadius / (float)CrystalCaveGen.MaxNormalRadius, 1f );

				if( value > bestValue ) {
					bestValue = value;
					bestNode = node;
				}
			}

			return bestNode;
		}
	}
}