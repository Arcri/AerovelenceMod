using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using AerovelenceMod.Items.Placeable.Terminal;

namespace AerovelenceMod.Blocks.Terminal
{
    public class TerminalBrick : ModTile
    {
        public override void SetDefaults()
        {
			mineResist = 2.5f;
			minPick = 200;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = false;
			dustType = 59;
			soundType = SoundID.Tink;
			drop = ModContent.ItemType<TerminalBrickItem>();
			
            AddMapEntry(new Color(076, 235, 228), Language.GetText("Terminal Brick")); // localized text for "Metal Bar"

        }
    }
}