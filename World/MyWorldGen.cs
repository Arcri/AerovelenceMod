using System;
using System.Collections.Generic;
using Terraria;
using Terraria.World.Generation;


namespace AerovelenceMod.World
{
	public partial class WormGenPass : GenPass {
		public delegate WormNode WormNodeGenerator( IList<WormNode> nodes, int maxNodes, int originTileX, int originTileY );



		////////////////

		public static int PaddingDistance { get; private set; } = 200;



		////////////////

		public WormGenPass() : base( "Generating Snaking Caves", 1f ) { }


		public override void Apply( GenerationProgress progress ) {
			progress.Message = "Generating Snaking Caves";   //Lang.gen[76].Value+"..Thin Ice"

			int pad = WormGenPass.PaddingDistance;
			int tileX = WorldGen.genRand.Next( pad, Main.maxTilesX - pad );
			int tileY = WorldGen.genRand.Next( (int)Main.worldSurface + pad, (Main.maxTilesY - 200) - pad );

			WormSystemGen wormSys = CrystalCaveSystemGen.Create( progress, 0.5f, tileX, tileY );
			wormSys.PaintNodes( progress, 0.5f );

			progress.Set( 1f );
		}
	}
}