using AerovelenceMod.Shields;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Systems.ShieldSystem
{
	public class ShieldTestItem : ModItem
	{
		public override string Texture => "Item_" + ItemID.GhostMask;

		public override void SetDefaults()
		{
			item.useTime = item.useAnimation = 30; // Swings once every 30 frames
			item.Size = new Vector2(20, 20); // The hitbox for this weapon if it swings
			item.autoReuse = false;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<ShieldPlayer>().SetShield(new KillShield(player));
			return true;
		}
	}
}