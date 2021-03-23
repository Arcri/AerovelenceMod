using Terraria.ModLoader;

namespace AerovelenceMod.meme
{
    public class ExamplePlayer : ModPlayer
    {
        public bool ExampleSentry;
        public override void Initialize()
        {
            ExampleSentry = false;
        }
        public override void ResetEffects()
        {
            ExampleSentry = false;
        }
    }
}
