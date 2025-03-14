
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
using Terraria.Utilities;
using System.Security.Cryptography.X509Certificates;


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
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.channel = true;

            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.rare = ItemRarityID.Pink;
        }

    }
    public class ExodiousProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 20f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 280f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 11f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;

            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 0;
            Projectile.penetrate = -1;

            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;

        }

        int timer = 0;
        public float scale = 0f;
        public float alpha = 0f;
        public float DustX = 0f;
        public float DustY = 0f;
        public float TimerRand = 200f;
 
        public override void PostAI()
        {
             
            if (timer == TimerRand)
            {
                Projectile.position.X = Projectile.position.X + (Main.rand.NextFloat(-200, 200));
                Projectile.position.Y = Projectile.position.Y + (Main.rand.NextFloat(-200, 200));
                for (float m = 0f; m < 5f; m += 0.7f)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 71, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, 0.25f));

                }
                
                float TimerRand = Main.rand.NextFloat(100, 260);
                timer = 0;
       
            }
            
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