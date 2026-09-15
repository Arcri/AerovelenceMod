using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class TungstenCluster : RareOreCluster
    {
        public override int RewardTier => 3;
        protected override int SpriteWidth => 20;
        protected override int SpriteHeight => 26;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Tungsten", EnglishTooltip)
                .AddName(Language.Spanish, "Supertungsteno")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
