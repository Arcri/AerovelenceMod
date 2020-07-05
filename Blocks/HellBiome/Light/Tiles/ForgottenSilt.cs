using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using AerovelenceMod.Items.Placeable.FrostDungeon;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Dusts;

namespace AerovelenceMod.Blocks.HellBiome.Light.Tiles
{
    public class ForgottenSilt : ModTile
    {


        private int t;

        public override void SetDefaults()
        {
            mineResist = 2.5f;
            minPick = 80;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
            dustType = 59;
            soundType = SoundID.Tink;
            drop = ModContent.ItemType<SubzeroBrickItem>();

            AddMapEntry(new Color(076, 235, 228), Language.GetText("Forgotten Silt")); // localized text for "Metal Bar"

        }

        public override void FloorVisuals(Player player)
        {
            Vector2 playerPosition = player.Center + new Vector2(-7, player.height / 3);
            if (t % 50 == 0)
            {
                Dust.NewDust(playerPosition, 16, 1, ModContent.DustType<WhiteSoul>(), 0, 0.2f);
            }
        }
    }
}