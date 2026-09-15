using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class SilverCluster : RareOreCluster
    {
        public override int RewardTier => 3;
        protected override int SpriteWidth => 26;
        protected override int SpriteHeight => 18;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Silver", EnglishTooltip)
                .AddName(Language.Spanish, "Superplata")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
