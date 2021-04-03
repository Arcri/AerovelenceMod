using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class MoonpiercerProj : ModProjectile
    {
        public bool Strike;
        public int Damage;
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
        }
        public override void AI()
        {
            if (projectile.timeLeft == 900)
            {       
                Damage = projectile.damage;
            }
            if (projectile.owner == Main.myPlayer && !Strike)
            {
                if (!Main.mouseLeft && projectile.damage == 0)
                {
                    Strike = true;
                    projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation();
                    projectile.damage = Damage;
                    float MaxSpeed = 32f;
                    Vector2 VectorCursor = Main.MouseWorld - projectile.Center;
                    float Distance = VectorCursor.Length();
                    if (Distance > MaxSpeed)
                    {
                        Distance = MaxSpeed / Distance;
                        VectorCursor *= Distance;
                    }
                    projectile.velocity = VectorCursor;
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.damage = 0;
                    projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation();
                    List<int> Rotato = new List<int>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].type == projectile.type && !(Main.projectile[i].modProjectile as MoonpiercerProj).Strike)
                        {
                            Rotato.Add(i);
                        }
                    }
                    for (int i2 = 0; i2 < Rotato.Count; i2++)
                    {
                        Main.projectile[Rotato[i2]].Center = Main.player[projectile.owner].Center + new Vector2(0f, -40f - (6f * Rotato.Count)).RotatedBy(MathHelper.ToRadians(360f / Rotato.Count * i2));
                        Main.projectile[Rotato[i2]].netUpdate = true;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}