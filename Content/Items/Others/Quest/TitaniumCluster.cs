using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class TitaniumCluster : RareOreCluster
    {
        public override int RewardTier => 8;
        protected override int SpriteWidth => 32;
        protected override int SpriteHeight => 32;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Titanium", EnglishTooltip)
                .AddName(Language.Spanish, "Supertitanio")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
