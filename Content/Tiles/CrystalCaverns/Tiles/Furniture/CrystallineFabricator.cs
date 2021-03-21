using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
    public class CrystallineFabricator : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Crystalline Fabricator");
			AddMapEntry(new Color(068, 077, 098), name);
			dustType = ModContent.DustType<Sparkle>();

			animationFrameHeight = 54;

		}
		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frame = frameCounter / 54;
			frameCounter += 6;
			if (frame == 5)
			{
				frame = 0;
				frameCounter = 0;
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            Item.NewItem(i * 16, j * 16, 32, 16, ModContent.ItemType<CrystallineFabricatorItem>());
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 0.75f;
			b = 1f;
		}
    }
}