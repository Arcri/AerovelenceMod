using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class LeadCluster : RareOreCluster
    {
        public override int RewardTier => 2;
        protected override int SpriteWidth => 22;
        protected override int SpriteHeight => 23;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Lead", EnglishTooltip)
                .AddName(Language.Spanish, "Superplomo")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
