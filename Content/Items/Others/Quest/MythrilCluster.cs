using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class MythrilCluster : RareOreCluster
    {
        public override int RewardTier => 7;
        protected override int SpriteWidth => 30;
        protected override int SpriteHeight => 26;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Mythril", EnglishTooltip)
                .AddName(Language.Spanish, "Supermitrilo")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
