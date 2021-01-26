using AerovelenceMod.Items.Placeable.Terminal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.Anomaly.AnomalousTechnology
{
    public class AtmosphericDivergenceTeleporter : ModTile
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
            drop = ModContent.ItemType<AtmosphericDivergenceTeleporterTile>();

            AddMapEntry(new Color(076, 235, 228), Language.GetText("Terminal Brick")); // localized text for "Metal Bar"

        }
    }
}