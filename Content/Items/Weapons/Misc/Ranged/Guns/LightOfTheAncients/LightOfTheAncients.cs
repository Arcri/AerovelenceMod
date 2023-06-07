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
using Terraria.GameContent;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.LightOfTheAncients
{
    public class LightOfTheAncients : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 0;

            Item.width = 46;
            Item.height = 28;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;

            Item.autoReuse = true;
            Item.noMelee = true;

            //Item.shoot = ModContent.ProjectileType<AdamantitePulseShot>();
            Item.shootSpeed = 2f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23, 4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            return true;
        }

    }


}