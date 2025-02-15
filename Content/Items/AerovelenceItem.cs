using AerovelenceMod.Common.Utilities;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items
{
    public abstract class AerovelenceItem : ModItem
    {
        public override string Texture
        {
            get
            {
                if (ModContent.HasAsset(base.Texture))
                    return base.Texture;

                return AerovelenceMod.PLACEHOLDER_TEXTURE;
            }
        }

        public sealed override void SetDefaults()
        {
			bool shouldAutosize = true;
            SafeSetDefaults(ref shouldAutosize);
			if (shouldAutosize)
				Item.Autosize();
        }

        public virtual void SafeSetDefaults(ref bool autoSize) { }
    }
}
