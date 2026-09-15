using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class OrichalcumCluster : RareOreCluster
    {
        public override int RewardTier => 7;
        protected override int SpriteWidth => 28;
        protected override int SpriteHeight => 26;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Orichalcum", EnglishTooltip)
                .AddName(Language.Spanish, "Superoricalco")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
