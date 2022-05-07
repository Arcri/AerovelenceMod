using AerovelenceMod.Content.Items.Placeables.MusicBoxes;
using AerovelenceMod.Content.Tiles.MusicBoxes;
using Terraria.Audio;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems
{
    public class MusicLoaderSystem : ModSystem
    {
        public override void Load()
        {
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCaverns"), ModContent.ItemType<CrystalCavernsBoxItem>(), ModContent.TileType<CrystalCavernsBox>());
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalTumbler"), ModContent.ItemType<CrystalTumblerBoxItem>(), ModContent.TileType<CrystalTumblerBox>());
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Snowrium"), ModContent.ItemType <SnowriumBoxItem>(), ModContent.TileType<SnowriumBox>());
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Cyvercry"), ModContent.ItemType <CyvercryBoxItem>(), ModContent.TileType<CyvercryBox>());
        }
    }
}