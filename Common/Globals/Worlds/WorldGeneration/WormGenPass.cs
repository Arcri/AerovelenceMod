using System.Collections.Generic;
using AerovelenceMod.Common.Globals.Worlds.WorldGeneration.WormCaveGen;
using AerovelenceMod.Common.Globals.Worlds.WorldGeneration.WormCaveGen.CrystalCaves;
using Terraria;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Common.Globals.Worlds.WorldGeneration
{
    public class WormGenPass : GenPass 
    {
		public delegate WormNode WormNodeGenerator ( IList<WormNode> nodes, int maxNodes, int originTileX, int originTileY );

        public static int PaddingDistance { get; } = 200;

        public WormGenPass() : base( "Generating Crystal Caverns", 1f ) { }

		public override void Apply( GenerationProgress progress ) 
        {
			progress.Message = "Generating Crystal Caverns";   //Lang.gen[76].Value+"..Thin Ice"

			int pad = PaddingDistance;

			int tileX = WorldGen.genRand.Next( pad, Main.maxTilesX - pad );
			int tileY = WorldGen.genRand.Next( (int)Main.worldSurface + pad, (Main.maxTilesY - 200) - pad );

			WormSystemGen wormSys = CrystalCaveSystemGen.Create( progress, 0.5f, tileX, tileY );
            wormSys.PaintNodes(progress, 0.5f);

            progress.Set(1f);
		}
	}
}