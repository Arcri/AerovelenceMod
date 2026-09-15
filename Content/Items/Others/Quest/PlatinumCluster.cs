using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class PlatinumCluster : RareOreCluster
    {
        public override int RewardTier => 4;
        protected override int SpriteWidth => 20;
        protected override int SpriteHeight => 22;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Platinum", EnglishTooltip)
                .AddName(Language.Spanish, "Superplatino")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
