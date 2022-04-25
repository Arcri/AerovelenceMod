using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.ShieldSystem;

namespace AerovelenceMod.Content.Items.Shields
{
    public class VoidSuit : ShieldItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("[WIP] Void Suit");
            Tooltip.SetDefault("500 shield capacity\n50 burst recharge rate\n6s recharge delay\nImpact shield\nReflects all projectiles\nReduces contact damage greatly");
        }
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            item.accessory = true;
            item.rare = ItemRarityID.Orange; 
            shield.capacity = 500;
            shield.rechargeRate = 50;
            shield.rechargeDelay = 6;
            shield.type = ShieldTypes.Impact;
            // This shield will protect from all projectiles and almost all contact damage. Recharges in bursts of 100 every 5 seconds.
            // shield.capacity = 500; (Low, attacks do more damage to shield capacity unlike HP)
            // shield.rechargeRate = 100; (Regenerates 100 every 5 seconds, but only in bursts)
            // shield.rechargeDelay = 8; (Measured in seconds)
            // shield.Impact = true;
        }
    }
}
