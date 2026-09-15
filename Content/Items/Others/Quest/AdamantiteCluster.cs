using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class AdamantiteCluster : RareOreCluster
    {
        public override int RewardTier => 8;
        protected override int SpriteWidth => 30;
        protected override int SpriteHeight => 30;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Adamantite", EnglishTooltip)
                .AddName(Language.Spanish, "Superadamantita")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
