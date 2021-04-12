using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class AstralShot : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Shot");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 18, 30);

            item.crit = 4;
            item.damage = 9;
            item.knockBack = 1;

            item.useTime = item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;

            item.ranged = true;
            item.noMelee = true;

            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Arrow;
            item.shoot = ProjectileID.WoodenArrowFriendly;

            item.UseSound = SoundID.Item5;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.dayTime)
            {
                type = ProjectileID.FireArrow;
            }
            if (!Main.dayTime)
            {
                type = ProjectileID.FrostburnArrow;
            }

            return true;
        }
    }
}
