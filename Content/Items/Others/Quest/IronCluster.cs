using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class IronCluster : RareOreCluster
    {
        public override int RewardTier => 2;
        protected override int SpriteWidth => 22;
        protected override int SpriteHeight => 22;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Iron", EnglishTooltip)
                .AddName(Language.Spanish, "Superhierro")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
