using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class PalladiumCluster : RareOreCluster
    {
        public override int RewardTier => 6;
        protected override int SpriteWidth => 28;
        protected override int SpriteHeight => 28;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Palladium", EnglishTooltip)
                .AddName(Language.Spanish, "Superpaladio")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
