using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class PhanticCluster : RareOreCluster
    {
        public override int RewardTier => 5;
        protected override int SpriteWidth => 20;
        protected override int SpriteHeight => 26;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Phantic", EnglishTooltip)
                .AddName(Language.Spanish, "SuperPhantic")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
