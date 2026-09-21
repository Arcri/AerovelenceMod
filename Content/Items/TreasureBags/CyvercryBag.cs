using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.Items.Accessories.Boss;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.CyverCannon;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.Oblivion;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.TrojanForce;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
    public class CyvercryBag : TranslatableModItem
    {
        private const string Description = "Right click to open";
        public static int[] Weapons => new[] { ModContent.ItemType<Oblivion>(), ModContent.ItemType<CyverCannon>(), ModContent.ItemType<TrojanForce>() };
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Treasure Bag (Cyvercry)", Description);
            base.SetStaticDefaults();
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }
        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnergyShield>()));
            itemLoot.Add(ItemDropRule.OneFromOptions(1, Weapons));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Content.NPCs.Bosses.Cyvercry.Cyvercry2>()));
        }
    }
}
