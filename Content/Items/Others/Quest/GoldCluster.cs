using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class GoldCluster : RareOreCluster
    {
        public override int RewardTier => 4;
        protected override int SpriteWidth => 20;
        protected override int SpriteHeight => 20;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Gold", EnglishTooltip)
                .AddName(Language.Spanish, "Superoro")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
