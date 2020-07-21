using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class LightningStarSpell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            DisplayName.SetDefault("Lightning star spell");
            Tooltip.SetDefault("Fires a burst of shock stars that home on to targets");
        }
        public override void SetDefaults()
        {
            item.crit = 4;
            item.damage = 50;
            item.magic = true;
            item.mana = 7;
            item.width = 28;
            item.height = 30;
            item.useTime = 17;
            item.useAnimation = 17;
            item.UseSound = SoundID.Item21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Green;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LightningSpellProj>();
            item.shootSpeed = 5f;
        }
    }
}

namespace AerovelenceMod.Items.Weapons.Magic
{
    public class LightningSpellProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.aiStyle = -1;
            projectile.width = 18;
            projectile.height = 38;
            projectile.alpha = 0;
            projectile.penetrate = 4;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
        }
        int TimeLeft = 0;
        public override void AI()
        {
            {
                TimeLeft++;
                projectile.ai[1] += 0.1f;
                projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
                float cos = 1 + (float)Math.Cos(projectile.ai[1]);
                float sin = 1 + (float)Math.Sin(projectile.ai[1]);
            }
            {
                if (projectile.alpha > 30)
                {
                    projectile.alpha -= 15;
                    if (projectile.alpha < 30)
                    {
                        projectile.alpha = 30;
                    }
                }
                if (projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref projectile.velocity);
                    projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && !Main.npc[k].immortal)
                    {
                        Vector2 newMove = Main.npc[k].Center - projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                    projectile.rotation += projectile.velocity.X * 0.099f;
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    projectile.velocity = (5 * projectile.velocity + move) / 6f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }
            projectile.rotation += 1f;
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 25f / magnitude;
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.AncientLight);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 2.5f;
            }
        }
    }
}