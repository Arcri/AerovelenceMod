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

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged
{
    public class ICER : ModItem
    {
        public int lockOutTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ICER");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Research.WithPitchOffset(0.8f).WithVolumeScale(0.8f);
            Item.crit = 4;
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ICERProj>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 4f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
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