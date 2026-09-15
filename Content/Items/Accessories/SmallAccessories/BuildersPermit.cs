using AerovelenceMod.Common.Systems.Language;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    public class BuildersPermit : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Builder's Permit", "+50% building and placement speed\n+1 max sentry");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 28;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.equippedAnyTileSpeedAcc = true;
            player.equippedAnyWallSpeedAcc = true;
            player.maxTurrets += 1;
        }
    }

    public class BuildersPermitShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
                shop.Add<BuildersPermit>(Condition.DownedSkeletron);
        }
    }
}