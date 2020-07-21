using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AerovelenceMod.Items.BossSummons
{
    public class AncientGeode : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ancient Geode");
			Tooltip.SetDefault("Summons the Crystal Tumbler");
		}
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = 100;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 30;
            item.useTime = 30;
			item.maxStack = 999;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<AeroPlayer>().ZoneCrystalCaverns && !NPC.AnyNPCs(mod.NPCType("CrystalTumbler"));
        }
        public override bool UseItem(Player player)
        {
			{
                NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, mod.NPCType("CrystalTumbler"), 0, 0f, 0f, 0f, 0f, 255);
            }
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            return true;
        }
    }
}
