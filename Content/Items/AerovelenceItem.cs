using AerovelenceMod.Common.Utilities;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items
{
    public abstract class AerovelenceItem : ModItem
    {
        public bool Autosize;

        public override string Texture
        {
            get
            {
                if (ModContent.HasAsset(base.Texture))
                    return base.Texture;

                return AerovelenceMod.PLACEHOLDER_TEXTURE;
            }
        }

        public override void SetDefaults()
        {
            if (Autosize)
                Item.Autosize();

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }
    }
}
