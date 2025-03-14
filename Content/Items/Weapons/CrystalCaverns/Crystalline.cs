using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Weapons.CrystalCaverns;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class Crystalline : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true; 
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 34; 
            Item.height = 44; 

            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.useTime = 25; 
            Item.useAnimation = 25; 
            Item.noMelee = true;
            Item.noUseGraphic = true; 
            Item.UseSound = SoundID.Item1; 

            Item.damage = 16; 
            Item.DamageType = DamageClass.MeleeNoSpeed; 
            Item.knockBack = 2f; 
            Item.crit = 4; 
            Item.channel = true;
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.buyPrice(silver: 75);

            Item.shoot = ModContent.ProjectileType<CrystallineProjectile>();
            Item.shootSpeed = 16f;		
        }

        public class CrystallineProjectile : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 4f;
                ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 210f;
                ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
            }

            public override void SetDefaults()
            {
                Projectile.width = 22;
                Projectile.height = 22; 
                Projectile.aiStyle = ProjAIStyleID.Yoyo; 
                Projectile.friendly = true;
                Projectile.DamageType = DamageClass.MeleeNoSpeed; 
                Projectile.penetrate = -1;
            }
            public int timer = 0;
            
            public override void PostAI()
            {
                int SpawnX = (int)Projectile.position.X;
                int SpawnY = (int)Projectile.position.Y;
                IEntitySource source = null;
                if (timer == 160 & (NPC.CountNPCS(ModContent.NPCType<CrystallineTumbler>()) <= 3))
                {
                    NPC.NewNPC(source, SpawnX, SpawnY, ModContent.NPCType<CrystallineTumbler>());
                    Projectile.Kill();
                }
                
             timer++;

            }
        }
    }
}