using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class DefendersWave : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defender's Wave");
            Tooltip.SetDefault("Cuts a hole through space-time\nWeapon's damage is multiplied by 0.25% the amount of defense you have");
        }
        public override void SetDefaults()
        {
            Item.crit = 4;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }
    }
}