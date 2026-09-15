using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class TinCluster : RareOreCluster
    {
        public override int RewardTier => 1;
        protected override int SpriteWidth => 24;
        protected override int SpriteHeight => 20;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Tin", EnglishTooltip)
                .AddName(Language.Spanish, "Superestaño")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
