using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class CobaltCluster : RareOreCluster
    {
        public override int RewardTier => 6;
        protected override int SpriteWidth => 28;
        protected override int SpriteHeight => 22;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Cobalt", EnglishTooltip)
                .AddName(Language.Spanish, "Supercobalto")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
