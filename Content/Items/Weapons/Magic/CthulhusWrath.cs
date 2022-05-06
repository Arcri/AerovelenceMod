using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class CthulhusWrath : ModItem
    {
		public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
			DisplayName.SetDefault("Cthulhu's Wrath");
			Tooltip.SetDefault("Its angered essence is trapped");
		}
        public override void SetDefaults()
        {
            Item.crit = 7;
            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
			Item.mana = 5;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.UseSound = SoundID.Item20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 5, 30, 0);
			Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Flames;
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.useTime = 70;
				Item.useAnimation = 70;
				Item.damage = 55;
				Item.mana = 0;
			}
			else
			{
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.useTime = 12;
				Item.useAnimation = 12;
				Item.damage = 10;
				Item.mana = 5;
				Item.shoot = ProjectileID.Flames;
				Item.shootSpeed = 12f;
			}
			return base.CanUseItem(player);
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			if (player.altFunctionUse == 2)
			{
				if(player == Main.LocalPlayer)
				{
					player.velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * 10;
				}

				if (player.velocity.X == 0 && player.velocity.Y == 0)
				{
					player.velocity.X += 2;
				}
			}


			return true;
        }


        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = new TooltipLine(Mod, "Verbose:RemoveMe", "Why do I exist, dawg");
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Cthulhu's Wrath", "Artifact weapon")
			{
				overrideColor = new Color(255, 241, 000)
			};
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.OverrideColor = new Color(255, 132, 000);
				}
			}
			tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
		}
    }
}