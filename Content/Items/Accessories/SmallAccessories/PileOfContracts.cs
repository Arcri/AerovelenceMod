using AerovelenceMod.Common.Systems.Language;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class PileOfContracts : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Pile of Contracts", "+4% summon damage\nYour minions are contractually obligated to crit\nMinion critical strikes now deal 5% more than base damage");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 35);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.04f;
            player.GetModPlayer<PileOfContractsPlayer>().Equipped = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 15)
                .AddIngredient(ItemID.BlackInk)
                .AddCondition(Condition.NearWater)
                .Register();
        }
    }

    public class PileOfContractsPlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects()
        {
            Equipped = false;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Equipped || proj.owner != Player.whoAmI)
                return;

            bool minionAttack = proj.IsMinionOrSentryRelated && !proj.sentry && !ProjectileID.Sets.SentryShot[proj.type];
            if (!minionAttack)
                return;

            modifiers.SetCrit();
            modifiers.CritDamage = new StatModifier(1.05f, 1f);
        }
    }
}