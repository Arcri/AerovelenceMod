using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Melee
{
    public class SlicerOfSouls : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slicer of Souls");
		}
        public override void SetDefaults()
        {
			item.useTurn = true;
			item.crit = 20;
            item.damage = 14;
            item.melee = true;
            item.width = 32;
            item.height = 32;
            item.useTime = 10;
            item.useAnimation = 10;
			item.UseSound = SoundID.Item1;
            item.useStyle = 1;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 63, 20);
            item.rare = 2;
            item.autoReuse = true;
        }
		public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if(target.type == 6)
            {
                damage = damage * 3;
            }
			{
			if(target.type == -11)
            {
                damage = damage * 3;
            }
						{
			if(target.type == -12)
            {
                damage = damage * 3;
            }
        }
    }
}
	}
}