using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Others.Quest
{
    public class CopperCluster : RareOreCluster
    {
        public override int RewardTier => 1;
        protected override int SpriteWidth => 20;
        protected override int SpriteHeight => 22;
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Super Copper", EnglishTooltip)
                .AddName(Language.Spanish, "Supercobre")
                .AddTooltip(Language.Spanish, SpanishTooltip);
            base.SetStaticDefaults();
        }
    }
}
