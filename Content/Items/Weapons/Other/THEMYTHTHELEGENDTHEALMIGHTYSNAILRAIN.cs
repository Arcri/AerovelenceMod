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
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = false;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = Mod.Find<ModProjectile>("SnailRainManager").Type;
        }
        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.projectile[i].type == Mod.Find<ModProjectile>("SnailRainManager").Type)
                    return false;
            }
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), position.X, position.Y, 0, 0, Mod.Find<ModProjectile>("SnailRainManager").Type, 0, 1f, player.whoAmI);
        }
    }

    public class SnailRainManager : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 160;
        }

        public override void AI()
        {
            Vector2 playerpos = Projectile.position;
            int randX = new Random().Next(-1200, 1201);
            int right = Projectile.NewProjectile(Projectile.GetSource_FromAI(),playerpos.X + randX, playerpos.Y - 1000, 15, 0, Mod.Find<ModProjectile>("FLYINGSNAIL").Type, 0, 0f, Main.myPlayer, 0f, 0f);
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    var calamity = ModLoader.GetMod("CalamityMod");
                    if (calamity != null)
                    {
                        if (Main.npc[i].type == calamity.Find<ModNPC>("SupremeCalamitas").Type)
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
                Projectile.active = false;
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
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 160;
        }
        public override void AI()
        {
            Projectile.velocity.X = 0;
            Projectile.velocity.Y = 9;
        }
    }
}