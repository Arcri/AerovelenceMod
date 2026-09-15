using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class AdamantiteSuperCluster : RareOreCluster
    {
        public override int RewardTier => 10;
        protected override int SpriteWidth => 38;
        protected override int SpriteHeight => 36;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Giant Super Adamantite", EnglishTooltip)
                .AddName(Language.Spanish, "Superadamantita Gigante")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
