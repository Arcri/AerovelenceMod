using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;

namespace AerovelenceMod.Content.Items.Weapons.Aurora
{
    public class Alpenine : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alpenine");
            // Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.crit = 4;
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuroraPillar>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.channel = true;
            Item.shootSpeed = 2f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            /*
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            if (player.direction == -1)
                position += new Vector2(0, 4).RotatedBy(velocity.ToRotation());
            else
                position += new Vector2(0, -4).RotatedBy(velocity.ToRotation());
            Main.NewText(player.direction);
            */
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            
            return true;
        }
        public override void HoldItem(Player player)
        {
            
        }

        public override bool CanUseItem(Player player)
        {

            return true;
        }
    }
}