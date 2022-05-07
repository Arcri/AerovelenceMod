using Terraria.ModLoader;

namespace AerovelenceMod.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind ArmorHotKey;

        public override void Load()
        {
            ArmorHotKey = KeybindLoader.RegisterKeybind(Mod, "Armor Set Bonus", "F");
        }

        public override void Unload()
        {
            ArmorHotKey = null;
        }
    }
}