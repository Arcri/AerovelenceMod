using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Other
{
    public class THEMYTHTHELEGENDTHEALMIGHTYSNAILRAIN : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snail Mail 2.0");
            Tooltip.SetDefault("Kills all enemies\n" +
                               "Yeets snails into existance\n" +
                               "'Sorry for the late shipping'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item4;
            item.consumable = false;
            item.rare = ItemRarityID.Blue;
            item.shoot = mod.ProjectileType("SnailRainManager");
        }
        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.projectile[i].type == mod.ProjectileType("SnailRainManager"))
                    return false;
            }
            return true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, 0, 0, mod.ProjectileType("SnailRainManager"), 0, knockBack, player.whoAmI);
            return false;
        }
    }

    public class SnailRainManager : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 160;
        }

        public override void AI()
        {
            Vector2 playerpos = projectile.position;
            int randX = new Random().Next(-1200, 1201);
            int right = Projectile.NewProjectile(playerpos.X + randX, playerpos.Y - 1000, 15, 0, mod.ProjectileType("FLYINGSNAIL"), 0, 0f, Main.myPlayer, 0f, 0f);
            if (projectile.timeLeft == 1)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    var calamity = ModLoader.GetMod("CalamityMod");
                    if (calamity != null)
                    {
                        if (Main.npc[i].type == calamity.NPCType("SupremeCalamitas"))
                        {
                            for (int j = 0; j < 6000; j++)
                            {
                                if (j % 5 == 0)
                                {
                                    KillAllProjectiles();
                                }
                                Main.npc[i].AI();
                            }
                            Main.npc[i].life = 1;
                            Main.npc[i].NPCLoot();
                            Main.npc[i].active = false; //thx Fab lmao
                        }
                    }
                    Main.npc[i].life = 1;
                    Main.npc[i].StrikeNPC(2, 0, 1);
                    Main.npc[i].NPCLoot();
                    Main.npc[i].active = false;
                }
                projectile.active = false;
            }
        }
        private void KillAllProjectiles()
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].hostile)
                {
                    Main.projectile[i].active = false;
                }
            }
        }
    }
    public class FLYINGSNAIL : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 160;
        }
        public override void AI()
        {
            projectile.velocity.X = 0;
            projectile.velocity.Y = 9;
        }
    }
}