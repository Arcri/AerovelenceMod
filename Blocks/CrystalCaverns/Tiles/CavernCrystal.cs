using Terraria;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using AerovelenceMod.Dusts;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
    public class CavernCrystal : ModTile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 40;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
            Main.tileMerge[Type][mod.TileType("CavernCrystal")] = true;
            Main.tileMerge[Type][mod.TileType("CavernStone")] = true;
            Main.tileMerge[Type][mod.TileType("FieldStone")] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(115, 230, 250));
            dustType = 59;
            soundType = SoundID.Tink;
            drop = ModContent.ItemType<CavernCrystalItem>();
        }
        /*public override void NearbyEffects(int i, int j, bool closer)
        {
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;

            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin(oneHelixRevolutionInUpdateTicks) * piFraction);
            Lighting.AddLight(new Vector2(i + 0.5f, j + 0.5f) * 16, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);
            if (Main.rand.Next(40) == 0)
            {
                Dust.NewDustPerfect(new Vector2(i + Main.rand.NextFloat(), j + Main.rand.NextFloat()) * 16, ModContent.DustType<Charge>(), new Vector2(0, -Main.rand.NextFloat(4, 6)), 0, default, Main.rand.NextFloat(0.4f, 0.7f));
            }
        }*/
        
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.6f;
            b = 0.9f;
        }
    }
}