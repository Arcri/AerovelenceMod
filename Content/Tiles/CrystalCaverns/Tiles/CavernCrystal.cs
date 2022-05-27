using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles
{
    public class CavernCrystal : ModTile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetStaticDefaults()
        {
            MineResist = 2.5f;
            MinPick = 40;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CrystalGrass").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernCrystal").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("CavernStone").Type] = true;
            Main.tileMerge[Type][Mod.Find<ModTile>("FieldStone").Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(115, 230, 250));
            DustType = 59;
            HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<Items.Placeables.Blocks.CavernCrystal>();
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
