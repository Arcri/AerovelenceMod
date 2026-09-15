using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class TitaniumSuperCluster : RareOreCluster
    {
        public override int RewardTier => 10;
        protected override int SpriteWidth => 38;
        protected override int SpriteHeight => 40;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Giant Super Titanium", EnglishTooltip)
                .AddName(Language.Spanish, "Supertitanio Gigante")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
