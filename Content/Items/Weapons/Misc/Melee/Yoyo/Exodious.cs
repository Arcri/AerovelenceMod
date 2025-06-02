/*
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using AerovelenceMod.Common.Utilities;
using System.Composition.Convention;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;


namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee.Yoyo
{
    public class Exodious : ModItem
    {
        //public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;

            //SLR has these for their yoyos so I will assume that it would be smart to also have it
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.knockBack = KnockbackTiers.Weak;

            Item.DamageType = DamageClass.MeleeNoSpeed;

            Item.width = 30;
            Item.height = 26;        
            Item.useTime = Item.useAnimation = 30;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<ExodiousProjectile>();
            Item.shootSpeed = 15f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.channel = true;

            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.rare = ItemRarityID.Red;
        }

    }
    public class ExodiousProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 30f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 350f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.scale = 0;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;

        }

        int timer = 0;
        public float scale = 0f;
        public float alpha = 0f;

        public override void PostAI()
        {

            timer++;

            base.PostAI();
        }

        Texture2D Proj = null;
        Texture2D Flare = null;
        Texture2D Orb = null;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Proj == null || Flare == null || Orb == null)
            {
                Proj = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Yoyo/ExodiousProjectile");
                Flare = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Yoyo/ExodiousProjectile");
                Orb = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/Yoyo/ExodiousProjectile");

            }

            Main.EntitySpriteDraw(Proj, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Proj.Size() / 2f, 1f, SpriteEffects.None);

            return true;
        }

        public override void PostDraw(Color lightColor)
        {

            base.PostDraw(lightColor);
        }

    }
}
*/ 